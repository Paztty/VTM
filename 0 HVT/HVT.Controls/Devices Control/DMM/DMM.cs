using HVT.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace HVT.Controls
{
    public class DMM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public enum DMM_DCV_Range
        {
            DC100mV,
            DC1V,
            DC10V,
            DC100V,
            DC1000V
        }
        public enum DMM_ACV_Range
        {
            AC100mV,
            AC1V,
            AC10V,
            AC100V,
            AC750V
        }

        public enum DMM_RES_Range
        {
            R100Ω,
            R1kΩ,
            R10kΩ,
            R100kΩ,
            R1MΩ,
            R10MΩ,
            R100MΩ
        }

        public enum DMM_Mode
        {
            NONE,
            DCV,
            ACV,
            FREQ,
            RES,
            DIODE
        }

        public enum DMM_Rate
        {
            NONE,
            SLOW,
            MID,
            FAST
        }


        public string Name = "DMM";

        // Command
        const string CALL = "*IDN?";

        // string value
        public string MinStringValue;
        public string AvgStringValue;
        public string MaxStringValue;

        // numberic value
        private double lastValue = 0;
        public double LastDoubleValue
        {
            get { return lastValue; }
            set
            {
                if (lastValue != value)
                {
                    lastValue = value;
                    NotifyPropertyChanged(nameof(LastDoubleValue));
                    NotifyPropertyChanged(nameof(LastStringValue));
                }
            }
        }
        public string LastStringValue
        {
            get
            {
                return GetCurrentValue();
            }
        }
        // Voltage DC
        public DMM_DCV_Range DCV_Range { get; set; } = DMM_DCV_Range.DC100mV;
        public double DCV_Min { get; set; }
        public double DCV_Max { get; set; }
        public double DCV_Arg { get; set; }
        public double DCV { get; set; }
        // Voltage AC
        public DMM_ACV_Range ACV_Range { get; set; } = DMM_ACV_Range.AC100mV;
        public double ACV_Min { get; set; }
        public double ACV_Max { get; set; }
        public double ACV_Arg { get; set; }
        public double ACV { get; set; }
        // Frequecy
        public double Freq_Min { get; set; }
        public double Freq_Max { get; set; }
        public double Freq_Arg { get; set; }
        public double Freq { get; set; }

        // Res
        public DMM_RES_Range RES_Range { get; set; } = DMM_RES_Range.R100Ω;
        public double RES_Min { get; set; }
        public double RES_Max { get; set; }
        public double RES_Arg { get; set; }
        public double RES { get; set; }

        // Diode
        public double DIODE_Min { get; set; }
        public double DIODE_Max { get; set; }
        public double DIODE_Arg { get; set; }
        public double DIODE { get; set; }

        // Mode
        public DMM_Mode Mode = DMM_Mode.NONE;
        public DMM_Rate DCrate = DMM_Rate.NONE;
        public DMM_Rate ACrate = DMM_Rate.NONE;
        public DMM_Rate FREQrate = DMM_Rate.NONE;
        public DMM_Rate RESrate = DMM_Rate.NONE;
        public DMM_Rate DIODErate = DMM_Rate.NONE;
        

        public List<double> ValuesCount1 = new List<double>();
        public List<double> ValuesCount2 = new List<double>();
        public double AVR_1
        {
            get
            {
                return ValuesCount1.Sum() / (double)ValuesCount1.Count;
            }
        }
        public double AVR_2
        {
            get
            {
                return ValuesCount2.Sum() / (double)ValuesCount2.Count;
            }
        }
        public double MAX_1
        {
            get
            {
                return ValuesCount1.Max();
            }
        }
        public double MAX_2
        {
            get
            {
                return ValuesCount2.Max();
            }
        }
        public double MIN_1
        {
            get
            {
                return ValuesCount1.Min();
            }
        }
        public double MIN_2
        {
            get
            {
                return ValuesCount2.Min();
            }
        }
        // Event

        public EventHandler DMM_MODE_CHANGE;

        private bool isAutoUpdate = false;
        public bool IsAutoUpdate
        {
            get { return isAutoUpdate; }
            set
            {
                if (value != isAutoUpdate)
                {
                    isAutoUpdate = value;
                }
            }
        }

        // Update parameter
        public int Time = 100;
        public Timer UpdateValueTimer = new Timer() { AutoReset = true, Interval = 500 };

        // comunication
        private SerialPortDisplay serialPort = new SerialPortDisplay();
        public SerialPortDisplay SerialPort
        {
            get { return serialPort; }
            set
            {
                serialPort = value;
            }
        }

        // Task list

        public bool IsMatchCalculate { get; set; } = false;
        public bool IsCancelUpdateTask { get; set; } = false;

        // Function
        public DMM()
        {
            SerialPort.Port = new SerialPort()
            {
                PortName = "COM1",
                BaudRate = 115200,
            };
            UpdateValueTimer.Elapsed += UpdateValueTimer_Elapsed;
        }
        public DMM(string Name)
        {
            this.Name = Name;
            SerialPort.DevieceName = Name;
            SerialPort.BlinkTime = 50;
            SerialPort.Port = new SerialPort()
            {
                PortName = "COM1",
                BaudRate = 115200,
                ReadTimeout = 500,
            };
            UpdateValueTimer.Elapsed += UpdateValueTimer_Elapsed;
        }

        public async void CheckCommunication(string COM_NAME)
        {
            var checkResult = await SerialPort.CheckComPort(COM_NAME, 115200, "*IDN?", ",GDM8261A", 500, this.Name);

            if (checkResult)
            {
                Send("SYSTem:RWLock");
                SetMode(DMM_Mode.DCV);
                ChangeRange((int)DMM_DCV_Range.DC1000V);
                ChangeRate(DMM_Rate.MID);
            }

        }

        private void UpdateValueTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateValueTimer.Stop();
            GetValue();
            if (IsAutoUpdate)
            {
                UpdateValueTimer.Start();
            }
        }

        public void Send(string Content)
        {
            Console.WriteLine("{0} -> {1}", Name, Content);
            SerialPort?.SendString(Content + "\r\n");
        }
        public void UpdateValue(bool isStart, int Preiod)
        {
            UpdateValueTimer.Interval = Preiod;
            UpdateValueTimer.Enabled = isStart;
            IsAutoUpdate = isStart;
            if (isStart)
            {
                UpdateValueTimer.Start();
            }
            else
            {
                UpdateValueTimer.Stop();
            }
        }

        public void GetValue()
        {
            if (SerialPort.Port.IsOpen)
            {
                SerialPort.Port.DiscardInBuffer();
            }
            else
            {
                LastDoubleValue = -9999;
                return;
            }

            Send("Val1?");
            Task.Delay(50).Wait();
            string val = SerialPort.ReadLine();
            double number_val = 0;
            if (val == "ERROR")
            {
                LastDoubleValue = -9999;
            }
            if (double.TryParse(val, out number_val))
            {
                LastDoubleValue = number_val;
            }
        }

        private int sampleCount = 0;

        public void RequestValues(int count)
        {
            if (SerialPort.Port.IsOpen)
            {
                SerialPort.Port.DiscardInBuffer();
            }
            else
            {
                LastDoubleValue = -9999;
                return;
            }
            Console.WriteLine("{0} read timeout {1}ms", Name, SerialPort.Port.ReadTimeout);
            if (sampleCount != count)
            {
                Send(String.Format("SAMPle:COUNt " + count.ToString()));
                sampleCount = count;
            }
            Send("Val1?");
        }

        public void GetValue(int count, int stack)
        {
            if (!SerialPort.Port.IsOpen)
            {
                LastDoubleValue = -9999;
                return;
            }

            if (stack == 1 && count == 0)
            {
                ValuesCount1.Clear();
            }
            else
            {
                ValuesCount2.Clear();
            }
            Task.Delay(50).Wait();
            string val = SerialPort.ReadLine();
            Console.WriteLine("{0} -> val {1}: {2}", Name, count, val);
            if (val == "ERROR")
            {
                LastDoubleValue = -9999;
            }
            if (double.TryParse(val, out double number_val))
            {
                LastDoubleValue = number_val;
                if (stack == 1)
                {
                    ValuesCount1.Add(LastDoubleValue);
                }
                else
                {
                    ValuesCount2.Add(LastDoubleValue);
                }
            }
        }


        private string GetCurrentValue()
        {
            double currentValue = lastValue;

            string StringValue = "";

            if (currentValue == -9999)
            {
                StringValue = "Sys";
                return StringValue;
            }

            if (System.Math.Abs(currentValue) < 0.001)
            {
                StringValue = Math.Round((currentValue * 1000000),1) + " u";
            }
            else if (System.Math.Abs(currentValue) < 1)
            {
                StringValue = Math.Round((currentValue * 1000), 1) + " m";
            }
            else if (System.Math.Abs(currentValue) < 1000)
            {
                StringValue = Math.Round(currentValue, 1) + " ";
            }
            else if (System.Math.Abs(currentValue) < 1000000)
            {
                StringValue = Math.Round((currentValue / 1000), 1) + " k";
            }
            else
            {
                StringValue = Math.Round((currentValue / 1000000), 1) + " M";
            }

            switch (Mode)
            {
                case DMM_Mode.NONE:
                    break;
                case DMM_Mode.DCV:
                    StringValue += "VDC";
                    break;
                case DMM_Mode.ACV:
                    StringValue += "VAC";
                    break;
                case DMM_Mode.FREQ:
                    StringValue += "Hz";
                    break;
                case DMM_Mode.RES:
                    StringValue += "Ohm";
                    break;
                case DMM_Mode.DIODE:
                    StringValue += "VDC";
                    break;
                default:
                    break;
            }
            if (currentValue == +1.200000E+37)
            {
                StringValue = "OVR";
            }
            return StringValue;
        }
        public string GetStringValue(double value)
        {
            double currentValue = value;

            string StringValue = "";

            if (currentValue == -9999)
            {
                StringValue = "Sys";
                return StringValue;
            }

            if (System.Math.Abs(currentValue) < 0.001)
            {
                StringValue = Math.Round((currentValue * 1000000), 1) + " u";
            }
            else if (System.Math.Abs(currentValue) < 1)
            {
                StringValue = Math.Round((currentValue * 1000), 1) + " m";
            }
            else if (System.Math.Abs(currentValue) < 1000)
            {
                StringValue = Math.Round(currentValue, 1) + " ";
            }
            else if (System.Math.Abs(currentValue) < 1000000)
            {
                StringValue = Math.Round((currentValue / 1000), 1) + " k";
            }
            else
            {
                StringValue = Math.Round((currentValue / 1000000), 1) + " M";
            }

            if (currentValue == +1.200000E+37)
            {
                StringValue = "OL ";
            }

            switch (Mode)
            {
                case DMM_Mode.NONE:
                    break;
                case DMM_Mode.DCV:
                    StringValue += "VDC";
                    break;
                case DMM_Mode.ACV:
                    StringValue += "VAC";
                    break;
                case DMM_Mode.FREQ:
                    StringValue += "Hz";
                    break;
                case DMM_Mode.RES:
                    StringValue += "OHm";
                    break;
                case DMM_Mode.DIODE:
                    StringValue += "VDC";
                    break;
                default:
                    break;
            }
            return StringValue;
        }

        public bool IsModeChange = false;

        public bool SetMode(DMM_Mode mode)
        {
            if (Mode != mode)
            {
                Mode = mode;

                switch (mode)
                {
                    case DMM_Mode.NONE:
                        break;
                    case DMM_Mode.DCV:
                        Send("CONFigure:VOLTage:DC 100");
                        break;
                    case DMM_Mode.ACV:
                        Send("CONFigure:VOLTage:AC 750");
                        break;
                    case DMM_Mode.FREQ:
                        Send("CONFigure:FREQuency 750");
                        break;
                    case DMM_Mode.RES:
                        Send("CONFigure:RESistance 10000");
                        break;
                    case DMM_Mode.DIODE:
                        Send("CONFigure:DIODe");
                        break;
                    default:
                        return false;
                }
                IsModeChange = true;
            }
            else
                IsModeChange = false;
            return IsModeChange;
        }

        public void ChangeRange(int RangeSelected)
        {
            switch (Mode)
            {
                case DMM_Mode.NONE:
                    break;
                case DMM_Mode.DCV:
                    if (RangeSelected < Enum.GetNames(typeof(DMM_DCV_Range)).Length)
                    {
                        if (DCV_Range == (DMM_DCV_Range)RangeSelected) return;
                        DCV_Range = (DMM_DCV_Range)RangeSelected;
                        switch ((DMM_DCV_Range)RangeSelected)
                        {
                            case DMM_DCV_Range.DC100mV:
                                Send("CONFigure:VOLTage:DC 0.1");
                                break;
                            case DMM_DCV_Range.DC1V:
                                Send("CONFigure:VOLTage:DC 1");
                                break;
                            case DMM_DCV_Range.DC10V:
                                Send("CONFigure:VOLTage:DC 10");
                                break;
                            case DMM_DCV_Range.DC100V:
                                Send("CONFigure:VOLTage:DC 100");
                                break;
                            case DMM_DCV_Range.DC1000V:
                                Send("CONFigure:VOLTage:DC 1000");
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case DMM_Mode.ACV:

                    if (RangeSelected < Enum.GetNames(typeof(DMM_ACV_Range)).Length)
                    {
                        if (ACV_Range == (DMM_ACV_Range)RangeSelected) return;
                        ACV_Range = (DMM_ACV_Range)RangeSelected;
                        switch ((DMM_ACV_Range)RangeSelected)
                        {
                            case DMM_ACV_Range.AC100mV:
                                Send("CONFigure:VOLTage:AC 0.1");
                                break;
                            case DMM_ACV_Range.AC1V:
                                Send("CONFigure:VOLTage:AC 1");
                                break;
                            case DMM_ACV_Range.AC10V:
                                Send("CONFigure:VOLTage:AC 10");
                                break;
                            case DMM_ACV_Range.AC100V:
                                Send("CONFigure:VOLTage:AC 100");
                                break;
                            case DMM_ACV_Range.AC750V:
                                Send("CONFigure:VOLTage:AC 750");
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case DMM_Mode.FREQ:

                    if (RangeSelected < Enum.GetNames(typeof(DMM_ACV_Range)).Length)
                    {
                        if (ACV_Range == (DMM_ACV_Range)RangeSelected) return;
                        ACV_Range = (DMM_ACV_Range)RangeSelected;
                        switch ((DMM_ACV_Range)RangeSelected)
                        {
                            case DMM_ACV_Range.AC100mV:
                                Send("CONFigure:FREQuency 0.1");
                                break;
                            case DMM_ACV_Range.AC1V:
                                Send("CONFigure:FREQuency 1");
                                break;
                            case DMM_ACV_Range.AC10V:
                                Send("CONFigure:FREQuency 10");
                                break;
                            case DMM_ACV_Range.AC100V:
                                Send("CONFigure:FREQuency 100");
                                break;
                            case DMM_ACV_Range.AC750V:
                                Send("CONFigure:FREQuency 750");
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case DMM_Mode.RES:

                    if (RangeSelected < Enum.GetNames(typeof(DMM_RES_Range)).Length)
                    {
                        if (RES_Range == (DMM_RES_Range)RangeSelected) return;
                        RES_Range = (DMM_RES_Range)RangeSelected;
                        switch ((DMM_RES_Range)RangeSelected)
                        {
                            case DMM_RES_Range.R100Ω:
                                Send("CONFigure:RESistance 100");
                                break;
                            case DMM_RES_Range.R1kΩ:
                                Send("CONFigure:RESistance 1000");
                                break;
                            case DMM_RES_Range.R10kΩ:
                                Send("CONFigure:RESistance 10000");
                                break;
                            case DMM_RES_Range.R100kΩ:
                                Send("CONFigure:RESistance 100000");
                                break;
                            case DMM_RES_Range.R1MΩ:
                                Send("CONFigure:RESistance 1000000");
                                break;
                            case DMM_RES_Range.R10MΩ:
                                Send("CONFigure:RESistance 10000000");
                                break;
                            case DMM_RES_Range.R100MΩ:
                                Send("CONFigure:RESistance 100000000");
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case DMM_Mode.DIODE:
                    Send("CONFigure:DIODe");
                    Task.Delay(500).Wait();
                    break;
                default:
                    break;
            }


        }
        public void ChangeRange(int RangeSelected, DMM_Mode mode)
        {
            switch (mode)
            {
                case DMM_Mode.NONE:
                    break;
                case DMM_Mode.DCV:
                    if (RangeSelected < Enum.GetNames(typeof(DMM_DCV_Range)).Length)
                    {
                        if (DCV_Range == (DMM_DCV_Range)RangeSelected && mode == Mode) return;
                        DCV_Range = (DMM_DCV_Range)RangeSelected;
                        switch ((DMM_DCV_Range)RangeSelected)
                        {
                            case DMM_DCV_Range.DC100mV:
                                Send("CONFigure:VOLTage:DC 0.1");
                                break;
                            case DMM_DCV_Range.DC1V:
                                Send("CONFigure:VOLTage:DC 1");
                                break;
                            case DMM_DCV_Range.DC10V:
                                Send("CONFigure:VOLTage:DC 10");
                                break;
                            case DMM_DCV_Range.DC100V:
                                Send("CONFigure:VOLTage:DC 100");
                                break;
                            case DMM_DCV_Range.DC1000V:
                                Send("CONFigure:VOLTage:DC 1000");
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case DMM_Mode.ACV:

                    if (RangeSelected < Enum.GetNames(typeof(DMM_ACV_Range)).Length)
                    {
                        if (ACV_Range == (DMM_ACV_Range)RangeSelected && mode == Mode) return;
                        ACV_Range = (DMM_ACV_Range)RangeSelected;
                        switch ((DMM_ACV_Range)RangeSelected)
                        {
                            case DMM_ACV_Range.AC100mV:
                                Send("CONFigure:VOLTage:AC 0.1");
                                break;
                            case DMM_ACV_Range.AC1V:
                                Send("CONFigure:VOLTage:AC 1");
                                break;
                            case DMM_ACV_Range.AC10V:
                                Send("CONFigure:VOLTage:AC 10");
                                break;
                            case DMM_ACV_Range.AC100V:
                                Send("CONFigure:VOLTage:AC 100");
                                break;
                            case DMM_ACV_Range.AC750V:
                                Send("CONFigure:VOLTage:AC 750");
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case DMM_Mode.FREQ:

                    if (RangeSelected < Enum.GetNames(typeof(DMM_ACV_Range)).Length)
                    {
                        if (ACV_Range == (DMM_ACV_Range)RangeSelected && mode == Mode) return;
                        ACV_Range = (DMM_ACV_Range)RangeSelected;
                        switch ((DMM_ACV_Range)RangeSelected)
                        {
                            case DMM_ACV_Range.AC100mV:
                                Send("CONFigure:FREQuency 0.1");
                                break;
                            case DMM_ACV_Range.AC1V:
                                Send("CONFigure:FREQuency 1");
                                break;
                            case DMM_ACV_Range.AC10V:
                                Send("CONFigure:FREQuency 10");
                                break;
                            case DMM_ACV_Range.AC100V:
                                Send("CONFigure:FREQuency 100");
                                break;
                            case DMM_ACV_Range.AC750V:
                                Send("CONFigure:FREQuency 750");
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case DMM_Mode.RES:

                    if (RangeSelected < Enum.GetNames(typeof(DMM_RES_Range)).Length)
                    {
                        if (RES_Range == (DMM_RES_Range)RangeSelected && mode == Mode) return;
                        RES_Range = (DMM_RES_Range)RangeSelected;
                        switch ((DMM_RES_Range)RangeSelected)
                        {
                            case DMM_RES_Range.R100Ω:
                                Send("CONFigure:RESistance 100");
                                break;
                            case DMM_RES_Range.R1kΩ:
                                Send("CONFigure:RESistance 1000");
                                break;
                            case DMM_RES_Range.R10kΩ:
                                Send("CONFigure:RESistance 10000");
                                break;
                            case DMM_RES_Range.R100kΩ:
                                Send("CONFigure:RESistance 100000");
                                break;
                            case DMM_RES_Range.R1MΩ:
                                Send("CONFigure:RESistance 1000000");
                                break;
                            case DMM_RES_Range.R10MΩ:
                                Send("CONFigure:RESistance 10000000");
                                break;
                            case DMM_RES_Range.R100MΩ:
                                Send("CONFigure:RESistance 100000000");
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case DMM_Mode.DIODE:
                    Send("CONFigure:DIODe");
                    Task.Delay(500).Wait();
                    break;
                default:
                    break;
            }


        }

        public void ChangeRange(string Range, DMM_Mode mode)
        {
            switch (mode)
            {
                case DMM_Mode.NONE:
                    break;
                case DMM_Mode.DCV:
                    int indexDCV = Enum.GetNames(typeof(DMM_DCV_Range)).ToList().IndexOf(Range);
                    if (DCV_Range == (DMM_DCV_Range)indexDCV && mode == Mode) return;
                    DCV_Range = (DMM_DCV_Range)indexDCV;
                    switch ((DMM_DCV_Range)indexDCV)
                    {
                        case DMM_DCV_Range.DC100mV:
                            Send("CONFigure:VOLTage:DC 0.1");
                            break;
                        case DMM_DCV_Range.DC1V:
                            Send("CONFigure:VOLTage:DC 1");
                            break;
                        case DMM_DCV_Range.DC10V:
                            Send("CONFigure:VOLTage:DC 10");
                            break;
                        case DMM_DCV_Range.DC100V:
                            Send("CONFigure:VOLTage:DC 100");
                            break;
                        case DMM_DCV_Range.DC1000V:
                            Send("CONFigure:VOLTage:DC 1000");
                            break;
                        default:
                            break;
                    }
                    break;
                case DMM_Mode.ACV:
                    int indexACV = Enum.GetNames(typeof(DMM_ACV_Range)).ToList().IndexOf(Range);
                    if (ACV_Range == (DMM_ACV_Range)indexACV && mode == Mode) return;
                    ACV_Range = (DMM_ACV_Range)indexACV;
                    switch ((DMM_ACV_Range)indexACV)
                    {
                        case DMM_ACV_Range.AC100mV:
                            Send("CONFigure:VOLTage:AC 0.1");
                            break;
                        case DMM_ACV_Range.AC1V:
                            Send("CONFigure:VOLTage:AC 1");
                            break;
                        case DMM_ACV_Range.AC10V:
                            Send("CONFigure:VOLTage:AC 10");
                            break;
                        case DMM_ACV_Range.AC100V:
                            Send("CONFigure:VOLTage:AC 100");
                            break;
                        case DMM_ACV_Range.AC750V:
                            Send("CONFigure:VOLTage:AC 750");
                            break;
                        default:
                            break;
                    }
                    break;
                case DMM_Mode.FREQ:
                    int indexFREQ = Enum.GetNames(typeof(DMM_ACV_Range)).ToList().IndexOf(Range);
                    if (ACV_Range == (DMM_ACV_Range)indexFREQ && mode == Mode) return;
                    ACV_Range = (DMM_ACV_Range)indexFREQ;
                    switch ((DMM_ACV_Range)indexFREQ)
                    {
                        case DMM_ACV_Range.AC100mV:
                            Send("CONFigure:FREQuency 0.1");
                            break;
                        case DMM_ACV_Range.AC1V:
                            Send("CONFigure:FREQuency 1");
                            break;
                        case DMM_ACV_Range.AC10V:
                            Send("CONFigure:FREQuency 10");
                            break;
                        case DMM_ACV_Range.AC100V:
                            Send("CONFigure:FREQuency 100");
                            break;
                        case DMM_ACV_Range.AC750V:
                            Send("CONFigure:FREQuency 750");
                            break;
                        default:
                            break;
                    }
                    break;
                case DMM_Mode.RES:

                    int indexRES = Enum.GetNames(typeof(DMM_RES_Range)).ToList().IndexOf(Range);
                    if (RES_Range == (DMM_RES_Range)indexRES && mode == Mode) return;
                    RES_Range = (DMM_RES_Range)indexRES;
                    switch ((DMM_RES_Range)indexRES)
                    {
                        case DMM_RES_Range.R100Ω:
                            Send("CONFigure:RESistance 100");
                            break;
                        case DMM_RES_Range.R1kΩ:
                            Send("CONFigure:RESistance 1000");
                            break;
                        case DMM_RES_Range.R10kΩ:
                            Send("CONFigure:RESistance 10000");
                            break;
                        case DMM_RES_Range.R100kΩ:
                            Send("CONFigure:RESistance 100000");
                            break;
                        case DMM_RES_Range.R1MΩ:
                            Send("CONFigure:RESistance 1000000");
                            break;
                        case DMM_RES_Range.R10MΩ:
                            Send("CONFigure:RESistance 10000000");
                            break;
                        case DMM_RES_Range.R100MΩ:
                            Send("CONFigure:RESistance 100000000");
                            break;
                        default:
                            break;
                    }
                    break;
                case DMM_Mode.DIODE:
                    break;
                default:
                    break;
            }
        }

        public void ChangeRate(DMM_Rate _Rate)
        {
            switch (Mode)
            {
                case DMM_Mode.NONE:
                    break;
                case DMM_Mode.DCV:
                    if (_Rate != DCrate)
                    {
                        DCrate = _Rate;
                        switch (_Rate)
                        {
                            case DMM_Rate.NONE:
                                break;
                            case DMM_Rate.SLOW:
                                Send("SENSe:DETector:RATE S");
                                break;
                            case DMM_Rate.MID:
                                Send("SENSe:DETector:RATE M");
                                break;
                            case DMM_Rate.FAST:
                                Send("SENSe:DETector:RATE F");
                                break;
                            default:
                                break;
                        }
                        Task.Delay(500).Wait();
                    }
                    break;
                case DMM_Mode.ACV:
                    if (_Rate != ACrate)
                    {
                        ACrate = _Rate;
                        switch (_Rate)
                        {
                            case DMM_Rate.NONE:
                                break;
                            case DMM_Rate.SLOW:
                                Send("SENSe:DETector:RATE S");
                                break;
                            case DMM_Rate.MID:
                                Send("SENSe:DETector:RATE M");
                                break;
                            case DMM_Rate.FAST:
                                Send("SENSe:DETector:RATE F");
                                break;
                            default:
                                break;
                        }
                        Task.Delay(500).Wait();

                    }
                    break;
                case DMM_Mode.FREQ:
                    if (_Rate != FREQrate)
                    {
                        FREQrate = _Rate;
                        switch (_Rate)
                        {
                            case DMM_Rate.NONE:
                                break;
                            case DMM_Rate.SLOW:
                                Send("SENSe:DETector:RATE S");
                                break;
                            case DMM_Rate.MID:
                                Send("SENSe:DETector:RATE M");
                                break;
                            case DMM_Rate.FAST:
                                Send("SENSe:DETector:RATE F");
                                break;
                            default:
                                break;
                        }
                        Task.Delay(500).Wait();

                    }
                    break;
                case DMM_Mode.RES:
                    if (_Rate != RESrate)
                    {
                        RESrate = _Rate;
                        switch (_Rate)
                        {
                            case DMM_Rate.NONE:
                                break;
                            case DMM_Rate.SLOW:
                                Send("SENSe:DETector:RATE S");
                                break;
                            case DMM_Rate.MID:
                                Send("SENSe:DETector:RATE M");
                                break;
                            case DMM_Rate.FAST:
                                Send("SENSe:DETector:RATE F");
                                break;
                            default:
                                break;
                        }
                        Task.Delay(500).Wait();

                    }
                    break;
                case DMM_Mode.DIODE:
                    if (_Rate != DIODErate)
                    {
                        DIODErate = _Rate;
                        switch (_Rate)
                        {
                            case DMM_Rate.NONE:
                                break;
                            case DMM_Rate.SLOW:
                                Send("SENSe:DETector:RATE S");
                                break;
                            case DMM_Rate.MID:
                                Send("SENSe:DETector:RATE M");
                                break;
                            case DMM_Rate.FAST:
                                Send("SENSe:DETector:RATE F");
                                break;
                            default:
                                break;
                        }
                        Task.Delay(500).Wait();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
