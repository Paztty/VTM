using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;

namespace HVT.VTM.Base
{
    public class PBA : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }




        private string sitename;
        public string SiteName
        {
            get { return sitename; }
            set
            {
                btManualSelect.Content = value;
                sitename = value;
                btManualSelect.Dispatcher.Invoke(new Action(() => btManualSelect.Content = value));
            }
        }
        public List<PBA_STEP> Steps { get; set; } = new List<PBA_STEP>();
        public Barcode Barcode { get; set; } = new Barcode();
        public Barcode BarcodeWaiting { get; set; } = new Barcode();

        private bool isWaiting = true;
        public bool IsWaiting
        {
            get { return isWaiting; }
            set
            {
                if (isWaiting != value)
                {
                    isWaiting = value;
                    btManualSelect.Dispatcher.Invoke(new Action(() => btManualSelect.IsChecked = value));
                    CbWait.Dispatcher.Invoke(new Action(() =>
                    {
                        CbWait.IsChecked = value;
                        CbWait.Content = value ? "Wait" : "Skip";
                        CbWait.Background = value ? new SolidColorBrush(Colors.Transparent): new SolidColorBrush(Colors.DarkGray);
                    }));
                    lbIsWaiting.Dispatcher.Invoke(new Action(() => 
                    {
                        lbIsWaiting.Content = value ? "Wait" : "Skip";
                        lbIsWaiting.Background = value ? new SolidColorBrush(Colors.Transparent) : new SolidColorBrush(Colors.DarkGray);
                    }));

                    NotifyPropertyChanged(nameof(IsWaiting));
                }
            }
        }

        public UUTPort Port1 = new UUTPort();
        public UUTPort Port2 = new UUTPort();

        public ObservableCollection<MuxChannel> MUXs { get; set; } = new ObservableCollection<MuxChannel>();
        public ObservableCollection<MuxChannel> Relays { get; set; } = new ObservableCollection<MuxChannel>();

        private Visibility visible = Visibility.Visible;
        public Visibility Visibility
        {
            get { return visible; }
            set {
                if (visible != value)
                {
                    visible = value == Visibility.Visible ? Visibility.Visible : Visibility.Hidden;
                    btManualSelect.Dispatcher.Invoke(new Action(() => btManualSelect.Visibility = value));
                    NotifyPropertyChanged(nameof(Visibility));
                }
            }
        }

        // UI component
        public ToggleButton btManualSelect = new ToggleButton()
        {
            Margin = new System.Windows.Thickness(2),
            Width = 48,
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
        };

        public Label lbIsWaiting = new Label()
        {
            Content = "Wait",
            FontFamily = new FontFamily("Lucida Console"),
            FontSize = 13,
            Foreground = new SolidColorBrush(Colors.White),
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
            VerticalContentAlignment = System.Windows.VerticalAlignment.Center,
            HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
        };

        public CheckBox CbWait = new CheckBox()
        {
            FontFamily = new FontFamily("Lucida Console"),
            FontSize = 13,
            Content = "Wait",
            Background = new SolidColorBrush(Colors.Transparent),
            IsChecked = true,
            VerticalAlignment = System.Windows.VerticalAlignment.Center,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
        };


        // Method
        public void GetStepList(List<Step> steps, int Site)
        {
            this.Steps.Clear();
            foreach (Step step in steps)
            {
                Steps.Add(
                    new PBA_STEP()
                    {
                        No = step.No,
                        IMQSCode = step.IMQSCode,
                        TestContent = step.TestContent,
                        Oper = step.Oper,
                        MinMax = step.Min_Max,
                        Value = Site == 1 ? step.ValueGet1 :
                            Site == 2 ? step.ValueGet2 :
                            Site == 3 ? step.ValueGet3 :
                            Site == 4 ? step.ValueGet4 : "",
                        Result = Site == 1 ? step.Result1 :
                            Site == 2 ? step.Result2 :
                            Site == 3 ? step.Result3 :
                            Site == 4 ? step.Result4 : "",
                    });
            }
        }


        public PBA()
        {
            btManualSelect.Loaded += BtManualSelect_Loaded;
            btManualSelect.Checked += BtManualSelect_Checked;
            btManualSelect.Unchecked += BtManualSelect_Unchecked;

            CbWait.Checked += CbWait_Checked;
            CbWait.Unchecked += CbWait_Unchecked;
        }

        private void CbWait_Unchecked(object sender, RoutedEventArgs e)
        {
            IsWaiting = false;
        }

        private void CbWait_Checked(object sender, RoutedEventArgs e)
        {
            IsWaiting = true;
        }

        private void BtManualSelect_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsWaiting = false;
        }

        private void BtManualSelect_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsWaiting = true;
        }

        private void BtManualSelect_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var tgbt = sender as ToggleButton;
            string template =
"         <ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'"
+ "       xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'"
+ "       x:Key=\"ToggleButtonLager\" TargetType=\"{x:Type ToggleButton}\">"
+ "        <Border x:Name=\"border\" BorderBrush=\"{TemplateBinding BorderBrush}\" BorderThickness=\"2\" Background=\"DarkGray\" SnapsToDevicePixels=\"True\">"
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
+ "                <Setter Property=\"BorderBrush\" TargetName=\"border\" Value=\"#FF2C628B\"/>"
+ "            </Trigger>"
+ "            <Trigger Property=\"ToggleButton.IsChecked\" Value=\"True\">"
+ "                <Setter Property=\"BorderThickness\" TargetName=\"border\" Value=\"1\"/>"
+ "                <Setter Property=\"Background\" TargetName=\"border\" Value=\"#3D92E2\"/>"
+ "                <Setter Property=\"Foreground\" Value=\"White\"/>"
+ "            </Trigger>"
+ "            <Trigger Property=\"IsEnabled\" Value=\"False\">"
+ "                <Setter Property=\"Background\" TargetName=\"border\" Value=\"#FFF4F4F4\"/>"
+ "                <Setter Property=\"BorderBrush\" TargetName=\"border\" Value=\"#FFADB2B5\"/>"
+ "                <Setter Property=\"Foreground\" Value=\"#FF838383\"/>"
+ "            </Trigger>"
+ "        </ControlTemplate.Triggers>"
+ "    </ControlTemplate>";

            tgbt.Template = (ControlTemplate)XamlReader.Parse(template);
        }
    }


    public class PBA_STEP
    {
        public int No;
        public string IMQSCode;
        public string TestContent;
        public string CMD;
        public string Oper;
        public string MinMax;
        public string Value;
        public string Result;
    }

}
