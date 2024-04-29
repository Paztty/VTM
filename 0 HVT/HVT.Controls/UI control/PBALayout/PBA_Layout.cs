using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HVT.Controls
{
    public class PBA_Layout: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event EventHandler PCB_COUNT_CHANGE;

        private int pcb_Count = 1;
        public int PCB_Count
        {
            get { return pcb_Count; }
            set
            {
                if (pcb_Count != value)
                {
                    pcb_Count = value;
                    PCB_COUNT_CHANGE?.Invoke(pcb_Count, null);
                    OnPropertyChanged(nameof(PCB_Count));
                }
            }
        }
        private int pcb_X_axis_Count = 1;
        public int PCB_X_axis_Count {
            get { return pcb_X_axis_Count; }
            set
            {
                if (value != pcb_X_axis_Count)
                {
                    pcb_X_axis_Count = value;
                    OnPropertyChanged(nameof(PCB_X_axis_Count));
                }
            }
        }
        public enum ArrayPositions
        {
            HorizontalTopLeft = 0,
            HorizontalTopRight = 1,
            HorizontalBottomLeft = 2,
            HorizontalBottomRight = 3,
            VerticalTopLeft = 4,
            VerticalTopRight = 5,
            VerticalBottomLeft = 6,
            VerticalBottomRight = 7,
        };
        
        private ArrayPositions arrayPosition = ArrayPositions.HorizontalTopLeft;
        public int Position
        {
            get
            {
               return (int)arrayPosition;
            }

            set {
                if (value >= 0 & value < 8)
                {
                    arrayPosition = (ArrayPositions)value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }
    }
}
