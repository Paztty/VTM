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
    public class Solenoid
    {
        public Machine.Port Port1 { get; set; } = new Machine.Port();
        Timer timer = new Timer()
        {
            Interval = 500,
        };


        public static ObservableCollection<SolenoidChannel> Chanels { get; set; } = new ObservableCollection<SolenoidChannel>()
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

        public void UpdateMainSolenoidChannels(WrapPanel panel, WrapPanel pnVision)
        {
            panel.Children.Clear();
            pnVision.Children.Clear();

            foreach (var item in Chanels)
            {
                panel.Children.Add(item.btOn);
                pnVision.Children.Add(item.btOnVision);
                item.STATE_CHANGE += Item_STATE_CHANGE;
            }
        }

        private void Item_STATE_CHANGE(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //timer.Stop();
            //timer.Start();
        }

        public Solenoid()
        {
            //timer.Elapsed += Timer_Elapsed;
            //timer.Enabled = true;
            //timer.AutoReset = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //timer.Stop();
            //SendCardStatus();
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
            byte[] cardChannel = new byte[5];

            cardChannel[0] = (byte)0x53;

            for (int i = 2; i >= 0; i--)
            {
                byte data1 = 0x00;
                for (int j = 0; j < 8; j++)
                {
                    if (i * 8 + j < Chanels.Count)
                    {
                        data1 = (byte)(Chanels[(2 - i) * 8 + j].isOn ? (data1 << 1) ^ 0x01 : (data1 << 1) ^ 0x00);
                    }
                }
                cardChannel[i + 1] = data1; 
            }
            Port1.Send(cardChannel);
        }
    }
}
