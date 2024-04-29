using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

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
            if (bytes.Count < 23) return false;
            double IrAt = 0.1;
            double UrAt = 0.1;

            var byteArray = bytes.ToArray();
            try
            {
                UU = BitConverter.ToInt16(byteArray, 4) * UrAt * 0.1;
                UW = BitConverter.ToInt16(byteArray, 6) * UrAt * 0.1;
                UV = BitConverter.ToInt16(byteArray, 8) * UrAt * 0.1;

                UUW = BitConverter.ToInt16(byteArray, 10) * UrAt * 0.1;
                UWV = BitConverter.ToInt16(byteArray, 12) * UrAt * 0.1;
                UVU = BitConverter.ToInt16(byteArray, 14) * UrAt * 0.1;

                IU = BitConverter.ToInt16(byteArray, 16) * IrAt * 0.001;
                IW = BitConverter.ToInt16(byteArray, 18) * IrAt * 0.001;
                IV = BitConverter.ToInt16(byteArray, 20) * IrAt * 0.001;
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
