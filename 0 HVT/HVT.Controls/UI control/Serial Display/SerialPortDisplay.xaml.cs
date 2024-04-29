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
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Runtime.CompilerServices;
using System.Timers;
using HVT.Utility;
using System.Threading;
using Timer = System.Timers.Timer;

namespace HVT.Controls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SerialPortDisplay : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public CancellationTokenSource _shutDown = new CancellationTokenSource();
        public event EventHandler SerialDataReciver;

        private string devieceName;
        public string DevieceName
        {
            get { return devieceName; }
            set
            {
                if (devieceName != value)
                {
                    devieceName = value;
                    NotifyPropertyChanged(nameof(DevieceName));
                }
            }
        }

        private string portName;
        public string PortName
        {
            get { return portName; }
            set
            {
                if (portName != value)
                {
                    portName = value;
                    NotifyPropertyChanged(nameof(PortName));
                }
            }
        }

        private bool _BufferReciver = false;
        public bool BufferReciver
        {
            get
            {
                bool lastStatus = _BufferReciver;
                _BufferReciver = false;
                return lastStatus;
            }
            set
            {
                if (value != _BufferReciver)
                {
                    _BufferReciver = value;
                }
            }
        }
        public List<byte> BytesReciever = new List<byte>();

        private Timer rxTimer = new Timer()
        {
            Interval = 25,
        };
        private Timer txTimer = new Timer()
        {
            Interval = 25,
        };

        public double BlinkTime
        {
            get { return txTimer.Interval; }
            set
            {
                if (value != txTimer.Interval)
                {
                    txTimer.Interval = value;
                    rxTimer.Interval = value;
                }
            }
        }

        private SerialPort port = new SerialPort();
        public SerialPort Port
        {
            get { return port; }
            set
            {
                if (port != null && port.PortName != value.PortName)
                {
                    try
                    {
                        port.Close();
                    }
                    catch (Exception)
                    {
                    }
                    port = value;
                    port.DataReceived += Port_DataReceived;
                }
            }
        }

        public SerialPortDisplay()
        {
            InitializeComponent();
            txTimer.Elapsed += TxTimer_Tick;
            rxTimer.Elapsed += RxTimer_Tick;

            txTimer.Enabled = true;
            rxTimer.Enabled = true;

            this.DataContext = this;

        }

        private void RxTimer_Tick(object sender, EventArgs e)
        {
            if (!this._shutDown.IsCancellationRequested)
            {
                Rx.Dispatcher.Invoke(new Action(() =>
            {
                Rx.Fill = new SolidColorBrush(Colors.Black);
            }), DispatcherPriority.Normal);
                rxTimer.Stop();
            }
        }

        private void TxTimer_Tick(object sender, EventArgs e)
        {
            if (!this._shutDown.IsCancellationRequested)
            {
                Tx.Dispatcher.Invoke(new Action(() =>
                        {
                            Tx.Fill = new SolidColorBrush(Colors.Black);
                        }), DispatcherPriority.Normal);
                txTimer.Stop();
            }
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            BufferReciver = true;
            if (!this._shutDown.IsCancellationRequested)
            {
                Rx.Dispatcher.Invoke(new Action(() =>
                {
                    Rx.Fill = new SolidColorBrush(Colors.LightGreen);
                    rxTimer.Start();
                }));
            }
            SerialDataReciver?.Invoke(sender, null);
        }

        public void OpenPort()
        {
            if (!Port.IsOpen)
            {

                lbPortName.ToolTip = Port.PortName;
                Open.Fill = new SolidColorBrush(Colors.Gray);

            }
            else
            {
                lbPortName.ToolTip = Port.PortName;
                Open.Fill = new SolidColorBrush(Color.FromRgb(1, 79, 1));
            }
        }

        public void IsClosed()
        {
            Open.Dispatcher.Invoke(new Action(() =>
            {
                Open.Fill = new SolidColorBrush(Colors.Gray);
            }), DispatcherPriority.Normal);
        }

        public void SerialSend()
        {
            Tx.Dispatcher.Invoke(new Action(() =>
            {
                Tx.Fill = new SolidColorBrush(Colors.Yellow);
                txTimer.Start();
            }), DispatcherPriority.Normal);
        }

        public void SendString(string str)
        {
            if (Port.IsOpen)
            {
                Port.Write(str);
                Tx.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Tx.Fill = new SolidColorBrush(Colors.LightYellow);
                    txTimer.Start();
                }), DispatcherPriority.Normal);
            }
            else
            {
                Open.Dispatcher.Invoke(new Action(() =>
                {
                    Open.Fill = new SolidColorBrush(Colors.Gray);
                }), DispatcherPriority.Normal);
            }
        }

        public string ReadLine()
        {
            if (port.IsOpen)
            {
                try
                {
                    string str = port.ReadLine();
                    return str;
                }
                catch (TimeoutException)
                {
                    return "ERROR";
                }
                catch
                {
                    return "ERROR";
                }
            }
            return "ERROR";
        }

        public void SendBytes(byte[] buf)
        {
            if (Port == null) { return; }
            Tx.Dispatcher.Invoke(new Action(() =>
            {
                Tx.Fill = new SolidColorBrush(Colors.Yellow);
                txTimer.Start();
            }), DispatcherPriority.Normal);
            try
            {
                if (Port.IsOpen)
                {
                    Port.Write(buf, 0, buf.Length);
                }
            }
            catch (Exception)
            {

            }

        }

        public void SendToControls(byte[] buf, bool IsNoSize = false)
        {
            if (Port == null) { return; }
            Tx.Dispatcher.Invoke(new Action(() =>
            {
                Tx.Fill = new SolidColorBrush(Colors.Yellow);
                txTimer.Start();
            }), DispatcherPriority.Normal);
            try
            {
                if (Port.IsOpen)
                {
                    var frame = SYSTEM_COMUNICATION.GetFrame(buf, IsNoSize);
                    Port.Write(frame, 0, frame.Length);

                }
            }
            catch (Exception)
            {

            }
        }

        public bool SendToControls(byte[] buf, int TimeOut, byte[] lookResponse)
        {
            if (Port == null || !Port.IsOpen)
            {
                // Ex = "Port not found";
                return false;
            }

            List<byte> frame = new List<byte>();
            try
            {
                if (Port.IsOpen)
                {
                    SerialSend();

                    var frameToSend = SYSTEM_COMUNICATION.GetFrame(buf);
                    Port.Write(frameToSend, 0, frameToSend.Length);
                    var start = DateTime.Now;
                    while (DateTime.Now.Subtract(start).TotalMilliseconds < TimeOut)
                    {
                        if (Port.BytesToRead >= 5)
                        {
                            byte startByte = (byte)Port.ReadByte();
                            if (startByte == SYSTEM_COMUNICATION.Prefix1)
                            {

                                var secondByte = (byte)Port.ReadByte();
                                if (secondByte == SYSTEM_COMUNICATION.Prefix2)
                                {
                                    frame.Clear();
                                    frame.Add(startByte);
                                    frame.Add(secondByte);
                                    for (int i = 0; i < 4; i++)
                                    {
                                        frame.Add((byte)Port.ReadByte());
                                    }
                                    var result = SYSTEM_COMUNICATION.GetResponse(lookResponse, frame.ToArray());
                                    Console.WriteLine(result ? "OK" : "Fail");
                                    Console.Write(DevieceName + " -> ");
                                    foreach (var item in frame)
                                    {
                                        Console.Write(item.ToString("X2") + " ");
                                    }
                                    Console.WriteLine(" ");
                                    return result;
                                }
                            }
                        }
                    }
                    return false;
                }
                else
                {
                    // Ex = "Port not opened";
                    return false;
                }
            }
            catch (Exception)
            {
                //Ex = "Port exception: " + e.Message;
                return false;
            }
        }

        public bool SendAndRead(byte[] buf, byte function, int TimeOut, out byte[] Response, bool addRange = true, int size = 0)
        {
            if (Port == null || !Port.IsOpen)
            {
                // Ex = "Port not found";
                Response = new byte[0];
                return false;
            }
            else
            {
                List<byte> frame = new List<byte>();
                try
                {
                    if (Port.IsOpen)
                    {
                        SerialSend();
                        Port.DiscardInBuffer();
                        var frameToSend = SYSTEM_COMUNICATION.GetFrame(buf);
                        Port.Write(frameToSend, 0, frameToSend.Length);
                        var start = DateTime.Now;
                        while (DateTime.Now.Subtract(start).TotalMilliseconds < TimeOut)
                        {
                        readData:
                            byte byteReaded = (byte)Port.ReadByte();
                            if (frame.Count > 0 || byteReaded == SYSTEM_COMUNICATION.Prefix1)
                            {
                                frame.Add(byteReaded);
                                if (byteReaded == 0x56)
                                {
                                    var secondByte = frame[1];
                                    if (secondByte == SYSTEM_COMUNICATION.Prefix2)
                                    {
                                        Response = frame.ToArray();
                                        foreach (var item in frame)
                                        {
                                            Console.Write(item.ToString("X2") + " ");
                                        }
                                        Port.DiscardInBuffer();
                                        return true;
                                    }
                                }
                                else
                                {
                                    goto readData;
                                }
                            }
                            else
                            {
                                goto readData;
                            }
                        }
                    }
                    foreach (var item in frame)
                    {
                        Console.Write(item.ToString("X2") + " ");
                    }
                    Response = frame.ToArray();
                    return false;
                }

                catch (Exception)
                {
                    //Ex = "Port exception: " + e.Message;
                    Response = frame.ToArray();
                    return false;
                }
            }
        }

        public async Task<bool> CheckComPort(string PORT_NAME, int baudrate, string DATA_ASK, string endFrame, int TimeOut, string DevieceName)
        {
            if (port != null)
            {
                if (port.IsOpen)
                {
                    port.Close();
                    var startClosePortTime = DateTime.Now;
                    while (port.IsOpen)
                    {
                        if (DateTime.Now.Subtract(startClosePortTime).TotalMilliseconds > 500)
                        {
                            break;
                        }
                    }
                }
            }

            PortName = PORT_NAME;
            port.PortName = PORT_NAME;
            port.BaudRate = baudrate;
            try
            {
                port.Open();
            }
            catch (Exception e)
            {
                OpenPort();
                HVT.Utility.Debug.Write(DevieceName + ": " + e.Message, Debug.ContentType.Error);
                return false;
            }

            for (int i = 0; i < 3; i++)
            {
                if (port.IsOpen)
                {
                    port.DataReceived -= Port_DataReceived;
                    port.DataReceived += Port_DataReceived;
                    port.ReadTimeout = TimeOut;
                    port.Write(DATA_ASK + "\r\n");
                    Console.WriteLine("Send:\t " + DATA_ASK);
                    Tx.Dispatcher.Invoke(new Action(() =>
                    {
                        Tx.Fill = new SolidColorBrush(Colors.Yellow);
                        txTimer.Start();
                    }), DispatcherPriority.Normal);
                    await Task.Delay(100);
                    string answerString = "No anwser";
                    try
                    {

                        answerString = port.ReadTo(endFrame);
                        Console.WriteLine("Response:\t " + answerString);

                        if (answerString != "No anwser")
                        {
                            OpenPort();
                            return true;
                        }
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine(err.Message);
                        HVT.Utility.Debug.Write(DevieceName + ": " + err.Message, Debug.ContentType.Error);
                    }
                }
            }
            if (port.IsOpen)
            {
                port.Close();
            }
            OpenPort();
            return false;
        }

        public async Task<bool> CheckBoardComPort(string PORT_NAME, int baudrate, byte[] DATA_ASK, byte[] returnFrame, int TimeOut, bool IsNoSize = false)
        {
            if (port != null)
            {
                if (port.IsOpen)
                {
                    port.Close();
                    var startClosePortTime = DateTime.Now;
                    while (port.IsOpen)
                    {
                        if (DateTime.Now.Subtract(startClosePortTime).TotalMilliseconds > 500)
                        {
                            break;
                        }
                    }
                }
            }

            PortName = PORT_NAME;
            port.PortName = PORT_NAME;
            port.BaudRate = baudrate;
            port.DataBits = 8;
            port.Parity = Parity.None;
            port.StopBits = StopBits.One;
            try
            {
                port.Open();
            }
            catch (Exception e)
            {
                OpenPort();
                HVT.Utility.Debug.Write(DevieceName + ": " + e.Message, Debug.ContentType.Error);
                return false;
            }

            for (int i = 0; i < 5; i++)
            {
                if (port.IsOpen)
                {
                    port.DataReceived -= Port_DataReceived;
                    port.DataReceived += Port_DataReceived;
                    port.ReadTimeout = TimeOut;

                    SendToControls(DATA_ASK, IsNoSize);

                    Tx?.Dispatcher.Invoke(new Action(() =>
                    {
                        Tx.Fill = new SolidColorBrush(Colors.Yellow);
                        txTimer.Start();
                    }), DispatcherPriority.Normal);

                    Console.WriteLine(" ");
                    await Task.Delay(100);
                    DateTime start = DateTime.Now;
                    try
                    {
                        while (DateTime.Now.Subtract(start).TotalMilliseconds < TimeOut)
                        {
                            if (port.IsOpen)
                            {
                                if (port.BytesToRead > 3)
                                {
                                    byte[] bytes = new byte[port.BytesToRead];
                                    port.Read(bytes, 0, bytes.Length);
                                    var buffer = bytes.ToList();
                                    foreach (var item in bytes)
                                    {
                                        Console.Write(" {0:X}", item);
                                    }
                                    Console.WriteLine(" ");
                                    OpenPort();
                                    return true;
                                }
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine(err.Message);
                        HVT.Utility.Debug.Write(DevieceName + ": " + port.PortName + " > " + err.Message, Debug.ContentType.Error);
                    }
                }
            }
            if (port.IsOpen)
            {
                port.Close();
            }
            OpenPort();
            HVT.Utility.Debug.Write(DevieceName + ": " + port.PortName + " > " + "Deviece not response.", Debug.ContentType.Error);
            return false;
        }
    }
}
