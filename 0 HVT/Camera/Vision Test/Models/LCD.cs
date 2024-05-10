using DirectShowLib;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
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
using static System.Net.Mime.MediaTypeNames;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rect = System.Windows.Rect;



namespace Camera
{
    public class LCD : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        static OpenCvSharp.Text.OCRTesseract tesseract = OpenCvSharp.Text.OCRTesseract.Create(psmode: 13);

        public event EventHandler Sellected;
        private void OnSelected()
        {
            Sellected?.Invoke(this, null);
        }

        private Mat mat { get; set; } = new Mat();


        private string _SpectString = "8888";
        [JsonIgnore]
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
        [JsonIgnore]
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
                    Label.Dispatcher.Invoke(new Action(() =>
                        Label.Content = string.Format("{0}: {1}", Name, value)
                    ));
                }
                OnPropertyChanged("IsPass");
            }
        }


        private bool _IsPass;
        [JsonIgnore]
        public bool IsPass
        {
            get { return _IsPass; }
            set
            {
                if (value != _IsPass)
                    _IsPass = value;
                OnPropertyChanged("IsPass");
            }
        }

        private List<Models.SettingParram> _Param = new List<Models.SettingParram>();
        [JsonIgnore]
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

        enum TurnningState
        {
            wait,
            turning,
            end
        }
        private TurnningState turnningState = LCD.TurnningState.wait;

        private bool _IsTurning;
        [JsonIgnore]
        public bool IsTurning
        {
            get { return _IsTurning; }
            set
            {
                if (value != _IsTurning)
                {
                    _IsTurning = value;
                    if (value)
                    {
                        TurningProgress = 0;
                    }
                    OnPropertyChanged("IsTurning");
                }
            }
        }

        private double _TurningProgress;
        [JsonIgnore]
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
            Size = new System.Windows.Size(100, 50)
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
                    }

                    if (value.Y > 0 && value.Y < ParentCanvasSize.Height - value.Height)
                    {
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





        public LCD()
        {
            Label.ToolTip = CropImageHolder;

            Label.GotKeyboardFocus += Label_GotKeyboardFocus;
            Label.LostKeyboardFocus += Label_LostKeyboardFocus;

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

            for (int j = 0; j < 10; j++)
            {
                for (int i = 1; i < 254; i++)
                {

                    Param.Add(new Models.SettingParram()
                    {
                        Threshold = i,
                        Blur = j,
                        IsPass = false
                    });
                }
            }
        }

        public LCD(int index)
        {
            rect = new Rect(670, 200 + 31 * index, 100, 30);
            Name = "LCD" + (index + 1);
            Label.ToolTip = CropImageHolder;

            Label.GotKeyboardFocus += Label_GotKeyboardFocus;
            Label.LostKeyboardFocus += Label_LostKeyboardFocus;

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

            for (int i = 1; i < 254; i++)
            {
                Param.Add(new Models.SettingParram()
                {
                    Threshold = i,
                    IsPass = false
                });
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

        //Get and process image

        public async void TestImage(Mat source, string target)
        {
            if (IsTurning && turnningState == TurnningState.wait)
            {
                mat = source.Clone();
                AutoCalibration();
                turnningState = TurnningState.turning;
                return;
            }
            if (IsTurning)
            {
                mat = source.Clone();
                return;
            }

            if (source == null) return;
            if (ParentCanvas == null) return;
            double scaleX = source.Width / ParentCanvasSize.Width;
            double scaleY = source.Height / ParentCanvasSize.Height;

            OpenCvSharp.Rect rect = new OpenCvSharp.Rect(
                new OpenCvSharp.Point(this.Rect.X * scaleX, this.Rect.Y * scaleY),
                new OpenCvSharp.Size(this.Rect.Width * scaleX, this.Rect.Height * scaleY));
            using (var croppedMat = new Mat(source, rect))
            {
                if (!Processing)
                {
                    //var processedBitmap = DetectString(croppedMat, (int)Threshold, Blur, out string data);
                    var processedBitmap = DetectStringRegion(croppedMat, (int)Threshold, out string data, target, NoiseSize);
                    Console.WriteLine(data);
                    DetectedString = data;
                    var cropImage = processedBitmap.ToBitmapSource();
                    cropImage.Freeze();
                    CropImageHolder.Dispatcher.Invoke(new Action(() => CropImageHolder.Source = cropImage));
                    CropImage = cropImage;
                }
                else
                {
                    var cropImage = source.ToBitmapSource();
                    cropImage.Freeze();
                    CropImageHolder.Source = cropImage;
                    CropImage = cropImage;
                }
                await Task.Delay(5);
            }
        }

        public async void AutoCalibration()
        {
            if (mat == null || ParentCanvas == null)
            {
                turnningState = TurnningState.wait;
                IsTurning = false;
                return;
            }

            turnningState = TurnningState.turning;

            double scaleX = mat.Width / ParentCanvasSize.Width;
            double scaleY = mat.Height / ParentCanvasSize.Height;

            OpenCvSharp.Rect rect = new OpenCvSharp.Rect(
                new OpenCvSharp.Point(this.Rect.X * scaleX, this.Rect.Y * scaleY),
                new OpenCvSharp.Size(this.Rect.Width * scaleX, this.Rect.Height * scaleY));

            for (int i = 0; i < Param.Count; i++)
            {
                using (var croppedMat = new Mat(mat, rect))
                {
                    TurningProgress = (i / (double)Param.Count * 100);
                    var brightness = Param[i];
                    //var processedBitmap = DetectString(croppedMat, (int)Threshold, Blur, out string data);
                    var processedBitmap = DetectStringRegion(croppedMat, (int)brightness.Threshold, out string data, SpectString, NoiseSize);
                    this.DetectedString = data;
                    Console.WriteLine(data);
                    DetectedString = data;
                    brightness.IsPass = IsPass;
                    var cropImage = processedBitmap.ToBitmapSource();
                    cropImage.Freeze();
                    CropImageHolder.Source = cropImage;
                    CropImage = cropImage;
                    if (i % 10 == 0)
                    {
                        await Task.Delay(5);
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(1));
                    }
                    Param[i] = brightness;
                }
                TurningProgress = 100;
            }
            var bestSlution = Param.Where(o => o.IsPass).ToList();
            if (bestSlution.Count > 1)
            {
                var bestBrightness = bestSlution.GroupBy(o => o.Threshold).OrderByDescending(s => s.Count()).First().Key;
                var bestBlur = bestSlution.GroupBy(o => o.Blur).OrderByDescending(s => s.Count()).First().Key;
                var bestNoise = bestSlution.GroupBy(o => o.Noise).OrderByDescending(s => s.Count()).First().Key;
                Threshold = bestBrightness;
                Blur = bestBlur;
                NoiseSize = (int)bestNoise;
            }
            turnningState = TurnningState.wait;
            IsTurning = false;
        }

        static double map(float s, double a1, double a2, double b1, double b2)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }

        public static bool Processing = false;
        //public static TesseractEngine engine = new TesseractEngine(@"./tessData", "eng", EngineMode.Default);

        //public static Mat DetectString(Mat source, double Threshold, double blur, out string str)
        //{
        //    engine.SetVariable("debug_file", "NUL");
        //    Processing = true;
        //    DateTime now = DateTime.Now;
        //    Mat mat = source.Clone();
        //    Bitmap output = mat.ToBitmap();
        //    var ocrtext = string.Empty;
        //    try
        //    {
        //        using (var img = PixConverter.ToPix(output))
        //        {
        //            using (var page = engine.Process(img))
        //            {
        //                ocrtext = page.GetText();
        //                var rects = page.GetSegmentedRegions(PageIteratorLevel.Block);
        //                foreach (var item in rects)
        //                {
        //                    mat.Rectangle(new OpenCvSharp.Rect(item.X, item.Y, item.Width, item.Height), new Scalar(0, 255, 0));
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    str = ocrtext.Replace("\r", "").Replace("\n", "");
        //    Processing = false;
        //    return mat;
        //}

        public static Mat DetectStringRegion(Mat source, int threshold, out string detectString,string targetStr, int noiseSize = 0)
        {
            Mat sourceToTest = source.Clone();
            Mat graysource = sourceToTest.CvtColor(ColorConversionCodes.BGR2GRAY);
            //float[] kdata = new float[] { 0, 1, 0, 1, 5, 1, 0, 1, 0 };
            //Mat kernel = new Mat(3, 3, MatType.CV_8U, kdata);
            Mat binarySource = graysource.Threshold(threshold, 255, ThresholdTypes.Binary);
            //Mat binarySource = graysource.Filter2D(0,kernel);
            //binarySource = binarySource.Erode(kernel);
            OpenCvSharp.Point[][] contour;
            Cv2.FindContours(binarySource, out contour, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            for (int i = 0; i < contour.Length; i++)
            {
                if (Cv2.ContourArea(contour[i]) < noiseSize)
                {
                    binarySource.DrawContours(contour, i, new Scalar(0, 0, 0), -1);
                }
            }
            Cv2.BitwiseNot(binarySource, binarySource);
            tesseract.SetWhiteList(targetStr);
            tesseract.Run(binarySource, out string text, out _, out _, out _, OpenCvSharp.Text.ComponentLevels.TextLine);
            Console.WriteLine(" Output text: {0}", text);
            detectString = text.Replace("\r", "\n").Replace("\n", "");
            return binarySource;
        }
    }
}


