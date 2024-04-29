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

        public List<PowerMettterValueHolder> ValueHolders = new List<PowerMettterValueHolder>(4);
        public PowerMetter()
        {
            SerialPort.DevieceName = this.Name;
            SerialPort.BlinkTime = 50;
            SerialPort.Port = new SerialPort()
            {
                PortName = "COM1",
                BaudRate = 115200,
                ReadTimeout = 500,
            };

            for (int i = 0; i < 4; i++)
            {
                ValueHolders.Add(new PowerMettterValueHolder());
            }
        }

        public async void CheckCommunication(string COM_NAME)
        {
            var checkResult = await SerialPort.CheckComPort(COM_NAME, 115200, "*IDN?", ",GDM8261A", 500, this.Name);
            if (checkResult)
            {

            }
        }

        public bool Read(char Site)
        {
            foreach (var item in ValueHolders)
            {
                item.ClearValue();
            }
            //Read holder resigter frame
            List<byte> frame =new List<byte> { 0x01, 0x03, 0x00, 0x0B, 0x00, 0x09, 0x45, 0x0A };

            switch (Site)
            {
                case 'A':
                    frame.Insert(0, 0x01);
                    break;
                case 'B':
                    frame.Insert(0, 0x02);
                    break;
                case 'C':
                    frame.Insert(0, 0x03);
                    break;
                case 'D':
                    frame.Insert(0, 0x04);
                    break;
                default:
                    return false;
            }

            if(SerialPort.SendAndRead(frame.ToArray(),500, out List<byte> Response))
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
