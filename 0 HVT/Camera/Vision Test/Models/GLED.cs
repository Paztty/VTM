using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;
using Rect = System.Windows.Rect;

namespace Camera
{
    public class GLED : INotifyPropertyChanged
    {
        private Int32 _calculatorOutput;
        public Int32 CalculatorOutput
        {
            get { return _calculatorOutput; }
            set
            {
                _calculatorOutput = value;
                CalculatorOutputString = value.ToString("X");
            }
        }
        private string _calculatorOutputString;
        public String CalculatorOutputString
        {
            get { return _calculatorOutputString; }
            set
            {
                _calculatorOutputString = value;
                NotifyPropertyChanged(nameof(CalculatorOutputString));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private Visibility visibility;
        public Visibility Visibility
        {
            get { return visibility; }
            set
            {
                visibility = value;
                foreach (SingleGLED gLED in GLEDs)
                {
                    gLED.Visibility  = gLED.Use ? value : Visibility.Hidden;

                }
            }
        }

        private bool _IsReadOnly;
        [JsonIgnore]
        public bool IsReadOnly
        {
            get { return _IsReadOnly; }
            set
            {
                if (value != _IsReadOnly) _IsReadOnly = value;
                foreach (var item in GLEDs)
                {
                    item.IsReadOnly = IsReadOnly;
                }
            }
        }

        public ObservableCollection<SingleGLED> GLEDs { get; set; } = new ObservableCollection<SingleGLED>();

        public GLED() { }

        public GLED(System.Windows.Point startLocation)
        {
            CalculatorOutputString = "";
            for (int i = 0; i < 32; i++)
            {
                GLEDs.Add(new SingleGLED(i, startLocation));
            }
        }

        public void CALC_THRESH()
        {
            foreach (SingleGLED gLED in GLEDs)
            {
                gLED.Thresh = (int)(gLED.ON - gLED.OFF) / 3 * 2 + gLED.OFF;
            }
        }

        public void GetValue(Mat mat)
        {
            string Value = "";
            foreach (SingleGLED gLED in GLEDs)
            {
                var output =gLED.Use ? gLED.TestImage(mat, true) : "0";
                Value = output.ToString() + Value;
            }
            CalculatorOutput = Convert.ToInt32(Value, 2);
        }

        public void GetValue()
        {
            string Value = "";
            foreach (SingleGLED gLED in GLEDs)
            {
                var output = gLED.IsPass ? "1" : "0";
                Value = output + Value;
            }
            CalculatorOutput = Convert.ToInt32(Value, 2);
        }

        public void SetParentCanvas(Canvas ParentCanvas)
        {
            foreach (var item in GLEDs)
            {
                item.SetParentCanvas(ParentCanvas);
                if (!item.Use)
                {
                    item.Visibility = Visibility.Hidden;
                }
            }
        }
    }

    public class SingleGLED : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event EventHandler Sellected;
        private void OnSelected()
        {
            Sellected?.Invoke(this, null);
        }


        public int Index { get; set; }

        public int ON { get; set; } = 250;
        public int OFF { get; set; } = 10;


        private double _x;
        public double X
        {
            get { return _x; }
            set
            {
                if (value != _x)
                {
                    _x = value;
                    Rect = new Rect(value, Rect.Y, Rect.Width, Rect.Height);
                    SetPosition();
                }
                OnPropertyChanged("X");
            }
        }
        private double _y;
        public double Y
        {
            get { return _y; }
            set
            {
                if (value != _y)
                {
                    _y = value;
                    Rect = new Rect(Rect.X, value, Rect.Width, Rect.Height);
                    SetPosition();
                }
                OnPropertyChanged("Y");
            }
        }
        private double _w;
        public double Width
        {
            get { return _w; }
            set
            {
                if (value != _w)
                {
                    _w = value;
                    Rect = new Rect(Rect.X, Rect.Y, value, Rect.Height);
                    SetPosition();
                }
                OnPropertyChanged("Width");
            }
        }
        private double _h;
        public double Height
        {
            get { return _h; }
            set
            {
                if (value != _h)
                {
                    _h = value;
                    Rect = new Rect(Rect.X, Rect.Y, Rect.Width, value);
                    SetPosition();
                }
                OnPropertyChanged("Height");
            }
        }

        private int thresh = 180;
        public int Thresh
        {
            get { return thresh; }
            set
            {
                thresh = value;
                OnPropertyChanged(nameof(Thresh));
            }
        }

        private bool use = false;
        public bool Use
        {
            get { return use; }
            set
            {
                if (use != value)
                {
                    use = value;
                    OnPropertyChanged(nameof(Use));
                    if (use)
                    {
                        Visibility = Visibility.Visible;
                        this.Label.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            Label.BorderBrush = new SolidColorBrush(Colors.Red);
                            Label.Foreground = new SolidColorBrush(Colors.Red);
                        }));
                    }
                    else
                    {
                        Visibility = Visibility.Hidden;
                        Label.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            Label.BorderBrush = new SolidColorBrush(Colors.Yellow);
                            Label.Foreground = new SolidColorBrush(Colors.Yellow);
                        }));
                    }
                }
            }

        }


        private int intens = 180;

        public int Intens
        {
            get { return intens; }
            set
            {
                intens = value;
                OnPropertyChanged("Intens");
                if (use)
                {
                    if (value >= Thresh)
                    {
                        Label.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            Label.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                            Label.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                        }));
                    }
                    else
                    {
                        Label.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            Label.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                            Label.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        }));
                    }
                }
            }
        }

        private Mat mat { get; set; } = new Mat();


        private string _SpectString = "8888";
        public string SpectString
        {
            get { return _SpectString; }
            set
            {
                if (value != null || value != _SpectString)
                    _SpectString = value;
                OnPropertyChanged("SpectString");
            }
        }

        private string _DetectedString;
        public string DetectedString
        {
            get { return _DetectedString; }
            set
            {
                if (value != null || value != _DetectedString)
                {
                    _DetectedString = value;
                    OnPropertyChanged("DetectedString");
                    IsPass = DetectedString == SpectString;
                }
            }
        }


        private bool _IsPass;
        public bool IsPass
        {
            get { return _IsPass; }
            set
            {
                if (value != _IsPass)
                    _IsPass = value;
                OnPropertyChanged("IsPass");
                if (Use)
                    if (value)
                    {
                        Label.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            Label.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                            Label.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                        }));
                    }
                    else
                    {
                        Label.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            Label.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                            Label.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        }));
                    }
            }
        }

        private List<Models.SettingParram> _Param = new List<Models.SettingParram>();
        public List<Models.SettingParram> Param
        {
            get { return _Param; }
            set
            {
                if (value != null || value != _Param) _Param = value;
                OnPropertyChanged("Param");
            }
        }

        private int _NoiseSize = 5;
        public int NoiseSize
        {
            get { return _NoiseSize; }
            set
            {
                if (value != _NoiseSize && value > 5)
                {
                    _NoiseSize = value;
                    OnPropertyChanged("NoiseSize");
                }
            }
        }


        private double _Threshold = 100;
        public double Threshold
        {
            get { return _Threshold; }
            set
            {
                if (value != _Threshold) _Threshold = value;
                OnPropertyChanged("Threshold");
            }
        }


        private double _Blur;
        public double Blur
        {
            get { return _Blur; }
            set
            {
                if (value % 2 != 0 || value != _Blur) _Blur = value;
                OnPropertyChanged("Blur");
            }
        }

        private double _TurningProgress;
        public double TurningProgress
        {
            get { return Math.Round(_TurningProgress, 2); }
            set
            {
                if (value != _TurningProgress) _TurningProgress = value;
                OnPropertyChanged("TurningProgress");
            }
        }

        private BitmapSource _CropImage;
        [JsonIgnore]
        public BitmapSource CropImage
        {
            get { return _CropImage; }
            set
            {
                if (value != null || value != _CropImage) _CropImage = value;
                OnPropertyChanged("CropImage");
            }
        }

        private Visibility _Visibility;
        public Visibility Visibility
        {
            get { return _Visibility; }
            set
            {
                if (value != _Visibility) _Visibility = value;
                Label.Visibility = value;
                if (_Visibility != Visibility.Visible)
                {
                    LabelBotLeft.Visibility = value;
                    LabelBotMid.Visibility = value;
                    LabelBotRight.Visibility = value;
                    LabelMidLeft.Visibility = value;
                    LabelMidRight.Visibility = value;
                    LabelTopLeft.Visibility = value;
                    LabelTopMid.Visibility = value;
                    LabelTopRight.Visibility = value;
                }
                OnPropertyChanged("Visibility");
            }
        }


        private Canvas _ParentCanvas;
        private Canvas ParentCanvas
        {
            get { return _ParentCanvas; }
            set
            {
                _ParentCanvas = value;
                ParentCanvasSize = new System.Windows.Rect()
                {
                    X = 0,
                    Y = 0,
                    Width = value.ActualWidth,
                    Height = value.ActualHeight
                };
            }
        }

        private Rect _ParentCanvasSize;
        public Rect ParentCanvasSize
        {
            get { return _ParentCanvasSize; }
            set { _ParentCanvasSize = value; }
        }

        public Label Label = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(1, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.Red),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Focusable = true,
            Padding = new Thickness(1),
            Cursor = Cursors.SizeAll,
            VerticalContentAlignment = VerticalAlignment.Top,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            FontSize = 9
        };

        public Image CropImageHolder = new Image();

        public Label LabelTopLeft = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeNWSE,
            Focusable = true,
        };
        public Label LabelTopMid = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeNS,
            Focusable = true,
        };
        public Label LabelTopRight = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeNESW,
            Focusable = true,
        };
        public Label LabelMidLeft = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeWE,
            Focusable = true,
        };
        public Label LabelMidRight = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeWE,
            Focusable = true,
        };
        public Label LabelBotLeft = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeNESW,
            Focusable = true,
        };
        public Label LabelBotMid = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeNS,
            Focusable = true,
        };
        public Label LabelBotRight = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeNWSE,
            Focusable = true,
        };

        private Rect OfsetMove;

        public Rect rect = new Rect()
        {
            Location = new System.Windows.Point(5, 5),
            Size = new System.Windows.Size(10, 10)
        };
        public Rect Rect
        {
            get { return rect; }
            set
            {
                if (rect != value)
                {

                    if (value.X > 0 && value.X < ParentCanvasSize.Width - value.Width)
                    {

                        rect.X = value.X;
                        rect.Width = value.Width;
                        Label.Width = value.Width;

                        X = (int)value.X;
                        Width = (int)value.Width;
                    }

                    if (value.Y > 0 && value.Y < ParentCanvasSize.Height - value.Height)
                    {
                        Y = (int)value.Y;
                        Height = (int)value.Height;

                        rect.Y = value.Y;
                        rect.Height = value.Height;
                        Label.Height = value.Height;
                    }
                }
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    Label.Content = name;
                }
            }
        }


        private bool _IsReadOnly = false;
        public bool IsReadOnly
        {
            get { return _IsReadOnly; }
            set
            {
                if (value != _IsReadOnly) _IsReadOnly = value;
                if (IsReadOnly)
                {
                    Label.Cursor = Cursors.Arrow;

                    Label.GotKeyboardFocus -= Label_GotKeyboardFocus;
                    Label.LostKeyboardFocus -= Label_LostKeyboardFocus;

                    Label.KeyDown -= Label_KeyDown;

                    Label.MouseDown -= Label_MouseDown;
                    Label.MouseMove -= Label_MouseMove;
                    Label.MouseUp -= Label_MouseUp;

                    LabelBotLeft.Visibility = Visibility.Hidden;
                    LabelBotMid.Visibility = Visibility.Hidden;
                    LabelBotRight.Visibility = Visibility.Hidden;
                    LabelMidLeft.Visibility = Visibility.Hidden;
                    LabelMidRight.Visibility = Visibility.Hidden;
                    LabelTopLeft.Visibility = Visibility.Hidden;
                    LabelTopMid.Visibility = Visibility.Hidden;
                    LabelTopRight.Visibility = Visibility.Hidden;

                    LabelBotLeft.MouseMove -= LabelBotLeft_MouseMove;
                    LabelBotMid.MouseMove -= LabelBotMid_MouseMove;
                    LabelBotRight.MouseMove -= LabelBotRight_MouseMove;
                    LabelMidLeft.MouseMove -= LabelMidLeft_MouseMove;
                    LabelMidRight.MouseMove -= LabelMidRight_MouseMove;
                    LabelTopLeft.MouseMove -= LabelTopLeft_MouseMove;
                    LabelTopMid.MouseMove -= LabelTopMid_MouseMove;
                    LabelTopRight.MouseMove -= LabelTopRight_MouseMove;

                    LabelBotLeft.MouseDown -= LabelResize_MouseDown;
                    LabelBotMid.MouseDown -= LabelResize_MouseDown;
                    LabelBotRight.MouseDown -= LabelResize_MouseDown;
                    LabelMidLeft.MouseDown -= LabelResize_MouseDown;
                    LabelMidRight.MouseDown -= LabelResize_MouseDown;
                    LabelTopLeft.MouseDown -= LabelResize_MouseDown;
                    LabelTopMid.MouseDown -= LabelResize_MouseDown;
                    LabelTopRight.MouseDown -= LabelResize_MouseDown;

                    LabelBotLeft.LostKeyboardFocus -= Label_LostKeyboardFocus;
                    LabelBotMid.LostKeyboardFocus -= Label_LostKeyboardFocus;
                    LabelBotRight.LostKeyboardFocus -= Label_LostKeyboardFocus;
                    LabelMidLeft.LostKeyboardFocus -= Label_LostKeyboardFocus;
                    LabelMidRight.LostKeyboardFocus -= Label_LostKeyboardFocus;
                    LabelTopLeft.LostKeyboardFocus -= Label_LostKeyboardFocus;
                    LabelTopMid.LostKeyboardFocus -= Label_LostKeyboardFocus;
                    LabelTopRight.LostKeyboardFocus -= Label_LostKeyboardFocus;


                    LabelBotLeft.GotKeyboardFocus -= Label_GotKeyboardFocus;
                    LabelBotMid.GotKeyboardFocus -= Label_GotKeyboardFocus;
                    LabelBotRight.GotKeyboardFocus -= Label_GotKeyboardFocus;
                    LabelMidLeft.GotKeyboardFocus -= Label_GotKeyboardFocus;
                    LabelMidRight.GotKeyboardFocus -= Label_GotKeyboardFocus;
                    LabelTopLeft.GotKeyboardFocus -= Label_GotKeyboardFocus;
                    LabelTopMid.GotKeyboardFocus -= Label_GotKeyboardFocus;
                    LabelTopRight.GotKeyboardFocus -= Label_GotKeyboardFocus;
                }
            }
        }

        public SingleGLED()
        {
            Label.ToolTip = CropImageHolder;

            Label.GotKeyboardFocus += Label_GotKeyboardFocus;
            Label.LostKeyboardFocus += Label_LostKeyboardFocus;
            Label.MouseDoubleClick += Label_MouseDoubleClick;

            Label.KeyDown += Label_KeyDown;

            Label.MouseDown += Label_MouseDown;
            Label.MouseMove += Label_MouseMove;
            Label.MouseUp += Label_MouseUp;

            LabelBotLeft.Visibility = Visibility.Hidden;
            LabelBotMid.Visibility = Visibility.Hidden;
            LabelBotRight.Visibility = Visibility.Hidden;
            LabelMidLeft.Visibility = Visibility.Hidden;
            LabelMidRight.Visibility = Visibility.Hidden;
            LabelTopLeft.Visibility = Visibility.Hidden;
            LabelTopMid.Visibility = Visibility.Hidden;
            LabelTopRight.Visibility = Visibility.Hidden;

            LabelBotLeft.MouseMove += LabelBotLeft_MouseMove;
            LabelBotMid.MouseMove += LabelBotMid_MouseMove;
            LabelBotRight.MouseMove += LabelBotRight_MouseMove;
            LabelMidLeft.MouseMove += LabelMidLeft_MouseMove;
            LabelMidRight.MouseMove += LabelMidRight_MouseMove;
            LabelTopLeft.MouseMove += LabelTopLeft_MouseMove;
            LabelTopMid.MouseMove += LabelTopMid_MouseMove;
            LabelTopRight.MouseMove += LabelTopRight_MouseMove;

            LabelBotLeft.MouseDown += LabelResize_MouseDown;
            LabelBotMid.MouseDown += LabelResize_MouseDown;
            LabelBotRight.MouseDown += LabelResize_MouseDown;
            LabelMidLeft.MouseDown += LabelResize_MouseDown;
            LabelMidRight.MouseDown += LabelResize_MouseDown;
            LabelTopLeft.MouseDown += LabelResize_MouseDown;
            LabelTopMid.MouseDown += LabelResize_MouseDown;
            LabelTopRight.MouseDown += LabelResize_MouseDown;

            LabelBotLeft.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelBotMid.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelBotRight.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelMidLeft.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelMidRight.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelTopLeft.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelTopMid.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelTopRight.LostKeyboardFocus += Label_LostKeyboardFocus;


            LabelBotLeft.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelBotMid.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelBotRight.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelMidLeft.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelMidRight.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelTopLeft.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelTopMid.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelTopRight.GotKeyboardFocus += Label_GotKeyboardFocus;
        }

        public SingleGLED(int index, System.Windows.Point startLocation)
        {
            rect = new Rect(index * 20 + startLocation.X, startLocation.Y, 20, 20);
            Name = "G" + (index + 1);
            Index = index;

            Label.ToolTip = CropImageHolder;

            Label.GotKeyboardFocus += Label_GotKeyboardFocus;
            Label.LostKeyboardFocus += Label_LostKeyboardFocus;

            Label.KeyDown += Label_KeyDown;

            Label.MouseDown += Label_MouseDown;
            Label.MouseMove += Label_MouseMove;
            Label.MouseUp += Label_MouseUp;
            Label.MouseDoubleClick += Label_MouseDoubleClick;

            LabelBotLeft.Visibility = Visibility.Hidden;
            LabelBotMid.Visibility = Visibility.Hidden;
            LabelBotRight.Visibility = Visibility.Hidden;
            LabelMidLeft.Visibility = Visibility.Hidden;
            LabelMidRight.Visibility = Visibility.Hidden;
            LabelTopLeft.Visibility = Visibility.Hidden;
            LabelTopMid.Visibility = Visibility.Hidden;
            LabelTopRight.Visibility = Visibility.Hidden;

            LabelBotLeft.MouseMove += LabelBotLeft_MouseMove;
            LabelBotMid.MouseMove += LabelBotMid_MouseMove;
            LabelBotRight.MouseMove += LabelBotRight_MouseMove;
            LabelMidLeft.MouseMove += LabelMidLeft_MouseMove;
            LabelMidRight.MouseMove += LabelMidRight_MouseMove;
            LabelTopLeft.MouseMove += LabelTopLeft_MouseMove;
            LabelTopMid.MouseMove += LabelTopMid_MouseMove;
            LabelTopRight.MouseMove += LabelTopRight_MouseMove;

            LabelBotLeft.MouseDown += LabelResize_MouseDown;
            LabelBotMid.MouseDown += LabelResize_MouseDown;
            LabelBotRight.MouseDown += LabelResize_MouseDown;
            LabelMidLeft.MouseDown += LabelResize_MouseDown;
            LabelMidRight.MouseDown += LabelResize_MouseDown;
            LabelTopLeft.MouseDown += LabelResize_MouseDown;
            LabelTopMid.MouseDown += LabelResize_MouseDown;
            LabelTopRight.MouseDown += LabelResize_MouseDown;

            LabelBotLeft.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelBotMid.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelBotRight.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelMidLeft.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelMidRight.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelTopLeft.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelTopMid.LostKeyboardFocus += Label_LostKeyboardFocus;
            LabelTopRight.LostKeyboardFocus += Label_LostKeyboardFocus;


            LabelBotLeft.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelBotMid.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelBotRight.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelMidLeft.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelMidRight.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelTopLeft.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelTopMid.GotKeyboardFocus += Label_GotKeyboardFocus;
            LabelTopRight.GotKeyboardFocus += Label_GotKeyboardFocus;
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!IsReadOnly)
            {
                this.IsPass = !this.IsPass;
            }
        }


        //Parent and positions

        public void SetParentCanvas(Canvas placeCanvas)
        {
            if (ParentCanvas != null)
            {
                ParentCanvas.Children.Remove(Label);
                ParentCanvas.Children.Remove(LabelTopLeft);
                ParentCanvas.Children.Remove(LabelTopMid);
                ParentCanvas.Children.Remove(LabelTopRight);
                ParentCanvas.Children.Remove(LabelMidLeft);
                ParentCanvas.Children.Remove(LabelMidRight);
                ParentCanvas.Children.Remove(LabelBotLeft);
                ParentCanvas.Children.Remove(LabelBotMid);
                ParentCanvas.Children.Remove(LabelBotRight);
            }
            this.ParentCanvas = placeCanvas;

            Label.Width = rect.Width;
            Label.Height = rect.Height;

            Canvas.SetTop(this.Label, rect.Y);
            Canvas.SetLeft(this.Label, rect.X);

            Canvas.SetTop(this.LabelTopLeft, rect.Y - 2);
            Canvas.SetLeft(this.LabelTopLeft, rect.X - 2);

            Canvas.SetTop(this.LabelTopMid, rect.Y - 2);
            Canvas.SetLeft(this.LabelTopMid, rect.X - 2 + rect.Width / 2);

            Canvas.SetTop(this.LabelTopRight, rect.Y - 2);
            Canvas.SetLeft(this.LabelTopRight, rect.X - 3 + rect.Width);

            Canvas.SetTop(this.LabelMidLeft, rect.Y - 2 + rect.Height / 2);
            Canvas.SetLeft(this.LabelMidLeft, rect.X - 2);

            Canvas.SetTop(this.LabelMidRight, rect.Y - 2 + rect.Height / 2);
            Canvas.SetLeft(this.LabelMidRight, rect.X - 3 + rect.Width);

            Canvas.SetTop(this.LabelBotLeft, rect.Y - 3 + rect.Height);
            Canvas.SetLeft(this.LabelBotLeft, rect.X - 2);

            Canvas.SetTop(this.LabelBotMid, rect.Y - 3 + rect.Height);
            Canvas.SetLeft(this.LabelBotMid, rect.X - 2 + rect.Width / 2);

            Canvas.SetTop(this.LabelBotRight, rect.Y - 3 + rect.Height);
            Canvas.SetLeft(this.LabelBotRight, rect.X - 3 + rect.Width);

            //(Label.Parent as Canvas).Children.Clear();
            //(LabelTopLeft.Parent as Canvas).Children.Clear();
            //(LabelTopMid.Parent as Canvas).Children.Clear();
            //(LabelTopRight.Parent as Canvas).Children.Clear();
            //(LabelMidLeft.Parent as Canvas).Children.Clear();
            //(LabelMidRight.Parent as Canvas).Children.Clear();
            //(LabelBotLeft.Parent as Canvas).Children.Clear();
            //(LabelBotMid.Parent as Canvas).Children.Clear();
            //(LabelBotRight.Parent as Canvas).Children.Clear();

            placeCanvas.Children.Add(Label);

            placeCanvas.Children.Add(LabelTopLeft);
            placeCanvas.Children.Add(LabelTopMid);
            placeCanvas.Children.Add(LabelTopRight);

            placeCanvas.Children.Add(LabelMidLeft);
            placeCanvas.Children.Add(LabelMidRight);

            placeCanvas.Children.Add(LabelBotLeft);
            placeCanvas.Children.Add(LabelBotMid);
            placeCanvas.Children.Add(LabelBotRight);

        }

        private void Label_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            Rect areaRect = Rect;
            double distanceMove = 1;
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                distanceMove = 30;
            }
            switch (e.Key)
            {
                case Key.Left:
                    areaRect.X = rect.X - distanceMove;
                    break;
                case Key.Up:
                    areaRect.Y = rect.Y - distanceMove;
                    break;
                case Key.Right:
                    areaRect.X = rect.X + distanceMove;
                    break;
                case Key.Down:
                    areaRect.Y = rect.Y + distanceMove;
                    break;
            }
            Rect = areaRect;
            SetPosition();
        }

        private void LabelResize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Keyboard.Focus(sender as Label);
            Keyboard.Focus(sender as Label);
        }

        private void LabelTopRight_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
                areaRect.Width = Math.Abs(e.GetPosition(ParentCanvas).X - rect.X);
                areaRect.Height = Math.Abs(areaRect.Height + rect.Y - e.GetPosition(ParentCanvas).Y);
                areaRect.Y = e.GetPosition(ParentCanvas).Y;
                Rect = areaRect;
                SetPosition();
            }
        }

        private void LabelTopMid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
                areaRect.Height = Math.Abs(areaRect.Height + rect.Y - e.GetPosition(ParentCanvas).Y);
                areaRect.Y = e.GetPosition(ParentCanvas).Y;
                Rect = areaRect;
                SetPosition();
            }
        }

        private void LabelTopLeft_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
                areaRect.Width = Math.Abs(areaRect.Width + rect.X - e.GetPosition(ParentCanvas).X);
                areaRect.Height = Math.Abs(areaRect.Height + rect.Y - e.GetPosition(ParentCanvas).Y);
                areaRect.Y = e.GetPosition(ParentCanvas).Y;
                areaRect.X = e.GetPosition(ParentCanvas).X;
                Rect = areaRect;
                SetPosition();
            }
        }

        private void LabelMidRight_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
                areaRect.Width = Math.Abs(e.GetPosition(ParentCanvas).X - areaRect.X);
                //areaRect.Height = Math.Abs(areaRect.Height + rect.Y - e.GetPosition(ParentCanvas).Y);
                //areaRect.Y = e.GetPosition(ParentCanvas).Y;
                //areaRect.X = e.GetPosition(ParentCanvas).X;
                Rect = areaRect;
                SetPosition();
            }
        }

        private void LabelMidLeft_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
                areaRect.Width = Math.Abs(areaRect.Width + rect.X - e.GetPosition(ParentCanvas).X);
                //areaRect.Height = Math.Abs(areaRect.Height + rect.Y - e.GetPosition(ParentCanvas).Y);
                //areaRect.Y = e.GetPosition(ParentCanvas).Y;
                areaRect.X = e.GetPosition(ParentCanvas).X;
                Rect = areaRect;
                SetPosition();
            }
        }

        private void LabelBotRight_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
                areaRect.Width = Math.Abs(e.GetPosition(ParentCanvas).X - rect.X);
                areaRect.Height = Math.Abs(e.GetPosition(ParentCanvas).Y - rect.Y);
                Rect = areaRect;
                SetPosition();
            }
        }

        private void LabelBotMid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
                //areaRect.Width = Math.Abs(areaRect.Width + rect.X - e.GetPosition(ParentCanvas).X);
                areaRect.Height = Math.Abs(e.GetPosition(ParentCanvas).Y - areaRect.Y);
                //areaRect.Y = e.GetPosition(ParentCanvas).Y;
                //areaRect.X = e.GetPosition(ParentCanvas).X;
                Rect = areaRect;
                SetPosition();
            }
        }

        private void LabelBotLeft_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
                areaRect.Width = Math.Abs(areaRect.Width + rect.X - e.GetPosition(ParentCanvas).X);
                areaRect.Height = Math.Abs(e.GetPosition(ParentCanvas).Y - rect.Y);
                //areaRect.Y = e.GetPosition(ParentCanvas).Y;
                areaRect.X = e.GetPosition(ParentCanvas).X;
                Rect = areaRect;
                SetPosition();
            }
        }


        private void Label_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                LabelBotLeft.MouseMove -= LabelBotLeft_MouseMove;
                LabelBotMid.MouseMove -= LabelBotMid_MouseMove;
                LabelBotRight.MouseMove -= LabelBotRight_MouseMove;
                LabelMidLeft.MouseMove -= LabelMidLeft_MouseMove;
                LabelMidRight.MouseMove -= LabelMidRight_MouseMove;
                LabelTopLeft.MouseMove -= LabelTopLeft_MouseMove;
                LabelTopMid.MouseMove -= LabelTopMid_MouseMove;
                LabelTopRight.MouseMove -= LabelTopRight_MouseMove;

                e.Handled = true;
                Label.Cursor = Cursors.SizeAll;
                Keyboard.Focus(Label);
                OfsetMove = new Rect()
                {
                    Width = Math.Max(Math.Abs(e.GetPosition(ParentCanvas).X - rect.X), 5),
                    Height = Math.Max(Math.Abs(e.GetPosition(ParentCanvas).Y - rect.Y), 5),
                };
                OnSelected();
            }
        }

        private void Label_MouseMove(object sender, MouseEventArgs e)
        {
            //Console.WriteLine("Label raise event");
            if (e.LeftButton == MouseButtonState.Pressed && !e.Handled && Label.IsKeyboardFocused)
            {
                LabelBotLeft.MouseMove -= LabelBotLeft_MouseMove;
                LabelBotMid.MouseMove -= LabelBotMid_MouseMove;
                LabelBotRight.MouseMove -= LabelBotRight_MouseMove;
                LabelMidLeft.MouseMove -= LabelMidLeft_MouseMove;
                LabelMidRight.MouseMove -= LabelMidRight_MouseMove;
                LabelTopLeft.MouseMove -= LabelTopLeft_MouseMove;
                LabelTopMid.MouseMove -= LabelTopMid_MouseMove;
                LabelTopRight.MouseMove -= LabelTopRight_MouseMove;

                e.Handled = true;
                Rect areaRect = Rect;
                areaRect.X = e.GetPosition(ParentCanvas).X - OfsetMove.Width;
                areaRect.Y = e.GetPosition(ParentCanvas).Y - OfsetMove.Height;
                Rect = areaRect;
                SetPosition();
            }

            if (e.LeftButton == MouseButtonState.Pressed && (e.Source as FrameworkElement) == Label)
            {
                var focusElement = Keyboard.FocusedElement;
                if (focusElement != null && focusElement.GetType() == typeof(Label))
                {
                    focusElement.RaiseEvent(e);
                }
            }

        }

        private void Label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            LabelBotLeft.MouseMove += LabelBotLeft_MouseMove;
            LabelBotMid.MouseMove += LabelBotMid_MouseMove;
            LabelBotRight.MouseMove += LabelBotRight_MouseMove;
            LabelMidLeft.MouseMove += LabelMidLeft_MouseMove;
            LabelMidRight.MouseMove += LabelMidRight_MouseMove;
            LabelTopLeft.MouseMove += LabelTopLeft_MouseMove;
            LabelTopMid.MouseMove += LabelTopMid_MouseMove;
            LabelTopRight.MouseMove += LabelTopRight_MouseMove;
        }

        private void Label_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

            LabelBotLeft.Visibility = Visibility.Hidden;
            LabelBotMid.Visibility = Visibility.Hidden;
            LabelBotRight.Visibility = Visibility.Hidden;
            LabelMidLeft.Visibility = Visibility.Hidden;
            LabelMidRight.Visibility = Visibility.Hidden;
            LabelTopLeft.Visibility = Visibility.Hidden;
            LabelTopMid.Visibility = Visibility.Hidden;
            LabelTopRight.Visibility = Visibility.Hidden;
        }

        private void Label_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            LabelBotLeft.Visibility = Visibility.Visible;
            LabelBotMid.Visibility = Visibility.Visible;
            LabelBotRight.Visibility = Visibility.Visible;
            LabelMidLeft.Visibility = Visibility.Visible;
            LabelMidRight.Visibility = Visibility.Visible;
            LabelTopLeft.Visibility = Visibility.Visible;
            LabelTopMid.Visibility = Visibility.Visible;
            LabelTopRight.Visibility = Visibility.Visible;

            LabelBotLeft.MouseMove -= LabelBotLeft_MouseMove;
            LabelBotMid.MouseMove -= LabelBotMid_MouseMove;
            LabelBotRight.MouseMove -= LabelBotRight_MouseMove;
            LabelMidLeft.MouseMove -= LabelMidLeft_MouseMove;
            LabelMidRight.MouseMove -= LabelMidRight_MouseMove;
            LabelTopLeft.MouseMove -= LabelTopLeft_MouseMove;
            LabelTopMid.MouseMove -= LabelTopMid_MouseMove;
            LabelTopRight.MouseMove -= LabelTopRight_MouseMove;

            LabelBotLeft.MouseMove += LabelBotLeft_MouseMove;
            LabelBotMid.MouseMove += LabelBotMid_MouseMove;
            LabelBotRight.MouseMove += LabelBotRight_MouseMove;
            LabelMidLeft.MouseMove += LabelMidLeft_MouseMove;
            LabelMidRight.MouseMove += LabelMidRight_MouseMove;
            LabelTopLeft.MouseMove += LabelTopLeft_MouseMove;
            LabelTopMid.MouseMove += LabelTopMid_MouseMove;
            LabelTopRight.MouseMove += LabelTopRight_MouseMove;
        }

        public void SetPosition()
        {
            Canvas.SetTop(this.Label, rect.Y);
            Canvas.SetLeft(this.Label, rect.X);

            Canvas.SetTop(this.LabelTopLeft, rect.Y - 2);
            Canvas.SetLeft(this.LabelTopLeft, rect.X - 2);

            Canvas.SetTop(this.LabelTopMid, rect.Y - 2);
            Canvas.SetLeft(this.LabelTopMid, rect.X - 2 + Label.Width / 2);

            Canvas.SetTop(this.LabelTopRight, rect.Y - 2);
            Canvas.SetLeft(this.LabelTopRight, rect.X - 3 + Label.Width);

            Canvas.SetTop(this.LabelMidLeft, rect.Y - 2 + Label.Height / 2);
            Canvas.SetLeft(this.LabelMidLeft, rect.X - 2);

            Canvas.SetTop(this.LabelMidRight, rect.Y - 2 + Label.Height / 2);
            Canvas.SetLeft(this.LabelMidRight, rect.X - 3 + Label.Width);

            Canvas.SetTop(this.LabelBotLeft, rect.Y - 3 + Label.Height);
            Canvas.SetLeft(this.LabelBotLeft, rect.X - 2);

            Canvas.SetTop(this.LabelBotMid, rect.Y - 3 + Label.Height);
            Canvas.SetLeft(this.LabelBotMid, rect.X - 2 + Label.Width / 2);

            Canvas.SetTop(this.LabelBotRight, rect.Y - 3 + Label.Height);
            Canvas.SetLeft(this.LabelBotRight, rect.X - 3 + Label.Width);
        }

        public void TestImage(Mat source)
        {
            if (source == null) return;
            if (ParentCanvas == null) return;

            double scaleX = source.Width / ParentCanvas.Width;
            double scaleY = source.Height / ParentCanvas.Height;

            OpenCvSharp.Rect rect = new OpenCvSharp.Rect(
                new OpenCvSharp.Point(this.Rect.X * scaleX, this.Rect.Y * scaleY),
                new OpenCvSharp.Size(this.Rect.Width * scaleX, this.Rect.Height * scaleY));

            using (var croppedMat = new Mat(source, rect))
            {
                Mat gray = croppedMat.CvtColor(ColorConversionCodes.BGR2GRAY);
                Intens = (int)gray.Mean().Val0;
                var cropImage = gray.ToBitmapSource();
                cropImage.Freeze();
                CropImageHolder.Source = cropImage;
                CropImage = cropImage;
            }
        }
        public string TestImage(Mat source, bool OutString = true)
        {
            if (source == null) return "0";
            if (ParentCanvas == null) return "0";

            double scaleX = source.Width / ParentCanvas.Width;
            double scaleY = source.Height / ParentCanvas.Height;

            OpenCvSharp.Rect rect = new OpenCvSharp.Rect(
                new OpenCvSharp.Point(this.Rect.X * scaleX, this.Rect.Y * scaleY),
                new OpenCvSharp.Size(this.Rect.Width * scaleX, this.Rect.Height * scaleY));

            using (var croppedMat = new Mat(source, rect))
            {
                Mat gray = croppedMat.CvtColor(ColorConversionCodes.BGR2GRAY);
                Intens = (int)gray.Mean().Val0;
                var cropImage = gray.ToBitmapSource();
                cropImage.Freeze();
                CropImageHolder.Source = cropImage;
                CropImage = cropImage;
            }

            if (Intens >= Thresh)
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }
    }
}
