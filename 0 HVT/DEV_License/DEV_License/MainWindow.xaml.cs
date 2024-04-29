using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
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
using DEVMachineLicense;

namespace DEV_License
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var macAddr =
                (
                    from nic in NetworkInterface.GetAllNetworkInterfaces()
                    where nic.OperationalStatus == OperationalStatus.Up
                    select nic.GetPhysicalAddress().ToString()
                ).FirstOrDefault();

            tbMAC.Text = macAddr;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
           DEVMachineLicense.DEV_License _License = new DEVMachineLicense.DEV_License();
            _License.DeviceMAC = tbMAC.Text;
            _License.StartDate = DateTime.Now;
            _License.MachineLisenseType = DEVMachineLicense.DEV_License.LicenseType.All;
            _License.MachineName = tbSoftwareName.Text;
            _License.CreatLicense();
        }

        public static string EncoderLicense(string plainText, Encoding encodingCode)
        {
            var plainTextBytes = encodingCode.GetBytes(plainText);
            for (int i = 0; i < plainTextBytes.Length; i++)
            {
                plainTextBytes[i] = (byte)(plainTextBytes[i] + 10);
            }
            return System.Convert.ToBase64String(plainTextBytes);
        }

        //Decoder string
        public static string DecoderLicense(string base64EncodedData, Encoding encodingCode)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            for (int i = 0; i < base64EncodedBytes.Count(); i++)
            {
                base64EncodedBytes[i] = (byte)(base64EncodedBytes[i] - 10);
            }
            return encodingCode.GetString(base64EncodedBytes);
        }
    }
}
