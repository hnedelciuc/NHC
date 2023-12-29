/********************************************************************************************************************
/ Needle in a Haystack in a Crypt v1.0.
/ Copyright (C) 2016-2023 by Horia Nedelciuc from Chisinau, Moldova.
/********************************************************************************************************************
/ Compression Service.
/ Zip-like compression using .NET's Deflate algorithm.
/ 7-Zip-like compression using Igor Pavlov's LZMA algorithm.
/********************************************************************************************************************/

using System;
using System.IO;
using System.IO.Compression;

class CompressionService
{
    /********************************************************************************************************************/

    // Compresses a given byte array using Deflate algorithm at desired compression level and returns result as byte array.

    internal static byte[] DeflateCompressArray(byte[] data, CompressionLevel compressionLevel)
    {
        using (var compressedStream = new MemoryStream())
        {
            using (var compressingStream = new DeflateStream(compressedStream, compressionLevel))
            {
                compressingStream.Write(data, 0, data.Length);
            }

            var result = compressedStream.ToArray();

            if (result.Length == 0)
                throw new Exception("Compress error.");

            return result;
        }
    }

    /********************************************************************************************************************/

    // Decompresses a given byte array using Deflate algorithm and returns result as byte array.

    internal static byte[] DeflateDecompressArray(byte[] data)
    {
        using (var compressedStream = new MemoryStream(data))
        {
            using (var decompressingStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            {
                using (var decompressedStream = new MemoryStream())
                {
                    decompressingStream.CopyTo(decompressedStream);
                    return decompressedStream.ToArray();
                }
            }
        }
    }

    /********************************************************************************************************************/

    // Compresses header using Deflate algorithm and returns it as byte array.

    internal static byte[] DeflateCompressHeader(byte[] header)
    {
        return DeflateCompressArray(header, CompressionLevel.Optimal);
    }

    /********************************************************************************************************************/

    // Decompresses byte array header using Deflate algorithm and returns it as byte array.

    internal static byte[] DeflateDecompressHeader(byte[] header)
    {
        return DeflateDecompressArray(header);
    }

    /********************************************************************************************************************/

    // Compresses a given byte array using LZMA algorithm at desired compression level and returns result as byte array.

    internal static byte[] LZMACompressArray(byte[] data)
    {
        //MemoryStream inStream = new MemoryStream(data);
        //MemoryStream outStream = new MemoryStream();

        //SevenZip.Compression.LZMA.Encoder encoder = new SevenZip.Compression.LZMA.Encoder();
        //encoder.WriteCoderProperties(outStream);
        //outStream.Write(BitConverter.GetBytes(inStream.Length), 0, 2);

        //encoder.Code(inStream, outStream, inStream.Length, -1, null);

        //var result = outStream.ToArray();

        //inStream.Dispose();

        var coder = new SevenZip.Compression.LZMACoder();
        var compressedStream = coder.Compress(new MemoryStream(data), true);
        return compressedStream.ToArray();

        //return SevenZip.Compression.LZMA.SevenZipHelper.Compress(data);
    }

    /********************************************************************************************************************/

    // Decompresses a given byte array using LZMA algorithm and returns result as byte array.

    internal static byte[] LZMADecompressArray(byte[] data)
    {
        //MemoryStream inStream = new MemoryStream(data);
        //MemoryStream outStream = new MemoryStream();

        //byte[] properties = new byte[5];
        //if (inStream.Read(properties, 0, 5) != 5)
        //    throw (new Exception("Input LZMA is too short"));
        //SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();
        //decoder.SetDecoderProperties(properties);

        //byte[] outSizeBytes = new byte[2];
        //inStream.Read(outSizeBytes, 0, 2);
        //long outSize = BitConverter.ToInt16(outSizeBytes, 0);

        //long compressedSize = inStream.Length - inStream.Position;
        //decoder.Code(inStream, outStream, inStream.Length, outSize, null);

        //var byteArray = new byte[outSize];
        //outStream.Read(byteArray, 0, (int)outSize);

        //outStream.Close();
        //inStream.Close();

        var coder = new SevenZip.Compression.LZMACoder();
        var decodedStream = coder.Decompress(new MemoryStream(data), true);
        return decodedStream.ToArray();
        
        //return SevenZip.Compression.LZMA.SevenZipHelper.Decompress(data);
    }

    /********************************************************************************************************************/

    // Compresses header using LZMA algorithm and returns it as byte array.

    internal static byte[] LZMACompressHeader(byte[] header)
    {
        MemoryStream inStream = new MemoryStream(header);
        MemoryStream outStream = new MemoryStream();

        SevenZip.Compression.LZMA.Encoder encoder = new SevenZip.Compression.LZMA.Encoder();
        encoder.WriteCoderProperties(outStream);
        outStream.Write(BitConverter.GetBytes(inStream.Length), 0, 8);

        encoder.Code(inStream, outStream, inStream.Length, -1, null);
        outStream.Flush();
        outStream.Close();
        inStream.Close();

        return outStream.ToArray();
    }

    /********************************************************************************************************************/

    // Decompresses byte array header using LZMA algorithm and returns it as byte array.

    internal static byte[] LZMADecompressHeader(byte[] header_block)
    {
        Stream inStream = new MemoryStream(header_block);
        Stream outStream = new MemoryStream();

        byte[] properties = new byte[5];
        if (inStream.Read(properties, 0, 5) != 5)
            throw (new Exception("Input LZMA is too short"));
        byte[] outSizeBytes = new byte[8];
        inStream.Read(outSizeBytes, 0, 8);
        long outSize = BitConverter.ToInt64(outSizeBytes, 0);

        SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();
        decoder.SetDecoderProperties(properties);
        
        long compressedSize = inStream.Length - inStream.Position;
        decoder.Code(inStream, outStream, compressedSize, outSize, null);

        var byteArray = new byte[outSize];
        outStream.Read(byteArray, 0, (int)outSize);

        outStream.Flush();
        outStream.Close();
        inStream.Close();

        return byteArray;
    }

}

internal class CDoubleStream : Stream
{
    internal System.IO.Stream s1;
    internal System.IO.Stream s2;
    internal int fileIndex;
    internal long skipSize;

    public override bool CanRead { get { return true; } }
    public override bool CanWrite { get { return false; } }
    public override bool CanSeek { get { return false; } }
    public override long Length { get { return s1.Length + s2.Length - skipSize; } }
    public override long Position
    {
        get { return 0; }
        set { }
    }
    public override void Flush() { }
    public override int Read(byte[] buffer, int offset, int count)
    {
        int numTotal = 0;
        while (count > 0)
        {
            if (fileIndex == 0)
            {
                int num = s1.Read(buffer, offset, count);
                offset += num;
                count -= num;
                numTotal += num;
                if (num == 0)
                    fileIndex++;
            }
            if (fileIndex == 1)
            {
                numTotal += s2.Read(buffer, offset, count);
                return numTotal;
            }
        }
        return numTotal;
    }
    public override void Write(byte[] buffer, int offset, int count)
    {
        throw (new Exception("can't Write"));
    }
    public override long Seek(long offset, System.IO.SeekOrigin origin)
    {
        throw (new Exception("can't Seek"));
    }
    public override void SetLength(long value)
    {
        throw (new Exception("can't SetLength"));
    }
}

/********************************************************************************************************************/
