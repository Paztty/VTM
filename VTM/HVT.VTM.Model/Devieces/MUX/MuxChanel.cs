using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HVT.VTM.Base
{
    public class MuxChannel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool isUse { get; set; } = true;
        public bool isOn { get; set; } = false;
        public int  channel_P { get; set; }
        public int  channel_N { get; set; }


        public bool IsUse {
            get { return isUse; }
            set {
                if (isUse != value)
                {
                    isUse = value;
                    NotifyPropertyChanged(nameof(IsUse));
                }
            }
        } 

        public bool IsON 
        {
            get { return isOn; }
            set
            {
                if (isOn != value)
                {
                    isOn = value;
                    NotifyPropertyChanged(nameof(IsON));
                }
            }
        }
        public int Channel_P {
            get { return channel_P; }
            set {
                if (value != channel_P)
                {
                    channel_P = value;
                    NotifyPropertyChanged(nameof(Channel_P));
                }
            }
        }
        public int Channel_N
        {
            get { return channel_N; }
            set
            {
                if (value != channel_N)
                {
                    channel_N = value;
                    NotifyPropertyChanged(nameof(Channel_N));
                }
            }
        }

    }
}
