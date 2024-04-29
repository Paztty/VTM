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
        }

        public async void CheckCommunication(string COM_NAME)
        {
            var checkResult = await SerialPort.CheckComPort(COM_NAME, 115200, "*IDN?", ",GDM8261A", 500, this.Name);

            if (checkResult)
            {

            }

        }
    }
}
