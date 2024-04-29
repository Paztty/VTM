using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HVT.Controls
{
    /// <summary>
    /// Interaction logic for LevelDataViewer.xaml
    /// </summary>
    public partial class LevelDataViewer : UserControl
    {
        private SerialPortDisplay _SerialPort = new SerialPortDisplay();
        public SerialPortDisplay SerialPort
        {
            get { return _SerialPort; }
            set
            {
                if (value != null || value != _SerialPort)
                    _SerialPort = value;
            }
        }

        private LevelCard _Card = new LevelCard();
        public LevelCard Card
        {
            get { return _Card; }
            set
            {
                if (value != null || value != _Card)
                {
                    _Card.Sampling.Stop();
                    _Card = value;
                    this.DataContext = _Card;
                    _Card.SerialPort = SerialPort;
                    if (_contentLoaded)
                    {
                        MatrixViewer.Children.Clear();
                        ChPanel.Children.Clear();
                        GraphPanel.Children.Clear();
                        _Card.SampleCountChange += _Card_SampleCountChange;
                        foreach (var item in _Card.Chanels)
                        {
                            MatrixViewer.Children.Add(item.btOn);

                            item.chartPanel.Background = 
                                (GraphPanel.Children.Count % 2) == 0 ?
                                new SolidColorBrush(Color.FromRgb(30, 30, 30)) :
                                new SolidColorBrush(Color.FromRgb(20, 20, 20));

                            item.chartLabel.Background = new SolidColorBrush(Color.FromRgb(20, 20, 20));
                            if (item.IsUse) ChPanel.Children.Add(item.chartLabel);
                            if (!item.IsUse) item.chartLabel.Background = new SolidColorBrush(Color.FromRgb(50, 50, 50));
                            if (item.IsUse) GraphPanel.Children.Add(item.chartPanel);
                        }
                    }
                }
            }
        }



        public LevelDataViewer()
        {
            InitializeComponent();
            SerialPort.DevieceName = "LEVEL";
            SerialPort.Port = new System.IO.Ports.SerialPort
            {
                PortName = "COM1",
                BaudRate = 9600,
                DataBits = 8,
                Parity = System.IO.Ports.Parity.None,
                StopBits = System.IO.Ports.StopBits.One
            };
            Card.SerialPort = SerialPort;

            this.DataContext = _Card;
            _Card.SampleCountChange += _Card_SampleCountChange;
            MatrixViewer.Children.Clear();
            ChPanel.Children.Clear();
            SamplePanel.Children.Clear();
            GraphPanel.Children.Clear();
            foreach (var item in _Card.Chanels)
            {
                MatrixViewer.Children.Add(item.btOn);
                item.chartPanel.Background = (GraphPanel.Children.Count % 2) == 0 ? new SolidColorBrush(Color.FromRgb(30, 30, 30)) : new SolidColorBrush(Color.FromRgb(20, 20, 20));
                item.chartLabel.Background = new SolidColorBrush(Color.FromRgb(20, 20, 20));
                if (item.IsUse) ChPanel.Children.Add(item.chartLabel);
                if (item.IsUse) GraphPanel.Children.Add(item.chartPanel);
                if (!item.IsUse) item.chartLabel.Background = new SolidColorBrush(Color.FromRgb(50, 50, 50));

            }

            var group = new TransformGroup();
            group.Children.Add(new RotateTransform() { Angle = 315 });
            for (int i = 0; i < 200; i++)
            {
                var sampleLabel = new Label()
                {
                    Content = (i * 10).ToString(),
                    Foreground = new SolidColorBrush(Colors.White),
                    FontSize = 10,
                    Width = 25,
                    Padding = new Thickness(0),
                    HorizontalContentAlignment = HorizontalAlignment.Right,
                };
                sampleLabel.RenderTransform = group;
                sampleLabel.RenderTransformOrigin = new Point(1, 0.5);
                SamplePanel.Children.Add(sampleLabel);
            }
        }

        public async void CheckCardComunication(string COMNAME)
        {
            byte[] cardChannel = new byte[1];

            cardChannel[0] = (byte)0x4C;

            bool result = await SerialPort.CheckBoardComPort(COMNAME, 9600, cardChannel, null, 250);
        }

        private void HorizontalScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            GraphView.ScrollToHorizontalOffset((sender as ScrollViewer).HorizontalOffset);
        }

        private void GraphView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            channelpanel.ScrollToVerticalOffset((sender as ScrollViewer).VerticalOffset);
        }

        private void _Card_SampleCountChange(object sender, EventArgs e)
        {
            if (_Card.SampleCount > 200 && _Card.SampleCount < 1800)
            {
                SampleViewer.ScrollToHorizontalOffset(_Card.SampleCount * 2.5 - 500);
            }
        }

        private void VerticalScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            GraphView.ScrollToVerticalOffset((sender as ScrollViewer).VerticalOffset);
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            _Card.AllowGetSample = true;
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            _Card.AllowGetSample = false;
        }

        public void StartGetSample(int Interval)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                Card.Sampling.Interval = TimeSpan.FromMilliseconds(Interval);
                Card.AllowGetSample = true;
            }
            ));
        }

        public void StopGetSample()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                Card.AllowGetSample = false;
            }
            ));
        }
    }
}
