using HVT.Controls;
using HVT.Utility;
using HVT.VTM.Base;
using HVT.VTM.Program;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VTM
{
    /// <summary>
    /// Interaction logic for ModelPage.xaml
    /// </summary>
    public partial class ModelPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Variable
        private Model editModel = new Model();
        public Model EditModel
        {
            get { return editModel; }
            set
            {
                if (value != editModel)
                {
                    editModel = value;
                    this.DataContext = EditModel;
                    if (this._contentLoaded)
                    {
                        StepsGridData.ItemsSource = EditModel.Steps;
                        lbModelName.Content = EditModel.Name;
                        LayoutWiewer.PBA = EditModel.Layout;
                        BarcodeOptionsWiewer.BarcodeOption = EditModel.BarcodeOption;
                        DischargeOptionsWiewer.DisChargeOptionProperties = EditModel.Discharge;
                        MuxCardViewer.Card = EditModel.MuxCard;
                        LevelCardViewer.Card = EditModel.LevelCard;
                        editModel.MuxCard.PCB_Count = EditModel.Layout.PCB_Count;
                        dgRX_data_naming.ItemsSource = EditModel.Naming.RxDatas;
                        dgTX_data_naming.ItemsSource = EditModel.Naming.TxDatas;
                        dgQRcodeNameCode.ItemsSource = EditModel.Naming.QRDatas;
                        P1_Config_Holder.UUT_config = EditModel.P1_Config;
                        P2_Config_Holder.UUT_config = EditModel.P2_Config;

                        CommandDescriptions.RXnaming = EditModel.Naming.RxDatas.Select(o => o.Name).ToList();
                        CommandDescriptions.TXnaming = EditModel.Naming.TxDatas.Select(o => o.Name).ToList();
                        CommandDescriptions.QRnaming = EditModel.Naming.QRDatas.Select(o => o.Context).ToList();
                        Command.UpdateCommand();
                    }
                    NotifyPropertyChanged("EditModel");
                }
            }
        }
        private Program program = new Program();
        public Program Program
        {
            get { return program; }
            set
            {
                program = value;
                //Program.StepTestChange += Program_StepTestChange;
                //Program.StateChange += Program_StateChange;
                //Program.TestRunFinish += Program_TestRunFinish;
            }
        }

        // Step tab variable 
        // Last sellected cell of step data grid.
        DataGridCellInfo lastSellectCellInfor = new DataGridCellInfo();

        public ModelPage()
        {
            InitializeComponent();

        }

        private void ModelPage_Loaded(object sender, RoutedEventArgs e)
        {
            StepsGridData.ItemsSource = EditModel.Steps;
            lbModelName.Content = EditModel.Name;
            LayoutWiewer.PBA = EditModel.Layout;
            BarcodeOptionsWiewer.BarcodeOption = EditModel.BarcodeOption;
            DischargeOptionsWiewer.DisChargeOptionProperties = EditModel.Discharge;
            MuxCardViewer.Card = EditModel.MuxCard;
            editModel.MuxCard.PCB_Count = editModel.Layout.PCB_Count;
            P1_Config_Holder.UUT_config = EditModel.P1_Config;
            P2_Config_Holder.UUT_config = EditModel.P2_Config;
            LevelCardViewer.Card = EditModel.LevelCard;
            this.DataContext = EditModel;
        }

        private void btOpenModel_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                DefaultExt = ".model",
                Title = "Open model",
            };
            openFile.Filter = "VTM model files (*.vmdl)|*.vmdl";
            openFile.RestoreDirectory = true;
            if (openFile.ShowDialog() == true)
            {
                HVT.Utility.Debug.Write("Load model:" + openFile.FileName, HVT.Utility.Debug.ContentType.Notify);
                //var fileInfor = new FileInfo(openFile.FileName);
                string modelStr = System.IO.File.ReadAllText(openFile.FileName);
                try
                {
                    string modelString = HVT.Utility.Extensions.Decoder(modelStr, System.Text.Encoding.UTF7);
                    //TestModel = HVT.Utility.Extensions.ConvertFromJson<Model>(modelString);
                    Program.EditModel = HVT.Utility.Extensions.ConvertFromJson<Model>(modelString);
                    EditModel = Program.EditModel;

                    Program.EditModel.Path = openFile.FileName;
                    foreach (var item in EditModel.Steps)
                    {
                        item.ValueGet1 = "";
                        item.ValueGet2 = "";
                        item.ValueGet3 = "";
                        item.ValueGet4 = "";
                    }
                    Program.OnEditModelLoaded();
                    LevelCardViewer.Card.PCB_Count = 1;
                    LevelCardViewer.Card.PCB_Count = EditModel.Layout.PCB_Count;
                }
                catch (Exception)
                {
                    HVT.Utility.Debug.Write("Load model fail, file not correct format. \n" +
                        "Model folder: " + openFile.FileName, HVT.Utility.Debug.ContentType.Error);
                }
            }
        }

        private async void btSaveModel_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(EditModel.Path))
            {
                saveLabel.Visibility = Visibility.Visible;
                await Task.Delay(100);
                EditModel.ModelSegmentLookup = Camera.FND.SEG_LOOKUP.Clone();
                HVT.Utility.Extensions.SaveToFile(EditModel, EditModel.Path);
                Program.EditModel = EditModel;
                await Task.Delay(100);
                saveLabel.Visibility = Visibility.Hidden;
            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = HVT.VTM.Program.FolderMap.RootFolder;
                saveFileDialog.AddExtension = true;
                saveFileDialog.DefaultExt = FolderMap.DefaultModelFileExt;
                if ((bool)saveFileDialog.ShowDialog())
                {
                    saveLabel.Visibility = Visibility.Visible;
                    EditModel.Name = saveFileDialog.SafeFileName;
                    EditModel.Path = saveFileDialog.FileName;
                    HVT.Utility.Extensions.SaveToFile(EditModel, saveFileDialog.FileName);
                    await Task.Delay(100);
                    saveLabel.Visibility = Visibility.Hidden;
                }
            }

            Program.OnEditModelSave();
        }

        private async void btSaveAsModel_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = HVT.VTM.Program.FolderMap.RootFolder;
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = FolderMap.DefaultModelFileExt;
            if ((bool)saveFileDialog.ShowDialog())
            {
                saveLabel.Visibility = Visibility.Visible;
                await Task.Delay(100);
                EditModel.ModelSegmentLookup = Camera.FND.SEG_LOOKUP.Clone();
                EditModel.Name = saveFileDialog.SafeFileName;
                EditModel.Path = saveFileDialog.FileName;
                HVT.Utility.Extensions.SaveToFile(EditModel, saveFileDialog.FileName);
                await Task.Delay(100);
                saveLabel.Visibility = Visibility.Hidden;
            }
            Program.OnEditModelSave();
        }

        #region Model steps 
        private void TestStepsGridData_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            HVT.VTM.Base.Command command = new Command();
            var cell = (sender as DataGrid).CurrentCell;
            if (cell != null)
            {
                if (cell != lastSellectCellInfor)
                {
                    lastSellectCellInfor = cell;
                    (sender as DataGrid).BeginEdit();
                }
            }
        }

        private void StepsGridData_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            lastSellectCellInfor = new DataGridCellInfo();
        }

        private void StepsGridData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var grid = sender as DataGrid;
            HVT.VTM.Base.Command command = new Command();
            var cell = (sender as DataGrid).CurrentCell;
            if (cell != null)
            {
                Step item = cell.Item as Step;
                if (item != null)
                {
                    command.cmd = item.cmd;
                    TestStepsGridRemark.ItemsSource = null;
                    TestStepsGridRemark.ItemsSource = command.CMD;
                    lbStepHelp.Content = command.CMD[0].Description;

                    if (StepsGridData.CurrentColumn != null)
                    {
                        TestStepsGridRemark.CurrentCell = new DataGridCellInfo(TestStepsGridRemark.Items[0], StepsGridData.CurrentColumn);
                        TestStepsGridRemark.SelectedCells.Clear();
                        TestStepsGridRemark.SelectedCells.Add(TestStepsGridRemark.CurrentCell);
                    }
                }
            }
        }

        Step bufferStep = new Step();
        private void StepsGridData_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var cell = (sender as DataGrid).CurrentCell;
            if (cell == null)
            {
                e.Handled = false;
                return;
            }

            var rowItem = (Step)cell.Item;
            var columnItem = cell.Column;
            int rowIndex = EditModel.Steps.IndexOf(rowItem);

            if (rowIndex == -1)
            {
                e.Handled = false;
                return;
            }

            if (e.Key == Key.F1)
            {
                tbHelp.Visibility = tbHelp.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
            }

            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                StepsGridData.SelectedCells.Clear();
                switch (e.Key)
                {
                    case Key.Down:
                        if (rowIndex < EditModel.Steps.Count - 1)
                        {
                            bufferStep = rowItem.Clone();
                            EditModel.Steps[rowIndex] = EditModel.Steps[rowIndex + 1].Clone();
                            EditModel.Steps[rowIndex + 1] = bufferStep.Clone();
                            for (int i = 0; i < EditModel.Steps.Count; i++)
                            {
                                EditModel.Steps[i].No = i + 1;
                            }
                            e.Handled = true;
                            StepsGridData.CurrentCell = new DataGridCellInfo(StepsGridData.Items[rowIndex + 1], columnItem);
                            StepsGridData.SelectedCells.Add(StepsGridData.CurrentCell);
                        }
                        break;
                    case Key.Up:
                        if (rowIndex > 0)
                        {
                            bufferStep = rowItem.Clone();
                            EditModel.Steps[rowIndex] = EditModel.Steps[rowIndex - 1].Clone();
                            EditModel.Steps[rowIndex - 1] = bufferStep.Clone();
                            for (int i = 0; i < EditModel.Steps.Count; i++)
                            {
                                EditModel.Steps[i].No = i + 1;
                            }
                            e.Handled = true;
                            StepsGridData.CurrentCell = new DataGridCellInfo(StepsGridData.Items[rowIndex - 1], columnItem);
                            StepsGridData.SelectedCells.Add(StepsGridData.CurrentCell);
                        }
                        break;
                }
                return;
            }


            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.Insert:
                        var newRowItem = HVT.Utility.Extensions.Clone(rowItem);
                        newRowItem.No = rowIndex + 1;
                        EditModel.Steps.Insert(rowIndex + 1, newRowItem);
                        for (int i = 0; i < EditModel.Steps.Count; i++)
                        {
                            EditModel.Steps[i].No = i + 1;
                        }
                        if (rowIndex < EditModel.Steps.Count)
                        {
                            StepsGridData.CurrentCell = new DataGridCellInfo(StepsGridData.Items[rowIndex + 1], columnItem);
                            StepsGridData.SelectedCells.Clear();
                            StepsGridData.SelectedCells.Add(StepsGridData.CurrentCell);
                        }

                        break;
                    case Key.Delete:
                        if (EditModel.Steps.Count < 2)
                        {
                            return;
                        }
                        EditModel.Steps.RemoveAt(rowIndex);
                        for (int i = 0; i < EditModel.Steps.Count; i++)
                        {
                            EditModel.Steps[i].No = i + 1;
                        }
                        if (rowIndex > EditModel.Steps.Count - 1)
                        {
                            StepsGridData.CurrentCell = new DataGridCellInfo(StepsGridData.Items[rowIndex - 1], columnItem);
                            StepsGridData.SelectedCells.Clear();
                            StepsGridData.SelectedCells.Add(StepsGridData.CurrentCell);
                        }
                        else
                        {
                            StepsGridData.CurrentCell = new DataGridCellInfo(StepsGridData.Items[rowIndex], columnItem);
                            StepsGridData.SelectedCells.Clear();
                            StepsGridData.SelectedCells.Add(StepsGridData.CurrentCell);
                        }
                        break;
                    case Key.Help:
                        break;
                    case Key.I:
                        EditModel.Load();
                        StepsGridData.Items.Refresh();
                        break;
                    case Key.S:
                        btSaveModel_Click(btSaveModel, null);
                        e.Handled = true;
                        break;
                    case Key.O:
                        btOpenModel_Click(btOpenModel, null);
                        e.Handled = true;
                        break;
                    case Key.Add:
                        EditModel.Steps.Insert(rowIndex, new Step(rowIndex + 1));
                        for (int i = 0; i < EditModel.Steps.Count; i++)
                        {
                            EditModel.Steps[i].No = i + 1;
                        }
                        break;
                    case Key.N:
                        EditModel.Steps.Insert(rowIndex, new Step(rowIndex + 1));
                        for (int i = 0; i < EditModel.Steps.Count; i++)
                        {
                            EditModel.Steps[i].No = i + 1;
                        }
                        e.Handled = true;
                        break;
                    case Key.D:
                        bufferStep = rowItem.Clone();
                        EditModel.Steps.Insert(rowIndex + 1, bufferStep.Clone());
                        for (int i = 0; i < EditModel.Steps.Count; i++)
                        {
                            EditModel.Steps[i].No = i + 1;
                        }
                        e.Handled = true;
                        StepsGridData.CurrentCell = new DataGridCellInfo(StepsGridData.Items[rowIndex + 1], columnItem);
                        StepsGridData.SelectedCells.Clear();
                        StepsGridData.SelectedCells.Add(StepsGridData.CurrentCell);
                        break;
                    case Key.C:
                        bufferStep = rowItem.Clone();
                        e.Handled = true;
                        break;
                    case Key.V:
                        if (bufferStep != null)
                        {
                            EditModel.Steps.Insert(rowIndex, bufferStep.Clone());
                            for (int i = 0; i < EditModel.Steps.Count; i++)
                            {
                                EditModel.Steps[i].No = i + 1;
                            }
                        }
                        e.Handled = true;
                        break;

                    case Key.Subtract:
                        break;
                    case Key.LeftShift:
                        break;
                    case Key.RightShift:
                        break;
                    case Key.LeftCtrl:
                        break;
                    case Key.RightCtrl:
                        break;
                    case Key.LeftAlt:
                        break;
                    case Key.RightAlt:
                        break;
                    default:
                        break;
                }
                //e.Handled = true;
                bufferStep = null;
                return;
            }

            var grid = sender as DataGrid;
            if (grid.CurrentCell != null)
            {
                var cellContent = grid.CurrentCell.Column?.GetCellContent(grid.CurrentCell.Item);
                if (cellContent != null)
                {
                    var cellParent = (DataGridCell)cellContent.Parent;
                    if (!cellParent.IsEditing)
                    {
                        if (Char.TryParse(e.Key.ToString(), out _) || e.Key.ToString().Contains("NumPad") || e.Key.ToString().StartsWith("D") || e.Key == Key.Enter)
                        {
                            grid.BeginEdit();
                            cellParent.IsEditing = true;
                            Keyboard.Focus(cellContent);
                            e.Handled = e.Key == Key.Enter;
                        }
                    }
                }
            }
        }


        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.I:
                        EditModel.Load();
                        e.Handled = true;
                        break;
                    case Key.S:
                        btSaveModel_Click(btSaveModel, null);
                        e.Handled = true;
                        break;
                    case Key.O:
                        btOpenModel_Click(btOpenModel, null);
                        e.Handled = true;
                        break;
                }
            }
        }

        private void btOpenTxNaming_Click(object sender, RoutedEventArgs e)
        {
            EditModel.Naming.OpenTxNamingFile();
            dgTX_data_naming.ItemsSource = EditModel.Naming.TxDatas;
            CommandDescriptions.TXnaming = EditModel.Naming.TxDatas.Select(o => o.Name).ToList();
            Command.UpdateCommand();
            foreach (var item in EditModel.Steps)
            {
                item.CommandDescriptions = Command.Commands.SingleOrDefault(x => x.CMD == item.cmd);
                NotifyPropertyChanged("CommandDescriptions");
            }
        }

        private void btSaveTxNaming_Click(object sender, RoutedEventArgs e)
        {
            EditModel.Naming.SaveTxNamingFile(dgTX_data_naming);
        }

        private void btOpenRxNaming_Click(object sender, RoutedEventArgs e)
        {
            EditModel.Naming.OpenRxNamingFile();
            dgRX_data_naming.ItemsSource = EditModel.Naming.RxDatas;
            CommandDescriptions.RXnaming = EditModel.Naming.RxDatas.Select(o => o.Name).ToList();
            Command.UpdateCommand();
            foreach (var item in EditModel.Steps)
            {
                item.CommandDescriptions = Command.Commands.SingleOrDefault(x => x.CMD == item.cmd);
                NotifyPropertyChanged("CommandDescriptions");
            }
        }

        private void btSaveRxNaming_Click(object sender, RoutedEventArgs e)
        {
            EditModel.Naming.SaveRxNamingFile(dgRX_data_naming);
        }

        private void btOpenQrNaming_Click(object sender, RoutedEventArgs e)
        {
            EditModel.Naming.OpenQRNamingFile();
            dgQRcodeNameCode.ItemsSource = EditModel.Naming.QRDatas;
            CommandDescriptions.QRnaming = EditModel.Naming.QRDatas.Select(o => o.Context).ToList();
            Command.UpdateCommand();
            foreach (var item in EditModel.Steps)
            {
                item.CommandDescriptions = Command.Commands.SingleOrDefault(x => x.CMD == item.cmd);
                NotifyPropertyChanged("CommandDescriptions");
            }
        }

        private void btSaveQrNaming_Click(object sender, RoutedEventArgs e)
        {
            EditModel.Naming.SaveQRNamingFile(dgQRcodeNameCode);
        }

        private System.Windows.Controls.DataGridCell GetDataGridCell(System.Windows.Controls.DataGridCellInfo cellInfo)
        {
            if (cellInfo.Column == null) return null;
            var cellContent = cellInfo.Column.GetCellContent(cellInfo.Item);

            if (cellContent != null)
                return ((System.Windows.Controls.DataGridCell)cellContent.Parent);

            return (null);
        }

        private void StepsGridData_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cell = GetDataGridCell(StepsGridData.CurrentCell);
            StepsGridData.BeginEdit();
            Keyboard.Focus(cell);
        }

        private void StepsGridData_CurrentCellChanged(object sender, EventArgs e)
        {
            if (StepsGridData.CurrentCell == null) return;
            var cell = GetDataGridCell(StepsGridData.CurrentCell);
            Keyboard.Focus(cell);
        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            StepsGridData.BeginEdit();
        }

    }
    #endregion

}

