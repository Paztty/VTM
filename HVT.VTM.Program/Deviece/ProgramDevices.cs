using HVT.VTM.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Linq;
using System.Collections.Generic;
using HVT.Utility;
using System.Windows;
using HVT.Controls;
using Camera;
using HVT.Controls.Devices_Control;

namespace HVT.VTM.Program
{
    public partial class Program
    {
        // Devieces list 
        public GWIN_TECH_DMM _DMM = new GWIN_TECH_DMM();
        public MuxCardControl MuxCard = new MuxCardControl();
        public RelayControls RELAY = new RelayControls();
        public LevelDataViewer LEVEL = new LevelDataViewer();
        public SolenoidControls Solenoid = new SolenoidControls();
        public VisionTester VisionTester = new VisionTester();
        public SYSIOcontrol SYSTEM = new SYSIOcontrol();
        public PowerMetter PowerMetter = new PowerMetter();
        public MotorExtension MotorExtension = new MotorExtension();
        public CameraControl Capture;
        public SerialPortDisplay BarcodeReader = new SerialPortDisplay();
        public List<UUTPort> UUTs = new List<UUTPort>()
        {
            new UUTPort(){
                serial = new SerialPortDisplay()
                {
                    DevieceName = "UUT 1",
                }
            },
            new UUTPort(){
                serial = new SerialPortDisplay()
                {
                    DevieceName = "UUT 2",
                }
            },
            new UUTPort(){
                serial = new SerialPortDisplay()
                {
                    DevieceName = "UUT 3",
                }
            },
            new UUTPort(){
                serial = new SerialPortDisplay()
                {
                    DevieceName = "UUT 4",
                }
            }
        };
        public Printer_QR Printer = new Printer_QR();
        // Check devieces comunications 

        public async void CheckComnunication()
        {
            SYSTEM.System_Board.SerialPort.Port?.Close();
            MuxCard.SerialPort1.Port?.Close();
            MuxCard.SerialPort2.Port?.Close();
            _DMM.DMM1.SerialPort.Port?.Close();
            _DMM.DMM2.SerialPort.Port?.Close();
            RELAY.SerialPort.Port?.Close();
            LEVEL.SerialPort.Port?.Close();
            Solenoid.SerialPort.Port?.Close();
            BarcodeReader.Port?.Close();
            PowerMetter.SerialPort.Port?.Close();
            MotorExtension.SerialPort.Port?.Close();
            BarcodeReader.Port?.Close();

            UUTs[0].serial.Port?.Close();
            UUTs[1].serial.Port?.Close();
            UUTs[2].serial.Port?.Close();
            UUTs[3].serial.Port?.Close();
            await Task.Delay(50);
            //SYSTEM check
            SYSTEM.System_Board.CheckCardComunication(appSetting.Communication.SystemIOPort);
            SYSTEM.System_Board.MachineIO.OnStartRequest += MachineIO_OnStartRequest;
            SYSTEM.System_Board.MachineIO.OnCancleRequest += MachineIO_OnCancleRequest;
            SYSTEM.System_Board.MachineIO.OnDoorStateChange += MachineIO_OnDoorStateChange;
            SYSTEM.System_Board.MachineIO.OnUpDown += MachineIO_OnUpDown;
            await Task.Delay(50);
            //MUX Check
            MuxCard.CheckCard1Comunication(appSetting.Communication.Mux1Port);
            await Task.Delay(50);
            MuxCard.CheckCard2Comunication(appSetting.Communication.Mux2Port);
            await Task.Delay(50);
            // DMM check
            _DMM.DMM1.CheckCommunication(appSetting.Communication.DMM1Port);
            await Task.Delay(50);
            _DMM.DMM2.CheckCommunication(appSetting.Communication.DMM2Port);
            await Task.Delay(50);
            // RELAY CHECK
            RELAY.CheckCardComunication(appSetting.Communication.RelayPort);
            await Task.Delay(50);
            LEVEL.CheckCardComunication(appSetting.Communication.LevelPort);
            await Task.Delay(50);
            Solenoid.CheckCardComunication(appSetting.Communication.SolenoidPort);
            await Task.Delay(50);
            //Power metter check
            PowerMetter.CheckCommunication(appSetting.Communication.PowerMetterPort);
            await Task.Delay(50);
            //Motor Extension port check 
            MotorExtension.CheckCommunication(appSetting.Communication.MotorPort);
            await Task.Delay(50);
            // Barcode scand
            CheckBarcodeReader(appSetting.Communication.ScannerPort);
            await Task.Delay(50);
            // UUTs

            UUTs[0].CheckPort(appSetting.Communication.UUT1Port);
            await Task.Delay(50);
            UUTs[1].CheckPort(appSetting.Communication.UUT2Port);
            await Task.Delay(50);
            UUTs[2].CheckPort(appSetting.Communication.UUT3Port);
            await Task.Delay(50);
            UUTs[3].CheckPort(appSetting.Communication.UUT4Port);
            await Task.Delay(50);
        }

        private void MachineIO_OnUpDown(object sender, EventArgs e)
        {
            string warningMess = "";
            if (!SYSTEM.System_Board.MachineIO.SS_BL)
            {
                warningMess += "Bottom JIG non lock \n";
            }

            if (!SYSTEM.System_Board.MachineIO.SS_TL)
            {
                warningMess += "Top JIG non lock \n";
            }

            if (SYSTEM.System_Board.MachineIO.SS_BR)
            {
                warningMess += "Bottom Card non lock \n";
            }

            if (SYSTEM.System_Board.MachineIO.SS_TR)
            {
                warningMess += "Top Card non lock \n";
            }

            if (warningMess != "")
            {
                Debug.Write(String.Format("VTM warning: {0}", warningMess), Debug.ContentType.Warning);
            }
        }

        private void MachineIO_OnDoorStateChange(object sender, EventArgs e)
        {
            if (SYSTEM.System_Board.MachineIO.IsDoorOpen)
            {
                Debug.Write("Machine door open.", Debug.ContentType.Warning);
            }
        }

        private void MachineIO_OnCancleRequest(object sender, EventArgs e)
        {
            if (IsTestting)
            {
                TestState = RunTestState.STOP;
                IsTestting = false;
            }
        }

        private void MachineIO_OnStartRequest(object sender, EventArgs e)
        {
            ResultPanel.Dispatcher.Invoke(new Action(() =>
            ResultPanel.Visibility = Visibility.Hidden));
            if (!IsTestting && IsloadModel)
            {
                if (pageActive == PageActive.AutoPage)
                {
                    IsTestting = true;
                }
            }
        }
    }
}
