using HVT.Controls.CustomControls;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Camera
{
    /// <summary>
    /// Interaction logic for VisionModelOption.xaml
    /// </summary>
    public partial class VisionModelOption : UserControl
    {
        public LCD lcd = new LCD();
        public FND fnd = new FND();

        bool isFnd = true;

        public VisionModelOption()
        {
            InitializeComponent();
        }

        private void IntegerUpDown_ValueChanged(object sender, EventArgs e)
        {

        }

        private void noiseFilterValueChanged(object sender, EventArgs e)
        {
          if(isFnd)   fnd.NoiseSize = (sender as IntegerUpDown).Value;
          else        lcd.NoiseSize = (sender as IntegerUpDown).Value;
        }

        public void SetDataContext(object sender)
        {
            FND tryCatchFND = (sender as FND);
            if (tryCatchFND != null)
            {
                DataContext = tryCatchFND;
                this.fnd = tryCatchFND;
                isFnd = true;
                threshould_nud.Value = (int)tryCatchFND.Threshold;
                noise_nud.Value = (int)tryCatchFND.NoiseSize;
                blur_nud.Value = (int)tryCatchFND.Blur;
            }
            LCD tryCatchLCD = (sender as LCD);
            if (tryCatchLCD != null)
            {
                DataContext = tryCatchLCD;
                this.lcd = tryCatchLCD;
                isFnd = false;
                threshould_nud.Value = (int)tryCatchLCD.Threshold;
                noise_nud.Value = (int)tryCatchLCD.NoiseSize;
                blur_nud.Value = (int)tryCatchLCD.Blur;
            }
        }

        private void threshold_ValueChanged(object sender, EventArgs e)
        {
            if (isFnd) fnd.Threshold = (sender as IntegerUpDown).Value;
            else lcd.Threshold = (sender as IntegerUpDown).Value;
        }

        private void BlurChange(object sender, EventArgs e)
        {
            if (isFnd) fnd.Blur = (sender as IntegerUpDown).Value;
            else lcd.Blur = (sender as IntegerUpDown).Value;
        }
    }
}
