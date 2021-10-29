using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.IO;
using System.ComponentModel;
using System.Windows.Media;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Win32;
using HVT.Utility;

namespace HVT.VTM.Base
{
    public partial class Model : ICloneable
    {
        public string Name { get; set; }
        public string Path { get; set; }

        // PCB 
        public class Contruction
        {
            public Grid PCBlayout;
            public List<Label> PCB_label = new List<Label>();

            public Grid PCB_result_layout;
            public class PCBResultPanel
            {
                public Label Barcode_label = new Label();
                public Label Result_label = new Label();
                public Label PCB_label = new Label();
            }
            public List<PCBResultPanel> resultPanels = new List<PCBResultPanel>();

            public int PCB_Count { get; set; } = 4;
            public int PCB_X_axis_Count { get; set; } = 2;
            public enum ArrayPositions
            {
                HorizontalTopLeft = 0,
                HorizontalTopRight = 1,
                HorizontalBottomLeft = 2,
                HorizontalBottomRight = 3,
                VerticalTopLeft = 4,
                VerticalTopRight = 5,
                VerticalBottomLeft = 6,
                VerticalBottomRight = 7,
            };
            public ArrayPositions ArrayPosition { get; set; } = ArrayPositions.HorizontalTopLeft;

            /// <summary>
            /// Check PCB layout element are enought for display or not
            /// </summary>
            /// <returns></returns>
            private bool CheckElement()
            {
                bool Enounght = PCBlayout != null;
                Enounght = PCB_result_layout != null;
                foreach (var item in PCB_label)
                {
                    Enounght = item != null;
                }
                foreach (var item in resultPanels)
                {
                    Enounght = item.PCB_label != null;
                    Enounght = item.Barcode_label != null;
                    Enounght = item.Result_label != null;
                }
                return Enounght;
            }

            /// <summary>
            /// Alight PCB in model page
            /// </summary>
            public void Align_PCB()
            {
                if (PCBlayout != null && PCB_label.Count >= PCB_Count)
                {

                    for (int i = 0; i < 8; i++)
                    {
                        PCBlayout.ColumnDefinitions[i].Width = new GridLength(0, GridUnitType.Star);
                        PCBlayout.RowDefinitions[i].Height = new GridLength(0, GridUnitType.Star);
                    }

                    int colunm = 0;
                    int row = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        if (i < PCB_Count)
                        {
                            PCB_label[i].Visibility = Visibility.Visible;
                            switch (ArrayPosition)
                            {
                                case ArrayPositions.HorizontalTopLeft:
                                    colunm = i % PCB_X_axis_Count;
                                    row = i / PCB_X_axis_Count;
                                    break;
                                case ArrayPositions.HorizontalTopRight:
                                    colunm = PCB_X_axis_Count - i % PCB_X_axis_Count - 1;
                                    row = i / PCB_X_axis_Count;
                                    break;
                                case ArrayPositions.HorizontalBottomLeft:
                                    colunm = i % PCB_X_axis_Count;
                                    row = (PCB_Count / PCB_X_axis_Count) - i / PCB_X_axis_Count;
                                    break;
                                case ArrayPositions.HorizontalBottomRight:
                                    colunm = PCB_X_axis_Count - i % PCB_X_axis_Count - 1;
                                    row = (PCB_Count / PCB_X_axis_Count) - i / PCB_X_axis_Count;
                                    break;

                                case ArrayPositions.VerticalTopLeft:
                                    colunm = i / PCB_X_axis_Count;
                                    row = i % PCB_X_axis_Count;
                                    break;
                                case ArrayPositions.VerticalTopRight:
                                    colunm = (PCB_Count / PCB_X_axis_Count) - i / PCB_X_axis_Count;
                                    row = i % PCB_X_axis_Count;
                                    break;
                                case ArrayPositions.VerticalBottomLeft:
                                    colunm = i / PCB_X_axis_Count;
                                    row = PCB_X_axis_Count - i % PCB_X_axis_Count - 1;
                                    break;
                                case ArrayPositions.VerticalBottomRight:
                                    colunm = (PCB_Count / PCB_X_axis_Count) - i / PCB_X_axis_Count;
                                    row = PCB_X_axis_Count - i % PCB_X_axis_Count - 1;
                                    break;
                                default:
                                    break;
                            }

                            PCBlayout.ColumnDefinitions[colunm].Width = new GridLength(1, GridUnitType.Star);
                            PCBlayout.RowDefinitions[row].Height = new GridLength(1, GridUnitType.Star);

                            Grid.SetColumn(PCB_label[i], colunm);
                            Grid.SetRow(PCB_label[i], row);
                        }
                        else
                        {
                            PCB_label[i].Visibility = Visibility.Hidden;
                        }
                    }
                }
            }

            /// <summary>
            /// Show resutl panel with pcb position and barcode 
            /// </summary>
            public void ShowResult()
            {

            }
        }
        public Contruction contruction { get; set; } = new Contruction();


        // Error Positions set
        public class ErrorPosition
        {
            public readonly System.Windows.Controls.Label label = new System.Windows.Controls.Label()
            {
                Foreground = new SolidColorBrush(Colors.White),
            };

            public readonly System.Windows.Controls.Label rect = new System.Windows.Controls.Label()
            {
                Background = new SolidColorBrush(Color.FromArgb(80, 255, 0, 0)),
                Foreground = new SolidColorBrush(Colors.White),
            };

            private int no;
            public int No
            {
                get { return no; }
                set
                {
                    no = value;
                    label.Content = value.ToString();
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
        public class Step : INotifyPropertyChanged
        {
            public int No { get; set; }
            public string IMQSCode { get; set; }
            public string TestContent { get; set; }
            public string CMD {
                get { return cmd.ToString(); }
                set {
                    CMDs ds = CMDs.NON;
                    if (Enum.TryParse<CMDs>(value, out ds))
                    {
                        cmd = ds;
                    }
                }
            }
            public CMDs cmd {
                get;
                set;
            }

            public CMDs SetCMD
            {
                get { return cmd; }
                set {
                    if (cmd != value)
                    {
                        cmd = value;
                        NotifyPropertyChanged("CMD");
                    }
                
                }
            }

            public string Condition1 { get; set; }
            public string Oper { get; set; }
            public string Condition2 { get; set; }
            public string Spect { get; set; }
            public string Min_Max { get; set; }
            public string Min { get; set; }
            public string Max { get; set; }
            public string ValueGet1data { get; set; }
            public string ValueGet2data { get; set; }
            public string ValueGet3data { get; set; }
            public string ValueGet4data { get; set; }
            public string Result1data { get; set; }
            public string Result2data { get; set; }
            public string Result3data { get; set; }
            public string Result4data { get; set; }
            public string Mode { get; set; }
            public int Count { get; set; }
            public string Mem { get; set; }
            public int E_Jump { get; set; }
            public string Remark { get; set; }
            public string ELoc { get; set; }
            public bool Skipdata { get; set; }
            public bool Result { get; set; }

            public string ValueGet1
            {
                get { return ValueGet1data; }
                set
                {
                    if (value != this.ValueGet1data)
                    {
                        this.ValueGet1data = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public string ValueGet2
            {
                get { return ValueGet2data; }
                set
                {
                    if (value != this.ValueGet2data)
                    {
                        this.ValueGet2data = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public string ValueGet3
            {
                get { return ValueGet3data; }
                set
                {
                    if (value != this.ValueGet3data)
                    {
                        this.ValueGet3data = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public string ValueGet4
            {
                get { return ValueGet4data; }
                set
                {
                    if (value != this.ValueGet4data)
                    {
                        this.ValueGet4data = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public string Result1
            {
                get { return Result1data; }
                set
                {
                    if (value != this.Result1data)
                    {
                        this.Result1data = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public string Result2
            {
                get { return Result2data; }
                set
                {
                    if (value != this.Result2data)
                    {
                        this.Result2data = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public string Result3
            {
                get { return Result3data; }
                set
                {
                    if (value != this.Result3data)
                    {
                        this.Result3data = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public string Result4
            {
                get { return Result4data; }
                set
                {
                    if (value != this.Result4data)
                    {
                        this.Result4data = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public bool Skip
            {
                get { return Skipdata; }
                set
                {
                    if (value != this.Skipdata)
                    {
                        this.Skipdata = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            public Step() { }
        }
        public ObservableCollection<Step> Steps = new ObservableCollection<Step>();

        //Barcode settup
        public class Barcode
        {
            public int No { get; set; }
            public string BarcodeData { get; set; }
            public int Lenght { get; set; }
            public int StartModelCodePosition { get; set; }
            public string ModelCode { get; set; }
        }
        public ObservableCollection<Barcode> Barcodes = new ObservableCollection<Barcode>();
        //Serial naming
        public Naming SerialNaming = new Naming();

        //Event
        public event EventHandler LoadFinish;
        public event EventHandler StepTestChange;
        public event EventHandler TestRunFinish;
        public event EventHandler StateChange;

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
                    step.Count = Int32.TryParse(datas[9], out _) == true ? Convert.ToInt32(datas[9]) : 0;
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

        #region model run program test 

        public bool IsTestting = false;
        public int StepTesting { get; set; } = 0;
        public int StepTestCallInterval = 5;

        public void RunTest()
        {
            IsTestting = true;
            var rand = new Random(100);
            StepTesting = 0;
            while (true)
            {
                switch (TestState)
                {
                    case RunTestState.WAIT:
                        TestState = RunTestState.READY;
                        break;
                    case RunTestState.TESTTING:
                        if (StepTesting == Steps.Count)
                        {
                            TestState = RunTestState.GOOD;
                        }
                        else
                        {
                            IsTestting = true;
                            StepTestChange?.Invoke(StepTesting, null);
                            Console.WriteLine(StepTesting + ":" + Steps[StepTesting].CMD + " " + Steps[StepTesting].Min_Max);
                            Steps[StepTesting].ValueGet1 = "exe";
                            Steps[StepTesting].ValueGet2 = "exe";
                            Steps[StepTesting].ValueGet3 = "exe";
                            Steps[StepTesting].ValueGet4 = "exe";
                            Steps[StepTesting].Result1 = "exe";
                            Steps[StepTesting].Result2 = "exe";
                            Steps[StepTesting].Result3 = "exe";
                            Steps[StepTesting].Result4 = "exe";
                            if (Steps[StepTesting].Min_Max == "~")
                            {

                            }
                            else
                                Thread.Sleep(rand.Next(500));
                            StepTesting++;
                            if (StepTesting == Steps.Count)
                            {
                                TestState = RunTestState.GOOD;
                            }
                        }
                        break;
                    case RunTestState.Pause:
                        break;
                    case RunTestState.STOP:
                        IsTestting = false;
                        StepTesting = 0;
                        StepTestChange?.Invoke(StepTesting, null);
                        TestState = RunTestState.WAIT;
                        break;
                    case RunTestState.GOOD:
                        IsTestting = false;
                        TestRunFinish?.Invoke("Good", null);
                        Thread.Sleep(rand.Next(5000));
                        TestState = RunTestState.READY;
                        break;
                    case RunTestState.FAIL:
                        IsTestting = false;
                        TestRunFinish?.Invoke("Fail", null);
                        Thread.Sleep(rand.Next(5000));
                        TestState = RunTestState.WAIT;
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

    }
}
