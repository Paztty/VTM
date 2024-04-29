using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HVT.Gwinstek
{
    /// <summary>
    /// Interaction logic for AFG2225_Control.xaml
    /// </summary>
    public partial class AFG2225Control : UserControl
    {
        private AFG2225Machine _AFG2225_mc;
        public AFG2225Machine AFG2225_mc
        {
            get { return _AFG2225_mc; }
            set
            {
                if (value != null && value != _AFG2225_mc)
                {
                    _AFG2225_mc = value;
                    this.DataContext = AFG2225_mc;
                }
            }
        }
        public AFG2225Control()
        {
            InitializeComponent();
            this.DataContext = AFG2225_mc;
        }

        private void ApplySetup(object sender, MouseButtonEventArgs e)
        {
        }

        private void CH1_ON(object sender, RoutedEventArgs e)
        {
            (sender as ToggleButton).Content = "OUTPUT ON";
            AFG2225_mc.APPLY("CH1", AFG2225_mc.CH1._Frequency, AFG2225_mc.CH1._Amplitude, AFG2225_mc.CH1._DC_Ofset);
            AFG2225_mc.Serial.SendString("OUTP1 ON\r\n");
        }

        private void CH1_OFF(object sender, RoutedEventArgs e)
        {
            (sender as ToggleButton).Content = "OUTPUT OFF";
            AFG2225_mc.Serial.SendString("OUTP1 OFF\r\n");

        }

        private void CH2_OFF(object sender, RoutedEventArgs e)
        {
            (sender as ToggleButton).Content = "OUTPUT OFF";
            AFG2225_mc.Serial.SendString("OUTP2 OFF\r\n");
        }

        private void CH2_ON(object sender, RoutedEventArgs e)
        {
            (sender as ToggleButton).Content = "OUTPUT ON";
            AFG2225_mc.APPLY("CH2", AFG2225_mc.CH2._Frequency, AFG2225_mc.CH2._Amplitude, AFG2225_mc.CH2._DC_Ofset);
            AFG2225_mc.Serial.SendString("OUTP2 ON\r\n");
        }
    }
}
