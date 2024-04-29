using HVT.VTM.Base;
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

namespace HVT.VTM.Program
{
    /// <summary>
    /// Interaction logic for StepViewer.xaml
    /// </summary>
    public partial class StepViewer : UserControl
    {

        private Step _StepToGet = new Step();
        public Step StepToGet
        {
            get { return _StepToGet; }
            set
            {
                if (value != _StepToGet)
                    _StepToGet = value;
                this.DataContext = StepToGet;
            }
        }
        public StepViewer()
        {
            InitializeComponent();
            this.DataContext = StepToGet;
        }
    }
}
