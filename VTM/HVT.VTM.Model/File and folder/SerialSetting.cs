using System.IO.Ports;

namespace HVT.VTM.Base
{
    public class SerialSetting
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public string PortName { get; set; }
        public int Baudrate { get; set; }
        public StopBits stopBits { get; set; }
        public Parity parity { get; set; }
        public int Databits { get; set; }
        public string EndFrame { get; set; } = "\r\n";

        public void Set_SettingParam(SerialPort port)
        {
            PortName = port.PortName;
            Baudrate = port.BaudRate;
            stopBits = port.StopBits;
            parity = port.Parity;
            stopBits = port.StopBits;
            Databits = port.DataBits;
        }

        public SerialPort Get_SettingParam()
        {
            SerialPort port = new SerialPort()
            {
                PortName = this.PortName,
                BaudRate = this.Baudrate,
                DataBits = this.Databits,
                Parity = this.parity,
                StopBits = this.stopBits,
            };
            return port;
        }

    }
}
