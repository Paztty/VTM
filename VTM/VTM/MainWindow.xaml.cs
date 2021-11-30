using HVT.VTM.Base;
using HVT.VTM.Program;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace VTM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void TestStepsGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            // Program.RootModel.Steps.Add(new HVT.VTM.Base.Step() { No = Program.RootModel.Steps.Count, });
        }

        private void TestStepsGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (TestStepsGridData.CurrentCell != null)
            {
                var cellContent = TestStepsGridData.CurrentCell.Column.GetCellContent(TestStepsGridData.CurrentCell.Item);
                if (cellContent != null)
                {
                    if (e.Key == Key.Enter)
                    {
                        var cell = (DataGridCell)cellContent.Parent;
                        if (!cell.IsEditing)
                        {
                            TestStepsGridData.BeginEdit();
                            cell.IsEditing = true;
                            cellContent.Focus();
                            e.Handled = true;
                        }
                        else
                        {
                            TestStepsGridData.CommitEdit(DataGridEditingUnit.Cell, false);
                            Keyboard.Focus(TestStepsGridData);
                        }
                    }
                }
            }
        }

        private void TestStepsGrid_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (TestStepsGridData.CurrentCell != null)
            {
                var cellContent = TestStepsGridData.CurrentCell.Column.GetCellContent(TestStepsGridData.CurrentCell.Item);
                if (cellContent != null)
                {
                    if (e.Key == Key.Enter)
                    {
                        var cell = (DataGridCell)cellContent.Parent;
                        if (cell.IsEditing)
                        {
                            cell.IsEditing = true;
                            cellContent.Focus();
                            e.Handled = true;
                        }
                    }
                }

            }
        }

        private void DataGridCheckBoxColumn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cellContent = TestStepsGridData.CurrentCell.Column.GetCellContent(TestStepsGridData.CurrentCell.Item);
            if (cellContent != null)
            {
                TestStepsGridData.BeginEdit();
                (cellContent as CheckBox).IsChecked = !(cellContent as CheckBox).IsChecked;
                TestStepsGridData.CommitEdit(DataGridEditingUnit.Cell, false);
            }
        }

        private void TestStepsGridData_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            if (TestStepsGridData.CurrentCell != null)
            {
                DataGridCellInfo cells = TestStepsGridData.CurrentCell;

                Step item = cells.Item as Step;
                if (item != null)
                {
                    Program.Command.cmd = item.cmd;
                    TestStepsGridRemark.ItemsSource = null;
                    TestStepsGridRemark.ItemsSource = Program.Command.CMD;
                    StepDecription.Text = Program.Command.CMD[0].Description;

                    if (TestStepsGridData.CurrentColumn != null)
                    {
                        TestStepsGridRemark.CurrentCell = new DataGridCellInfo(TestStepsGridRemark.Items[0], TestStepsGridData.CurrentColumn);
                        TestStepsGridRemark.SelectedCells.Clear();
                        TestStepsGridRemark.SelectedCells.Add(TestStepsGridRemark.CurrentCell);
                    }
                }
            }
        }

        private void Detect_Click(object sender, RoutedEventArgs e)
        {
            Program.StartUpdateLCD();
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