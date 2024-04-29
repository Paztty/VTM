using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HVT.VTM.Base
{
    public class QRData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int no;

        public int No
        {
            get { return no; }
            set {
                if (no != value)
                {
                    no = value;
                    NotifyPropertyChanged("No");
                }
            }
        }
        public string Context { get; set; }
        public string Code { get; set; }

        public override string ToString()
        {
            return No + "," + Context + "," + Code;
        }
    }
}
