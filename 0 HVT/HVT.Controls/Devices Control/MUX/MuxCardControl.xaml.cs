using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for MuxCardControl.xaml
    /// </summary>
    public partial class MuxCardControl : UserControl
    {

        private MuxCard _Card = new MuxCard();
        public MuxCard Card
        {
            get { return _Card; }
            set
            {
                if (value != null || value != _Card)
                {
                    _Card = value;
                    this.DataContext = _Card;
                    _Card.SerialPort1 = SerialPort1;
                    _Card.SerialPort2 = SerialPort2;
                    if (_contentLoaded)
                    {
                        CardChannelPanel1.Children.Clear();
                        CardChannelPanel2.Children.Clear();
                        for (int i = 0; i < _Card.Chanels.Count; i++)
                        {
                            if (i < 48)
                            {
                                CardChannelPanel1.Children.Add(_Card.Chanels[i].btOn);
                            }
                            else
                            {
                                CardChannelPanel2.Children.Add(_Card.Chanels[i].btOn);
                            }
                        }
                        _Card.Update();
                    }
                }
            }
        }

        private SerialPortDisplay serialCard1 = new SerialPortDisplay();

        public SerialPortDisplay SerialPort1
        {
            get { return serialCard1; }
            set
            {
                serialCard1 = value;
            }
        }

        private SerialPortDisplay serialCard2 = new SerialPortDisplay();

        public SerialPortDisplay SerialPort2
        {
            get { return serialCard2; }
            set
            {
                serialCard2 = value;
                serialCard2.DevieceName = "MUX#2";
            }
        }

        public MuxCardControl()
        {
            InitializeComponent();
            this.DataContext = _Card;
            serialCard1.DevieceName = "MUX#1";
            serialCard2.DevieceName = "MUX#2";
            CardChannelPanel1.Children.Clear();
            CardChannelPanel2.Children.Clear();
            for (int i = 0; i < _Card.Chanels.Count; i++)
            {
                if (i < 48)
                {
                    CardChannelPanel1.Children.Add(_Card.Chanels[i].btOn);
                }
                else
                {
                    CardChannelPanel2.Children.Add(_Card.Chanels[i].btOn);
                }
            }
        }

        public async void CheckCard1Comunication(string COMNAME)
        {
            byte[] cardChannel1 = new byte[13];
            byte[] cardChannel2 = new byte[13];

            cardChannel1[0] = (byte)0x4D;
            cardChannel2[0] = (byte)0x6D;

            byte[] cardResponse = SYSTEM_COMUNICATION.GetFrame(new byte[] { 0x00 });
            bool result = await SerialPort1.CheckBoardComPort(COMNAME, 9600, cardChannel1, cardResponse, 250);
        }
        public async void CheckCard2Comunication(string COMNAME)
        {
            byte[] cardChannel1 = new byte[13];
            byte[] cardChannel2 = new byte[13];

            cardChannel1[0] = (byte)0x4D;
            cardChannel2[0] = (byte)0x6D;

            byte[] cardResponse = SYSTEM_COMUNICATION.GetFrame(new byte[] { 0x00 });
            bool result = await SerialPort2.CheckBoardComPort(COMNAME, 9600, cardChannel2, cardResponse, 250);
        }

        private void Manual_Channel_Click(object sender, RoutedEventArgs e)
        {
            if (_Card.CurrentChannel != null)
            {
                if (_Card.CurrentChannel.Channel_P == 0)
                {
                    (sender as ToggleButton).IsChecked = false;
                    return;
                }
                if ((bool)(sender as ToggleButton).IsChecked)
                {
                    if (_Card.CurrentChannel.Channel_P >= 49 && ChN.Value < 49)
                    {
                        ChN.Value = _Card.CurrentChannel.Channel_N;
                    }
                    else if (Card.CurrentChannel.Channel_P < 49 && ChN.Value >= 49)
                    {
                        ChN.Value = 1;
                    }

                    bool setOK = _Card.ManualSetCardStatus(_Card.CurrentChannel.Channel_P, (int)ChN.Value);
                    (sender as ToggleButton).IsChecked = setOK;
                }
                else
                {
                    (sender as ToggleButton).IsChecked = false;
                    _Card.ReleaseChannels();
                }
            }
            else
            {
                (sender as ToggleButton).IsChecked = false;
            }
        }

        public bool SetChannel(int P, int N)
        {
          return Card.ManualSetCardStatus(P, N);
        }
    }
}
