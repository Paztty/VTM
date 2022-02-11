using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

namespace HVT.VTM.Base.VisionFunctions
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

        public Bitmap bitmap;

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
                    gLED.Visibility = value;
                }
            }
        }

        public ObservableCollection<SingleGLED> GLEDs { get; set; } = new ObservableCollection<SingleGLED>();

        public GLED() {
            CalculatorOutputString = "";
        }

        public GLED(Canvas manual, Canvas displayCanvas, Canvas placeCanvas, int index)
        {
            CalculatorOutputString = "";
            for (int i = 0; i < 32; i++)
            {
                GLEDs.Add(new SingleGLED(i, index, placeCanvas, displayCanvas, manual));
            }
        }

        public void GetValue(BitmapSource bitmap, double[] raito)
        {
            string Value = "";
            foreach (SingleGLED gLED in GLEDs)
            {
                gLED.raito = raito;
                var output = gLED.GetImage(bitmap);
                Value = output.ToString() + Value;
            }
            CalculatorOutput = Convert.ToInt32(Value, 2);
        }

        public void CALC_THRESH()
        {
            foreach (SingleGLED gLED in GLEDs)
            {
                gLED.Thresh = (int)(gLED.ON - gLED.OFF) / 3 * 2 + gLED.OFF;
            }
        }
    }

    public class SingleGLED : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private Canvas parent;
        private Canvas Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                parentSize = new Rect()
                {
                    X = 0,
                    Y = 0,
                    Width = value.ActualWidth,
                    Height = value.ActualHeight
                };
            }
        }

        private Canvas display;
        private Canvas Display
        {
            get { return display; }
            set
            {
                display = value;
                displaySize = new Rect()
                {
                    X = 0,
                    Y = 0,
                    Width = value.ActualWidth,
                    Height = value.ActualHeight
                };
            }
        }

        private Canvas manualDisplay;
        private Canvas ManualDisplay
        {
            get { return manualDisplay; }
            set
            {
                ManualDisplaySize = new Rect()
                {
                    X = 0,
                    Y = 0,
                    Width = value.ActualWidth,
                    Height = value.ActualHeight
                };
                manualDisplay = value;
            }
        }

        private Rect displaySize;
        public Rect DisplaySize
        {
            get { return displaySize; }
            set { displaySize = value; }
        }

        private Rect parentSize;
        public Rect ParentSize
        {
            get { return parentSize; }
            set { parentSize = value; }
        }
        private Rect manualdisplaySize;
        public Rect ManualDisplaySize
        {
            get { return manualdisplaySize; }
            set { manualdisplaySize = value; }
        }


        public int Index { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ON { get; set; } = 250;
        public int OFF { get; set; } = 10;

        private int thresh = 180;
        public int Thresh
        {
            get { return thresh; }
            set
            {
                thresh = value;
                NotifyPropertyChanged(nameof(Thresh));
            }
        }


        private int intens = 180;

        public int Intens
        {
            get { return intens; }
            set
            {
                intens = value;
                if (use)
                {
                    if (value >= Thresh)
                    {
                        Label.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            Label.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                            Label.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                        }));
                        LabelDisplay.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            LabelDisplay.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                            LabelDisplay.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                        }));
                        ManualLabelDisplay.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            ManualLabelDisplay.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                            ManualLabelDisplay.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                        }));
                    }
                    else
                    {
                        Label.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            Label.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                            Label.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        }));
                        LabelDisplay.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            LabelDisplay.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                            LabelDisplay.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        }));
                        ManualLabelDisplay.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            ManualLabelDisplay.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                            ManualLabelDisplay.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        }));
                    }
                }
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
                    NotifyPropertyChanged(nameof(Use));
                    if (use)
                    {
                        //Visibility = Visibility.Visible;
                        this.Label.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            Label.BorderBrush = new SolidColorBrush(Colors.Red);
                            Label.Foreground = new SolidColorBrush(Colors.Red);
                        }));
                    }
                    else
                    {
                        //Visibility = Visibility.Hidden;
                        Label.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            Label.BorderBrush = new SolidColorBrush(Colors.Yellow);
                            Label.Foreground = new SolidColorBrush(Colors.Yellow);
                        }));
                    }
                }
            }

        }


        private double[] rt = new double[2] { 1, 1 };
        public double[] raito
        {
            get { return rt; }
            set
            {
                if (value.Length == 2)
                {
                    rt = value;
                    Area = new Int32Rect((int)(rect.X * value[0]), (int)(rect.Y * value[1]), (int)(rect.Width * value[0]), (int)(rect.Height * value[1]));
                }
            }
        }
        private Int32Rect area = new Int32Rect(0, 0, 10, 10);
        public Int32Rect Area
        {
            get { return area; }
            set
            {
                area = value;
                X = area.X;
                Y = area.Y;
                Width = area.Width;
                Height = area.Height;
                NotifyPropertyChanged(nameof(X));
                NotifyPropertyChanged(nameof(Y));
                NotifyPropertyChanged(nameof(Width));
                NotifyPropertyChanged(nameof(Height));
            }
        }

        private string data;

        public string Data
        {
            get { return data; }
            private set { data = value; }
        }

        public double Threshold { get; set; }

        private Visibility visibility;
        public Visibility Visibility
        {
            get { return visibility; }
            set
            {
                visibility = value;
                Label.Visibility = value;
                if (value == Visibility.Visible)
                {
                    LabelDisplay.Visibility = use ? Visibility.Visible : Visibility.Collapsed;
                    ManualLabelDisplay.Visibility = use ? Visibility.Visible : Visibility.Collapsed;
                }
                else if (value != Visibility.Visible)
                {
                    LabelDisplay.Visibility = value;
                    ManualLabelDisplay.Visibility = value;
                    Keyboard.ClearFocus();
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
                    NotifyPropertyChanged(nameof(Name));
                }
            }
        }

        public Label ManualLabelDisplay = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(1, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.Red),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Focusable = true,
            VerticalContentAlignment = VerticalAlignment.Top,
            HorizontalContentAlignment = HorizontalAlignment.Left,
        };

        public Label LabelDisplay = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(1, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.Red),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Focusable = true,
            Cursor = Cursors.SizeAll,
            VerticalContentAlignment = VerticalAlignment.Top,
            HorizontalContentAlignment = HorizontalAlignment.Left,
        };

        public Label Label = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(1, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.Yellow),
            BorderBrush = new SolidColorBrush(Colors.Yellow),
            BorderThickness = new Thickness(1, 1, 1, 1),
            Padding = new Thickness(1, 1, 1, 1),
            FontSize = 10,
            Focusable = true,
            Cursor = Cursors.SizeAll,
            VerticalContentAlignment = VerticalAlignment.Top,
            HorizontalContentAlignment = HorizontalAlignment.Left,
        };

        public Label LabelTopLeft = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeNWSE,
            Focusable = true,
        };
        public Label LabelTopMid = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeNS,
            Focusable = true,
        };
        public Label LabelTopRight = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeNESW,
            Focusable = true,
        };
        public Label LabelMidLeft = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeWE,
            Focusable = true,
        };
        public Label LabelMidRight = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderThickness = new Thickness(1),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeWE,
            Focusable = true,
        };
        public Label LabelBotLeft = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderThickness = new Thickness(1),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeNESW,
            Focusable = true,
        };
        public Label LabelBotMid = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderThickness = new Thickness(1),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeNS,
            Focusable = true,
        };
        public Label LabelBotRight = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderThickness = new Thickness(1),
            Width = 5,
            Height = 5,
            Cursor = Cursors.SizeNWSE,
            Focusable = true,
        };

        private Rect OfsetMove = new Rect();

        private Rect rect = new Rect()
        {
            Location = new System.Windows.Point(0, 0),
            Size = new System.Windows.Size(10, 10)
        };

        private Rect rectDisplay = new Rect()
        {
            Location = new System.Windows.Point(0, 0),
            Size = new System.Windows.Size(10, 10)
        };
        public Rect manualRectDisplay = new Rect()
        {
            Location = new System.Windows.Point(0, 0),
            Size = new System.Windows.Size(10, 10)
        };

        public Rect Rect
        {
            get { return rect; }
            set
            {
                if (rect != value)
                {
                    if (value.X > 0 && value.X < parentSize.Width - value.Width)
                    {
                        rect.X = value.X;
                        rectDisplay.X = value.X * (displaySize.Width / parentSize.Width);
                        manualRectDisplay.X = value.X * (ManualDisplaySize.Width / parentSize.Width);
                        rect.Width = value.Width;
                        rectDisplay.Width = value.Width * (displaySize.Width / parentSize.Width);
                        manualRectDisplay.Width = value.Width * (ManualDisplaySize.Width / parentSize.Width);
                        Label.Width = value.Width;
                        LabelDisplay.Width = value.Width * (displaySize.Width / parentSize.Width);
                        ManualLabelDisplay.Width = value.Width * (ManualDisplaySize.Width / parentSize.Width);
                    }

                    if (value.Y > 0 && value.Y < parentSize.Height - value.Height)
                    {
                        rect.Y = value.Y;
                        rectDisplay.Y = value.Y * (displaySize.Height / parentSize.Height);
                        manualRectDisplay.Y = value.Y * (ManualDisplaySize.Height / parentSize.Height);

                        rect.Height = value.Height;
                        rectDisplay.Height = value.Height * (displaySize.Height / parentSize.Height);
                        manualRectDisplay.Height = value.Height * (ManualDisplaySize.Height / parentSize.Height);

                        Label.Height = value.Height;
                        LabelDisplay.Height = value.Height * (displaySize.Height / parentSize.Height);
                        ManualLabelDisplay.Height = value.Height * (ManualDisplaySize.Height / parentSize.Height);
                    }
                    Area = new Int32Rect((int)(rect.X * raito[0]), (int)(rect.Y * raito[1]), (int)(rect.Width * raito[0]), (int)(rect.Height * raito[1]));
                }
            }
        }

        public SingleGLED() { }
        public SingleGLED(int index, int yIndex, Canvas parent, Canvas Display, Canvas ManualDisplay)
        {
            this.Parent = parent;
            this.Display = Display;
            this.ManualDisplay = ManualDisplay;

            Name = index.ToString();

            Index = index;
            this.Rect = new Rect()
            {
                X = index * 20 + 2,
                Y = yIndex < 2 ? parent.ActualHeight / 10 * yIndex + 2 : parent.ActualHeight - (parent.ActualHeight / 10 * (4 - yIndex) + 2),
                Width = 20,
                Height = 20,
            };

            Label.GotKeyboardFocus += Label_GotKeyboardFocus;
            Label.LostKeyboardFocus += Label_LostKeyboardFocus;

            Label.MouseDown += Label_MouseDown;
            Label.MouseMove += Label_MouseMove;
            Label.MouseUp += Label_MouseUp;

            Label.KeyDown += Label_KeyDown;

            Use = false;

            Visibility = Use ? Visibility.Visible : Visibility.Collapsed;

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

        public void ReInit(Canvas parent, Canvas Display, Canvas manualDisplay)
        {
            this.Parent = parent;
            this.Display = Display;
            this.ManualDisplay = manualDisplay;

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

        }


        private void LabelResize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(sender as Label);
            Keyboard.Focus(sender as Label);
            e.Handled = true;
        }

        private void LabelTopRight_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Console.WriteLine("Label top right event");
                Rect areaRect = Rect;
                areaRect.Width = Math.Abs(e.GetPosition(Parent).X - rect.X);
                areaRect.Height = Math.Abs(areaRect.Height + rect.Y - e.GetPosition(Parent).Y);
                areaRect.Y = e.GetPosition(Parent).Y;
                Rect = areaRect;
                SetPosition();
            }
        }

        private void LabelTopMid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
                areaRect.Height = Math.Abs(areaRect.Height + rect.Y - e.GetPosition(Parent).Y);
                areaRect.Y = e.GetPosition(Parent).Y;
                Rect = areaRect;
                SetPosition();
            }
        }

        private void LabelTopLeft_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
                areaRect.Width = Math.Abs(areaRect.Width + rect.X - e.GetPosition(Parent).X);
                areaRect.Height = Math.Abs(areaRect.Height + rect.Y - e.GetPosition(Parent).Y);
                areaRect.Y = e.GetPosition(Parent).Y;
                areaRect.X = e.GetPosition(Parent).X;
                Rect = areaRect;
                SetPosition();
            }
        }

        private void LabelMidRight_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
                areaRect.Width = Math.Abs(e.GetPosition(Parent).X - areaRect.X);
                //areaRect.Height = Math.Abs(areaRect.Height + rect.Y - e.GetPosition(Parent).Y);
                //areaRect.Y = e.GetPosition(Parent).Y;
                //areaRect.X = e.GetPosition(Parent).X;
                Rect = areaRect;
                SetPosition();
            }
        }

        private void LabelMidLeft_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
                areaRect.Width = Math.Abs(areaRect.Width + rect.X - e.GetPosition(Parent).X);
                //areaRect.Height = Math.Abs(areaRect.Height + rect.Y - e.GetPosition(Parent).Y);
                //areaRect.Y = e.GetPosition(Parent).Y;
                areaRect.X = e.GetPosition(Parent).X;
                Rect = areaRect;
                SetPosition();
            }
        }

        private void LabelBotRight_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
                areaRect.Width = Math.Abs(e.GetPosition(Parent).X - rect.X);
                areaRect.Height = Math.Abs(e.GetPosition(Parent).Y - rect.Y);
                Rect = areaRect;
                SetPosition();
            }
        }

        private void LabelBotMid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
                //areaRect.Width = Math.Abs(areaRect.Width + rect.X - e.GetPosition(Parent).X);
                areaRect.Height = Math.Abs(e.GetPosition(Parent).Y - areaRect.Y);
                //areaRect.Y = e.GetPosition(Parent).Y;
                //areaRect.X = e.GetPosition(Parent).X;
                Rect = areaRect;
                SetPosition();
            }
        }

        private void LabelBotLeft_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                Rect areaRect = Rect;
                areaRect.Width = Math.Abs(areaRect.Width + rect.X - e.GetPosition(Parent).X);
                areaRect.Height = Math.Abs(e.GetPosition(Parent).Y - rect.Y);
                //areaRect.Y = e.GetPosition(Parent).Y;
                areaRect.X = e.GetPosition(Parent).X;
                Rect = areaRect;
                SetPosition();
            }
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
                    Width = Math.Max(Math.Abs(e.GetPosition(Parent).X - rect.X), 5),
                    Height = Math.Max(Math.Abs(e.GetPosition(Parent).Y - rect.Y), 5),
                };
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
                areaRect.X = e.GetPosition(Parent).X - OfsetMove.Width;
                areaRect.Y = e.GetPosition(Parent).Y - OfsetMove.Height;
                Rect = areaRect;
                SetPosition();
            }

            if (e.LeftButton == MouseButtonState.Pressed && (e.Source as FrameworkElement) == Label)
            {
                var focusElement = Keyboard.FocusedElement;
                if (focusElement != null && focusElement.GetType() == typeof(Label))
                {
                    Console.WriteLine(sender.ToString() + " + " + focusElement.ToString());
                    focusElement.RaiseEvent(e);
                    Console.WriteLine("Fire event to focus Element");
                    //Canvas.SetTop((Label)focusElement, e.GetPosition(DrawingCanvas).Y);
                    //Canvas.SetRight((Label)focusElement, e.GetPosition(DrawingCanvas).X);
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
            Keyboard.Focus(sender as Label);
            LabelBotLeft.Visibility = Visibility.Visible;
            LabelBotMid.Visibility = Visibility.Visible;
            LabelBotRight.Visibility = Visibility.Visible;
            LabelMidLeft.Visibility = Visibility.Visible;
            LabelMidRight.Visibility = Visibility.Visible;
            LabelTopLeft.Visibility = Visibility.Visible;
            LabelTopMid.Visibility = Visibility.Visible;
            LabelTopRight.Visibility = Visibility.Visible;
        }

        public void PlaceIn(Canvas placeCanvas, Canvas displayCanvas, Canvas manualCanvasDisplay)
        {
            Parent = placeCanvas;
            Display = displayCanvas;
            ManualDisplay = manualCanvasDisplay;


            manualCanvasDisplay.Children.Add(ManualLabelDisplay);
            Canvas.SetTop(this.ManualLabelDisplay, manualRectDisplay.Y);
            Canvas.SetLeft(this.ManualLabelDisplay, manualRectDisplay.X);

            Canvas.SetTop(this.LabelDisplay, rectDisplay.Y);
            Canvas.SetLeft(this.LabelDisplay, rectDisplay.X);

            placeCanvas.Children.Add(Label);
            displayCanvas.Children.Add(LabelDisplay);

            placeCanvas.Children.Add(LabelTopLeft);
            placeCanvas.Children.Add(LabelTopMid);
            placeCanvas.Children.Add(LabelTopRight);

            placeCanvas.Children.Add(LabelMidLeft);
            placeCanvas.Children.Add(LabelMidRight);

            placeCanvas.Children.Add(LabelBotLeft);
            placeCanvas.Children.Add(LabelBotMid);
            placeCanvas.Children.Add(LabelBotRight);

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
        public void SetPosition()
        {
            //rect.X = e.GetPosition(placeCanvas).X;
            //rect.Y = e.GetPosition(placeCanvas).Y;

            Canvas.SetTop(this.LabelDisplay, rectDisplay.Y);
            Canvas.SetLeft(this.LabelDisplay, rectDisplay.X);

            Canvas.SetTop(this.ManualLabelDisplay, manualRectDisplay.Y);
            Canvas.SetLeft(this.ManualLabelDisplay, manualRectDisplay.X);

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
        public string GetImage(BitmapSource source)
        {
            if (source != null && Use)
            {

                Intens = VisionWorker.Meansure(area, source, Thresh);
                NotifyPropertyChanged(nameof(Intens));
                if (Intens >= Thresh)
                {
                    return "1";
                }
            }
            return "0";
        }
    }
}
