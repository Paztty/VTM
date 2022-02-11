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

namespace HVT.VTM.Base
{
    public class RelayCard
    {
        public Machine.Port Port1 { get; set; } = new Machine.Port();
        Timer timer = new Timer()
        {
            Interval = 500,
        };


        public static ObservableCollection<RelayChannel> Chanels { get; set; } = new ObservableCollection<RelayChannel>()
        {
           
        };

        public ObservableCollection<RelayChannel> ChanelsEdittingPart1
        {
            get;
            set;
        } = new ObservableCollection<RelayChannel>()
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
            
        };
        public ObservableCollection<RelayChannel> ChanelsEdittingPart2
        {
            get;
            set;
        } = new ObservableCollection<RelayChannel>()
        {
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

        public void UpdateMainRelayChannels(WrapPanel panelSelect, WrapPanel pnMux1, WrapPanel pnMux2, WrapPanel pnVision)
        {

            //panelSelect.Children.Clear();
            pnMux1.Children.Clear();
            pnMux2.Children.Clear();
            pnVision.Children.Clear();


            Chanels.Clear();
            foreach (var item in ChanelsEdittingPart1)
            {
                Chanels.Add(item);
                pnMux1.Children.Add(item.btOn);
            }
            foreach (var item in ChanelsEdittingPart2)
            {
                Chanels.Add(item);
                pnMux2.Children.Add(item.btOn);
            }
            foreach (var item in Chanels)
            {
                //panelSelect.Children.Add(item.CbUse);
                pnVision.Children.Add(item.btOnVision);
                item.STATE_CHANGE += Item_STATE_CHANGE;
            }
        }

        private void Item_STATE_CHANGE(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //timer.Stop();
            //timer.Start();
        }

        public RelayCard(WrapPanel panelSelect)
        {
            panelSelect.Children.Clear();
            foreach (var item in Chanels)
            {
                panelSelect.Children.Add(item.CbUse);
            }
            timer.Elapsed += Timer_Elapsed; ;
            timer.Enabled = true;
            timer.AutoReset = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //timer.Stop();
            //SendCardStatus();
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

        public bool SetPort(
            string portname1, Rectangle portTx1, Rectangle portRx1, Rectangle portStatus1
            )
        {
            var port1ok = Port1.SetPort(portname1, portTx1, portRx1, portStatus1);
            return port1ok;
        }

        public void SendCardStatus()
        {
            timer.Stop();
            byte[] cardChannel = new byte[7];

            cardChannel[0] = (byte)0x52;

            for (int i = 5; i >= 0; i--)
            {
                byte data1 = 0x00;
                for (int j = 0; j < 8; j++)
                {
                    if (i * 8 + j < Chanels.Count)
                    {
                        data1 = (byte)(Chanels[(5 - i) * 8 + j].isOn ? (data1 << 1) ^ 0x01 : (data1 << 1) ^ 0x00);
                    }
                }
                cardChannel[i + 1] = data1;
            }
            Port1.Send(cardChannel);
        }
    }
}
