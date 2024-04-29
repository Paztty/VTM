using HVT.Utility;
using HVT.VTM.Program;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using static VTM_Report.MainWindow;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using RadioButton = System.Windows.Controls.RadioButton;
using TextBox = System.Windows.Controls.TextBox;

namespace VTM_Report
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public FolderMap Folder = new FolderMap();
        public class HistoryFilter
        {
            public DateTime StartFilterTime { get; set; } = DateTime.Now;
            public DateTime EndFilterTime { get; set; } = DateTime.Now;
            public string Result { get; set; } = "All";
            public string Modelname { get; set; } = "";
            public string Barcode { get; set; } = "";
        }
        public List<Board> Boards = new List<Board>();

        System.Timers.Timer GetDetailTimer = new System.Timers.Timer()
        {
            Interval = 250,
        };

        public MainWindow()
        {
            FolderMap.RootFolder = "D:\\Daeyoung Electronis Vina\\VTM";
            InitializeComponent();
            startDatePicker.SelectedDate = DateTime.Now;
            endDatePicker.SelectedDate = DateTime.Now;
            Task.Run(LoadHistories);
            GetDetailTimer.Elapsed += GetDetailTimer_Elapsed;
            GetDetailTimer.Enabled = true;
            GetDetailTimer.AutoReset = false;
        }

        private void GetDetailTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            GetDetailTimer.Stop();
            this.Dispatcher.Invoke(new Action(() =>
            {
                var item = dgrHistory.SelectedItem;
                if (item != null)
                {
                    dgrHistoryItem.ItemsSource = ((Board)item).TestStep;
                    lbModelName.Content = "MODEL: " + ((Board)item).ModelName;
                    lbTime.Content = "TIME: " + ((Board)item).StartTest.ToString("yyyy-MM-dd HH:mm:ss");
                    lbSite.Content = "SITE: " + ((Board)item).SiteName;
                    lbHistoryResult.Content = "RESULT: " + ((Board)item).Result;
                }
            }));
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void btCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Close button click");
            Close();
        }
        private void btMaximizeWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
        private void btMinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }


        public void LoadHistories()
        {
            Boards.Clear();
            LoadHistory();
            dgrHistory.Dispatcher.Invoke(new Action(delegate
            {
                dgrHistory.ItemsSource = Boards;
                dgrHistory.Items.Refresh();
                pnLoading.Visibility = Visibility.Hidden;
                lbAllCount.Content = Boards.Count();
                lbFailCount.Content = Boards.Where(x => x.Result == "FAIL").Count();
                lbPassCount.Content = Boards.Where(x => x.Result == "OK").Count();
            }));
        }

        public HistoryFilter historyFilter = new HistoryFilter();
        public double percent = 0.0;
        public event EventHandler Loadding;
        public async void LoadHistory()
        {
            var historyDay = historyFilter.StartFilterTime == null ? DateTime.Now : historyFilter.StartFilterTime;
            List<string> historesStr = new List<string>();
            while (historyFilter.EndFilterTime.Subtract(historyDay).TotalDays >= 0)
            {
                Console.WriteLine(String.Format("{0}\\{1}", FolderMap.RootFolder + @"\History\" + historyDay.ToString(@"yyyy\\MM"), historyDay.ToString("dd") + ".vtmh"));
                if (File.Exists(String.Format("{0}\\{1}", FolderMap.RootFolder + @"\History\" + historyDay.ToString(@"yyyy\\MM"), historyDay.ToString("dd") + ".vtmh")))
                {
                    historesStr.AddRange(File.ReadAllLines(String.Format("{0}\\{1}", FolderMap.RootFolder + @"\History\" + historyDay.ToString(@"yyyy\\MM"), historyDay.ToString("dd") + ".vtmh")));

                }
                historyDay = historyDay.AddDays(1);
            }

            if (historesStr.Count < 1) return;

            for (int i = 0; i < historesStr.Count; i++)
            {
                percent = i / (double)historesStr.Count * 100;
                Loadding?.Invoke(percent, null);
                var item = historesStr[i];
                var history = Extensions.ConvertFromJson<Board>(item);
                history.ModelName = history.ModelName != null ? history.ModelName : "";
                history.QRout = history.QRout != null ? history.QRout : "";
                bool filterOk = true;
                filterOk = historyFilter.Modelname != null ? filterOk & history.ModelName.Contains(historyFilter.Modelname) : filterOk & true;
                filterOk = historyFilter.Barcode != null ? filterOk & history.Barcode.Contains(historyFilter.Barcode) : filterOk & true;

                if (historyFilter.Result == "Pass")
                {
                    if (filterOk && (history.Result == "OK"))
                    {
                        history.No = Boards.Count + 1;
                        Boards.Add(history);
                    }
                }
                if (historyFilter.Result == "Fail")
                {
                    if (filterOk && (history.Result != "OK"))
                    {
                        history.No = Boards.Count + 1;
                        Boards.Add(history);
                    }
                }
                if (historyFilter.Result == "All")
                {
                    if (filterOk)
                    {
                        history.No = Boards.Count + 1;
                        Boards.Add(history);
                    }
                }
            }
            await Task.Delay(100);
        }

        private void dgrHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetDetailTimer.Stop();
            GetDetailTimer.Start();
        }

        private void btExport_Click(object sender, RoutedEventArgs e)
        {
            Export_HistoryData();
        }

        private void btExportDetail_Click(object sender, RoutedEventArgs e)
        {
            Export_HistoryDetailData();
        }

        private void Export_HistoryData()
        {
            List<string> DatasExport = new List<string>();
            string ExportData = String.Format("{0},{1},{2},{3},{4},{5}",
            "No",
            "Start test time",
            "Model code",
            "QR",
            "Site",
            "Result");
            DatasExport.Add(ExportData);
            foreach (var item in Boards)
            {
                ExportData = String.Format("{0},{1},{2},{3},{4},{5}",
                            item.No.ToString(),
                            item.StartTest.ToString("yyyy/MM/dd hh:mm:ss"),
                            item.ModelName.Replace(".vmdl",""),
                            item.QRout,
                            item.SiteName,
                            item.Result
                    );
                DatasExport.Add(ExportData);
            }
            SaveFileDialog openFile = new SaveFileDialog
            {
                Title = "Export history data.",
            };
            openFile.Filter = "csv (*.csv)|*.csv";
            openFile.RestoreDirectory = true;
            if ((bool)openFile.ShowDialog() == true)
            {
                try
                {
                    File.Delete(openFile.FileName);
                    File.AppendAllLines(openFile.FileName, DatasExport.ToArray());
                    foreach (var item in Boards)
                    {
                        Export_HistoryDetailData(item, openFile.FileName.Substring(0, openFile.FileName.LastIndexOf("\\")));
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Export file exception: " + e.Message.ToString(), "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                try
                {
                    Process.Start(openFile.FileName);
                }
                catch (Exception)
                {
                }
            }
        }

        private void Export_HistoryDetailData()
        {
            if (dgrHistory.SelectedItem == null) return;
            List<string> DatasExport = new List<string>();
            string ExportData = String.Format("{0},{1},{2},{3},{4},{5}",
            "No",
            "Content",
            "CMD",
            "Min - Max",
            "Value",
            "Result");
            DatasExport.Add(ExportData);
            foreach (var item in (dgrHistory.SelectedItem as Board).TestStep)
            {
                ExportData = String.Format("{0},{1},{2},{3},{4},{5}",
                            item.No.ToString(),
                            item.TestContent,
                            item.CMD,
                            item.Min_Max,
                            item.Value,
                            item.ResultValue
                    );
                DatasExport.Add(ExportData);
            }
            SaveFileDialog openFile = new SaveFileDialog
            {
                Title = "Export history data.",
            };
            openFile.Filter = "csv (*.csv)|*.csv";
            openFile.RestoreDirectory = true;
            if ((bool)openFile.ShowDialog() == true)
            {
                try
                {
                    File.Delete(openFile.FileName);
                    File.AppendAllLines(openFile.FileName, DatasExport.ToArray());
                }
                catch (Exception e)
                {
                    MessageBox.Show("Export file exception: " + e.Message.ToString(), "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                try
                {
                    Process.Start(openFile.FileName);
                }
                catch (Exception)
                {
                }
            }
        }
        private void Export_HistoryDetailData( Board board, string Path)
        {
            if (board == null) return;
            List<string> DatasExport = new List<string>();
            string ExportData = String.Format("{0},{1},{2},{3},{4},{5}",
            "No",
            "Content",
            "CMD",
            "Min - Max",
            "Value",
            "Result");
            DatasExport.Add(ExportData);
            foreach (var item in board.TestStep)
            {
                ExportData = String.Format("{0},{1},{2},{3},{4},{5}",
                            item.No.ToString(),
                            item.TestContent,
                            item.CMD,
                            item.Min_Max,
                            item.Value,
                            item.ResultValue
                    );
                DatasExport.Add(ExportData);
            }

            if (!Directory.Exists(Path + "\\Details"))
            {
                Directory.CreateDirectory(Path + "\\Details");
            }

            File.AppendAllLines(Path + "\\Details\\" +
                board.EndTest.ToString("yyyyMMddHHmmss") +
                "_" + board.Barcode +
                board.SiteName +
                ".csv",
                DatasExport.ToArray());
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            historyFilter.StartFilterTime = (DateTime)startDatePicker.SelectedDate;
            historyFilter.EndFilterTime = (DateTime)endDatePicker.SelectedDate;
            if (tbBarcode.Text.Length >= 1 && tbBarcode.Text != "Barcode")
            {
                historyFilter.Barcode = tbBarcode.Text;
            }
            if (tbModelName.Text.Length >= 1 && tbModelName.Text != "Model name")
            {
                historyFilter.Barcode = tbModelName.Text;
            }
            Task.Run(LoadHistories);
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            string content = (sender as RadioButton).Content as string;
            historyFilter.Result = content;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((sender as TextBox).Text != null && (sender as TextBox).Text != "Model name" && (sender as TextBox).Text != "")
            {
                historyFilter.Modelname = (sender as TextBox).Text;
            }
        }

        private void TextBoxBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if ((sender as TextBox).Text != null && (sender as TextBox).Text != "Barcode" && (sender as TextBox).Text != "")
            {
                historyFilter.Barcode = (sender as TextBox).Text;
            }
        }
    }
}
