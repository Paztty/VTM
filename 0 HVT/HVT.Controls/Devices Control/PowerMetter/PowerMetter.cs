using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static HVT.Controls.DMM;

namespace HVT.Controls.Devices_Control
{
    public class PowerMetter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //Name
        public string Name = "Power Metter";
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
        // static get values frame
        private byte[] GetFrameID01 = new byte[8] { 0x01, 0x03, 0x00, 0x23, 0x00, 0x0B, 0xF5, 0xC7 };
        private byte[] GetFrameID02 = new byte[8] { 0x02, 0x03, 0x00, 0x23, 0x00, 0x0B, 0xF5, 0xF4 };
        private byte[] GetFrameID03 = new byte[8] { 0x03, 0x03, 0x00, 0x23, 0x00, 0x0B, 0xF4, 0x25 };
        private byte[] GetFrameID04 = new byte[8] { 0x01, 0x03, 0x00, 0x23, 0x00, 0x0B, 0x55, 0x92 };

        public List<PowerMettterValueHolder> ValueHolders = new List<PowerMettterValueHolder>(4);// MAX 4 BOARD 
        public PowerMetter()
        {
            SerialPort.DevieceName = this.Name;
            SerialPort.BlinkTime = 50;
            SerialPort.Port = new SerialPort()
            {
                PortName = "COM1",
                BaudRate = 9600,
                ReadTimeout = 500,
            };

            for (int i = 0; i < 4; i++)
            {
                ValueHolders.Add(new PowerMettterValueHolder());
            }
        }

        public async void CheckCommunication(string COM_NAME)
        {
            try
            {
                SerialPort.DevieceName = this.Name;
                SerialPort.BlinkTime = 50;
                SerialPort.Port = new SerialPort()
                {
                    PortName = COM_NAME,
                    BaudRate = 9600,
                    ReadTimeout = 500,
                };
                SerialPort.Port.Open();
                SerialPort.OpenPort();
            }
            catch (Exception)
            {
            }
        }

        public bool Read(char Site)
        {
            foreach (var item in ValueHolders)
            {
                item.ClearValue();
            }
            byte[] frame;
            switch (Site)
            {
                case 'A':
                    frame = GetFrameID01;
                    break;
                case 'B':
                    frame = GetFrameID02;
                    break;
                case 'C':
                    frame = GetFrameID03;
                    break;
                case 'D':
                    frame = GetFrameID04;
                    break;
                default:
                    return false;
            }

            if (SerialPort.SendAndRead(frame,500, out List<byte> Response))
            {
                switch (Site)
                {
                    case 'A':
                        return ValueHolders[0].GetValue(Response);
                    case 'B':
                        return ValueHolders[1].GetValue(Response);
                    case 'C':
                        return ValueHolders[2].GetValue(Response);
                    case 'D':
                        return ValueHolders[3].GetValue(Response);
                    default:
                        return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
