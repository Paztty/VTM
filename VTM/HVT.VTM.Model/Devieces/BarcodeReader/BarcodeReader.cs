using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.VTM.Base
{
    public class BarcodeReader
    {
        public SerialPort serialPort =
            new SerialPort()
            {
                PortName = "COM1",
                BaudRate = 9600,
                Parity = Parity.None,
                StopBits = StopBits.One,
            };
        public string PortName { get; set; } = "COM1";
        public HVT.Utility.SerialDisplay SerialDisplay = new HVT.Utility.SerialDisplay();
        //event
        public event EventHandler OnReciverData;
        private void OnReciverCode()
        {
            OnReciverData?.Invoke(this, new EventArgs());
        }

        public event EventHandler OnConnected;
        private void OnConnectedChange(bool IsOpen)
        {
            OnConnected?.Invoke(IsOpen, new EventArgs());
        }



        private string barcodeBuffer = "";

        public string BarcodeBuffer
        {
            get { return barcodeBuffer; }

            set
            {
                if (IsCompareModelCode)
                {
                    if (ModelCodeCompare(this.ModelCode, value))
                    {
                        barcodeBuffer = value;
                        OnReciverCode();
                    }
                    else
                    {
                        HVT.Utility.Debug.Write("New barcode input not have model code inside.", Utility.Debug.ContentType.Error);
                    }
                }
                else
                {
                    barcodeBuffer = value;
                    OnReciverCode();
                }
            }
        }
        public bool IsUserBarcodeInput { get; set; }
        public bool IsBarcodelenghtFix { get; set; }
        public int BarcodeLength { get; set; }
        public bool IsCompareModelCode { get; set; }
        public int ModelCodePosition { get; set; }
        public string ModelCode { get; set; }

        private bool ModelCodeCompare(string modelCode, string Barcode)
        {
            return Barcode.IndexOf(modelCode) == ModelCodePosition - 1;
        }

        public void SerialStartConnect()
        {
            OnConnectedChange(false);
            if (serialPort.IsOpen) serialPort.Close();
            serialPort.PortName = PortName;
            serialPort.DataReceived -= SerialPort_DataReceived;
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.NewLine = "\n";
            try
            {
                serialPort.Open();
                OnConnectedChange(serialPort.IsOpen);
            }
            catch (Exception err)
            {
                HVT.Utility.Debug.Write(("Barcode open error on" + PortName + ": " + err.Message), Utility.Debug.ContentType.Error);
            }

        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    string buffer = serialPort.ReadExisting();
                    if (buffer != null)
                    {
                        BarcodeBuffer = buffer;
                    }
                }
                catch (Exception)
                { }
            }
        }
    }
}
