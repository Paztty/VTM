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

namespace HVT.Controls
{
    /// <summary>
    /// Interaction logic for DisChargeOption.xaml
    /// </summary>
    public partial class DisChargeOption : UserControl
    {
        private DischargeOption disChargeOption;

        public DischargeOption DisChargeOptionProperties
        {
            get { return disChargeOption; }
            set {
                if (disChargeOption != value)
                {
                    disChargeOption = value;
                    this.DataContext = DisChargeOptionProperties;
                }
            }
        }

        public DisChargeOption()
        {
            DisChargeOptionProperties = new DischargeOption();
            InitializeComponent();
            this.DataContext = DisChargeOptionProperties;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(HVT.Utility.Extensions.ConvertToJson(DisChargeOptionProperties));
        }
    }
}
