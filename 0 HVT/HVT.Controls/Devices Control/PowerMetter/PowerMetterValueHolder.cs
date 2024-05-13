using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.Controls.Devices_Control
{
    public class PowerMettterValueHolder
    {
        public double UU;
        public double UW;
        public double UV;

        public double UUW;
        public double UWV;
        public double UVU;

        public double IU;
        public double IW;
        public double IV;

        public void ClearValue()
        {
            UU = 0;
            UW = 0;
            UV = 0;

            UUW = 0;
            UWV = 0;
            UVU = 0;

            IU = 0;
            IW = 0;
            IV = 0;
        }
        public bool GetValue(List<byte> bytes)
        {
            if (bytes.Count < 19) return false;

            var byteArray = bytes.ToArray();
            var DPT = byteArray[3];
            var DCT = byteArray[4];

            try
            {
                UU = BitConverter.ToInt16(new byte[2] { byteArray[8], byteArray[7] }, 0) / 10000.0 * (Math.Pow(10, DPT));
                UW = BitConverter.ToInt16(new byte[2] { byteArray[10], byteArray[9] }, 0) / 10000.0 * (Math.Pow(10, DPT));
                UV = BitConverter.ToInt16(new byte[2] { byteArray[12], byteArray[11] }, 0) / 10000.0 * (Math.Pow(10, DPT));

                UUW = UU - UW;
                UWV = UW - UV;
                UVU = UV - UU;

                //IU = BitConverter.ToInt16(new byte[2] { byteArray[20], byteArray[19] }, 0) / 10000.0 * (Math.Pow(10, DCT));
                //IW = BitConverter.ToInt16(new byte[2] { byteArray[22], byteArray[21] }, 0) / 10000.0 * (Math.Pow(10, DCT));
                //IV = BitConverter.ToInt16(new byte[2] { byteArray[24], byteArray[23] }, 0) / 10000.0 * (Math.Pow(10, DCT));
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
