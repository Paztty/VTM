using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HVT.Controls
{
    /// <summary>
    /// Discharge model
    /// </summary>
    public class DischargeOption : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _CheckBeforeTest;
        public bool CheckBeforeTest
        {
            get { return _CheckBeforeTest; }
            set
            {
                if (value != _CheckBeforeTest) _CheckBeforeTest = value;
                OnPropertyChanged("checkBeforeTest");
            }
        }

        private int _DelayBeforeStart;
        public int DelayBeforeStart
        {
            get { return _DelayBeforeStart; }
            set
            {
                if (value != _DelayBeforeStart) _DelayBeforeStart = value;
                OnPropertyChanged("DelayBeforeStart");
            }
        }

        private int _DischargeTime;
        public int DischargeTime
        {
            get { return _DischargeTime; }
            set
            {
                if (value != _DischargeTime) _DischargeTime = value;
                OnPropertyChanged("DischargeTime");
            }
        }

        private int _DischargeItem1;
        public int DischargeItem1
        {
            get { return _DischargeItem1; }
            set
            {
                if ( value != _DischargeItem1) _DischargeItem1 = value;
                OnPropertyChanged("DischargeItem1");
            }
        }

        private int _DischargeItem2;
        public int DischargeItem2
        {
            get { return _DischargeItem2; }
            set
            {
                if ( value != _DischargeItem2) _DischargeItem2 = value;
                OnPropertyChanged("DischargeItem2");
            }
        }

        private int _DischargeItem3;
        public int DischargeItem3
        {
            get { return _DischargeItem3; }
            set
            {
                if ( value != _DischargeItem3) _DischargeItem3 = value;
                OnPropertyChanged("DischargeItem3");
            }
        }

        private int _Item1ChannelP;
        public int Item1ChannelP
        {
            get { return _Item1ChannelP; }
            set
            {
                if (value != _Item1ChannelP) _Item1ChannelP = value;
                OnPropertyChanged("Item1ChannelP");
            }
        }

        private int _Item1ChannelN;
        public int Item1ChannelN
        {
            get { return _Item1ChannelN; }
            set
            {
                if (value != _Item1ChannelP) _Item1ChannelN = value;
                OnPropertyChanged("Item1ChannelN");
            }
        }

        private int _Item2ChannelP;
        public int Item2ChannelP
        {
            get { return _Item2ChannelP; }
            set
            {
                if (value != _Item2ChannelP) _Item2ChannelP = value;
                OnPropertyChanged("Item2ChannelP");
            }
        }

        private int _Item2ChannelN;
        public int Item2ChannelN
        {
            get { return _Item2ChannelN; }
            set
            {
                if (value != _Item2ChannelP) _Item2ChannelN = value;
                OnPropertyChanged("Item2ChannelN");
            }
        }

        private int _Item3ChannelP;
        public int Item3ChannelP
        {
            get { return _Item3ChannelP; }
            set
            {
                if (value != _Item3ChannelP) _Item3ChannelP = value;
                OnPropertyChanged("Item3ChannelP");
            }
        }

        private int _Item3ChannelN;
        public int Item3ChannelN
        {
            get { return _Item3ChannelN; }
            set
            {
                if (value != _Item3ChannelP) _Item3ChannelN = value;
                OnPropertyChanged("Item3ChannelN");
            }
        }

        private double _Item1VoltageBelow;
        public double Item1VoltageBelow
        {
            get { return _Item1VoltageBelow; }
            set
            {
                if (value != _Item1VoltageBelow) _Item1VoltageBelow = value;
                OnPropertyChanged("Item1VoltageBelow");
            }
        }
        
        private double _Item2VoltageBelow;
        public double Item2VoltageBelow
        {
            get { return _Item2VoltageBelow; }
            set
            {
                if (value != _Item2VoltageBelow) _Item2VoltageBelow = value;
                OnPropertyChanged("Item2VoltageBelow");
            }
        }

        private double _Item3VoltageBelow;
        public double Item3VoltageBelow
        {
            get { return _Item3VoltageBelow; }
            set
            {
                if (value != _Item3VoltageBelow) _Item3VoltageBelow = value;
                OnPropertyChanged("Item3VoltageBelow");
            }
        }

    }
}
