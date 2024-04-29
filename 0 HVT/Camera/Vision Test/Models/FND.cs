using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Rect = System.Windows.Rect;

namespace Camera
{
    public class FND : INotifyPropertyChanged
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
                }
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

        private int _NoiseSize = 1;
        public int NoiseSize
        {
            get { return _NoiseSize; }
            set
            {
                if (value != _NoiseSize && value > 1)
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
        private TurnningState turnningState = FND.TurnningState.wait;

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

        public FND()
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

            for (int j = 3; j < 11; j++)
            {
                for (int i = 1; i < 254; i++)
                {
                    for (int z = 3; z < 11; z++)
                    {
                        Param.Add(new Models.SettingParram()
                        {
                            Threshold = i,
                            Blur = j,
                            Noise = z,
                            IsPass = false
                        });
                    }
                }
            }
        }
        public FND(int index)
        {
            rect = new Rect(670, 31 * index, 100, 30);
            Name = "FND" + (index + 1);
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

            for (int j = 3; j < 11; j++)
            {
                for (int i = 1; i < 254; i++)
                {
                    for (int z = 3; z < 11; z++)
                    {
                        Param.Add(new Models.SettingParram()
                        {
                            Threshold = i,
                            Blur = j,
                            Noise = z,
                            IsPass = false
                        });
                    }
                }
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

        public void TestImage(Mat source)
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
            double scaleX = source.Width / ParentCanvas.Width;
            double scaleY = source.Height / ParentCanvas.Height;

            OpenCvSharp.Rect rect = new OpenCvSharp.Rect(
                new OpenCvSharp.Point(this.Rect.X * scaleX, this.Rect.Y * scaleY),
                new OpenCvSharp.Size(this.Rect.Width * scaleX, this.Rect.Height * scaleY));
            using (var croppedMat = new Mat(source, rect))
            {
                var processedBitmap = SeventSegmentDetect(croppedMat, Threshold, Blur, NoiseSize, out string data);
                this.DetectedString = data;
                var cropImage = processedBitmap.ToBitmapSource();
                cropImage.Freeze();
                CropImageHolder.Source = cropImage;
                CropImage = cropImage;
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

            double scaleX = mat.Width / ParentCanvas.Width;
            double scaleY = mat.Height / ParentCanvas.Height;

            OpenCvSharp.Rect rect = new OpenCvSharp.Rect(
                new OpenCvSharp.Point(this.Rect.X * scaleX, this.Rect.Y * scaleY),
                new OpenCvSharp.Size(this.Rect.Width * scaleX, this.Rect.Height * scaleY));

            for (int i = 0; i < Param.Count; i++)
            {
                using (var croppedMat = new Mat(mat, rect))
                {
                    TurningProgress = (i / (double)Param.Count * 100);
                    var brightness = Param[i];
                    var processedBitmap = SeventSegmentDetect(croppedMat, brightness.Threshold, brightness.Blur, (int)brightness.Noise, out string data);
                    DetectedString = data;
                    brightness.IsPass = IsPass;
                    var cropImage = processedBitmap.ToBitmapSource();
                    cropImage.Freeze();
                    CropImageHolder.Source = cropImage;
                    CropImage = cropImage;
                    if (i % 100 == 0)
                    {
                        await Task.Delay(5);
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

        public static Mat SeventSegmentDetect(Mat source, double brightness, double blur, int noiseSize, out string detectedString)
        {
            detectedString = "";

            if (source.Height < 10 || source.Width < 10)
            {
                return new Mat();
            }

            Mat mInput = source;
            Mat gray = mInput.CvtColor(ColorConversionCodes.RGB2GRAY);


            gray.MinMaxIdx(out _, out double maxval);

            Mat edge = gray.Threshold(brightness, 255, ThresholdTypes.Binary);
            //Cv2.FastNlMeansDenoising(edge, edge, 0,4);
            Mat blurred = edge.GaussianBlur(new OpenCvSharp.Size(1, 21), 0, blur);
            Mat blurgreen = blurred.Threshold(50, 255, ThresholdTypes.Binary);
            OpenCvSharp.Point[][] contour;
            HierarchyIndex[] hierarchy;
            List<OpenCvSharp.Rect> digitContour = new List<OpenCvSharp.Rect>();

            // make corect region 
            Mat blurWhiteDown = blurgreen;
            Mat blurWhiteUp = blurgreen;

            Mat moutput = edge.CvtColor(ColorConversionCodes.GRAY2RGB);

            Cv2.FindContours(blurgreen, out contour, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            for (int i = 0; i < contour.Length; i++)
            {
                //mInput.DrawContours(contour, i, new Scalar(255, 0, 0));
                OpenCvSharp.Rect rect = Cv2.BoundingRect(contour[i]);
                if (Cv2.ContourArea(contour[i]) > noiseSize * noiseSize && rect.Height > (blur * 2 + 2))
                {
                    digitContour.Add(rect);
                }
                else
                {
                    moutput.DrawContours(contour, i, new Scalar(0, 0, 255));
                }
            }
            digitContour = digitContour.OrderBy(o => o.Left).ToList();
            if (digitContour.Count < 1) return moutput;
            int maxHeightValue = digitContour.OrderBy(x => x.Height).ToList()[0].Height;
            int maxTopValue = digitContour.OrderBy(x => x.Top).ToList()[0].Top;
            foreach (var item in digitContour)
            {
                //Console.Write(item.Left.ToString() + "->");
                var rect = item;
                try
                {
                    detectedString += DetectSegmentChar(rect, new Mat(edge, item), moutput, (int)blur, maxHeight: maxHeightValue, minTop: maxTopValue);
                }
                catch (OpenCvSharp.OpenCvSharpException er)
                {
                    Console.WriteLine( er.Message);
                }
            }
            return moutput;
        }

        private static char DetectSegmentChar(OpenCvSharp.Rect rectg, Mat Matinput, Mat colorMat, int blurOffset, bool IsDoubleDot = false, int maxHeight = 0, int minTop = 0)
        {
            double threadShot = 100;
            //Matinput.MinMaxIdx(out _, out threadShot);
            //threadShot *= 0.5;
            Mat input = Matinput.Threshold(threadShot, 255, ThresholdTypes.Otsu);
            SegementCharacter segement = new SegementCharacter();
            var W = input.Width;
            var H = input.Height;
            Mat matDigit;

            OpenCvSharp.Rect rect = new OpenCvSharp.Rect();

            if (IsDoubleDot)
            {
                W = input.Width;
                H = input.Height;

                rect = new OpenCvSharp.Rect(0, 0, W, H / 2);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > 10)
                {
                    segement.digit[2] = 1;
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }

                rect = new OpenCvSharp.Rect(0, H / 2, W, H / 2);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > 10)
                {
                    segement.digit[5] = 1;
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }
            }

            else if ((double)H / W > 2)
            {
                W = input.Width;
                H = input.Height;

                rect = new OpenCvSharp.Rect(0, 0, W, H / 2);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[2] = 1;
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }

                rect = new OpenCvSharp.Rect(0, H / 2, W, H / 2);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[5] = 1;
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }
            }
            else if ((double)W / H > 1.2)
            {
                W = input.Width;
                H = input.Height;

                rect = new OpenCvSharp.Rect(0, 0, W, H);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[3] = 1;
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }
            }
            else if ((double)H / W < 1.2)
            {
                if (rectg.Top < (minTop + maxHeight / 2))
                {
                    input.Resize(new OpenCvSharp.Size(4 * W, 4 * H), 4, 8);
                    W = input.Width;
                    H = input.Height - blurOffset * 2;
                    rect = new OpenCvSharp.Rect(W / 4, blurOffset, W / 2, H / 6);
                    matDigit = new Mat(input, rect);
                    if (matDigit.Mean().Val0 > threadShot)
                    {
                        segement.digit[0] = 1;
                        colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                        //colorMat.PutText("1", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                    }

                    rect = new OpenCvSharp.Rect(0, H / 7 + blurOffset, W / 4, H - H/6);
                    matDigit = new Mat(input, rect);
                    if (matDigit.Mean().Val0 > threadShot)
                    {
                        segement.digit[1] = 1;
                        // colorMat.PutText("2", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                        colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                    }

                    rect = new OpenCvSharp.Rect(3 * W / 4, H / 7 + blurOffset, W / 4, H - H/6);
                    matDigit = new Mat(input, rect);
                    if (matDigit.Mean().Val0 > threadShot)
                    {
                        segement.digit[2] = 1;
                        //colorMat.PutText("3", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                        colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                    }

                    rect = new OpenCvSharp.Rect(W / 4, H - H / 4 + blurOffset, W / 2, H / 4);
                    matDigit = new Mat(input, rect);
                    if (matDigit.Mean().Val0 > threadShot)
                    {
                        segement.digit[3] = 1;
                        //colorMat.PutText("4", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                        colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                    }
                }
                else
                {
                    input.Resize(new OpenCvSharp.Size(4 * W, 8 * H), 4, 8);
                    W = input.Width;
                    H = input.Height - blurOffset * 2;

                    rect = new OpenCvSharp.Rect(W / 4, blurOffset, W / 2, H / 6);
                    matDigit = new Mat(input, rect);
                    if (matDigit.Mean().Val0 > threadShot)
                    {
                        segement.digit[3] = 1;
                        colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                        //colorMat.PutText("1", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                    }

                    rect = new OpenCvSharp.Rect(0, H / 7 + blurOffset, W / 4, H - H/6);
                    matDigit = new Mat(input, rect);
                    if (matDigit.Mean().Val0 > threadShot)
                    {
                        segement.digit[4] = 1;
                        // colorMat.PutText("2", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                        colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                    }

                    rect = new OpenCvSharp.Rect(3 * W / 4, H / 7 + blurOffset, W / 4, H - H / 6);
                    matDigit = new Mat(input, rect);

                    Console.WriteLine(matDigit.Mean().Val0);
                    if (matDigit.Mean().Val0 > threadShot)
                    {
                        segement.digit[5] = 1;
                        //colorMat.PutText("3", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                        colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                    }

                    rect = new OpenCvSharp.Rect(W / 4, H - H / 4 + blurOffset, W / 2, H / 4);
                    matDigit = new Mat(input, rect);
                    if (matDigit.Mean().Val0 > threadShot)
                    {
                        segement.digit[6] = 1;
                        //colorMat.PutText("4", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                        colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                    }

                }
            }
            else
            {
                input.Resize(new OpenCvSharp.Size(4 * W, 8 * H), 4, 8);
                W = input.Width;
                H = input.Height - blurOffset * 2;

                rect = new OpenCvSharp.Rect(W / 4, blurOffset, W / 2, H / 6);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[0] = 1;
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                    //colorMat.PutText("1", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                }

                rect = new OpenCvSharp.Rect(0, H / 7 + blurOffset, W / 4, H / 3);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[1] = 1;
                    // colorMat.PutText("2", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }

                rect = new OpenCvSharp.Rect(3 * W / 4, H / 7 + blurOffset, W / 4, H / 3);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[2] = 1;
                    //colorMat.PutText("3", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }

                rect = new OpenCvSharp.Rect(W / 4, H / 2 - H / 12 + blurOffset, W / 2, H / 6);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[3] = 1;
                    //colorMat.PutText("4", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }

                rect = new OpenCvSharp.Rect(0, 4 * H / 7 + blurOffset, W / 4, H / 3);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[4] = 1;
                    //colorMat.PutText("5", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }

                rect = new OpenCvSharp.Rect(3 * W / 4, 4 * H / 7 + blurOffset, W / 4, H / 3);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[5] = 1;
                    //colorMat.PutText("6", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }

                rect = new OpenCvSharp.Rect(W / 4, H - H / 6 + blurOffset, W / 2, H / 6);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[6] = 1;
                    //colorMat.PutText("7", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }
            }

            var returnVal = new SegementCharacter();
            returnVal.digit = segement.digit;
            foreach (var item in SEG_LOOKUP)
            {
                if (item.digitString == returnVal.digitString)
                {
                    returnVal = item;
                    break;
                }
            }
            return returnVal.character;
        }


        public static ObservableCollection<SegementCharacter> SEG_LOOKUP { get; set; } = new ObservableCollection<SegementCharacter>()
        {
            new SegementCharacter(){character = ' ', digit = new byte[7] {0,0,0,0,0,0,0}},
            new SegementCharacter(){character = '-', digit = new byte[7] {0,0,0,1,0,0,0}},
            new SegementCharacter(){character = 'N', digit = new byte[7] {0,0,0,1,1,1,0}},
            new SegementCharacter(){character = 'O', digit = new byte[7] {0,0,0,1,1,1,1}},
            new SegementCharacter(){character = '0', digit = new byte[7] {1,1,1,0,1,1,1}},
            new SegementCharacter(){character = '1', digit = new byte[7] {0,0,1,0,0,1,0}},
            new SegementCharacter(){character = '2', digit = new byte[7] {1,0,1,1,1,0,1}},
            new SegementCharacter(){character = '3', digit = new byte[7] {1,0,1,1,0,1,1}},
            new SegementCharacter(){character = '4', digit = new byte[7] {0,1,1,1,0,1,0}},
            new SegementCharacter(){character = '5', digit = new byte[7] {1,1,0,1,0,1,1}},
            new SegementCharacter(){character = '6', digit = new byte[7] {1,1,0,1,1,1,1}},
            new SegementCharacter(){character = '7', digit = new byte[7] {1,0,1,0,0,1,0}},
            new SegementCharacter(){character = '8', digit = new byte[7] {1,1,1,1,1,1,1}},
            new SegementCharacter(){character = '9', digit = new byte[7] {1,1,1,1,0,1,1}},
            new SegementCharacter(){character = 'A', digit = new byte[7] {1,1,1,1,1,1,0}},
            new SegementCharacter(){character = 'B', digit = new byte[7] {0,1,0,1,1,1,1}},
            new SegementCharacter(){character = 'C', digit = new byte[7] {1,1,0,0,1,0,1}},
            new SegementCharacter(){character = 'D', digit = new byte[7] {0,0,1,1,1,1,1}},
            new SegementCharacter(){character = 'E', digit = new byte[7] {1,1,0,1,1,0,1}},
            new SegementCharacter(){character = 'F', digit = new byte[7] {1,1,0,1,1,0,0}},
        };

    }

    public class SegementCharacter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private char _character = ' ';
        public char character
        {
            get { return _character; }
            set
            {
                if (value != _character)
                {
                    _character = value;
                    NotifyPropertyChanged("character");
                }
            }
        }
        private byte[] digits = new byte[7] { 0, 0, 0, 0, 0, 0, 0 };
        public byte[] digit
        {
            get { return digits; }
            set
            {
                if (digits != value)
                {
                    digits = value;
                    digitString = "";
                    foreach (byte b in digits)
                    {
                        digitString += b;
                    }
                }
            }
        }
        public string digitString
        {
            get
            {
                var digitStr = "";
                foreach (byte b in digits)
                {
                    digitStr += b;
                }
                return digitStr;
            }
            private set { }
        }
        public void DigitChange()
        {
            NotifyPropertyChanged("digitString");
            NotifyPropertyChanged("digit");
        }
    }

}


