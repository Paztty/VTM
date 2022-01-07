using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.VTM.Base
{
    public class ETCSetting
    {
        public int MUXdelay_slow_RES { get; set; } = 500;
        public int MUXdelay_slow_DCV { get; set; } = 100;
        public int MUXdelay_slow_ACVFRQ { get; set; } = 50;

        public int MUXdelay_Mid_RES { get; set; } = 500;
        public int MUXdelay_Mid_DCV { get; set; } = 100;
        public int MUXdelay_Mid_ACVFRQ { get; set; } = 50;

        public int MUXdelay_Fast_RES { get; set; } = 500;
        public int MUXdelay_Fast_DCV { get; set; } = 100;
        public int MUXdelay_Fast_ACVFRQ { get; set; } = 50;

        public bool UseDischargeError { get; set; } = false;
        public bool UseDischargeConfig { get; set; } = false;
        public bool UseDischargeTestStart { get; set; } = false;
        public int DischargeTime { get; set; } = 500;
        public double DischargeVolt { get; set; } = 5.0;
    }
}
