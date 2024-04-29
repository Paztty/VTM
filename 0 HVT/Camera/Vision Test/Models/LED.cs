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
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;
using Rect = System.Windows.Rect;

namespace Camera
{
    public class LED
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
                foreach (SingleLED led in LEDs)
                {
                    led.Visibility = led.Use ? value : Visibility.Hidden;
                }
            }
        }

        public ObservableCollection<SingleLED> LEDs { get; set; } = new ObservableCollection<SingleLED>();

        private bool _IsReadOnly;
        [JsonIgnore]
        public bool IsReadOnly
        {
            get { return _IsReadOnly; }
            set
            {
                if (value != _IsReadOnly) _IsReadOnly = value;
                foreach (var item in LEDs)
                {
                    item.IsReadOnly = IsReadOnly;
                }
            }
        }

        public LED(System.Windows.Point startLocation)
        {
            CalculatorOutputString = "";
            for (int i = 0; i < 32; i++)
            {
                LEDs.Add(new SingleLED(i, startLocation));
            }
        }

        public LED() { }
        public void SetParentCanvas(Canvas ParentCanvas)
        {
            foreach (var item in LEDs)
            {
                item.SetParentCanvas(ParentCanvas);
                if (!item.Use)
                {
                    item.Visibility = Visibility.Hidden;
                }
            }
        }
        public void CALC_THRESH()
        {
            foreach (SingleLED LED in LEDs)
            {
                LED.Thresh = (int)(LED.ON - LED.OFF) / 3 * 2 + LED.OFF;
            }
        }

        public void GetValue(Mat mat)
        {
            string Value = "";
            foreach (SingleLED led in LEDs)
            {
                var output = led.Use ? led.TestImage(mat, true) : "0";
                Value = output.ToString() + Value;
            }
            CalculatorOutput = Convert.ToInt32(Value, 2);
        }

        public void GetValue()
        {
            string Value = "";
            foreach (SingleLED led in LEDs)
            {
                var output = led.IsPass ? "1" : "0";
                Value = output + Value;
            }
            CalculatorOutput = Convert.ToInt32(Value, 2);
        }
    }

    public class SingleLED : INotifyPropertyChanged
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
        public int ON { get; set; } = 250;
        public int OFF { get; set; } = 10;

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
                            elip.Fill = new SolidColorBrush(Colors.Yellow);
                            UseTurnOn = UseTurnOn;
                        }));
                    }
                    else
                    {
                        Visibility = Visibility.Hidden;
                        Label.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            Label.BorderBrush = new SolidColorBrush(Colors.Yellow);
                            Label.Foreground = new SolidColorBrush(Colors.Yellow);
                            elip.Fill = new SolidColorBrush(Colors.Gray);
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
                            elip.Fill = new SolidColorBrush(Colors.Green);

                        }));
                    }
                    else
                    {
                        Label.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            elip.Fill = new SolidColorBrush(Colors.Red);
                        }));
                    }
                }
            }
        }

        private int dir = 10;
        public int Dir
        {
            get { return dir; }
            set
            {
                dir = value;
                Rect recNew = Rect;
                recNew.Width = value;
                recNew.Height = value;
                Rect = recNew;
                OnPropertyChanged(nameof(Dir));
            }
        }

        [JsonIgnore]
        private bool _UseTurnOn;
        [JsonIgnore]
        public bool UseTurnOn
        {
            get { return _UseTurnOn; }
            set
            {
                if (value != _UseTurnOn)
                    _UseTurnOn = value;

                if (Use)
                    if (_UseTurnOn)
                    {
                        elip.Fill = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        elip.Fill = new SolidColorBrush(Colors.Yellow);
                    }
                OnPropertyChanged("UseTurnOn");
            }
        }
        [JsonIgnore]
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
                if (Use)
                    if (value)
                    {
                        elip.Fill = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        elip.Fill = new SolidColorBrush(Colors.Yellow);
                    }
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


        private Visibility _Visibility;
        public Visibility Visibility
        {
            get { return _Visibility; }
            set
            {
                if (value != _Visibility) _Visibility = value;
                Label.Visibility = value;
                OnPropertyChanged("Visibility");
            }
        }
        [JsonIgnore]
        public Grid grid = new Grid()
        {
            Background = null,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        [JsonIgnore]
        public Ellipse elip = new Ellipse()
        {
            Width = 18,
            Height = 18,
            Fill = new SolidColorBrush(Colors.Yellow),
        };
        [JsonIgnore]
        public Label Context = new Label()
        {
            Foreground = new SolidColorBrush(Colors.Red),
            Padding = new Thickness(0),
            FontSize = 9,
            Focusable = true,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center,
        };
        [JsonIgnore]
        public Label Label = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(1, 255, 0, 0)),
            Focusable = true,
            Padding = new Thickness(0),
            BorderThickness = new Thickness(0),
            Cursor = Cursors.Hand,
            VerticalContentAlignment = VerticalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            FontSize = 9
        };
        [JsonIgnore]
        public Image CropImageHolder = new Image();
        [JsonIgnore]
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
                        X = (int)value.X;
                        Width = (int)value.Width;
                        rect.X = value.X;
                        elip.Width = value.Width - 2;
                        rect.Width = value.Width;
                        Label.Width = value.Width + 2;
                    }

                    if (value.Y > 0 && value.Y < ParentCanvasSize.Height - value.Height)
                    {
                        Y = (int)value.Y;
                        Height = (int)value.Height;
                        rect.Y = value.Y;
                        elip.Height = value.Height - 2;
                        rect.Height = value.Height;
                        Label.Height = value.Height + 2;
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
                    Context.Content = name;
                    OnPropertyChanged(nameof(Name));
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
                    Label.MouseDoubleClick -= Label_MouseDoubleClick;
                }
            }
        }

        public SingleLED()
        {
            grid.Children.Add(elip);
            grid.Children.Add(Context);

            Label.Content = grid;

            Label.ToolTip = CropImageHolder;

            Label.GotKeyboardFocus += Label_GotKeyboardFocus;
            Label.LostKeyboardFocus += Label_LostKeyboardFocus;

            Label.KeyDown += Label_KeyDown;

            Label.MouseDown += Label_MouseDown;
            Label.MouseMove += Label_MouseMove;
            Label.MouseUp += Label_MouseUp;

            Label.MouseDoubleClick += Label_MouseDoubleClick;
        }

        public SingleLED(int index, System.Windows.Point startLocation)
        {
            rect = new Rect(index * 20 + startLocation.X, startLocation.Y, 20, 20);
            Name = "L" + (index + 1).ToString();
            Index = index;

            Label.ToolTip = CropImageHolder;

            Label.GotKeyboardFocus += Label_GotKeyboardFocus;
            Label.LostKeyboardFocus += Label_LostKeyboardFocus;

            Label.KeyDown += Label_KeyDown;


            dir = 20;
            Name = index.ToString();

            grid.Children.Add(elip);
            grid.Children.Add(Context);

            Label.Content = grid;

            Label.MouseDown += Label_MouseDown;
            Label.MouseMove += Label_MouseMove;
            Label.MouseUp += Label_MouseUp;
            Label.MouseDoubleClick += Label_MouseDoubleClick;
        }


        //Parent and positions

        public void SetParentCanvas(Canvas placeCanvas)
        {
            if (ParentCanvas != null)
            {
                ParentCanvas.Children.Remove(Label);
            }
            this.ParentCanvas = placeCanvas;

            Label.Width = rect.Width;
            Label.Height = rect.Height;

            Canvas.SetTop(this.Label, rect.Y);
            Canvas.SetLeft(this.Label, rect.X);

            placeCanvas.Children.Add(Label);

        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IsPass = !IsPass;
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


        private void Label_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {

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

        }

        private void Label_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

        }

        private void Label_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

        }

        public void SetPosition()
        {
            Canvas.SetTop(this.Label, rect.Y);
            Canvas.SetLeft(this.Label, rect.X);
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
                Mat gray = croppedMat.CvtColor(ColorConversionCodes.RGB2GRAY);
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
