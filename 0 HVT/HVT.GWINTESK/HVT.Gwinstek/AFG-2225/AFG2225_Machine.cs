using HVT.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media.TextFormatting;

namespace HVT.Gwinstek
{
    public enum AFG2225_FUNCTIONS
    {
        SIN,
        SQU,
        RAMP,
        PULS,
        NOIS,
        USER
    }
    public class AFG2225Machine : INotifyPropertyChanged
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
            DevieceName = "AFG2225"
        };

        private AFG_CHANNEL _CH1 = new AFG_CHANNEL() { ChannelNumber = '1' };
        public AFG_CHANNEL CH1
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

        private AFG_CHANNEL _CH2 = new AFG_CHANNEL() { ChannelNumber = '2' };
        public AFG_CHANNEL CH2
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

        public AFG2225Machine()
        {

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
            Serial.DevieceName = "AFG-2225";
            Serial.OpenPort();

            return Serial.Port.IsOpen;
        }

        public void GET_CURRENT_APPLY()
        {
            if (!Serial.Port.IsOpen)
            {
                return;
            }
            string Channel = "CH1";
            string Ask = String.Format("SOUR{0}:APPL?\r\n",
                Channel == "CH1" ? 1 : Channel == "CH2" ? 2 : 1);
            Console.WriteLine(Ask);
            Serial.SendString(Ask);
            var startTime = DateTime.Now;
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < 200)
            {
                var dataResponse = Serial.Port.ReadLine();
                dataResponse = dataResponse.Replace("\"", "");
                if (dataResponse.Contains("SIN"))
                {
                    dataResponse = dataResponse.Replace("SIN", "");
                    var itemResponse = dataResponse.Split(',');
                    var channelSet = Channel == "CH1" ? CH1 : Channel == "CH2" ? CH2 : null;
                    if (channelSet != null)
                    {
                        channelSet.FUNCTION = AFG2225_FUNCTIONS.SIN;
                        channelSet.Frequency = itemResponse[0];
                        channelSet.Amplitude = itemResponse[1];
                        channelSet.DC_Ofset = itemResponse[2];
                    }
                    goto out1;
                } 
            }
            out1:
            Channel = "CH2";
            Ask = String.Format("SOUR{0}:APPL?\r\n",
            Channel == "CH1" ? 1 : Channel == "CH2" ? 2 : 1);
            Console.WriteLine(Ask);
            Serial.SendString(Ask);
            startTime = DateTime.Now;
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < 200)
            {
                var dataResponse = Serial.Port.ReadLine();
                dataResponse = dataResponse.Replace("\"", "");
                if (dataResponse.Contains("SIN"))
                {
                    dataResponse = dataResponse.Replace("SIN", "");
                    var itemResponse = dataResponse.Split(',');
                    var channelSet = Channel == "CH1" ? CH1 : Channel == "CH2" ? CH2 : null;
                    if (channelSet != null)
                    {
                        channelSet.FUNCTION = AFG2225_FUNCTIONS.SIN;
                        channelSet.Frequency = itemResponse[0];
                        channelSet.Amplitude = itemResponse[1];
                        channelSet.DC_Ofset = itemResponse[2];
                    }
                    goto out2;
                }
            }
        out2:
            return;
        }

        string LastQueryString = "";
        public bool APPLY(string Channel, double frequency, double Amplitude, double DC_offset)
        {
            //Setting
            if (!Serial.Port.IsOpen)
            {
                return false;
            }
            string F_STRING;
            if (frequency / 1000 >= 1)
            {
                F_STRING = Math.Round(frequency / 1000.0, 2) + "KHZ";
            }
            else
            {
                F_STRING = Math.Round(frequency, 2) + "HZ";
            }

            string Query = String.Format("SOUR{0}:APPL:SIN {1},{2},{3}\r\n",
                Channel == "CH1" ? 1 : Channel == "CH2" ? 2 : 1,
                F_STRING,
                Amplitude,
                DC_offset);
            if (Query == LastQueryString)
            {
                return true;
            }
            else
            {
                LastQueryString = Query;
            }

            string setRMS = String.Format("SOUR{0}:VOLT:UNIT VRMS\r\n",
                                Channel == "CH1" ? 1 : Channel == "CH2" ? 2 : 1);
            Task.Delay(100).Wait();
            Serial.SendString(setRMS);
            Console.WriteLine(Query);
            Serial.Port.ReadTimeout = 2000;
            Serial.SendString(Query);
            Task.Delay(1000).Wait();
            //Compare

            string Ask = String.Format("SOUR{0}:APPL?\r\n",
                        Channel == "CH1" ? 1 : Channel == "CH2" ? 2 : 1);
            Console.WriteLine(Ask);
            Serial.SendString(Ask);
            var startTime = DateTime.Now;
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < 5000)
            {
                try
                {
                    var dataResponse = Serial.Port.ReadLine();
                    Console.WriteLine(dataResponse);
                    dataResponse = dataResponse.Replace("\"", "");
                    if (dataResponse.Contains("SIN"))
                    {
                        dataResponse = dataResponse.Replace("SIN", "");
                        var itemResponse = dataResponse.Split(',');
                        var channelSet = Channel == "CH1" ? CH1 : Channel == "CH2" ? CH2 : null;
                        if (channelSet != null)
                        {
                            channelSet.FUNCTION = AFG2225_FUNCTIONS.SIN;
                            channelSet.Frequency = itemResponse[0];
                            channelSet.Amplitude = itemResponse[1];
                            channelSet.DC_Ofset = itemResponse[2];
                            return true;
                        }
                    }
                }
                catch (Exception)
                {
                    return false;
                }

            }
            return false;
        }
    }

    public class AFG_CHANNEL : INotifyPropertyChanged
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

        public AFG2225_FUNCTIONS _FUNCTION = AFG2225_FUNCTIONS.SIN;
        public AFG2225_FUNCTIONS FUNCTION
        {
            get { return _FUNCTION; }
            set
            {
                if (value != _FUNCTION)
                {
                    _FUNCTION = value;
                    NotifyPropertyChanged("FUNCTION");
                    NotifyPropertyChanged("FUNCTION_S");
                    switch (_FUNCTION)
                    {
                        case AFG2225_FUNCTIONS.SIN:
                            Property = (0.0000001, 25000000);
                            break;
                        case AFG2225_FUNCTIONS.SQU:
                            Property = (0.0000001, 25000000);
                            break;
                        case AFG2225_FUNCTIONS.RAMP:
                            Property = (0.0000001, 1000000);
                            break;
                        case AFG2225_FUNCTIONS.PULS:
                            Property = (0.0000001, 25000000);
                            break;
                        case AFG2225_FUNCTIONS.NOIS:
                            Property = (0, 0);
                            break;
                        case AFG2225_FUNCTIONS.USER:
                            Property = (0.0000001, 60000000);
                            break;
                        default:
                            Property = (0, 0);
                            break;
                    }
                }
            }
        }
        public string FUNCTION_S
        {
            get
            {
                switch (_FUNCTION)
                {
                    case AFG2225_FUNCTIONS.SIN:
                        return "SIN";
                    case AFG2225_FUNCTIONS.SQU:
                        return "SQU";
                    case AFG2225_FUNCTIONS.RAMP:
                        return "RAMP";
                    case AFG2225_FUNCTIONS.PULS:
                        return "PULS";
                    case AFG2225_FUNCTIONS.NOIS:
                        return "NOIS";
                    case AFG2225_FUNCTIONS.USER:
                        return "USER";
                    default:
                        return "NON";
                }
            }
        }

        public (double MinimumFreq, double MaximunFreq) _Property;
        public (double MinimumFreq, double MaximunFreq) Property
        {
            get { return _Property; }
            set
            {
                if (value != _Property)
                {
                    _Property = value;
                    NotifyPropertyChanged("Property");
                }
            }
        }


        public double _Frequency;
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
            }
        }

        public double _DC_Ofset;
        public string DC_Ofset
        {
            get { return _DC_Ofset.ToString("r"); }
            set
            {
                double Buffer = _DC_Ofset;
                if (double.TryParse(value, out Buffer))
                {
                    _DC_Ofset = Buffer;
                    NotifyPropertyChanged("DC_Ofset");
                }
            }
        }

        public double _Amplitude;
        public string Amplitude
        {
            get { return _Amplitude.ToString(); }
            set
            {
                if (double.TryParse(value, out double buffer))
                {
                    _Amplitude = buffer;
                    NotifyPropertyChanged("Amplitude");
                }
            }
        }

        public double _Phase;
        public string Phase
        {
            get { return _Phase.ToString(); }
            set
            {
                if (double.TryParse(value, out double buffer))
                {
                    _Phase = buffer;
                    NotifyPropertyChanged("Phase");
                }
            }
        }
    }
}
