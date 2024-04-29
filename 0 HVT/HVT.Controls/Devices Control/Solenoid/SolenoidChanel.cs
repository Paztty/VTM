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

namespace HVT.Controls
{
    public class SolenoidChannel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler ManualStateChange;

        public CheckBox CbUse = new CheckBox()
        {
            Content = "Ch",
            IsChecked = true,
        };

        public ToggleButton btOn = new ToggleButton()
        {
            Cursor = System.Windows.Input.Cursors.Hand,
            Margin = new System.Windows.Thickness(2),
            Width = 30,
            Content = "Ch",
            IsChecked = false,
            IsEnabled = true,
            HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
        };

        public ToggleButton btOnVision = new ToggleButton()
        {
            Cursor = System.Windows.Input.Cursors.Hand,
            Margin = new System.Windows.Thickness(2),
            Width = 30,
            Content = "Ch",
            IsChecked = false,
            IsEnabled = false,
            HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
        };

        public int CardID = 0x4D;

        private bool _isUse = true;
        public bool isUse
        {
            get { return _isUse; }
            set
            {
                if (_isUse != value)
                {
                    _isUse = value;
                    CbUse.Dispatcher.Invoke(new Action(() => CbUse.IsChecked = _isUse));
                    btOn.Dispatcher.Invoke(new Action(() => btOn.IsEnabled = _isUse));
                    btOnVision.Dispatcher.Invoke(new Action(() => btOnVision.IsEnabled = _isUse));
                    NotifyPropertyChanged("isUse");
                }
            }
        }

        public bool isOn { get; set; } = false;
        public int channel_P { get; set; }

        public bool IsUse
        {
            get { return isUse; }
            set
            {
                if (isUse != value)
                {
                    isUse = value;
                    NotifyPropertyChanged(nameof(IsUse));
                }
            }
        }

        public bool IsON
        {
            get { return isOn; }
            set
            {
                if (isOn != value)
                {
                    isOn = value;
                    btOn.Dispatcher.Invoke(new Action(() => btOn.IsChecked = value));
                    btOnVision.Dispatcher.Invoke(new Action(() => btOnVision.IsChecked = value));
                    NotifyPropertyChanged(nameof(IsON));
                }
            }
        }
        public int Channel_P
        {
            get { return channel_P; }
            set
            {
                if (value != channel_P)
                {
                    channel_P = value;
                    CbUse.Content = value.ToString();
                    btOn.Content = value.ToString();
                    btOnVision.Content = value.ToString();
                    NotifyPropertyChanged(nameof(Channel_P));
                }
            }
        }


        public SolenoidChannel()
        {
            CbUse.Checked += CbUse_Checked;
            CbUse.Unchecked += CbUse_Unchecked;

            btOn.Loaded += BtOn_Loaded;
            btOn.Checked += BtOn_Checked;
            btOn.Unchecked += BtOn_Unchecked;

            btOnVision.Loaded += BtOn_Loaded;
            btOnVision.Checked += BtOn_Checked;
            btOnVision.Unchecked += BtOn_Unchecked;
        }

        private void BtOn_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var tgbt = sender as ToggleButton;
            string template =
  "    <ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'"
+ "       xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'"
+ "       x:Key=\"ToggleButtonLager\" TargetType=\"{x:Type ToggleButton}\">"
+ "        <Border x:Name=\"border\" BorderBrush=\"{TemplateBinding BorderBrush}\" BorderThickness=\"0\" Background=\"DarkGray\" SnapsToDevicePixels=\"True\">"
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
+ "            <Trigger Property=\"IsMouseOver\" Value=\"True\">"
+ "                <Setter Property=\"Background\" TargetName=\"border\" Value=\"#BEDAF5\"/>"
+ "            </Trigger>"
+ "            <Trigger Property=\"IsPressed\" Value=\"True\">"
+ "                <Setter Property=\"Background\" TargetName=\"border\" Value=\"#3D92E2\"/>"
+ "            </Trigger>"
+ "            <Trigger Property=\"ToggleButton.IsChecked\" Value=\"True\">"
+ "                <Setter Property=\"Background\" TargetName=\"border\" Value=\"#3D92E2\"/>"
+ "                <Setter Property=\"Foreground\" Value=\"White\"/>"
+ "            </Trigger>"
+ "            <Trigger Property=\"IsEnabled\" Value=\"False\">"
+ "                <Setter Property=\"Background\" TargetName=\"border\" Value=\"#FFF4F4F4\"/>"
+ "                <Setter Property=\"Foreground\" Value=\"#FF838383\"/>"
+ "            </Trigger>"
+ "        </ControlTemplate.Triggers>"
+ "     </ControlTemplate>";


            tgbt.Template = (ControlTemplate)XamlReader.Parse(template);
        }

        private void BtOn_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            isOn = false;
            ManualStateChange?.Invoke(this, null);
        }

        private void BtOn_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            isOn = true;
            ManualStateChange?.Invoke(this, null);
        }

        private void CbUse_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            isUse = false;
            //NotifyPropertyChanged("isUse");
        }

        private void CbUse_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            isUse = true;
            //NotifyPropertyChanged("isUse");
        }
    }
}
