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
        //Event
        public event EventHandler LoadFinish;
        public event EventHandler StepTestChange;
        public event EventHandler TestRunFinish;
        public event EventHandler StateChange;

        public void LoadFinishEvent()
        {
            this.TestState = RunTestState.READY;
            LoadFinish?.Invoke(null, null);
        }

        public Model() { }

        public void Init()
        {

        }

        public enum RunTestState
        {
            WAIT = 0,
            TESTTING = 1,
            Pause = 2,
            STOP = 3,
            GOOD = 4,
            FAIL = 5,
            BUSY = 6,
            READY = 7,
        }
        private RunTestState testState;
        public RunTestState TestState
        {
            get { return testState; }
            set
            {
                if (value != testState)
                {
                    testState = value;
                    StateChange?.Invoke(value, null);
                }
            }
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
