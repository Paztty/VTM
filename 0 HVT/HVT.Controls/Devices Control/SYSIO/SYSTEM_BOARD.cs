using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.Primitives;

namespace HVT.Controls
{
    public class SYSTEM_BOARD
    {
        public SystemMachineIO MachineIO = new SystemMachineIO();

        private SerialPortDisplay _SerialPort = new SerialPortDisplay();
        public SerialPortDisplay SerialPort
        {
            get { return _SerialPort; }
            set
            {
                if (value != _SerialPort) _SerialPort = value;
            }
        }

        public SYSTEM_BOARD()
        {
            SerialPort.DevieceName = "SYSTEM";
            SerialPort.Port = new System.IO.Ports.SerialPort
            {
                PortName = "COM1",
                BaudRate = 9600,
                DataBits = 8,
                Parity = System.IO.Ports.Parity.None,
                StopBits = System.IO.Ports.StopBits.One,
                ReceivedBytesThreshold = 1
            };
        }

        public async void CheckCardComunication(string COMNAME)
        {
            byte[] inputAsk = new byte[2];
            SerialPort.SerialDataReciver -= SerialPort_SerialDataReciver;
            inputAsk[0] = (byte)0x49;
            byte[] cardResponse = SYSTEM_COMUNICATION.GetFrame(new byte[] { 0x00 });
            bool result = await SerialPort.CheckBoardComPort(COMNAME, 9600, inputAsk, cardResponse, 250, true);
            SerialPort.SerialDataReciver += SerialPort_SerialDataReciver;
            GetInput();
        }

        private void SerialPort_SerialDataReciver(object sender, EventArgs e)
        {
            if (SerialPort.Port.IsOpen)
            {
                List<byte> frame = new List<byte>();
                int size = SerialPort.Port.BytesToRead;
                byte[] bytes = new byte[size];
                try
                {
                    SerialPort.Port.Read(bytes, 0, SerialPort.Port.BytesToRead);
                }
                catch (Exception)
                {
                    return;
                }
                if (bytes.Length < 7) return;
                for (int i = 0; i < bytes.Length; i++)
                {
                    byte startByte = bytes[i];
                    if (startByte == SYSTEM_COMUNICATION.Prefix1)
                    {
                        var secondByte = bytes[i + 1];
                        if (secondByte == SYSTEM_COMUNICATION.Prefix2)
                        {
                            frame.Clear();
                            frame.Add(startByte);
                            frame.Add(secondByte);
                            frame.Add(bytes[i + 2]);
                            if ((int)bytes[i + 2] + 3 >= bytes.Length) return;
                            for (int j = i + 3; j <= (int)bytes[i + 2] + 3; j++)
                            {
                                frame.Add(bytes[j]);
                                Console.WriteLine(j);
                            }
                            MachineIO.DataToIO(new byte[] { frame[4], frame[5], frame[6], frame[7] });
                            {
                                Console.Write("SYS INPUT:");
                                foreach (var item in frame)
                                {
                                    Console.Write(item.ToString("X2") + " ");
                                }
                                Console.WriteLine(" ");
                                return;
                            }
                        }
                    }
                }
            }
        }

        public void SendControl()
        {
            var data = MachineIO.IOtoData();
            if (SerialPort.Port.IsOpen)
            {
                SerialPort.SendBytes(SYSTEM_COMUNICATION.GetFrame(data));
            }
        }

        public void GetInput()
        {
            byte[] inputAsk = new byte[2];
            inputAsk[0] = (byte)0x49;
            if (SerialPort.Port.IsOpen)
            {
                Console.WriteLine("Query input ");
                SerialPort.SendBytes(SYSTEM_COMUNICATION.GetFrame(inputAsk, true));
            }
        }

        public void PowerRelease()
        {
            MachineIO.AC0 = false;
            MachineIO.BC0 = false;
            MachineIO.ADSC = false;
            MachineIO.BDSC = false;
            MachineIO.LPG = true;
            MachineIO.BUZZER = false;
            SendControl();
        }

        public bool GEN(int value, List<int> Channel)
        {
            SerialPort.SerialDataReciver -= SerialPort_SerialDataReciver;
            byte[] bytes = new byte[13];
            if (MachineIO.GEN_BYTES.Count() == 13)
            {
                bytes = MachineIO.GEN_BYTES.ToArray();
            }
            bytes[0] = 0x47;
            byte[] intBytes = BitConverter.GetBytes(value);
            Array.Reverse(intBytes);
            byte[] result = intBytes;
            foreach (var item in Channel)
            {
                for (int i = 1; i < 4; i++)
                {
                    bytes[(item - 1) * 3 + i] = result[i];
                }
            }

            Console.Write("GEN: ");
            MachineIO.GEN_BYTES = bytes.ToList();
            bool IsOK = SerialPort.SendAndRead(bytes, 0x47, 1000, out _, false);
            SerialPort.SerialDataReciver += SerialPort_SerialDataReciver;
            return IsOK;
        }
    }
}
