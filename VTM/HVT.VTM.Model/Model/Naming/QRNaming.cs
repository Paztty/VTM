using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.VTM.Base
{
    public class QRData
    {
        public int No  {get; set;}
        public string Context { get; set; }
        public string Code { get; set; }

        public override string ToString()
        {
            return No + "," + Context + "," + Code;
        }
    }
}
