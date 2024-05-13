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
    /// Interaction logic for ModelBarcodeOption.xaml
    /// </summary>
    public partial class ModelBarcodeOption : UserControl
    {
        private BarcodeOption barcodeOption = new BarcodeOption();
        public BarcodeOption BarcodeOption
        {
            get { return barcodeOption; }
            set
            {
                barcodeOption = value;
                this.DataContext = BarcodeOption;
            }
        }

        public ModelBarcodeOption()
        {
            InitializeComponent();
            this.DataContext = BarcodeOption;
        }

        private void nUD_BarcodeModelStart_ValueChanged(object sender, EventArgs e)
        {
            if (nUD_BarcodeLenght == null || nUD_BarcodeModelStart == null)
            {
                return;
            }
            if ((int)nUD_BarcodeModelStart.Value + tbBacodeModelCode.Text.Length > BarcodeOption.BarcodeLenght)
            {
                return;
            }
            string barcodePreview = "";
            for (int i = 0; i < (int)nUD_BarcodeLenght.Value; i++)
            {
                barcodePreview += 'x';
            }
            barcodePreview = barcodePreview.Remove((int)nUD_BarcodeModelStart.Value, tbBacodeModelCode.Text.Length).Insert((int)nUD_BarcodeModelStart.Value, tbBacodeModelCode.Text);
            lbBarcodePreview.Content = barcodePreview;
        }
    }
}
