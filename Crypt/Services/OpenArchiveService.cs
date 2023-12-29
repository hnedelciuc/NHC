/********************************************************************************************************************
/ Needle in a Haystack in a Crypt v1.0.
/ Copyright (C) 2016-2023 by Horia Nedelciuc from Chisinau, Moldova.
/********************************************************************************************************************
/ Open Archive Service.
/
/********************************************************************************************************************/

using GracefulDynamicDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;

internal class OpenArchiveService
{
    /********************************************************************************************************************/

    private const int blockSize = MainService.blockSize;
    internal static bool[] file_attributes = null;
    internal static bool[] nhc_settings = null;
    
    /********************************************************************************************************************/

    // Opens given .NHC archive and passes back contents as nodes in CryptForm's treeView

    internal static TreeNode OpenArchive(
        long stepNumber, 
        long numberOfSteps, 
        string archiveFileName,
        HelperService.CryptionAlgorithm cryptionAlgorithm,
        string passPhrase,
        byte[] key,
        byte[] keyArray,
        byte[] IV,
        Crypt.ProgressDelegate progress, 
        Crypt.CurrentFileProcessedDelegate currentFileProcessed,
        Crypt.NumberOfEntriesProcessedDelegate numberOfFilesProcessed, 
        Crypt.CurrentDateTimeDelegate currentDateTime)
    {
        TreeNode rootNode = new TreeNode(Path.GetFileName(archiveFileName)) { Name = archiveFileName, ImageIndex = 0, SelectedImageIndex = 0 };
        long compressedfilesize, filesize, f_length;
        string filename, currentFileProcessedShow = String.Empty;
        long currentProgress;
        int headerCompressedAndEncryptedBlockSize, headerCompressedAndEncryptedFullBlockSize; //, compressedheadersize;
        DateTime currentDateTimeShow = DateTime.Now;
        bool isDirectory, isHidden, isReadOnly, isSystem, isArchive, isTemporary, isSparse, isNormal;
        bool isCompressed, isLZMA, isKeyBased, isRC2, isTripleDES, isAES, isCompressedHeader;
        bool highCompressionArchiveDetected = false;

        switch (cryptionAlgorithm)
        {
            case HelperService.CryptionAlgorithm.AesKey:
            case HelperService.CryptionAlgorithm.RC2Key:
            case HelperService.CryptionAlgorithm.TripleDesKey:
                passPhrase = null;
                break;
            case HelperService.CryptionAlgorithm.NeedleCryptKey:
                passPhrase = null;
                keyArray = key;
                break;
            case HelperService.CryptionAlgorithm.NeedleCryptPassword:
                keyArray = key;
                break;
        }

        Tuple<byte[], byte[]> needleCryptParams = CryptionService.PrepareNeedleCrypt(CryptionService.saltVal, passPhrase, key);

        byte[] keyHash = needleCryptParams.Item1;
        byte[] combinedHash = needleCryptParams.Item2;

        //HelperService.archiveFileSize = 0;
        //HelperService.compressedFilesSize = 0;
        //HelperService.compressedHeadersSize = 0;
        //HelperService.uncompressedFilesSize = 0;
        var splitCount = 0;
        var archFileName = $"{archiveFileName}";
        var archiveExtension = Path.GetExtension(archiveFileName);
        var archiveNameWithoutExtension = LongDirectory.Combine(Path.GetDirectoryName(archiveFileName), Path.GetFileNameWithoutExtension(archiveFileName));
        int count = 0;
        long files_length = 0;
        long globalpos = 0;
        bool isSplitArchive = false;
        byte[] nextIV = (byte[])IV.Clone();

        while (splitCount == 0 || LongFile.Exists(archFileName))
        {
            using (var inputFileHandle = LongFile.GetFileHandle(LongFile.GetWin32LongPath(archFileName)))
            using (var fileStreamInput = new FileStream(inputFileHandle, FileAccess.Read))
            {
                f_length = fileStreamInput.Length;
                files_length += f_length;
                long splitGlobalPos = 0;

                byte[] header_settings;
                byte[] header_compressed_file_size;
                byte[] header_compressed_and_encrypted_header_block_size;
                byte[] header_file_size;
                byte[] header_file_name;
                byte[] header_block;

                // Start iterating through all entries (i.e. files) in the .NHC archive and extracting them.
                do
                {
                    header_settings = new byte[2];
                    header_compressed_file_size = new byte[8];
                    header_compressed_and_encrypted_header_block_size = new byte[2];

                    try
                    {
                        // Set input file (.NHC archive) index position at beginning of next entry (i.e. next input file or folder).
                        fileStreamInput.Position = splitGlobalPos;

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

                        if (isLZMA)
                        {
                            HelperService.compressionLevel = CompressionLevel.Optimal;
                            highCompressionArchiveDetected = true;
                        }
                        else if (!highCompressionArchiveDetected)
                        {
                            if (isCompressed)
                                HelperService.compressionLevel = CompressionLevel.Fastest;
                            else HelperService.compressionLevel = CompressionLevel.NoCompression;
                        }

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
                            compressedfilesize = BitConverter.ToInt64(
                                CryptionService.NeedleDecryptArray(header_compressed_file_size, keyArray, IV, keyHash, combinedHash), 0);
                        }
                        else compressedfilesize = 0;

                        // Reading encrypted header_filenamesize.
                        fileStreamInput.Read(header_compressed_and_encrypted_header_block_size, 0, header_compressed_and_encrypted_header_block_size.Length);
                        header_compressed_and_encrypted_header_block_size = CryptionService.NeedleDecryptArray(header_compressed_and_encrypted_header_block_size, keyArray, nextIV, keyHash, combinedHash);
                        headerCompressedAndEncryptedBlockSize = BitConverter.ToUInt16(header_compressed_and_encrypted_header_block_size, 0);
                        header_block = new byte[headerCompressedAndEncryptedBlockSize];
                        fileStreamInput.Read(header_block, 0, headerCompressedAndEncryptedBlockSize);

                        // Decrypting and decompressing header_block.
                        byte[] decrypted_header_block = CryptionService.NeedleDecryptArray(header_block, keyArray, nextIV, keyHash, combinedHash);

                        try
                        {
                            if (isCompressedHeader)
                                header_block = CompressionService.DeflateDecompressHeader(decrypted_header_block);
                            else header_block = decrypted_header_block;

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
                        }
                        catch (Exception exx)
                        {
                            header_block = CompressionService.DeflateDecompressHeader(decrypted_header_block);

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
                            if (filename.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                            {
                                throw new Exception("Invalid signature.");
                            }
                        }
                        
                        // Decrypting header_filesize and retrieving filesize.
                        filesize = isDirectory ? 0 : BitConverter.ToInt64(header_file_size, 0);

                        headerCompressedAndEncryptedFullBlockSize = headerCompressedAndEncryptedBlockSize;

                        bool isSplitContinuation = false;
                        
                        HelperService.AddNodePath(rootNode, String.Empty, filename, filesize, compressedfilesize, headerCompressedAndEncryptedBlockSize, isDirectory, ref isSplitContinuation);

                        count += isSplitContinuation ? 0 : 1;

                        HelperService.uncompressedFilesSize += isSplitContinuation ? 0 : filesize;
                        HelperService.compressedHeadersSize += headerCompressedAndEncryptedFullBlockSize + 2 + (isDirectory ? 0 : 8) + 2;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("The file " + archiveFileName +
                                        " either is not a valid .NHC file, or is corrupted, or an incorrect cryption key or password was provided." + Environment.NewLine + Environment.NewLine + ex.Message);
                    }

                    // Recalculate global index position of output file (i.e. .NHC archive).
                    if (!isDirectory)
                    {
                        splitGlobalPos += header_settings.Length + header_compressed_file_size.Length + header_compressed_and_encrypted_header_block_size.Length + headerCompressedAndEncryptedBlockSize + compressedfilesize;
                        globalpos += header_settings.Length + header_compressed_file_size.Length + header_compressed_and_encrypted_header_block_size.Length + headerCompressedAndEncryptedBlockSize + compressedfilesize;
                    }
                    else
                    {
                        splitGlobalPos += header_settings.Length + header_compressed_and_encrypted_header_block_size.Length + headerCompressedAndEncryptedBlockSize;
                        globalpos += header_settings.Length + header_compressed_and_encrypted_header_block_size.Length + headerCompressedAndEncryptedBlockSize;
                    }
                    

                    fileStreamInput.Position = splitGlobalPos > IV.Length ? splitGlobalPos - IV.Length : 0;

                    byte[] lastBytes = new byte[splitGlobalPos > IV.Length ? IV.Length : splitGlobalPos];

                    fileStreamInput.Read(lastBytes, 0, splitGlobalPos > IV.Length ? IV.Length : (int)splitGlobalPos);

                    nextIV = lastBytes.Length == IV.Length
                        ? lastBytes
                        : HelperService.CombineByteArrays(
                            HelperService.GetByteArraySegment(nextIV, lastBytes.Length,
                                IV.Length - lastBytes.Length), lastBytes);

                    // Report progress to UI.
                    currentProgress = (stepNumber * 100 / numberOfSteps) + (globalpos * 100 / files_length / numberOfSteps);
                    progress((int)currentProgress);

                    currentDateTimeShow = DateTime.Now;
                    currentFileProcessedShow = Path.GetFileName(filename);

                    if (count % 10 == 0)
                    {
                        currentDateTime(currentDateTimeShow);
                        numberOfFilesProcessed(count);
                        currentFileProcessed(currentFileProcessedShow);
                    }

                } while (f_length - splitGlobalPos > 0);
            }

            HelperService.archiveFileSize += f_length;
            splitCount++;
            archFileName = $"{archiveNameWithoutExtension}.part{splitCount}{archiveExtension}";
        }

        var allNodes = new ArrayList();
        HelperService.SelectAllNodes(rootNode.Nodes, true, ref allNodes);

        foreach (dynamic node in allNodes)
        {
            if (HelperService.backgroundWorkerClosePending)
                break;

            long? cmprssdFullFileSize = null;
            long? cmprssdFullHeaderSize = null;

            // node.Tag is DDict, it won't throw exception if value not found
            cmprssdFullFileSize = (dynamic)node.Tag.compressedFullFileSize;
            cmprssdFullHeaderSize = (dynamic)node.Tag.compressedFullHeaderSize;

            HelperService.compressedFilesSize += cmprssdFullFileSize ?? ((dynamic)node.Tag)?.compressedFileSize ?? 0;
        }

        rootNode.Tag = new DDict(new Dictionary<string, dynamic>()
        {
            { "isArchiveRoot", true },
            { "isSplitArchive", isSplitArchive},
            { "splitCount", splitCount}
        });

        HelperService.splitCount = splitCount;

        return rootNode;
    }

    /********************************************************************************************************************/
}
