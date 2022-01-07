using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.Utility
{
    public static class StringToByteArray
    {
        public static byte[] Convert(string Str)
        {
            if (Str.Length % 2 != 0)    //lấy phần dư khác 0
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "HexaDecimal cannot have an odd number of digits: {0}", Str)); //Hệ thập lục phân k có chữ số lẻ
            }

            byte[] hexByteArray = new byte[Str.Length / 2]; //độ dài chuỗi chia 2
            for (int index = 0; index < hexByteArray.Length; index++)     // 
            {
                string byteValue = Str.Substring(index * 2, 2);   // Trả về chuỗi mới được cắt từ vị trí StartIndex với số ký tự cắt là length từ chuỗi ban đầu ( cắt 2)

                hexByteArray[index] = byte.Parse(byteValue, NumberStyles.HexNumber); //tạo mảng hex
            }
            return hexByteArray;
        }
    }
}
