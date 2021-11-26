using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace HVT.VTM.Base
{
    public class Naming
    {
        public ObservableCollection<TxData> TxDatas { get; set; } = new ObservableCollection<TxData>();

        public ObservableCollection<RxData> RxDatas { get; set; } = new ObservableCollection<RxData>();

        public ObservableCollection<QRData> QRDatas { get; set; } = new ObservableCollection<QRData>();

        public string OpenQRNamingFile()
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                DefaultExt = ".ILN",
                Title = "Open QR naming file",
            };
            openFile.Filter = "VTM QR naming files (*.ILN)|*.ILN";
            openFile.RestoreDirectory = true;
            if ((bool)openFile.ShowDialog())
            {
                var QrDataList = File.ReadAllLines(openFile.FileName);
                QRDatas.Clear();
                for (int index = 1; index < QrDataList.Length; index++)
                {
                    var item = QrDataList[index].Replace("\"\"", "");
                    var dataItem = item.Split(',');
                    Console.WriteLine(item.ToString() + index);
                    QRDatas.Add(new QRData()
                    {
                        No = Convert.ToInt32(dataItem[0]),
                        Context = dataItem[1],
                        Code = dataItem[2],
                    });
                }
                return openFile.FileName;
            }
            else
            {
                return null;
            }
        }
        public List<TxData> OpenTxNamingFile()
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                DefaultExt = ".CTN",
                Title = "Open Tx naming file",
            };
            openFile.Filter = "VTM Tx naming files (*.CTN)|*.CTN";
            openFile.RestoreDirectory = true;
            if ((bool)openFile.ShowDialog())
            {
                var TxDataList = File.ReadAllLines(openFile.FileName);
                TxDatas.Clear();
                for (int index = 1; index < TxDataList.Length; index++)
                {
                    var item = TxDataList[index].Replace("\"\"", "");
                    var dataItem = item.Split(',');
                    Console.WriteLine(item.ToString() + index);
                    TxDatas.Add(new TxData()
                    {
                        No = Convert.ToInt32(dataItem[0]),
                        Name = dataItem[1],
                        Data = dataItem[2],
                        Blank = dataItem[3],
                        Remark = dataItem[4]
                    });
                }
                return TxDatas.ToList();
            }
            else
            {
                return null;
            }
        }
        public List<RxData> OpenRxNamingFile()
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                DefaultExt = ".CRN",
                Title = "Open Rx naming file",
            };
            openFile.Filter = "VTM Rx files (*.CRN)|*.CRN";
            openFile.RestoreDirectory = true;
            if ((bool)openFile.ShowDialog())
            {
                var RxDataList = File.ReadAllLines(openFile.FileName);
                RxDatas.Clear();
                for (int index = 1; index < RxDataList.Length; index++)
                {
                    var item = RxDataList[index].Replace("\"\"", "");
                    var dataItem = item.Split(',');
                    RxDatas.Add(new RxData()
                    {
                        No = Convert.ToInt32(dataItem[0]),
                        Name = dataItem[1],
                        ModeLoc = dataItem[2],
                        Mode = dataItem[3],
                        DataKind = dataItem[4],
                        MByte = dataItem[5],
                        M_Mbit = dataItem[6],
                        M_Lbit = dataItem[7],
                        LByte = dataItem[8],
                        L_Mbit = dataItem[9],
                        L_Lbit = dataItem[10],
                        Type = dataItem[11],
                        Remark = dataItem[12]
                    });
                }
                return RxDatas.ToList();
            }
            else
            {
                return null;
            }
        }

        public void SaveTxNamingFile(DataGrid Grid)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "VTM TxNaming File (*.CTN)|*.CTN";
            saveDlg.FilterIndex = 0;
            saveDlg.RestoreDirectory = true;
            saveDlg.Title = "Save TxNaming File";
            if ((bool)saveDlg.ShowDialog())
            {
                string CsvFpath = saveDlg.FileName;
                System.IO.StreamWriter csvFileWriter = new StreamWriter(CsvFpath, false);
                string columnHeaderText = "";

                int countColumn = Grid.Columns.Count - 1;
                if (countColumn >= 0)
                {
                    columnHeaderText = Grid.Columns[0].Header.ToString();
                }

                // Writing column headers
                for (int i = 1; i <= countColumn; i++)
                {
                    columnHeaderText = columnHeaderText + ',' + Grid.Columns[i].Header.ToString();
                }
                csvFileWriter.WriteLine(columnHeaderText);

                // Writing values row by row
                string csv = "";
                for (int i = 0; i < TxDatas.Count; i++)
                {
                    TxDatas[i].No = i + 1;
                    csv += TxDatas[i].ToString().Replace(",,", ",\"\",").Replace(",,", ",\"\",") + Environment.NewLine;
                }
                Console.WriteLine(csv);
                csvFileWriter.Write(csv);

                csvFileWriter.Flush();
                csvFileWriter.Close();
            }
        }
        public void SaveRxNamingFile(DataGrid Grid)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "VTM RxNaming File (*.CRN)|*.CRN";
            saveDlg.FilterIndex = 0;
            saveDlg.RestoreDirectory = true;
            saveDlg.Title = "Save RxNaming File";
            if ((bool)saveDlg.ShowDialog())
            {
                string CsvFpath = saveDlg.FileName;
                System.IO.StreamWriter csvFileWriter = new StreamWriter(CsvFpath, false);
                string columnHeaderText = "";

                int countColumn = Grid.Columns.Count - 1;
                if (countColumn >= 0)
                {
                    columnHeaderText = Grid.Columns[0].Header.ToString();
                }

                // Writing column headers
                for (int i = 1; i <= countColumn; i++)
                {
                    columnHeaderText = columnHeaderText + ',' + Grid.Columns[i].Header.ToString();
                }
                csvFileWriter.WriteLine(columnHeaderText);

                // Writing values row by row
                string csv = "";
                for (int i = 0; i < RxDatas.Count; i++)
                {
                    RxDatas[i].No = i + 1;
                    csv += RxDatas[i].ToString().Replace(",,", ",\"\",").Replace(",,", ",\"\",") + Environment.NewLine;
                }
                Console.WriteLine(csv);
                csvFileWriter.Write(csv);

                csvFileWriter.Flush();
                csvFileWriter.Close();
            }
        }
        public void SaveQRNamingFile(DataGrid Grid)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "VTM QRNaming File (*.ILN)|*.ILN";
            saveDlg.FilterIndex = 0;
            saveDlg.RestoreDirectory = true;
            saveDlg.Title = "Save QRNaming File";
            if ((bool)saveDlg.ShowDialog())
            {
                string CsvFpath = saveDlg.FileName;
                System.IO.StreamWriter csvFileWriter = new StreamWriter(CsvFpath, false);
                string columnHeaderText = "";

                int countColumn = Grid.Columns.Count - 1;
                if (countColumn >= 0)
                {
                    columnHeaderText = Grid.Columns[0].Header.ToString();
                }

                // Writing column headers
                for (int i = 1; i <= countColumn; i++)
                {
                    columnHeaderText = columnHeaderText + ',' + Grid.Columns[i].Header.ToString();
                }
                csvFileWriter.WriteLine(columnHeaderText);

                // Writing values row by row
                string csv = "";
                for (int i = 0; i < QRDatas.Count; i++)
                {
                    QRDatas[i].No = i + 1;
                    csv += QRDatas[i].ToString().Replace(",,", ",\"\",").Replace(",,", ",\"\",") + Environment.NewLine;
                }
                Console.WriteLine(csv);
                csvFileWriter.Write(csv);

                csvFileWriter.Flush();
                csvFileWriter.Close();
            }
        }


    }
}
