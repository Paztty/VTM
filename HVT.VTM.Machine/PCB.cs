using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.VTM.Machine
{
    public class TestPCB
    {
        public string Name { get; set; }
        public string Barcode { get; set; }
        public bool   IsWaitTest { get; set; }
        public List<PCBTestValue> TestValues { get; set; }
    }
}
