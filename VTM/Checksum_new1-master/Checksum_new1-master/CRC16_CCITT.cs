using System;
using System.Globalization;


static class Program
{
    static void Main()
    {
        string input = "88aa";
        var bytes = HexToBytes(input);
        string hex = Crc16.Crc16Ccitt(bytes).ToString("x2");
        Console.WriteLine(hex);  
    }
    static byte[] HexToBytes(string input)
    {
        byte[] result = new byte[input.Length/2 ];
        for(int i = 0; i < result.Length; i++)
        {
            result[i] = Convert.ToByte(input.Substring(2 * i, 2), 16);
        }
        return result;
    }

    public static class Crc16
    {
        //const ushort polynomial = 0xA001;
        //static readonly ushort[] table = new ushort[256];
        public static ushort Crc16Ccitt(byte[] bytes)
        {
            const ushort poly = 0x1021;
            ushort[] table = new ushort[256];
            ushort initialValue = 0xffff;
            ushort temp, a;
            ushort crc = initialValue;
            for (int i = 0; i < table.Length; ++i)
            {
                temp = 0;
                a = (ushort)(i << 8);
                for (int j = 0; j < 8; ++j)
                {
                    if (((temp ^ a) & 0x8000) != 0)
                        temp = (ushort)((temp << 1) ^ poly);
                    else
                        temp <<= 1;
                    a <<= 1;
                }
                table[i] = temp;
            }
            for (int i = 0; i < bytes.Length; ++i)
            {
                crc = (ushort)((crc << 8) ^ table[((crc >> 8) ^ (0xff & bytes[i]))]);
            }
            return crc;
        }
    }
}