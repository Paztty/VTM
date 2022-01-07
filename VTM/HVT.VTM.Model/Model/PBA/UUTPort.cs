using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using HVT.Utility;


namespace HVT.VTM.Base
{

    public class UUT_Config
    {
        public bool Use { get; set; } = true;

        public enum PortKind
        {
            TTL,
            RS485
        }

        private PortKind kind;
        public PortKind Kind { get { return kind; } set { kind = value; } }

        private int baudrate = 9600;
        public int Baudrate
        { get { return baudrate; } set { baudrate = value; } }

        private SerialData serialData;
        public SerialData SerialData
        { get { return serialData; } set { serialData = value; } }

        private Parity parity = Parity.Even;
        public Parity Parity
        { get { return parity; } set { parity = value; } }

        private StopBits stopBits = StopBits.One;
        public StopBits StopBits
        { get { return stopBits; } set { stopBits = value; } }

        private int dataBit = 8;
        public int DataBit
        { get { return dataBit; } set { dataBit = value; } }


        private bool usePrefix1 = true;
        public bool UsePrefix1 { get { return usePrefix1; } set { usePrefix1 = value; } }

        private int prefix1 = 0x5A;
        public int Prefix1
        { get { return prefix1; } set { prefix1 = value; } }

        private bool usePrefix2 = true;
        public bool UsePrefix2 { get { return usePrefix2; } set { usePrefix2 = value; } }

        private int prefix2 = 0xA5;
        public int Prefix2
        { get { return prefix2; } set { prefix2 = value; } }

        private bool useSuffix = false;
        public bool UseSuffix { get { return useSuffix; } set { useSuffix = value; } }

        private int suffix;
        public int Suffix
        { get { return suffix; } set { suffix = value; } }

        private bool useLengFixed = true;
        public bool UseLengFixed { get { return useLengFixed; } set { useLengFixed = value; } }

        public int lenghtFixed;
        public int LenghtFixed
        { get { return lenghtFixed; } set { lenghtFixed = value; } }

        private bool useRxPrefix1 = true;
        public bool UseRxPrefix1 { get { return useRxPrefix1; } set { useRxPrefix1 = value; } }

        private int Rxprefix1 = 0x5A;
        public int RxPrefix1
        { get { return Rxprefix1; } set { Rxprefix1 = value; } }

        private bool useRxPrefix2 = true;
        public bool UseRxPrefix2 { get { return useRxPrefix2; } set { useRxPrefix2 = value; } }

        private int Rxprefix2 = 0xA5;
        public int RxPrefix2
        { get { return Rxprefix2; } set { Rxprefix2 = value; } }

        private bool useRxSuffix = false;
        public bool UseRxSuffix { get { return useRxSuffix; } set { useRxSuffix = value; } }

        private int Rxsuffix;
        public int RxSuffix
        { get { return Rxsuffix; } set { Rxsuffix = value; } }

        private bool useRxLengFixed = true;
        public bool UseRxLengFixed { get { return useRxLengFixed; } set { useRxLengFixed = value; } }

        public int RxlenghtFixed;
        public int RxLenghtFixed
        { get { return RxlenghtFixed; } set { RxlenghtFixed = value; } }

        //public enum CheckSumType
        //{
        //    XOR,
        //    CRC_16,
        //    _CRC_16_CCITT,
        //    MODBUS_CRC_16,
        //    CRC_8,
        //    CRC_32,
        //    CRC_8_REVERSED,
        //    SUM
        //}

        public CheckSumType Checksum { get; set; } = CheckSumType.XOR;

        private int startChecksumCal = 0;
        public int StartChecksumCal
        { get { return startChecksumCal; } set { startChecksumCal = value; } }

        private int endChecksumCal = 0;
        public int EndChecksumCal
        { get { return endChecksumCal; } set { endChecksumCal = value; } }

        private int clearRxTime = 100;
        public int ClearRxTime { get { return clearRxTime; } set { clearRxTime = value; } }

        public bool ClearRxTimeSpecified { get; set; } = false;

        public byte[] GetFrame(TxData Txdata)
        {
            List<byte> dataToSend = new List<byte>();
            var dataStr = Txdata.Data.Replace(" ", "");
            var data = StringToByteArray.Convert(dataStr);
            if (data == null) return null;

            dataToSend.AddRange(data);

            if (usePrefix2)
            {
                dataToSend.Insert(0, (byte)prefix2);
            }

            if (usePrefix1)
            {
                dataToSend.Insert(0, (byte)prefix1);
            }

            if (useLengFixed)
            {
                for (int i = 0; i < LenghtFixed; i++)
                {
                    if (i == dataToSend.Count)
                    {
                        dataToSend.Add(Convert.ToByte(Txdata.Blank));
                    }
                }
            }

            var checksum = CheckSum.Get(dataToSend.ToArray(), CheckSumType.XOR);
            dataToSend.Add(checksum);

            if (useSuffix)
            {
                dataToSend[dataToSend.Count - 1] = (byte)Suffix;
            }

            //dataToSend.Insert(2, (Byte)(dataToSend.Count - 3) );
            foreach (var item in dataToSend)
            {
                Console.Write(item.ToString("X2") + " ");
            }
            return dataToSend.ToArray();
        }
        public byte[] GetFrame(string Txdata)
        {
            List<byte> dataToSend = new List<byte>();
            var dataStr = Txdata.Replace(" ", "");
            var data = StringToByteArray.Convert(dataStr);
            if (data == null) return null;

            dataToSend.AddRange(data);

            if (usePrefix2)
            {
                dataToSend.Insert(0, (byte)prefix2);
            }

            if (usePrefix1)
            {
                dataToSend.Insert(0, (byte)prefix1);
            }

            var checksum = CheckSum.Get(dataToSend.ToArray(), CheckSumType.XOR);
            dataToSend.Add(checksum);

            if (useSuffix)
            {
                dataToSend[dataToSend.Count - 1] = (byte)Suffix;
            }

            //dataToSend.Insert(2, (Byte)(dataToSend.Count - 3) );
            foreach (var item in dataToSend)
            {
                Console.Write(item.ToString("X2") + " ");
            }
            return dataToSend.ToArray();
        }

    }


    public class UUTPort : IProgress
    {
        public SerialDisplay SerialDisplay = new SerialDisplay();
        private SerialPort serial = new SerialPort();

        private UUT_Config config = new UUT_Config();
        public UUT_Config Config
        {
            get { return config; }
            set
            {
                if (config != value)
                {
                    if (SerialPort.GetPortNames().Contains(serialname))
                    {
                        config = value;
                        serial.BaudRate = value.Baudrate;
                        serial.Parity = value.Parity;
                        serial.StopBits = value.StopBits;
                        serial.DataBits = value.DataBit;
                        ClearTime = value.ClearRxTime;
                        try
                        {
                            if (!serial.IsOpen)
                                serial.Open();
                        }
                        catch (Exception e)
                        {
                            Debug.Write(e.Message, Debug.ContentType.Error);
                            Console.WriteLine(e.StackTrace);
                        }
                    }
                }
            }
        }

        Timer clearRxTimer = new Timer();
        private int clearTime;
        private int ClearTime
        {
            get { return clearTime; }
            set
            {
                if (value == 0)
                {
                    clearRxTimer.Enabled = false;
                    clearTime = 0;
                }
                else
                {
                    clearRxTimer.Interval = value;
                    clearTime = value;
                    clearRxTimer.Enabled = true;
                    clearRxTimer.Start();
                }
            }

        }

        private string serialname;
        public string SerialName
        {
            get { return serialname; }
            set
            {
                if (serialname != value)
                {
                    serialname = value;
                }
            }
        }

        private List<int> buffer = new List<int>();
        public List<int> Buffer
        {
            get { return buffer; }
            set { buffer = value; }
        }
        public int[] Data;

        public UUTPort()
        {
            serial.DataReceived += Serial_DataReceived;
            clearRxTimer.Elapsed += ClearRxTimer_Elapsed;
        }

        public void SetPort(Rectangle portTx, Rectangle portRx, Rectangle portStatus)
        {
            this.SerialDisplay.TX = portTx;
            SerialDisplay.RX = portRx;
            SerialDisplay.IsOpenRect = portStatus;
            SerialDisplay.ShowCOMStatus(serial.IsOpen);
            RefindCom();
        }

        public bool OpenPort()
        {
            if (!serial.IsOpen)
            {
                try
                {
                    serial.Open();
                    return serial.IsOpen;
                }
                catch (Exception err)
                {
                    return false;
                }
            }
            return serial.IsOpen;
        }

        public bool ClosePort()
        {
            if (!serial.IsOpen)
            {
                try
                {
                    serial.Close();
                    return serial.IsOpen;
                }
                catch (Exception err)
                {
                    return false;
                }
            }
            return serial.IsOpen;
        }

        private void ClearRxTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!config.ClearRxTimeSpecified) config.ClearRxTimeSpecified = true;
            clearRxTimer.Stop();
        }

        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serial.IsOpen)
            {
                SerialDisplay.BlinkRX();
                if (config.ClearRxTimeSpecified)
                {
                    //Buffer.Clear();
                    config.ClearRxTimeSpecified = false;
                    clearRxTimer.Start();
                }
                int length = serial.BytesToRead;
                string dataRead = "";
                for (int i = 0; i < length; i++)
                {
                    if (serial.IsOpen)
                    {
                        Buffer.Add(serial.ReadByte());
                        dataRead += Buffer.Count > 0 ? Buffer[Buffer.Count - 1].ToString("X2") + " " : "";
                        //Console.Write(Buffer[i].ToString("X2") + " ");
                    }
                }
                Write_Log(dataRead, false);
            }
        }

        private bool RefindCom()
        {
            try
            {
                if (SerialPort.GetPortNames().Contains(serialname))
                {
                    Console.WriteLine(serialname);
                    if (serial.IsOpen)
                    {
                        serial.Close();
                        SerialDisplay.ShowCOMStatus(false);
                    }
                    serial.PortName = SerialName;

                    serial.Open();
                    SerialDisplay.ShowCOMStatus(true);

                    return true;
                }
                else
                    return false;
            }
            catch (Exception err)
            {
                SerialDisplay.ShowCOMStatus(false);
                HVT.Utility.Debug.Write(err.Message, Debug.ContentType.Error);
                return false;
            }
        }

        public bool Send(byte[] data)
        {
            Buffer.Clear();
            string datalog = "";

            foreach (var item in data)
            {
                datalog += item.ToString("X2") + " ";
            }
            Write_Log(datalog);

            if (serial.IsOpen)
            {
                SerialDisplay.BlinkTX();
                try
                {
                    serial.Write(data, 0, data.Length);
                    return true;
                }
                catch (System.IO.IOException)
                {
                    return false;
                }
                catch (System.TimeoutException)
                {
                    return false;
                }
            }
            //else if (RefindCom())
            //{
            //    serial.DiscardInBuffer();
            //    serial.Write(data, 0, data.Length);
            //    return true;
            //}
            else
            {
                return false;
            }
        }

        public bool Send(string dataStr)
        {
            Buffer.Clear();
            var data = config.GetFrame(dataStr);
            string datalog = "";
            foreach (var item in data)
            {
                datalog += item.ToString("X2") + " ";
            }
            Write_Log(datalog);
            if (serial.IsOpen)
            {
                SerialDisplay.BlinkTX();
                try
                {
                    serial.Write(data, 0, data.Length);
                    return true;
                }
                catch (System.IO.IOException)
                {
                    return false;
                }
                catch (System.TimeoutException)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        public bool Send(TxData txData)
        {
            Buffer.Clear();
            var data = config.GetFrame(txData);
            string datalog = "";
            foreach (var item in data)
            {
                datalog += item.ToString("X2") + " ";
            }
            Write_Log(datalog);

            if (serial.IsOpen)
            {
                SerialDisplay.BlinkTX();
                try
                {
                    serial.Write(data, 0, data.Length);
                    return true;
                }
                catch (System.IO.IOException)
                {
                    return false;
                }
                catch (System.TimeoutException)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public int CheckBuffer(RxData rxData)
        {
            if (rxData.dataKind == RxDataKind.Range)
            {
                int mbyte = 0;
                int lbyte = 0;
                int mbit = 0;
                int lbit = 0;
                if (Int32.TryParse(rxData.MByte, out mbyte))
                {
                    if (Int32.TryParse(rxData.LByte, out lbyte))
                    {
                        if (Int32.TryParse(rxData.M_Mbit, out mbit))
                        {
                            if (Int32.TryParse(rxData.L_Lbit, out lbit))
                            {
                                int byteToCheck = 0;
                                for (int i = mbyte; i <= lbyte; i++)
                                {
                                    if (Buffer.Count > i)
                                    {
                                        Console.Write(Buffer[i].ToString("x"));
                                    }
                                    //   byteToCheck = byteToCheck << 8 ^ Buffer[i];
                                }
                                Console.WriteLine();
                            }
                        }
                    }
                }
            }
            return 0;
        }
        public string CheckBufferString(RxData rxData)
        {
            Console.WriteLine();
            // foreach (int i in Buffer) Console.Write(i.ToString("X2") + " ");
            Console.WriteLine();
            string dataReturn = "~";
            if (rxData.dataKind == RxDataKind.Range)
            {
                int mbyte = 0;
                int lbyte = 0;
                int mbit = 0;
                int lbit = 0;
                if (Int32.TryParse(rxData.MByte, out mbyte))
                {
                    if (Int32.TryParse(rxData.LByte, out lbyte))
                    {
                        if (Int32.TryParse(rxData.M_Mbit, out mbit))
                        {
                            if (Int32.TryParse(rxData.L_Lbit, out lbit))
                            {
                                int byteToCheck = 0;
                                dataReturn = "";
                                for (int i = mbyte; i <= lbyte; i++)
                                {
                                    if (Buffer.Count > i && i > 0)
                                    {
                                        byteToCheck = byteToCheck << 8 ^ Buffer[i - 1];
                                        Console.Write(((char)Buffer[i - 1]).ToString());
                                        dataReturn += ((char)Buffer[i - 1]).ToString();
                                    }
                                }
                                Console.WriteLine();
                                switch (rxData.dataType)
                                {
                                    case RxDataType.DEC:
                                        return byteToCheck.ToString();
                                    case RxDataType.HEX:
                                        return byteToCheck.ToString("x2");
                                    case RxDataType.ASCII:
                                        return dataReturn;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            return "null";
        }

        public bool HaveBuffer()
        {
            return Buffer.Count > 0;
        }


        public RichTextBox LogBox = new RichTextBox();
        public Dispatcher dispatcher;
        public event EventHandler LOG_UPDATE;

        private void Write_Log(string log, bool IsTxWrite = true)
        {
            var paragraph = new Paragraph();
            paragraph.LineHeight = 10;

            if (IsTxWrite)
            {
                paragraph.Inlines.Add(new Run(DateTime.Now.ToString("HH:mm:ss -> Tx: ") + log));
                paragraph.Foreground = new SolidColorBrush(Colors.Blue);
            }

            else
            {
                paragraph.Inlines.Add(new Run(DateTime.Now.ToString("HH:mm:ss <- Rx: ") + log));
                paragraph.Foreground = new SolidColorBrush(Colors.Black);
            }

            LOG_UPDATE?.Invoke(paragraph, null);

            if (LogBox.Dispatcher.CheckAccess())
            {
                LogBox.Dispatcher.Invoke(new Action(delegate
                {
                    LogBox.Document.Blocks.Add(paragraph);
                    LogBox.ScrollToEnd();
                }), DispatcherPriority.Normal);
            }

        }
    }
}

