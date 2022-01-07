
using HVT.VTM.Base;
using HVT.VTM.Program;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Xceed.Wpf.Toolkit;
using DataGridCell = System.Windows.Controls.DataGridCell;
using DataGrid = System.Windows.Controls.DataGrid;
using System.Windows.Media;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using CheckBox = System.Windows.Controls.CheckBox;
using System.Collections.Generic;
using System.IO.Ports;
using ComboBox = System.Windows.Controls.ComboBox;

namespace VTM
{
    public partial class MainWindow : Window
    {
        public void LoadModelPage()
        {
            Program.RootModel.contruction.PCBlayout = PCBlayout;
            //PCB panel layout load
            Program.RootModel.contruction.PCB_label.Add(PCB1);
            Program.RootModel.contruction.PCB_label.Add(PCB2);
            Program.RootModel.contruction.PCB_label.Add(PCB3);
            Program.RootModel.contruction.PCB_label.Add(PCB4);

            TestStepsGridData.ItemsSource = null;
            TestStepsGridData.ItemsSource = Program.RootModel.Steps;
            TestStepsGridRemark.ItemsSource = Program.Command.CMD;

            PCB_Ui_Layout_Init();

            Program.PrinterUiInit(TX_RECT_COM13, RX_RECT_COM13, CONNECTED_RECT_COM13);

            Program.UUTpageInit(dgTX_data_naming, dgRX_data_naming, dgQRcodeNameCode, tbQRcodeLink);

            Program.MuxUIInit(MUX_Channels_Table1, MUX_Channels_Table2, wrapPanelMuxSelect, pnMux1, pnMux2,
                TX_RECT_COM3, RX_RECT_COM3, CONNECTED_RECT_COM3,
                TX_RECT_COM4, RX_RECT_COM4, CONNECTED_RECT_COM4
                );

            Program.RelayUIInit(pnRelaySelect, pnRelay1, pnRelay2, pnVisionRelays,
            TX_RECT_COM5, RX_RECT_COM5, CONNECTED_RECT_COM5);

            Program.SolenoidUIInit(pnSolenoid, pnVisionSolenoid,
            TX_RECT_COM6, RX_RECT_COM6, CONNECTED_RECT_COM6);

            Program.UUTPortUIInit(
                TX_RECT_COM8, RX_RECT_COM8, CONNECTED_RECT_COM8,
                TX_RECT_COM9, RX_RECT_COM9, CONNECTED_RECT_COM9,
                TX_RECT_COM10, RX_RECT_COM10,CONNECTED_RECT_COM10,
                TX_RECT_COM11, RX_RECT_COM11,CONNECTED_RECT_COM11
                );
            
        }

        #region Model Action
        //Model Init see at MainWindow.cs
        //Event 
        private void Model_LoadFinish_ModelPage(object sender, EventArgs e)
        {
            tbModelName.Text = Program.RootModel.Name;
            tbModelNamePath.Text = Program.RootModel.Path;
            TestStepsGridData.ItemsSource = null;
            TestStepsGridData.ItemsSource = Program.RootModel.Steps;

            TestStepsGridRemark.ItemsSource = null;
            TestStepsGridRemark.ItemsSource = Program.Command.CMD;
            Error_Positions_Table.ItemsSource = Program.RootModel.ErrorPositions;

            Program.RootModel.contruction.PCBlayout = PCBlayout;
            //PCB panel layout load
            Program.RootModel.contruction.PCB_label.Clear();
            Program.RootModel.contruction.PCB_label.Add(PCB1);
            Program.RootModel.contruction.PCB_label.Add(PCB2);
            Program.RootModel.contruction.PCB_label.Add(PCB3);
            Program.RootModel.contruction.PCB_label.Add(PCB4);

            nUD_PCBcount.Value = Program.RootModel.contruction.PCB_Count;
            nUD_X_axis_count.Value = Program.RootModel.contruction.PCB_X_axis_Count;

            cbbVisionTxnaming.ItemsSource = Program.RootModel.Naming.TxDatas;
        }

        private void btSaveModel_Click(object sender, RoutedEventArgs e)
        {
            GetPortSetting();
            Program.SaveModel();
            foreach (Step step in Program.RootModel.Steps)
                Console.WriteLine(step.CMD);
            Program.RootModel.LoadFinishEvent();
        }

        private void btSaveAsModel_Click(object sender, RoutedEventArgs e)
        {
            GetPortSetting();
            Program.SaveModelAs();
            Program.RootModel.LoadFinishEvent();
        }

        private void btOpenEditModel_Click(object sender, RoutedEventArgs e)
        {
            Program.LoadModel();

            Program.RootModel.LoadFinish += Model_LoadFinish_AutoPage;
            Program.RootModel.LoadFinish += Model_LoadFinish_ManualPage;
            Program.RootModel.LoadFinish += Model_LoadFinish_ModelPage;

            //Program.RootModel.StepTestChange += Model_StepTestChangeAsync;
            //Program.RootModel.TestRunFinish += Model_TestRunFinish;
            //Program.RootModel.StateChange += Model_StateChange;

            Program.RootModel.LoadFinishEvent();
        }

        private void TestStepsGridData_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void TestStepsGridData_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex().ToString();
        }


        private void dgTX_data_naming_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex().ToString();
        }


        private void TestStepsGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new HVT.VTM.Base.Step()
            {
                No = Program.RootModel.Steps.Count + 1,
            };
        }

        private void TestStepsGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var grid = sender as DataGrid;
            if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                return;
            }

            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
                string valueToPaste = System.Windows.Clipboard.GetText();
                int location = ((Step)grid.CurrentCell.Item).No - 1;
                if (location > -1)
                {
                    Console.WriteLine("paste location" + location);
                    pasteStepDataFromClipBoard(location, valueToPaste);
                }
                for (int i = 0; i < Program.RootModel.Steps.Count; i++)
                {
                    Program.RootModel.Steps[i].No = i + 1;
                }
                return;
            }
            if (e.Key == Key.Insert && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                int location = ((Step)grid.CurrentCell.Item).No - 1;
                if (location > -1)
                {
                    Program.RootModel.Steps.Insert(location, new Step());
                    e.Handled = true;
                }
                for (int i = 0; i < Program.RootModel.Steps.Count; i++)
                {
                    Program.RootModel.Steps[i].No = i + 1;
                }
                return;
            }
            if (e.Key == Key.Delete && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
                if (Program.RootModel.Steps.Count == 1)
                {
                    return;
                }
                List<Step> lastedClear = new List<Step>();
                foreach (var item in grid.SelectedCells)
                {
                    lastedClear.Add((Step)item.Item);
                }
                foreach (Step item in lastedClear)
                {
                    Program.RootModel.Steps.Remove(item);
                }

                for (int i = 0; i < Program.RootModel.Steps.Count; i++)
                {
                    Program.RootModel.Steps[i].No = i + 1;
                }
                return;
            }

            if (grid.CurrentCell != null)
            {
                var cellContent = grid.CurrentCell.Column?.GetCellContent(grid.CurrentCell.Item);
                if (cellContent != null)
                {
                    var cell = (DataGridCell)cellContent.Parent;
                    if (!cell.IsEditing)
                    {
                        if (Char.TryParse(e.Key.ToString(), out _) || e.Key.ToString().Contains("NumPad") || e.Key == Key.Enter)
                        {
                            grid.BeginEdit();
                            cell.IsEditing = true;
                            cellContent.Focus();
                            e.Handled = e.Key == Key.Enter;
                        }
                    }
                    else
                    {
                        if (e.Key == Key.Enter)
                        {
                            grid.CommitEdit();
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


        private void Grid_LostFocus(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        #endregion

        #region Model Page
        /// step table actions
        private void TestStepsGridData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (TestStepsGridData.SelectedCells.Count > 0)
            {
                DataGridCellInfo cells = TestStepsGridData.SelectedCells[0];
                DataGrid grd = (DataGrid)sender;
                grd.CommitEdit(DataGridEditingUnit.Cell, true);

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

            if (TestStepsGridData.CurrentCell != null)
            {
                if (TestStepsGridData.CurrentCell.Column?.Header.ToString() == "No")
                {
                    foreach (var item in TestStepsGridData.Columns)
                    {
                        if (!TestStepsGridData.SelectedCells.Contains(new DataGridCellInfo(TestStepsGridData.CurrentCell.Item, item)))
                        {
                            TestStepsGridData.SelectedCells.Add(new DataGridCellInfo(TestStepsGridData.CurrentCell.Item, item));
                        }
                    }
                }
            }
        }

        private void TestStepsGridData_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((sender as DataGrid).SelectedCells != null)
            {
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);
            }
        }

        public void pasteStepDataFromClipBoard(int pasteLocation, String pasteValue)
        {
            if (pasteValue.Contains("No	QR Code	Test Context	CMD	Condition 1	Oper	Condition 2	Spect	Min	Max	Mode	Count	E-Jump	Remark	E-Loc	Skip"))
            {
                var pasteValues = pasteValue.Replace("\r\n", "\n").Split('\n');
                var location = pasteLocation;
                for (int i = 1; i < pasteValues.Length; i++)
                {
                    var item = pasteValues[i];
                    if (item != "")
                    {
                        var dataItem = item.Split('\t');
                        if (dataItem.Length == TestStepsGridData.Columns.Count)
                        {
                            Program.RootModel.Steps.Insert(location, new HVT.VTM.Base.Step()
                            {
                                No = Convert.ToInt32(dataItem[0]),
                                IMQSCode = dataItem[1],
                                TestContent = dataItem[2],
                                CMD = dataItem[3],
                                Condition1 = dataItem[4],
                                Oper = dataItem[5],
                                Condition2 = dataItem[6],
                                Spect = dataItem[7],
                                Min = dataItem[8],
                                Max = dataItem[9],
                                Mode = dataItem[10],
                                Count = dataItem[11],
                                Mem = dataItem[12],
                                ELoc = dataItem[13],
                                Remark = dataItem[14],
                                Skip = Convert.ToBoolean(dataItem[15])
                            });
                            location++;
                        }
                    }
                }
                for (int i = 0; i < Program.RootModel.Steps.Count; i++)
                {
                    Program.RootModel.Steps[i].No = i + 1;
                }
                TestStepsGridData.ItemsSource = null;
                TestStepsGridData.ItemsSource = Program.RootModel.Steps;
                //TestStepsGridData.Items.Refresh();
            }
        }

        private void TabControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsUnderTabHeader(e.OriginalSource as DependencyObject))
                CommitTables(modeltabControl);
        }

        private bool IsUnderTabHeader(DependencyObject control)
        {
            if (control is TabItem)
                return true;
            DependencyObject parent = VisualTreeHelper.GetParent(control);
            if (parent == null)
                return false;
            return IsUnderTabHeader(parent);
        }

        private void CommitTables(DependencyObject control)
        {
            if (control is DataGrid)
            {
                DataGrid grid = control as DataGrid;
                grid.CommitEdit(DataGridEditingUnit.Row, true);
                return;
            }
            int childrenCount = VisualTreeHelper.GetChildrenCount(control);
            for (int childIndex = 0; childIndex < childrenCount; childIndex++)
                CommitTables(VisualTreeHelper.GetChild(control, childIndex));
        }

        //PCB align tab

        public void PCB_Ui_Layout_Init()
        {


            nUD_PCBcount.Value = Program.RootModel.contruction.PCB_Count;
            nUD_X_axis_count.Value = Program.RootModel.contruction.PCB_X_axis_Count;
            cbbArrayLocation.SelectedIndex = (int)Program.RootModel.contruction.ArrayPosition;
            Align_PCB();
        }

        private void nUD_PCBcount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if ((sender as IntegerUpDown).Value != null)
            {

                int Count = (int)((sender as IntegerUpDown).Value);
                if ((sender as IntegerUpDown).Name == "nUD_PCBcount")
                {
                    Program.RootModel.contruction.PCB_Count = Count;
                }
                if ((sender as IntegerUpDown).Name == "nUD_X_axis_count")
                {
                    Program.RootModel.contruction.PCB_X_axis_Count = Count;
                }
                Align_PCB();
            }
        }

        private void Align_PCB()
        {
            PCBlayout.Dispatcher.Invoke(new Action(() => Program.RootModel.contruction.Align_PCB()));
            pnResult.Dispatcher.Invoke(new Action(() => Program.RootModel.contruction.AlignResult()));
            //if (PCBlayout != null)
            //{
            //    for (int i = 0; i < 8; i++)
            //    {
            //        PCBlayout.ColumnDefinitions[i].Width = new GridLength(0, GridUnitType.Star);
            //        PCBlayout.RowDefinitions[i].Height = new GridLength(0, GridUnitType.Star);
            //    }

            //    int colunm = 0;
            //    int row = 0;
            //    for (int i = 0; i < 8; i++)
            //    {
            //        if (i < Program.RootModel.contruction.PCB_Count)
            //        {
            //            PCB_LABEL[i].Visibility = Visibility.Visible;
            //            switch (Program.RootModel.contruction.ArrayPosition)
            //            {
            //                case Contruction.ArrayPositions.HorizontalTopLeft:
            //                    colunm = i % Program.RootModel.contruction.PCB_X_axis_Count;
            //                    row = i / Program.RootModel.contruction.PCB_X_axis_Count;
            //                    break;
            //                case Contruction.ArrayPositions.HorizontalTopRight:
            //                    colunm = Program.RootModel.contruction.PCB_X_axis_Count - i % Program.RootModel.contruction.PCB_X_axis_Count - 1;
            //                    row = i / Program.RootModel.contruction.PCB_X_axis_Count;
            //                    break;
            //                case Contruction.ArrayPositions.HorizontalBottomLeft:
            //                    colunm = i % Program.RootModel.contruction.PCB_X_axis_Count;
            //                    row = (Program.RootModel.contruction.PCB_Count / Program.RootModel.contruction.PCB_X_axis_Count) - i / Program.RootModel.contruction.PCB_X_axis_Count;
            //                    break;
            //                case Contruction.ArrayPositions.HorizontalBottomRight:
            //                    colunm = Program.RootModel.contruction.PCB_X_axis_Count - i % Program.RootModel.contruction.PCB_X_axis_Count - 1;
            //                    row = (Program.RootModel.contruction.PCB_Count / Program.RootModel.contruction.PCB_X_axis_Count) - i / Program.RootModel.contruction.PCB_X_axis_Count;
            //                    break;

            //                case Contruction.ArrayPositions.VerticalTopLeft:
            //                    colunm = i / Program.RootModel.contruction.PCB_X_axis_Count;
            //                    row = i % Program.RootModel.contruction.PCB_X_axis_Count;
            //                    break;
            //                case Contruction.ArrayPositions.VerticalTopRight:
            //                    colunm = (Program.RootModel.contruction.PCB_Count / Program.RootModel.contruction.PCB_X_axis_Count) - i / Program.RootModel.contruction.PCB_X_axis_Count;
            //                    row = i % Program.RootModel.contruction.PCB_X_axis_Count;
            //                    break;
            //                case Contruction.ArrayPositions.VerticalBottomLeft:
            //                    colunm = i / Program.RootModel.contruction.PCB_X_axis_Count;
            //                    row = Program.RootModel.contruction.PCB_X_axis_Count - i % Program.RootModel.contruction.PCB_X_axis_Count - 1;
            //                    break;
            //                case Contruction.ArrayPositions.VerticalBottomRight:
            //                    colunm = (Program.RootModel.contruction.PCB_Count / Program.RootModel.contruction.PCB_X_axis_Count) - i / Program.RootModel.contruction.PCB_X_axis_Count;
            //                    row = Program.RootModel.contruction.PCB_X_axis_Count - i % Program.RootModel.contruction.PCB_X_axis_Count - 1;
            //                    break;
            //                default:
            //                    break;
            //            }

            //            PCBlayout.ColumnDefinitions[colunm].Width = new GridLength(1, GridUnitType.Star);
            //            PCBlayout.RowDefinitions[row].Height = new GridLength(1, GridUnitType.Star);

            //            Grid.SetColumn(PCB_LABEL[i], colunm);
            //            Grid.SetRow(PCB_LABEL[i], row);
            //        }
            //        else
            //        {
            //            PCB_LABEL[i].Visibility = Visibility.Hidden;
            //        }
            //    }
            //}
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as System.Windows.Controls.ComboBox).SelectedIndex > -1)
            {
                Program.RootModel.contruction.ArrayPosition = (Contruction.ArrayPositions)(sender as System.Windows.Controls.ComboBox).SelectedIndex;
                Program.RootModel.contruction.Align_PCB();
            }
        }


        //Error position Tab

        bool IsAddingErrorPosition = false;
        Model.ErrorPosition newErrorPosition = new Model.ErrorPosition();

        private void btPCB_Error_Position_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (Error_Positions_Table.SelectedIndex >= 0 && Error_Positions_Table.SelectedIndex < Program.RootModel.ErrorPositions.Count)
            {
                int index = Error_Positions_Table.SelectedIndex;

                Program.RootModel.ErrorPositions.Remove((Model.ErrorPosition)Error_Positions_Table.SelectedItem);
                Canvas_PCB_Error_Mark.Children.Remove(((Model.ErrorPosition)Error_Positions_Table.SelectedItem).rect);

                for (int i = 0; i < Program.RootModel.ErrorPositions.Count; i++)
                {
                    Program.RootModel.ErrorPositions[i].No = i + 1;
                }
                Error_Positions_Table.Items.Refresh();
                Error_Positions_Table.SelectedIndex = index < Program.RootModel.ErrorPositions.Count ? index : index > 0 ? index - 1 : 0;
            }
        }

        private void Error_Positions_Table_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("Select change");
        }

        private void imgPCB_Error_Position_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsAddingErrorPosition)
            {
                Program.RootModel.ErrorPositions.Add(new Model.ErrorPosition()
                {
                    No = Program.RootModel.ErrorPositions.Count + 1,
                    Y = e.GetPosition(Canvas_PCB_Error_Mark).Y,
                    X = e.GetPosition(Canvas_PCB_Error_Mark).X,
                    lbTop = e.GetPosition(Canvas_PCB_Error_Mark).Y,
                    lbLeft = e.GetPosition(Canvas_PCB_Error_Mark).X,
                    Position = e.GetPosition(imgPCB_Error_Position).X.ToString("F2") + " ~ " + e.GetPosition(imgPCB_Error_Position).Y.ToString("F2"),
                    Width = 0,
                    Height = 0
                });

                Canvas.SetTop(Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].rect, Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].Y);
                Canvas.SetLeft(Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].rect, Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].X);

                Canvas_PCB_Error_Mark.Children.Add(Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].rect);

            }
        }

        private void imgPCB_Error_Position_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsAddingErrorPosition)
            {
                //Program.RootModel.ErrorPositions.Add(newErrorPosition);
                Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].X = Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].lbLeft;
                Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].Y = Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].lbTop;

                //Canvas.SetTop(Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].rect, Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].Y);
                //Canvas.SetLeft(Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].rect, Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].X);

                Error_Positions_Table.ItemsSource = Program.RootModel.ErrorPositions;
                Error_Positions_Table.Items.Refresh();
                IsAddingErrorPosition = false;
                btPCB_Error_Position_Add.IsChecked = false;
            }
        }

        private void Canvas_PCB_Error_Mark_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (IsAddingErrorPosition)
                {
                    Console.WriteLine("X : {0} ---- Y : {1}", e.GetPosition(Canvas_PCB_Error_Mark).X, e.GetPosition(Canvas_PCB_Error_Mark).Y);

                    if (e.GetPosition(Canvas_PCB_Error_Mark).X >= Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].X)
                    {
                        Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].Width = e.GetPosition(Canvas_PCB_Error_Mark).X - Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].lbLeft;
                    }
                    else
                    {
                        //var X_temp = Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].lbLeft;

                        Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].lbLeft = e.GetPosition(Canvas_PCB_Error_Mark).X;
                        Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].Width = Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].X - e.GetPosition(Canvas_PCB_Error_Mark).X;
                        Canvas.SetLeft(Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].rect, Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].lbLeft);
                    }

                    if (e.GetPosition(Canvas_PCB_Error_Mark).Y >= Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].Y)
                    {
                        Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].Height = e.GetPosition(Canvas_PCB_Error_Mark).Y - Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].lbTop;
                    }
                    else
                    {
                        //var X_temp = Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].lbLeft;

                        Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].lbTop = e.GetPosition(Canvas_PCB_Error_Mark).Y;
                        Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].Height = Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].Y - e.GetPosition(Canvas_PCB_Error_Mark).Y;
                        Canvas.SetTop(Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].rect, Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].lbTop);
                    }
                }
            }
        }

        private void btPCB_Error_Position_Browser_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Open PCB Image";
            openFile.Filter = "JPG files (*.jpg)|*.jpg|Bitmap files (*.bmp)|*.*";
            openFile.RestoreDirectory = true;
            if (openFile.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            Uri fileUri = new Uri(openFile.FileName);
            tbPCB_Error_Position_Path.Text = openFile.FileName;
            imgPCB_Error_Position.Source = new BitmapImage(fileUri);
        }

        private void btPCB_Error_Position_Add_Checked(object sender, RoutedEventArgs e)
        {
            IsAddingErrorPosition = true;
        }

        private void btPCB_Error_Position_Add_Unchecked(object sender, RoutedEventArgs e)
        {
            IsAddingErrorPosition = false;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void btReference_Copy_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }



        #endregion
        
        #region MUX_LEVEL
        public void ModelPage_UpdateLayout(Contruction contruction)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Program.MUX_CARD.UpdateCardSelect(contruction.PCB_Count, wrapPanelMuxSelect);
            }));
        }

        #endregion

        #region Port Config
        // Port 1

        private void Port1_Kind_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UUT_Config.PortKind portKind;
            if (Enum.TryParse<UUT_Config.PortKind>((sender as ComboBox).SelectedItem.ToString(), out portKind))
                Program.RootModel.P1_Config.Kind = portKind;
        }

        private void Port1_baudrate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                var item = sender as ComboBox;
                ComboBoxItem typeItem = (ComboBoxItem)item.SelectedItem;
                int baurate = Convert.ToInt32(typeItem.Content.ToString());
                Console.WriteLine(baurate);
                Program.RootModel.P1_Config.Baudrate = baurate;
            }
        }

        private void Port1_Parity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                Parity parity;
                if (Enum.TryParse<Parity>((sender as ComboBox).SelectedItem.ToString(), out parity))
                    Program.RootModel.P1_Config.Parity = parity;
            }
        }

        private void Port1_Databits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                var item = sender as ComboBox;
                var typeItem = item.SelectedItem as ComboBoxItem;
                int databits = Convert.ToInt32(typeItem.Content.ToString());
                Program.RootModel.P1_Config.DataBit = databits;
            }
        }

        private void Port1_Stopbit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                StopBits stopbit;
                if (Enum.TryParse<StopBits>((sender as ComboBox).SelectedItem.ToString(), out stopbit))
                    Program.RootModel.P1_Config.StopBits = stopbit;
            }
        }

        private void Port1_UsePrefix1_Click(object sender, RoutedEventArgs e)
        {
            Program.RootModel.P1_Config.UsePrefix1 = (bool)(sender as CheckBox).IsChecked;
        }

        private void Port1_UsePrefix2_Click(object sender, RoutedEventArgs e)
        {
            Program.RootModel.P1_Config.UsePrefix2 = (bool)(sender as CheckBox).IsChecked;
        }

        private void Port1_UseSuffix_Click(object sender, RoutedEventArgs e)
        {
            Program.RootModel.P1_Config.UseSuffix = (bool)(sender as CheckBox).IsChecked;
        }

        private void Port1_FixLength_Click(object sender, RoutedEventArgs e)
        {
            Program.RootModel.P1_Config.UseLengFixed = (bool)(sender as CheckBox).IsChecked;
        }

        private void nUD_P1TX_FrefixData1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Program.RootModel.P1_Config.Prefix1 = (byte)(sender as IntegerUpDown).Value;
        }

        private void nUD_P1TX_FrefixData2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Program.RootModel.P1_Config.Prefix2 = (byte)(sender as IntegerUpDown).Value;
        }

        private void GetPortSetting()
        {
            Program.RootModel.P1_Config.Use = (bool)cbUsePort1.IsChecked;
            Program.RootModel.P1_Config.Kind = Enum.TryParse<UUT_Config.PortKind>(Port1_Kind.Text, out UUT_Config.PortKind pkind) ? pkind : UUT_Config.PortKind.TTL;
            Program.RootModel.P1_Config.Baudrate = Int32.TryParse(Port1_baudrate.Text, out int baud)? baud: 9600;
            Program.RootModel.P1_Config.Parity = Enum.TryParse<Parity>(Port1_Parity.Text, out Parity parity) ? parity : Parity.None;
            Program.RootModel.P1_Config.DataBit = Int32.TryParse(Port1_Databits.Text, out int databit) ? databit : 8;
            Program.RootModel.P1_Config.StopBits = Enum.TryParse<StopBits>(Port1_Stopbit.Text, out StopBits stbit) ? stbit : StopBits.None;

            Program.RootModel.P1_Config.UsePrefix1 = (bool)Port1_UsePrefix1.IsChecked;
            Program.RootModel.P1_Config.UsePrefix1 = (bool)Port1_UsePrefix2.IsChecked;
            Program.RootModel.P1_Config.UseSuffix = (bool)Port1_UseSuffix.IsChecked;
            Program.RootModel.P1_Config.UseLengFixed= (bool)Port1_FixLength.IsChecked;

            Program.RootModel.P1_Config.Prefix1 = (int)nUD_P1TX_PrefixData1.Value;
            Program.RootModel.P1_Config.Prefix2 = (int)nUD_P1TX_PrefixData2.Value;
            Program.RootModel.P1_Config.Suffix = (int)nUD_P1TX_SuffixData.Value;
            Program.RootModel.P1_Config.LenghtFixed = (int)nUD_P1TX_FixLenghtData.Value;

            Program.RootModel.P1_Config.Checksum = (CheckSumType)Enum.ToObject(typeof(CheckSumType), Port1_Checksum.SelectedIndex);
            Program.RootModel.P1_Config.StartChecksumCal = (int)nUD_P1TX_CheckSumRageStart.Value;
            Program.RootModel.P1_Config.EndChecksumCal = (int)nUD_P1TX_CheckSumRageEnd.Value;

            Program.RootModel.P1_Config.UseRxPrefix1 = (bool)Port1_RX_UsePrefix1.IsChecked;
            Program.RootModel.P1_Config.UseRxPrefix1 = (bool)Port1_RX_UsePrefix2.IsChecked;
            Program.RootModel.P1_Config.UseRxSuffix = (bool)Port1_RX_UseSuffix.IsChecked;
            Program.RootModel.P1_Config.UseRxLengFixed = (bool)Port1_RX_FixLenght.IsChecked;

            Program.RootModel.P1_Config.RxPrefix1 = (int)nUD_P1RX_PrefixData1.Value;
            Program.RootModel.P1_Config.RxPrefix2 = (int)nUD_P1RX_PrefixData2.Value;
            Program.RootModel.P1_Config.RxSuffix = (int)nUD_P1RX_SuffixData.Value;
            Program.RootModel.P1_Config.RxLenghtFixed = (int)nUD_P1RX_FixLenghtData.Value;



            Program.RootModel.P2_Config.Use = (bool)cbUsePort2.IsChecked;
            Program.RootModel.P2_Config.Kind = Enum.TryParse<UUT_Config.PortKind>(Port2_Kind.Text, out UUT_Config.PortKind pkind2) ? pkind2 : UUT_Config.PortKind.TTL;
            Program.RootModel.P2_Config.Baudrate = Int32.TryParse(Port2_baudrate.Text, out int baud2) ? baud : 9600;
            Program.RootModel.P2_Config.Parity = Enum.TryParse<Parity>(Port2_Parity.Text, out Parity parity2) ? parity2 : Parity.None;
            Program.RootModel.P2_Config.DataBit = Int32.TryParse(Port2_Databits.Text, out int databit2) ? databit2 : 8;
            Program.RootModel.P2_Config.StopBits = Enum.TryParse<StopBits>(Port2_Stopbit.Text, out StopBits stbit2) ? stbit2 : StopBits.None;

            Program.RootModel.P2_Config.UsePrefix1 = (bool)Port2_UsePrefix1.IsChecked;
            Program.RootModel.P2_Config.UsePrefix1 = (bool)Port2_UsePrefix2.IsChecked;
            Program.RootModel.P2_Config.UseSuffix = (bool)Port2_UseSuffix.IsChecked;
            Program.RootModel.P2_Config.UseLengFixed = (bool)Port2_FixLength.IsChecked;

            Program.RootModel.P2_Config.Prefix1 = (int)nUD_P2TX_PrefixData1.Value;
            Program.RootModel.P2_Config.Prefix2 = (int)nUD_P2TX_PrefixData2.Value;
            Program.RootModel.P2_Config.Suffix = (int)nUD_P2TX_SuffixData.Value;
            Program.RootModel.P2_Config.LenghtFixed = (int)nUD_P2TX_FixLenghtData.Value;

            Program.RootModel.P2_Config.Checksum = (CheckSumType)Enum.ToObject(typeof(CheckSumType), Port2_Checksum.SelectedIndex);
            Program.RootModel.P2_Config.StartChecksumCal = (int)nUD_P2TX_CheckSumRageStart.Value;
            Program.RootModel.P2_Config.EndChecksumCal = (int)nUD_P2TX_CheckSumRageEnd.Value;

            Program.RootModel.P2_Config.UseRxPrefix1 = (bool)Port2_RX_UsePrefix1.IsChecked;
            Program.RootModel.P2_Config.UseRxPrefix1 = (bool)Port2_RX_UsePrefix2.IsChecked;
            Program.RootModel.P2_Config.UseRxSuffix = (bool)Port2_RX_UseSuffix.IsChecked;
            Program.RootModel.P2_Config.UseRxLengFixed = (bool)Port2_RX_FixLenght.IsChecked;

            Program.RootModel.P2_Config.RxPrefix1 = (int)nUD_P2RX_PrefixData1.Value;
            Program.RootModel.P2_Config.RxPrefix2 = (int)nUD_P2RX_PrefixData2.Value;
            Program.RootModel.P2_Config.RxSuffix = (int)nUD_P2RX_SuffixData.Value;
            Program.RootModel.P2_Config.RxLenghtFixed = (int)nUD_P2RX_FixLenghtData.Value;

        }

        #endregion

        #region UUT

        ///////// Naming 

        private void btOpenQRNaming_Click(object sender, RoutedEventArgs e)
        {
            Program.OpenQRNaming();
        }
        private void btOpenTxNaming_Click(object sender, RoutedEventArgs e)
        {
            Program.OpenTxNaming();
        }
        private void btOpenRxNaming_Click(object sender, RoutedEventArgs e)
        {
            Program.OpenRxNaming();
        }

        private void btSaveQRnaming_Click(object sender, RoutedEventArgs e)
        {
            Program.SaveQRNaming();
        }
        private void btSaveRxnaming_Click(object sender, RoutedEventArgs e)
        {
            Program.SaveRxNaming();
        }
        private void btSaveTxnaming_Click(object sender, RoutedEventArgs e)
        {
            Program.SaveTxNaming();
        }
        private void dgRX_data_naming_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex().ToString();
        }

        private void dgTX_data_naming_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var grid = sender as DataGrid;

            if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                return;
            }
            if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Program.SaveTxNaming();
                return;
            }
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (grid.CurrentCell != null)
                {
                    e.Handled = true;
                    int location = ((TxData)grid.CurrentCell.Item).No;
                    string valueToPaste = System.Windows.Clipboard.GetText();
                    Console.WriteLine(valueToPaste);
                    if (location > -1)
                    {
                        Console.WriteLine("paste location" + location);
                        Program.pasteTxNamingFromClipBoard(location, valueToPaste);
                    }
                }
                for (int i = 0; i < Program.Naming.TxDatas.Count; i++)
                {
                    Program.Naming.TxDatas[i].No = i + 1;
                }
                return;
            }
            if (e.Key == Key.Insert && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (grid.CurrentCell != null)
                {
                    e.Handled = true;
                    int location = ((TxData)grid.CurrentCell.Item).No;
                    if (location > -1)
                    {
                        Program.Naming.TxDatas.Add(new TxData());
                        e.Handled = true;
                    }
                }
                for (int i = 0; i < Program.Naming.TxDatas.Count; i++)
                {
                    Program.Naming.TxDatas[i].No = i + 1;
                }
                return;
            }

            if (e.Key == Key.Delete && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
                foreach (var item in grid.SelectedCells)
                {
                    try
                    {
                        Program.Naming.TxDatas.Remove((TxData)item.Item);
                    }
                    catch (Exception)
                    { }
                }
                for (int i = 0; i < Program.Naming.TxDatas.Count; i++)
                {
                    Program.Naming.TxDatas[i].No = i + 1;
                }
                return;
            }

            if (grid.CurrentCell != null)
            {
                var cellContent = grid.CurrentCell.Column?.GetCellContent(grid.CurrentCell.Item);
                if (cellContent != null)
                {
                    var cell = (DataGridCell)cellContent.Parent;
                    if (!cell.IsEditing)
                    {
                        if (Char.TryParse(e.Key.ToString(), out _) || e.Key.ToString().Contains("NumPad") || e.Key == Key.Enter)
                        {
                            grid.BeginEdit();
                            cell.IsEditing = true;
                            cellContent.Focus();
                            e.Handled = e.Key == Key.Enter;
                        }
                    }
                    else
                    {
                        if (e.Key == Key.Enter)
                        {
                            grid.CommitEdit();
                            e.Handled = true;
                        }
                    }
                }
            }

        }
        private void dgRX_data_naming_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var grid = sender as DataGrid;
            if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                return;
            }
            if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Program.SaveRxNaming();
                return;
            }
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
                string valueToPaste = System.Windows.Clipboard.GetText();
                int location = ((RxData)grid.CurrentCell.Item).No;
                if (location > -1)
                {
                    Console.WriteLine("paste location" + location);
                    Program.pasteRxNamingFromClipBoard(location, valueToPaste);
                }
                for (int i = 0; i < Program.Naming.RxDatas.Count; i++)
                {
                    Program.Naming.RxDatas[i].No = i + 1;
                }
                return;
            }
            if (e.Key == Key.Insert && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                int location = ((RxData)grid.CurrentCell.Item).No;
                if (location > -1)
                {
                    Program.Naming.RxDatas.Insert(location, new RxData());
                    e.Handled = true;
                }
                for (int i = 0; i < Program.Naming.RxDatas.Count; i++)
                {
                    Program.Naming.RxDatas[i].No = i + 1;
                }
                return;
            }
            if (e.Key == Key.Delete && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
                foreach (var item in grid.SelectedCells)
                {
                    try
                    {
                        Program.Naming.RxDatas.Remove((RxData)item.Item);
                    }
                    catch (Exception)
                    { }
                }
                for (int i = 0; i < Program.Naming.RxDatas.Count; i++)
                {
                    Program.Naming.RxDatas[i].No = i + 1;
                }
                return;
            }

            if (grid.CurrentCell != null)
            {
                var cellContent = grid.CurrentCell.Column?.GetCellContent(grid.CurrentCell.Item);
                if (cellContent != null)
                {
                    var cell = (DataGridCell)cellContent.Parent;
                    if (!cell.IsEditing)
                    {
                        if (Char.TryParse(e.Key.ToString(), out _) || e.Key.ToString().Contains("NumPad") || e.Key == Key.Enter)
                        {
                            grid.BeginEdit();
                            cell.IsEditing = true;
                            cellContent.Focus();
                            e.Handled = e.Key == Key.Enter;
                        }
                    }
                    else
                    {
                        if (e.Key == Key.Enter)
                        {
                            grid.CommitEdit();
                            e.Handled = true;
                        }
                    }
                }
            }

        }



        private void dgQRcodeNameCode_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var grid = sender as DataGrid;
            if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                return;
            }
            if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Program.SaveQRNaming();
                return;
            }
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
                string valueToPaste = System.Windows.Clipboard.GetText();
                int location = ((QRData)grid.CurrentCell.Item).No;
                if (location > -1)
                {
                    Console.WriteLine("paste location" + location);
                    Program.pasteQrNamingFromClipBoard(location, valueToPaste);
                }
                for (int i = 0; i < Program.Naming.RxDatas.Count; i++)
                {
                    Program.Naming.RxDatas[i].No = i + 1;
                }
                return;
            }
            if (e.Key == Key.Insert && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                int location = ((QRData)grid.CurrentCell.Item).No;
                if (location > -1)
                {
                    Program.Naming.QRDatas.Insert(location, new QRData());
                    e.Handled = true;
                }
                for (int i = 0; i < Program.Naming.RxDatas.Count; i++)
                {
                    Program.Naming.RxDatas[i].No = i + 1;
                }
                return;
            }
            if (e.Key == Key.Delete && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
                List<QRData> lastedClear = new List<QRData>();
                foreach (var item in grid.SelectedCells)
                {
                    lastedClear.Add((QRData)item.Item);
                }
                foreach (QRData item in lastedClear)
                {
                    Program.Naming.QRDatas.Remove(item);
                }
                for (int i = 0; i < Program.Naming.RxDatas.Count; i++)
                {
                    Program.Naming.RxDatas[i].No = i + 1;
                }
                return;
            }

            if (e.Key == Key.Enter)
            {
                if (grid.CurrentCell != null)
                {
                    var cellContent = grid.CurrentCell.Column?.GetCellContent(grid.CurrentCell.Item);
                    if (cellContent != null)
                    {
                        var cell = (DataGridCell)cellContent.Parent;
                        if (!cell.IsEditing)
                        {
                            grid.BeginEdit();
                            cell.IsEditing = true;
                            cellContent.Focus();
                            e.Handled = true;
                        }
                        else
                        {
                            grid.CommitEdit();
                            cellContent.Focus();
                            e.Handled = true;
                        }
                    }
                }

            }
        }

        //////// UUT port config






        #endregion

    }
}
