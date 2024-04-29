using HVT.VTM.Base;
using HVT.Utility;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HVT.VTM.Program
{
    public partial class Program
    {
        private ObservableCollection<Board> _Boards = new ObservableCollection<Board>();
        public ObservableCollection<Board> Boards
        {
            get { return _Boards; }
            set
            {
                if (value != null || value != _Boards) _Boards = value;
            }
        }

        public BoardResultPanel ResultPanel = new BoardResultPanel();

        public void SetBoards()
        {
            Boards.Clear();
            for (int i = 0; i < TestModel.Layout.PCB_Count; i++)
            {
                Boards.Add(new Board()
                {
                    ModelSource = TestModel.Path,
                    ModelName = TestModel.Name
                });
            }
            if (Boards.Count >= 1) Boards[0].SiteName = "A";
            if (Boards.Count >= 2) Boards[1].SiteName = "B";
            if (Boards.Count >= 3) Boards[2].SiteName = "C";
            if (Boards.Count >= 4) Boards[3].SiteName = "D";
            switch (Boards.Count)
            {
                case 1:
                    for (int channel = 0; channel < 96; channel++)
                    {
                        Boards[0].MuxChannels.Add(MuxCard.Card.Chanels[channel]);
                    }

                    for (int channel = 0; channel < 8; channel++)
                    {
                        Boards[0].LevelChannels.Add(TestModel.LevelCard.Chanels[channel]);
                    }
                    for (int channel = 16; channel < 40; channel++)
                    {
                        Boards[0].LevelChannels.Add(TestModel.LevelCard.Chanels[channel]);
                    }
                    for (int channel = 48; channel < 64; channel++)
                    {
                        Boards[0].LevelChannels.Add(TestModel.LevelCard.Chanels[channel]);
                    }
                    break;
                case 2:
                    for (int channel = 0; channel < 48; channel++)
                    {
                        Boards[0].MuxChannels.Add(MuxCard.Card.Chanels[channel]);
                        Boards[1].MuxChannels.Add(MuxCard.Card.Chanels[channel + 48]);
                    }
                    for (int channel = 0; channel < 32; channel++)
                    {
                        if (channel < 8 || channel > 15)
                        {
                            Boards[0].LevelChannels.Add(TestModel.LevelCard.Chanels[channel]);
                            Boards[1].LevelChannels.Add(TestModel.LevelCard.Chanels[channel + 32]);
                        }
                    }
                    break;
                case 3:
                    for (int channel = 0; channel < 24; channel++)
                    {
                        Boards[0].MuxChannels.Add(MuxCard.Card.Chanels[channel]);
                        Boards[1].MuxChannels.Add(MuxCard.Card.Chanels[channel + 48]);
                        Boards[2].MuxChannels.Add(MuxCard.Card.Chanels[channel + 24]);
                    }

                    for (int channel = 0; channel < 4; channel++)
                    {
                        Boards[0].LevelChannels.Add(TestModel.LevelCard.Chanels[channel]);
                        Boards[1].LevelChannels.Add(TestModel.LevelCard.Chanels[channel + 32]);
                        Boards[2].LevelChannels.Add(TestModel.LevelCard.Chanels[channel + 4]);
                    }
                    for (int channel = 16; channel < 24; channel++)
                    {
                        Boards[0].LevelChannels.Add(TestModel.LevelCard.Chanels[channel]);
                        Boards[1].LevelChannels.Add(TestModel.LevelCard.Chanels[channel + 32]);
                        Boards[2].LevelChannels.Add(TestModel.LevelCard.Chanels[channel + 8]);
                    }

                    break;
                case 4:
                    for (int channel = 0; channel < 24; channel++)
                    {
                        Boards[0].MuxChannels.Add(MuxCard.Card.Chanels[channel]);
                        Boards[1].MuxChannels.Add(MuxCard.Card.Chanels[channel + 48]);
                        Boards[2].MuxChannels.Add(MuxCard.Card.Chanels[channel + 24]);
                        Boards[3].MuxChannels.Add(MuxCard.Card.Chanels[channel + 72]);
                    }

                    for (int channel = 0; channel < 4; channel++)
                    {
                        Boards[0].LevelChannels.Add(TestModel.LevelCard.Chanels[channel]);
                        Boards[1].LevelChannels.Add(TestModel.LevelCard.Chanels[channel + 32]);
                        Boards[2].LevelChannels.Add(TestModel.LevelCard.Chanels[channel + 4]);
                        Boards[3].LevelChannels.Add(TestModel.LevelCard.Chanels[channel + 36]);
                    }
                    for (int channel = 16; channel < 24; channel++)
                    {
                        Boards[0].LevelChannels.Add(TestModel.LevelCard.Chanels[channel]);
                        Boards[1].LevelChannels.Add(TestModel.LevelCard.Chanels[channel + 32]);
                        Boards[2].LevelChannels.Add(TestModel.LevelCard.Chanels[channel + 8]);
                        Boards[3].LevelChannels.Add(TestModel.LevelCard.Chanels[channel + 40]);
                    }
                    break;
                default:
                    break;
            }
            for (int i = 0; i < Boards[0].LevelChannels.Count(); i++)
            {
                if (Boards.Count >= 2) Boards[1].LevelChannels[i].IsUse = Boards[0].LevelChannels[i].IsUse;
                if (Boards.Count >= 3) Boards[2].LevelChannels[i].IsUse = Boards[0].LevelChannels[i].IsUse;
                if (Boards.Count >= 4) Boards[3].LevelChannels[i].IsUse = Boards[0].LevelChannels[i].IsUse;
            }
        }

        /// <summary>
        /// Turn MUX channel on
        /// </summary>
        /// <param name="paramString">MUX channel each board (1 ~ 48)</param>
        /// <param name="board">board index (1 ~ 4)</param>
        public bool SetBoardMux(string paramString, int boardIndex, out bool IsMux2)
        {
            int P = 99;
            int N = 99;
            IsMux2 = false;

            if (boardIndex > Boards.Count) return false;

            if (paramString.Contains("/"))
            {
                var channelStrs = paramString.Split('/');
                if (!int.TryParse(channelStrs[0], out P))
                {
                    return false;
                }

                if (!int.TryParse(channelStrs[1], out N))
                {
                    return false;
                }

                switch (Boards.Count)
                {
                    case 1:
                        if (P >= 49) IsMux2 = true;
                        return MuxCard.Card.ManualSetCardStatus(P, N);
                    case 2:
                        if (P >= 49) return false;
                        switch (boardIndex)
                        {
                            case 1:
                                return MuxCard.Card.ManualSetCardStatus(P, N);
                            case 2:
                                return MuxCard.Card.ManualSetCardStatus(P + 48, N + 48);
                            default:
                                break;
                        }
                        break;
                    case 3:
                        if (P >= 25) return false;
                        switch (boardIndex)
                        {
                            case 1:
                                return MuxCard.Card.ManualSetCardStatus(P, N);
                            case 2:
                                return MuxCard.Card.ManualSetCardStatus(P + 48, N + 48);
                            case 3:
                                return MuxCard.Card.ManualSetCardStatus(P + 24, N + 24);
                            default:
                                break;
                        }
                        break;
                    case 4:
                        if (P >= 25) return false;
                        switch (boardIndex)
                        {
                            case 1:
                                return MuxCard.Card.ManualSetCardStatus(P, N);
                            case 2:
                                return MuxCard.Card.ManualSetCardStatus(P + 48, N + 48);
                            case 3:
                                return MuxCard.Card.ManualSetCardStatus(P + 24, N + 24);
                            case 4:
                                return MuxCard.Card.ManualSetCardStatus(P + 72, N + 72);
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (!int.TryParse(paramString, out P))
                {
                    return false;
                }
                else
                {
                    switch (Boards.Count)
                    {
                        case 1:
                            if (P >= 49) IsMux2 = true;
                            return MuxCard.Card.ManualSetCardStatus(P);
                        case 2:
                            if (P >= 49) return false;

                            switch (boardIndex)
                            {
                                case 1:
                                    return MuxCard.Card.ManualSetCardStatus(P);
                                case 2:
                                    return MuxCard.Card.ManualSetCardStatus(P + 48);
                                default:
                                    break;
                            }
                            break;
                        case 3:
                            if (P >= 25) return false;

                            switch (boardIndex)
                            {
                                case 1:
                                    return MuxCard.Card.ManualSetCardStatus(P);
                                case 2:
                                    return MuxCard.Card.ManualSetCardStatus(P + 48);
                                case 3:
                                    return MuxCard.Card.ManualSetCardStatus(P + 24);
                                default:
                                    break;
                            }
                            break;
                        case 4:
                            if (P >= 25) return false;

                            switch (boardIndex)
                            {
                                case 1:
                                    return MuxCard.Card.ManualSetCardStatus(P, N);
                                case 2:
                                    return MuxCard.Card.ManualSetCardStatus(P + 48);
                                case 3:
                                    return MuxCard.Card.ManualSetCardStatus(P + 24);
                                case 4:
                                    return MuxCard.Card.ManualSetCardStatus(P + 72);
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return false;
        }
        // Barcode 
        public void CheckBarcodeReader(string COMPORT)
        {
            BarcodeReader.Port = new System.IO.Ports.SerialPort()
            {
                PortName = appSetting.Communication.ScannerPort,
                BaudRate = appSetting.Communication.Scan_Baudrate,
                DataBits = appSetting.Communication.Scan_Databit,
                Parity = appSetting.Communication.Scan_Parity
            };
            BarcodeReader.DevieceName = "Scanner";
            BarcodeReader.PortName = appSetting.Communication.ScannerPort;
            BarcodeReader.SerialDataReciver -= BarcodeReader_SerialDataReciver;
            BarcodeReader.SerialDataReciver += BarcodeReader_SerialDataReciver;
            BarcodeReader.Port.ReadTimeout = 1000;
            BarcodeReader.Port.NewLine = "\r";
            try
            {
                BarcodeReader.Port.Open();
                BarcodeReader.OpenPort();
            }
            catch (Exception err)
            {
                HVT.Utility.Debug.Write(String.Format("Scanner -> {0}: {1}", BarcodeReader.PortName, err.Message), Debug.ContentType.Error);
            }
        }

        private bool IsDuplicatedBarcodeHandler;
        private void BarcodeReader_SerialDataReciver(object sender, EventArgs e)
        {
            if (IsTestting)
            {
                BarcodeReader.Port.DiscardInBuffer();
                return;
            }
            string barcode = "";

            barcode = BarcodeReader.ReadLine();
            Console.WriteLine(barcode);
            BarcodeReader.Port.DiscardInBuffer();
            if (TestModel.BarcodeOption.BarcodeCheck(barcode))
            {
                if (Boards.Where(x => x.Barcode == barcode).Count() > 0)
                {
                    if (!IsDuplicatedBarcodeHandler)
                    {
                        IsDuplicatedBarcodeHandler = true;
                        return;
                    }
                    else
                    {
                        IsDuplicatedBarcodeHandler = false;
                        foreach (var item in Boards)
                        {
                            item.Barcode = "";
                        }
                        foreach (var item in Boards)
                        {
                            if (item.Barcode == "" && !item.Skip)
                            {
                                Debug.Appent(String.Format("\t{0} Barcode input: {1}", item.SiteName, barcode), Debug.ContentType.Notify);
                                item.Barcode = barcode;
                                break;
                            }
                        }
                        return;
                    }
                }
                else
                {
                    IsDuplicatedBarcodeHandler = false;

                    if (!IsTestting)
                    {
                        var barcodeGetted = false;
                        if (!IsDuplicatedBarcodeHandler)
                        {
                            foreach (var item in Boards)
                            {
                                if (item.Barcode == "" && !item.Skip)
                                {
                                    Debug.Appent(String.Format("\t{0} Barcode input: {1}", item.SiteName, barcode), Debug.ContentType.Notify);
                                    item.Barcode = barcode;
                                    barcodeGetted = true;
                                    break;
                                }
                            }
                        }
                        if (!barcodeGetted && IsDuplicatedBarcodeHandler)
                        {
                            IsDuplicatedBarcodeHandler = false;
                            foreach (var item in Boards)
                            {
                                item.Barcode = "";
                            }
                            foreach (var item in Boards)
                            {
                                if (item.Barcode == "" && !item.Skip)
                                {
                                    Debug.Appent(String.Format("\t{0} Barcode input: {1}", item.SiteName, barcode), Debug.ContentType.Notify);
                                    item.Barcode = barcode;
                                    barcodeGetted = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        var barcodeGetted = false;
                        if (!IsDuplicatedBarcodeHandler)
                        {
                            foreach (var item in Boards)
                            {
                                if (item.Barcode == "" && !item.Skip)
                                {
                                    Debug.Appent(String.Format("\t{0} Barcode input: {1}", item.SiteName, barcode), Debug.ContentType.Notify);
                                    item.Barcode = barcode;
                                    barcodeGetted = true;
                                    break;
                                }
                            }
                        }
                        if (!barcodeGetted && IsDuplicatedBarcodeHandler)
                        {
                            IsDuplicatedBarcodeHandler = false;
                            foreach (var item in Boards)
                            {
                                item.Barcode = "";
                            }
                            foreach (var item in Boards)
                            {
                                if (item.Barcode == "" && !item.Skip)
                                {
                                    Debug.Appent(String.Format("\t{0} Barcode input: {1}", item.SiteName, barcode), Debug.ContentType.Notify);
                                    item.Barcode = barcode;
                                    barcodeGetted = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.Appent(String.Format("\tBarcode input nor correct format: {0}", barcode), Debug.ContentType.Error);
            }
        }

        public void CheckBoardReady()
        {

        }
    }
}
