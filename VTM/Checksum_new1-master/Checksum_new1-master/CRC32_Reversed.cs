
using System;

namespace ionic.utils.zip
{
    public class Program
    {
        static void Main()
        {
            CRC32 crc32 = new CRC32();
            //string input = "1234567890";
            byte[] input = { 0x45 };
                                                            //var buf = HexToBytes(input);
            string hex = crc32.GetCrc32FromByteArr(in input).ToString("X2");
            Console.WriteLine(hex);
            Console.WriteLine("End");
            Console.ReadLine();
        }
        static byte[] HexToBytes(string input)
        {
            byte[] result = new byte[input.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (byte)Convert.ToByte(input.Substring(2 * i, 2), 16);
            }
            return result;
        }
        public class CRC32
        {
            private UInt32[] crc32Table;
            private const int BUFFER_SIZE = 8192;

            private Int32 _TotalBytesRead = 0;
            public Int32 TotalBytesRead
            {
                get
                {
                    return _TotalBytesRead;
                }
            }
            public UInt32 GetCrc32(System.IO.Stream input)
            {
                return GetCrc32AndCopy(input, null);
            }
            public UInt32 GetCrc32AndCopy(System.IO.Stream input, System.IO.Stream output)
            {
                unchecked
                {
                    UInt32 crc32Result;
                    crc32Result = 0xFFFFFFFF;
                    byte[] buffer = new byte[BUFFER_SIZE];
                    int readSize = BUFFER_SIZE;

                    _TotalBytesRead = 0;
                    int count = input.Read(buffer, 0, readSize);
                    if (output != null) output.Write(buffer, 0, count);
                    _TotalBytesRead += count;
                    while (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            crc32Result = ((crc32Result) >> 8) ^ crc32Table[(buffer[i]) ^ ((crc32Result) & 0x000000ff)];
                        }
                        count = input.Read(buffer, 0, readSize);
                        if (output != null) output.Write(buffer, 0, count);
                        _TotalBytesRead += count;
                    }

                    return ~crc32Result;
                }
            }
            public UInt32 GetCrc32FromByteArr(in byte[] input)
            {
                UInt32 crc32Result;
                crc32Result = 0xFFFFFFFF;
                byte[] buffer = input;
                int count = input.Length;

                for (int i = 0; i < count; i++)
                {
                    crc32Result = ((crc32Result) >> 8) ^ crc32Table[(buffer[i]) ^ ((crc32Result) & 0x000000ff)];
                }
                return ~crc32Result;
            }
            public CRC32()
            {
                unchecked
                {
                    UInt32 dwPolynomial = 0xEDB88320;
                    UInt32 i, j;
                    crc32Table = new UInt32[256];
                    UInt32 dwCrc;
                    for (i = 0; i < 256; i++)
                    {
                        //uint dwCrc = (uint)(i << 24);
                        dwCrc = i;
                        for (j = 8; j > 0; j--)
                        {
                            if ((dwCrc & 1) == 1)
                            {
                                dwCrc = (dwCrc >> 1) ^ dwPolynomial;
                            }
                            else
                            {
                                dwCrc >>= 1;
                            }
                        }
                        crc32Table[i] = dwCrc;
                       // Console.WriteLine(crc32Table[i].ToString("X2"));

                    }
                }
            }
        }
    }
}

