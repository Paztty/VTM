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
    public class MuxChannel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ManualStateChange;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public CheckBox CbUse = new CheckBox()
        {
            Content = "Ch",
            IsChecked = false,
            VerticalAlignment = System.Windows.VerticalAlignment.Center,
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
                    NotifyPropertyChanged("IsUse");
                }
            }
        }

        private bool isOn = false;
        private int channel_P;
        private int channel_N;


        public bool IsON
        {
            get { return isOn; }
            set
            {
                if (isOn != value)
                {
                    isOn = value;
                    btOn.Dispatcher.Invoke(new Action(() => btOn.IsChecked = value));
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
                    CardID = value >= 48 ? 0x6D : 0x4D;
                    NotifyPropertyChanged(nameof(Channel_P));
                }
            }
        }
        public int Channel_N
        {
            get { return channel_N; }
            set
            {
                if (value != channel_N)
                {
                    channel_N = value;
                    NotifyPropertyChanged(nameof(Channel_N));
                }
            }
        }

        public MuxChannel()
        {
            CbUse.Checked += CbUse_Checked;
            CbUse.Unchecked += CbUse_Unchecked;

            btOn.Loaded += BtOn_Loaded;
            //btOn.Checked += BtOn_Checked;
            //btOn.Unchecked += BtOn_Unchecked;
            btOn.Click += BtOn_Click;
        }

        private void BtOn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ManualStateChange?.Invoke(this, null);
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
            tgbt.IsEnabled = IsUse;
        }

        //private void BtOn_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    IsON = false;
        //    ManualStateChange?.Invoke(this, null);
        //}

        //private void BtOn_Checked(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    IsON = true;
        //    ManualStateChange?.Invoke(this, null);
        //}

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
    }
}
