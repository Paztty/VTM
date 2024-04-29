using HVT.Utility;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using HVT.Controls;
using System.Windows;
using System.Threading.Tasks;

namespace HVT.VTM.Base
{
    public class UUTPort
    {
        public SerialPortDisplay serial = new SerialPortDisplay()
        {
            PortName = "COM1"
        };

        private HVT.Controls.UUT_Config config = new HVT.Controls.UUT_Config();
        public HVT.Controls.UUT_Config Config
        {
            get { return config; }
            set
            {
                if (config != value)
                {
                    config = value;
                    serial.Port.BaudRate = config.baudrate;
                    serial.Port.DataBits = config.dataBit;
                    serial.Port.Parity = config.parity;
                    serial.Port.StopBits = config.stopBits;
                }
            }
        }

        Timer clearRxTimer = new Timer();
        Timer ReSendTimer = new Timer();
        byte[] dataSendTimer;

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

        private List<int> buffer = new List<int>();
        public List<int> Buffer
        {
            get { return buffer; }
            set { buffer = value; }
        }
        public int[] Data;

        public UUTPort()
        {
            ReSendTimer.Elapsed += ReSendTimer_Elapsed;
            clearRxTimer.Elapsed += ClearRxTimer_Elapsed;
            serial.SerialDataReciver += Serial_SerialDataReciver;
            LogBox.Document.Blocks.Clear();
        }

        public void CheckPort(string COMNAME)
        {
            serial.Port = new System.IO.Ports.SerialPort()
            {
                PortName = COMNAME,
            };
            try
            {
                serial.Port.Open();
                serial.OpenPort();
                serial.SerialDataReciver -= Serial_SerialDataReciver;
                serial.SerialDataReciver += Serial_SerialDataReciver;
            }
            catch (Exception err)
            {
                HVT.Utility.Debug.Write(String.Format("{0} -> {1}: {2}", serial.DevieceName, serial.PortName, err.Message), Debug.ContentType.Error);
            }
        }

        private void ReSendTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (dataSendTimer != null)
            {
                Send(dataSendTimer);
            }
        }

        private void ClearRxTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!config.ClearRxTimeSpecified)
            {
                config.ClearRxTimeSpecified = true;
                clearRxTimer.Stop();
            }
        }

        private void Serial_SerialDataReciver(object sender, EventArgs e)
        {
            if (serial.Port.IsOpen)
            {
                if (config.ClearRxTimeSpecified)
                {
                    //Buffer.Clear();
                    config.ClearRxTimeSpecified = false;
                    clearRxTimer.Start();
                }
                if (Buffer.Count == 0)
                {
                    Write_Log(" ", false, false);
                }
                int length = serial.Port.BytesToRead;
                string dataRead = "";
                for (int i = 0; i < length; i++)
                {
                    if (serial.Port.IsOpen)
                    {
                        Buffer.Add(serial.Port.ReadByte());
                        dataRead += Buffer.Count > 0 ? Buffer[Buffer.Count - 1].ToString("X2") + " " : "";
                        Console.Write(Buffer[Buffer.Count - 1].ToString("X2") + " ");
                    }
                }
                Write_Log(dataRead, false, true);
            }
        }

        public bool SendTimer(string txData, int time)
        {
            if (time == 0)
            {
                dataSendTimer = null;
                ReSendTimer.Enabled = false;
                ReSendTimer.Stop();
                return true;
            }

            if (txData == null)
            {
                dataSendTimer = null;
                ReSendTimer.Enabled = false;
                ReSendTimer.Stop();
                return false;
            }

            if (!serial.Port.IsOpen)
            {
                return false;
            }

            dataSendTimer = config.GetFrame(txData);
            ReSendTimer.Interval = time;
            ReSendTimer.AutoReset = true;
            ReSendTimer.Enabled = true;
            ReSendTimer.Start();
            return true;
        }

        public bool SendTimer(TxData txData, int time)
        {
            if (time == 0)
            {
                dataSendTimer = null;
                ReSendTimer.Enabled = false;
                ReSendTimer.Stop();
                return true;
            }

            if (txData == null)
            {
                dataSendTimer = null;
                ReSendTimer.Enabled = false;
                ReSendTimer.Stop();
                return false;
            }

            if (!serial.Port.IsOpen)
            {
                return false;
            }

            dataSendTimer = config.GetFrame(txData.Data);
            ReSendTimer.Interval = time;
            ReSendTimer.AutoReset = true;
            ReSendTimer.Enabled = true;
            ReSendTimer.Start();
            return true;
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

            if (serial.Port.IsOpen)
            {
                try
                {
                    serial.SendBytes(data);
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

        public bool Send(string dataStr)
        {
            Buffer.Clear();
            var data = config.GetFrame(dataStr);
            if (data == null) return false;
            string datalog = "";
            foreach (var item in data)
            {
                datalog += item.ToString("X2") + " ";
            }
            Write_Log(datalog);
            if (serial.Port.IsOpen)
            {
                try
                {
                    serial.SendBytes(data);
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
            var data = config.GetFrame(txData.Data);
            string datalog = "";
            foreach (var item in data)
            {
                datalog += item.ToString("X2") + " ";
            }
            Write_Log(datalog);

            if (serial.Port.IsOpen)
            {
                try
                {
                    serial.Port.DiscardInBuffer();
                    serial.Port.DiscardOutBuffer();
                    serial.SendBytes(data);
                    return true;
                }
                catch (System.IO.IOException)
                {
                    return false;
                }
                catch (TimeoutException)
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
            foreach (int i in Buffer) Console.Write(i.ToString("X2") + " ");
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
                                        return Convert.ToInt32(byteToCheck).ToString();
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
            else if (rxData.dataKind == RxDataKind.Byte)
            {
                int mbyte = 0;
                int mbit = 0;
                int lbit = 0;
                if (Int32.TryParse(rxData.MByte, out mbyte))
                {
                    if (Int32.TryParse(rxData.M_Mbit, out mbit))
                    {
                        if (Int32.TryParse(rxData.M_Lbit, out lbit))
                        {
                            int byteToCheck = 0;
                            dataReturn = "";
                            if (Buffer.Count >= mbyte)
                            {
                                byteToCheck = byteToCheck << 8 ^ Buffer[mbyte];
                                Console.Write(((char)Buffer[mbyte]).ToString());
                                dataReturn += ((char)Buffer[mbyte]).ToString();
                            }
                            Console.WriteLine();
                            switch (rxData.dataType)
                            {
                                case RxDataType.DEC:
                                    return Convert.ToInt32(byteToCheck).ToString();
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
            else if (rxData.dataKind == RxDataKind.bit)
            {
                int mbyte = 0;
                int mbit = 0;
                if (Int32.TryParse(rxData.MByte, out mbyte))
                {
                    if (Int32.TryParse(rxData.M_Mbit, out mbit))
                    {
                        int byteToCheck = 0x01;
                        int bitValue = 0;
                        dataReturn = "";
                        if (Buffer.Count >= mbyte)
                        {
                            bitValue = ((byteToCheck << mbit) & Buffer[mbyte]) == 0 ? 0 : 1;
                            Console.Write(((char)Buffer[mbyte]).ToString());
                            dataReturn += ((char)Buffer[mbyte]).ToString();
                        }
                        Console.WriteLine();
                        switch (rxData.dataType)
                        {
                            case RxDataType.DEC:
                                return bitValue.ToString();
                            case RxDataType.HEX:
                                return bitValue.ToString("x2");
                            case RxDataType.ASCII:
                                return dataReturn;
                            default:
                                break;
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

        public RichTextBox LogBox = new RichTextBox()
        {
            Background = new SolidColorBrush(Colors.Black),
            Width = 776
        };

        public RichTextBox LogBoxVision = new RichTextBox()
        {
            Background = new SolidColorBrush(Colors.Black),
            Width = 776
        };

        public Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        Paragraph lastParagrah = new Paragraph();
        Paragraph lastParagrahV = new Paragraph();

        private void Write_Log(string log, bool IsTxWrite = true, bool IsConinue = false)
        {
            if (log.Length < 1) return;
            LogBox.Dispatcher.BeginInvoke(new Action(delegate
            {
                if (LogBox.Document.Blocks.Count > 100) LogBox.Document.Blocks.Clear();
                if (LogBoxVision.Document.Blocks.Count > 100) LogBoxVision.Document.Blocks.Clear();



                var paragraph = new Paragraph();
                paragraph.LineHeight = 1;
                var paragraphV = new Paragraph();
                paragraphV.LineHeight = 1;
                if (IsConinue)
                {
                    paragraph = lastParagrah;
                     paragraphV = lastParagrahV;
                }

                if (IsConinue)
                {
                    if (IsTxWrite)
                    {
                        paragraph.Inlines.Add(new Run(String.Format("{0} ", log)));
                        paragraph.Foreground = new SolidColorBrush(Colors.Yellow);
                        paragraphV.Inlines.Add(new Run(String.Format("{0} ", log)));
                        paragraphV.Foreground = new SolidColorBrush(Colors.Yellow);
                    }

                    else
                    {
                        paragraph.Inlines.Add(new Run(String.Format("{0} ", log)));
                        paragraph.Foreground = new SolidColorBrush(Colors.LightGreen);
                        paragraphV.Inlines.Add(new Run(String.Format("{0} ", log)));
                        paragraphV.Foreground = new SolidColorBrush(Colors.LightGreen);
                    }
                }
                else
                {
                    if (IsTxWrite)
                    {
                        paragraph.Inlines.Add(new Run(String.Format("L{0} {1}: {2}", LogBox.Document.Blocks.Count.ToString().PadLeft(3, '0'), DateTime.Now.ToString("HH:mm:ss -> Tx: "), log)));
                        paragraph.Foreground = new SolidColorBrush(Colors.Yellow);
                        paragraphV.Inlines.Add(new Run(String.Format("L{0} {1}: {2}", LogBoxVision.Document.Blocks.Count.ToString().PadLeft(3, '0'), DateTime.Now.ToString("HH:mm:ss -> Tx: "), log)));
                        paragraphV.Foreground = new SolidColorBrush(Colors.Yellow);
                    }

                    else
                    {
                        paragraph.Inlines.Add(new Run(String.Format("L{0} {1}: {2}", LogBox.Document.Blocks.Count.ToString().PadLeft(3, '0'), DateTime.Now.ToString("HH:mm:ss <- Rx: "), log)));
                        paragraph.Foreground = new SolidColorBrush(Colors.LightGreen);
                        paragraphV.Inlines.Add(new Run(String.Format("L{0} {1}: {2}", LogBoxVision.Document.Blocks.Count.ToString().PadLeft(3, '0'), DateTime.Now.ToString("HH:mm:ss <- Rx: "), log)));
                        paragraphV.Foreground = new SolidColorBrush(Colors.LightGreen);
                    }
                    lastParagrah = paragraph;
                    lastParagrahV = paragraphV;
                }

                LogBox.Document.Blocks.Add(paragraph);
                LogBoxVision.Document.Blocks.Add(paragraphV);
                LogBox.ScrollToEnd();
                LogBoxVision.ScrollToEnd();
            }), DispatcherPriority.Send);

        }
    }
}

