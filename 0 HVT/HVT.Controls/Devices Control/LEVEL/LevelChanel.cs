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

namespace HVT.Controls
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
            VerticalAlignment = System.Windows.VerticalAlignment.Center
        };


        private bool _IsOn;
        public bool IsOn
        {
            get { return _IsOn; }
            set
            {
                if (value != _IsOn)
                {
                    _IsOn = value;
                    NotifyPropertyChanged("IsOn");
                    btOn.Dispatcher.Invoke(new Action(() => btOn.IsChecked = value));
                } 
            }
        }

        public ToggleButton btOn = new ToggleButton(){ };

        public Label chartLabel = new Label
        {
            Content = "Ch",
            HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right,
            FontSize = 9,
            Foreground = new SolidColorBrush(Colors.White),
            Background = new SolidColorBrush(Color.FromRgb(24, 24, 24)),
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
                    btOn.Dispatcher.Invoke(new Action(() => btOn.IsEnabled = _isUse));
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
            btOn.Loaded += BtOn_Loaded;
            CbUse.Checked += CbUse_Checked;
            CbUse.Unchecked += CbUse_Unchecked;
            chartPanel.Children.Clear();
            chartPanel.Children.Add(CharPolyline);
        }
        private void BtOn_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var tgbt = sender as ToggleButton;
            string template =
  "    <ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'"
+ "       xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'"
+ "       x:Key=\"ToggleButtonLager\" TargetType=\"{x:Type ToggleButton}\">"
+ "        <Border x:Name=\"border\" BorderBrush=\"Black\" BorderThickness=\"0.5\" Background=\"#1F1F1F\" SnapsToDevicePixels=\"True\">"
+ "            <ContentPresenter x:Name=\"contentPresenter\" "
+ "                              ContentTemplate=\"{TemplateBinding ContentTemplate}\""
+ "                              Content=\"{TemplateBinding Content}\""
+ "                              ContentStringFormat=\"{TemplateBinding ContentStringFormat}\""
+ "                              Focusable=\"False\""
+ "                              HorizontalAlignment=\"{TemplateBinding HorizontalContentAlignment}\""
+ "                              Margin=\"{TemplateBinding Padding}\""
+ "                              RecognizesAccessKey=\"True\""
+ "                              SnapsToDevicePixels=\"{TemplateBinding SnapsToDevicePixels}\""
+ "                              VerticalAlignment=\"{TemplateBinding VerticalContentAlignment}\""
+ "                              />"
+ "        </Border>"
+ "        <ControlTemplate.Triggers>"
+ "            <Trigger Property=\"ToggleButton.IsChecked\" Value=\"True\">"
+ "                <Setter Property=\"Background\" TargetName=\"border\" Value=\"#1FFF1F\"/>"
+ "                <Setter Property=\"Foreground\" Value=\"White\"/>"
+ "            </Trigger>"
+ "            <Trigger Property=\"IsEnabled\" Value=\"False\">"
+ "                <Setter Property=\"Background\" TargetName=\"border\" Value=\"#F4F4F4\"/>"
+ "                <Setter Property=\"Foreground\" Value=\"#FF838383\"/>"
+ "            </Trigger>"
+ "        </ControlTemplate.Triggers>"
+ "     </ControlTemplate>";

            tgbt.Template = (ControlTemplate)XamlReader.Parse(template);
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
