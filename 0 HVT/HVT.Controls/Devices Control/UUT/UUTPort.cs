using HVT.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace HVT.Controls
{

    public class UUT_Config : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public bool Use { get; set; } = true;

        public enum PortKind
        {
            TTL,
            RS485
        }

        private PortKind kind;
        public int Kind
        {
            get { return (int)kind; }
            set
            {
                if (value != (int)kind)
                {
                    if (Enum.IsDefined(typeof(PortKind), value))
                    {
                        kind = (PortKind)value;
                    }
                    NotifyPropertyChanged("Kind");
                }
            }
        }
        public int[] Baudrates { get; } = new int[] { 110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000 };
        public int baudrate = 9600;
        public int Baudrate
        {
            get { return Array.IndexOf(Baudrates, baudrate);}
            set
            {
                if (value != Array.IndexOf(Baudrates, baudrate) && value < Baudrates.Length)
                {
                    baudrate = Baudrates[value];
                    NotifyPropertyChanged("Baudrate");
                }
            }
        }

        public Parity parity = Parity.Even;
        public int ParityBit
        {
            get { return (int)parity; }
            set
            {
                if (value != (int)parity)
                {
                    if (Enum.IsDefined(typeof(Parity), value))
                    {
                        parity = (Parity)value;
                    }
                    NotifyPropertyChanged("ParityBit");
                }
            }
        }

        public StopBits stopBits = StopBits.One;
        public int StopBits_Index
        {
            get { return (int)stopBits; }
            set
            {
                if (value != (int)stopBits)
                {
                    if (Enum.IsDefined(typeof(StopBits), value))
                    {
                        stopBits = (StopBits)value;
                    }
                    NotifyPropertyChanged("StopBits_Index");
                }
            }
        }

        public int dataBit = 8;
        public int DataBit
        {
            get { return 8 - dataBit; }
            set
            {
                if (dataBit != value)
                {
                    if (value == 0 || value == 1)
                    {
                        dataBit = 8 - value;
                        NotifyPropertyChanged("DataBit");
                    }
                }
            }
        }


        private bool usePrefix1 = true;
        public bool UsePrefix1
        {
            get
            {
                return usePrefix1;
            }
            set
            {
                if (value != usePrefix1)
                {
                    usePrefix1 = value;
                    NotifyPropertyChanged(nameof(UsePrefix1));
                }
            }
        }

        private int prefix1 = 0x5A;
        public int Prefix1
        {
            get { return prefix1; }
            set
            {
                if (value != prefix1)
                {
                    prefix1 = value;
                    NotifyPropertyChanged(nameof(Prefix1));
                }
            }
        }

        private bool usePrefix2 = true;
        public bool UsePrefix2
        {
            get
            {
                return usePrefix2;
            }
            set
            {
                if (value != usePrefix2)
                {
                    usePrefix2 = value;
                    NotifyPropertyChanged(nameof(UsePrefix2));
                }
            }
        }

        private int prefix2 = 0x5A;
        public int Prefix2
        {
            get { return prefix2; }
            set
            {
                if (value != prefix2)
                {
                    prefix2 = value;
                    NotifyPropertyChanged(nameof(Prefix2));
                }
            }
        }

        private bool useSuffix = false;
        public bool UseSuffix
        {
            get { return useSuffix; }
            set
            {
                if (value != useSuffix)
                {
                    useSuffix = value;
                    NotifyPropertyChanged(nameof(UseSuffix));
                }
            }
        }

        private int suffix;
        public int Suffix
        {
            get { return suffix; }
            set
            {
                if (value != suffix)
                {
                    suffix = value;
                    NotifyPropertyChanged(nameof(Suffix));
                }
            }
        }

        private bool useLenghtFixed = true;
        public bool UseLenghtFixed
        {
            get { return useLenghtFixed; }
            set
            {
                if (value != useLenghtFixed)
                {
                    useLenghtFixed = value;
                    NotifyPropertyChanged(nameof(UseLenghtFixed));
                }
            }
        }

        public int lenghtFixed;
        public int LenghtFixed
        {
            get { return lenghtFixed; }
            set
            {
                if (value != lenghtFixed)
                {
                    lenghtFixed = value;
                    NotifyPropertyChanged(nameof(LenghtFixed));
                }
            }
        }

        private bool useRxPrefix1 = true;
        public bool UseRxPrefix1
        {
            get
            {
                return useRxPrefix1;
            }
            set
            {
                if (value != useRxPrefix1)
                {
                    useRxPrefix1 = value;
                    NotifyPropertyChanged(nameof(UseRxPrefix1));
                }
            }
        }

        private int Rxprefix1 = 0x5A;
        public int RxPrefix1
        {
            get { return Rxprefix1; }
            set
            {
                if (value != Rxprefix1)
                {
                    Rxprefix1 = value;
                    NotifyPropertyChanged(nameof(RxPrefix1));
                }
            }
        }

        private bool useRxPrefix2 = true;
        public bool UseRxPrefix2
        {
            get
            {
                return useRxPrefix2;
            }
            set
            {
                if (value != useRxPrefix2)
                {
                    useRxPrefix2 = value;
                    NotifyPropertyChanged(nameof(UseRxPrefix2));
                }
            }
        }

        private int Rxprefix2 = 0x5A;
        public int RxPrefix2
        {
            get { return Rxprefix2; }
            set
            {
                if (value != Rxprefix2)
                {
                    Rxprefix2 = value;
                    NotifyPropertyChanged(nameof(RxPrefix2));
                }
            }
        }

        private bool useRxSuffix = false;
        public bool UseRxSuffix
        {
            get { return useRxSuffix; }
            set
            {
                if (value != useRxSuffix)
                {
                    useRxSuffix = value;
                    NotifyPropertyChanged(nameof(UseRxSuffix));
                }
            }
        }

        private int Rxsuffix;
        public int RxSuffix
        {
            get { return Rxsuffix; }
            set
            {
                if (value != Rxsuffix)
                {
                    Rxsuffix = value;
                    NotifyPropertyChanged(nameof(RxSuffix));
                }
            }
        }

        private bool useRxLenghtFixed = true;
        public bool UseRxLenghtFixed
        {
            get { return useRxLenghtFixed; }
            set
            {
                if (value != useRxLenghtFixed)
                {
                    useRxLenghtFixed = value;
                    NotifyPropertyChanged(nameof(UseRxLenghtFixed));
                }
            }
        }

        public int RxlenghtFixed;
        public int RxLenghtFixed
        {
            get { return RxlenghtFixed; }
            set
            {
                if (value != RxlenghtFixed)
                {
                    RxlenghtFixed = value;
                    NotifyPropertyChanged(nameof(RxLenghtFixed));
                }
            }
        }

        private bool useRxLengFixed = true;
        public bool UseRxLengFixed
        {
            get { return useRxLengFixed; }
            set
            {
                if (value != useRxLengFixed)
                {
                    useRxLengFixed = value;
                    NotifyPropertyChanged(nameof(UseRxGetLengtLoc));
                    NotifyPropertyChanged(nameof(UseRxLengFixed));
                }
            }
        }

        public bool UseRxGetLengtLoc
        {
            get { return !useRxLengFixed; }
            set
            {
                if (value == useRxLengFixed)
                {
                    useRxLengFixed = !value;
                    NotifyPropertyChanged(nameof(UseRxLengFixed));
                    NotifyPropertyChanged(nameof(UseRxGetLengtLoc));
                }
            }
        }

        public int rxlenghtStart;
        public int RxLenghtStart
        {
            get { return rxlenghtStart; }
            set
            {
                if (value != rxlenghtStart)
                {
                    rxlenghtStart = value;
                    NotifyPropertyChanged(nameof(RxLenghtStart));
                }
            }
        }

        public int rxlenghtEnd;
        public int RxLenghtEnd
        {
            get { return rxlenghtEnd; }
            set
            {
                if (value != rxlenghtEnd)
                {
                    rxlenghtEnd = value;
                    NotifyPropertyChanged(nameof(RxLenghtEnd));
                }
            }
        }

        public int rxlenghtByteCount;
        public int RxLenghtByteCount
        {
            get { return rxlenghtByteCount; }
            set
            {
                if (value != rxlenghtByteCount)
                {
                    rxlenghtByteCount = value;
                    NotifyPropertyChanged(nameof(RxLenghtByteCount));
                }
            }
        }

        private CheckSumType checkSumType = CheckSumType.XOR;
        public CheckSumType Checksum
        {
            get { return checkSumType; }
            set
            {
                if (checkSumType != value)
                {
                    if (Enum.IsDefined(typeof(CheckSumType), value))
                    {
                        checkSumType = (CheckSumType)value;
                    }
                    NotifyPropertyChanged("Checksum");
                }
            }
        }


        private int startChecksumCal = 0;
        public int StartChecksumCal
        {
            get { return startChecksumCal; }
            set
            {
                if (value != startChecksumCal)
                {
                    startChecksumCal = value;
                    NotifyPropertyChanged(nameof(StartChecksumCal));
                }
            }
        }

        private int endChecksumCal = 0;
        public int EndChecksumCal
        {
            get { return endChecksumCal; }
            set
            {
                if (value != endChecksumCal)
                {
                    endChecksumCal = value;
                    NotifyPropertyChanged(nameof(EndChecksumCal));
                }
            }
        }

        private int clearRxTime = 100;
        public int ClearRxTime
        {
            get { return clearRxTime; }
            set
            {
                if (value != clearRxTime)
                {
                    clearRxTime = value;
                    NotifyPropertyChanged(nameof(ClearRxTime));
                }
            }
        }

        public bool ClearRxTimeSpecified { get; set; } = false;

         public byte[] GetFrame(string Txdata)
        {
            List<byte> dataToSend = new List<byte>();
            var dataStr = Txdata.Replace(" ", "");
            if (dataStr == null) return null;
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
}

