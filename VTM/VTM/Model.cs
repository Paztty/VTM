using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Threading;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace VTM
{
    public class Model : ICloneable
    {
        public ObservableCollection<Step> Steps = new ObservableCollection<Step>();

        public string Name { get; set; }
        public string Path { get; set; }

        // PCB layout 
        public class Contruction
        {
            public int PCB_Count { get; set; } = 1;
            public int PCB_X_axis_Count { get; set; } = 1;
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
            public ArrayPositions ArrayPosition = ArrayPositions.HorizontalTopLeft;
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
            public string CMD { get; set; }
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
            public string ValueGet5data { get; set; }
            public string ValueGet6data { get; set; }
            public string ValueGet7data { get; set; }
            public string ValueGet8data { get; set; }
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
            public string ValueGet5
            {
                get { return ValueGet5data; }
                set
                {
                    if (value != this.ValueGet5data)
                    {
                        this.ValueGet5data = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public string ValueGet6
            {
                get { return ValueGet6data; }
                set
                {
                    if (value != this.ValueGet6data)
                    {
                        this.ValueGet6data = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public string ValueGet7
            {
                get { return ValueGet7data; }
                set
                {
                    if (value != this.ValueGet7data)
                    {
                        this.ValueGet7data = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public string ValueGet8
            {
                get { return ValueGet8data; }
                set
                {
                    if (value != this.ValueGet8data)
                    {
                        this.ValueGet8data = value;
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

        //Barcode settup
        public class Barcode
        {
            public int No { get; set; }
            public string BarcodeData { get; set; }
            public int Lenght { get; set; }
            public int StartModelCodePosition { get; set; }
            public string ModelCode { get; set; }
        }




        public Model() { }

        public event EventHandler LoadFinish;
        public void Load()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.DefaultExt = "model";
            openFile.FileOk += OpenFile_FileOk;
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
            return SystemExtension.Clone<Model>(this);
        }

        #region model run program test 

        public bool IsTestting = false;
        public int StepTesting { get; set; } = 0;
        public int StepTestCallInterval = 5;
        public event EventHandler StepTestChange;
        public event EventHandler TestRunFinish;

        public void runTest()
        {
            IsTestting = true;
            var rand = new Random(100);
            for (int i = 0; i < Steps.Count; i++)
            {
                StepTestChange?.Invoke(i, null);
                StepTesting = i;
                Console.WriteLine(StepTesting + ":" + Steps[StepTesting].CMD + " " + Steps[StepTesting].Min_Max);
                Steps[StepTesting].ValueGet1 = "exe";
                Steps[StepTesting].ValueGet2 = "exe";
                Steps[StepTesting].ValueGet3 = "exe";
                Steps[StepTesting].ValueGet4 = "exe";
                Steps[StepTesting].ValueGet5 = "exe";
                Steps[StepTesting].ValueGet6 = "exe";
                Steps[StepTesting].ValueGet7 = "exe";
                Steps[StepTesting].ValueGet8 = "exe";
                Thread.Sleep(rand.Next(200));
            }
            IsTestting = false;
            TestRunFinish?.Invoke("Good", null);
        }
        #endregion

    }


    public static class SystemExtension
    {
        public static T Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}
