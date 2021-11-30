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
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;

namespace HVT.VTM.Base.VisionFunctions
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
                foreach (SingleLED gLED in LEDs)
                {
                    gLED.Visibility = value;
                }
            }
        }

        public ObservableCollection<SingleLED> LEDs { get; set; } = new ObservableCollection<SingleLED>();

        public LED() { }

        public LED(Canvas displayCanvas, Canvas placeCanvas, int index)
        {
            for (int i = 0; i < 32; i++)
            {
                LEDs.Add(new SingleLED(i, index, placeCanvas, displayCanvas));
            }
        }

        public void GetValue(BitmapSource bitmap, double[] raito)
        {
            string Value = "";
            foreach (SingleLED LED in LEDs)
            {
                LED.raito = raito;
                var output = LED.GetImage(bitmap);
                Value = output.ToString() + Value;
            }
            CalculatorOutput = Convert.ToInt32(Value, 2);
        }

        public void CALC_THRESH()
        {
            foreach (SingleLED LED in LEDs)
            {
                LED.Thresh = (int)(LED.ON - LED.OFF) / 3 * 2 + LED.OFF;
            }
        }
    }

    public class SingleLED : INotifyPropertyChanged
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
        private Bitmap bitmap = null;

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


        public int Index { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ON { get; set; } = 250;
        public int OFF { get; set; } = 10;

        private int dir = 10;
        public int Dir
        {
            get { return dir; }
            set
            {
                dir = value;
                elip.Width = value - 2;
                elip.Height = value - 2;

                elipDisplay.Width = value * (displaySize.Width / parentSize.Width) - 2;
                elipDisplay.Height = value * (displaySize.Width / parentSize.Width) - 2;

                Rect recNew = Rect;
                recNew.Width = value;
                recNew.Height = value;
                Rect = recNew;
                NotifyPropertyChanged(nameof(Dir));
            }
        }

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
                Context.Dispatcher.BeginInvoke(new Action(() => Context.Foreground = new SolidColorBrush(Colors.Yellow)));
                ContextDisplay.Dispatcher.BeginInvoke(new Action(() => ContextDisplay.Foreground = new SolidColorBrush(Colors.Yellow)));
                if (use)
                {
                    if (value >= Thresh)
                    {
                        Label.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            elipDisplay.Fill = new SolidColorBrush(Colors.Green);
                            elip.Fill = new SolidColorBrush(Colors.Green);

                        }));
                    }
                    else
                    {
                        Label.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            elipDisplay.Fill = new SolidColorBrush(Colors.Red);
                            elip.Fill = new SolidColorBrush(Colors.Red);
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
                use = value;
                NotifyPropertyChanged(nameof(Use));
                if (use)
                {
                    Visibility = Visibility.Visible;
                    Context.Dispatcher.BeginInvoke(new Action(() => Context.Foreground = new SolidColorBrush(Colors.Yellow)));
                    ContextDisplay.Dispatcher.BeginInvoke(new Action(() => ContextDisplay.Foreground = new SolidColorBrush(Colors.Yellow)));
                    Label.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        elipDisplay.Fill = new SolidColorBrush(Colors.Red);
                        elip.Fill = new SolidColorBrush(Colors.Red);
                    }));
                }
                else
                {
                    Visibility = Visibility.Hidden;
                    Context.Dispatcher.BeginInvoke(new Action(() => Context.Foreground = new SolidColorBrush(Colors.Red)));
                    ContextDisplay.Dispatcher.BeginInvoke(new Action(() => ContextDisplay.Foreground = new SolidColorBrush(Colors.Red)));
                    Label.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        elipDisplay.Fill = new SolidColorBrush(Colors.Yellow);
                        elip.Fill = new SolidColorBrush(Colors.Yellow);
                    }));
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
                if (value == Visibility.Visible)
                {
                    LabelDisplay.Visibility = use ? Visibility.Visible : Visibility.Collapsed;
                }

                if (value != Visibility.Visible)
                {
                    LabelDisplay.Visibility = value;
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
                    Context.Content = name;
                    ContextDisplay.Content = name;
                    NotifyPropertyChanged(nameof(Name));
                }
            }
        }

        public Label Label = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(1, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.Red),
            Padding = new Thickness(0),
            FontSize = 9,
            Focusable = true,
            Cursor = Cursors.SizeAll,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center,
        };

        public Grid grid = new Grid()
        {
            Background = null,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch

        };

        public Ellipse elip = new Ellipse()
        {
            Width = 10,
            Height = 10,
            Fill = new SolidColorBrush(Colors.Yellow),

        };
        public Label Context = new Label()
        {
            Foreground = new SolidColorBrush(Colors.Red),
            Padding = new Thickness(0),
            FontSize = 9,
            Focusable = true,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center,
        };


        public Label LabelDisplay = new Label()
        {
            Background = new SolidColorBrush(Color.FromArgb(1, 255, 0, 0)),
            Foreground = new SolidColorBrush(Colors.Red),
            Padding = new Thickness(0),
            FontSize = 9,
            Focusable = true,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center,
        };
        public Grid gridDisplay = new Grid()
        {
            Background = null,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch

        };
        public Ellipse elipDisplay = new Ellipse()
        {
            Width = 10,
            Height = 10,
            Fill = new SolidColorBrush(Colors.Yellow),

        };
        public Label ContextDisplay = new Label()
        {
            Foreground = new SolidColorBrush(Colors.Red),
            Padding = new Thickness(0),
            FontSize = 9,
            Focusable = true,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center,
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



        public Rect Rect
        {
            get { return rect; }
            set
            {
                if (rect != value)
                {
                    if (value.X > 0 && value.X < parentSize.Width - value.Width)
                    {
                        elip.Width = value.Width;
                        elipDisplay.Width = value.Width * (displaySize.Width / parentSize.Width);

                        rect.X = value.X;
                        rectDisplay.X = value.X * (displaySize.Width / parentSize.Width);
                        rect.Width = value.Width;
                        rectDisplay.Width = value.Width * (displaySize.Width / parentSize.Width);
                        Label.Width = value.Width;
                        LabelDisplay.Width = value.Width * (displaySize.Width / parentSize.Width);
                    }

                    if (value.Y > 0 && value.Y < parentSize.Height - value.Height)
                    {
                        elip.Height = value.Height;
                        elipDisplay.Height = value.Height * (displaySize.Height / parentSize.Height);

                        rect.Y = value.Y;
                        rectDisplay.Y = value.Y * (displaySize.Height / parentSize.Height);
                        rect.Height = value.Height;
                        rectDisplay.Height = value.Height * (displaySize.Height / parentSize.Height);
                        Label.Height = value.Height;
                        LabelDisplay.Height = value.Height * (displaySize.Height / parentSize.Height);
                    }
                    Area = new Int32Rect((int)(rect.X * raito[0]), (int)(rect.Y * raito[1]), (int)(rect.Width * raito[0]), (int)(rect.Height * raito[1]));
                }
            }
        }

        public SingleLED() { }

        public SingleLED(int index, int yIndex, Canvas parent, Canvas Display)
        {
            this.Parent = parent;
            this.Display = Display;
            Index = index;
            this.Rect = new Rect()
            {
                X = index * 20 + 2,
                Y = parent.ActualHeight / 4 * yIndex + 2 + 25,
                Width = 15,
                Height = 15,
            };
            dir = 15;
            Name = index.ToString();

            grid.Children.Add(elip);
            grid.Children.Add(Context);

            gridDisplay.Children.Add(elipDisplay);
            gridDisplay.Children.Add(ContextDisplay);

            LabelDisplay.Content = gridDisplay;
            Label.Content = grid;

            Label.GotKeyboardFocus += Label_GotKeyboardFocus;
            Label.LostKeyboardFocus += Label_LostKeyboardFocus;

            Label.MouseDown += Label_MouseDown;
            Label.MouseMove += Label_MouseMove;
            Label.KeyDown += Label_KeyDown;

            Visibility = Use ? Visibility.Visible : Visibility.Collapsed;
        }

        public void ReInit(Canvas parent, Canvas Display)
        {
            this.Parent = parent;
            this.Display = Display;
            grid.Children.Add(elip);
            grid.Children.Add(Context);

            gridDisplay.Children.Add(elipDisplay);
            gridDisplay.Children.Add(ContextDisplay);

            LabelDisplay.Content = gridDisplay;
            Label.Content = grid;

            Label.GotKeyboardFocus += Label_GotKeyboardFocus;
            Label.LostKeyboardFocus += Label_LostKeyboardFocus;

            Label.MouseDown += Label_MouseDown;
            Label.MouseMove += Label_MouseMove;
            Label.KeyDown += Label_KeyDown;

            Visibility = Use ? Visibility.Visible : Visibility.Collapsed;
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

        private void Label_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Label.BorderBrush = null;
            Label.BorderThickness = new Thickness(0);
        }

        private void Label_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Label.BorderBrush = new SolidColorBrush(Colors.GreenYellow);
            Label.BorderThickness = new Thickness(2);
        }

        public void PlaceIn(Canvas placeCanvas, Canvas displayCanvas)
        {
            Parent = placeCanvas;
            Display = displayCanvas;

            Canvas.SetTop(this.LabelDisplay, rectDisplay.Y);
            Canvas.SetLeft(this.LabelDisplay, rectDisplay.X);

            displayCanvas.Children.Add(LabelDisplay);

            placeCanvas.Children.Add(Label);

            Canvas.SetTop(this.Label, rect.Y);
            Canvas.SetLeft(this.Label, rect.X);
        }
        public void SetPosition()
        {
            //rect.X = e.GetPosition(placeCanvas).X;
            //rect.Y = e.GetPosition(placeCanvas).Y;
            Canvas.SetTop(this.LabelDisplay, rectDisplay.Y);
            Canvas.SetLeft(this.LabelDisplay, rectDisplay.X);

            Canvas.SetTop(this.Label, rect.Y);
            Canvas.SetLeft(this.Label, rect.X);

        }
        public string GetImage(BitmapSource source)
        {
            if (source != null && Use)
            {
                this.bitmap = VisionWorker.GetBitmap(source);
                if (bitmap != null)
                {
                    Intens = VisionWorker.Meansure(area, bitmap, Thresh);
                    NotifyPropertyChanged(nameof(Intens));
                    if (Intens >= Thresh)
                    {
                        return "1";
                    }
                }
            }
            return "0";
        }
    }
}
