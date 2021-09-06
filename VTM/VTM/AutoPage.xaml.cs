using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VTM
{
    /// <summary>
    /// Interaction logic for AutoPage.xaml
    /// </summary>
    public partial class AutoPage : Page 
    {
        Model model = new Model();
        Model MainModel = new Model();



        public DateTime EscapTime = DateTime.Now;

        public bool UserSite1 = true;
        public bool UserSite2 = true;
        public bool UserSite3 = true;
        public bool UserSite4 = true;

        public string TestResult = "REALDY";
        //public ImageSource CameraImage { get; set; }


        public AutoPage(Model model)
        {
            InitializeComponent();
            MainModel = model;
            MainModel.LoadFinish += Model_LoadFinish;

        }



        private void Model_LoadFinish(object sender, EventArgs e)
        {
            //model = MainModel;
            model = (Model)MainModel.Clone();
            model.StepTestChange += Model_StepTestChangeAsync;
            model.TestRunFinish += Model_TestRunFinish;

            TestStepsGrid.ItemsSource = null;
            TestStepsGrid.ItemsSource = new ObservableCollection<Model.Step>(model.Steps);

            // hide another site if not use
            //Site3.Width = new GridLength(0, GridUnitType.Star);
            //Site4.Width = new GridLength(0, GridUnitType.Star);

            //
            //TestStepsGrid.Columns[7].Visibility = Visibility.Hidden;
            //TestStepsGrid.Columns[8].Visibility = Visibility.Hidden;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            

            List<Model.Barcode> barcodes = new List<Model.Barcode>()
            {
                new Model.Barcode(){ No = 1, BarcodeData = "06DC9212345678DEC" },
                new Model.Barcode(){ No = 2, BarcodeData = "06DC9212345688DEC" },
            };
            BacodesTestingList.ItemsSource = barcodes;
            BacodesWaitingList.ItemsSource = barcodes;
        }

        private void TestStepsGrid_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public void RefreshData()
        {
            TestStepsGrid.Items.Refresh();
        }


        private void TestStepsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (model.IsTestting)
            {
                ProgressTestSlider.Value =(int)((double)model.StepTesting / (model.Steps.Count - 2)* 100);
            }
        }

        private void WaitSite1_Checked(object sender, RoutedEventArgs e)
        {
            WaitSite1.Background = new SolidColorBrush(Colors.DarkGray);
            WaitSite1.Content = "Skip";
        }

        private void WaitSite1_Unchecked(object sender, RoutedEventArgs e)
        {
            WaitSite1.Background = new SolidColorBrush(Colors.Black);
            WaitSite1.Content = "Wait";
        }

        System.Windows.Forms.Timer runtestTimer = new System.Windows.Forms.Timer();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EscapTime = DateTime.Now;
            lbTestResultTesting.Visibility = Visibility.Visible;
            lbTestResultGood.Visibility = Visibility.Hidden;
            Runtest();
        }

        public void Runtest()
        {
            foreach (var item in model.Steps)
            {
                item.ValueGet1 = "";
                item.ValueGet2 = "";
                item.ValueGet3 = "";
                item.ValueGet4 = "";
                item.ValueGet5 = "";
                item.ValueGet6 = "";
                item.ValueGet7 = "";
                item.ValueGet8 = "";
            }
            TestStepsGrid.Items.Refresh();
            if (!model.IsTestting)
            {
                model.StepTesting = 0;
                model.IsTestting = false;
                Thread RunTest = new Thread(model.runTest, BackgroundProperty.GlobalIndex);
                RunTest.IsBackground = true;
                RunTest.Start();
            }
        }

        private void Model_StepTestChangeAsync(object sender, EventArgs e)
        {
            if (model.StepTesting >= 0)
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    TestStepsGrid.SelectedItem = model.Steps[(int)sender];
                    lbEscapTime.Content = DateTime.Now.Subtract(EscapTime).TotalSeconds.ToString("F1") + "s";
                    TestStepsGrid.ScrollIntoView(TestStepsGrid.SelectedItem);
                }), DispatcherPriority.Send);
            }
        }
        private void Model_TestRunFinish(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                lbTestResultTesting.Visibility = Visibility.Hidden;
                lbTestResultGood.Visibility = Visibility.Visible;
            }));
        }
    }


}
