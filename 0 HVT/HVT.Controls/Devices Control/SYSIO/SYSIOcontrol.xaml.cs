using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HVT.Controls
{
    /// <summary>
    /// Interaction logic for SYSIOcontrol.xaml
    /// </summary>
    public partial class SYSIOcontrol : UserControl
    {

        private SYSTEM_BOARD _System_Board = new SYSTEM_BOARD();
        public SYSTEM_BOARD System_Board
        {
            get { return _System_Board; }
            set
            {
                if (value != null || value != _System_Board)
                    _System_Board = value;
                this.DataContext = System_Board.MachineIO;
            }
        }

        public SYSIOcontrol()
        {
            InitializeComponent();
            this.DataContext = System_Board.MachineIO;

        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            System_Board.SendControl();
        }
    }
}
