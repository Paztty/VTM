using System;
namespace crc8r
{
    public class Program
    {
        static void Main()
        {
            string input = "00005678";
            var buf = HexToBytes(input);
            string hex = ModRTU_CRC(buf,4).ToString("X2");
            Console.WriteLine(hex);
        }

        static byte[] HexToBytes(string input)
        {
            byte[] result = new byte [input.Length/2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToByte(input.Substring(2 * i, 2),16);
            }
            return result;
        }
        public static UInt16 ModRTU_CRC(byte[] buf, int len)
        {
            UInt16 crc = 0xFFFF;

            for (int pos = 0; pos < len; pos++)
            {
                crc ^= (UInt16)buf[pos];          // XOR byte into least sig. byte of crc

                for (int i = 8; i != 0; i--)
                {    // Loop over each bit
                    if ((crc & 0x0001) != 0)
                    {      // If the LSB is set
                        crc >>= 1;                    // Shift right and XOR 0xA001
                        crc ^= 0xA001;
                    }
                    else                            // Else LSB is not set
                        crc >>= 1;                    // Just shift right
                }
            }
            // Note, this number has low and high bytes swapped, so use it accordingly (or swap bytes)
            return crc;
        }



        

    }
}
