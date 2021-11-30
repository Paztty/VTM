using System;
using System.Globalization;
static class Program
{
    static void Main()
    {
        while (true)
        {
            Console.Write("Hex input: ");
            string input = Console.ReadLine();

            var bytes = HexToBytes(input);
            foreach (var item in bytes)
            {
                Console.Write(item);
            }
            Console.WriteLine();
            string hex = Crc16.ComputeChecksum(bytes).ToString("x2"); // định dạng chuỗi theo hệ thập lục phân
            Console.WriteLine( "CRC 16: " +hex);
            if (Console.ReadKey().Key == ConsoleKey.Escape)
            {
                break;
            }
        }

    }
    static byte[] HexToBytes(string input)
    {
        byte[] result = new byte[input.Length / 2];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = Convert.ToByte(input.Substring(2 * i, 2), 16); 
        }
        return result;
    }
    public static class Crc16
    {
        const ushort polynomial = 0xA001; //1010 0000 0000 0001
        static readonly ushort[] table = new ushort[256]; //mảng table chứa 256 phần tử
        public static ushort ComputeChecksum(byte[] bytes)   // mảng bytes 
        {
            ushort crc = 0;
            for (int i = 0; i < bytes.Length; ++i)   // length = 2
            {                  
                byte index = (byte)(crc ^ bytes[i]);
                var fun = crc >> 8;
                Console.WriteLine(fun.ToString("X") + "---" + crc.ToString("X") + "---" + index.ToString("X") + "---" + table[index].ToString("X"));
                crc = (ushort)((crc >> 8) ^ table[index]);
            }
            return crc;
        }
        static Crc16()
        {
            ushort value;
            ushort temp;
            for (ushort i = 0; i < table.Length; ++i) //  table.Length= 256
            {
                value = 0;
                temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
                Console.WriteLine(i.ToString("X") + "---" + value.ToString("X"));
            }
        }
    }
}

//'https://stackoverflow.com/questions/22860356/how-to-generate-a-crc-16-from-c-sharp'