using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;

namespace VTM
{
    /// <summary>
    /// Interaction logic for ModelPage.xaml
    /// </summary>

    public partial class ModelPage : Page
    {
        Model model = new Model();
        Model MainModel = new Model();

        MUX MachineMux = new MUX();

        List<Label> PCB_LABEL = new List<Label>();

        public ModelPage(Model model)
        {
            InitializeComponent();
            MainModel = model;
            MainModel.LoadFinish += Model_LoadFinish;
            //this.model = model;
            //this.model.LoadFinish += Model_LoadFinish;

            Error_Positions_Table.ItemsSource = model.ErrorPositions;

            MUX_Channels_Table.ItemsSource = null;
            MUX_Channels_Table.ItemsSource = MachineMux.Channels;

            PCB_LABEL.Add(PCB1);
            PCB_LABEL.Add(PCB2);
            PCB_LABEL.Add(PCB3);
            PCB_LABEL.Add(PCB4);
            PCB_LABEL.Add(PCB5);
            PCB_LABEL.Add(PCB6);
            PCB_LABEL.Add(PCB7);
            PCB_LABEL.Add(PCB8);
        }

        private void Model_LoadFinish(object sender, EventArgs e)
        {
            model = (Model)MainModel.Clone();
            tbModelName.Text = model.Name;
            tbModelNamePath.Text = model.Path;
            TestStepsGrid.ItemsSource = null;
            TestStepsGrid.ItemsSource = model.Steps;
            Error_Positions_Table.ItemsSource = model.ErrorPositions;
            //await Task.Run(UpdateDataToGrid);
        }





        //PCB align tab
        private void nUD_PCBcount_ValueChanged(object sender, EventArgs e)
        {
            int Count = (int)((sender as System.Windows.Forms.NumericUpDown).Value);
            if ((sender as System.Windows.Forms.NumericUpDown).Name == "nUD_PCBcount")
            {
                model.contruction.PCB_Count = Count;
            }
            if ((sender as System.Windows.Forms.NumericUpDown).Name == "nUD_X_axis_count")
            {
                model.contruction.PCB_X_axis_Count = Count;
            }
            Align_PCB();
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
                    if (i < model.contruction.PCB_Count)
                    {
                        PCB_LABEL[i].Visibility = Visibility.Visible;
                        switch (model.contruction.ArrayPosition)
                        {
                            case Model.Contruction.ArrayPositions.HorizontalTopLeft:
                                colunm = i % model.contruction.PCB_X_axis_Count;
                                row = i / model.contruction.PCB_X_axis_Count;
                                break;
                            case Model.Contruction.ArrayPositions.HorizontalTopRight:
                                colunm = model.contruction.PCB_X_axis_Count - i % model.contruction.PCB_X_axis_Count - 1;
                                row = i / model.contruction.PCB_X_axis_Count;
                                break;
                            case Model.Contruction.ArrayPositions.HorizontalBottomLeft:
                                colunm = i % model.contruction.PCB_X_axis_Count;
                                row = (model.contruction.PCB_Count / model.contruction.PCB_X_axis_Count) - i / model.contruction.PCB_X_axis_Count;
                                break;
                            case Model.Contruction.ArrayPositions.HorizontalBottomRight:
                                colunm = model.contruction.PCB_X_axis_Count - i % model.contruction.PCB_X_axis_Count - 1;
                                row = (model.contruction.PCB_Count / model.contruction.PCB_X_axis_Count) - i / model.contruction.PCB_X_axis_Count;
                                break;

                            case Model.Contruction.ArrayPositions.VerticalTopLeft:
                                colunm = i / model.contruction.PCB_X_axis_Count;
                                row = i % model.contruction.PCB_X_axis_Count;
                                break;
                            case Model.Contruction.ArrayPositions.VerticalTopRight:
                                colunm = (model.contruction.PCB_Count / model.contruction.PCB_X_axis_Count) - i / model.contruction.PCB_X_axis_Count;
                                row = i % model.contruction.PCB_X_axis_Count;
                                break;
                            case Model.Contruction.ArrayPositions.VerticalBottomLeft:
                                colunm = i / model.contruction.PCB_X_axis_Count;
                                row = model.contruction.PCB_X_axis_Count - i % model.contruction.PCB_X_axis_Count - 1;
                                break;
                            case Model.Contruction.ArrayPositions.VerticalBottomRight:
                                colunm = (model.contruction.PCB_Count / model.contruction.PCB_X_axis_Count) - i / model.contruction.PCB_X_axis_Count;
                                row = model.contruction.PCB_X_axis_Count - i % model.contruction.PCB_X_axis_Count - 1;
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
            if ((sender as ComboBox).SelectedIndex > -1)
            {
                model.contruction.ArrayPosition = (Model.Contruction.ArrayPositions)(sender as ComboBox).SelectedIndex;
                Align_PCB();
            }
        }


        //Error position Tab

        bool IsAddingErrorPosition = false;
        Model.ErrorPosition newErrorPosition = new Model.ErrorPosition();

        private void btPCB_Error_Position_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (Error_Positions_Table.SelectedIndex >= 0 && Error_Positions_Table.SelectedIndex < model.ErrorPositions.Count)
            {
                int index = Error_Positions_Table.SelectedIndex;

                model.ErrorPositions.Remove((Model.ErrorPosition)Error_Positions_Table.SelectedItem);
                Canvas_PCB_Error_Mark.Children.Remove(((Model.ErrorPosition)Error_Positions_Table.SelectedItem).label);
                Canvas_PCB_Error_Mark.Children.Remove(((Model.ErrorPosition)Error_Positions_Table.SelectedItem).rect);

                for (int i = 0; i < model.ErrorPositions.Count; i++)
                {
                    model.ErrorPositions[i].No = i + 1;
                }
                Error_Positions_Table.Items.Refresh();
                Error_Positions_Table.SelectedIndex = index < model.ErrorPositions.Count ? index : index > 0 ? index - 1 : 0;
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
                model.ErrorPositions.Add(new Model.ErrorPosition()
                {
                    No = model.ErrorPositions.Count + 1,
                    Y = e.GetPosition(Canvas_PCB_Error_Mark).Y,
                    X = e.GetPosition(Canvas_PCB_Error_Mark).X,
                    lbTop = e.GetPosition(Canvas_PCB_Error_Mark).Y,
                    lbLeft = e.GetPosition(Canvas_PCB_Error_Mark).X,
                    Position = e.GetPosition(imgPCB_Error_Position).X.ToString("F2") + " ~ " + e.GetPosition(imgPCB_Error_Position).Y.ToString("F2"),
                    Width = 0,
                    Height = 0
                });

                Canvas.SetTop(model.ErrorPositions[model.ErrorPositions.Count - 1].label, model.ErrorPositions[model.ErrorPositions.Count - 1].Y);
                Canvas.SetLeft(model.ErrorPositions[model.ErrorPositions.Count - 1].label, model.ErrorPositions[model.ErrorPositions.Count - 1].X);

                Canvas.SetTop(model.ErrorPositions[model.ErrorPositions.Count - 1].rect, model.ErrorPositions[model.ErrorPositions.Count - 1].Y);
                Canvas.SetLeft(model.ErrorPositions[model.ErrorPositions.Count - 1].rect, model.ErrorPositions[model.ErrorPositions.Count - 1].X);

                Canvas_PCB_Error_Mark.Children.Add(model.ErrorPositions[model.ErrorPositions.Count - 1].rect);
                Canvas_PCB_Error_Mark.Children.Add(model.ErrorPositions[model.ErrorPositions.Count - 1].label);

            }
        }

        private void imgPCB_Error_Position_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsAddingErrorPosition)
            {
                //model.ErrorPositions.Add(newErrorPosition);
                model.ErrorPositions[model.ErrorPositions.Count - 1].X = model.ErrorPositions[model.ErrorPositions.Count - 1].lbLeft;
                model.ErrorPositions[model.ErrorPositions.Count - 1].Y = model.ErrorPositions[model.ErrorPositions.Count - 1].lbTop;

                Canvas.SetTop(model.ErrorPositions[model.ErrorPositions.Count - 1].label, model.ErrorPositions[model.ErrorPositions.Count - 1].Y);
                Canvas.SetLeft(model.ErrorPositions[model.ErrorPositions.Count - 1].label, model.ErrorPositions[model.ErrorPositions.Count - 1].X);

                //Canvas.SetTop(model.ErrorPositions[model.ErrorPositions.Count - 1].rect, model.ErrorPositions[model.ErrorPositions.Count - 1].Y);
                //Canvas.SetLeft(model.ErrorPositions[model.ErrorPositions.Count - 1].rect, model.ErrorPositions[model.ErrorPositions.Count - 1].X);

                Error_Positions_Table.ItemsSource = model.ErrorPositions;
                Error_Positions_Table.Items.Refresh();
                IsAddingErrorPosition = false;
                btPCB_Error_Position_Add.IsChecked = false;
            }
        }

        public void Error_Mark_Draw()
        {

        }

        private void Canvas_PCB_Error_Mark_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (IsAddingErrorPosition)
                {
                    Console.WriteLine("X : {0} ---- Y : {1}", e.GetPosition(Canvas_PCB_Error_Mark).X, e.GetPosition(Canvas_PCB_Error_Mark).Y);

                    if (e.GetPosition(Canvas_PCB_Error_Mark).X >= model.ErrorPositions[model.ErrorPositions.Count - 1].X)
                    {
                        model.ErrorPositions[model.ErrorPositions.Count - 1].Width = e.GetPosition(Canvas_PCB_Error_Mark).X - model.ErrorPositions[model.ErrorPositions.Count - 1].lbLeft;
                    }
                    else
                    {
                        //var X_temp = model.ErrorPositions[model.ErrorPositions.Count - 1].lbLeft;

                        model.ErrorPositions[model.ErrorPositions.Count - 1].lbLeft = e.GetPosition(Canvas_PCB_Error_Mark).X;
                        model.ErrorPositions[model.ErrorPositions.Count - 1].Width = model.ErrorPositions[model.ErrorPositions.Count - 1].X - e.GetPosition(Canvas_PCB_Error_Mark).X;
                        Canvas.SetLeft(model.ErrorPositions[model.ErrorPositions.Count - 1].rect, model.ErrorPositions[model.ErrorPositions.Count - 1].lbLeft);
                    }

                    if (e.GetPosition(Canvas_PCB_Error_Mark).Y >= model.ErrorPositions[model.ErrorPositions.Count - 1].Y)
                    {
                        model.ErrorPositions[model.ErrorPositions.Count - 1].Height = e.GetPosition(Canvas_PCB_Error_Mark).Y - model.ErrorPositions[model.ErrorPositions.Count - 1].lbTop;
                    }
                    else
                    {
                        //var X_temp = model.ErrorPositions[model.ErrorPositions.Count - 1].lbLeft;

                        model.ErrorPositions[model.ErrorPositions.Count - 1].lbTop = e.GetPosition(Canvas_PCB_Error_Mark).Y;
                        model.ErrorPositions[model.ErrorPositions.Count - 1].Height = model.ErrorPositions[model.ErrorPositions.Count - 1].Y - e.GetPosition(Canvas_PCB_Error_Mark).Y;
                        Canvas.SetTop(model.ErrorPositions[model.ErrorPositions.Count - 1].rect, model.ErrorPositions[model.ErrorPositions.Count - 1].lbTop);
                    }
                }
            }
        }


        private void btPCB_Error_Position_Browser_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Open PCB Image";
            if (openFile.ShowDialog() == true)
            {
                Uri fileUri = new Uri(openFile.FileName);
                tbPCB_Error_Position_Path.Text = openFile.FileName;
                imgPCB_Error_Position.Source = new BitmapImage(fileUri);
            }
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
    }
}
