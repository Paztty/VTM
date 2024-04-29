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

namespace HVT.Controls
{
    public class MuxCard : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler PCB_COUNT_CHANGE;

        private SerialPortDisplay serial1;

        public SerialPortDisplay SerialPort1
        {
            get { return serial1; }
            set
            {
                serial1 = value;
            }
        }

        private SerialPortDisplay serial2;

        public SerialPortDisplay SerialPort2
        {
            get { return serial2; }
            set
            {
                serial2 = value;
            }
        }


        public int PCB_Remap_Count { get; set; } = 96;
        private int pcb_count = 4;
        public int PCB_Count
        {
            get { return pcb_count; }
            set
            {
                if (value != pcb_count)
                {
                    pcb_count = value;
                    PCB_COUNT_CHANGE?.Invoke(pcb_count, null);
                }
            }
        }

        public ObservableCollection<MuxChannel> Chanels { get; set; } = new ObservableCollection<MuxChannel>()
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


        private MuxChannel _CurrentChannel = new MuxChannel();
        public MuxChannel CurrentChannel
        {
            get { return _CurrentChannel; }
            set
            {
                if (value != null || value != _CurrentChannel) _CurrentChannel = value;
                NotifyPropertyChanged("CurrentChannel");
            }
        }

        public void UpdateMainMuxChannels(WrapPanel panelSelect, WrapPanel pnMux1, WrapPanel pnMux2)
        {
            panelSelect.Children.Clear();
            pnMux1?.Children.Clear();
            pnMux2?.Children.Clear();

            foreach (var item in Chanels)
            {
                if (item.Channel_P < 49)
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
                item.ManualStateChange += Item_ManualStateChange;
            }
        }
        public MuxCard()
        {
            foreach (var item in Chanels)
            {
                if (item.Channel_P < 49)
                {
                    ChanelsEdittingPart1.Add(item);
                }
                else
                {
                    ChanelsEdittingPart2.Add(item);
                }
                item.ManualStateChange += Item_ManualStateChange;
            }
        }
        public void Update()
        {
            foreach (var item in Chanels)
            {
                if (item.Channel_P < 49)
                {
                    ChanelsEdittingPart1.Add(item);
                }
                else
                {
                    ChanelsEdittingPart2.Add(item);
                }
                item.ManualStateChange += Item_ManualStateChange;
            }

            for (int i = 0; i < PCB_Remap_Count; i++)
            {
                if (Chanels[i].IsUse)
                {
                    for (int pcb = 0; pcb < PCB_Count; pcb++)
                    {
                        Chanels[i + PCB_Remap_Count * pcb].IsUse = Chanels[i].IsUse;
                    }
                }
            }
        }

        private void Item_ManualStateChange(object sender, EventArgs e)
        {
            CurrentChannel = (sender as MuxChannel);
            
            for (int i = 0; i < Chanels.Count; i++)
            {
                if (Chanels[i] != CurrentChannel)
                {
                    Chanels[i].IsON = false;
                    Chanels[i].btOn.IsChecked = false;
                }
            }
            SendCardStatus();
        }

        public void SelectAll()
        {
            foreach (var item in Chanels)
            {
                item.IsUse = true;
            }
        }

        public void ClearAll()
        {
            foreach (var item in Chanels)
            {
                item.IsUse = false;
            }
        }

        public bool SetPort()
        {
            return false;
        }

        public void SetChannels(string setParam)
        {
            for (int i = 0; i < PCB_Remap_Count; i++)
            {
                if (Int32.TryParse(setParam, out int channel_select))
                {
                    Chanels[i].IsON = false;
                    Chanels[channel_select - 1].IsON = true;
                    for (int PCB = 1; PCB < Chanels.Count / PCB_Remap_Count; PCB++)
                    {
                        if (channel_select - 1 + PCB_Remap_Count < Chanels.Count)
                        {
                            Chanels[channel_select - 1 + PCB_Remap_Count * PCB].Channel_N = Chanels[channel_select - 1].Channel_N + PCB_Remap_Count * PCB;
                            Chanels[channel_select - 1 + PCB_Remap_Count * PCB].IsON = true;
                        }
                    }
                }
            }
            SendCardStatus();
        }

        public void SetChannels(string setParam, int PCB1, int PCB2)
        {
            for (int i = 0; i < PCB_Remap_Count; i++)
            {
                if (Int32.TryParse(setParam, out int channel_select))
                {
                    Chanels[i].IsON = false;
                    Chanels[channel_select - 1].IsON = true;
                    Chanels[channel_select - 1 + PCB_Remap_Count * PCB1].Channel_N = Chanels[channel_select - 1].Channel_N + PCB_Remap_Count * PCB1;
                    Chanels[channel_select - 1 + PCB_Remap_Count * PCB1].IsON = true;
                    Chanels[channel_select - 1 + PCB_Remap_Count * PCB2].Channel_N = Chanels[channel_select - 1].Channel_N + PCB_Remap_Count * PCB2;
                    Chanels[channel_select - 1 + PCB_Remap_Count * PCB2].IsON = true;
                }
            }
            SendCardStatus();
        }

        public void ReleaseChannels()
        {
            for (int i = 0; i < Chanels.Count; i++)
            {
                Chanels[i].IsON = false;
            }
            SendCardStatus(true);
            Task.Delay(50).Wait();
        }

        public void SendCardStatus(bool off_all = false)
        {
            byte[] cardChannel1 = new byte[13];
            byte[] cardChannel2 = new byte[13];

            cardChannel1[0] = (byte)0x4D;
            cardChannel2[0] = (byte)0x6D;
            if (!off_all)
            {
                var OnChannels = Chanels.Where(o => o.IsON == true).ToList();
                foreach (var item in OnChannels)
                {
                    Console.WriteLine(item.Channel_P + "+++" + item.Channel_P / 8 + "====" + item.Channel_P % 9 + "====" + ((byte)(0b00000001 << (item.Channel_P - 1) % 8)).ToString("X"));

                    if (item.Channel_P < 49)
                    {
                        cardChannel1[6 - (item.Channel_P - 1) / 8] |= (byte)(0b00000001 << ((item.Channel_P - 1) % 8));
                        cardChannel1[12 - (item.Channel_N - 1) / 8] |= (byte)(0b00000001 << ((item.Channel_N - 1) % 8));
                    }
                    else
                    {
                        cardChannel2[6 - (item.Channel_P - 49) / 8] |= (byte)(0x00000001 << ((item.Channel_P - 1) % 8));
                        cardChannel2[12 - (item.Channel_N - 49) / 8] |= (byte)(0x00000001 << ((item.Channel_N - 1) % 8));
                    }
                }
            }
            else {
                for (int i = 1; i < 13; i++)
                {
                    cardChannel1[i] = (byte)0x00;
                    cardChannel2[i] = (byte)0x00;
                }
            }
            SerialPort1.SendToControls(cardChannel1);
            SerialPort2.SendToControls(cardChannel2);
        }

        public bool ManualSetCardStatus(int P, int N)
        {
            byte[] cardChannel1 = new byte[13];
            byte[] cardChannel2 = new byte[13];

            cardChannel1[0] = (byte)0x4D;
            cardChannel2[0] = (byte)0x6D;
            for (int i = 0; i < Chanels.Count; i++)
            {
                Chanels[i].IsON = false;
            }
            Chanels[P - 1].IsON = true;

            //CurrentChannel = new MuxChannel() { Channel_P = P, Channel_N = N };

            if (P < 49)
            {
                cardChannel1[6 - (P - 1) / 8] |= (byte)(0b00000001 << ((P - 1) % 8));
                cardChannel1[12 - (N - 1) / 8] |= (byte)(0b00000001 << ((N - 1) % 8));
                return SerialPort1.SendToControls(cardChannel1, 500, new byte[] { 0x4D, 0x00 });
            }
            else
            {
                cardChannel2[6 - (P - 49) / 8] |= (byte)(0x00000001 << ((P - 1) % 8));
                cardChannel2[12 - (N - 49) / 8] |= (byte)(0x00000001 << ((N - 1) % 8));
                return SerialPort2.SendToControls(cardChannel2, 500, new byte[] { 0x6D, 0x00 });
            }
        }

        public bool SetCardStatus(int P, int N)
        {
            byte[] cardChannel1 = new byte[13];
            byte[] cardChannel2 = new byte[13];

            cardChannel1[0] = (byte)0x4D;
            cardChannel2[0] = (byte)0x6D;
            for (int i = 0; i < Chanels.Count; i++)
            {
                Chanels[i].IsON = false;
            }
            Chanels[P - 1].IsON = true;


            if (P < 49)
            {
                cardChannel1[6 - (P - 1) / 8] |= (byte)(0b00000001 << ((P - 1) % 8));
                cardChannel1[12 - (N - 1) / 8] |= (byte)(0b00000001 << ((N - 1) % 8));
                return SerialPort1.SendToControls(cardChannel1, 500, new byte[] { 0x4D, 0x00 });
            }
            else
            {
                cardChannel2[6 - (P - 49) / 8] |= (byte)(0x00000001 << ((P - 1) % 8));
                cardChannel2[12 - (N - 49) / 8] |= (byte)(0x00000001 << ((N - 1) % 8));
                return SerialPort2.SendToControls(cardChannel2, 500, new byte[] { 0x6D, 0x00 });
            }
        }

        public bool ManualSetCardStatus(int P)
        {
            byte[] cardChannel1 = new byte[13];
            byte[] cardChannel2 = new byte[13];

            cardChannel1[0] = (byte)0x4D;
            cardChannel2[0] = (byte)0x6D;
            for (int i = 0; i < Chanels.Count; i++)
            {
                Chanels[i].IsON = false;
            }
            Chanels[P - 1].IsON = true;
            int N = Chanels[P - 1].Channel_N;

            if (P < 49)
            {
                cardChannel1[6 - (P - 1) / 8] |= (byte)(0b00000001 << ((P - 1) % 8));
                cardChannel1[12 - (N - 1) / 8] |= (byte)(0b00000001 << ((N - 1) % 8));
                return SerialPort1.SendToControls(cardChannel1, 500, new byte[] { 0x4D, 0x00 });
            }
            else
            {
                cardChannel2[6 - (P - 49) / 8] |= (byte)(0x00000001 << ((P - 1) % 8));
                cardChannel2[12 - (N - 49) / 8] |= (byte)(0x00000001 << ((N - 1) % 8));
                return SerialPort2.SendToControls(cardChannel2, 500, new byte[] { 0x6D, 0x00 });
            }
        }

        public void UpdateCardSelect(WrapPanel panelSelect)
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
                    if (item.Channel_P < 49)
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

    }
}
