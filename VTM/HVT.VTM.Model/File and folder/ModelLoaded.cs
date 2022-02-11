using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace HVT.VTM.Base
{
    public class ModelLoaded 
    {
        private string path;
        public string Path {
            get { return path; }
            set {
                path = value;
                FileLabel.Content = path;
                FileExited = File.Exists(path);
            }
        }

        private bool fileExited;
        public bool FileExited
        {
            get { return fileExited; }
            set
            {
                fileExited = value;
                FileLabel.Dispatcher.Invoke(new Action(() => FileLabel.Background = value == true ? new SolidColorBrush(Color.FromArgb(255, 46, 63, 63)): new SolidColorBrush(Color.FromArgb(255 ,150, 0, 0))));
            }
        }


        public bool LastLoadFail = false;
        public Button FileLabel = new Button()
        {
            Content = "",
            Background = new SolidColorBrush(Color.FromArgb(180, 12,12,12)),
            Foreground = new SolidColorBrush(Color.FromArgb(255,200,200,200)),
        };

        public ModelLoaded()
        {

        }


        private void BtOn_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var tgbt = sender as Button;
            string template =
                            "    <ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'"
                            + "       xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'"
                            + "       x:Key=\"ButtonLager\" TargetType=\"{x:Type Button}\">"
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
                            + "                <Setter Property=\"Background\" TargetName=\"border\" Value=\"#2E3F3F\"/>"
                            + "            </Trigger>"
                            + "            <Trigger Property=\"IsPressed\" Value=\"True\">"
                            + "                <Setter Property=\"Background\" TargetName=\"border\" Value=\"#3D92E2\"/>"
                            + "                <Setter Property=\"BorderBrush\" TargetName=\"border\" Value=\"#FF2C628B\"/>"
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
}
