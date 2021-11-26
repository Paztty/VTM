using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HVT.VTM.Base
{
    public enum Mode
    {
        NORMAL,
        R_WAIT,
        SEND_R,
        TIMER
    }

    public enum CMDs
    {
        NON,
        PWR,
        DLY,
        GEN,
        LOD,
        RLY,
        //FRY,
        MAK,
        //MSG,
        //RCO,
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
        //UTD,
        //UTR,
        //UPM,
        //MAT,
        //RMC,
        //MCH,
        //RMD,
        //DCH,
        //RBZ,
        //BCH,
        STL,
        EDL,
        LCC,
        LEC,
        //LSQ,
        //LTM,
        //STD,
        //EDD,
        //DCC,
        //DEC,
        //DSQ,
        //DTM,
        //CMT,
        CAL,
        GLED,
        FND,
        LED,
        LCD,
        //PPA,
        //RMT,
        //DFR,
        //DPH,
        //DCU,
        //DVT,
        //DCD,
        //DIM,
        //DIR,
        //OTS,
        //OTP,
        PCB
    }
    public class CommandDescriptions : INotifyPropertyChanged
    {
        //
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        // static list string parameter
        public static List<string> CommandMode_UUT = new List<string>()
        {
        "NORMAL",
        "R_WAIT",
        "SEND_R",
        "TIMER"
        };

        public static List<string> CommandMode_DMM = new List<string>()
        {
        "SPEC",
        "CONT",
        "MIN",
        "MAX"
        };

        public static List<string> CommandMode_DMMresol = new List<string>()
        {
        "FAST",
        "MID",
        "SLOW",
        };

        public static List<string> CommandMode_PowerKIND = new List<string>()
        {
        "220VAC",
        "110VAC",
        "25VDC",
        "3.3VDC"
        };

        public static List<string> CommandMode_DMM_DCVrange = new List<string>()
        {
            "100 mV",
            "1V",
            "10V",
            "100V",
            "1000V"
        };
        public static List<string> CommandMode_DMM_ACVrange = new List<string>()
        {
            "100 mV",
            "1V",
            "10V",
            "100V",
            "750V"
        };
        public static List<string> CommandMode_DMM_RESrange = new List<string>()
        {
            "100 Ohm",
            "1 kOhm",
            "10 kOhm",
            "100 kOhm",
            "1 MOhm",
            "10 MOhm",
            "100 MOhm",
        };

        public static List<string> CommandMode_UUT_Port = new List<String>()
        {
            "P1",
            "P2"
        };

        public static List<string> TXnaming = new List<string>();
        public static List<string> RXnaming = new List<string>();
        public static List<string> QRnaming
        {
            get;
            set;
        } = new List<string>();

        public string No { get; set; } = "{Number of step}";
        public string IMQSCode { get; set; } = "Code";
        public string TestContent { get; set; } = "Content of step";
        private List<string> testContentsList { get; set; }
        public List<string> TestContentsList {
            get { return testContentsList; }
            set {
                if (testContentsList != value)
                {
                    testContentsList = value;
                    NotifyPropertyChanged("TestContentsList");
                }
            }
        }

        public CMDs CMD { get; set; }
        public List<string> CMDList { get; set; } = Enum.GetNames(typeof(CMDs)).ToList();

        public string Condition1 { get; set; } = "not use";
        public bool IsListCondition1 { get; set; } = false;
        private List<string> condition1List;
        public List<string> Condition1List {
            get {
                return condition1List; 
            }
            set { 
                condition1List = value;
                IsListCondition1 = true;
                NotifyPropertyChanged("Condition1");
            }
        }
        public string Oper { get; set; } = "not use";
        public bool IsListOper { get; set; } = false;
        public List<string> OperList { get; set; }

        private List<string> condition2List;
        public string Condition2 { get; set; } = "not use";
        public bool IsListCondition2 { get; set; } = false; 
        public List <string> Condition2List
        {
            get { return condition2List; }
            set
            {
                if (condition2List != value)
                {
                    condition2List = value;
                    NotifyPropertyChanged("Condition2List");
                }
            }
        }



        public string Spect { get; set; } = "not use";
        public bool IsListSpect { get; set; } = false;  
        public List<string> SpectList { get; set; }

        public string Min { get; set; } = "not use";
        public bool IsListMin { get; set; } = false;
        public List<string> MinList { get; set; }   

        public string Max { get; set; } = "not use";
        public bool IsListMax { get; set; } = false;
        public List<string> MaxList { get; set; }

        public string Mode { get; set; } = "not use";
        public bool IsListMode { get; set; } = false;
        public List<string> ModeList { get; set; }

        public string Count { get; set; } = "not use";
        public string EJump { get; set; } = "{Step Jump}";
        public string Remark { get; set; } = "{Remark Step}";
        public string ELoc { get; set; } = "Eloc";
        public string Skip { get; set; } = "Skip";


        public string Description { get; set; }
    }
}
