using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.VTM.Base.Machine
{
    public static class SYSTEM_COMUNICATION
    {
        private static byte Prefix1 = 0x44;
        private static byte Prefix2 = 0x45;
        private static byte Suffix = 0x56;

        public static byte[] GetFrame(byte[] datas)
        {

            if (datas == null) return null;

            List<byte> dataToSend = datas.ToList();

            dataToSend.Insert(0, (byte)(dataToSend.Count +1 ));
            var checksum = CheckSum.Get(dataToSend.ToArray(), CheckSumType.XOR);
            dataToSend.Add(checksum);

            dataToSend.Insert(0, Prefix2);
            dataToSend.Insert(0, Prefix1);
            dataToSend.Add(Suffix);

            //dataToSend.Insert(2, (Byte)(dataToSend.Count - 3) );
            foreach (var item in dataToSend)
            {
                Console.Write(item.ToString("X2") + " ");
            }
            return dataToSend.ToArray();
        }
    }


    public static class Machine
    {


    }
}
