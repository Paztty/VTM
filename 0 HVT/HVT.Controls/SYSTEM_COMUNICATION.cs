using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.Controls
{
    public static class SYSTEM_COMUNICATION
    {
        public static byte Prefix1 = 0x44;
        public static byte Prefix2 = 0x45;
        public static byte Suffix = 0x56;

        //public static byte[] GetFrame(byte[] datas)
        //{

        //    if (datas == null) return null;

        //    List<byte> dataToSend = datas.ToList();
        //    if (datas.Length > 1)
        //    {
        //        dataToSend.Insert(0, (byte)(dataToSend.Count + 1));
        //    }
        //    else
        //    {
        //        dataToSend.Add(0x00);
        //    }    
        //    dataToSend.Insert(0, Prefix2);
        //    dataToSend.Insert(0, Prefix1);
        //    var checksum = CheckSum.Get(dataToSend.ToArray(), CheckSumType.XOR);
        //    dataToSend.Add(checksum);
        //    dataToSend.Add(Suffix);

        //    //dataToSend.Insert(2, (Byte)(dataToSend.Count - 3) );
        //    foreach (var item in dataToSend)
        //    {
        //        Console.Write(item.ToString("X2") + " ");
        //    }
        //    Console.WriteLine(" ");
        //    return dataToSend.ToArray();
        //}

        public static byte[] GetFrame(byte[] datas, bool IsNoSize = false)
        {

            if (datas == null) return null;

            List<byte> dataToSend = datas.ToList();
            if (!IsNoSize)
            {
                if (datas.Length > 1)
                {
                    dataToSend.Insert(0, (byte)(dataToSend.Count + 1));
                }
                else
                {
                    dataToSend.Add(0x00);
                }
            }
            dataToSend.Insert(0, Prefix2);
            dataToSend.Insert(0, Prefix1);
            var checksum = CheckSum.Get(dataToSend.ToArray(), CheckSumType.XOR);
            dataToSend.Add(checksum);
            dataToSend.Add(Suffix);

            //dataToSend.Insert(2, (Byte)(dataToSend.Count - 3) );
            foreach (var item in dataToSend)
            {
                Console.Write(item.ToString("X2") + " ");
            }
            Console.WriteLine(" ");
            return dataToSend.ToArray();
        }

        public static bool GetResponse(byte[] datas, byte[] compare)
        {
            if (datas == null) return false;

            List<byte> dataToSend = datas.ToList();

            dataToSend.Insert(0, Prefix2);
            dataToSend.Insert(0, Prefix1);
            var checksum = CheckSum.Get(dataToSend.ToArray(), CheckSumType.XOR);
            dataToSend.Add(checksum);

            dataToSend.Add(Suffix);

            Console.WriteLine(" ");
            //dataToSend.Insert(2, (Byte)(dataToSend.Count - 3) );
            //foreach (var item in dataToSend)
            //{
            //    Console.Write(item.ToString("X2") + " ");
            //}
            //Console.WriteLine(" ");
            //foreach (var item in compare)
            //{
            //    Console.Write(item.ToString("X2") + " ");
            //}

            var result = true;
            var byteData = dataToSend.ToArray();
            for (int i = 0; i < compare.Length; i++)
            {
                if (i < byteData.Length)
                {
                    result = byteData[i] == compare[i];
                }
            }
            return result;
        }
    }
}
