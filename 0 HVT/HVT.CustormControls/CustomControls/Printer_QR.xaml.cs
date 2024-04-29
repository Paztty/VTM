using System;
using System.Collections.Generic;
using System.IO.Ports;
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
using HVT.Utility;

namespace CustomControls
{
    /// <summary>
    /// Interaction logic for Printer_QR.xaml
    /// </summary>
    public partial class Printer_QR : UserControl
    {
        public GT800_Printer GT800 = new GT800_Printer();
        public SamsungQRcode QRcode = new SamsungQRcode();

        public event EventHandler PortChange;

        private CustomControls.SerialPortDisplay serial = new SerialPortDisplay();
        public CustomControls.SerialPortDisplay Serial
        {
            get { return serial; }
            set {
                serial = value;
                GT800.serialPort = Serial.Port;
            }
        }

        public Printer_QR()
        {
            InitializeComponent();
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
            QRcode.ArrayPCBPrint = (bool)cbPCBArrayPrint.IsChecked;
            QRcode.CountryCode = tbCountryCode.Text;
            QRcode.ProductionLine = tbProductLine.Text;
            QRcode.InspectionEquipment = tbProductMachine.Text;
            QRcode.label.speed = (int)nudPrintSpeed.Value;
            QRcode.label.dark = (int)nudDarkness.Value;
            QRcode.label.Lenght = (int)nudLabelLenght.Value;
            QRcode.label.Pad = (int)nudLabelPad.Value;
            QRcode.label.home_x = (int)nudLabelHomeX.Value;
            QRcode.label.home_y = (int)nudLableHomeY.Value;
            QRcode.label.qr_x = (int)nudQRCodeX.Value;
            QRcode.label.qr_y = (int)nudQRCodeY.Value;
            QRcode.Size = (int)nudSize.Value;
            QRcode.label.SN1_X = (int)nudSNPart1X.Value;
            QRcode.label.SN1_Y = (int)nudSNPart1Y.Value;
            QRcode.label.SN1_W = (int)nudSNPart1W.Value;
            QRcode.label.SN1_H = (int)nudSNPart1H.Value;
            QRcode.label.SN1_Font = tbSNPart1Font.Text;
            QRcode.label.SN2_X = (int)nudSNPart2X.Value;
            QRcode.label.SN2_Y = (int)nudSNPart2Y.Value;
            QRcode.label.SN2_W = (int)nudSNPart2W.Value;
            QRcode.label.SN2_H = (int)nudSNPart2H.Value;
            QRcode.label.SN2_Font = tbSNPart2Font.Text;
            QRcode.label.MainCodeVersion_X = (int)nudMainCodeVersionX.Value;
            QRcode.label.MainCodeVersion_Y = (int)nudMainCodeVersionY.Value;
            QRcode.label.MainCodeVersion_W = (int)nudMainCodeVersionW.Value;
            QRcode.label.MainCodeVersion_H = (int)nudMainCodeVersionH.Value;
            QRcode.label.MainCodeVersion_Font = tbMainCodeVersionFont.Text;
            QRcode.label.InvCodeVersion_X = (int)nudInvCodeVersionX.Value;
            QRcode.label.InvCodeVersion_Y = (int)nudInvCodeVersionY.Value;
            QRcode.label.InvCodeVersion_W = (int)nudInvCodeVersionW.Value;
            QRcode.label.InvCodeVersion_H = (int)nudInvCodeVersionH.Value;
            QRcode.label.InvCodeVersion_Font = tbInvCodeVersionFont.Text;
            QRcode.label.PCBArrayText_X = (int)nudPCBArrayTextX.Value;
            QRcode.label.PCBArrayText_Y = (int)nudPCBArrayTextY.Value;
            QRcode.label.PCBArrayText_W = (int)nudPCBArrayTextW.Value;
            QRcode.label.PCBArrayText_H = (int)nudPCBArrayTextH.Value;
            QRcode.label.PCBArrayText_Font = tbPCBArrayTextFont.Text;
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
            cbPCBArrayPrint.IsChecked = QRcode.ArrayPCBPrint;
            tbCountryCode.Text = QRcode.CountryCode;
            tbProductLine.Text = QRcode.ProductionLine;
            tbProductMachine.Text = QRcode.InspectionEquipment;

            nudPrintSpeed.Value = QRcode.label.speed;
            nudDarkness.Value = QRcode.label.dark;
            nudLabelLenght.Value = QRcode.label.Lenght;
            nudLabelPad.Value = QRcode.label.Pad;
            nudLabelHomeX.Value = QRcode.label.home_x;
            nudLableHomeY.Value = QRcode.label.home_y;
            nudQRCodeX.Value = QRcode.label.qr_x;
            nudQRCodeY.Value = QRcode.label.qr_y;
            nudSize.Value = QRcode.Size;

            nudSNPart1X.Value = QRcode.label.SN1_X;
            nudSNPart1Y.Value = QRcode.label.SN1_Y;
            nudSNPart1W.Value = QRcode.label.SN1_W;
            nudSNPart1H.Value = QRcode.label.SN1_H;
            tbSNPart1Font.Text = QRcode.label.SN1_Font.ToString();

            nudSNPart2X.Value = QRcode.label.SN2_X;
            nudSNPart2Y.Value = QRcode.label.SN2_Y;
            nudSNPart2W.Value = QRcode.label.SN2_W;
            nudSNPart2H.Value = QRcode.label.SN2_H;
            tbSNPart2Font.Text = QRcode.label.SN2_Font.ToString();

            nudMainCodeVersionX.Value = QRcode.label.MainCodeVersion_X;
            nudMainCodeVersionY.Value = QRcode.label.MainCodeVersion_Y;
            nudMainCodeVersionW.Value = QRcode.label.MainCodeVersion_W;
            nudMainCodeVersionH.Value = QRcode.label.MainCodeVersion_H;
            tbMainCodeVersionFont.Text = QRcode.label.MainCodeVersion_Font.ToString();

            nudInvCodeVersionX.Value = QRcode.label.InvCodeVersion_X;
            nudInvCodeVersionY.Value = QRcode.label.InvCodeVersion_Y;
            nudInvCodeVersionW.Value = QRcode.label.InvCodeVersion_W;
            nudInvCodeVersionH.Value = QRcode.label.InvCodeVersion_H;
            tbInvCodeVersionFont.Text = QRcode.label.InvCodeVersion_Font.ToString();

            nudPCBArrayTextX.Value = QRcode.label.PCBArrayText_X;
            nudPCBArrayTextY.Value = QRcode.label.PCBArrayText_Y;
            nudPCBArrayTextW.Value = QRcode.label.PCBArrayText_W;
            nudPCBArrayTextH.Value = QRcode.label.PCBArrayText_H;
            tbPCBArrayTextFont.Text = QRcode.label.PCBArrayText_Font.ToString();

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
                Serial.Port = new SerialPort()
                {
                    PortName = GT800.serialPortName
                };
                GT800.PortChange(GT800.serialPortName);
                PortChange?.Invoke(this.GT800.serialPort, null);
            }
        }

        private void btPrintTest_Click(object sender, RoutedEventArgs e)
        {
            GetQRSettingFromUi();
            GT800.SendStringToPrinter(QRcode.GenerateSampleCode());
            Serial.SerialSend();
            QRcode.saveQRFormat();
            GT800.saveConfig();
        }
        public void Print(string Code)
        {
            Console.WriteLine(Code);
            GetQRSettingFromUi();
            GT800.SendStringToPrinter(Code);
            Serial.SerialSend();
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
    }
}
