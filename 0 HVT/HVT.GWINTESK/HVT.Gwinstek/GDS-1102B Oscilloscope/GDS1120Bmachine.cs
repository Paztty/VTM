using HVT.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media.TextFormatting;

namespace HVT.Gwinstek
{

    public class GDS1120machine : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Constan
        /// </summary>
        /// 
        public SerialPortDisplay Serial = new SerialPortDisplay()
        {
            DevieceName = "GDS1120"
        };

        private Task StreamValue;

        private bool _AutoSet;
        public bool AutoSet
        {
            get { 
                return _AutoSet;
            }
            set
            {
                if (value != _AutoSet)
                {
                    _AutoSet = value;
                    NotifyPropertyChanged("AutoSet");
                }
            }
        }

        private GDS1120_CHANNEL _CH1 = new GDS1120_CHANNEL() { ChannelNumber = '1' };
        public GDS1120_CHANNEL CH1
        {
            get { return _CH1; }
            set
            {
                if (value != null && value != _CH1)
                {
                    _CH1 = value;
                    NotifyPropertyChanged("CH1");
                }
            }
        }

        private GDS1120_CHANNEL _CH2 = new GDS1120_CHANNEL() { ChannelNumber = '2' };
        public GDS1120_CHANNEL CH2
        {
            get { return _CH2; }
            set
            {
                if (value != null && value != _CH2)
                {
                    _CH2 = value;
                    NotifyPropertyChanged("CH2");
                }
            }
        }

        public GDS1120machine() {
            StartStream();
        }
        public bool SetPort(string COM_NAME)
        {
            if (Serial.Port.IsOpen)
            {
                Serial.Port.Close();
                Task.Delay(50).Wait();
            }

            Serial.Port = new System.IO.Ports.SerialPort()
            {
                PortName = COM_NAME,
                BaudRate = 115200,
            };
            Serial.DevieceName = "GDS-1120";
            Serial.OpenPort();

            return Serial.Port.IsOpen;
        }

        public bool QUERY(string Channel)
        {
            //Setting
            if (!Serial.Port.IsOpen)
            {
                return false;
            }
            Serial.Port.ReadTimeout = 500;
            string Query = String.Format(":MEASure:SOURce1 {0}\r\n :MEASure:RMS?\r\n", Channel);
            //Console.WriteLine(Query);
            Serial.SendString(Query);
            Task.Delay(50).Wait();
            var startTime = DateTime.Now;
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < 500)
            {
                try
                {
                    var dataResponse = Serial.Port.ReadLine();
                    dataResponse = dataResponse.Replace("\r", "");
                    if (Channel == "CH1")
                    {
                        CH1.Amplitude = dataResponse;
                    }
                    else if (Channel == "CH2")
                    {
                        CH2.Amplitude = dataResponse;
                    }
                    goto out1;
                }
                catch (Exception)
                {
                }
            }
            out1:
            Query = String.Format(":MEASure:SOURce1 {0}\r\n :MEASure:FREQuency?\r\n", Channel);
            //Console.WriteLine(Query);
            Serial.SendString(Query);
            Task.Delay(50).Wait();
            startTime = DateTime.Now;
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < 500)
            {
                try
                {
                    var dataResponse = Serial.Port.ReadLine();
                    dataResponse = dataResponse.Replace("\r", "");
                    if (Channel == "CH1")
                    {
                        CH1.Frequency = dataResponse;
                    }
                    else if (Channel == "CH2")
                    {
                        CH2.Frequency = dataResponse;
                    }
                    goto out2;
                }
                catch (Exception)
                {
                }

            }
            out2:
            return false;
        }

        public void StartStream()
        {
            if (StreamValue == null)
            {
                StreamValue = new Task(StreammingValue);
                StreamValue.Start();
            }
            else if (StreamValue.Status != TaskStatus.Running)
            {
                StreamValue = new Task(StreammingValue);
                StreamValue.Start();
            }
        }

        public async void StreammingValue()
        {
            while (true)
            {
                try
                {
                    if (Serial.Port.IsOpen)
                    {
                        if (AutoSet)
                        {
                            Serial.SendString(":CHANnel1:SCALe 5\r\n");
                            Serial.SendString(":CHANnel2:SCALe 5\r\n");
                            Serial.SendString(":CHANnel1:POSition 10\r\n");
                            Serial.SendString(":CHANnel2:POSition -10\r\n");
                            await Task.Delay(1000);
                            AutoSet = false;
                        }
                        else
                        {
                            QUERY("CH1");
                            QUERY("CH2");
                        }
                    }
                }
                catch (Exception)
                {
                }
                await Task.Delay(250);
            }
        }
    }

    public class GDS1120_CHANNEL : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private char _ChannelNumber;
        public char ChannelNumber
        {
            get { return _ChannelNumber; }
            set
            {
                if (value != _ChannelNumber)
                {
                    _ChannelNumber = value;
                    NotifyPropertyChanged("ChannelNumber");
                }
            }
        }

        public double _Frequency = 0;
        public string Frequency
        {
            get { return _Frequency.ToString("r"); }
            set
            {
                double freqBuffer = _Frequency;
                if (double.TryParse(value, out freqBuffer))
                {
                    _Frequency = freqBuffer;
                    NotifyPropertyChanged("Frequency");
                }
                else
                {
                    _Frequency = 0;
                    NotifyPropertyChanged("Frequency");
                }
            }
        }

        public double _Amplitude = 0;
        public string Amplitude
        {
            get { return _Amplitude.ToString(); }
            set
            {
                if (double.TryParse(value, out double buffer))
                {
                    _Amplitude = Math.Round(buffer, 2);
                    NotifyPropertyChanged("Amplitude");
                }
                else
                {
                    _Amplitude = 0;
                    NotifyPropertyChanged("Amplitude");

                }
            }
        }

    }
}
