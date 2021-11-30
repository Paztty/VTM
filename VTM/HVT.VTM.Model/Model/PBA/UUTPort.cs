using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HVT.Utility;


namespace HVT.VTM.Base
{

    public enum CheckSumType
    { 
        XOR,
        CRC8,
        CRC16,
        CRC16_CCITT,
        CRC16_MOSBUS,
        CRC32,
        CRC8_REVERSED,
        SUM
    }

    public class UUT_Config
    {
        private int baudrate = 9600;
        public int Baudrate
        { get { return baudrate; } set { baudrate = value; } }

        private SerialData serialData;
        public SerialData SerialData 
        { get { return serialData; } set { serialData = value; } }

        private Parity parity = Parity.None;
        public Parity Parity
        { get { return parity; } set { parity = value; } }

        private StopBits stopBits = StopBits.None;
        public StopBits StopBits
        { get { return stopBits; } set { stopBits = value; } }

        private int dataBit = 8;
        public int DataBit
        { get { return dataBit; } set { dataBit = value; } }

        
        private bool usePrefix1 = false;
        public bool UsePrefix1 { get { return usePrefix1; } set { usePrefix1 = value; } }

        private byte prefix1;
        public byte Prefix1 
        { get { return prefix1; } set { prefix1 = value; } }

        private bool usePrefix2 = false;
        public bool UsePrefix2 { get { return usePrefix2; } set { usePrefix2 = value; } }

        private byte prefix2;
        public byte Prefix2
        { get { return prefix2; } set { prefix2 = value; } }

        private bool useSuffix = false; 
        public bool UsingSuffix { get { return useSuffix; } set { useSuffix = value; } }

        private byte suffix;
        public byte Suffix
        { get { return suffix; } set { suffix = value; } }

        public int lenghtFixed;
        public int LenghtFixed 
        { get { return lenghtFixed; } set { lenghtFixed = value; } }

        private int startChecksumCal = 0;
        public int StartChecksumCal
        { get { return startChecksumCal; } set { startChecksumCal = value; } }

        public byte[] GetFrame(byte[] data)
        {
            List<byte> dataToSend = new List<byte>();
            if (data == null) return null; 

            dataToSend.AddRange(data);
            if (usePrefix1) dataToSend.Insert(0, prefix1);
            if (usePrefix1 && usePrefix2) dataToSend.Insert(1, prefix2);

            return dataToSend.ToArray();
        }
    }


    public class UUTPort
    {
        public SerialDisplay SerialDisplay = new SerialDisplay();
        private SerialPort serial = new SerialPort();

        private string serialname;
        public string SerialName
        {
            get { return serialname; }
            set
            {
                if (serialname != value)
                {
                    serialname = value;
                    RefindCom();
                }
            }
        }




        private void RefindCom()
        {
            if (serial.IsOpen)
            {
                serial.Close();
                SerialDisplay.ShowCOMStatus(false);
            }
            serial.PortName = SerialName;
            serial.BaudRate = 9600;
            try
            {
                serial.Open();
                SerialDisplay.ShowCOMStatus(true);
            }
            catch (Exception)
            { }
        }


    }
}
