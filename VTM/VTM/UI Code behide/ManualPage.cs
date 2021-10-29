
using System;
using System.Windows;
using HVT.VTM.Program;
using HVT.VTM.Base;

namespace VTM
{
    /// <summary>
    /// Manual control page
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Load default Element
        /// </summary>
        public void LoadManualPage()
        {
            dgtUUT_TX.ItemsSource = null;
            dgtUUT_TX.ItemsSource = Program.RootModel.SerialNaming.TxDatas;
            dgtUUT_TX_Naming.ItemsSource = Program.RootModel.SerialNaming.TxDatas;
        }

        #region Model Action
        //Model Init see at MainWindow.cs
        //Event 
        private void Model_LoadFinish_ManualPage(object sender, EventArgs e)
        {
            //TestStepsGridManual.ItemsSource = null;

        }


        #endregion





        private void btTestManual_Checked(object sender, RoutedEventArgs e)
        {
            if (!Program.RootModel.IsTestting)
            {
                foreach (var item in Program.RootModel.Steps)
                {
                    item.ValueGet1 = "";
                    item.ValueGet2 = "";
                    item.ValueGet3 = "";
                    item.ValueGet4 = "";
                    item.Result1 = "";
                    item.Result2 = "";
                    item.Result3 = "";
                    item.Result4 = "";
                }
                TestStepsGrid.Items.Refresh();

                Program.RootModel.StepTesting = 0;
                Program.RootModel.IsTestting = true;
                EscapTime = DateTime.Now;
                EscapTimer.Elapsed -= EscapTimer_Elapsed;
                EscapTimer.Elapsed += EscapTimer_Elapsed;
                EscapTimer.Start();
                Program.RootModel.TestState = Model.RunTestState.TESTTING;
            }
            else
            {
                EscapTime = DateTime.Now;
                Program.RootModel.TestState = Model.RunTestState.TESTTING;
                EscapTimer.Start();
            }

        }

        private void btTestManual_Unchecked(object sender, RoutedEventArgs e)
        {
            Program.RootModel.TestState = Model.RunTestState.Pause;
            EscapTimer.Stop();
        }

    }
}
