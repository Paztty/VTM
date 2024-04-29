using HVT.Controls;
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

namespace HVT.VTM.Program
{
    /// <summary>
    /// Interaction logic for BoardResultPanel.xaml
    /// </summary>
    public partial class BoardResultPanel : UserControl
    {
        public List<Grid> PCB_label = new List<Grid>();
        private PBA_Layout _PBA = new PBA_Layout();
        public PBA_Layout PBA
        {
            get { return _PBA; }
            set
            {
                if (value != null || value != _PBA)
                {

                    _PBA = value;
                    if (_contentLoaded)
                    {
                        Align_PCB();
                    }
                }
            }
        }
        public BoardResultPanel()
        {
            InitializeComponent();
            PCB_label.Add(PCB1);
            PCB_label.Add(PCB2);
            PCB_label.Add(PCB3);
            PCB_label.Add(PCB4);
            Align_PCB();
        }

        /// <summary>
        /// Alight PCB in model page
        /// </summary>
        public void Align_PCB()
        {
            if (PCBlayout != null && PCB_label.Count >= PBA.PCB_Count)
            {
                for (int i = 0; i < 4; i++)
                {
                    PCBlayout.ColumnDefinitions[i].Width = new GridLength(0, GridUnitType.Star);
                    PCBlayout.RowDefinitions[i].Height = new GridLength(0, GridUnitType.Star);
                }

                int colunm = 0;
                int row = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (i < PBA.PCB_Count)
                    {
                        PCB_label[i].Visibility = Visibility.Visible;
                        switch ((PBA_Layout.ArrayPositions)PBA.Position)
                        {
                            case PBA_Layout.ArrayPositions.HorizontalTopLeft:
                                colunm = i % PBA.PCB_X_axis_Count;
                                row = i / PBA.PCB_X_axis_Count;
                                break;
                            case PBA_Layout.ArrayPositions.HorizontalTopRight:
                                colunm = PBA.PCB_X_axis_Count - i % PBA.PCB_X_axis_Count - 1;
                                row = i / PBA.PCB_X_axis_Count;
                                break;
                            case PBA_Layout.ArrayPositions.HorizontalBottomLeft:
                                colunm = i % PBA.PCB_X_axis_Count;
                                row = (PBA.PCB_Count / PBA.PCB_X_axis_Count) - i / PBA.PCB_X_axis_Count;
                                break;
                            case PBA_Layout.ArrayPositions.HorizontalBottomRight:
                                colunm = PBA.PCB_X_axis_Count - i % PBA.PCB_X_axis_Count - 1;
                                row = (PBA.PCB_Count / PBA.PCB_X_axis_Count) - i / PBA.PCB_X_axis_Count;
                                break;

                            case PBA_Layout.ArrayPositions.VerticalTopLeft:
                                colunm = i / PBA.PCB_X_axis_Count;
                                row = i % PBA.PCB_X_axis_Count;
                                break;
                            case PBA_Layout.ArrayPositions.VerticalTopRight:
                                colunm = (PBA.PCB_Count / PBA.PCB_X_axis_Count) - i / PBA.PCB_X_axis_Count;
                                row = i % PBA.PCB_X_axis_Count;
                                break;
                            case PBA_Layout.ArrayPositions.VerticalBottomLeft:
                                colunm = i / PBA.PCB_X_axis_Count;
                                row = PBA.PCB_X_axis_Count - i % PBA.PCB_X_axis_Count - 1;
                                break;
                            case PBA_Layout.ArrayPositions.VerticalBottomRight:
                                colunm = (PBA.PCB_Count / PBA.PCB_X_axis_Count) - i / PBA.PCB_X_axis_Count;
                                row = PBA.PCB_X_axis_Count - i % PBA.PCB_X_axis_Count - 1;
                                break;
                            default:
                                break;
                        }

                        PCBlayout.ColumnDefinitions[colunm].Width = new GridLength(1, GridUnitType.Star);
                        PCBlayout.RowDefinitions[row].Height = new GridLength(1, GridUnitType.Star);

                        Grid.SetColumn(PCB_label[i], colunm);
                        Grid.SetRow(PCB_label[i], row);
                    }
                    else
                    {
                        PCB_label[i].Visibility = Visibility.Hidden;
                    }
                }
            }
        }


        public void ShowResult(List<Board> Boards)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (Boards.Count >= 1) { BarcodeA.Content = Boards[0].UserSkip ? "SKIP" : Boards[0].Barcode; ResultA.Content = Boards[0].UserSkip ? "SKIP" : Boards[0].Result;
                    ResultDataContextChanged(ResultA);
                }
                if (Boards.Count >= 2) { BarcodeB.Content = Boards[1].UserSkip ? "SKIP" : Boards[1].Barcode; ResultB.Content = Boards[1].UserSkip ? "SKIP" : Boards[1].Result;
                    ResultDataContextChanged(ResultB);
                }
                if (Boards.Count >= 3) { BarcodeC.Content = Boards[2].UserSkip ? "SKIP" : Boards[2].Barcode; ResultC.Content = Boards[2].UserSkip ? "SKIP" : Boards[2].Result;
                    ResultDataContextChanged(ResultC);
                }
                if (Boards.Count >= 4) { BarcodeD.Content = Boards[3].UserSkip ? "SKIP" : Boards[3].Barcode; ResultD.Content = Boards[3].UserSkip ? "SKIP" : Boards[3].Result;
                    ResultDataContextChanged(ResultD);
                }

                Align_PCB();
                this.Visibility = Visibility.Visible;
            }));
        }

        private void ResultDataContextChanged(object sender)
        {
            if (((sender as Label).Content as string) == "SKIP")
            {
                (sender as Label).Background = new SolidColorBrush(Colors.DarkGray);
            }
            if (((sender as Label).Content as string) == "FAIL")
            {
                (sender as Label).Background = new SolidColorBrush(Colors.DarkRed);
            }
            if (((sender as Label).Content as string) == "OK")
            {
                (sender as Label).Background = new SolidColorBrush(Colors.Green);
            }
        }
    }
}
