using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

namespace HVT.VTM.Base.VisionFunctions
{
    public class LCD
    {
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
        private Int32Rect area = new Int32Rect(0, 0, 100, 50);
        public Int32Rect Area
        {
            get { return area; }
            set
            {
                area = value;

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

                //LabelBotLeft.Visibility = value;
                //LabelBotMid.Visibility = value;
                //LabelBotRight.Visibility = value;
                //LabelMidLeft.Visibility = value;
                //LabelMidRight.Visibility = value;
                //LabelTopLeft.Visibility = value;
                //LabelTopMid.Visibility = value;
                //LabelTopRight.Visibility = value;

                if (value != Visibility.Visible)
                {
                    Keyboard.ClearFocus();
                }
            }
        }

        public Image Image = new Image();

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
            Foreground = new SolidColorBrush(Colors.Red),
            BorderBrush = new SolidColorBrush(Colors.Red),
            BorderThickness = new Thickness(1),
            Focusable = true,
            Cursor = Cursors.SizeAll,
            VerticalContentAlignment = VerticalAlignment.Top,
            HorizontalContentAlignment = HorizontalAlignment.Left,
        };
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

        private Rect OfsetMove = new Rect();

        private Rect rect = new Rect()
        {
            Location = new Point(0, 0),
            Size = new System.Windows.Size(10, 10)
        };

        private Rect rectDisplay = new Rect()
        {
            Location = new Point(0, 0),
            Size = new System.Windows.Size(10, 10)
        };

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
                        rect.Width = value.Width;
                        rectDisplay.Width = value.Width * (displaySize.Width / parentSize.Width);
                        Label.Width = value.Width;
                        LabelDisplay.Width = value.Width * (displaySize.Width / parentSize.Width);
                    }

                    if (value.Y > 0 && value.Y < parentSize.Height - value.Height)
                    {
                        rect.Y = value.Y;
                        rectDisplay.Y = value.Y * (displaySize.Height / parentSize.Height);
                        rect.Height = value.Height;
                        rectDisplay.Height = value.Height * (displaySize.Height / parentSize.Height);
                        Label.Height = value.Height;
                        LabelDisplay.Height = value.Height * (displaySize.Height / parentSize.Height);
                    }
                    Area = new Int32Rect((int)(value.X * raito[0]), (int)(value.Y * raito[1]), (int)(value.Width * raito[0]), (int)(value.Height * raito[1]));
                }
            }
        }

        public LCD() { }

        public LCD(int index, string context, Canvas parent, Canvas Display)
        {
            this.Parent = parent;
            this.Display = Display;

            Name = context;
            this.Rect = new Rect()
            {
                X = parent.ActualWidth / 3 - 50,
                Y = parent.ActualHeight / 8 * (2 * index - 1) - 25,
                Width = 100,
                Height = 50,
            };

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

        public void ReInit(Canvas parent, Canvas Display)
        {
            this.Parent = parent;
            this.Display = Display;

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
            LabelBotLeft.Visibility = Visibility.Visible;
            LabelBotMid.Visibility = Visibility.Visible;
            LabelBotRight.Visibility = Visibility.Visible;
            LabelMidLeft.Visibility = Visibility.Visible;
            LabelMidRight.Visibility = Visibility.Visible;
            LabelTopLeft.Visibility = Visibility.Visible;
            LabelTopMid.Visibility = Visibility.Visible;
            LabelTopRight.Visibility = Visibility.Visible;
        }

        public void PlaceIn(Canvas placeCanvas, Canvas displayCanvas)
        {
            Parent = placeCanvas;
            Display = displayCanvas;

            Canvas.SetTop(this.LabelDisplay, rectDisplay.Y);
            Canvas.SetLeft(this.LabelDisplay, rectDisplay.X);

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

            displayCanvas.Children.Add(LabelDisplay);

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
        public void SetPosition()
        {
            //rect.X = e.GetPosition(placeCanvas).X;
            //rect.Y = e.GetPosition(placeCanvas).Y;

            Canvas.SetTop(this.LabelDisplay, rectDisplay.Y);
            Canvas.SetLeft(this.LabelDisplay, rectDisplay.X);

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
        public void GetImage(BitmapSource source)
        {
            if (source != null)
            {
                this.bitmap = VisionWorker.GetBitmap(source);
            }
        }
        public void SetImage(System.Windows.Controls.Image control, Label detectedResultlabel)
        {
            if (bitmap != null && control != null)
            {
                string str = "";
                var result = VisionWorker.DetectString(bitmap, out str);
                var imageSource = VisionWorker.Convert(result);
                Data = str.Replace("\r", "").Replace("\n","");
                control.Dispatcher.BeginInvoke(new Action(() => control.Source = imageSource));
                detectedResultlabel.Content = str;
            }

        }
    }
}
