using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using HVT.Utility;
using System.IO.Ports;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Documents;
using System.Windows.Media;
using System.Threading;
using Timer = System.Timers.Timer;

namespace HVT.VTM.Base.Machine
{
    public class Port
    {
        public SerialDisplay SerialDisplay = new SerialDisplay();
        
        public SerialPort serialPort = new SerialPort()
        {
            WriteTimeout = 100,
        };

        public bool ClearRxTimeSpecified { get; set; } = false;

        Timer clearRxTimer = new Timer();
        
        private int clearTime;
        
        private int ClearTime
        {
            get { return clearTime; }
            set
            {
                if (value == 0)
                {
                    clearRxTimer.Enabled = false;
                    clearTime = 0;
                }
                else
                {
                    clearRxTimer.Interval = 50;
                    clearTime = value;
                    clearRxTimer.Enabled = true;
                    clearRxTimer.Start();
                }
            }

        }

        private string serialname;
        
        public string SerialName
        {
            get { return serialname; }
            set
            {
                if (serialname != value)
                {
                    serialname = value;
                }
            }
        }
        
        private List<int> _buffer = new List<int>();
        
        public List<int> Buffer
        {
            get { return _buffer; }
            set { _buffer = value; }
        }
        
        public int[] Data;

        public Port()
        {
            serialPort.DataReceived += Serial_DataReceived;
            clearRxTimer.Elapsed += ClearRxTimer_Elapsed;
        }



        public bool SetPort(string portname, Rectangle portTx, Rectangle portRx, Rectangle portStatus)
        {
            serialname = portname;
            this.SerialDisplay.TX = portTx;
            SerialDisplay.RX = portRx;
            SerialDisplay.IsOpenRect = portStatus;
            return RefindCom();
        }

        private void ClearRxTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SerialDisplay.BlinkRX();
            ClearRxTimeSpecified = true;
            clearRxTimer.Stop();
        }

        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    SerialDisplay.BlinkRX();
                    int length = serialPort.BytesToRead;
                    string dataRead = "";
                    for (int i = 0; i < length; i++)
                    {
                        var byteChar = serialPort.ReadByte();
                        Buffer.Add(byteChar);
                        dataRead += byteChar.ToString("X2") + " ";
                    }
                    Write_Log(dataRead, false);
                }
            }
            catch (Exception)
            {

            }

        }

        private bool RefindCom()
        {
            SerialDisplay.ShowCOMStatus(false);
            try
            {
                if (SerialPort.GetPortNames().Contains(serialname))
                {
                    Console.WriteLine(serialname);
                    try
                    {
                        serialPort.Close();
                    }
                    catch (Exception err) {
                        Console.WriteLine(err.Message);
                    }

                    serialPort.PortName = SerialName;
                    var startWaitPortClose = DateTime.Now;
                    while (DateTime.Now.Subtract(startWaitPortClose).TotalMilliseconds < 1000)
                    {
                        try
                        {
                            serialPort.Open();
                            SerialDisplay.ShowCOMStatus(true);
                            return true;
                        }
                        catch (Exception)
                        {
                            Thread.Sleep(50);
                        }
                    }
                   
                }
                else
                {
                    SerialDisplay.ShowCOMStatus(false);
                    HVT.Utility.Debug.Write("\"" + serialname + "\"" + " not found.", Debug.ContentType.Error);
                }
            }
            catch (Exception err)
            {
                SerialDisplay.ShowCOMStatus(false);
                HVT.Utility.Debug.Write(err.Message, Debug.ContentType.Error);  
            }
            return false;
        }

        public bool OpenPort()
        {
            if (!serialPort.IsOpen)
            {
                try
                {
                    serialPort.Open();
                    return serialPort.IsOpen;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return serialPort.IsOpen;
        }

        public bool ClosePort()
        {
            if (!serialPort.IsOpen)
            {
                try
                {
                    serialPort.Close();
                    return serialPort.IsOpen;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return serialPort.IsOpen;
        }

        public bool Send(byte[] data)
        {
            if (data == null) return false;
            else
            {
                var frame = SYSTEM_COMUNICATION.GetFrame(data);
                string datalog = "";
                foreach (var item in frame)
                {
                    datalog += item.ToString("X2") + " ";
                }
                Write_Log(datalog);

                Buffer.Clear();
                if (serialPort.IsOpen)
                {
                    SerialDisplay.BlinkTX();
                    serialPort.DiscardInBuffer();
                    try
                    {
                        serialPort.Write(frame, 0, frame.Length);
                        return true;
                    }
                    catch (TimeoutException)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public bool CheckReturn(byte[] confirmReturn)
        {
            if (confirmReturn.Length > _buffer.Count) return false;
            else
            {
                for (int i = 0; i < _buffer.Count - confirmReturn.Length; i++)
                {
                    if (_buffer[i] == confirmReturn[0] && _buffer[i + 1] == confirmReturn[1])
                    {
                        bool constansReturnFrame = false;
                        for (int j = 0; j < confirmReturn.Length; j++)
                        {
                            constansReturnFrame = constansReturnFrame && _buffer[i + j] == confirmReturn[j];
                        }
                        if (constansReturnFrame) return true;
                    }
                }
            }
            return false;
        }

        public RichTextBox LogBox = new RichTextBox();
        
        private void Write_Log(string log, bool IsTxWrite = true)
        {
            if (LogBox != null)
            {
                var paragraph = new Paragraph();
                paragraph.LineHeight = 10;
                if (IsTxWrite)
                {
                    paragraph.Inlines.Add(new Run(DateTime.Now.ToString("HH:mm:ss -> Tx: ") + log));
                    paragraph.Foreground = new SolidColorBrush(Colors.Blue);
                }
                    
                else
                {
                    paragraph.Inlines.Add(new Run(DateTime.Now.ToString("HH:mm:ss <- Rx: ") + log));
                    paragraph.Foreground = new SolidColorBrush(Colors.Black);
                }

                if (LogBox.Dispatcher.CheckAccess())
                {
                    LogBox.Dispatcher.Invoke(new Action(delegate
                    {
                        LogBox.Document.Blocks.Add(paragraph);
                        LogBox.ScrollToEnd();
                    }), DispatcherPriority.Normal);
                }
            }
        }
    }
}
