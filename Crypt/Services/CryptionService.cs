/********************************************************************************************************************
/ Needle in a Haystack in a Crypt v1.0.
/ Copyright (C) 2016-2023 by Horia Nedelciuc from Chisinau, Moldova.
/********************************************************************************************************************
/ Cryption Service.
/ -- Symmetric key encryption and decryption using proprietary NeedleCrypt algorithm by Horia Nedelciuc.
/ -- Other helper methods.
/********************************************************************************************************************/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

internal class CryptionService
{
    /********************************************************************************************************************/

    // Default values used for cryption:
    // Note: For Password-Based cryption, the following saltValue and initVector strings will be modified 
    //       by combining them with the user-provided passPhrase.
    // Note: For Key-Based "Use Built-In Default" option, all these values will be used.

    internal static string saltVal = @"k]j%V_M>J);(9{)C#/avd#sTM?w@Mx4}<hH`1WgjJPz7rt=RAekh?pV*B\LX3:3.";  // can be any string, this one is 64 bytes.
    internal static string initVect = "{>NR6F#~(3?E!yXU";   // must be 16 bytes.
    internal static int passwordIterations = 1134;          // can be any number.
    internal static int keySize = 256;                      // key size in bits. Can be 256, 192 or 128.

    // The cipher block mode used for encryption / decryption: Cipher Block Chaining (CBC).

    /********************************************************************************************************************/

    internal static byte[] GetCryptedBytesFromStringAndPassword(string str, string key)
    {
        var xorCrypted = new StringBuilder();

        for (int c = 0; c < str.Length; c++)
        {
            // take next character from string
            char character = str[c];

            // cast to a uint
            uint charCode = (uint)character;

            // figure out which character to take from the key
            int keyPosition = c % key.Length; // use modulo to "wrap round"

            // take the key character
            char keyChar = key[keyPosition];

            // cast it to a uint also
            uint keyCode = (uint)keyChar;

            // perform XOR on the two character codes
            uint combinedCode = (charCode ^ keyCode) ^ (uint)c + 13;

            // cast back to a char
            char combinedChar = (char)combinedCode;

            // add to the result
            xorCrypted.Append(combinedChar);
        }

        SHA512 sha512 = new SHA512Managed();
        byte[] salt = sha512.ComputeHash(Encoding.ASCII.GetBytes(xorCrypted.ToString()));

        Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(xorCrypted.ToString(), salt, passwordIterations);

        return password.GetBytes(str.Length); 
    }

    /********************************************************************************************************************/

    internal static Tuple<byte[], byte[]> PrepareNeedleCrypt(string saltVal, string passPhrase, byte[] key)
    {
        SHA512 sha512 = new SHA512Managed();
        byte[] keyHash = sha512.ComputeHash(key);

        passPhrase = string.IsNullOrEmpty(passPhrase) ? initVect : passPhrase;

        byte[] salt = HelperService.CombineByteArrays(Encoding.ASCII.GetBytes(saltVal), Encoding.ASCII.GetBytes(passPhrase), keyHash);

        Rfc2898DeriveBytes saltValPwdKey = new Rfc2898DeriveBytes(passPhrase + Encoding.ASCII.GetString(key), salt, passwordIterations);
        byte[] saltValPwdKeyBytes = saltValPwdKey.GetBytes(256);

        byte[] combinedHash = sha512.ComputeHash(HelperService.CombineByteArrays(keyHash, saltValPwdKeyBytes));

        return new Tuple<byte[], byte[]>(keyHash, combinedHash);
    }

    /********************************************************************************************************************/

    // Encrypts given array using NeedleCrypt algorithm with given password / salt value and key.

    internal static byte[] NeedleEncryptArray(byte[] block, byte[] keyArray, byte[] IV, byte[] keyHash, byte[] combinedHash)
    {
        block = HelperService.CombineByteArrays(IV, block);

        byte[] result = new byte[block.Length];

        for (int i = 0; i < block.Length; i++)
        {
            // Encrypt character according to cryption key table and prepare it for next XOR encryption.
            byte charCode = keyArray[block[i]];

            int keyPosition1 = i % saltVal.Length; // use modulo to "wrap round".
            char keyChar1 = saltVal[keyPosition1];
            byte keyCode1 = (byte)keyChar1;

            int keyPosition2 = i % keyHash.Length;
            byte keyCode2 = keyHash[keyPosition2];

            int keyPosition3 = i % combinedHash.Length;
            byte keyCode3 = combinedHash[keyPosition3];

            // Perform encryption.
            byte combinedCode;

            if (i == 0)
            {
                combinedCode = (byte)(charCode ^ keyCode1 ^ ((keyCode2 ^ keyCode3) + keyCode3 + 13));
            }
            else if (i == 1)
            {
                combinedCode = (byte)(charCode ^ keyCode1 ^ (((keyCode2 ^ keyCode3) + 1) ^ block[i - 1]) + keyCode3 - 13);
            }
            else if ((i ^ keyArray[block[i - 2]]) % 2 == 0)
            {
                combinedCode = (byte)(charCode ^ keyCode1 ^ ((keyPosition1 + (keyCode2 ^ keyCode3)) ^ block[i - 1]) + i + keyCode3 ^ block[i - 2] + keyPosition2);
            }
            else
            {
                combinedCode = (byte)(charCode ^ keyCode1 ^ ((keyPosition2 + (keyCode2 ^ keyCode3)) ^ block[i - 2]) + i + keyCode3 ^ block[i - 1] - keyPosition3);
            }

            // Save resulting character byte.
            result[i] = combinedCode;
        }

        return HelperService.GetByteArraySegment(result, IV.Length);
    }

    // Decrypts given array using NeedleCrypt algorithm with given password / salt value and key.

    internal static byte[] NeedleDecryptArray(byte[] block, byte[] keyArray, byte[] IV, byte[] keyHash, byte[] combinedHash)
    {
        byte[] encryptedIV = NeedleEncryptArray(IV, keyArray, new byte[0], keyHash, combinedHash);

        block = HelperService.CombineByteArrays(encryptedIV, block);
        
        byte[] result = new byte[block.Length];

        for (int i = 0; i < block.Length; i++)
        {
            // Decrypt character according to cryption key table and prepare it for next XOR decryption.
            byte charCode = block[i];

            int keyPosition1 = i % saltVal.Length; // use modulo to "wrap round".
            char keyChar1 = saltVal[keyPosition1];
            byte keyCode1 = (byte)keyChar1;

            int keyPosition2 = i % keyHash.Length;
            byte keyCode2 = keyHash[keyPosition2];

            int keyPosition3 = i % combinedHash.Length;
            byte keyCode3 = combinedHash[keyPosition3];

            // Perform encryption.
            byte combinedCode;

            if (i == 0)
            {
                combinedCode = (byte)(charCode ^ keyCode1 ^ ((keyCode2 ^ keyCode3) + keyCode3 + 13));
            }
            else if (i == 1)
            {
                combinedCode = (byte)(charCode ^ keyCode1 ^ (((keyCode2 ^ keyCode3) + 1) ^ result[i - 1]) + keyCode3 - 13);
            }
            else if ((i ^ keyArray[result[i - 2]]) % 2 == 0)
            {
                combinedCode = (byte)(charCode ^ keyCode1 ^ ((keyPosition1 + (keyCode2 ^ keyCode3)) ^ result[i - 1]) + i + keyCode3 ^ result[i - 2] + keyPosition2);
            }
            else
            {
                combinedCode = (byte)(charCode ^ keyCode1 ^ ((keyPosition2 + (keyCode2 ^ keyCode3)) ^ result[i - 2]) + i + keyCode3 ^ result[i - 1] - keyPosition3);
            }

            // Save resulting character byte.
            result[i] = keyArray[combinedCode];
        }

        return HelperService.GetByteArraySegment(result, IV.Length);
    }

    /********************************************************************************************************************/

    // Checks the arguments for the EncryptBlock with password and DecryptBlock with password methods.

    internal static bool CheckArgumentsCryptWithPwd(HelperService.CryptionAlgorithm cryptionAlg, string passPhrase, string saltValue, 
        int passwordIterations, byte[] initVector, int keySize)
    {
        if (passPhrase == null || passPhrase.Length == 0)
            throw new ArgumentNullException("passPhrase");
        if (saltValue == null || saltValue.Length == 0)
            throw new ArgumentNullException("saltValue");
        if (passwordIterations <= 0)
            throw new ArgumentNullException("passwordIterations");

        switch (cryptionAlg)
        {
            case HelperService.CryptionAlgorithm.RC2Password:
                if (keySize != 128)
                    throw new ArgumentException("key");
                if (initVector == null || initVector.Length != 8)
                    throw new ArgumentException("initVector");
                break;
            case HelperService.CryptionAlgorithm.TripleDesPassword:
                if (keySize != 192)
                    throw new ArgumentException("key");
                if (initVector == null || initVector.Length != 8)
                    throw new ArgumentException("initVector");
                break;
            case HelperService.CryptionAlgorithm.AesPassword:
                if (keySize != 256)
                    throw new ArgumentException("key");
                if (initVector == null || initVector.Length != 16)
                    throw new ArgumentException("initVector");
                break;
            case HelperService.CryptionAlgorithm.NeedleCryptPassword:
                break;
        }
        
        return true;
    }

    /********************************************************************************************************************/

    // Checks the arguments for the EncryptBlock with key and DecryptBlock with key methods.

    internal static bool CheckArguments(byte[] key, byte[] initVector, HelperService.CryptionAlgorithm cryptionAlgorithm)
    {
        switch (cryptionAlgorithm)
        {
            case HelperService.CryptionAlgorithm.RC2Key:
            case HelperService.CryptionAlgorithm.RC2Password:
                if (key == null || key.Length != (128 / 8))
                    throw new ArgumentException("key");
                if (initVector == null || initVector.Length != 8)
                    throw new ArgumentException("initVector");
                break;
            case HelperService.CryptionAlgorithm.TripleDesKey:
            case HelperService.CryptionAlgorithm.TripleDesPassword:
                if (key == null || key.Length != (192 / 8))
                    throw new ArgumentException("key");
                if (initVector == null || initVector.Length != 8)
                    throw new ArgumentException("initVector");
                break;
            case HelperService.CryptionAlgorithm.AesKey:
            case HelperService.CryptionAlgorithm.AesPassword:
                if (key == null || key.Length != (256 / 8))
                    throw new ArgumentException("key");
                if (initVector == null || initVector.Length != 16)
                    throw new ArgumentException("initVector");
                break;
            case HelperService.CryptionAlgorithm.NeedleCryptKey:
            case HelperService.CryptionAlgorithm.NeedleCryptPassword:
                break;
        }
        
        return true;
    }

    /********************************************************************************************************************/

    // Generate new random cryption key to be used with AES algorithm and save it to a given file.

    internal static bool GenerateKey(string keyFileName, HelperService.CryptionAlgorithm cryptionAlgorithm, string pwd = null)
    {
        if (cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptKey || cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptPassword)
        {
            byte?[] keyArray = new byte?[256];
            byte[] resultArray = new byte[256];

            if (keyFileName != String.Empty)
            {
                if (!File.Exists(keyFileName)
                    || (System.Windows.Forms.MessageBox.Show("The file " + keyFileName + " already exists. Do you want to replace it?", "Warning!", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes))
                {
                    // Open the stream, generate and write the cryption key in it.
                    using (FileStream fileStreamKey = File.Open(keyFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        while (HelperService.CheckNull(keyArray))
                        {
                            byte index = HelperService.RandomByte();
                            byte element = HelperService.RandomByte();
                            if (index != element && HelperService.ElementAvailable(keyArray, index, element))
                            {
                                keyArray[index] = element;
                                keyArray[element] = index;
                            }
                        }

                        // Since keyArray is of nullable byte array type, it must be first converted
                        // to regular byte array (resultArray) before being written to stream
                        for (int i = 0; i <= 255; i++)
                        {
                            resultArray[i] = (byte)keyArray[i];
                        }

                        fileStreamKey.Write(resultArray, 0, 256);

                        pwd = string.IsNullOrEmpty(pwd) ? saltVal + Encoding.ASCII.GetString(resultArray) : (pwd + saltVal + Encoding.ASCII.GetString(resultArray));

                        byte[] initVectorBytes = GetCryptedBytesFromStringAndPassword(initVect, pwd);

                        fileStreamKey.Write(initVectorBytes, 0, initVectorBytes.Length);

                        return true;
                    }
                }
            }
            return false;
        }
        else
        {
            if (keyFileName != String.Empty)
            {
                if (!File.Exists(keyFileName)
                    || (System.Windows.Forms.MessageBox.Show("The file " + keyFileName + " already exists. Do you want to replace it?", "Warning", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes))
                {
                    dynamic symmetricKey = null;

                    switch (cryptionAlgorithm)
                    {
                        case HelperService.CryptionAlgorithm.RC2Key:
                            symmetricKey = new RC2CryptoServiceProvider();
                            break;
                        case HelperService.CryptionAlgorithm.TripleDesKey:
                            symmetricKey = new TripleDESCryptoServiceProvider();
                            break;
                        case HelperService.CryptionAlgorithm.AesKey:
                            symmetricKey = new AesCryptoServiceProvider();
                            break;
                        default: break;
                    }

                    using (symmetricKey)
                    {
                        symmetricKey.GenerateKey();
                        symmetricKey.GenerateIV();

                        byte[] key = symmetricKey.Key;
                        byte[] IV = symmetricKey.IV;
                        try
                        {
                            using (FileStream fileStreamKey = File.Open(keyFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                fileStreamKey.Write(key, 0, key.Length);
                                fileStreamKey.Write(IV, 0, IV.Length);
                            }
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    }
                }
            }

            return false;
        }
    }

    /********************************************************************************************************************/

    // Checks whether given file contains a valid cryption key to be used with AES algorithm.

    internal static bool CheckKey(string keyFileName, HelperService.CryptionAlgorithm cryptionAlgorithm)
    {
        if (cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptKey || cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptPassword)
        {
            byte[] keyArray = new byte[256];

            if (keyFileName != String.Empty)
            {
                FileInfo f = new FileInfo(keyFileName);

                if (File.Exists(keyFileName))
                {
                    // Verifies if length of cryption key file is valid.
                    if (f.Length == 256 + initVect.Length)
                    {
                        // Open the stream and read the cryption key from it.
                        FileStream fileStreamKey = null;
                        using (fileStreamKey = File.Open(keyFileName, FileMode.Open, FileAccess.Read, FileShare.None))
                        {
                            fileStreamKey.Read(keyArray, 0, 256);

                            // Verifies if each byte in this possible cryption key file is unique (i.e. cryption key is valid).
                            for (int i = 0; i <= 255; i++)
                            {
                                int count = 0;
                                for (int j = 0; j <= 255; j++)
                                {
                                    if (keyArray[i] == keyArray[j])
                                    {
                                        count++;
                                    }
                                    if (count > 1)
                                    {
                                        return false;
                                    }
                                }
                            }
                            return true;
                        }
                    }
                    return false;
                }
                return false;
            }
            return false;
        }
        else
        {
            int keyFileSize = 0;

            switch (cryptionAlgorithm)
            {
                case HelperService.CryptionAlgorithm.RC2Key:
                    keyFileSize = 24;
                    break;
                case HelperService.CryptionAlgorithm.TripleDesKey:
                    keyFileSize = 32;
                    break;
                case HelperService.CryptionAlgorithm.AesKey:
                    keyFileSize = 48;
                    break;
                default: break;
            }

            if (keyFileName != String.Empty)
            {
                FileInfo f = new FileInfo(keyFileName);

                if (File.Exists(keyFileName))
                {
                    if (f.Length == keyFileSize)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            return false;
        }
    }

    /********************************************************************************************************************/

    // Loads cryption key from given file to be used with AES algorithm.
    // If cryption key file is not valid, returns an error message.
    // If no cryption key file is mentioned, default key is generated and loaded.

    internal static string LoadKey(string keyFileName, out byte[] key, out byte[] IV, HelperService.CryptionAlgorithm cryptionAlgorithm, string pwd = null)
    {
        key = new byte[1];
        IV = new byte[initVect.Length];

        if (cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptKey || cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptPassword)
        {
            key = new byte[256];

            if (string.IsNullOrEmpty(keyFileName))
            {
                for (int i = 0; i <= 255; i++)
                {
                    key[i] = (byte)(255 - i);
                }

                pwd = string.IsNullOrEmpty(pwd) ? saltVal + Encoding.ASCII.GetString(key) : (pwd + saltVal + Encoding.ASCII.GetString(key));

                IV = GetCryptedBytesFromStringAndPassword(initVect, pwd);
            }
            else
                try
                {
                    if (CheckKey(keyFileName, HelperService.CryptionAlgorithm.NeedleCryptKey) == true)
                    {
                        FileStream fileStreamKey = null;

                        // Open the stream and read the cryption key from it.
                        using (fileStreamKey = File.Open(keyFileName, FileMode.Open, FileAccess.Read, FileShare.None))
                        {
                            fileStreamKey.Read(key, 0, 256);
                            fileStreamKey.Read(IV, 0, initVect.Length);
                        }
                    }
                    else throw new Exception("Key file not valid.");
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

            return "Success";
        }
        else
        {
            string initVectStr = null;
            int keySize = 0;

            switch (cryptionAlgorithm)
            {
                case HelperService.CryptionAlgorithm.RC2Key:
                    key = new byte[16];
                    IV = new byte[8];
                    initVectStr = initVect.Substring(0, 8);
                    keySize = 128;
                    break;
                case HelperService.CryptionAlgorithm.RC2Password:
                    initVectStr = initVect.Substring(0, 8);
                    keySize = 128;
                    break;
                case HelperService.CryptionAlgorithm.TripleDesKey:
                    key = new byte[24];
                    IV = new byte[8];
                    initVectStr = initVect.Substring(0, 8);
                    keySize = 192;
                    break;
                case HelperService.CryptionAlgorithm.TripleDesPassword:
                    initVectStr = initVect.Substring(0, 8);
                    keySize = 192;
                    break;
                case HelperService.CryptionAlgorithm.AesKey:
                    key = new byte[32];
                    IV = new byte[16];
                    initVectStr = initVect;
                    keySize = 256;
                    break;
                case HelperService.CryptionAlgorithm.AesPassword:
                    initVectStr = initVect;
                    keySize = 256;
                    break;
                default: break;
            }

            if (!string.IsNullOrEmpty(keyFileName))
            {
                if (CheckKey(keyFileName, cryptionAlgorithm) == true)
                {
                    try
                    {
                        using (FileStream fileStreamKey = File.Open(keyFileName, FileMode.Open, FileAccess.Read, FileShare.None))
                        {
                            fileStreamKey.Read(key, 0, key.Length);
                            fileStreamKey.Read(IV, 0, IV.Length);
                        }
                        return "Success";
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }
                else return "Cryption key file not valid.";
            }
            else
            {
                pwd = string.IsNullOrEmpty(pwd) ? saltVal : pwd;

                // Below we convert initVector & saltValue strings into byte arrays.
                // Below it is assumed that strings contain ASCII codes only.
                // If strings include Unicode characters, use Unicode, UTF7, or UTF8 encoding.
                byte[] initVectorBytes = GetCryptedBytesFromStringAndPassword(initVectStr, pwd);
                byte[] saltValueBytes = GetCryptedBytesFromStringAndPassword(saltVal, pwd);

                Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(pwd, initVectorBytes, passwordIterations);
                IV = password.GetBytes(initVectorBytes.Length);

                CheckArgumentsCryptWithPwd(cryptionAlgorithm, pwd, saltVal, passwordIterations, IV, keySize);

                // Now we create a password, from which the key will be derived.
                // This password will be generated from the specified passphrase and salt value. 
                // The password will be created using the specified hash algorithm.
                // Password creation can be done in several iterations.
                password = new Rfc2898DeriveBytes(pwd, saltValueBytes, passwordIterations);

                // Use the password to generate pseudo-random bytes for the encryption key.
                // Specify the size of the key in bytes (instead of bits).
                key = password.GetBytes(keySize / 8);

                

                return "Success";
            }
        }
    }
}

/********************************************************************************************************************/
