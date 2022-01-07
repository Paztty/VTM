using HVT.Utility;
using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace HVT.VTM.Base
{
    public class PPS
    {

        // comunication defines
        const string CALL = "*IDN?";
        const string ISET = "ISET<X>:<NR2>";
        const string ISET_ASK = "ISET<X>?";
        const string VSET = "VSET<X>:<NR2>";
        const string VSET_ASK = "VSET<X>?";
        const string IOUT = "IOUT<X>?";
        const string VOUT = "VOUT<X>?";
        const string STATUS = "STATUS?";
        const string ERR = "ERR?";
        const string OUT = "OUT<NR2>";
        // enum set, get param
        public enum SetFlag
        {
            SetVolt,
            SetAmpe,
            SetOutput
        };
        public enum GetFlag
        {
            GetVolt,
            GetAmpe,
            GetOutput,
            ErrCheck,
        };


        public int Channel { get; set; } = 1;
        public string ID { get; set; } = "EP999999";
        public SerialPort Port = new SerialPort();

        public bool IsOutputOn = false;

        public PPSsite Site1 { get; set; } = new PPSsite() { SiteNumber = 1 };
        public PPSsite Site2 { get; set; } = new PPSsite() { SiteNumber = 2 };

        //Event
        public event EventHandler OnSend;
        public event EventHandler OnRecive;
        public event EventHandler OnConnected;
        public event EventHandler OnChange;

        public SerialDisplay serialDisplay = new SerialDisplay();
        public SerialPort serialPort = new SerialPort()
        {
            BaudRate = 9600,
            Parity = Parity.None,
            DataBits = 8,
            StopBits = StopBits.One,
        };
        private string comData;

        public bool canUpdate = true;
        public Task UpdateValueTask;
        public PPS()
        {
            UpdateValueTask = new Task(UpdateValue);
            serialPort.DataReceived += SerialPort_DataReceived;
        }

        public async Task<bool> SearchCom()
        {
            canUpdate = false;
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
                catch (Exception)
                {
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
                            if (comData.Contains(ID))
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
                    break;
                }
                else
                {
                    serialPort.Close();
                    OnConnected?.Invoke(serialPort.IsOpen, null);
                }
            }
            canUpdate = true;
            if (UpdateValueTask.Status != TaskStatus.Running)
            {
                UpdateValueTask = new Task(UpdateValue);
                UpdateValueTask.Start();
            }

            return IsBoardPort;
        }

        public void Send(string Content)
        {
            if (serialPort.IsOpen)
            {
                OnSend?.Invoke(true, null);
                serialPort.Write(Content + "\r\n");
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
                        Console.WriteLine("PPS : " + comData);
                        value = comData;
                    }

                    if (DateTime.Now.Subtract(startScanTime).TotalMilliseconds > ScanTimeOut)
                    {
                        break;
                    }
                    else
                    {
                        Send(Content);
                        await Task.Delay(100);
                    }
                }
            }
            return value;
        }
        public string QueryOneTime(string Content)
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

                    if (DateTime.Now.Subtract(startScanTime).TotalMilliseconds > 100)
                    {
                        break;
                    }
                }
            }
            return value;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            OnRecive?.Invoke(null, null);
            if (serialPort.IsOpen)
            {
                try
                {
                    while (serialPort.BytesToRead > 0)
                    {
                        if (serialPort.IsOpen) comData = serialPort.ReadLine();
                        serialPort.DiscardInBuffer();
                        OnConnected?.Invoke(true, null);
                        break;
                    }
                }
                catch (Exception err)
                {
                    Extensions.LogErr("PPS serial reciver: " + err.Message);
                    OnConnected?.Invoke(false, null);
                }
            }
            OnRecive?.Invoke(null, null);
        }

        public async Task<bool> SetParam(SetFlag setFlag, int Site, double value)
        {
            bool result = false;
            switch (setFlag)
            {
                case SetFlag.SetVolt:
                    Send(VSET.Replace("<X>", Site.ToString()).Replace("<NR2>", value.ToString()));
                    return value.ToString() == await QueryAsync(VSET_ASK.Replace("<X>", Site.ToString()), 100);
                case SetFlag.SetAmpe:
                    Send(ISET.Replace("<X>", Site.ToString()).Replace("<NR2>", value.ToString()));
                    return value.ToString() == await QueryAsync(ISET_ASK.Replace("<X>", Site.ToString()), 100);
                case SetFlag.SetOutput:
                    Send(OUT.Replace("<X>", value.ToString()).Replace("<NR2>", value.ToString()));
                    result = true;
                    break;
                default:
                    break;
            }
            return result;
        }

        public void UpdateValue()
        {
            double lastedValue = 0;
            while (canUpdate)
            {
                var lastStartUpdate = DateTime.Now;
                double Value = 0;
                if (double.TryParse(QueryOneTime(VOUT.Replace("<X>", "1")).Replace("V", ""), out Value))
                    Site1.ActualVolt = Value;
                if (double.TryParse(QueryOneTime(IOUT.Replace("<X>", "1")).Replace("A", ""), out Value))
                    Site1.ActualAmpe = Value;

                if (double.TryParse(QueryOneTime(VOUT.Replace("<X>", "2")).Replace("V", ""), out Value))
                    Site2.ActualVolt = Value;
                if (double.TryParse(QueryOneTime(IOUT.Replace("<X>", "2")).Replace("A", ""), out Value))
                    Site2.ActualAmpe = Value;

                if (Site1.ActualVolt + Site1.ActualAmpe + Site2.ActualVolt + Site2.ActualAmpe != lastedValue)
                {
                    OnChange?.Invoke(null, null);
                    lastedValue = Site1.ActualVolt + Site1.ActualAmpe + Site2.ActualVolt + Site2.ActualAmpe;
                }

                while (DateTime.Now.Subtract(lastStartUpdate).TotalSeconds < 1)
                {
                    if (!canUpdate)
                    {
                        break;
                    }
                }
                if (!canUpdate)
                {
                    break;
                }
            }
        }
    }
}
