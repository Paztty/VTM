
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VTM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void TestStepsGridData_SelectedCellsChanged(object sender, System.Windows.Controls.SelectedCellsChangedEventArgs e)
        {
            TestStepsGridRemark.CurrentCell = new DataGridCellInfo(TestStepsGridRemark.Items[0], TestStepsGridData.CurrentColumn);
            TestStepsGridRemark.SelectedCells.Clear();
            TestStepsGridRemark.SelectedCells.Add(TestStepsGridRemark.CurrentCell);
        }

        //////      /////    ////////////////////    ////////        ////
        /////     ////             /////           ////// ///      ////   
        ////    ///              /////           //////  ///    // //
        ///   ///              /////           //////   ///  //  //
        //  //               /////           //////     ////   //
        ///                /////           //////       //   //

        //Main window code  : MainWindow.cs
        //Auto page code    : AutoPage.cs
        //Manual page code  : ManualPage.cs
        //Model page code   : ModelPage.cs
        //Vision page code  : VisionPage.cs


        //Machine: PPS x2 , DMM x2, Main control x2, Mux x2, Level x1, Relay x1, Sol x1

        //Vision: Camera x1, 4 site {LED x30, GLED x30, FND x1}

        //PBA: list max 4 site:
        //      Site Name
        //      Site Skip
        //      List test steps
        //      Image
        //      Datetime
        //      QR in
        //      QR out
        //      PCB array config

        //Model program:
        //  - Model name
        //  - PCB array config
        //  - Camera setting
        //  - TX naming, RX naming (link file)
        //  - UUT serial config
        //  - QR naming (link file)
        //  - Barcode setting 
        //  - PPS DC Power setting
        //  - Discharge setting
        //  - Vision list steps test ()
        //  - PCB image
        //  - Channel setting
        //  - Mux use
        //  - Level use

        // Step test

        // Vistion test

    }
}