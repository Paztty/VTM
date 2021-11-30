
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
using HVT.VTM.Base;
using DataGridCell = System.Windows.Controls.DataGridCell;
using DataGrid = System.Windows.Controls.DataGrid;
using System.Windows.Media;

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

            Program.UUTpageInit(dgTX_data_naming, dgRX_data_naming, dgQRcodeNameCode, tbQRcodeLink);
            Program.MuxUIInit(MUX_Channels_Table1, MUX_Channels_Table2);
        }



        #region Model Action
        //Model Init see at MainWindow.cs
        //Event 
        private void Model_LoadFinish_ModelPage(object sender, EventArgs e)
        {
            tbModelName.Text = Program.RootModel.Name;
            tbModelNamePath.Text = Program.RootModel.Path;
            TestStepsGridData.ItemsSource = null;
            TestStepsGridData.ItemsSource = new ObservableCollection<Step>(Program.RootModel.Steps);
            Error_Positions_Table.ItemsSource = Program.RootModel.ErrorPositions;
        }

        private void btSaveModel_Click(object sender, RoutedEventArgs e)
        {
            Program.SaveModel();
            Program.RootModel.LoadFinishEvent();
        }

        private void btSaveAsModel_Click(object sender, RoutedEventArgs e)
        {
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

        private void TestStepsGridData_UnloadingRow(object sender, DataGridRowEventArgs e)
        {

        }

        private void dgTX_data_naming_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex().ToString();
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

                //Step item = cells.Item as Step;
                //if (item != null)
                //{
                //    Program.Command.cmd = item.cmd;
                //    TestStepsGridRemark.ItemsSource = null;
                //    TestStepsGridRemark.ItemsSource = Program.Command.CMD;
                //    StepDecription.Text = Program.Command.CMD[0].Description;

                //    if (TestStepsGridData.CurrentColumn != null)
                //    {
                //        TestStepsGridRemark.CurrentCell = new DataGridCellInfo(TestStepsGridRemark.Items[0], TestStepsGridData.CurrentColumn);
                //        TestStepsGridRemark.SelectedCells.Clear();
                //        TestStepsGridRemark.SelectedCells.Add(TestStepsGridRemark.CurrentCell);
                //    }
                //}
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
        private void TestStepsGridData_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //if (e.Column.Header.ToString() == "CMD")
            //{
            //    Program.Command.cmd = ((HVT.VTM.Base.Step)e.Row.Item).cmd;
            //    TestStepsGridRemark.ItemsSource = null;
            //    TestStepsGridRemark.ItemsSource = Program.Command.CMD;
            //    StepDecription.Text = Program.Command.CMD[0].Description;
            //}
        }

        private void TestStepsGridData_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((sender as DataGrid).SelectedCells != null)
            {
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);
            }
        }
        private void TestStepsGridData_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
                string valueToPaste = System.Windows.Clipboard.GetText();
                var location = TestStepsGridData.Items.IndexOf(TestStepsGridData.CurrentItem);
                Console.WriteLine("paste location" + location);
                Console.WriteLine(valueToPaste);
                if (location > -1)
                {
                    Console.WriteLine("paste location" + location);
                    pasteStepDataFromClipBoard(location, valueToPaste);
                    TestStepsGridData.Items.Refresh();
                }
            }

            if (e.Key == Key.Delete && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
                var location = TestStepsGridData.Items.IndexOf(TestStepsGridData.CurrentItem);
                Console.WriteLine("paste location" + location);
                if (location > -1)
                {
                    Program.RootModel.Steps.RemoveAt(location);
                    TestStepsGridData.Items.Refresh();
                }
            }

            if (e.Key == Key.Insert && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
                var location = TestStepsGridData.Items.IndexOf(TestStepsGridData.CurrentItem);
                Console.WriteLine("paste location" + location);
                if (location > -1)
                {
                    Program.RootModel.Steps.Insert(location, new Step());
                    for (int i = 0; i < Program.RootModel.Steps.Count; i++)
                    {
                        Program.RootModel.Steps[i].No = i + 1;
                    }
                    TestStepsGridData.Items.Refresh();
                }
            }


        }
        public void pasteStepDataFromClipBoard(int pasteLocation, String pasteValue)
        {
            if (pasteValue.Contains("QR Code	Test Context	CMD	Condition 1	Oper	Condition 2	Spect	Min	Max	Mode	Count	E-Jump	Remark	E-Loc	Skip"))
            {
                var pasteValues = pasteValue.Replace("\r\n", "\n").Split('\n');
                var location = pasteLocation;
                for (int i = 1; i < pasteValues.Length; i++)
                {
                    var item = pasteValues[i];
                    var dataItem = item.Split('\t');
                    if (dataItem.Length == TestStepsGridData.Columns.Count)
                    {
                        Program.RootModel.Steps.Insert(location, new HVT.VTM.Base.Step()
                        {
                            //No = Convert.ToInt32(dataItem[0]),
                            IMQSCode = dataItem[0],
                            TestContent = dataItem[1],
                            CMD = dataItem[2],
                            Condition1 = dataItem[3],
                            Oper = dataItem[4],
                            Condition2 = dataItem[5],
                            Spect = dataItem[6],
                            Min = dataItem[7],
                            Max = dataItem[8],
                            Mode = dataItem[9],
                            Count = dataItem[10],
                            Mem = dataItem[11],
                            ELoc = dataItem[12],
                            Remark = dataItem[13],
                            Skip = Convert.ToBoolean(dataItem[14])
                        });
                        location++;
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

        #region UUT
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
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
                int location = dgTX_data_naming.SelectedIndex;
                string valueToPaste = System.Windows.Clipboard.GetText();
                Console.WriteLine(valueToPaste);
                if (location > -1)
                {
                    Console.WriteLine("paste location" + location);
                    Program.pasteTxNamingFromClipBoard(location, valueToPaste);
                }
            }
            if (e.Key == Key.Add && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Program.Naming.TxDatas.Add(new TxData());
                e.Handled = true;
            }
            if (e.Key == Key.Subtract && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
                int location = dgTX_data_naming.SelectedIndex;
                if (location > -1)
                {
                    if (Program.Naming.TxDatas.Count > 1)
                    {
                        Program.Naming.TxDatas.RemoveAt(location);
                        if (location < Program.Naming.TxDatas.Count)
                        {

                            dgTX_data_naming.SelectedIndex = location;
                            dgTX_data_naming.Focus();
                        }
                        else
                        {
                            dgTX_data_naming.SelectedIndex = location - 1;
                            dgTX_data_naming.Focus();
                        }
                    }
                }
            }
        }
        private void dgRX_data_naming_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
                int location = dgRX_data_naming.SelectedIndex;
                string valueToPaste = System.Windows.Clipboard.GetText();
                Console.WriteLine(valueToPaste);
                if (location > -1)
                {
                    Console.WriteLine("paste location" + location);
                    Program.pasteRxNamingFromClipBoard(location, valueToPaste);
                }
            }
            if (e.Key == Key.Add && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Program.Naming.RxDatas.Add(new RxData());
                e.Handled = true;
            }
            if (e.Key == Key.Subtract && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
                int location = dgRX_data_naming.SelectedIndex;
                if (location > -1)
                {
                    if (Program.Naming.RxDatas.Count > 1)
                    {
                        Program.Naming.RxDatas.RemoveAt(location);
                        if (location < Program.Naming.RxDatas.Count)
                        {

                            dgRX_data_naming.SelectedIndex = location;
                            dgRX_data_naming.Focus();
                        }
                        else
                        {
                            dgRX_data_naming.SelectedIndex = location - 1;
                            dgRX_data_naming.Focus();
                        }
                    }
                }

            }
        }

        private void dgQRcodeNameCode_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
                int location = dgQRcodeNameCode.SelectedIndex;
                string valueToPaste = System.Windows.Clipboard.GetText();
                Console.WriteLine(valueToPaste);
                if (location > -1)
                {
                    Console.WriteLine("paste location" + location);
                    Program.pasteQrNamingFromClipBoard(location, valueToPaste);
                }
            }
        }
        #endregion

        // PreviewMouseDown event handler on the TabControl

    }
}
