using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HVT.VTM.Base
{
    public class TxData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int no { get; set; } = 0;
        public string name { get; set; } = "ABC";
        public string data { get; set; } = "00";
        public string blank { get; set; } = "00";
        public string remark { get; set; } = "Remark data";

        public int No{get; set; }
        public string Name
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                    NotifyPropertyChanged(nameof(Name));
                }
            }
        }
        public string Data
        {
            get { return data; }
            set
            {
                if (data!= value)
                {
                    data = value;
                    NotifyPropertyChanged(nameof(Data));
                }
            }
        }
        public string Blank {
            get { return blank; }
            set
            {
                if (blank != value)
                {
                    blank = value;
                    NotifyPropertyChanged(nameof(Blank));
                }
            }
        }
        public string Remark {
            get { return remark; }
            set
            {
                if (remark != value)
                {
                    remark = value;
                    NotifyPropertyChanged(nameof(Remark));
                }
            }
        }

        public TxData() { }

        public override string ToString()
        {
            string strReturn = No + "," + Name + "," + Data + "," + Blank + "," + Remark;
            return strReturn;
        }
    }


}
