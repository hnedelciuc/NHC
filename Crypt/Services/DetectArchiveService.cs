/********************************************************************************************************************
/ Needle in a Haystack in a Crypt v1.0.
/ Copyright (C) 2016-2023 by Horia Nedelciuc from Chisinau, Moldova.
/********************************************************************************************************************
/ Open Archive Service.
/
/********************************************************************************************************************/

using System.IO;
using System.IO.Compression;

internal class DetectArchiveService
{
    /********************************************************************************************************************/

    internal static bool[] nhc_settings = null;

    /********************************************************************************************************************/

    // Opens given .NHC archive and passes back contents as nodes in CryptForm's treeView

    internal static HelperService.CryptionAlgorithm DetectArchive(string archiveFileName)
    {
        bool isCompressed, isLZMA, isKeyBased, isRC2, isTripleDES, isAES;

        using (var inputFileHandle = LongFile.GetFileHandle(LongFile.GetWin32LongPath(archiveFileName)))
        using (var fileStreamInput = new FileStream(inputFileHandle, FileAccess.Read))
        {
            var f_length = fileStreamInput.Length;

            byte[] header_settings = new byte[2];

            // Set input file (.NHC archive) index position at beginning of next entry (i.e. next input file or folder).
            fileStreamInput.Position = 0;

            // Reading encrypted header_isdirectory.
            fileStreamInput.Read(header_settings, 0, header_settings.Length);

            nhc_settings = HelperService.ConvertByteToBoolArray(header_settings[1]);

            // Retrieving nhc_settings.
            isCompressed = nhc_settings[0];
            isLZMA = nhc_settings[1];
            isKeyBased = nhc_settings[2];
            isRC2 = nhc_settings[3];
            isTripleDES = nhc_settings[4];
            isAES = nhc_settings[5];

            if (isCompressed)
            {
                if (isLZMA)
                    HelperService.compressionLevel = CompressionLevel.Optimal;
                else
                    HelperService.compressionLevel = CompressionLevel.Fastest;
            }
            else HelperService.compressionLevel = CompressionLevel.NoCompression;

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

        }
        
        return HelperService.cryptionAlgorithm;
    }

    /********************************************************************************************************************/
}
