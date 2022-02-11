using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HVT.VTM.Base

{
    public class Contruction : INotifyPropertyChanged
    {
        public event EventHandler<EventArgs> ContructionChanged;
        private void ContructionChange()
        {
            ContructionChanged?.Invoke(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        //Contruction layout
        public Grid PCBlayout;
        public List<Label> PCB_label = new List<Label>();

        public Grid PCB_result_layout;

        public class PCBResultPanel
        {
            public Label Barcode_label = new Label();
            public Label Result_label = new Label();
            public Label PCB_label = new Label();
        }
        public List<PCBResultPanel> resultPanels = new List<PCBResultPanel>();

        public Grid PCB_resultGrid;
        public List<Grid> _resultGrid = new List<Grid>();

        private int pcb_Count = 4;
        public int PCB_Count
        {
            get { return pcb_Count; }
            set
            {
                if (pcb_Count != value)
                {
                    pcb_Count = value;
                    for (int i = 0; i < PBAs.Count; i++)
                    {
                        PBAs[i].Visibility = pcb_Count > i ? Visibility.Visible : Visibility.Collapsed;
                        PBAs[i].IsWaiting = i < pcb_Count;
                        PBAs[i].IsUse = i < pcb_Count;
                    }
                    ContructionChange();
                }
            }
        }
        public int PCB_X_axis_Count { get; set; } = 2;
        public enum ArrayPositions
        {
            HorizontalTopLeft = 0,
            HorizontalTopRight = 1,
            HorizontalBottomLeft = 2,
            HorizontalBottomRight = 3,
            VerticalTopLeft = 4,
            VerticalTopRight = 5,
            VerticalBottomLeft = 6,
            VerticalBottomRight = 7,
        };
        public ArrayPositions ArrayPosition { get; set; } = ArrayPositions.HorizontalBottomRight;

        public ObservableCollection<PBA> PBAs = new ObservableCollection<PBA>()
        {
            new PBA(){ SiteName = "A"},
            new PBA(){ SiteName = "B"},
            new PBA(){ SiteName = "C"},
            new PBA(){ SiteName = "D"},
        };


        /// <summary>
        /// Check PCB layout element are enought for display or not
        /// </summary>
        /// <returns></returns>
        private bool CheckElement()
        {
            bool Enounght = PCBlayout != null;
            Enounght = PCB_result_layout != null;
            foreach (var item in PCB_label)
            {
                Enounght = item != null;
            }
            foreach (var item in resultPanels)
            {
                Enounght = item.PCB_label != null;
                Enounght = item.Barcode_label != null;
                Enounght = item.Result_label != null;
            }
            return Enounght;
        }

        /// <summary>
        /// Alight PCB in model page
        /// </summary>
        public void Align_PCB()
        {
            if (PCBlayout != null && PCB_label.Count >= PCB_Count)
            {

                for (int i = 0; i < 8; i++)
                {
                    PCBlayout.ColumnDefinitions[i].Width = new GridLength(0, GridUnitType.Star);
                    PCBlayout.RowDefinitions[i].Height = new GridLength(0, GridUnitType.Star);
                }

                int colunm = 0;
                int row = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (i < PCB_Count)
                    {
                        PCB_label[i].Visibility = Visibility.Visible;
                        switch (ArrayPosition)
                        {
                            case ArrayPositions.HorizontalTopLeft:
                                colunm = i % PCB_X_axis_Count;
                                row = i / PCB_X_axis_Count;
                                break;
                            case ArrayPositions.HorizontalTopRight:
                                colunm = PCB_X_axis_Count - i % PCB_X_axis_Count - 1;
                                row = i / PCB_X_axis_Count;
                                break;
                            case ArrayPositions.HorizontalBottomLeft:
                                colunm = i % PCB_X_axis_Count;
                                row = (PCB_Count / PCB_X_axis_Count) - i / PCB_X_axis_Count;
                                break;
                            case ArrayPositions.HorizontalBottomRight:
                                colunm = PCB_X_axis_Count - i % PCB_X_axis_Count - 1;
                                row = (PCB_Count / PCB_X_axis_Count) - i / PCB_X_axis_Count;
                                break;

                            case ArrayPositions.VerticalTopLeft:
                                colunm = i / PCB_X_axis_Count;
                                row = i % PCB_X_axis_Count;
                                break;
                            case ArrayPositions.VerticalTopRight:
                                colunm = (PCB_Count / PCB_X_axis_Count) - i / PCB_X_axis_Count;
                                row = i % PCB_X_axis_Count;
                                break;
                            case ArrayPositions.VerticalBottomLeft:
                                colunm = i / PCB_X_axis_Count;
                                row = PCB_X_axis_Count - i % PCB_X_axis_Count - 1;
                                break;
                            case ArrayPositions.VerticalBottomRight:
                                colunm = (PCB_Count / PCB_X_axis_Count) - i / PCB_X_axis_Count;
                                row = PCB_X_axis_Count - i % PCB_X_axis_Count - 1;
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
            AlignResult();
        }

        /// <summary>
        /// Show resutl panel with pcb position and barcode 
        /// </summary>
        public void AlignResult()
        {
            if (PCB_resultGrid != null && _resultGrid.Count >= PCB_Count)
            {

                for (int i = 0; i < 8; i++)
                {
                    PCB_resultGrid.ColumnDefinitions[i].Width = new GridLength(0, GridUnitType.Star);
                    PCB_resultGrid.RowDefinitions[i].Height = new GridLength(0, GridUnitType.Star);
                }

                int colunm = 0;
                int row = 0;

                for (int i = 0; i < 4; i++)
                {
                    if (i < PCB_Count)
                    {
                        _resultGrid[i].Visibility = Visibility.Visible;
                        switch (ArrayPosition)
                        {
                            case ArrayPositions.HorizontalTopLeft:
                                colunm = i % PCB_X_axis_Count;
                                row = i / PCB_X_axis_Count;
                                break;
                            case ArrayPositions.HorizontalTopRight:
                                colunm = PCB_X_axis_Count - i % PCB_X_axis_Count - 1;
                                row = i / PCB_X_axis_Count;
                                break;
                            case ArrayPositions.HorizontalBottomLeft:
                                colunm = i % PCB_X_axis_Count;
                                row = (PCB_Count / PCB_X_axis_Count) - i / PCB_X_axis_Count;
                                break;
                            case ArrayPositions.HorizontalBottomRight:
                                colunm = PCB_X_axis_Count - i % PCB_X_axis_Count - 1;
                                row = (PCB_Count / PCB_X_axis_Count) - i / PCB_X_axis_Count;
                                break;

                            case ArrayPositions.VerticalTopLeft:
                                colunm = i / PCB_X_axis_Count;
                                row = i % PCB_X_axis_Count;
                                break;
                            case ArrayPositions.VerticalTopRight:
                                colunm = (PCB_Count / PCB_X_axis_Count) - i / PCB_X_axis_Count;
                                row = i % PCB_X_axis_Count;
                                break;
                            case ArrayPositions.VerticalBottomLeft:
                                colunm = i / PCB_X_axis_Count;
                                row = PCB_X_axis_Count - i % PCB_X_axis_Count - 1;
                                break;
                            case ArrayPositions.VerticalBottomRight:
                                colunm = (PCB_Count / PCB_X_axis_Count) - i / PCB_X_axis_Count;
                                row = PCB_X_axis_Count - i % PCB_X_axis_Count - 1;
                                break;
                            default:
                                break;
                        }

                        PCB_resultGrid.ColumnDefinitions[colunm].Width = new GridLength(1, GridUnitType.Star);
                        PCB_resultGrid.RowDefinitions[row].Height = new GridLength(1, GridUnitType.Star);

                        Grid.SetColumn(_resultGrid[i], colunm);
                        Grid.SetRow(_resultGrid[i], row);
                    }
                    else
                    {
                        _resultGrid[i].Visibility = Visibility.Hidden;
                    }
                }
            }
        }
    }
}
