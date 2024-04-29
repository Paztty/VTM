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

namespace CustomControls
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

        public event EventHandler SerialDataReciver;

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

        public List<byte> BytesReciever = new List<byte>();

        private Timer rxTimer = new Timer()
        {
            Interval = 100,
        };
        private Timer txTimer = new Timer()
        {
            Interval = 100,
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
            Rx.Dispatcher.Invoke(new Action(() =>
            {
                Rx.Fill = new SolidColorBrush(Colors.DarkGreen);
            }), DispatcherPriority.Normal);
            rxTimer.Stop();
        }

        private void TxTimer_Tick(object sender, EventArgs e)
        {
            Tx.Dispatcher.Invoke(new Action(() =>
            {
                Tx.Fill = new SolidColorBrush(Colors.DarkRed);
            }), DispatcherPriority.Normal);
            txTimer.Stop();
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Rx.Dispatcher.Invoke(new Action(() =>
            {
                Rx.Fill = new SolidColorBrush(Colors.LightGreen);
                rxTimer.Start();
            }), DispatcherPriority.Normal);
            SerialDataReciver?.Invoke(sender, null);
        }

        public void OpenPort()
        {
            if (!Port.IsOpen & SerialPort.GetPortNames().Contains(Port.PortName))
            {
                try
                {
                    Port.Open();
                    lbPortName.ToolTip = Port.PortName;
                    Open.Fill = new SolidColorBrush(Colors.Green);
                }
                catch (Exception)
                {
                    lbPortName.ToolTip = Port.PortName + " ERROR";
                    Open.Fill = new SolidColorBrush(Colors.Gray);
                }
            }
            else
            {
                lbPortName.ToolTip = Port.PortName;
                Open.Fill = new SolidColorBrush(Colors.Green);
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
                Tx.Fill = new SolidColorBrush(Colors.LightYellow);
                txTimer.Start();
            }), DispatcherPriority.Normal);
        }

        public void SendString(string str)
        {
            if (Port.IsOpen)
            {
                Port.Write(str);
                Tx.Dispatcher.Invoke(new Action(() =>
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
        public void SendBytes(byte[] buf)
        {
            if (Port == null) { return; }
            Tx.Dispatcher.Invoke(new Action(() =>
            {
                Tx.Fill = new SolidColorBrush(Colors.LightYellow);
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
    }
}
