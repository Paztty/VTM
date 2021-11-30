using System;
using System.Globalization;

public class Program
{
    public static void Main() { 
    //suman first convert to byte[]
    byte[] bb = ConvertHexadecimalStringToByteArray("fa27da1547ff");    // số kí tự trong chuỗi phải là số chẵn
    //calculate checksum with the above byte[]
    Console.WriteLine(calculateCheckSum(bb));
        Console.WriteLine("Hex: {0:X}", calculateCheckSum(bb));

    }

//Following converts the hex to a byte[]
public static byte[] ConvertHexadecimalStringToByteArray(string hexadecimalString)
{
    if (hexadecimalString.Length % 2 != 0)    //lấy phần dư khác 0
    {
        throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "HexaDecimal cannot have an odd number of digits: {0}", hexadecimalString)); //Hệ thập lục phân k có chữ số lẻ
    }

    byte[] hexByteArray = new byte[hexadecimalString.Length / 2]; //độ dài chuỗi chia 2
    for (int index = 0; index < hexByteArray.Length; index++)     // 
    {
        string byteValue = hexadecimalString.Substring(index * 2, 2);   // Trả về chuỗi mới được cắt từ vị trí StartIndex với số ký tự cắt là length từ chuỗi ban đầu ( cắt 2)

           

            hexByteArray[index] = byte.Parse(byteValue, NumberStyles.HexNumber); //tạo mảng hex

                       
            Console.WriteLine("index " + hexByteArray[index]);
            Console.WriteLine("Hex: {0:X}", hexByteArray[index]);




        }

    return hexByteArray;
}

//calculates the checksum from the byte[]
private static byte calculateCheckSum(byte[] byteData) //
{
    Byte chkSumByte = 0x00;
    for (int i = 0; i < byteData.Length; i++)
        chkSumByte ^= byteData[i]; // 00000000 XOR 00000010= 00000010, 00000010 XOR 00000011 =00000001,  00000001 XOR 00000100= 00000101, 00000101 XOR 00000101=0000000 => kết quả bằng 0
        return chkSumByte;
        // Giá trị là ở dạng thập phân
}



}

