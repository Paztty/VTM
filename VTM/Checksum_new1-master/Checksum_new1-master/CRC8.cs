
//CheckSum8 2s Complement

using System;
namespace crc8r
{
    public class Program
    {
        static void Main()
        {
            string input = "45";
            var bytes = HexToBytes(input);
            string hex = Crc8.ComputeChecksum(bytes).ToString("X");
            Console.WriteLine(hex);
            // Console.ReadLine();
        }

        static byte[] HexToBytes(string input)
        {
            byte[] result = new byte[input.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToByte(input.Substring(2 * i, 2),16);
            }
            return result;
        }
        public static class Crc8
        {
            const ushort poly = 0x07; //...................................0x07

            static readonly ushort[] table = new ushort[256];
            public static byte ComputeChecksum(byte[] bytes)
            {
                byte crc = 0;
                if (bytes != null && bytes.Length > 0)
                {
                    foreach (byte b in bytes)
                    {
                    
                        crc = (byte)table [crc ^ b];
                    }
                }
                return crc;
            }

            static Crc8()
            {
                for (int i = 0; i < 256; ++i)
                {
                    int temp = i;
                    for (int j = 0; j < 8; ++j)
                    {
                        if ((temp & 0x80) != 0)
                        {
                            temp = (temp << 1) ^ poly;
                        }
                        else
                        {
                            temp <<= 1;
                        }
                    }
                    table[i] = (byte)temp;
                }
            }
        }
    }
}
