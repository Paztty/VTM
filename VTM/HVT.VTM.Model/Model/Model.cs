using HVT.Utility;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HVT.VTM.Base
{
    public partial class Model : ICloneable
    {

        public string Name { get; set; }
        public string Path { get; set; }

        #region PCB layout
        public Contruction contruction { get; set; } = new Contruction();

        #endregion

        #region Error Positions set
        public class ErrorPosition
        {
            public readonly System.Windows.Controls.Label rect = new System.Windows.Controls.Label()
            {
                Background = new SolidColorBrush(Color.FromArgb(0, 255, 0, 0)),
                Foreground = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(Colors.Red),
                BorderThickness = new Thickness(1),
                Cursor = Cursors.Hand,
            };

            private int no;
            public int No
            {
                get { return no; }
                set
                {
                    no = value;
                    rect.Content = value;
                }
            }

            public double X { get; set; }
            public double Y { get; set; }

            public double lbTop { get; set; }
            public double lbLeft { get; set; }

            private double width;
            public double Width
            {
                get { return width; }
                set
                {
                    width = value;
                    rect.Width = Math.Abs(value);
                }
            }
            private double height;
            public double Height
            {
                get { return height; }
                set
                {
                    height = value;
                    rect.Height = Math.Abs(value);
                }
            }
            public string Position { get; set; }
        }
        public List<ErrorPosition> ErrorPositions { get; set; } = new List<ErrorPosition>();
        #endregion

        #region Step test 
        public ObservableCollection<Step> Steps { get; set; } = new ObservableCollection<Step>();

        public void CleanSteps()
        {
            foreach (var step in Steps)
            {
                step.Result1 = "";
                step.Result2 = "";
                step.Result3 = "";
                step.Result4 = "";

                step.ValueGet1 = "";
                step.ValueGet2 = "";
                step.ValueGet3 = "";
                step.ValueGet4 = "";
            }
        }

        public void UpdateCommand()
        {
            foreach (var step in Steps)
            {
                step.CommandDescriptions = Command.Commands.SingleOrDefault(x => x.CMD == step.cmd);
            }
        }
        #endregion

        #region Barcode settup
        public class Barcode
        {
            public int No { get; set; }
            public string BarcodeData { get; set; }
            public int Lenght { get; set; }
            public int StartModelCodePosition { get; set; }
            public string ModelCode { get; set; }
        }
        public ObservableCollection<Barcode> Barcodes { get; set; } = new ObservableCollection<Barcode>()
        {
         new Barcode(){No = 0, BarcodeData = "", Lenght=23, StartModelCodePosition= 3, ModelCode="" },
         new Barcode(){No = 1, BarcodeData = "", Lenght=23, StartModelCodePosition= 3, ModelCode="" },
         new Barcode(){No = 2, BarcodeData = "", Lenght=23, StartModelCodePosition= 3, ModelCode="" },
         new Barcode(){No = 3, BarcodeData = "", Lenght=23, StartModelCodePosition= 3, ModelCode="" },
        };
        public ObservableCollection<Barcode> BarcodesWaitting = new ObservableCollection<Barcode>()
        {
         new Barcode(){No = 0, BarcodeData = "", Lenght=23, StartModelCodePosition= 3, ModelCode="" },
         new Barcode(){No = 1, BarcodeData = "", Lenght=23, StartModelCodePosition= 3, ModelCode="" },
         new Barcode(){No = 2, BarcodeData = "", Lenght=23, StartModelCodePosition= 3, ModelCode="" },
         new Barcode(){No = 3, BarcodeData = "", Lenght=23, StartModelCodePosition= 3, ModelCode="" },
        };
        #endregion

        #region Naming
        //Serial naming
        private Naming _naming = new Naming();
        public Naming Naming
        {
            get { return _naming; }
            set
            {
                _naming = value;
            }
        }
        #endregion

        #region Camera setting
        public bool HaveApplyCamsetting = false;
        private CameraSetting _cameraSetting = new CameraSetting();
        public CameraSetting CameraSetting
        {
            get { return _cameraSetting; }
            set { _cameraSetting = value; }
        }
        #endregion

        #region Vision test 

        public List<VisionFunctions.FND> FNDs { get; set; }
        public List<VisionFunctions.LCD> LCDs { get; set; }
        public List<VisionFunctions.GLED> GLEDs { get; set; }
        public List<VisionFunctions.LED> LEDs { get; set; }

        public void VisionTestInit(Canvas DrawingCanvas, Canvas DisplayFunction, Canvas ManualDisplayCanvas)
        {
            FNDs = new List<Base.VisionFunctions.FND>()
                    {
                    new Base.VisionFunctions.FND(1, "FND A", DrawingCanvas, DisplayFunction,ManualDisplayCanvas),
                    new Base.VisionFunctions.FND(2, "FND B", DrawingCanvas, DisplayFunction,ManualDisplayCanvas),
                    new Base.VisionFunctions.FND(3, "FND C", DrawingCanvas, DisplayFunction,ManualDisplayCanvas),
                    new Base.VisionFunctions.FND(4, "FND D", DrawingCanvas, DisplayFunction,ManualDisplayCanvas),
                    };

            LCDs = new List<Base.VisionFunctions.LCD>()
                    {
                    new Base.VisionFunctions.LCD(1, "LCD A", DrawingCanvas, DisplayFunction,ManualDisplayCanvas),
                    new Base.VisionFunctions.LCD(2, "LCD B", DrawingCanvas, DisplayFunction,ManualDisplayCanvas),
                    new Base.VisionFunctions.LCD(3, "LCD C", DrawingCanvas, DisplayFunction,ManualDisplayCanvas),
                    new Base.VisionFunctions.LCD(4, "LCD D", DrawingCanvas, DisplayFunction,ManualDisplayCanvas),
                    };

            GLEDs = new List<Base.VisionFunctions.GLED>()
            {
                new Base.VisionFunctions.GLED(ManualDisplayCanvas, DisplayFunction, DrawingCanvas, 0),
                new Base.VisionFunctions.GLED(ManualDisplayCanvas, DisplayFunction, DrawingCanvas, 1),
                new Base.VisionFunctions.GLED(ManualDisplayCanvas, DisplayFunction, DrawingCanvas, 2),
                new Base.VisionFunctions.GLED(ManualDisplayCanvas, DisplayFunction, DrawingCanvas, 3),
            };

            LEDs = new List<Base.VisionFunctions.LED>()
            {
                new Base.VisionFunctions.LED(ManualDisplayCanvas, DisplayFunction, DrawingCanvas, 0),
                new Base.VisionFunctions.LED(ManualDisplayCanvas, DisplayFunction, DrawingCanvas, 1),
                new Base.VisionFunctions.LED(ManualDisplayCanvas, DisplayFunction, DrawingCanvas, 2),
                new Base.VisionFunctions.LED(ManualDisplayCanvas, DisplayFunction, DrawingCanvas, 3),
            };


            foreach (var item in FNDs)
            {
                item.PlaceIn(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
            }
            foreach (var item in LCDs)
            {
                item.PlaceIn(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
            }
            foreach (var item in GLEDs)
            {
                foreach (var led in item.GLEDs)
                {
                    led.PlaceIn(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
                }
            }
            foreach (var item in LEDs)
            {
                foreach (var led in item.LEDs)
                {
                    led.PlaceIn(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
                }
            }

        }

        public void ReplaceComponent(Canvas DrawingCanvas, Canvas DisplayFunction, Canvas ManualDisplayCanvas)
        {
            DrawingCanvas.Children.Clear();
            DisplayFunction.Children.Clear();
            ManualDisplayCanvas.Children.Clear();

            foreach (var item in FNDs)
            {
                item.ReInit(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
                item.PlaceIn(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
            }
            foreach (var item in LCDs)
            {
                item.ReInit(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
                item.PlaceIn(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
            }
            foreach (var item in GLEDs)
            {
                foreach (var led in item.GLEDs)
                {

                    led.ReInit(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
                    led.PlaceIn(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
                }
            }
            foreach (var item in LEDs)
            {
                foreach (var led in item.LEDs)
                {

                    led.ReInit(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
                    led.PlaceIn(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
                }
            }

        }

        #endregion

        #region UUT config

        private UUT_Config p1_Config = new UUT_Config();
        private UUT_Config p2_Config = new UUT_Config();

        public UUT_Config P1_Config
        {
            get { return p1_Config; }
            set
            {
                if (p1_Config != value)
                {
                    p1_Config = value;
                }
            }
        }
        public UUT_Config P2_Config
        {
            get { return p2_Config; }
            set
            {
                if (p2_Config != value)
                {
                    p2_Config = value;
                }
            }
        }

        #endregion

        #region Event
        public event EventHandler LoadFinish;

        #endregion


        public void LoadFinishEvent()
        {
            foreach (var item in Barcodes) item.BarcodeData = "";
            LoadFinish?.Invoke(null, null);
        }

        public void Contruction_ContructionChanged()
        {
            Barcodes = new ObservableCollection<Barcode> { };
            BarcodesWaitting = new ObservableCollection<Barcode> { };

            for (int i = 0; i < contruction.PCB_Count; i++)
            {
                Barcodes.Add(new Barcode() { No = i + 1, BarcodeData = "", Lenght = 23, StartModelCodePosition = 3, ModelCode = "" });
                BarcodesWaitting.Add(new Barcode() { No = i + 1, BarcodeData = "", Lenght = 23, StartModelCodePosition = 3, ModelCode = "" });
            }
        }

        public Model() {
            Steps.Add(new Step()
            {
                No = 1,
            });
        }

        public void Load()
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                Title = "Import from FPT model",
            };
            openFile.FileOk += OpenFile_FileOk;
            openFile.Filter = "VTM model files (*.fdmdl)|*.fdmdl";
            openFile.RestoreDirectory = true;
            openFile.ShowDialog();
        }

        private void OpenFile_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var fileInfor = new FileInfo((sender as OpenFileDialog).FileName);
            Name = System.IO.Path.GetFileNameWithoutExtension((sender as OpenFileDialog).FileName);
            Path = (sender as OpenFileDialog).FileName;
            //Support.WriteLine("Load model : " + Name + Environment.NewLine + Path);
            string[] lines = System.IO.File.ReadAllLines((sender as OpenFileDialog).FileName);
            Steps = new ObservableCollection<Step>();

            Naming.TxDatas = new ObservableCollection<TxData>();
            Naming.RxDatas = new ObservableCollection<RxData>();
            Naming.QRDatas = new ObservableCollection<QRData>();

            foreach (string line in lines)
            {
                if (line.Contains("TEST STEP="))
                {
                    Step step = new Step();
                    string[] datas = line.Replace("TEST STEP=", "").Split('^');
                    step.No = Steps.Count;
                    step.TestContent = datas[0];
                    step.CMD = datas[1];
                    step.Condition1 = datas[2];
                    step.Oper = datas[3];
                    step.Condition2 = datas[4];
                    step.Spect = datas[5];
                    step.Min = datas[6];
                    step.Max = datas[7];
                    step.Min_Max = step.Min + "~" + step.Max;
                    step.Mode = datas[8];
                    step.Count = datas[9];
                    step.Mem = datas[10];
                    step.E_Jump = Int32.TryParse(datas[14], out _) == true ? Convert.ToInt32(datas[14]) : step.E_Jump;
                    step.Remark = datas[13];
                    step.Skip = datas[12] == "Y" ? true : false;
                    Steps.Add(step);
                }

                else if (line.Contains("PCB TOTAL="))
                {
                    var data = line.Replace("PCB TOTAL=", "");
                    contruction.PCB_Count = int.Parse(data);
                }
                else if (line.Contains("UUT TX NAMING="))
                {
                    var dataItem = line.Replace("UUT TX NAMING=", "").Split('|');
                    Naming.TxDatas.Add(new TxData()
                    {
                        No = Naming.TxDatas.Count,
                        Name = dataItem[0],
                        Data = dataItem[1],
                        Blank = dataItem[2],
                        Remark = dataItem[3]
                    });
                }
                else if (line.Contains("UUT RX NAMING="))
                {
                    var dataItem = line.Replace("UUT RX NAMING=", "").Split('|');
                    Naming.RxDatas.Add(new RxData()
                    {
                        No = Naming.RxDatas.Count,
                        Name = dataItem[0],
                        ModeLoc = dataItem[1],
                        Mode = dataItem[2],
                        DataKind = dataItem[3],
                        MByte = dataItem[4],
                        M_Mbit = dataItem[5],
                        M_Lbit = dataItem[6],
                        LByte = dataItem[7],
                        L_Mbit = dataItem[8],
                        L_Lbit = dataItem[9],
                        Type = dataItem[10],
                        Remark = dataItem[11]
                    });
                }
            }
            LoadFinish?.Invoke(null, e);
        }

        public object Clone()
        {
            return Extensions.Clone(this);
        }
    }
}
