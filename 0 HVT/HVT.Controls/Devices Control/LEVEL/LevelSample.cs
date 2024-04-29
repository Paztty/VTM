using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.Controls
{
    public class LevelSample
    {
        private bool _Level;
        public bool Level
        {
            get { return _Level; }
            set
            {
                if (value != _Level) _Level = value;
            }
        }
        private double x;
        private double y;

        public double X {
            get { return x; }
            set {
                if (x !=  value)
                {
                    x = value;
                }
            }
        }

        public double Y
        {
            get { return y; }
            set
            {
                if (y != value)
                {
                    y = value;
                }
            }
        }

        public System.Windows.Point Point
        {
            get { return new System.Windows.Point(x, y);}
        }

    }
}
