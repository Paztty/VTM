using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HVT.Controls.Devices_Control
{
    public class MotorExtension: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //Name
        public string Name = "MotorExtension";

        private SerialPortDisplay serialPort = new SerialPortDisplay();
        public SerialPortDisplay SerialPort
        {
            get { return serialPort; }
            set
            {
                serialPort = value;
            }
        }
        
        public MotorExtension()
        {
            SerialPort.DevieceName = this.Name;
            SerialPort.BlinkTime = 50;
            SerialPort.Port = new SerialPort()
            {
                PortName = "COM1",
                BaudRate = 115200,
                ReadTimeout = 500,
            };
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
                    BaudRate = 115200,
                    ReadTimeout = 500,
                };
                SerialPort.Port.Open();
                SerialPort.OpenPort();
            }
            catch (Exception)
            {
            }
        }

        public bool Read(out List<float> RPMs)
        {
            RPMs = new List<float>(4) { 0,0,0,0 };
            if (SerialPort.SendAndRead(new byte[] { 0x50 },0x50, 1000, out byte[] Response))
            {
                for (int i = 0; i < 4; i++)
                {
                    RPMs[i] = ConvertToFloat(Response, i * 4 + 4);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public float ConvertToFloat(byte[] value, int index)
        {
            float f = 0;
            byte[] bytes = { value[index], value[index + 1], value[index + 2], value[index + 3] };
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes); // Convert big endian to little endian
            }
            f = BitConverter.ToSingle(bytes, 0);
            return f;
        }
    }
}
