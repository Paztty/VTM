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

namespace HVT.Gwinstek
{
    /// <summary>
    /// Interaction logic for AFG2225.xaml
    /// </summary>
    public partial class GDS1102B : UserControl
    {
        private GDS1120machine _GDS1120;
        public GDS1120machine GDS1120
        {
            get { return _GDS1120; }
            set
            {
                if (value != null && value != _GDS1120)
                {
                    _GDS1120 = value;
                    this.DataContext = GDS1120;
                }
            }
        }

        public GDS1102B()
        {
            InitializeComponent();
            this.DataContext = GDS1120;
        }

        private void AutoSet(object sender, MouseButtonEventArgs e)
        {
            GDS1120.Serial.SendString(":AUTOSet\r\n");
            Task.Delay(1000).Wait();
            GDS1120.QUERY("CH1");
            GDS1120.QUERY("CH2");
        }
    }
}
