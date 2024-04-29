using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.Utility
{
    public class SQCI
    {
        public string Site = "";
        public string SEHC_code { get; set; } = "SEC";
        public string InspectionDate { get; set; }
        public string PartCode { get; set;}
        public static string space1 = "[]";
        public string VenderCode { get; set; } = "DYSC";
        public string VenderMakerCode { get; set; } = "DAYELC";
        public static string space3 = "[][][]";
        public string QR;
        public static string space4 = "[][][][]";
        public string Rework = "";

        private int countItem = 0;
        public string CountItem { get { return countItem.ToString(); } }

        private List<SQCI_Item> _Items = new List<SQCI_Item>();
        public List<SQCI_Item> Items 
        {
            get { return _Items; }
            set 
            {
                if (_Items != value)
                {
                    _Items = value;
                    countItem = value.Count;
                }   
            }
        }

        public string FinalResult;

        public override string ToString()
        {
            countItem = Items.Count;

            string str = SEHC_code + "]["
                + InspectionDate + "]["
                + PartCode + "][]["
                + VenderCode + "]["
                + VenderMakerCode + "][][][]["
                + QR + "][][" 
                + Rework + "][][]["
                + CountItem + "]";
            FinalResult = "PASS";

            foreach (var item in _Items)
            { 
                str += item.ToString();
                if (item.ResultTest == "NG")
                {
                    FinalResult = "FAIL";
                }
            }
            str += ("[" + FinalResult);
            return str;
        }


        public void SaveConfig()
        {
            Extensions.SaveToFile(this, "\\SQCI_AGENT_CONFIG.cfg");
        }
        public void Load()
        { 
            
        }

        public void AppendToFile(string FileName = "C:\\SQCI_AGENT_CTF\\")
        {
            if (!Directory.Exists(FileName)) Directory.CreateDirectory(FileName);
            File.AppendAllText(FileName + "VacuumTester_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv", this.ToString() + "\r\n");
            Console.WriteLine(this.ToString());
        }

    }
    public class SQCI_Item
    {
        private string _code;
        private string _name;
        private string _value;
        private string _min;
        private string _max;
        private string _testResult;

        public string Code { get { return _code; } set { _code = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public string Value { get { return _value; } set { _value = value; } }
        public string Min { get { return _min; } set { _min = value; } }
        public string Max { get { return _max; } set { _max = value; } }
        public string ResultTest { get { return _testResult; } set { _testResult = value; } }

        public override string ToString()
        {
            string str =
                 "[" + _code + "]" +
                 "[C]" +
                 "[" + _min + "_" + _max + "]" +
                 "[]" +
                 "[" + _value + "]" +
                 "[" + _testResult + "]" ;
            return str;
        }
    }
}
