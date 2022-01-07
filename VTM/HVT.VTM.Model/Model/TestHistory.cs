using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.VTM.Base
{
    public class TestHistory
    {
        public int No { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        private double testTime;
        public double TestTime {
            get { return Math.Round(testTime, 2); }
            set { testTime = value; }
        }
        public string project { get; set; }
        public string model { get; set; }
        public string site { get; set; }
        public string serial { get; set; }
        public string Result { get; set; }
        public string failItem { get; set; }

        public TestHistory()
        {}
        public TestHistory(int No) {
            this.No = No;
        }
    }
}
