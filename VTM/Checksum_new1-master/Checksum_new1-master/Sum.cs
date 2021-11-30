using System;
namespace crc8r
{
    class Program
    {
       
        static void Main(string[] args)
        {
            byte [] arr = new byte[1] {25} ;
            byte sum = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                sum += arr[i];
                Console.WriteLine(sum); 
            }
            Console.WriteLine("Sum Final: " + sum);


            byte checksum = (byte)(sum % 256);

            //byte checksum = (byte) (0xff - sum);
            
            Console.WriteLine("Checksum " + checksum);

            

        }
    }
}

