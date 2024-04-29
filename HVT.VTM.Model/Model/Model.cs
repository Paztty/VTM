using Camera;
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
using HVT.Controls;

namespace HVT.VTM.Base
{
    public partial class Model : ICloneable
    {

        public string Name { get; set; }
        public string Path { get; set; }

        #region PCB layout
        private PBA_Layout layout = new PBA_Layout();
        public PBA_Layout Layout
        {
            get {
                return layout;
            }
            set
            {
                layout = value;
                layout.PCB_COUNT_CHANGE += Layout_PCB_COUNT_CHANGE;
                Layout_PCB_COUNT_CHANGE(layout.PCB_Count, null);
            }
        }
        #endregion

        #region Discharge
        public DischargeOption Discharge { get; set; } = new DischargeOption();
        #endregion
        //#region Error Positions set
        //public class ErrorPosition
        //{
        //    public readonly System.Windows.Controls.Label rect = new System.Windows.Controls.Label()
        //    {
        //        Background = new SolidColorBrush(Color.FromArgb(0, 255, 0, 0)),
        //        Foreground = new SolidColorBrush(Colors.White),
        //        BorderBrush = new SolidColorBrush(Colors.Red),
        //        BorderThickness = new Thickness(1),
        //        Cursor = Cursors.Hand,
        //    };

        //    private int no;
        //    public int No
        //    {
        //        get { return no; }
        //        set
        //        {
        //            no = value;
        //            rect.Content = value;
        //        }
        //    }

        //    public double X { get; set; }
        //    public double Y { get; set; }

        //    public double lbTop { get; set; }
        //    public double lbLeft { get; set; }

        //    private double width;
        //    public double Width
        //    {
        //        get { return width; }
        //        set
        //        {
        //            width = value;
        //            rect.Width = Math.Abs(value);
        //        }
        //    }
        //    private double height;
        //    public double Height
        //    {
        //        get { return height; }
        //        set
        //        {
        //            height = value;
        //            rect.Height = Math.Abs(value);
        //        }
        //    }
        //    public string Position { get; set; }
        //}
        //public List<ErrorPosition> ErrorPositions { get; set; } = new List<ErrorPosition>();
        //#endregion

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

                step.Result = true;
            }
        }
        #endregion

        #region Barcode settup
        private BarcodeOption barcodeOption = new BarcodeOption();
        public BarcodeOption BarcodeOption {
            get { return barcodeOption; }
            set {
                if (value != barcodeOption && value != null)
                {
                    barcodeOption = value;
                }
            }
        }
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

        private ObservableCollection<SegementCharacter> _ModelSegmentLookup = new ObservableCollection<SegementCharacter>();
        public ObservableCollection<SegementCharacter>  ModelSegmentLookup
        {
            get { return _ModelSegmentLookup; }
            set
            {
                if (value != null && value != _ModelSegmentLookup)
                {
                    _ModelSegmentLookup = value;
                }
            }
        }
        private VisionModel _VisionModels = new VisionModel();
        public VisionModel VisionModels
        {
            get { return _VisionModels; }
            set
            {
                if (value != null) _VisionModels = value;
            }
        }
        //public List<VisionFunctions.FND> FNDs { get; set; }
        //public List<VisionFunctions.LCD> LCDs { get; set; }
        //public List<VisionFunctions.GLED> GLEDs { get; set; }
        //public List<VisionFunctions.LED> LEDs { get; set; }

        //public void VisionTestInit(
        //    Canvas DrawingCanvas,
        //    Canvas DisplayFunction,
        //    Canvas ManualDisplayCanvas,
        //                        DockPanel LCDA,
        //                        DockPanel LCDB,
        //                        DockPanel LCDC,
        //                        DockPanel LCDD,

        //                        DockPanel FNDA,
        //                        DockPanel FNDB,
        //                        DockPanel FNDC,
        //                        DockPanel FNDD
        //    )
        //{
        //    FNDs = new List<Base.VisionFunctions.FND>()
        //            {
        //            new Base.VisionFunctions.FND(1, "FND A", DrawingCanvas, DisplayFunction,ManualDisplayCanvas),
        //            new Base.VisionFunctions.FND(2, "FND B", DrawingCanvas, DisplayFunction,ManualDisplayCanvas),
        //            new Base.VisionFunctions.FND(3, "FND C", DrawingCanvas, DisplayFunction,ManualDisplayCanvas),
        //            new Base.VisionFunctions.FND(4, "FND D", DrawingCanvas, DisplayFunction,ManualDisplayCanvas),
        //            };
        //    FNDA.Children.Add(FNDs[0].Image);
        //    FNDB.Children.Add(FNDs[1].Image);
        //    FNDC.Children.Add(FNDs[2].Image);
        //    FNDD.Children.Add(FNDs[3].Image);

        //    FNDA.Children.Add(FNDs[0].LabelContent);
        //    FNDB.Children.Add(FNDs[1].LabelContent);
        //    FNDC.Children.Add(FNDs[2].LabelContent);
        //    FNDD.Children.Add(FNDs[3].LabelContent);

        //    LCDs = new List<Base.VisionFunctions.LCD>()
        //            {
        //            new Base.VisionFunctions.LCD(1, "LCD A", DrawingCanvas, DisplayFunction,ManualDisplayCanvas),
        //            new Base.VisionFunctions.LCD(2, "LCD B", DrawingCanvas, DisplayFunction,ManualDisplayCanvas),
        //            new Base.VisionFunctions.LCD(3, "LCD C", DrawingCanvas, DisplayFunction,ManualDisplayCanvas),
        //            new Base.VisionFunctions.LCD(4, "LCD D", DrawingCanvas, DisplayFunction,ManualDisplayCanvas),
        //            };
        //    LCDA.Children.Add(LCDs[0].Image);
        //    LCDA.Children.Add(LCDs[0].LabelContent);
        //    LCDB.Children.Add(LCDs[1].Image);
        //    LCDB.Children.Add(LCDs[1].LabelContent);
        //    LCDC.Children.Add(LCDs[2].Image);
        //    LCDC.Children.Add(LCDs[2].LabelContent);
        //    LCDD.Children.Add(LCDs[3].Image);
        //    LCDD.Children.Add(LCDs[3].LabelContent);

        //    GLEDs = new List<Base.VisionFunctions.GLED>()
        //    {
        //        new Base.VisionFunctions.GLED(ManualDisplayCanvas, DisplayFunction, DrawingCanvas, 0),
        //        new Base.VisionFunctions.GLED(ManualDisplayCanvas, DisplayFunction, DrawingCanvas, 1),
        //        new Base.VisionFunctions.GLED(ManualDisplayCanvas, DisplayFunction, DrawingCanvas, 2),
        //        new Base.VisionFunctions.GLED(ManualDisplayCanvas, DisplayFunction, DrawingCanvas, 3),
        //    };

        //    LEDs = new List<Base.VisionFunctions.LED>()
        //    {
        //        new Base.VisionFunctions.LED(ManualDisplayCanvas, DisplayFunction, DrawingCanvas, 0),
        //        new Base.VisionFunctions.LED(ManualDisplayCanvas, DisplayFunction, DrawingCanvas, 1),
        //        new Base.VisionFunctions.LED(ManualDisplayCanvas, DisplayFunction, DrawingCanvas, 2),
        //        new Base.VisionFunctions.LED(ManualDisplayCanvas, DisplayFunction, DrawingCanvas, 3),
        //    };


        //    foreach (var item in FNDs)
        //    {
        //        item.PlaceIn(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
        //    }
        //    foreach (var item in LCDs)
        //    {
        //        item.PlaceIn(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
        //    }
        //    foreach (var item in GLEDs)
        //    {
        //        foreach (var led in item.GLEDs)
        //        {
        //            led.PlaceIn(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
        //        }
        //    }
        //    foreach (var item in LEDs)
        //    {
        //        foreach (var led in item.LEDs)
        //        {
        //            led.PlaceIn(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
        //        }
        //    }

        //}

        //public void ReplaceComponent(Canvas DrawingCanvas,
        //    Canvas DisplayFunction,
        //    Canvas ManualDisplayCanvas,
        //                        DockPanel LCDA,
        //                        DockPanel LCDB,
        //                        DockPanel LCDC,
        //                        DockPanel LCDD,

        //                        DockPanel FNDA,
        //                        DockPanel FNDB,
        //                        DockPanel FNDC,
        //                        DockPanel FNDD
        //    )
        //{
        //    DrawingCanvas.Children.Clear();
        //    DisplayFunction.Children.Clear();
        //    ManualDisplayCanvas.Children.Clear();

        //    foreach (var item in FNDs)
        //    {
        //        item.ReInit(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
        //        item.PlaceIn(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
        //    }
        //    foreach (var item in LCDs)
        //    {
        //        item.ReInit(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
        //        item.PlaceIn(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
        //    }
        //    FNDA.Children.Clear();
        //    FNDB.Children.Clear();
        //    FNDC.Children.Clear();
        //    FNDD.Children.Clear();

        //    FNDA.Children.Add(FNDs[0].Image);
        //    FNDB.Children.Add(FNDs[1].Image);
        //    FNDC.Children.Add(FNDs[2].Image);
        //    FNDD.Children.Add(FNDs[3].Image);

        //    FNDA.Children.Add(FNDs[0].LabelContent);
        //    FNDB.Children.Add(FNDs[1].LabelContent);
        //    FNDC.Children.Add(FNDs[2].LabelContent);
        //    FNDD.Children.Add(FNDs[3].LabelContent);


        //    LCDA.Children.Clear();
        //    LCDB.Children.Clear();
        //    LCDC.Children.Clear();
        //    LCDD.Children.Clear();

        //    LCDA.Children.Add(LCDs[0].Image);
        //    LCDA.Children.Add(LCDs[0].LabelContent);
        //    LCDB.Children.Add(LCDs[1].Image);
        //    LCDB.Children.Add(LCDs[1].LabelContent);
        //    LCDC.Children.Add(LCDs[2].Image);
        //    LCDC.Children.Add(LCDs[2].LabelContent);
        //    LCDD.Children.Add(LCDs[3].Image);
        //    LCDD.Children.Add(LCDs[3].LabelContent);


        //    foreach (var item in GLEDs)
        //    {
        //        foreach (var led in item.GLEDs)
        //        {

        //            led.ReInit(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
        //            led.PlaceIn(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
        //        }
        //    }
        //    foreach (var item in LEDs)
        //    {
        //        foreach (var led in item.LEDs)
        //        {

        //            led.ReInit(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
        //            led.PlaceIn(DrawingCanvas, DisplayFunction, ManualDisplayCanvas);
        //        }
        //    }

        //}

        #endregion

        #region Machine Card
        public HVT.Controls.MuxCard MuxCard { get; set; } = new Controls.MuxCard();
        public HVT.Controls.LevelCard LevelCard { get; set; } = new Controls.LevelCard();
        #endregion

        #region UUT config

        private HVT.Controls.UUT_Config p1_Config = new HVT.Controls.UUT_Config();
        private HVT.Controls.UUT_Config p2_Config = new HVT.Controls.UUT_Config();

        public HVT.Controls.UUT_Config P1_Config
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
        public HVT.Controls.UUT_Config P2_Config
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


        //public void LoadFinishEvent()
        //{
        //    LoadFinish?.Invoke(null, null);
        //}

        public Model() {
            Steps.Add(new Step()
            {
                No = 1,
            });
            Layout.PCB_COUNT_CHANGE += Layout_PCB_COUNT_CHANGE;
        }

        private void Layout_PCB_COUNT_CHANGE(object sender, EventArgs e)
        {
            MuxCard.PCB_Count = Layout.PCB_Count;
            LevelCard.PCB_Count = Layout.PCB_Count;
            VisionModels.UpdateLayout(Layout.PCB_Count);
        }

        public void Load()
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                Title = "Import from FPT model",
            };
            openFile.Filter = "FPT model files (*.fdmdl)|*.fdmdl";
            openFile.RestoreDirectory = true;
            if ((bool)openFile.ShowDialog() == true)
            {
                var fileInfor = new FileInfo(openFile.FileName);
                Name = System.IO.Path.GetFileNameWithoutExtension(openFile.FileName);
                //Support.WriteLine("Load model : " + Name + Environment.NewLine + Path);
                string[] lines = System.IO.File.ReadAllLines(openFile.FileName);

                bool IsFPTfile = false;
                foreach (string line in lines)
                {
                    if (line.Contains("TEST STEP="))
                    {
                        Steps.Clear();
                        Naming.TxDatas = new ObservableCollection<TxData>();
                        Naming.RxDatas = new ObservableCollection<RxData>();
                        Naming.QRDatas = new ObservableCollection<QRData>();
                        IsFPTfile = true;
                    }
                }

                if (!IsFPTfile)
                {
                    MessageBox.Show("File not in FPT model format!", "Exception Import", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

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
                        Layout.PCB_Count = int.Parse(data);
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
            }
        }

        public object Clone()
        {
            return Extensions.Clone(this);
        }
    }
}
