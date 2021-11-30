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

        // PCB 
        public Contruction contruction { get; set; } = new Contruction();


        // Error Positions set
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

        // Step test 
        public ObservableCollection<Step> Steps { get; set; } = new ObservableCollection<Step>();


        //Barcode settup
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
        public ObservableCollection<string> BarcodesWaitting = new ObservableCollection<string>() { "", "", "", "" };
        //Serial naming
        public Naming Naming { get; set; }


        //Vision test 

        private List<VisionFunctions.FND> fNDs;
        private List<VisionFunctions.LCD> lCDs;
        private List<VisionFunctions.GLED> gLEDs;
        private List<VisionFunctions.LED> lEDs;

        public List<VisionFunctions.FND> FNDs { get; set ; }
        public List<VisionFunctions.LCD> LCDs { get; set; }
        public List<VisionFunctions.GLED> GLEDs { get; set; }
        public List<VisionFunctions.LED> LEDs { get; set; }

        public void VisionTestInit(Canvas DrawingCanvas, Canvas DisplayFunction)
        {
             FNDs = new List<Base.VisionFunctions.FND>()
                    {
                    new Base.VisionFunctions.FND(1, "FND A", DrawingCanvas, DisplayFunction),
                    new Base.VisionFunctions.FND(2, "FND B", DrawingCanvas, DisplayFunction),
                    new Base.VisionFunctions.FND(3, "FND C", DrawingCanvas, DisplayFunction),
                    new Base.VisionFunctions.FND(4, "FND D", DrawingCanvas, DisplayFunction),
                    };

             LCDs = new List<Base.VisionFunctions.LCD>()
                    {
                    new Base.VisionFunctions.LCD(1, "LCD A", DrawingCanvas, DisplayFunction),
                    new Base.VisionFunctions.LCD(2, "LCD B", DrawingCanvas, DisplayFunction),
                    new Base.VisionFunctions.LCD(3, "LCD C", DrawingCanvas, DisplayFunction),
                    new Base.VisionFunctions.LCD(4, "LCD D", DrawingCanvas, DisplayFunction),
                    };

             GLEDs = new List<Base.VisionFunctions.GLED>()
            {
                new Base.VisionFunctions.GLED(DisplayFunction, DrawingCanvas, 0),
                new Base.VisionFunctions.GLED(DisplayFunction, DrawingCanvas, 1),
                new Base.VisionFunctions.GLED(DisplayFunction, DrawingCanvas, 2),
                new Base.VisionFunctions.GLED(DisplayFunction, DrawingCanvas, 3),
            };

             LEDs = new List<Base.VisionFunctions.LED>()
            {
                new Base.VisionFunctions.LED(DisplayFunction,DrawingCanvas, 0),
                new Base.VisionFunctions.LED(DisplayFunction,DrawingCanvas, 1),
                new Base.VisionFunctions.LED(DisplayFunction,DrawingCanvas, 2),
                new Base.VisionFunctions.LED(DisplayFunction,DrawingCanvas, 3),
            };


            foreach (var item in  FNDs)
            {
                item.PlaceIn(DrawingCanvas, DisplayFunction);
            }
            foreach (var item in  LCDs)
            {
                item.PlaceIn(DrawingCanvas, DisplayFunction);
            }
            foreach (var item in  GLEDs)
            {
                foreach (var led in item.GLEDs)
                {
                    led.PlaceIn(DrawingCanvas, DisplayFunction);
                }
            }
            foreach (var item in  LEDs)
            {
                foreach (var led in item.LEDs)
                {
                    led.PlaceIn(DrawingCanvas, DisplayFunction);
                }
            }

        }

        public void ReplaceComponent(Canvas DrawingCanvas, Canvas DisplayFunction)
        {
            DrawingCanvas.Children.Clear();
            DisplayFunction.Children.Clear();

            foreach (var item in FNDs)
            {
                item.ReInit(DrawingCanvas, DisplayFunction);
                item.PlaceIn(DrawingCanvas, DisplayFunction);
                
            }
            foreach (var item in LCDs)
            {
                item.ReInit(DrawingCanvas, DisplayFunction);
                item.PlaceIn(DrawingCanvas, DisplayFunction);
            }
            foreach (var item in GLEDs)
            {
                foreach (var led in item.GLEDs)
                {

                    led.ReInit(DrawingCanvas, DisplayFunction);
                    led.PlaceIn(DrawingCanvas, DisplayFunction);
                }
            }
            foreach (var item in LEDs)
            {
                foreach (var led in item.LEDs)
                {

                    led.ReInit(DrawingCanvas, DisplayFunction);
                    led.PlaceIn(DrawingCanvas, DisplayFunction);
                }
            }

        }

        //Event
        public event EventHandler LoadFinish;

        public void LoadFinishEvent()
        {
            LoadFinish?.Invoke(null, null);
        }

        public Model() { }

        public void Init()
        {

        }

        public void Load()
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                DefaultExt = ".model",
                Title = "Open model",
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
            }
            LoadFinish?.Invoke(null, e);
        }

        public object Clone()
        {
            return Extensions.Clone(this);
        }

    }
}
