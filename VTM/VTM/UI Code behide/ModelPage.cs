
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using HVT.VTM.Core;
using HVT.VTM.Program;

namespace VTM
{
    public partial class MainWindow : Window
    {
        #region Model Action
        //Model Init see at MainWindow.cs
        //Event 
        private void Model_LoadFinish_ModelPage(object sender, EventArgs e)
        {
            tbModelName.Text = Program.RootModel.Name;
            tbModelNamePath.Text = Program.RootModel.Path;
            TestStepsGridData.ItemsSource = null;
            TestStepsGridData.ItemsSource = new ObservableCollection<Model.Step>(Program.RootModel.Steps);
            Error_Positions_Table.ItemsSource = Program.RootModel.ErrorPositions;
        }
        #endregion

        #region Model Page
        public void LoadModelPage()
        {
            LoadNamingFile(true);

            //PCB panel layout load
            PCB_LABEL.Add(PCB1);
            PCB_LABEL.Add(PCB2);
            PCB_LABEL.Add(PCB3);
            PCB_LABEL.Add(PCB4);
            PCB_LABEL.Add(PCB5);
            PCB_LABEL.Add(PCB6);
            PCB_LABEL.Add(PCB7);
            PCB_LABEL.Add(PCB8);

            PCB_Ui_Layout_Init();

        }

        // Load NamingFile

        public void LoadNamingFile(bool IsFileDefault)
        {
            if (IsFileDefault)
            {
                dgTX_data_naming.ItemsSource = Program.RootModel.SerialNaming.TxDatas;
                dgRX_data_naming.ItemsSource = Program.RootModel.SerialNaming.RxDatas;
                // load data to dg tx UUT at manual page

            }
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
            //int Count = (int)((sender as IntegerUpDown).Value);
            //if ((sender as IntegerUpDown).Name == "nUD_PCBcount")
            //{
            //    Program.RootModel.contruction.PCB_Count = Count;
            //}
            //if ((sender as IntegerUpDown).Name == "nUD_X_axis_count")
            //{
            //    Program.RootModel.contruction.PCB_X_axis_Count = Count;
            //}
            //Align_PCB();
        }

        private void Align_PCB()
        {
            if (PCBlayout != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    PCBlayout.ColumnDefinitions[i].Width = new GridLength(0, GridUnitType.Star);
                    PCBlayout.RowDefinitions[i].Height = new GridLength(0, GridUnitType.Star);
                }

                int colunm = 0;
                int row = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (i < Program.RootModel.contruction.PCB_Count)
                    {
                        PCB_LABEL[i].Visibility = Visibility.Visible;
                        switch (Program.RootModel.contruction.ArrayPosition)
                        {
                            case Model.Contruction.ArrayPositions.HorizontalTopLeft:
                                colunm = i % Program.RootModel.contruction.PCB_X_axis_Count;
                                row = i / Program.RootModel.contruction.PCB_X_axis_Count;
                                break;
                            case Model.Contruction.ArrayPositions.HorizontalTopRight:
                                colunm = Program.RootModel.contruction.PCB_X_axis_Count - i % Program.RootModel.contruction.PCB_X_axis_Count - 1;
                                row = i / Program.RootModel.contruction.PCB_X_axis_Count;
                                break;
                            case Model.Contruction.ArrayPositions.HorizontalBottomLeft:
                                colunm = i % Program.RootModel.contruction.PCB_X_axis_Count;
                                row = (Program.RootModel.contruction.PCB_Count / Program.RootModel.contruction.PCB_X_axis_Count) - i / Program.RootModel.contruction.PCB_X_axis_Count;
                                break;
                            case Model.Contruction.ArrayPositions.HorizontalBottomRight:
                                colunm = Program.RootModel.contruction.PCB_X_axis_Count - i % Program.RootModel.contruction.PCB_X_axis_Count - 1;
                                row = (Program.RootModel.contruction.PCB_Count / Program.RootModel.contruction.PCB_X_axis_Count) - i / Program.RootModel.contruction.PCB_X_axis_Count;
                                break;

                            case Model.Contruction.ArrayPositions.VerticalTopLeft:
                                colunm = i / Program.RootModel.contruction.PCB_X_axis_Count;
                                row = i % Program.RootModel.contruction.PCB_X_axis_Count;
                                break;
                            case Model.Contruction.ArrayPositions.VerticalTopRight:
                                colunm = (Program.RootModel.contruction.PCB_Count / Program.RootModel.contruction.PCB_X_axis_Count) - i / Program.RootModel.contruction.PCB_X_axis_Count;
                                row = i % Program.RootModel.contruction.PCB_X_axis_Count;
                                break;
                            case Model.Contruction.ArrayPositions.VerticalBottomLeft:
                                colunm = i / Program.RootModel.contruction.PCB_X_axis_Count;
                                row = Program.RootModel.contruction.PCB_X_axis_Count - i % Program.RootModel.contruction.PCB_X_axis_Count - 1;
                                break;
                            case Model.Contruction.ArrayPositions.VerticalBottomRight:
                                colunm = (Program.RootModel.contruction.PCB_Count / Program.RootModel.contruction.PCB_X_axis_Count) - i / Program.RootModel.contruction.PCB_X_axis_Count;
                                row = Program.RootModel.contruction.PCB_X_axis_Count - i % Program.RootModel.contruction.PCB_X_axis_Count - 1;
                                break;
                            default:
                                break;
                        }

                        PCBlayout.ColumnDefinitions[colunm].Width = new GridLength(1, GridUnitType.Star);
                        PCBlayout.RowDefinitions[row].Height = new GridLength(1, GridUnitType.Star);

                        Grid.SetColumn(PCB_LABEL[i], colunm);
                        Grid.SetRow(PCB_LABEL[i], row);
                    }
                    else
                    {
                        PCB_LABEL[i].Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Align_PCB();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as System.Windows.Controls.ComboBox).SelectedIndex > -1)
            {
                Program.RootModel.contruction.ArrayPosition = (Model.Contruction.ArrayPositions)(sender as System.Windows.Controls.ComboBox).SelectedIndex;
                Align_PCB();
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
                Canvas_PCB_Error_Mark.Children.Remove(((Model.ErrorPosition)Error_Positions_Table.SelectedItem).label);
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

                Canvas.SetTop(Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].label, Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].Y);
                Canvas.SetLeft(Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].label, Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].X);

                Canvas.SetTop(Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].rect, Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].Y);
                Canvas.SetLeft(Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].rect, Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].X);

                Canvas_PCB_Error_Mark.Children.Add(Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].rect);
                Canvas_PCB_Error_Mark.Children.Add(Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].label);

            }
        }

        private void imgPCB_Error_Position_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsAddingErrorPosition)
            {
                //Program.RootModel.ErrorPositions.Add(newErrorPosition);
                Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].X = Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].lbLeft;
                Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].Y = Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].lbTop;

                Canvas.SetTop(Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].label, Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].Y);
                Canvas.SetLeft(Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].label, Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].X);

                //Canvas.SetTop(Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].rect, Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].Y);
                //Canvas.SetLeft(Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].rect, Program.RootModel.ErrorPositions[Program.RootModel.ErrorPositions.Count - 1].X);

                Error_Positions_Table.ItemsSource = Program.RootModel.ErrorPositions;
                Error_Positions_Table.Items.Refresh();
                IsAddingErrorPosition = false;
                btPCB_Error_Position_Add.IsChecked = false;
            }
        }

        public void Error_Mark_Draw()
        {

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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}
