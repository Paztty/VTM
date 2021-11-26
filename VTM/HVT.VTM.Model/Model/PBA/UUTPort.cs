using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HVT.Utility;


namespace HVT.VTM.Base
{
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
