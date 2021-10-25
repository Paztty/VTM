using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HVT.WPF.GraphicDraw
{
    public class GLED
    {
        public string Name = "GLED";
        public Rectangle Outline = new Rectangle()
        {
            RadiusX = 0,
            RadiusY = 0,
            Width = 100,
            Height = 50
        };
        public BitmapImage sourceImage;
        public BitmapImage GledImage;

        private string text = "";
        public string Text
        {
            get { return text; }
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
