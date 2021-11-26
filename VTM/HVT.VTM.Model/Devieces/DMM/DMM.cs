using HVT.Utility;
using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace HVT.VTM.Base
{
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

    public class DMM
    {
        public string SN = "GET857611";

        // Command
        const string CALL = "*IDN?";

        // string value

        public string LastStringValue = "";
        // numberic value
        public double LastDoubleValue = 0;
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
        public DMM_Mode Mode = DMM_Mode.DCV;
        public DMM_Rate Rate = DMM_Rate.SLOW;
        // Event
        public event EventHandler OnSend;
        public event EventHandler OnRecive;
        public event EventHandler OnConnected;
        public event EventHandler OnChange;

        // Update parameter
        public int Time = 100;
        public Timer UpdateValueTimer = new Timer() { AutoReset = true, Interval = 500 };
        // comunication
        public SerialDisplay serialDisplay = new SerialDisplay();
        public SerialPort serialPort = new SerialPort()
        {
            BaudRate = 115200,
            Parity = Parity.None,
            DataBits = 8,
            StopBits = StopBits.One,
        };

        // Task list
        public Task UpdateValueTask;
        private string comData;

        public bool IsAutoUpdate { get; set; } = false;
        public bool IsMatchCalculate { get; set; } = false;
        public bool IsCancelUpdateTask { get; set; } = false;

        // Function
        public DMM()
        {
            serialPort.DataReceived += SerialPort_DataReceived;
            UpdateValueTimer.Elapsed += UpdateValueTimer_Elapsed;
        }

        private void UpdateValueTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateValueTimer.Stop();
            GetValue();
            UpdateValueTimer.Start();
        }

        public async Task<bool> SearchCom()
        {
            OnConnected?.Invoke(false, null);
            int ScanTimeOut = 500;
            bool IsBoardPort = false;
            foreach (var item in SerialPort.GetPortNames())
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }

                serialPort.PortName = item;
                try
                {
                    serialPort.Open();
                    await Task.Delay(100).ConfigureAwait(true);
                }
                catch (UnauthorizedAccessException err)
                {
                    Console.WriteLine(err.Message);
                }
                var startScanTime = DateTime.Now;
                comData = "";
                Send(CALL);
                if (serialPort.IsOpen)
                {
                    while (true)
                    {
                        if (comData != null)
                        {
                            if (comData.Contains(SN))
                            {
                                IsBoardPort = true;
                                break;
                            }
                            else
                            {
                                comData = "";
                                IsBoardPort = false;
                            }
                        }

                        if (DateTime.Now.Subtract(startScanTime).TotalMilliseconds > ScanTimeOut)
                        {
                            IsBoardPort = false;
                            break;
                        }
                        else
                        {
                            Send(CALL);
                            await Task.Delay(50);
                        }
                    }
                }
                if (IsBoardPort)
                {
                    OnConnected?.Invoke(serialPort.IsOpen, null);
                    Send("CONFigure:VOLTage:DC 100");
                    Send("SENSe:DETector:RATE S");
                    break;
                }
                else
                {
                    serialPort.Close();
                    OnConnected?.Invoke(serialPort.IsOpen, null);
                }
            }

            return IsBoardPort;
        }

        public void Send(string Content)
        {
            if (serialPort.IsOpen)
            {
                OnSend?.Invoke(true, null);
                serialPort.Write(Content + "\r\n");
                Thread.Sleep(10);
                OnSend?.Invoke(true, null);
            }
        }
        public async Task<string> QueryAsync(string Content, int ScanTimeOut)
        {
            string value = "";
            DateTime startScanTime = DateTime.Now;
            if (serialPort.IsOpen)
            {
                serialPort.DiscardInBuffer();
                comData = "";
                Send(Content);
                while (true)
                {
                    if (comData != null)
                    {
                        value = comData;
                    }

                    if (DateTime.Now.Subtract(startScanTime).TotalMilliseconds > ScanTimeOut)
                    {
                        break;
                    }
                    else
                    {
                        Send(Content);
                        await Task.Delay(50);
                    }
                }
            }
            return value;
        }
        public async Task<string> QueryOneTime(string Content)
        {
            string value = "";
            DateTime startScanTime = DateTime.Now;
            if (serialPort.IsOpen)
            {
                serialPort.DiscardInBuffer();
                comData = null;
                Send(Content);
                while (DateTime.Now.Subtract(startScanTime).TotalMilliseconds < 1000)
                {
                    if (comData != null)
                    {
                        value = comData;
                        break;
                    }
                    await Task.Delay(50);
                }
            }
            return value;
        }
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            OnRecive?.Invoke(null, null);

            try
            {
                while (serialPort.BytesToRead > 0)
                {
                    comData = serialPort.ReadLine();
                    Console.WriteLine(comData);
                    serialPort.DiscardInBuffer();
                    break;
                }
            }
            catch (Exception err)
            {
                Extensions.LogErr(err.Message);
            }
        }

        public void UpdateValue(bool isStart, int Preiod)
        {
            UpdateValueTimer.Interval = Preiod;
            UpdateValueTimer.Enabled = isStart;
            if (isStart)
            {
                UpdateValueTimer.Start();
            }
            else
            {
                UpdateValueTimer.Stop();
            }
        }

        public async void GetValue()
        {

            double currentValue = LastDoubleValue;
            if (double.TryParse(await QueryOneTime("VAL1?"), out LastDoubleValue))
            {
                currentValue = LastDoubleValue;
            }

            if (System.Math.Abs(currentValue) < 0.001)
            {
                LastStringValue = (currentValue * 1000000) + " u";
            }
            else if (System.Math.Abs(currentValue) < 1)
            {
                LastStringValue = (currentValue * 1000) + " m";
            }
            else
            {
                LastStringValue = (System.Math.Abs(currentValue) < 1000) ? (currentValue / 1000) + " k" : (currentValue / 1000000) + " M";
            }
            if (currentValue == +1.200000E+37)
            {
                LastStringValue = "OL ";
            }
            switch (Mode)
            {
                case DMM_Mode.NONE:
                    break;
                case DMM_Mode.DCV:
                    LastStringValue += "VDC";
                    break;
                case DMM_Mode.ACV:
                    LastStringValue += "VAC";
                    break;
                case DMM_Mode.FREQ:
                    LastStringValue += "Hz";
                    break;
                case DMM_Mode.RES:
                    LastStringValue += "OHm";
                    break;
                case DMM_Mode.DIODE:
                    LastStringValue += "VDC";
                    break;
                default:
                    break;
            }

            OnChange?.Invoke(LastStringValue, null);

        }

        public async Task<bool> SetMode(DMM_Mode mode)
        {
            Mode = mode;
            switch (mode)
            {
                case DMM_Mode.NONE:
                    return false;
                case DMM_Mode.DCV:
                    Send("CONFigure:VOLTage:DC 100");
                    return await QueryOneTime("CONFigure:FUNCtion?") == "VOLT";
                case DMM_Mode.ACV:
                    Send("CONFigure:VOLTage:AC 750");
                    return await QueryOneTime("CONFigure:FUNCtion?") == "VOLT:AC";
                case DMM_Mode.FREQ:
                    Send("CONFigure:FREQuency 750");
                    return await QueryOneTime("CONFigure:FUNCtion?") == "FREQ";
                case DMM_Mode.RES:
                    Send("CONFigure:RESistance 10000");
                    return await QueryOneTime("CONFigure:FUNCtion?") == "RES";
                case DMM_Mode.DIODE:
                    Send("CONFigure:DIODe");
                    return await QueryOneTime("CONFigure:FUNCtion?") == "DIOD";
                default:
                    return false;
            }
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
                    break;
                default:
                    break;
            }

        }


        public void ChangeRate(DMM_Rate _Rate)
        {
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
        }
    }
}
