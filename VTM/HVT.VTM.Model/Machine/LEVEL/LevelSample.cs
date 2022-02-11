using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.VTM.Base
{
    public class LevelSample
    {
        private int x;
        private int y;

        public int X {
            get { return x; }
            set {
                if (x !=  value)
                {
                    x = value;
                }
            }
        }

        public int Y
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
