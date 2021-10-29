using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.VTM.Base
{
    public class TxData
    {
        public int No { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
        public string Blank { get; set; }
        public string Remark { get; set; }

        public override string ToString()
        {
            string strReturn = No + "," + Name + "," + Data + "," + Blank + "," + Remark;
            return strReturn;
        }
    }

    
}
