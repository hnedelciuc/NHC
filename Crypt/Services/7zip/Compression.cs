using System;
using System.IO;
using SevenZip;
using SevenZip.Compression;

namespace SevenZip.Compression
{
    class LZMACoder : IDisposable
    {
        private bool isDisposed = false;

        //These properties are straight copy from the SDK.
        //Actually, I don't know what these mean.

        private static Int32 dictionary = MainService.blockSize; //1 << 21; //No dictionary
        private static Int32 posStateBits = 2;
        private static Int32 litContextBits = 3;   // for normal files  // UInt32 litContextBits = 0; // for 32-bit data                                             
        private static Int32 litPosBits = 0;       // UInt32 litPosBits = 2; // for 32-bit data
        private static Int32 algorithm = 2;        // Horia: LzmaEncoder.cs file was tweaked to enable algorithm prop according to Igor Pavlov's comments
        private static Int32 numFastBytes = 32;    // Horia: was 128
        private static bool eos = false;
        private static string mf = "bt2";          // Horia: was "bt4"

        private static CoderPropID[] propIDs =
        {
        CoderPropID.DictionarySize,
        CoderPropID.PosStateBits,
        CoderPropID.LitContextBits,
        CoderPropID.LitPosBits,
        CoderPropID.Algorithm,
        CoderPropID.NumFastBytes,
        CoderPropID.MatchFinder,
        CoderPropID.EndMarker
    };
        private static object[] properties =
        {
        (Int32)(dictionary),
        (Int32)(posStateBits),
        (Int32)(litContextBits),
        (Int32)(litPosBits),
        (Int32)(algorithm),
        (Int32)(numFastBytes),
        mf,
        eos
    };

        internal LZMACoder()
        {
            if (BitConverter.IsLittleEndian == false)
            {
                Dispose();
                throw new Exception("Not implemented");
            }
        }

        internal MemoryStream Decompress(MemoryStream inStream)
        {
            return Decompress(inStream, false);
        }

        internal MemoryStream Decompress(MemoryStream inStream, bool closeInStream)
        {
            inStream.Position = 0;
            MemoryStream outStream = new MemoryStream();

            byte[] properties = new byte[5];
            if (inStream.Read(properties, 0, 5) != 5)
                throw (new Exception("input .lzma is too short"));

            SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();
            decoder.SetDecoderProperties(properties);

            long outSize = 0;

            if (BitConverter.IsLittleEndian)
            {
                for (int i = 0; i < 8; i++)
                {
                    int v = inStream.ReadByte();
                    if (v < 0)
                        throw (new Exception("Can't Read 1"));

                    outSize |= ((long)(byte)v) << (8 * i);
                }
            }

            long compressedSize = inStream.Length - inStream.Position;
            decoder.Code(inStream, outStream, compressedSize, outSize, null);

            if (closeInStream)
                inStream.Close();

            return outStream;
        }

        internal MemoryStream Compress(MemoryStream inStream)
        {
            return Compress(inStream, false);
        }

        internal MemoryStream Compress(MemoryStream inStream, bool closeInStream)
        {
            inStream.Position = 0;
            Int64 fileSize = inStream.Length;
            MemoryStream outStream = new MemoryStream();

            SevenZip.Compression.LZMA.Encoder encoder = new SevenZip.Compression.LZMA.Encoder();
            encoder.SetCoderProperties(propIDs, properties);
            encoder.WriteCoderProperties(outStream);

            if (BitConverter.IsLittleEndian)
            {
                byte[] LengthHeader = BitConverter.GetBytes(fileSize);
                outStream.Write(LengthHeader, 0, LengthHeader.Length);
            }

            encoder.Code(inStream, outStream, -1, -1, null);

            if (closeInStream)
                inStream.Close();

            return outStream;
        }

        ~LZMACoder()
        {
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.isDisposed == false)
            {
                if (disposing)
                {
                    //Console.WriteLine("dispose"); 
                    GC.SuppressFinalize(this);
                }
            }
            this.isDisposed = true;
        }
    }
}