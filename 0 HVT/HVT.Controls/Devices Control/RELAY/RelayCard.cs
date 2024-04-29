using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace HVT.Controls
{
    public class RelayCard
    {
        public ObservableCollection<RelayChannel> Chanels { get; set; } = new ObservableCollection<RelayChannel>()
        {
            new RelayChannel { Channel_P = 1 },
            new RelayChannel { Channel_P = 2 },
            new RelayChannel { Channel_P = 3 },
            new RelayChannel { Channel_P = 4 },
            new RelayChannel { Channel_P = 5 },
            new RelayChannel { Channel_P = 6 },
            new RelayChannel { Channel_P = 7 },
            new RelayChannel { Channel_P = 8 },
            new RelayChannel { Channel_P = 9 },
            new RelayChannel { Channel_P = 10 },
            new RelayChannel { Channel_P = 11 },
            new RelayChannel { Channel_P = 12 },
            new RelayChannel { Channel_P = 13 },
            new RelayChannel { Channel_P = 14 },
            new RelayChannel { Channel_P = 15 },
            new RelayChannel { Channel_P = 16 },
            new RelayChannel { Channel_P = 17 },
            new RelayChannel { Channel_P = 18 },
            new RelayChannel { Channel_P = 19 },
            new RelayChannel { Channel_P = 20 },
            new RelayChannel { Channel_P = 21 },
            new RelayChannel { Channel_P = 22 },
            new RelayChannel { Channel_P = 23 },
            new RelayChannel { Channel_P = 24 },
            new RelayChannel { Channel_P = 25 },
            new RelayChannel { Channel_P = 26 },
            new RelayChannel { Channel_P = 27 },
            new RelayChannel { Channel_P = 28 },
            new RelayChannel { Channel_P = 29 },
            new RelayChannel { Channel_P = 30 },
            new RelayChannel { Channel_P = 31 },
            new RelayChannel { Channel_P = 32 },
            new RelayChannel { Channel_P = 33 },
            new RelayChannel { Channel_P = 34 },
            new RelayChannel { Channel_P = 35 },
            new RelayChannel { Channel_P = 36 },
            new RelayChannel { Channel_P = 37 },
            new RelayChannel { Channel_P = 38 },
            new RelayChannel { Channel_P = 39 },
            new RelayChannel { Channel_P = 40 },
            new RelayChannel { Channel_P = 41 },
            new RelayChannel { Channel_P = 42 },
            new RelayChannel { Channel_P = 43 },
            new RelayChannel { Channel_P = 44 },
            new RelayChannel { Channel_P = 45 },
            new RelayChannel { Channel_P = 46 },
            new RelayChannel { Channel_P = 47 },
            new RelayChannel { Channel_P = 48 }
        };
        private SerialPortDisplay _SerialPort;
        public SerialPortDisplay SerialPort
        {
            get { return _SerialPort; }
            set
            {
                if (value != null || value != _SerialPort)
                    _SerialPort = value;
            }
        }
        public RelayCard(WrapPanel panelSelect)
        {
            panelSelect.Children.Clear();
            foreach (var item in Chanels)
            {
                panelSelect.Children.Add(item.CbUse);
            }
        }
        public RelayCard()
        {
            foreach (var item in Chanels)
            {
                item.ManualStateChange += Item_ManualStateChange;
            }
        }

        public void Update()
        {
            foreach (var item in Chanels)
            {
                item.ManualStateChange -= Item_ManualStateChange; 
                item.ManualStateChange += Item_ManualStateChange;
            }
        }

        private void Item_ManualStateChange(object sender, EventArgs e)
        {
            (sender as RelayChannel).ManualStateChange -= Item_ManualStateChange;
            bool changeOk = SendCardStatus();
            (sender as RelayChannel).IsON = (sender as RelayChannel).IsON ? changeOk : !changeOk;
            (sender as RelayChannel).ManualStateChange += Item_ManualStateChange;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {

        }

        public void SelectAll()
        {
            foreach (var item in Chanels)
            {
                item.isUse = true;
            }
        }

        public void ClearAll()
        {
            foreach (var item in Chanels)
            {
                item.isUse = false;
            }
        }

        public void SetPort()
        { 

        }

        public bool SendCardStatus()
        {
            byte[] cardChannel = new byte[7];

            cardChannel[0] = (byte)0x52;
            Int64 data = 0;
            for (int i = Chanels.Count - 1; i >= 0; i--)
            {
                data = data << 1;
                if (Chanels[i].IsON)
                {
                    data |= 1;
                }
            }
            Console.WriteLine("- {0}- ", Convert.ToString(data, 2).PadLeft(64, '0'));
            var bytes = BitConverter.GetBytes(data);
            for (int i = cardChannel.Length - 1 ; i >= 1 ; i--)
            {
                cardChannel[i] = bytes[cardChannel.Length - 1 - i];
                Console.WriteLine("byte {0} : {1}", i, Convert.ToString(cardChannel[i], 2));
            }

            return SerialPort.SendToControls(cardChannel, 500, new byte[] { 0x52, 0x00 });
        }
        public bool Release()
        {
            byte[] cardChannel = new byte[7];

            cardChannel[0] = (byte)0x52;
            foreach (var item in Chanels)
            {
                item.IsON = false;
            }
            Int64 data = 0;

            var bytes = BitConverter.GetBytes(data);
            for (int i = cardChannel.Length - 1; i >= 1; i--)
            {
                cardChannel[i] = bytes[cardChannel.Length - 1 - i];
                Console.WriteLine("byte {0} : {1}", i, Convert.ToString(cardChannel[i], 2));
            }

            return SerialPort.SendToControls(cardChannel, 500, new byte[] { 0x52, 0x00 });
        }
    }
}
