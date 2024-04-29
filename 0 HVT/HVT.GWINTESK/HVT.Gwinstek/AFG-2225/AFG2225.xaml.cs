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
    public partial class AFG2225 : UserControl
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

        public AFG2225()
        {
            InitializeComponent();
        }
    }
}
