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
using System.Net.NetworkInformation;
using System.IO;
using System.Security.Principal;
using System.Diagnostics;
using System.Timers;
using System.Threading;

namespace Technical_App_Update_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        System.Timers.Timer timer = new System.Timers.Timer();
        string ApplicationFile = "VTM.exe";
        string ApplicationFolder = "VTM";

        public MainWindow()
        {
            InitializeComponent();

            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 500;
            timer.Enabled = true;
            timer.Start();
        }

        public MainWindow(string ApplicationName)
        {
            InitializeComponent();
            ApplicationFile = ApplicationName;
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 500;
            timer.Enabled = true;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Disponse timer
            timer.Dispose();
              
            //Check version of update software
            string newVersion = "";
            string tagetVersion = "~";

            //get cuuren version
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + ApplicationFile))
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(AppDomain.CurrentDomain.BaseDirectory + ApplicationFile);
                tagetVersion = versionInfo.FileVersion;
            }

            //check network folder
            DirectoryInfo di = new DirectoryInfo(@"\\DESKTOP-U660IDL\Technical App Deloy\");
            DirectoryInfo CurenDisrectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            List<DirectoryInfo> nets = new List<DirectoryInfo>();

            // if exist folder, get software version
            if (di.Exists)
            {
                if (File.Exists(di.FullName + ApplicationFolder + "\\" + ApplicationFile))
                {
                    var versionInfo = FileVersionInfo.GetVersionInfo(di.FullName + ApplicationFolder + "\\" + ApplicationFile);
                    newVersion = versionInfo.FileVersion;
                }
                else
                {
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + ApplicationFile))
                    {
                        Process.Start(AppDomain.CurrentDomain.BaseDirectory + ApplicationFile);
                        this.Dispatcher.BeginInvoke(new Action(() => this.Close()));
                        return;
                    }
                    else
                    {
                        this.Dispatcher.BeginInvoke(new Action(() => this.Close()));
                        return;
                    }

                }
            }
            // if not finded application, start current app
            else
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + ApplicationFile))
                {
                    Process.Start(AppDomain.CurrentDomain.BaseDirectory + ApplicationFile);
                    this.Dispatcher.BeginInvoke(new Action(() => this.Close()));
                    return;
                }
                else
                {
                    this.Dispatcher.BeginInvoke(new Action(() => this.Close()));
                    return;
                }
            }

            // compare version 
            if (newVersion == tagetVersion)
            {
                Process.Start(AppDomain.CurrentDomain.BaseDirectory + ApplicationFile);
                this.Dispatcher.BeginInvoke(new Action(() => this.Close()));
                return;
            }

            nets = di.GetDirectories().ToList();

            double itemIndex = 0;
            double total = 0;

            foreach (var item in nets)
            {
                if (item.Name == ApplicationFolder)
                {
                    total += item.GetDirectories().Length;
                    total += item.GetFiles().Length;
                }
            }
                
            foreach (var item in nets)
            {
                if (item.Name == ApplicationFolder)
                {
                    foreach (DirectoryInfo dir in item.GetDirectories())
                    {
                        itemIndex++;
                        progressBar.Dispatcher.Invoke(new Action(() => progressBar.Value = Math.Round(((itemIndex / total) * 100), 1)));
                        lbUpdateItem.Dispatcher.Invoke(new Action(() => lbUpdateItem.Content = dir.Name));
                        CopyFilesRecursively(dir, CurenDisrectory.CreateSubdirectory(dir.Name));
                    }

                    foreach (FileInfo file in item.GetFiles())
                    {
                        itemIndex++;
                        progressBar.Dispatcher.Invoke(new Action(() => progressBar.Value = Math.Round(((itemIndex / total) * 100), 1)));
                        lbUpdateItem.Dispatcher.Invoke(new Action(() => lbUpdateItem.Content = file.Name));
                        file.CopyTo(System.IO.Path.Combine(CurenDisrectory.FullName, file.Name), true);
                    }
                }
            }
            lbUpdateItem.Dispatcher.Invoke(new Action(() => lbUpdateItem.Content = "Start " + ApplicationFile));
            Process.Start(AppDomain.CurrentDomain.BaseDirectory + ApplicationFile);
            Thread.Sleep(2000);
            this.Dispatcher.BeginInvoke(new Action(() => this.Close()));

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(System.IO.Path.Combine(target.FullName, file.Name), true);
        }
    }
}
