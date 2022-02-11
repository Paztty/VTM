using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace HVT.VTM.Base
{
    public class MuxCard : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Machine.Port Port1 { get; set; } = new Machine.Port();
        public Machine.Port Port2 { get; set; } = new Machine.Port();

        public int PCB_Remap_Count = 96;

        public static ObservableCollection<MuxChannel> Chanels { get; set; } = new ObservableCollection<MuxChannel>()
        {
            new MuxChannel { Channel_P = 1, Channel_N = 48 },
            new MuxChannel { Channel_P = 2, Channel_N = 48 },
            new MuxChannel { Channel_P = 3, Channel_N = 48 },
            new MuxChannel { Channel_P = 4, Channel_N = 48 },
            new MuxChannel { Channel_P = 5, Channel_N = 48 },
            new MuxChannel { Channel_P = 6, Channel_N = 48 },
            new MuxChannel { Channel_P = 7, Channel_N = 48 },
            new MuxChannel { Channel_P = 8, Channel_N = 48 },
            new MuxChannel { Channel_P = 9, Channel_N = 48 },
            new MuxChannel { Channel_P = 10, Channel_N = 48 },
            new MuxChannel { Channel_P = 11, Channel_N = 48 },
            new MuxChannel { Channel_P = 12, Channel_N = 48 },
            new MuxChannel { Channel_P = 13, Channel_N = 48 },
            new MuxChannel { Channel_P = 14, Channel_N = 48 },
            new MuxChannel { Channel_P = 15, Channel_N = 48 },
            new MuxChannel { Channel_P = 16, Channel_N = 48 },
            new MuxChannel { Channel_P = 17, Channel_N = 48 },
            new MuxChannel { Channel_P = 18, Channel_N = 48 },
            new MuxChannel { Channel_P = 19, Channel_N = 48 },
            new MuxChannel { Channel_P = 20, Channel_N = 48 },
            new MuxChannel { Channel_P = 21, Channel_N = 48 },
            new MuxChannel { Channel_P = 22, Channel_N = 48 },
            new MuxChannel { Channel_P = 23, Channel_N = 48 },
            new MuxChannel { Channel_P = 24, Channel_N = 48 },
            new MuxChannel { Channel_P = 25, Channel_N = 48 },
            new MuxChannel { Channel_P = 26, Channel_N = 48 },
            new MuxChannel { Channel_P = 27, Channel_N = 48 },
            new MuxChannel { Channel_P = 28, Channel_N = 48 },
            new MuxChannel { Channel_P = 29, Channel_N = 48 },
            new MuxChannel { Channel_P = 30, Channel_N = 48 },
            new MuxChannel { Channel_P = 31, Channel_N = 48 },
            new MuxChannel { Channel_P = 32, Channel_N = 48 },
            new MuxChannel { Channel_P = 33, Channel_N = 48 },
            new MuxChannel { Channel_P = 34, Channel_N = 48 },
            new MuxChannel { Channel_P = 35, Channel_N = 48 },
            new MuxChannel { Channel_P = 36, Channel_N = 48 },
            new MuxChannel { Channel_P = 37, Channel_N = 48 },
            new MuxChannel { Channel_P = 38, Channel_N = 48 },
            new MuxChannel { Channel_P = 39, Channel_N = 48 },
            new MuxChannel { Channel_P = 40, Channel_N = 48 },
            new MuxChannel { Channel_P = 41, Channel_N = 48 },
            new MuxChannel { Channel_P = 42, Channel_N = 48 },
            new MuxChannel { Channel_P = 43, Channel_N = 48 },
            new MuxChannel { Channel_P = 44, Channel_N = 48 },
            new MuxChannel { Channel_P = 45, Channel_N = 48 },
            new MuxChannel { Channel_P = 46, Channel_N = 48 },
            new MuxChannel { Channel_P = 47, Channel_N = 48 },
            new MuxChannel { Channel_P = 48, Channel_N = 48 },
            new MuxChannel { Channel_P = 49, Channel_N = 96 },
            new MuxChannel { Channel_P = 50, Channel_N = 96 },
            new MuxChannel { Channel_P = 51, Channel_N = 96 },
            new MuxChannel { Channel_P = 52, Channel_N = 96 },
            new MuxChannel { Channel_P = 53, Channel_N = 96 },
            new MuxChannel { Channel_P = 54, Channel_N = 96 },
            new MuxChannel { Channel_P = 55, Channel_N = 96 },
            new MuxChannel { Channel_P = 56, Channel_N = 96 },
            new MuxChannel { Channel_P = 57, Channel_N = 96 },
            new MuxChannel { Channel_P = 58, Channel_N = 96 },
            new MuxChannel { Channel_P = 59, Channel_N = 96 },
            new MuxChannel { Channel_P = 60, Channel_N = 96 },
            new MuxChannel { Channel_P = 61, Channel_N = 96 },
            new MuxChannel { Channel_P = 62, Channel_N = 96 },
            new MuxChannel { Channel_P = 63, Channel_N = 96 },
            new MuxChannel { Channel_P = 64, Channel_N = 96 },
            new MuxChannel { Channel_P = 65, Channel_N = 96 },
            new MuxChannel { Channel_P = 66, Channel_N = 96 },
            new MuxChannel { Channel_P = 67, Channel_N = 96 },
            new MuxChannel { Channel_P = 68, Channel_N = 96 },
            new MuxChannel { Channel_P = 69, Channel_N = 96 },
            new MuxChannel { Channel_P = 70, Channel_N = 96 },
            new MuxChannel { Channel_P = 71, Channel_N = 96 },
            new MuxChannel { Channel_P = 72, Channel_N = 96 },
            new MuxChannel { Channel_P = 73, Channel_N = 96 },
            new MuxChannel { Channel_P = 74, Channel_N = 96 },
            new MuxChannel { Channel_P = 75, Channel_N = 96 },
            new MuxChannel { Channel_P = 76, Channel_N = 96 },
            new MuxChannel { Channel_P = 77, Channel_N = 96 },
            new MuxChannel { Channel_P = 78, Channel_N = 96 },
            new MuxChannel { Channel_P = 79, Channel_N = 96 },
            new MuxChannel { Channel_P = 80, Channel_N = 96 },
            new MuxChannel { Channel_P = 81, Channel_N = 96 },
            new MuxChannel { Channel_P = 82, Channel_N = 96 },
            new MuxChannel { Channel_P = 83, Channel_N = 96 },
            new MuxChannel { Channel_P = 84, Channel_N = 96 },
            new MuxChannel { Channel_P = 85, Channel_N = 96 },
            new MuxChannel { Channel_P = 86, Channel_N = 96 },
            new MuxChannel { Channel_P = 87, Channel_N = 96 },
            new MuxChannel { Channel_P = 88, Channel_N = 96 },
            new MuxChannel { Channel_P = 89, Channel_N = 96 },
            new MuxChannel { Channel_P = 90, Channel_N = 96 },
            new MuxChannel { Channel_P = 91, Channel_N = 96 },
            new MuxChannel { Channel_P = 92, Channel_N = 96 },
            new MuxChannel { Channel_P = 93, Channel_N = 96 },
            new MuxChannel { Channel_P = 94, Channel_N = 96 },
            new MuxChannel { Channel_P = 95, Channel_N = 96 },
            new MuxChannel { Channel_P = 96, Channel_N = 96 }
        };

        private ObservableCollection<MuxChannel> chanelsEdittingPart1 = new ObservableCollection<MuxChannel>();
        public ObservableCollection<MuxChannel> ChanelsEdittingPart1
        {
            get { return chanelsEdittingPart1; }
            set
            {
                if (chanelsEdittingPart1 != value)
                {
                    chanelsEdittingPart1 = value;
                    NotifyPropertyChanged(nameof(ChanelsEdittingPart1));
                }
            }
        }
        private ObservableCollection<MuxChannel> chanelsEdittingPart2 = new ObservableCollection<MuxChannel>();
        public ObservableCollection<MuxChannel> ChanelsEdittingPart2
        {
            get { return chanelsEdittingPart2; }
            set
            {
                if (chanelsEdittingPart2 != value)
                {
                    chanelsEdittingPart2 = value;
                    NotifyPropertyChanged(nameof(ChanelsEdittingPart2));
                }
            }
        }

        public void UpdateMainMuxChannels(WrapPanel panelSelect, WrapPanel pnMux1, WrapPanel pnMux2)
        {

            panelSelect.Children.Clear();
            pnMux1?.Children.Clear();
            pnMux2?.Children.Clear();

            foreach (var item in Chanels)
            {
                if (item.channel_P < 49)
                {
                    ChanelsEdittingPart1.Add(item);
                    pnMux1.Children.Add(item.btOn);
                }
                else
                {
                    ChanelsEdittingPart2.Add(item);
                    pnMux2.Children.Add(item.btOn);
                }
                panelSelect.Children.Add(item.CbUse);
            }
        }

        public MuxCard(WrapPanel panelSelect)
        {
            panelSelect.Children.Clear();
            foreach (var item in Chanels)
            {
                panelSelect.Children.Add(item.CbUse);
            }
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
            string portname1, Rectangle portTx1, Rectangle portRx1, Rectangle portStatus1,
            string portname2, Rectangle portTx2, Rectangle portRx2, Rectangle portStatus2)
        {
            var port1ok = Port1.SetPort(portname1, portTx1, portRx1, portStatus1);
            var port2ok = Port2.SetPort(portname2, portTx2, portRx2, portStatus2);
            return port1ok & port2ok;
        }

        public void SetChannels(string setParam)
        {
            for (int i = 0; i < Chanels.Count; i++)
            {
                if (Int32.TryParse(setParam, out int channel_select))
                {
                    Chanels[i].IsON = false;
                    Chanels[channel_select - 1].IsON = true;
                    for (int PCB = 1; PCB < 5; PCB++)
                    {
                        if (channel_select - 1 + PCB_Remap_Count < Chanels.Count)
                        {
                            Chanels[channel_select - 1 + PCB_Remap_Count].IsON = true;
                        }
                    }
                }
            }
            SendCardStatus();
        }

        public void SendCardStatus()
        {
            byte[] cardChannel1 = new byte[13];
            byte[] cardChannel2 = new byte[13];

            cardChannel1[0] = (byte)0x4D;
            cardChannel2[0] = (byte)0x6D;

            for (int i = 5; i >= 0; i--)
            {
                byte data1 = 0x00, data2 = 0x00;
                byte data3 = 0x00, data4 = 0x00;
                for (int j = 0; j < 8; j++)
                {
                    if ((5 - i) * 8 + j < ChanelsEdittingPart1.Count)
                    {
                        data1 = (byte)(ChanelsEdittingPart1[(5 - i) * 8 + j].isOn ? (data1 << 1) ^ 0x01 : (data1 << 1) ^ 0x00);
                        data2 = (byte)(ChanelsEdittingPart1[(5 - i) * 8 + j].isOn ? (data2 << 1) ^ 0x01 : (data2 << 1) ^ 0x00);
                    }
                    if ((5 - i) * 8 + j < ChanelsEdittingPart2.Count)
                    {
                        data3 = (byte)(ChanelsEdittingPart2[(5 - i) * 8 + j].isOn ? (data3 << 1) ^ 0x01 : (data3 << 1) ^ 0x00);
                        data4 = (byte)(ChanelsEdittingPart2[(5 - i) * 8 + j].isOn ? (data4 << 1) ^ 0x01 : (data4 << 1) ^ 0x00);
                    }
                }
                cardChannel1[i + 1] = data1;
                cardChannel1[i + 7] = data2;

                cardChannel2[i + 1] = data3;
                cardChannel2[i + 7] = data4;
            }
            Port1.Send(cardChannel1);
            Port2.Send(cardChannel2);
        }

        public void UpdateCardSelect(int PCB_Count, WrapPanel panelSelect)
        {
            PCB_Remap_Count = 4 % PCB_Count > 0 ? 96 / (PCB_Count + 1) : 96 / PCB_Count;
            ChanelsEdittingPart1.Clear();
            ChanelsEdittingPart2.Clear();
            panelSelect.Children.Clear();
            if (PCB_Count > 1)
            {
                for (int channel = 0; channel < PCB_Remap_Count; channel++)
                {
                    ChanelsEdittingPart1.Add(Chanels[channel]);
                    panelSelect.Children.Add(Chanels[channel].CbUse);
                }
            }
            else
            {
                foreach (var item in Chanels)
                {
                    if (item.channel_P < 49)
                    {
                        ChanelsEdittingPart1.Add(item);
                    }
                    else
                    {
                        ChanelsEdittingPart2.Add(item);
                    }
                    panelSelect.Children.Add(item.CbUse);
                }
            }
        }

        public List<int> GetUseChannels()
        {
            List<int> useChannels = new List<int>();
            for (int i = 0; i < Chanels.Count; i++)
            {
                if (Chanels[i].IsUse)
                {
                    useChannels.Add(i);
                }
            }
            return useChannels;
        }

        public void SetUseChannels( List<int> useChannels)
        {
            if (useChannels == null || useChannels.Count < 1)
            {
                return;
            }
            foreach (var item in useChannels)
            {
                if (item < Chanels.Count)
                {
                    Chanels[item].IsUse = true;
                }
            }
        }

    }
}
