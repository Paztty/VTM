using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.VTM.Base
{
    public enum CheckSumType
    {
        XOR,
        CRC8,
        CRC16,
        CRC16_CCITT,
        CRC16_MOSBUS,
        CRC32,
        CRC8_REVERSED,
        SUM
    }
    public static class CheckSum
    {
        public static byte Get(byte[] bytes, CheckSumType type)
        {
            switch (type)
            {
                case CheckSumType.XOR:
                    return XOR.calculateCheckSum(bytes);
                case CheckSumType.CRC8:
                    break;
                case CheckSumType.CRC16:
                    break;
                case CheckSumType.CRC16_CCITT:
                    break;
                case CheckSumType.CRC16_MOSBUS:
                    break;
                case CheckSumType.CRC32:
                    break;
                case CheckSumType.CRC8_REVERSED:
                    break;
                case CheckSumType.SUM:
                    break;
                default:
                    break;
            }

            return 0x00;
        }
    }

    public static class XOR
    {
        public static byte calculateCheckSum(byte[] byteData) //
        {
            Byte chkSumByte = 0x00;
            for (int i = 0; i < byteData.Length; i++)
                chkSumByte ^= byteData[i]; // 00000000 XOR 00000010= 00000010, 00000010 XOR 00000011 =00000001,  00000001 XOR 00000100= 00000101, 00000101 XOR 00000101=0000000 => kết quả bằng 0
            return chkSumByte;
            // Giá trị là ở dạng thập phân
        }
    }

}
