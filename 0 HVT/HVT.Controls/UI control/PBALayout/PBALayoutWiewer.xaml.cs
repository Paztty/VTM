using HVT.Controls.CustomControls;
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

namespace HVT.Controls
{
    /// <summary>
    /// Interaction logic for PBALayoutWiewer.xaml
    /// </summary>
    public partial class PBALayoutWiewer : UserControl
    {
        private PBA_Layout _PBA = new PBA_Layout();
        public PBA_Layout PBA
        {
            get { return _PBA; }
            set
            {
                if (value != null || value != _PBA )
                {
                    
                    _PBA = value;
                    this.DataContext = _PBA;
                }
            }
        }

        public List<Label> PCB_label = new List<Label>();
        public PBALayoutWiewer()
        {
            InitializeComponent();
            this.DataContext = _PBA;
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PBA.Position = (sender as ComboBox).SelectedIndex;
            Align_PCB();
        }

        private void nUD_PCBcount_ValueChanged(object sender, EventArgs e)
        {
            if ((sender as IntegerUpDown).Value != null)
            {
                int Count = (int)((sender as IntegerUpDown).Value);
                if ((sender as IntegerUpDown).Name == "nUD_PCBcount")
                {
                    PBA.PCB_Count = Count;
                    if (nUD_X_axis_count != null)
                    nUD_X_axis_count.Maximum = Count;
                }
                if ((sender as IntegerUpDown).Name == "nUD_X_axis_count")
                {
                    PBA.PCB_X_axis_Count = Count;
                }
                Align_PCB();
            }
        }
    }
}
