using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.VTM.Base
{
    public class Operations
    {
        public bool NoTest_PressUp { get; set; } = true;
        public int StartDelaytime { get; set; } = 500;
        public int TestPressUpTime { get; set; } = 500;

        public bool FailContinue { get; set; } = false;
        public bool FailStopAll { get; set; } = true;
        public bool FailStopPCB { get; set; } = false;

        public bool FailResistanceStopAll { get; set; } = true;
        public int ErrorJumpCount { get; set; } = 3;


        public bool SaveFailPCB { get; set; } = true;
        public bool UsePre_endSignal { get; set; } = true;


        public int RetryCount { get; set; } = 2;
        public bool PassSkipPCB { get; set; } = true;
        public bool UseRetryUpdown { get; set; } = false;
        public int RetryUpdownTime { get; set; } = 500;

    }
}
