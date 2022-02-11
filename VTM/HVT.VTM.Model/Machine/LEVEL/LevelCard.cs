using HVT.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HVT.VTM.Base
{
    public class LevelCard
    {
        public SerialDisplay SerialDisplay = new SerialDisplay();

        public SerialPort serialPort = new SerialPort()
        {
            WriteTimeout = 100,
        };

        public DispatcherTimer Sampling = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(100),
        };


        public static ObservableCollection<LevelChannel> Chanels { get; set; } = new ObservableCollection<LevelChannel>()
        {
            new LevelChannel { Channel = 1 },
            new LevelChannel { Channel = 2 },
            new LevelChannel { Channel = 3 },
            new LevelChannel { Channel = 4 },
            new LevelChannel { Channel = 5 },
            new LevelChannel { Channel = 6 },
            new LevelChannel { Channel = 7 },
            new LevelChannel { Channel = 8 },
            new LevelChannel { Channel = 9 },
            new LevelChannel { Channel = 10 },
            new LevelChannel { Channel = 11 },
            new LevelChannel { Channel = 12 },
            new LevelChannel { Channel = 13 },
            new LevelChannel { Channel = 14 },
            new LevelChannel { Channel = 15 },
            new LevelChannel { Channel = 16 },
            new LevelChannel { Channel = 17 },
            new LevelChannel { Channel = 18 },
            new LevelChannel { Channel = 19 },
            new LevelChannel { Channel = 20 },
            new LevelChannel { Channel = 21 },
            new LevelChannel { Channel = 22 },
            new LevelChannel { Channel = 23 },
            new LevelChannel { Channel = 24 },
            new LevelChannel { Channel = 25 },
            new LevelChannel { Channel = 26 },
            new LevelChannel { Channel = 27 },
            new LevelChannel { Channel = 28 },
            new LevelChannel { Channel = 29 },
            new LevelChannel { Channel = 30 },
            new LevelChannel { Channel = 31 },
            new LevelChannel { Channel = 32 }
        };

        public void UpdateMainLevelChannels(WrapPanel panelSelect)
        {
            panelSelect.Children.Clear();

            //Chanels.Clear();
            foreach (var item in Chanels)
            {
                panelSelect.Children.Add(item.CbUse);
                item.STATE_CHANGE += Item_STATE_CHANGE;
            }
        }

        private void Item_STATE_CHANGE(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //timer.Stop();
            //timer.Start();
        }

        public Grid ChartGrid = new Grid()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 62, 62, 62))
        };
        public StackPanel LabelStack = new StackPanel();
        public StackPanel ChartStackBackground = new StackPanel();
        public StackPanel ChartStack = new StackPanel();

        public bool IsInitted = false;

        public LevelCard(WrapPanel panelSelect, Grid PlaceChart)
        {
            if (!IsInitted)
            {

                PlaceChart.Children.Clear();
                ChartGrid.Children.Clear();
                panelSelect.Children.Clear();
                ChartStackBackground.Children.Clear();
                ChartStack.Children.Clear();
                LabelStack.Children.Clear();

                for (int i = 0; i < Chanels.Count; i++)
                {
                    panelSelect.Children.Add(Chanels[i].CbUse);

                    LabelStack.Children.Add(Chanels[i].chartLabel);
                    Chanels[i].lbbackGround.Background = i % 2 == 0 ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.DarkGray);
                    ChartStackBackground.Children.Add(
                        Chanels[i].lbbackGround
                        );
                    ChartStack.Children.Add(Chanels[i].chartPanel);
                }

                ChartGrid.Children.Add(LabelStack);
                ChartGrid.Children.Add(ChartStackBackground);
                ChartGrid.Children.Add(ChartStack);

                ChartGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(30),
                });
                ChartGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = GridLength.Auto,
                });

                Grid.SetColumn(LabelStack, 0);
                Grid.SetColumn(ChartStackBackground, 1);
                Grid.SetColumn(ChartStack, 1);

                Sampling.Tick += Sampling_Tick;
                //Sampling.Start();
                PlaceChart.Children.Add(ChartGrid);
                IsInitted = true;
            }
        }

        Random random = new Random();
        private void Sampling_Tick(object sender, EventArgs e)
        {
            SampleCount++;
            foreach (var item in Chanels)
            {
                if (item.IsUse)
                {
                    item.Samples.Add(new LevelSample()
                    {
                        X = SampleCount,
                        Y = random.Next(0, 2) < 1 ? 5 : 15,
                    });
                    item.Draw();
                }
            }
            if (SampleCount >= TotalSample)
            {
                Sampling.Stop();
                SampleCount = 0;
            }
        }

        public int SampleCount = 0;
        public int TotalSample = 1000;
        public void ClearChart()
        {
            foreach (var item in Chanels)
            {
                item.Samples.Clear();
                item.polygonPoints.Clear();
                item.CharPolyline.Points.Clear();
                item.Draw();
            }
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

        public void SetUseChannels(List<int> useChannels)
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
        public bool SetPort(string portname1, Rectangle portTx1, Rectangle portRx1, Rectangle portStatus1)
        {

            SerialDisplay.TX = portTx1;
            SerialDisplay.RX = portRx1;
            SerialDisplay.IsOpenRect = portStatus1;

            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }

            serialPort = new SerialPort
            {
                PortName = portname1,
                BaudRate = 9600,
                Parity = Parity.None,
                DataBits = 8
            };
            try
            {
                serialPort.Open();

            }
            catch (Exception)
            {

            }

            SerialDisplay.ShowCOMStatus(serialPort.IsOpen);
            return serialPort.IsOpen;
        }
    }
}
