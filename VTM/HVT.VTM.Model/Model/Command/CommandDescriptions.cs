using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.VTM.Base
{
    public enum CMDs
    {
        NON,
        PWR,
        DLY,
        GEN,
        LOD,
        RLY,
        FRY,
        MAK,
        MSG,
        RCO,
        DIS,
        END,
        ACV,
        DCV,
        FRQ,
        RES,
        URD,
        UTN,
        UTX,
        UCN,
        UCP,
        UTD,
        UTR,
        UPM,
        MAT,
        RMC,
        MCH,
        RMD,
        DCH,
        RBZ,
        BCH,
        STL,
        EDL,
        LCC,
        LEC,
        LSQ,
        LTM,
        STD,
        EDD,
        DCC,
        DEC,
        DSQ,
        DTM,
        CMT,
        CAL,
        PPA,
        RMT,
        DFR,
        DPH,
        DCU,
        DVT,
        DCD,
        DIM,
        DIR,
        OTS,
        OTP,
        PCB
    }
    public class CommandDescriptions
    {

        public string No { get; set; } = "{Number of step}";
        public string IMQSCode { get; set; } = "Code";
        public string TestContent { get; set; } = "Content of step";

        public CMDs CMD { get; set; }
        public string Condition1 { get; set; } = "{not use}";
        public string Oper { get; set; } = "{not use}";
        public string Condition2 { get; set; } = "{not use}";
        public string Spect { get; set; } = "{not use}";
        public string Min { get; set; } = "{not use}";
        public string Max { get; set; } = "{not use}";
        public string Mode { get; set; } = "{not use}";
        public string Count { get; set; } = "{not use}";
        public string EJump { get; set; } = "{Step Jump}";
        public string Remark { get; set; } = "{Remark Step}";
        public string ELoc { get; set; } = "Eloc";
        public string Skip { get; set; } = "Skip";


        public string Description { get; set; }
    }
}
