using System;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HVT.Utility;
using Point = System.Drawing.Point;

namespace HVT.Printer
{
    /// <summary>
    /// Interaction logic for Printer_QR.xaml
    /// </summary>
    public partial class Printer_QR : UserControl
    {
        public GT800_Printer GT800 = new GT800_Printer();
        public SamsungQRcode QRcode = new SamsungQRcode();
        public DEV_Label DEV_Label { get; set; } = new DEV_Label();
        public event EventHandler PortChange;

        public Printer_QR()
        {
            InitializeComponent();
            this.DataContext = this;

            DEV_Label = Extensions.OpenFromFile<DEV_Label>(DEV_Label.LabelConfigPath);
            if (DEV_Label == null)
            {
                DEV_Label = new DEV_Label();
            }


            GT800 = Extensions.OpenFromFile<GT800_Printer>(GT800_Printer.PrinterConfigPath);
            if (GT800 == null)
            {
                GT800 = new GT800_Printer();
            }
            if (GT800.IsSerialPrint)
            {
                GT800.SerialInit();
            }

            QRcode = Extensions.OpenFromFile<SamsungQRcode>(SamsungQRcode.ConfigPath);
            if (QRcode == null)
            {
                QRcode = new SamsungQRcode();
            }
            SetQRSettingToUi();
        }

        public void GetQRSettingFromUi()
        {
            QRcode.TestPCBPrintAll = (bool)rbtTestPassAll.IsChecked;
            QRcode.TestPCBPassPrint = (bool)rbtPassOnly.IsChecked;
            QRcode.PrintMaxStepCount = (int)nudPrintMaxStepCount.Value;

            QRcode.PrintUpsideDown = (bool)cbPrintUSD.IsChecked;
            QRcode.UnitCode = tbQR_UnitCode.Text;
            QRcode.SupplierCode = tbQR_VenderCode.Text;
            QRcode.QRCode = tbQR_GubunCode.Text;
            QRcode.CountryCode = tbQR_CountryCode.Text;
            QRcode.ProductionLine = tbQR_LineCode.Text;
            QRcode.InspectionEquipment = tbQR_EQPMCode.Text;

            QRcode.SerialBase = (int)nudSerialBase.Value;

            GT800.IsSerialPrint = (bool)cbSerialPrint.IsChecked;
            if (cbbPrinterPort.SelectedItem != null)
            {
                GT800.serialPortName = (string)cbbPrinterPort.SelectedItem;
            }
        }

        public void SetQRSettingToUi()
        {
            rbtTestPassAll.IsChecked = QRcode.TestPCBPrintAll;
            rbtPassOnly.IsChecked = QRcode.TestPCBPassPrint;
            nudPrintMaxStepCount.Value = QRcode.PrintMaxStepCount;

            cbPrintUSD.IsChecked = QRcode.PrintUpsideDown;

            tbQR_UnitCode.Text = QRcode.UnitCode;
            tbQR_VenderCode.Text = QRcode.SupplierCode;
            tbQR_GubunCode.Text = QRcode.QRCode;
            tbQR_CountryCode.Text = QRcode.CountryCode;
            tbQR_LineCode.Text = QRcode.ProductionLine;
            tbQR_EQPMCode.Text = QRcode.InspectionEquipment;

            nudSerialBase.Value = QRcode.SerialBase;

            cbbPrinterPort.ItemsSource = SerialPort.GetPortNames();
            if (cbbPrinterPort.Items.Contains(GT800.serialPortName))
            {
                cbbPrinterPort.SelectedItem = GT800.serialPortName;
            }
            cbSerialPrint.IsChecked = GT800.IsSerialPrint;
            cbbPrinterPort.IsEnabled = GT800.IsSerialPrint;
        }

        private void cbbPrinterPort_PreviewDrop(object sender, DragEventArgs e)
        {
            cbbPrinterPort.ItemsSource = SerialPort.GetPortNames();
        }

        private void cbbPrinterPort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbPrinterPort.SelectedItem != null)
            {
                GT800.serialPortName = (string)cbbPrinterPort.SelectedItem;
                GT800.PortChange(GT800.serialPortName);
                PortChange?.Invoke(this.GT800.serialPort, null);
            }
        }

        private void btPrintTest_Click(object sender, RoutedEventArgs e)
        {
            //GetQRSettingFromUi();
            //GT800.SendStringToPrinter(QRcode.GenerateSampleCode());
            //Serial.SerialSend();
            //QRcode.saveQRFormat();
            GT800.saveConfig();
            DEV_Label.Save();

            string str = DEV_Label.MakeLabel("32DC9200256A", "DYSCT9R1232", "QR TEXT FROM FT MACHINE", array: "A");
            GT800.SendStringToPrinter(str);
        }
        public void Print(string Code)
        {
        }
        private void btDefault_Click(object sender, RoutedEventArgs e)
        {
            GT800 = new GT800_Printer();
            SetQRSettingToUi();
        }

        private void cbSerialPrint_Checked(object sender, RoutedEventArgs e)
        {
            cbbPrinterPort.ItemsSource = SerialPort.GetPortNames();
            cbbPrinterPort.IsEnabled = true;
        }

        private void cbSerialPrint_Unchecked(object sender, RoutedEventArgs e)
        {
            cbbPrinterPort.IsEnabled = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void QR_Content_Change(object sender, TextChangedEventArgs e)
        {
            DEV_Label.QR.ContentString = (sender as TextBox).Text;
            
        }
    }
}
