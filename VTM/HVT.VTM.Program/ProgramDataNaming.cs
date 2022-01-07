using HVT.VTM.Base;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HVT.VTM.Program
{
    public partial class Program
    {
        public Naming testtingNaming;

        private Naming naming = new Naming();
        public Naming Naming
        { 
            get { return naming; }
            set {
                naming = value;
                if (testtingNaming == null)
                {
                    testtingNaming = value;
                }
            } 
        }

        public DataGrid TxGrid, RxGrid, QRGrid;
        public TextBox QRlink;

        public void UUTpageInit(DataGrid gridTx, DataGrid gridRx, DataGrid gridQR, TextBox textBoxQRlink)
        {
            TxGrid = gridTx;
            RxGrid = gridRx;
            QRGrid = gridQR;
            QRlink = textBoxQRlink;

            Naming.TxDatas.Add(new TxData());

            RxGrid.ItemsSource = Naming.RxDatas;
            QRGrid.ItemsSource = Naming.QRDatas;
            TxGrid.ItemsSource = Naming.TxDatas;
            
        }

        public async void OpenQRNaming()
        {
            if (QRGrid == null)
            {
                Console.WriteLine("Program naming : QRGrid is null ");
            }
            await Task.Run(() =>
            {
                _ = QRGrid.Dispatcher.BeginInvoke
                (new Action(
                    delegate
                    {
                        if (QRlink != null)
                        {
                            QRlink.Text = Naming.OpenQRNamingFile();

                            CommandDescriptions.QRnaming = Naming.QRDatas.Select(o => o.Context).ToList();
                            Command.UpdateCommand();
                            RootModel.Naming = Naming;
                        }
                    }
                    ));

                return Task.CompletedTask;
            });
        }
        public async void OpenRxNaming()
        {
            if (RxGrid == null)
            {
                Console.WriteLine("Program naming : RxGrid is null ");
            }
            await Task.Run(() =>
            {
                _ = RxGrid.Dispatcher.BeginInvoke
                (new Action(
                    delegate
                    {
                        Naming.OpenRxNamingFile();
                        CommandDescriptions.RXnaming = Naming.RxDatas.Select(o => o.Name).ToList();
                        Command.UpdateCommand();
                        RootModel.Naming = Naming;
                    }
                    ));

                return Task.CompletedTask;
            });
        }
        public async void OpenTxNaming()
        {
            if (TxGrid == null)
            {
                Console.WriteLine("Program naming : TxGrid is null ");
            }
            await Task.Run(() =>
            {
                _ = TxGrid.Dispatcher.BeginInvoke
                      (new Action(
                          delegate
                          {
                              Naming.OpenTxNamingFile();
                              CommandDescriptions.TXnaming = Naming.TxDatas.Select(o => o.Name).ToList();
                              Command.UpdateCommand();
                              RootModel.Naming = Naming;
                          }
                          ));
                return Task.CompletedTask;
            });
        }
        public async void SaveQRNaming()
        {
            if (QRGrid == null)
            {
                Console.WriteLine("Program naming : QRGrid is null ");
            }
            await Task.Run(() =>
            {
                _ = QRGrid.Dispatcher.BeginInvoke
               (new Action(
                   delegate
                   {
                       Naming.SaveQRNamingFile(QRGrid);
                       CommandDescriptions.QRnaming = Naming.QRDatas.Select(o => o.Context).ToList();
                       Command.UpdateCommand();
                       RootModel.Naming = Naming;
                   }
                   ));
                return Task.CompletedTask;
            });
        }
        public async void SaveTxNaming()
        {
            if (TxGrid == null)
            {
                Console.WriteLine("Program naming : TxGrid is null ");
            }
            await Task.Run(() =>
            {
                _ = TxGrid.Dispatcher.BeginInvoke
               (new Action(
                   delegate
                   {
                       Naming.SaveTxNamingFile(TxGrid);
                       CommandDescriptions.TXnaming = Naming.TxDatas.Select(o => o.Name).ToList();
                       Command.UpdateCommand();
                       RootModel.Naming = Naming;
                   }
                   ));
                return Task.CompletedTask;
            });
        }
        public async void SaveRxNaming()
        {
            if (TxGrid == null)
            {
                Console.WriteLine("Program naming : RxGrid is null ");
            }
            await Task.Run(() =>
            {
                _ = RxGrid.Dispatcher.BeginInvoke
               (new Action(
                   delegate
                   {
                       Naming.SaveRxNamingFile(RxGrid);
                       CommandDescriptions.RXnaming = Naming.RxDatas.Select(o => o.Name).ToList();
                       Command.UpdateCommand();
                       RootModel.Naming = Naming;
                   }
                   ));
                return Task.CompletedTask;
            });
        }
        public void pasteTxNamingFromClipBoard(int pasteLocation, String pasteValue)
        {
            if (pasteValue.Contains("Name	Data (Hex)	Blank (Hex)	Remark"))
            {

                var pasteValues = pasteValue.Replace("\r\n", "\n").Split('\n');
                var location = pasteLocation;
                for (int i = 1; i < pasteValues.Length; i++)
                {
                    var item = pasteValues[i];
                    var dataItem = item.Split('\t');
                    if (dataItem.Length == TxGrid.Columns.Count)
                    {

                        Naming.TxDatas.Insert(location, new TxData()
                        {
                            //No = Convert.ToInt32(dataItem[0]),
                            Name = dataItem[0],
                            Data = dataItem[1],
                            Blank = dataItem[2],
                            Remark = dataItem[3]
                        });
                        location++;
                    }
                }
                for (int i = 0; i < Naming.TxDatas.Count; i++)
                {
                    Naming.TxDatas[i].No = i + 1;
                }
                TxGrid.Dispatcher.BeginInvoke(new Action(delegate { TxGrid.Items.Refresh(); }));
            }
        }
        public void pasteRxNamingFromClipBoard(int pasteLocation, String pasteValue)
        {
            if (pasteValue.Contains("Name	Mode Loc	Mode (Hex)	Data Kind	M Byte	M bit	L bit	L Byte	M bit	L bit	Type	Remark"))
            {
                var pasteValues = pasteValue.Replace("\r\n", "\n").Split('\n');
                var location = pasteLocation;
                for (int i = 1; i < pasteValues.Length; i++)
                {
                    var item = pasteValues[i];
                    var dataItem = item.Split('\t');
                    if (dataItem.Length == RxGrid.Columns.Count)
                    {
                        Naming.RxDatas.Insert(location, new RxData()
                        {
                            //No = Convert.ToInt32(dataItem[0]),
                            Name = dataItem[0],
                            ModeLoc = dataItem[1],
                            Mode = dataItem[2],
                            DataKind = dataItem[3],
                            MByte = dataItem[4],
                            M_Mbit = dataItem[5],
                            M_Lbit = dataItem[6],
                            LByte = dataItem[7],
                            L_Mbit = dataItem[8],
                            L_Lbit = dataItem[9],
                            Type = dataItem[10],
                            Remark = dataItem[11]
                        });
                        location++;
                    }
                }
                for (int i = 0; i < Naming.TxDatas.Count; i++)
                {
                    Naming.RxDatas[i].No = i + 1;
                }
                RxGrid.Dispatcher.BeginInvoke(new Action(delegate { RxGrid.Items.Refresh(); }));
            }
        }
        public void pasteQrNamingFromClipBoard(int pasteLocation, String pasteValue)
        {
            if (pasteValue.Contains("Contents	Code"))
            {
                var pasteValues = pasteValue.Replace("\r\n", "\n").Split('\n');
                var location = pasteLocation;
                for (int i = 1; i < pasteValues.Length; i++)
                {
                    var item = pasteValues[i];
                    var dataItem = item.Split('\t');
                    if (dataItem.Length == QRGrid.Columns.Count)
                    {
                        Naming.QRDatas.Insert(location, new QRData()
                        {
                            Context = dataItem[0],
                            Code = dataItem[1],
                        });
                        location++;
                    }
                }
                for (int i = 0; i < Naming.TxDatas.Count; i++)
                {
                    Naming.QRDatas[i].No = i + 1;
                }
                QRGrid.Dispatcher.BeginInvoke(new Action(delegate { QRGrid.Items.Refresh(); }));
            }
        }


        // after load new model file, load naming to resource, update naming datagrids 
        public void LoadNamingFromModel()
        {
            if (RootModel.Naming != null)
            {
                Naming = RootModel.Naming;
                CommandDescriptions.TXnaming = RootModel.Naming == null ? null : RootModel.Naming.TxDatas.Select(o => o.Name).ToList();
                CommandDescriptions.RXnaming = RootModel.Naming == null ? null : RootModel.Naming.RxDatas.Select(o => o.Name).ToList();
                CommandDescriptions.QRnaming = RootModel.Naming == null ? null : RootModel.Naming.QRDatas.Select(o => o.Context).ToList();
                Command.UpdateCommand();
                TxGrid.ItemsSource = Naming.TxDatas;
                RxGrid.ItemsSource = Naming.RxDatas;
                QRGrid.ItemsSource = Naming.QRDatas;
            }
        }


    }
}
