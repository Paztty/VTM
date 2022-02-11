using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HVT.VTM.Base
{
    public class LevelChannel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<LevelSample> Samples = new List<LevelSample>();

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler STATE_CHANGE;
        private void StateChanged()
        {
            STATE_CHANGE?.Invoke(null, null);
        }

        private int channel;
        public int Channel
        {
            get { return channel; }
            set
            {
                channel = value;
                CbUse.Content = value.ToString();
                chartLabel.Content = value.ToString();

            }
        }

        public CheckBox CbUse = new CheckBox()
        {
            Content = "Ch",
            IsChecked = false,
        };

        public Label chartLabel = new Label
        {
            Content = "Ch",
            HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right,
            FontSize = 9,
            Foreground = new SolidColorBrush(Colors.White),
            Background = new SolidColorBrush(Colors.Black),
            Margin = new System.Windows.Thickness(0, 0, 2, 0),
            Height = 20
        };

        public Label lbbackGround = new Label
        {
            HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
            Margin = new System.Windows.Thickness(0, 0, 2, 0),
            Height = 20
        };

        public DockPanel chartPanel = new DockPanel()
        {
            Height = 20,
            MinWidth = 1000,
            LastChildFill = true,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
        };
        public PointCollection polygonPoints = new PointCollection();
        public Polyline CharPolyline = new Polyline()
        {
            Stroke = new SolidColorBrush(Colors.Red),
            StrokeThickness = 1,
        };


        public int CardID = 0x4D;

        private bool _isUse = false;
        public bool IsUse
        {
            get { return _isUse; }
            set
            {
                if (_isUse != value)
                {
                    _isUse = value;
                    CbUse.Dispatcher.Invoke(new Action(() => CbUse.IsChecked = _isUse));
                    chartLabel.Dispatcher.Invoke(new Action(() =>
                    {
                        chartLabel.Visibility = value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                        chartPanel.Visibility = value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                        chartPanel.Height = chartLabel.Height;
                    }));
                    NotifyPropertyChanged("isUse");
                }
            }
        }

        public LevelChannel()
        {
            CbUse.Checked += CbUse_Checked;
            CbUse.Unchecked += CbUse_Unchecked;
            chartPanel.Children.Clear();
            chartPanel.Children.Add(CharPolyline);
        }

        private void CbUse_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsUse = false;
            //NotifyPropertyChanged("isUse");
        }

        private void CbUse_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsUse = true;
            //NotifyPropertyChanged("isUse");
        }

        public void CreateAPolyline()
        {
            // Create a blue and a black Brush  
            SolidColorBrush yellowBrush = new SolidColorBrush();
            yellowBrush.Color = Colors.Yellow;
            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.Black;
            // Create a polyline  
            Polyline yellowPolyline = new Polyline();
            yellowPolyline.Stroke = yellowBrush;
            yellowPolyline.StrokeThickness = 1;
            // Create a collection of points for a polyline  
            System.Windows.Point Point1 = new System.Windows.Point(10, 5);
            System.Windows.Point Point2 = new System.Windows.Point(20, 15);
            System.Windows.Point Point3 = new System.Windows.Point(30, 5);
            System.Windows.Point Point4 = new System.Windows.Point(40, 15);
            System.Windows.Point Point5 = new System.Windows.Point(50, 5);
            PointCollection polygonPoints = new PointCollection();
            polygonPoints.Add(Point1);
            polygonPoints.Add(Point2);
            polygonPoints.Add(Point3);
            polygonPoints.Add(Point4);
            polygonPoints.Add(Point5);
            // Set Polyline.Points properties  
            yellowPolyline.Points = polygonPoints;
            yellowPolyline.Points.Add(Point5);
            // Add polyline to the page  
            chartPanel.Children.Add(yellowPolyline);
        }

        public void Draw()
        {
            if (Samples.Count > 0)
            {
                CharPolyline.Dispatcher.Invoke(new Action(() =>
                {
                    polygonPoints.Add(
                            Samples[Samples.Count - 1].Point
                        );
                    CharPolyline.Points = polygonPoints;
                }
              ));
            }
        }
    }
}
