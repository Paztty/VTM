using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace HVT.VTM.Base
{
    public class FND
    {
        public char Channel { get; set; }

        private Rectangle zone = new Rectangle(0, 0, 10, 10);
        public Rectangle Zone {
            get { return zone; }
            set {
                if (value.Width > 10 || value.Height > 10)
                {
                    zone = value;
                }
            }
        }
        public string Spect;
        public string Output; 
    }
}
