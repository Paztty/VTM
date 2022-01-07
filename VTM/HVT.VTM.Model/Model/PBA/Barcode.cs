using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.VTM.Base
{
    public class Barcode
    {
        public int No { get; set; }
        public string BarcodeData { get; set; }
        public int Lenght { get; set; }
        public int StartModelCodePosition { get; set; }
        public string ModelCode { get; set; }

        public bool Check()
        {
            return (BarcodeData.IndexOf(ModelCode) == StartModelCodePosition - 1);
        }

    }
}
