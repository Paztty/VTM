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
    public class SolenoidCard
    {
        public ObservableCollection<SolenoidChannel> Chanels { get; set; } = new ObservableCollection<SolenoidChannel>()
        {
            new SolenoidChannel { Channel_P = 1 },
            new SolenoidChannel { Channel_P = 2 },
            new SolenoidChannel { Channel_P = 3 },
            new SolenoidChannel { Channel_P = 4 },
            new SolenoidChannel { Channel_P = 5 },
            new SolenoidChannel { Channel_P = 6 },
            new SolenoidChannel { Channel_P = 7 },
            new SolenoidChannel { Channel_P = 8 },
            new SolenoidChannel { Channel_P = 9 },
            new SolenoidChannel { Channel_P = 10 },
            new SolenoidChannel { Channel_P = 11 },
            new SolenoidChannel { Channel_P = 12 },
            new SolenoidChannel { Channel_P = 13 },
            new SolenoidChannel { Channel_P = 14 },
            new SolenoidChannel { Channel_P = 15 },
            new SolenoidChannel { Channel_P = 16 },
            new SolenoidChannel { Channel_P = 17 },
            new SolenoidChannel { Channel_P = 18 },
            new SolenoidChannel { Channel_P = 19 },
            new SolenoidChannel { Channel_P = 20 },
            new SolenoidChannel { Channel_P = 21 },
            new SolenoidChannel { Channel_P = 22 },
            new SolenoidChannel { Channel_P = 23 },
            new SolenoidChannel { Channel_P = 24 }
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
        public SolenoidCard(WrapPanel panelSelect)
        {
            panelSelect.Children.Clear();
            foreach (var item in Chanels)
            {
                panelSelect.Children.Add(item.CbUse);
            }
        }
        public SolenoidCard()
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
            (sender as SolenoidChannel).ManualStateChange -= Item_ManualStateChange;
            bool changeOk = SendCardStatus();
            (sender as SolenoidChannel).IsON = (sender as SolenoidChannel).IsON ? changeOk : !changeOk;
            (sender as SolenoidChannel).ManualStateChange += Item_ManualStateChange;
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

        public void Release()
        {
            foreach (var item in Chanels)
            {
                item.isOn = false;
            }
            SendCardStatus();
        }

        public bool SendCardStatus()
        {
            byte[] cardChannel = new byte[5];

            cardChannel[0] = (byte)0x53;

            UInt32 data = 0;
            for (int i = Chanels.Count - 1; i >= 0; i--)
            {
                data = data << 1;
                if (Chanels[i].IsON)
                {
                    data |= 1;
                }
            }
            Console.WriteLine("- {0}- ", Convert.ToString(data, 2).PadLeft(24,'0'));
            var bytes = BitConverter.GetBytes(data);
            for (int i = cardChannel.Length - 1; i >= 1; i--)
            {
                cardChannel[i] = bytes[cardChannel.Length - 1 - i];
                Console.WriteLine("byte {0} : {1}", i, Convert.ToString(cardChannel[i], 2));
            }

            //for (int i = 2; i >= 0; i--)
            //{
            //    byte data1 = 0x00;
            //    for (int j = 0; j < 8; j++)
            //    {
            //        if (i * 8 + j < Chanels.Count)
            //        {
            //            data1 = (byte)(Chanels[(2 - i) * 8 + (7 - j)].isOn ? (data1 << 1) ^ 0x01 : (data1 << 1) ^ 0x00);
            //        }
            //    }
            //    cardChannel[i + 2] = data1;
            //    Console.WriteLine(Convert.ToString(data1,2));
            //}

            return SerialPort.SendToControls(cardChannel, 500, new byte[] { 0x53, 0x00 });
        }
    }
}
