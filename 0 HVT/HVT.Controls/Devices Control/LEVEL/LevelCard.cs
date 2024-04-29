using HVT.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HVT.Controls
{
    public class LevelCard : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event EventHandler PCB_COUNT_CHANGE;
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


        private bool _AllowGetSample = false;
        public bool AllowGetSample
        {
            get { return _AllowGetSample; }
            set
            {
                if (value != _AllowGetSample)
                {
                    _AllowGetSample = value;
                    if (_AllowGetSample)
                    {
                        if (Chanels.Where(x => x.IsUse).Any())
                        {
                            SampleCount = 0;
                            ClearChart();
                            Sampling.Start();
                        }
                        else
                        {
                            _AllowGetSample = false;
                        }
                    }
                    OnPropertyChanged("AllowGetSample");
                }
            }
        }

        public event EventHandler SampleCountChange;
        private int _SampleCount;
        public int SampleCount
        {
            get { return _SampleCount; }
            set
            {
                if (value != _SampleCount) _SampleCount = value;
                OnPropertyChanged("SampleCount");
                SampleCountChange?.Invoke(SampleCount, null);
            }
        }

        public DispatcherTimer Sampling = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(100),
        };

        public ObservableCollection<LevelChannel> Chanels { get; set; } = new ObservableCollection<LevelChannel>()
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
            new LevelChannel { Channel = 32 },
            new LevelChannel { Channel = 33  },
            new LevelChannel { Channel = 34  },
            new LevelChannel { Channel = 35  },
            new LevelChannel { Channel = 36  },
            new LevelChannel { Channel = 37  },
            new LevelChannel { Channel = 38  },
            new LevelChannel { Channel = 39  },
            new LevelChannel { Channel = 40  },
            new LevelChannel { Channel = 41  },
            new LevelChannel { Channel = 42  },
            new LevelChannel { Channel = 43  },
            new LevelChannel { Channel = 44  },
            new LevelChannel { Channel = 45  },
            new LevelChannel { Channel = 46  },
            new LevelChannel { Channel = 47  },
            new LevelChannel { Channel = 48  },
            new LevelChannel { Channel = 49  },
            new LevelChannel { Channel = 50  },
            new LevelChannel { Channel = 51  },
            new LevelChannel { Channel = 52  },
            new LevelChannel { Channel = 53  },
            new LevelChannel { Channel = 54  },
            new LevelChannel { Channel = 55  },
            new LevelChannel { Channel = 56  },
            new LevelChannel { Channel = 57  },
            new LevelChannel { Channel = 58  },
            new LevelChannel { Channel = 59  },
            new LevelChannel { Channel = 60  },
            new LevelChannel { Channel = 61  },
            new LevelChannel { Channel = 62  },
            new LevelChannel { Channel = 63  },
            new LevelChannel { Channel = 64  },
        };

        private int _PCB_Count = 1;
        public int PCB_Count
        {
            get { return _PCB_Count; }
            set
            {
                if (value > 0 && value != _PCB_Count)
                {
                    _PCB_Count = value;
                    OnPropertyChanged("PCB_Count");
                    PCB_COUNT_CHANGE?.Invoke(_PCB_Count, null);
                }
            }
        }

        public LevelCard()
        {
            Sampling.Tick += Sampling_Tick;
        }

        private void Sampling_Tick(object sender, EventArgs e)
        {
            Sampling.Stop();
            if (AllowGetSample)
            {
                SampleCount++;
                if (SampleCount < 1000)
                {
                    Sampling.Start();
                }
                else
                {
                    AllowGetSample = false;
                }
            }

            UInt64 Data64Bit = 0;
            byte[] Response;
            if (SerialPort.SendAndRead(new byte[] { 0x4C }, 0x4C, 500, out Response))
                if (Response.Length > 10)
                {
                    Array.Reverse(Response, 0, Response.Length);

                    //Console.WriteLine();
                    //foreach (var item in Response)
                    //{
                    //    Console.Write(item.ToString("X2") + " ");
                    //}
                    Data64Bit = BitConverter.ToUInt64(Response, 2);
                }

            Console.WriteLine(Data64Bit);

            for (int i = 0; i < Chanels.Count; i++)
            {
                bool lastState = Chanels[i].IsOn;
                Chanels[i].IsOn = GetValue(Data64Bit, i);

                //if (Chanels[i].IsOn)
                //{
                //    Console.WriteLine("{0} - {1}", i + 1, Chanels[i].IsOn);
                //}

                if (AllowGetSample)
                {
                    if (Chanels[i].IsUse)
                    {
                        if (Chanels[i].IsOn != lastState)
                        {
                            if (Chanels[i].Samples.Count > 0)
                            {
                                Chanels[i].Samples.Add(new LevelSample()
                                {
                                    X = SampleCount * 2.5,
                                    Y = lastState ? 5 : 15,
                                });
                                Chanels[i].Draw();
                            }
                        }

                        Chanels[i].Samples.Add(new LevelSample()
                        {
                            X = SampleCount * 2.5,
                            Y = Chanels[i].IsOn ? 5 : 15,
                            Level = Chanels[i].IsOn,
                        });
                        Chanels[i].Draw();
                    }
                }
            }
            //Console.WriteLine();
        }

        public bool GetValue(UInt64 data, int position)
        {

            UInt64 shift = 1;
            //Console.WriteLine( "Positition : {0}" , position);
            //Console.WriteLine(Convert.ToString((long)data,2).PadLeft(64,'0'));
            //Console.WriteLine(Convert.ToString((long)(shift << position),2).PadLeft(64, '0'));
            //Console.WriteLine();
            return (data & (shift << position)) != 0;
        }

        public const int TotalSample = 1000;

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
    }

    public class LevelCardPart
    {
        public string Name { get; set; }
        public List<LevelChannel> Channels { get; set; }
    }
}
