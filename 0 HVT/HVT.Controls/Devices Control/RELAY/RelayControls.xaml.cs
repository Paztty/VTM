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
    /// Interaction logic for RelayControls.xaml
    /// </summary>
    public partial class RelayControls : UserControl
    {


        private SerialPortDisplay _SerialPort = new SerialPortDisplay();
        public SerialPortDisplay SerialPort
        {
            get { return _SerialPort; }
            set
            {
                if (value != null || value != _SerialPort)
                    _SerialPort = value;
            }
        }

        private RelayCard _Card = new RelayCard();
        public RelayCard Card
        {
            get { return _Card; }
            set
            {
                if (value != null || value != _Card)
                {
                    _Card = value;
                    _Card.SerialPort = _SerialPort;
                    this.DataContext = _Card;
                    CardChannelPanel.Children.Clear();
                    foreach (var item in _Card.Chanels)
                    {
                        CardChannelPanel.Children.Add(item.btOn);
                    }
                }
            }
        }
        public RelayControls()
        {
            InitializeComponent();
            SerialPort.DevieceName = "RELAY";
            SerialPort.Port = new System.IO.Ports.SerialPort
            {
                PortName = "COM1",
                BaudRate = 9600,
                DataBits = 8,
                Parity = System.IO.Ports.Parity.None,
                StopBits = System.IO.Ports.StopBits.One
            };
            _Card.SerialPort = _SerialPort;
            this.DataContext = _Card;
            CardChannelPanel.Children.Clear();
            foreach (var item in _Card.Chanels)
            {
                CardChannelPanel.Children.Add(item.btOn);
            }
        }

        public async void CheckCardComunication(string COMNAME)
        {
            byte[] cardChannel = new byte[7];

            cardChannel[0] = (byte)0x52;
            byte[] cardResponse = SYSTEM_COMUNICATION.GetFrame(new byte[] { 0x00 });
            await SerialPort.CheckBoardComPort(COMNAME, 9600, cardChannel, cardResponse, 250);
        }

        public bool SetChannels(List<int> channels, Boolean IsOn)
        {
            foreach (var item in channels)
            {
                Card.Chanels[item].IsON = IsOn;
            }
          return  Card.SendCardStatus();
        }

        public void Clear()
        {
            Card.Release();
        }
    }
}
