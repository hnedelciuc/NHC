/********************************************************************************************************************
/ Needle in a Haystack in a Crypt v1.0.
/ Copyright (C) 2016-2020 by Horia Nedelciuc from Chisinau, Moldova.
/********************************************************************************************************************
/ Main Service.
/ Compression using the following algorithms:
/ -- Deflate (.NET library)
/ -- LZMA (Igor Pavlov's library)
/ Encryption / Decryption using the following algorithms:
/ -- Symmetric Algorithms (.NET library): RC2, TripleDES, AES with Key or Password
/ -- NeedleCrypt proprietary algorithm developed by Horia Nedelciuc with Key and/or Password.
/********************************************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

class MainService
{
    /********************************************************************************************************************/

    // Defining buffer block size which will be used for reading/writing data from/to files.
    // Can be any multiple of 16; must be less than (2^16 - 1) for zero padding encryption and less than
    // (2^16 - IV.Length * 2) for encryption that uses padding.

    internal const int blockSize = 65504;

    /********************************************************************************************************************/

    // File Attributes "byte":
    // ------------------------------------------------------------------------------------------------------------------------------
    // |  Setting  |  isDirectory  |  isHidden  |  isReadOnly  |  isSystem  |  isArchive  |  isTemporary  |  isSparse  |  isNormal  |
    // ------------------------------------------------------------------------------------------------------------------------------
    // |   Index   |       0       |     1      |      2       |     3      |      4      |       5       |     6      |      7     |
    // ------------------------------------------------------------------------------------------------------------------------------
    //
    // NHC Settings "byte":
    // ------------------------------------------------------------------------------------------------------------------------------------
    // |  Setting  |  isCompressed  |  isLZMA  |  isKeyBased  |  isRC2  |  isTripleDES  |  isAES  |  isHeaderCompressed  | isSplitArchive |
    // ------------------------------------------------------------------------------------------------------------------------------------
    // |   Index   |       0        |    1     |      2       |    3    |       4       |    5    |          6           |       7        |
    // ------------------------------------------------------------------------------------------------------------------------------------
    //
    // If "isLZMA" == false, then it is Deflate (CompressionLevel.Optimal)
    // If "isKeyBased" == true and "isRC2", "isTripleDES", "isAES" are all false, then it is NeedleCrypt with Key

    internal static bool[] file_attributes = new bool[8];
    internal static bool[] nhc_settings = new bool[8];

    /********************************************************************************************************************/

    internal static ArrayList lzmaCompressionExtensionList = new ArrayList()
    {
        "pdf", "doc", "docx", "docm", "dot", "dotm", "rtf", "odt", "txt", "log", "ini", "cfg", "xml", "json", "html", "htm",
        "xls", "xlsx", "xlsm", "xlsb", "xlam", "xlt", "xltx", "xltm", "xla", "xlm", "xlw", "odc", "ods", "csv",
        "js", "ts", "jsx", "css", "scss", "less", "sln", "cs", "cshtml", "csproj", "asax", "resx", "config", "pubxml", "user",
        "map", "nuspec", "xaml", "vb", "vbs", "c", "h", "cpp",  "gitattributes", "gitignore", "npmignore", "yaml", "bowerrc",
        "java", "jav", "jsp", "jst", "py", "pyt", "pyw", "pth", "bat", "cmd", "ps1", "rb", "rhtml", "gemspec", "pp", "pas"
    };
    internal static int lzmaCompressionFileSizeLimit = 3000000;

    /********************************************************************************************************************/

    enum BlockCompressionType { isNotCompressed, isCompressedDeflate, isCompressedLZMA, isNotDefinedYet };

    // Compresses and encrypts given files using one of the available .NET symmetric algorithms or the NeedleCrypt algorithm and adds them to given .NHC archive file.

    internal static void CompressEncryptFiles(
        ArrayList items,
        string archiveFileName,
        HelperService.CryptionAlgorithm cryptionAlgorithm,
        CompressionLevel compressionLevel,
        string passPhrase,
        byte[] key,
        byte[] keyArray,
        byte[] IV,
        long splitArchiveSize,
        Crypt.ProgressDelegate progress,
        Crypt.CurrentFileProcessedDelegate currentFileProcessed,
        Crypt.NumberOfEntriesProcessedDelegate numberOfEntriesProcessed,
        Crypt.CurrentDateTimeDelegate currentDateTime
    )
    {
        bool isDirectory, isHidden, isReadOnly, isSystem, isArchive, isTemporary, isSparse, isNormal;
        bool isCompressed, isLZMA, isKeyBased, isRC2, isTripleDES, isAES, isHeaderCompressed, isSplitArchive = splitArchiveSize > 0;

        CryptionService.CheckArguments(key, IV, cryptionAlgorithm);

        if (cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptKey &&
            cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptPassword)
            key = keyArray;

        Tuple<byte[], byte[]> needleCryptParams =
            CryptionService.PrepareNeedleCrypt(CryptionService.saltVal, passPhrase, key);

        byte[] keyHash = needleCryptParams.Item1;
        byte[] combinedHash = needleCryptParams.Item2;

        byte[] header_settings;
        byte[] header_compressed_file_size;
        byte[] header_compressed_and_encrypted_header_block_size;
        byte[] header_file_size;
        byte[] header_file_name;
        byte[] header_block;

        var numberOfSteps = items.Count;
        var nrOfEntriesProcessed = 0;
        var splitArchiveNumberOfBlocks = isSplitArchive ? splitArchiveSize / blockSize : 0;
        var splitBlockCounter = 0;
        var splitCounter = 0;
        var splitPos = 0L;

        string inputFileName, savedFileName;

        byte[] nextIV = (byte[])IV.Clone();

        int stepNumber = 0;

        var archiveExtension = Path.GetExtension(archiveFileName);
        var archiveNameWithoutExtension = Path.Combine(Path.GetDirectoryName(archiveFileName), Path.GetFileNameWithoutExtension(archiveFileName));

        while (stepNumber < numberOfSteps)
        {
            dynamic item = items[stepNumber];

            inputFileName = item.fullPath;
            savedFileName = item.relativePath;
            string extension = Path.GetExtension(savedFileName);
            if (!string.IsNullOrEmpty(extension)) { extension = extension.Substring(1); }
            long inputFileLength = 0;

            using (var outputHandle = LongFile.CreateFileForAppend(LongFile.GetWin32LongPath((splitCounter == 0) ? archiveFileName : $"{archiveNameWithoutExtension}.part{splitCounter}{archiveExtension}")))
            using (var fileStreamOutput = new FileStream(outputHandle, FileAccess.ReadWrite))
            {
                try
                {
                    // file_attributes
                    isDirectory = item.isDirectory;

                    if (splitBlockCounter == 0 && !isDirectory)
                    {
                        fileStreamOutput.SetLength(0);
                    }

                    var isProcessed = !isDirectory || isDirectory && (LongDirectory.Exists(item.fullPath) ? LongDirectory.IsDirectoryEmpty(item.fullPath) : false);

                    if (!isProcessed)
                    {
                        stepNumber++;

                        // Report progress level to UI.
                        long currentProgress = stepNumber * 100 / numberOfSteps;
                        progress((int)currentProgress);

                        // Report current time to UI.
                        currentDateTime(DateTime.Now);

                        continue;
                    }

                    var attr = LongFile.GetAttributes(inputFileName);

                    isHidden = (attr & FileAttributes.Hidden) == FileAttributes.Hidden;
                    isReadOnly = (attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
                    isSystem = (attr & FileAttributes.System) == FileAttributes.System;
                    isArchive = (attr & FileAttributes.Archive) == FileAttributes.Archive;
                    isTemporary = (attr & FileAttributes.Temporary) == FileAttributes.Temporary;
                    isSparse = (attr & FileAttributes.SparseFile) == FileAttributes.SparseFile;
                    isNormal = attr == FileAttributes.Normal;

                    if (!isDirectory)
                        using (var handleInput = LongFile.GetFileHandle(LongFile.GetWin32LongPath(inputFileName)))
                        using (var inputFileStream = new FileStream(handleInput, FileAccess.Read))
                        {
                            inputFileLength = inputFileStream.Length;
                        }

                    // nhc_settings
                    isCompressed = compressionLevel != CompressionLevel.NoCompression;
                    isLZMA = (compressionLevel == CompressionLevel.Optimal) &&
                             (HelperService.forceLZMA || (lzmaCompressionExtensionList.Contains(extension) &&
                             (inputFileLength < lzmaCompressionFileSizeLimit)));
                    isKeyBased = (cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptKey) ||
                                 (cryptionAlgorithm == HelperService.CryptionAlgorithm.RC2Key) ||
                                 (cryptionAlgorithm == HelperService.CryptionAlgorithm.TripleDesKey) ||
                                 (cryptionAlgorithm == HelperService.CryptionAlgorithm.AesKey);
                    isRC2 = (cryptionAlgorithm == HelperService.CryptionAlgorithm.RC2Key) ||
                            (cryptionAlgorithm == HelperService.CryptionAlgorithm.RC2Password);
                    isTripleDES = (cryptionAlgorithm == HelperService.CryptionAlgorithm.TripleDesKey) ||
                                  (cryptionAlgorithm == HelperService.CryptionAlgorithm.TripleDesPassword);
                    isAES = (cryptionAlgorithm == HelperService.CryptionAlgorithm.AesKey) ||
                            (cryptionAlgorithm == HelperService.CryptionAlgorithm.AesPassword);
                    isHeaderCompressed = false; // this value will be updated later
                    isSplitArchive = splitArchiveSize > 0;

                    file_attributes = new bool[8] { isDirectory, isHidden, isReadOnly, isSystem, isArchive, isTemporary, isSparse, isNormal };
                    nhc_settings = new bool[8] { isCompressed, isLZMA, isKeyBased, isRC2, isTripleDES, isAES, isHeaderCompressed, isSplitArchive };

                    currentFileProcessed("Processing entry " + Path.GetFileName(inputFileName) + "...");

                    // full_header for file entry = header_settings + header_compressed_file_size + header_compressed_header_block_size + header_block.
                    // full_header for directory entry = header_settings + header_compressed_header_block_size + header_filename.
                    // header_block for file entry = header_file_size + header_file_name.
                    // header_block for directory entry = header_filename.

                    header_compressed_file_size = new byte[8];
                    header_compressed_and_encrypted_header_block_size = new byte[2];
                    header_file_size = new byte[8];
                    header_file_name = Encoding.UTF8.GetBytes(savedFileName);
                    header_block = isDirectory
                        ? header_file_name
                        : new byte[header_file_size.Length + header_file_name.Length];

                    // The header_compressedfilesize is already created empty.
                    // The size of "header_compressedfilesize" will be added later, after file will be processed.

                    long initialFileSize = fileStreamOutput.Length;
                    long afterHeaderFileSize;
                    byte fourBlocksCompressionTypeByte = 0;

                    // Report progress level = 0 if this is the first entry (i.e. file) added to the archive.
                    if (stepNumber == 0)
                    {
                        progress(0);
                    }

                    if (!isDirectory) // i.e. if isFile
                    {
                        byte[] encryptedBlock = null;

                        using (var inputHandle = LongFile.GetFileHandle(LongFile.GetWin32LongPath(inputFileName)))
                        using (var fileStreamInput = new FileStream(inputHandle, FileAccess.Read))
                        {
                            long f_length = fileStreamInput.Length;

                            // Creating header_file_size.
                            header_file_size = BitConverter.GetBytes(f_length);

                            // Compiling together the header_block.
                            header_block = HelperService.CombineByteArrays(header_file_size, header_file_name);

                            // Compressing header_block.
                            byte[] compressed_header_block = CompressionService.DeflateCompressHeader(header_block);

                            if (compressed_header_block.Length >= header_block.Length)
                            {
                                compressed_header_block = header_block;
                                nhc_settings[6] = false;
                            }
                            else nhc_settings[6] = true;

                            header_settings = new byte[2]
                            {
                                CryptionService.NeedleEncryptArray(
                                    new byte[] { HelperService.ConvertBoolArrayToByte(file_attributes) }, keyArray, nextIV,
                                    keyHash, combinedHash)[0],
                                HelperService.ConvertBoolArrayToByte(nhc_settings)
                            };

                            // Encrypting compressed_header_block.
                            byte[] encrypted_header_block = CryptionService.NeedleEncryptArray(compressed_header_block,
                                keyArray, nextIV, keyHash, combinedHash);

                            // Calculating header_compressed_header_block_size.
                            header_compressed_and_encrypted_header_block_size = CryptionService.NeedleEncryptArray(
                                BitConverter.GetBytes((UInt16)encrypted_header_block.Length), keyArray, nextIV, keyHash,
                                combinedHash);

                            // Setting current input index position.
                            long pos = splitCounter > 0 ? splitPos : 0;
                            fileStreamInput.Position = pos;

                            // Setting current output index position.
                            fileStreamOutput.Position = initialFileSize;

                            // Writing header of current entry (i.e. file) to archive file.
                            // Note: Full header = header_settings + header_compressedfilesize (which at this point is still null) + header_compressed_and_encrypted_header_block_size + encrypted_header_block (header_filesize + header_filename)
                            fileStreamOutput.Write(header_settings, 0, header_settings.Length);
                            fileStreamOutput.Write(header_compressed_file_size, 0, header_compressed_file_size.Length);
                            fileStreamOutput.Write(header_compressed_and_encrypted_header_block_size, 0, header_compressed_and_encrypted_header_block_size.Length);
                            fileStreamOutput.Write(encrypted_header_block, 0, encrypted_header_block.Length);

                            // lastBytes ideally should be updated here, but since header_compressed_file_size is unknown to us at this point (it is all zeros),
                            // we will deal with it later (look for it somewhere below)

                            // Compressing and encrypting data.
                            afterHeaderFileSize = fileStreamOutput.Length;

                            byte[] compressedBlock, compressedBlock2, compressedBlock3;
                            BlockCompressionType blockCompressionType = BlockCompressionType.isNotCompressed;

                            if (f_length > 0)
                            {
                                dynamic symmetricAlgorithm = null;

                                switch (cryptionAlgorithm)
                                {
                                    case HelperService.CryptionAlgorithm.RC2Key:
                                    case HelperService.CryptionAlgorithm.RC2Password:
                                        symmetricAlgorithm = new RC2CryptoServiceProvider();
                                        break;
                                    case HelperService.CryptionAlgorithm.TripleDesKey:
                                    case HelperService.CryptionAlgorithm.TripleDesPassword:
                                        symmetricAlgorithm = new TripleDESCryptoServiceProvider();
                                        break;
                                    case HelperService.CryptionAlgorithm.AesKey:
                                    case HelperService.CryptionAlgorithm.AesPassword:
                                        symmetricAlgorithm = new AesCryptoServiceProvider();
                                        break;
                                }

                                // Encryption mode set to Cipher Block Chaining (CBC).
                                // Use default options for other symmetric key parameters.
                                if (symmetricAlgorithm != null)
                                {
                                    symmetricAlgorithm.Mode = CipherMode.CBC;
                                    symmetricAlgorithm.Padding = PaddingMode.PKCS7;
                                    symmetricAlgorithm.Key = key;
                                }

                                ICryptoTransform encryptor = null;

                                var fourBlocksCounter = 0;
                                var isBlockCompressedArr = new bool[8] { false, false, false, false, false, false, false, false };

                                try
                                {
                                    // Processing most data between beginning of file and almost end of file which fitted into buffer blocks of size = "blockSize".
                                    byte[] block = new byte[blockSize];
                                    bool isLastBlock = false;
                                    bool isLastSplitBlock = false;

                                    while (pos < f_length)
                                    {
                                        if (HelperService.backgroundWorkerClosePending)
                                            break;

                                        splitBlockCounter++;
                                        fourBlocksCounter++;

                                        if (fourBlocksCounter > 4)
                                        {
                                            fourBlocksCounter = 1;
                                            isBlockCompressedArr = new bool[8] { false, false, false, false, false, false, false, false };
                                        }

                                        if (cryptionAlgorithm != HelperService.CryptionAlgorithm.NeedleCryptKey &&
                                            cryptionAlgorithm != HelperService.CryptionAlgorithm.NeedleCryptPassword)
                                        {
                                            symmetricAlgorithm.IV = nextIV;
                                            encryptor = symmetricAlgorithm.CreateEncryptor();
                                        }

                                        isLastBlock = fileStreamInput.Length - fileStreamInput.Position <= blockSize;

                                        if (isLastBlock || isSplitArchive && isLastSplitBlock)
                                        {
                                            block = new byte[f_length - pos];
                                        }

                                        // Read original content from input file.
                                        fileStreamInput.Read(block, 0, block.Length);

                                        if (block == null || block.Length == 0)
                                        {
                                            throw new ArgumentNullException("block");
                                        }

                                        // Compress content and save it to buffer array.
                                        if (isCompressed)
                                        {
                                            if (compressionLevel == CompressionLevel.Optimal && HelperService.forceMAX)
                                            {
                                                compressedBlock = CompressionService.DeflateCompressArray(block,
                                                        CompressionLevel.Optimal);
                                                compressedBlock2 = CompressionService.DeflateCompressArray(block,
                                                        CompressionLevel.Fastest);
                                                compressedBlock3 = CompressionService.LZMACompressArray(block);

                                                var arr = new int[] { block.Length, compressedBlock.Length, compressedBlock2.Length, compressedBlock3.Length };
                                                int cbIndex = Array.IndexOf(arr, System.Linq.Enumerable.Min(arr));

                                                switch (cbIndex)
                                                {
                                                    case 0: compressedBlock = block; break;
                                                    case 1: break;
                                                    case 2: compressedBlock = compressedBlock2; break;
                                                    case 3: compressedBlock = compressedBlock3; break;
                                                }

                                                blockCompressionType = cbIndex == 0
                                                    ? BlockCompressionType.isNotCompressed
                                                        : ((cbIndex == 1) || (cbIndex == 2))
                                                        ? BlockCompressionType.isCompressedDeflate
                                                            : BlockCompressionType.isCompressedLZMA;
                                            }
                                            else
                                            {
                                                if (isLZMA)
                                                {
                                                    compressedBlock = CompressionService.LZMACompressArray(block);

                                                    blockCompressionType = BlockCompressionType.isCompressedLZMA;
                                                }
                                                else
                                                {
                                                    compressedBlock =
                                                        CompressionService.DeflateCompressArray(block,
                                                            CompressionLevel.Optimal);

                                                    blockCompressionType = BlockCompressionType.isCompressedDeflate;
                                                }

                                                if (compressedBlock.Length >= block.Length)
                                                {
                                                    compressedBlock = block;

                                                    blockCompressionType = BlockCompressionType.isNotCompressed;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            compressedBlock = block;

                                            blockCompressionType = BlockCompressionType.isNotCompressed;
                                        }

                                        // Encrypt compressed content.
                                        if (cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptKey ||
                                            cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptPassword)
                                            encryptedBlock = CryptionService.NeedleEncryptArray(compressedBlock, key,
                                                nextIV, keyHash, combinedHash);
                                        else
                                            encryptedBlock = encryptor.TransformFinalBlock(compressedBlock, 0,
                                                compressedBlock.Length);

                                        isLastSplitBlock = fileStreamOutput.Length + encryptedBlock.Length > splitArchiveSize;

                                        if (isSplitArchive && isLastSplitBlock) // extra iteration condition when archive is split
                                        {
                                            splitBlockCounter--;
                                            fileStreamInput.Position = pos;
                                            fourBlocksCounter = fourBlocksCounter == 1 ? 4 : fourBlocksCounter - 1;

                                            if (isCompressed && fourBlocksCounter != 4)
                                            {
                                                fourBlocksCompressionTypeByte = HelperService.ConvertBoolArrayToByte(isBlockCompressedArr);
                                                // Write blockCompressionType which may vary from block to block depending on settings and compressed block size.
                                                fileStreamOutput.Write(BitConverter.GetBytes(fourBlocksCompressionTypeByte), 0, 1);

                                                nextIV = HelperService.CombineByteArrays(
                                                    HelperService.GetByteArraySegment(nextIV, 1),
                                                    new byte[1] { fourBlocksCompressionTypeByte });
                                            }

                                            goto nextSplit;
                                        }
                                        else
                                        {
                                            var isEncryptedWithPadding = isAES || isTripleDES || isRC2;
                                            
                                            if (isCompressed || isEncryptedWithPadding)
                                            {
                                                // Write the size of the compressed buffer.
                                                fileStreamOutput.Write(BitConverter.GetBytes((UInt16)encryptedBlock.Length), 0, 2);
                                            }

                                            // Write encrypted and compressed content to archive file.
                                            fileStreamOutput.Write(encryptedBlock, 0, encryptedBlock.Length);
                                        }

                                        if (isCompressed)
                                        {
                                            switch (blockCompressionType)
                                            {
                                                case BlockCompressionType.isNotCompressed:
                                                    isBlockCompressedArr[fourBlocksCounter * 2 - 2] = false;
                                                    isBlockCompressedArr[fourBlocksCounter * 2 - 1] = false;
                                                    break;
                                                case BlockCompressionType.isCompressedDeflate:
                                                    isBlockCompressedArr[fourBlocksCounter * 2 - 2] = true;
                                                    isBlockCompressedArr[fourBlocksCounter * 2 - 1] = false;
                                                    break;
                                                case BlockCompressionType.isCompressedLZMA:
                                                    isBlockCompressedArr[fourBlocksCounter * 2 - 2] = false;
                                                    isBlockCompressedArr[fourBlocksCounter * 2 - 1] = true;
                                                    break;
                                                case BlockCompressionType.isNotDefinedYet:
                                                    isBlockCompressedArr[fourBlocksCounter * 2 - 2] = true;
                                                    isBlockCompressedArr[fourBlocksCounter * 2 - 1] = true;
                                                    break;
                                            }

                                            if ((fourBlocksCounter == 4) || isLastBlock)
                                            {
                                                fourBlocksCompressionTypeByte = HelperService.ConvertBoolArrayToByte(isBlockCompressedArr);
                                                // Write blockCompressionType which may vary from block to block depending on settings and compressed block size.
                                                fileStreamOutput.Write(BitConverter.GetBytes(fourBlocksCompressionTypeByte), 0, 1);
                                            }
                                        }

                                        if (!isLastBlock) // is fourBlock
                                        {
                                            // Recalculate index position.
                                            pos += blockSize;

                                            nextIV = (!isCompressed || (fourBlocksCounter < 4))
                                            ? HelperService.GetByteArraySegment(
                                                encryptedBlock,
                                                encryptedBlock.Length - IV.Length,
                                                IV.Length)
                                            : HelperService.CombineByteArrays(
                                                HelperService.GetByteArraySegment(
                                                    encryptedBlock,
                                                    encryptedBlock.Length - IV.Length + 1,
                                                    IV.Length - 1),
                                                new byte[1] { fourBlocksCompressionTypeByte });

                                            // Report progress level to UI.
                                            long currentProgress =
                                                (stepNumber * 100 / numberOfSteps + (pos * 100 / f_length / numberOfSteps));
                                            progress((int)currentProgress);

                                            // Report current time to UI.
                                            currentDateTime(DateTime.Now);
                                        }
                                        else // if (isLastBlock)
                                        {
                                            // Recalculate index position.
                                            pos = fileStreamInput.Position;

                                            splitPos = 0;

                                            // Don't delete these three lines below. They are not redunant. Resetting counter for the next file in queue.
                                            fourBlocksCounter = 0;
                                            isBlockCompressedArr = new bool[8] { false, false, false, false, false, false, false, false };

                                            if (encryptedBlock.Length >= IV.Length)
                                            {
                                                nextIV = isCompressed
                                                    ? HelperService.CombineByteArrays(
                                                        HelperService.GetByteArraySegment(
                                                            encryptedBlock,
                                                            encryptedBlock.Length - IV.Length + 1,
                                                            IV.Length - 1),
                                                        new byte[1] { fourBlocksCompressionTypeByte })
                                                    : HelperService.GetByteArraySegment(
                                                        encryptedBlock,
                                                        encryptedBlock.Length - IV.Length,
                                                        IV.Length);
                                            }
                                            else
                                            {
                                                long currFileSize = fileStreamOutput.Length;
                                                long strmSize = currFileSize - afterHeaderFileSize;

                                                if (strmSize >= IV.Length)
                                                {
                                                    nextIV = isCompressed
                                                        ? HelperService.CombineByteArrays(nextIV, BitConverter.GetBytes((UInt16)encryptedBlock.Length), encryptedBlock, new byte[1] { fourBlocksCompressionTypeByte })
                                                        : HelperService.CombineByteArrays(nextIV, encryptedBlock);
                                                }
                                                else
                                                {
                                                    nextIV = isCompressed
                                                        ? HelperService.CombineByteArrays(header_compressed_and_encrypted_header_block_size, encrypted_header_block, BitConverter.GetBytes((UInt16)encryptedBlock.Length), encryptedBlock, new byte[1] { fourBlocksCompressionTypeByte })
                                                        : HelperService.CombineByteArrays(header_compressed_and_encrypted_header_block_size, encrypted_header_block, encryptedBlock);

                                                    if (nextIV.Length < IV.Length)
                                                    {
                                                        // Encrypt header_compressedfilesize.
                                                        // NOTE: Here we must pass IV, not nextIV!
                                                        header_compressed_file_size = CryptionService.NeedleEncryptArray(
                                                            BitConverter.GetBytes(strmSize),
                                                            keyArray,
                                                            IV,
                                                            keyHash,
                                                            combinedHash);
                                                        nextIV = HelperService.CombineByteArrays(header_settings, header_compressed_file_size, nextIV);

                                                        if (nextIV.Length < IV.Length)
                                                        {
                                                            nextIV = HelperService.CombineByteArrays(IV, nextIV);
                                                        }
                                                    }
                                                }

                                                nextIV = HelperService.GetByteArraySegment(nextIV, nextIV.Length - IV.Length, IV.Length);
                                            }

                                            // Report progress level to UI.
                                            long currentProgress = (stepNumber + 1) * 100 / numberOfSteps;
                                            progress((int)currentProgress);

                                            // Report current time to UI.
                                            currentDateTime(DateTime.Now);
                                        }
                                    }
                                }
                                catch (Exception excpt)
                                {
                                    throw excpt;
                                }
                                finally
                                {
                                    if (symmetricAlgorithm != null)
                                    {
                                        symmetricAlgorithm.Clear();
                                        encryptor.Dispose();
                                    }
                                }
                            }
                            else // if f_length == 0...
                            {
                                nextIV = HelperService.CombineByteArrays(header_settings, header_compressed_file_size, header_compressed_and_encrypted_header_block_size, encrypted_header_block);
                                if (nextIV.Length < IV.Length)
                                {
                                    nextIV = HelperService.CombineByteArrays(IV, nextIV);
                                }
                                nextIV = HelperService.GetByteArraySegment(nextIV, nextIV.Length - IV.Length, IV.Length);

                                splitPos = 0;
                                splitBlockCounter++;

                                // Report progress level to UI.
                                long currentProgress = ((stepNumber + 1) * 100 / numberOfSteps);
                                progress((int)currentProgress);

                                // Report current time to UI.
                                currentDateTime(DateTime.Now);
                            }

                        nextSplit:
                            if (isSplitArchive && (fileStreamOutput.Length + (encryptedBlock?.Length ?? 0) > splitArchiveSize))
                            {
                                splitPos = fileStreamInput.Position != fileStreamInput.Length ? fileStreamInput.Position : 0;
                                splitBlockCounter = 0;
                                splitCounter++;

                                if (fileStreamInput.Position == 0)
                                {
                                    fileStreamOutput.SetLength(initialFileSize);
                                    stepNumber--;
                                }
                            }
                        }

                        long currentFileSize = fileStreamOutput.Length;
                        long streamSize = currentFileSize - afterHeaderFileSize;
                        // Encrypt header_compressedfilesize.
                        // NOTE: Here we must pass IV, not nextIV!
                        if (streamSize >= 0)
                        {
                            header_compressed_file_size = CryptionService.NeedleEncryptArray(
                                BitConverter.GetBytes(streamSize),
                                keyArray,
                                IV,
                                keyHash,
                                combinedHash);

                            // Find the position of header_compressedfilesize.
                            fileStreamOutput.Position = initialFileSize + 2;

                            fileStreamOutput.Write(header_compressed_file_size, 0, header_compressed_file_size.Length);
                        }

                        header_compressed_file_size = null;
                    }
                    else // if isDirectory
                    {
                        // Compressing header_block.
                        byte[] compressed_header_block = CompressionService.DeflateCompressHeader(header_block);

                        if (compressed_header_block.Length >= header_block.Length)
                        {
                            compressed_header_block = header_block;
                            nhc_settings[6] = false; // isCompressedHeader
                        }
                        else nhc_settings[6] = true;

                        header_settings = new byte[2]
                        {
                            CryptionService.NeedleEncryptArray(
                                new byte[] { HelperService.ConvertBoolArrayToByte(file_attributes) }, keyArray, nextIV,
                                keyHash, combinedHash)[0],
                            HelperService.ConvertBoolArrayToByte(nhc_settings)
                        };

                        // Encrypting compressed_header_block.
                        byte[] encrypted_header_block = CryptionService.NeedleEncryptArray(compressed_header_block,
                            keyArray, nextIV, keyHash, combinedHash);

                        // Calculating header_compressed_header_block_size.
                        header_compressed_and_encrypted_header_block_size = CryptionService.NeedleEncryptArray(
                            BitConverter.GetBytes((UInt16)encrypted_header_block.Length), keyArray, nextIV, keyHash,
                            combinedHash);

                        // Setting current output index position.
                        fileStreamOutput.Position = initialFileSize;

                        // Writing header of current entry (i.e. file) to archive file.
                        fileStreamOutput.Write(header_settings, 0, header_settings.Length);
                        fileStreamOutput.Write(header_compressed_and_encrypted_header_block_size, 0,
                            header_compressed_and_encrypted_header_block_size.Length);
                        fileStreamOutput.Write(encrypted_header_block, 0, encrypted_header_block.Length);

                        nextIV = HelperService.CombineByteArrays(nextIV, header_settings, header_compressed_and_encrypted_header_block_size, encrypted_header_block);
                        nextIV = HelperService.GetByteArraySegment(
                            nextIV,
                            nextIV.Length - IV.Length,
                            IV.Length
                        );
                        
                        afterHeaderFileSize = fileStreamOutput.Length;

                        splitPos = 0;
                        splitBlockCounter++;

                        long currentProgress = ((stepNumber + 1) * 100 / numberOfSteps);
                        progress((int)currentProgress);

                        // Report current time to UI.
                        currentDateTime(DateTime.Now);
                    }

                    if (HelperService.backgroundWorkerClosePending)
                        break;

                    numberOfEntriesProcessed(++nrOfEntriesProcessed);

                }
                catch (Exception excptn)
                {
                    throw excptn;
                }
            }

            if (splitPos == 0)
            {
                stepNumber++;
            }
        }

        // The code below fixes a very rare edge case when last split file has been completely filled
        // and the stepNumber is almost equal to numberOfSteps. The code creates the next split file
        // only to find out that the last stepNumber(s) are directories whose path(s) is (/are) implied
        // in previous files already added to the archive and thus this(these) directory(ies) should not
        // be added to the next empty split file (because their path is implied). This leaves the last
        // split file empty (0 bytes) which is not only redundant, but also causes error when trying to
        // open the split archive. The code below deletes the empty/redundant split file if needed.
        if (stepNumber == numberOfSteps && isSplitArchive && splitCounter > 0)
        {
            var isLastSplitNullLength = false;

            using (var handleInput = LongFile.GetFileHandle(LongFile.GetWin32LongPath($"{archiveNameWithoutExtension}.part{splitCounter}{archiveExtension}")))
            using (var lastSplitFileStream = new FileStream(handleInput, FileAccess.Read))
            {
                isLastSplitNullLength = lastSplitFileStream.Length == 0;
            }

            if (isLastSplitNullLength)
            {
                LongFile.Delete($"{archiveNameWithoutExtension}.part{splitCounter}{archiveExtension}");
            }
        }
    }

    /********************************************************************************************************************/

    // Decompresses and decrypts given .NHC archive which was encrypted with one of .NET's symmetric algorithms with a cryption key.

    internal static void DecompressDecryptArchive(
        string outputDirectoryName,
        string archiveFileName,
        ArrayList itemsToBeExtracted,
        bool extractAll,
        byte[] key,
        byte[] keyArray,
        byte[] IV,
        string passPhrase,
        HelperService.CryptionAlgorithm cryptionAlgorithm,
        HelperService.OverwriteFilesSetting overwriteFilesSetting,
        Crypt.ProgressDelegate progress,
        Crypt.CurrentFileProcessedDelegate currentFileProcessed,
        Crypt.NumberOfEntriesProcessedDelegate numberOfEntriesProcessed,
        int numberOfSteps,
        int stepNumber,
        Crypt.CurrentDateTimeDelegate currentDateTime
        )
    {
        if (itemsToBeExtracted.Count == 0 && !extractAll)
        {
            return;
        }

        switch (cryptionAlgorithm)
        {
            case HelperService.CryptionAlgorithm.AesKey:
            case HelperService.CryptionAlgorithm.RC2Key:
            case HelperService.CryptionAlgorithm.TripleDesKey:
            case HelperService.CryptionAlgorithm.NeedleCryptKey:
                passPhrase = null;
                break;
        }

        Tuple<byte[], byte[]> needleCryptParams = CryptionService.PrepareNeedleCrypt(CryptionService.saltVal, passPhrase, key);

        bool isSplitArchive = false;
        long globalpos = 0; // Input file(s) global position index.
        long splitGlobalPos = 0; // position in globalpos where last split occurred
        long splitArchivePos = 0; // Current input file (either monolithic archive or split archive) segment position
        byte[] keyHash = needleCryptParams.Item1;
        byte[] combinedHash = needleCryptParams.Item2;
        byte[] nextIV = (byte[])IV.Clone();
        int nrOfEntriesProcessed = 0;
        int splitArchiveCount = 0;
        var archiveExtension = Path.GetExtension(archiveFileName);
        var archiveNameWithoutExtension = Path.Combine(Path.GetDirectoryName(archiveFileName), Path.GetFileNameWithoutExtension(archiveFileName));
        string archFileName = (string)archiveFileName.Clone();
        var processedFilesNames = new List<string>();
        int confirmation = 0;
        long files_length = HelperService.compressedFilesSize + HelperService.compressedHeadersSize;


        while (!isSplitArchive || isSplitArchive && LongFile.Exists(LongFile.GetWin32LongPath(archFileName)))
        {
            bool isDirectory, isHidden, isReadOnly, isSystem, isArchive, isTemporary, isSparse, isNormal;
            bool isCompressed, isLZMA, isKeyBased, isRC2, isTripleDES, isAES, isCompressedHeader;
            long compressedfilesize = 0, filesize, f_length;
            string filename, output_filename;
            bool decompressCurrentFile = false;
            int nrOfCurrentEntriesProcessed = 0;
            bool isProcessingSplit = false;

            using (var inputFileHandle = LongFile.GetFileHandle(LongFile.GetWin32LongPath(archFileName)))
            using (var fileStreamInput = new FileStream(inputFileHandle, FileAccess.Read))
            {
                f_length = fileStreamInput.Length;

                byte[] header_settings;
                byte[] header_compressed_file_size;
                byte[] header_compressed_and_encrypted_header_block_size;
                byte[] header_file_size;
                byte[] header_file_name;
                byte[] header_block;
                UInt16 headerCompressedAndEncryptedHeaderBlockSize;
                int fullHeaderSize = 0;

                // Checks whether arguments that will be passed for decryption are valid.
                CryptionService.CheckArguments(key, IV, cryptionAlgorithm);

                // Start iterating through all entries (i.e. files or directories) in the .NHC archive and extracting them.
                do
                {
                    header_compressed_file_size = new byte[8];
                    header_settings = new byte[2];
                    header_compressed_and_encrypted_header_block_size = new byte[2];

                    try
                    {
                        fileStreamInput.Position = isSplitArchive ? splitArchivePos : globalpos;
                        if (isSplitArchive && (globalpos - splitGlobalPos != splitArchivePos))
                            throw new Exception("globalpos - splitGlobalPos != splitArchivePos");

                        // Reading encrypted header_isdirectory.
                        fileStreamInput.Read(header_settings, 0, header_settings.Length);

                        file_attributes = HelperService.ConvertByteToBoolArray(CryptionService.NeedleDecryptArray(header_settings, keyArray, nextIV, keyHash, combinedHash)[0]);
                        nhc_settings = HelperService.ConvertByteToBoolArray(header_settings[1]);

                        // Retrieving file_attributes.
                        isDirectory = file_attributes[0];
                        isHidden = file_attributes[1];
                        isReadOnly = file_attributes[2];
                        isSystem = file_attributes[3];
                        isArchive = file_attributes[4];
                        isTemporary = file_attributes[5];
                        isSparse = file_attributes[6];
                        isNormal = file_attributes[7];

                        // Retrieving nhc_settings.
                        isCompressed = nhc_settings[0];
                        isLZMA = nhc_settings[1];
                        isKeyBased = nhc_settings[2];
                        isRC2 = nhc_settings[3];
                        isTripleDES = nhc_settings[4];
                        isAES = nhc_settings[5];
                        isCompressedHeader = nhc_settings[6];
                        isSplitArchive = nhc_settings[7];

                        if (isKeyBased && isRC2 && !isTripleDES && !isAES)
                            HelperService.cryptionAlgorithm = HelperService.CryptionAlgorithm.RC2Key;
                        else if (isKeyBased && !isRC2 && isTripleDES && !isAES)
                            HelperService.cryptionAlgorithm = HelperService.CryptionAlgorithm.TripleDesKey;
                        else if (isKeyBased && !isRC2 && !isTripleDES && isAES)
                            HelperService.cryptionAlgorithm = HelperService.CryptionAlgorithm.AesKey;
                        else if (!isKeyBased && isRC2 && !isTripleDES && !isAES)
                            HelperService.cryptionAlgorithm = HelperService.CryptionAlgorithm.RC2Password;
                        else if (!isKeyBased && !isRC2 && isTripleDES && !isAES)
                            HelperService.cryptionAlgorithm = HelperService.CryptionAlgorithm.TripleDesPassword;
                        else if (!isKeyBased && !isRC2 && !isTripleDES && isAES)
                            HelperService.cryptionAlgorithm = HelperService.CryptionAlgorithm.AesPassword;
                        else if (isKeyBased && !isRC2 && !isTripleDES && !isAES)
                            HelperService.cryptionAlgorithm = HelperService.CryptionAlgorithm.NeedleCryptKey;
                        else
                            HelperService.cryptionAlgorithm = HelperService.CryptionAlgorithm.NeedleCryptPassword;

                        if (!isDirectory)
                        {
                            // Reading encrypted header_compressedfilesize.
                            fileStreamInput.Read(header_compressed_file_size, 0, header_compressed_file_size.Length);

                            // Decrypting header_compressedfilesize and retrieving compressedfilesize.
                            // NOTE: Here we must pass IV, not nextIV!
                            compressedfilesize = BitConverter.ToInt64(CryptionService.NeedleDecryptArray(header_compressed_file_size, keyArray, IV, keyHash, combinedHash), 0);
                        }
                        else compressedfilesize = 0;

                        // Reading encrypted header_compressed_and_encrypted_header_block_size, decrypting it and obtaining the value.
                        fileStreamInput.Read(header_compressed_and_encrypted_header_block_size, 0, header_compressed_and_encrypted_header_block_size.Length);
                        header_compressed_and_encrypted_header_block_size = CryptionService.NeedleDecryptArray(header_compressed_and_encrypted_header_block_size, keyArray, nextIV, keyHash, combinedHash);
                        headerCompressedAndEncryptedHeaderBlockSize = BitConverter.ToUInt16(header_compressed_and_encrypted_header_block_size, 0);

                        // Reading the encrypted header_block and decrypting it.
                        header_block = new byte[headerCompressedAndEncryptedHeaderBlockSize];
                        fileStreamInput.Read(header_block, 0, headerCompressedAndEncryptedHeaderBlockSize);
                        byte[] decrypted_header_block = CryptionService.NeedleDecryptArray(header_block, keyArray, nextIV, keyHash, combinedHash);

                        try
                        {
                            if (isCompressedHeader)
                                header_block = CompressionService.DeflateDecompressHeader(decrypted_header_block);
                            else header_block = decrypted_header_block;
                        }
                        catch (Exception exc) { throw exc; }

                        // Preparing to read header.
                        header_file_size = new byte[8];
                        header_file_name = new byte[isDirectory ? header_block.Length : header_block.Length - 8];

                        if (!isDirectory)
                        {
                            // Retrieving header_file_size and header_file_name.
                            Tuple<byte[], byte[]> headerBlock = HelperService.SplitByteArray(header_block, 8);
                            header_file_size = headerBlock.Item1;
                            header_file_name = headerBlock.Item2;
                        }
                        else
                        {
                            // Retrieving header_file_name.
                            header_file_name = header_block;
                        }

                        // Retrieving filename.
                        filename = Encoding.UTF8.GetString(header_file_name);

                        // Validate filename.
                        if (filename.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                        {
                            throw new Exception("Invalid signature.");
                        }

                        // Decrypting header_filesize and retrieving filesize.
                        filesize = isDirectory ? 0 : BitConverter.ToInt64(header_file_size, 0);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("This is not a valid .NHC file or the file is corrupted." + Environment.NewLine + Environment.NewLine + ex.Message);
                    }

                    // Updating output file index position.
                    if (!isDirectory) // if isFile
                    {
                        fullHeaderSize = header_settings.Length +
                            header_compressed_file_size.Length +
                            header_compressed_and_encrypted_header_block_size.Length +
                            headerCompressedAndEncryptedHeaderBlockSize;

                        globalpos += fullHeaderSize;
                        splitArchivePos = globalpos - splitGlobalPos;
                    }
                    else // if isDirectory
                    {
                        fullHeaderSize = header_settings.Length +
                            header_compressed_and_encrypted_header_block_size.Length +
                            headerCompressedAndEncryptedHeaderBlockSize;

                        globalpos += fullHeaderSize;
                        splitArchivePos = globalpos - splitGlobalPos;

                        if (isSplitArchive && (fileStreamInput.Position - splitArchivePos >= compressedfilesize))
                        {
                            if (fileStreamInput.Position - splitArchivePos > compressedfilesize) // they should be equal
                            {
                                throw new Exception("Error: fileStreamInput.Position - splitArchivePos > compressedfilesize");
                            }

                            if (fileStreamInput.Position == f_length)
                            {
                                splitArchiveCount++;
                                archFileName = $"{archiveNameWithoutExtension}.part{splitArchiveCount}{archiveExtension}";
                                splitGlobalPos = globalpos;
                            }
                        }
                    }

                    fileStreamInput.Position = isSplitArchive ? splitArchivePos : globalpos;

                    string currentOutputDirectoryName = isDirectory ?
                        Path.Combine(outputDirectoryName, filename) :
                        Path.Combine(outputDirectoryName, LongDirectory.GetDirectoryName(filename));

                    if (!LongDirectory.Exists(currentOutputDirectoryName))
                    {
                        if (extractAll || itemsToBeExtracted.Contains(filename))
                        {
                            LongDirectory.CreateDirectory(currentOutputDirectoryName);

                            if (isDirectory)
                            {
                                if (isHidden)
                                {
                                    LongFile.SetAttributes(currentOutputDirectoryName, LongFile.GetAttributes(currentOutputDirectoryName) | FileAttributes.Hidden);
                                }
                                if (isReadOnly)
                                {
                                    LongFile.SetAttributes(currentOutputDirectoryName, LongFile.GetAttributes(currentOutputDirectoryName) | FileAttributes.ReadOnly);
                                }
                                if (isSystem)
                                {
                                    LongFile.SetAttributes(currentOutputDirectoryName, LongFile.GetAttributes(currentOutputDirectoryName) | FileAttributes.System);
                                }
                                if (isArchive)
                                {
                                    LongFile.SetAttributes(currentOutputDirectoryName, LongFile.GetAttributes(currentOutputDirectoryName) | FileAttributes.Archive);
                                }
                            }
                        }
                    }

                    if (!isDirectory) // isFile
                    {
                        output_filename = Path.Combine(currentOutputDirectoryName, Path.GetFileName(filename));

                        isProcessingSplit = isSplitArchive && processedFilesNames.Contains(output_filename);

                        if (extractAll || itemsToBeExtracted.Contains(filename))
                        {
                            if (LongFile.Exists(output_filename))
                            {
                                if (!isProcessingSplit)
                                {
                                    if (overwriteFilesSetting == HelperService.OverwriteFilesSetting.Ask && confirmation != 3 && confirmation != 2) // !YesToAll and !NoToAll
                                    {
                                        Crypt.InputBoxForm inputBoxForm = new Crypt.InputBoxForm("Warning", "The file " + output_filename + " already exists. Do you want to overwrite it?", HelperService.ConfirmationButtons.YesToAllYesNoNoToAll);
                                        inputBoxForm.ShowDialog();

                                        confirmation = inputBoxForm.Result;

                                        if (confirmation == 1 || confirmation == 3) // Yes or YesToAll
                                        {
                                            if (confirmation == 3) // YesToAll
                                                overwriteFilesSetting = HelperService.OverwriteFilesSetting.Yes;

                                            processedFilesNames.Add(output_filename);
                                            isProcessingSplit = true;
                                            currentFileProcessed("Processing file " + Path.GetFileName(filename) + "...");
                                            decompressCurrentFile = true;
                                        }
                                        else
                                        {
                                            if (confirmation == 2) // NoToAll
                                                overwriteFilesSetting = HelperService.OverwriteFilesSetting.No;

                                            decompressCurrentFile = false;
                                        }
                                    }
                                    else if (overwriteFilesSetting == HelperService.OverwriteFilesSetting.Yes)
                                    {
                                        processedFilesNames.Add(output_filename);
                                        isProcessingSplit = true;
                                        currentFileProcessed("Processing file " + Path.GetFileName(filename) + "...");
                                        decompressCurrentFile = true;
                                    }
                                    else if (overwriteFilesSetting == HelperService.OverwriteFilesSetting.No)
                                    {
                                        decompressCurrentFile = false;
                                    }
                                }
                                else
                                {
                                    currentFileProcessed("Processing file " + Path.GetFileName(filename) + "...");
                                    decompressCurrentFile = true;
                                }
                            }
                            else
                            {
                                processedFilesNames.Add(output_filename);
                                isProcessingSplit = true;
                                currentFileProcessed("Processing file " + Path.GetFileName(filename) + "...");
                                decompressCurrentFile = true;
                            }
                        }
                        else decompressCurrentFile = false;

                        if (decompressCurrentFile)
                        {
                            using (var outputFileHandle = LongFile.CreateFileForAppend(LongFile.GetWin32LongPath(output_filename)))
                            using (var fileStreamOutput = new FileStream(outputFileHandle, FileAccess.ReadWrite))
                            {
                                // Decompressing and decrypting data.
                                fileStreamOutput.Position = isProcessingSplit ? fileStreamOutput.Length : 0;
                                long file_pos = isSplitArchive && isProcessingSplit ? splitGlobalPos : 0; // Entry file uncompressed position index.
                                byte[] compressed_buffer_size_bytes = new byte[2];
                                int compressed_buffer_size = 0;
                                byte[] blockCompressionTypeArr = new byte[1];
                                BlockCompressionType blockCompressionType;

                                if (filesize > 0)
                                {
                                    dynamic symmetricAlgorithm = null;

                                    switch (cryptionAlgorithm)
                                    {
                                        case HelperService.CryptionAlgorithm.RC2Key:
                                        case HelperService.CryptionAlgorithm.RC2Password:
                                            symmetricAlgorithm = new RC2CryptoServiceProvider();
                                            break;
                                        case HelperService.CryptionAlgorithm.TripleDesKey:
                                        case HelperService.CryptionAlgorithm.TripleDesPassword:
                                            symmetricAlgorithm = new TripleDESCryptoServiceProvider();
                                            break;
                                        case HelperService.CryptionAlgorithm.AesKey:
                                        case HelperService.CryptionAlgorithm.AesPassword:
                                            symmetricAlgorithm = new AesCryptoServiceProvider();
                                            break;
                                    }

                                    // Encryption mode set to Cipher Block Chaining (CBC).
                                    // Use default options for other symmetric key parameters.
                                    if (symmetricAlgorithm != null)
                                    {
                                        symmetricAlgorithm.Mode = CipherMode.CBC;
                                        symmetricAlgorithm.Padding = PaddingMode.PKCS7;
                                        symmetricAlgorithm.Key = key;
                                    }

                                    ICryptoTransform decryptor = null;

                                    byte[] block = new byte[0];
                                    byte[] compressed_block = null;
                                    blockCompressionType = BlockCompressionType.isNotCompressed; // this is a dummy value; will be replaced below.
                                    var processedBlockCount = 0;
                                    var isBlockCompressedArr = new bool[8] { false, false, false, false, false, false, false, false };
                                    var blockCompressionTypesArr = new BlockCompressionType[4];
                                    var compressedBlocksArray = new byte[4][];
                                    compressedBlocksArray[0] = new byte[0] { };
                                    compressedBlocksArray[1] = new byte[0] { };
                                    compressedBlocksArray[2] = new byte[0] { };
                                    compressedBlocksArray[3] = new byte[0] { };
                                    var fourBlocksCounter = 0;
                                    var isLastBlock = false;

                                    try
                                    {
                                        // file_pos and fileStreamInput.Position are not the same!
                                        // file_pos is the uncompressed entry file pos, while the other one is the compressed (split) archive file pos.
                                        while (isSplitArchive ? (fileStreamInput.Position - splitArchivePos < compressedfilesize) && !isLastBlock : file_pos < filesize)
                                        {
                                            if (HelperService.backgroundWorkerClosePending)
                                            {
                                                isSplitArchive = false; // this line will force to exit the loop completely
                                                break;
                                            }

                                            fourBlocksCounter++;

                                            if (fourBlocksCounter > 4)
                                            {
                                                fourBlocksCounter = 1;
                                                isBlockCompressedArr = new bool[8] { false, false, false, false, false, false, false, false };
                                                compressedBlocksArray[0] = new byte[0];
                                                compressedBlocksArray[1] = new byte[0];
                                                compressedBlocksArray[2] = new byte[0];
                                                compressedBlocksArray[3] = new byte[0];
                                            }

                                            if (isCompressed)
                                            {
                                                // Reads size of next buffer block.
                                                fileStreamInput.Read(compressed_buffer_size_bytes, 0, 2);

                                                // Retrieves "compressed_buffer_size" integer value.
                                                compressed_buffer_size = BitConverter.ToUInt16(compressed_buffer_size_bytes, 0);

                                                // Reading "compressed_block" from archive file.
                                                compressedBlocksArray[fourBlocksCounter - 1] = new byte[compressed_buffer_size];
                                                fileStreamInput.Read(compressedBlocksArray[fourBlocksCounter - 1], 0, compressed_buffer_size);

                                                // Recalculate index position.
                                                file_pos += blockSize; // may be corrected somewhere below
                                                globalpos += 2 + compressed_buffer_size;

                                                isLastBlock = isSplitArchive
                                                    ? fileStreamInput.Position == f_length - 1 || fileStreamInput.Position - splitArchivePos >= compressedfilesize - 1
                                                    : filesize - file_pos <= 0;

                                                if (!isLastBlock && fourBlocksCounter == 4 || isLastBlock)
                                                {
                                                    // Read isUncompressed block flag
                                                    fileStreamInput.Read(blockCompressionTypeArr, 0, 1);

                                                    isBlockCompressedArr = HelperService.ConvertByteToBoolArray(blockCompressionTypeArr[0]);

                                                    globalpos += 1;

                                                    if (isBlockCompressedArr[0] == false && isBlockCompressedArr[1] == false)
                                                    {
                                                        blockCompressionTypesArr[0] = BlockCompressionType.isNotCompressed;
                                                    }
                                                    else if (isBlockCompressedArr[0] == true && isBlockCompressedArr[1] == false)
                                                    {
                                                        blockCompressionTypesArr[0] = BlockCompressionType.isCompressedDeflate;
                                                    }
                                                    else if (isBlockCompressedArr[0] == false && isBlockCompressedArr[1] == true)
                                                    {
                                                        blockCompressionTypesArr[0] = BlockCompressionType.isCompressedLZMA;
                                                    }
                                                    else if (isBlockCompressedArr[0] == true && isBlockCompressedArr[1] == true)
                                                    {
                                                        blockCompressionTypesArr[0] = BlockCompressionType.isNotDefinedYet;
                                                        throw new Exception("Undefined block type");
                                                    }

                                                    if (isBlockCompressedArr[2] == false && isBlockCompressedArr[3] == false)
                                                    {
                                                        blockCompressionTypesArr[1] = BlockCompressionType.isNotCompressed;
                                                    }
                                                    else if (isBlockCompressedArr[2] == true && isBlockCompressedArr[3] == false)
                                                    {
                                                        blockCompressionTypesArr[1] = BlockCompressionType.isCompressedDeflate;
                                                    }
                                                    else if (isBlockCompressedArr[2] == false && isBlockCompressedArr[3] == true)
                                                    {
                                                        blockCompressionTypesArr[1] = BlockCompressionType.isCompressedLZMA;
                                                    }
                                                    else if (isBlockCompressedArr[2] == true && isBlockCompressedArr[3] == true)
                                                    {
                                                        blockCompressionTypesArr[0] = BlockCompressionType.isNotDefinedYet;
                                                        throw new Exception("Undefined block type");
                                                    }

                                                    if (isBlockCompressedArr[4] == false && isBlockCompressedArr[5] == false)
                                                    {
                                                        blockCompressionTypesArr[2] = BlockCompressionType.isNotCompressed;
                                                    }
                                                    else if (isBlockCompressedArr[4] == true && isBlockCompressedArr[5] == false)
                                                    {
                                                        blockCompressionTypesArr[2] = BlockCompressionType.isCompressedDeflate;
                                                    }
                                                    else if (isBlockCompressedArr[4] == false && isBlockCompressedArr[5] == true)
                                                    {
                                                        blockCompressionTypesArr[2] = BlockCompressionType.isCompressedLZMA;
                                                    }
                                                    else if (isBlockCompressedArr[4] == true && isBlockCompressedArr[5] == true)
                                                    {
                                                        blockCompressionTypesArr[0] = BlockCompressionType.isNotDefinedYet;
                                                        throw new Exception("Undefined block type");
                                                    }

                                                    if (isBlockCompressedArr[6] == false && isBlockCompressedArr[7] == false)
                                                    {
                                                        blockCompressionTypesArr[3] = BlockCompressionType.isNotCompressed;
                                                    }
                                                    else if (isBlockCompressedArr[6] == true && isBlockCompressedArr[7] == false)
                                                    {
                                                        blockCompressionTypesArr[3] = BlockCompressionType.isCompressedDeflate;
                                                    }
                                                    else if (isBlockCompressedArr[6] == false && isBlockCompressedArr[7] == true)
                                                    {
                                                        blockCompressionTypesArr[3] = BlockCompressionType.isCompressedLZMA;
                                                    }
                                                    else if (isBlockCompressedArr[6] == true && isBlockCompressedArr[7] == true)
                                                    {
                                                        blockCompressionTypesArr[0] = BlockCompressionType.isNotDefinedYet;
                                                        throw new Exception("Undefined block type");
                                                    }
                                                }
                                                else continue;
                                            }
                                            else // !isCompressed
                                            {
                                                blockCompressionType = BlockCompressionType.isNotCompressed;

                                                var isEncryptedWithPadding = isAES || isTripleDES || isRC2;

                                                if (isEncryptedWithPadding)
                                                {
                                                    // Reads size of next buffer block.
                                                    fileStreamInput.Read(compressed_buffer_size_bytes, 0, 2);

                                                    // Retrieves "compressed_buffer_size" integer value.
                                                    compressed_buffer_size = BitConverter.ToUInt16(compressed_buffer_size_bytes, 0);
                                                }

                                                isLastBlock = isSplitArchive
                                                    ? fileStreamInput.Position + (isEncryptedWithPadding ? compressed_buffer_size : blockSize) == f_length || fileStreamInput.Position - splitArchivePos + (isEncryptedWithPadding ? compressed_buffer_size : blockSize) >= compressedfilesize
                                                    : filesize - file_pos <= (isEncryptedWithPadding ? blockSize + IV.Length : blockSize);

                                                var blkSize = isEncryptedWithPadding
                                                    ? compressed_buffer_size
                                                    : isLastBlock
                                                        ? isSplitArchive
                                                            ? compressedfilesize - (fileStreamInput.Position - splitArchivePos)
                                                            : filesize - file_pos
                                                        : blockSize;

                                                // Reading "compressed_block" (in this case it's rather the encrypted block) from archive file.
                                                compressed_block = new byte[blkSize];
                                                fileStreamInput.Read(compressed_block, 0, compressed_block.Length);

                                                // Recalculate index position.
                                                file_pos += blkSize;
                                                globalpos += blkSize + (isEncryptedWithPadding ? 2 : 0);
                                            }

                                            processedBlockCount = 0;

                                            if (isSplitArchive && (fileStreamInput.Position - splitArchivePos >= compressedfilesize))
                                            {
                                                if (fileStreamInput.Position - splitArchivePos > compressedfilesize) // they should be equal
                                                {
                                                    throw new Exception("Error: fileStreamInput.Position - splitArchivePos > compressedfilesize");
                                                }

                                                if (fileStreamInput.Position == f_length)
                                                {
                                                    splitArchiveCount++;
                                                    archFileName = $"{archiveNameWithoutExtension}.part{splitArchiveCount}{archiveExtension}";
                                                    splitGlobalPos = globalpos;
                                                }
                                            }

                                        processBlocks:

                                            if (cryptionAlgorithm != HelperService.CryptionAlgorithm.NeedleCryptKey && cryptionAlgorithm != HelperService.CryptionAlgorithm.NeedleCryptPassword)
                                            {
                                                symmetricAlgorithm.IV = nextIV;
                                                decryptor = symmetricAlgorithm.CreateDecryptor();
                                            }

                                            if (isCompressed) // && fourBlocksCounter == 4
                                            {
                                                compressed_block = compressedBlocksArray[processedBlockCount];
                                                blockCompressionType = blockCompressionTypesArr[processedBlockCount];
                                                processedBlockCount++;
                                            }

                                            // Decrypt "compressed_block" and retrieve "decrypted_block".
                                            byte[] decrypted_block;
                                            if (cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptKey || cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptPassword)
                                                decrypted_block = CryptionService.NeedleDecryptArray(compressed_block, key, nextIV, keyHash, combinedHash);
                                            else decrypted_block = decryptor.TransformFinalBlock(compressed_block, 0, compressed_block.Length);

                                            // Decompress "decrypted_block" and retrieve "block".
                                            if (blockCompressionType != BlockCompressionType.isNotCompressed)
                                            {
                                                block = (blockCompressionType == BlockCompressionType.isCompressedLZMA)
                                                    ? CompressionService.LZMADecompressArray(decrypted_block)
                                                    : CompressionService.DeflateDecompressArray(decrypted_block);
                                            }
                                            else
                                            {
                                                block = decrypted_block;
                                            }

                                            // Write decompressed and decrypted "block" to output file.
                                            fileStreamOutput.Write(block, 0, block.Length);

                                            file_pos = isCompressed ? file_pos - blockSize + block.Length : file_pos; // here we are correcting file_pos value (in case it was wrong for last segment)

                                            byte[] lastBytes = HelperService.GetByteArraySegment(compressed_block,
                                                    compressed_block.Length >= IV.Length
                                                        ? compressed_block.Length - IV.Length
                                                        : 0,
                                                    compressed_block.Length >= IV.Length ? IV.Length : compressed_block.Length);

                                            lastBytes = (processedBlockCount < fourBlocksCounter) || !isCompressed
                                                ? lastBytes
                                                : HelperService.CombineByteArrays(
                                                    lastBytes,
                                                    blockCompressionTypeArr);

                                            lastBytes = HelperService.GetByteArraySegment(
                                                lastBytes, lastBytes.Length >= IV.Length ? lastBytes.Length - IV.Length : 0, lastBytes.Length >= IV.Length ? IV.Length : lastBytes.Length);

                                            nextIV = lastBytes.Length == IV.Length
                                                    ? lastBytes
                                                    : HelperService.CombineByteArrays(
                                                        HelperService.GetByteArraySegment(nextIV, lastBytes.Length, IV.Length - lastBytes.Length),
                                                        lastBytes);

                                            if (!isCompressed || processedBlockCount < fourBlocksCounter)
                                            {
                                                // Report progress to UI.

                                                long currentProgress = (stepNumber * 100 / numberOfSteps) + (globalpos * 100 / files_length / numberOfSteps);

                                                progress((int)currentProgress);

                                                // Report current time to UI.
                                                currentDateTime(DateTime.Now);
                                            }

                                            if (isCompressed && processedBlockCount < fourBlocksCounter) // && fourBlocksCounter == 4
                                            {
                                                goto processBlocks;
                                            }

                                            if (processedBlockCount == fourBlocksCounter)
                                            {
                                                // Report progress to UI.

                                                long currentProgress = (stepNumber * 100 / numberOfSteps) + (globalpos * 100 / files_length / numberOfSteps);

                                                progress((int)currentProgress);

                                                // Report current time to UI.
                                                currentDateTime(DateTime.Now);
                                            }

                                            processedBlockCount = 0;
                                        }

                                        if (HelperService.backgroundWorkerClosePending)
                                        {
                                            isSplitArchive = false; // this line will force to exit the loop completely
                                            break;
                                        }
                                    }
                                    catch (Exception exx)
                                    {
                                        throw exx;
                                    }
                                    finally
                                    {
                                        if (symmetricAlgorithm != null)
                                        {
                                            symmetricAlgorithm.Clear();
                                            decryptor.Dispose();
                                        }
                                    }
                                }
                                else
                                {
                                    // Report progress to UI.

                                    long currentProgress = (stepNumber * 100 / numberOfSteps) + (globalpos * 100 / files_length / numberOfSteps);

                                    progress((int)currentProgress);


                                    // Report current time to UI.
                                    currentDateTime(DateTime.Now);
                                }
                            }

                            if (isHidden)
                            {
                                LongFile.SetAttributes(output_filename, LongFile.GetAttributes(output_filename) | FileAttributes.Hidden);
                            }
                            if (isReadOnly)
                            {
                                LongFile.SetAttributes(output_filename, LongFile.GetAttributes(output_filename) | FileAttributes.ReadOnly);
                            }
                            if (isSystem)
                            {
                                LongFile.SetAttributes(output_filename, LongFile.GetAttributes(output_filename) | FileAttributes.System);
                            }
                            if (isArchive)
                            {
                                LongFile.SetAttributes(output_filename, LongFile.GetAttributes(output_filename) | FileAttributes.Archive);
                            }
                            if (isTemporary)
                            {
                                LongFile.SetAttributes(output_filename, LongFile.GetAttributes(output_filename) | FileAttributes.Temporary);
                            }
                            if (isSparse)
                            {
                                LongFile.SetAttributes(output_filename, LongFile.GetAttributes(output_filename) | FileAttributes.SparseFile);
                            }
                            if (isNormal)
                            {
                                LongFile.SetAttributes(output_filename, FileAttributes.Normal);
                            }

                        }
                        else // if not decompress current file
                        { 
                            globalpos += compressedfilesize;
                            fileStreamInput.Position += compressedfilesize;

                            if (fileStreamInput.Position == f_length)
                            {
                                splitArchiveCount++;
                                archFileName = $"{archiveNameWithoutExtension}.part{splitArchiveCount}{archiveExtension}";
                                splitGlobalPos = globalpos;
                            }
                        }

                        // Recalculate global index position of input file (i.e. .NHC archive).
                        splitArchivePos += compressedfilesize;
                    }

                    if (isDirectory || !extractAll && !itemsToBeExtracted.Contains(filename) || filesize == 0)
                    {
                        fileStreamInput.Position = isSplitArchive 
                            ? splitArchivePos > IV.Length ? splitArchivePos - IV.Length : 0
                            : globalpos > IV.Length ? globalpos - IV.Length : 0;

                        var lastBytez = new byte[isSplitArchive 
                            ? splitArchivePos > IV.Length ? IV.Length : splitArchivePos
                            : globalpos > IV.Length ? IV.Length : globalpos];

                        fileStreamInput.Read(lastBytez, 0, fileStreamInput.Position == 0 ? (int)(isSplitArchive ? splitArchivePos : globalpos) : IV.Length);

                        nextIV = lastBytez.Length == IV.Length
                            ? lastBytez
                            : HelperService.CombineByteArrays(
                                HelperService.GetByteArraySegment(nextIV, lastBytez.Length,
                                    IV.Length - lastBytez.Length), lastBytez);

                    }

                    if (HelperService.backgroundWorkerClosePending)
                        break;

                    // Report number of files extracted so far to UI.
                    if (extractAll || itemsToBeExtracted.Contains(filename))
                    {
                        numberOfEntriesProcessed(++nrOfEntriesProcessed);
                        nrOfCurrentEntriesProcessed++;
                    }
                    else
                    {
                        // Report progress to UI.

                        long currentProgress = (stepNumber * 100 / numberOfSteps) + (globalpos * 100 / files_length / numberOfSteps);

                        progress((int)currentProgress);

                        // Report current time to UI.
                        currentDateTime(DateTime.Now);
                    }

                } while (f_length - splitArchivePos > 0);

                splitArchivePos = fileStreamInput.Position == f_length ? 0 : splitArchivePos;

                if (!isSplitArchive)
                    break;
            }
        }

        long crrntProgress = (stepNumber + 1) * 100 / numberOfSteps;
        progress((int)crrntProgress);
    }
}

/********************************************************************************************************************/