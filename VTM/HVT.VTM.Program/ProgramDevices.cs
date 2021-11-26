using HVT.VTM.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Linq;

namespace HVT.VTM.Program
{
    public partial class Program
    {
        #region SerialDevide check connect
        public async void ConnectCheck()
        {
            await DMM1.SearchCom();
            await DMM2.SearchCom();
            //await PPS1.SearchCom();
            //await PPS2.SearchCom();
        }
        #endregion

        #region PPS

        public PPS PPS1 = new PPS();
        public PPS PPS2 = new PPS() { ID = "EP900099" };

        // PPS display value UI Link
        Label ST1_CH1_V, ST1_CH2_V, ST2_CH1_V, ST2_CH2_V, ST1_CH1_A, ST1_CH2_A, ST2_CH1_A, ST2_CH2_A;

        //
        public void PPS_Init(Label ST1_CH1_V, Label ST1_CH2_V, Label ST2_CH1_V, Label ST2_CH2_V, Label ST1_CH1_A, Label ST1_CH2_A, Label ST2_CH1_A, Label ST2_CH2_A, Rectangle PPS1Rx, Rectangle PPS1Tx, Rectangle PPS1Status, Rectangle PPS2Rx, Rectangle PPS2Tx, Rectangle PPS2Status)
        {
            this.ST2_CH1_A = ST2_CH1_A;
            this.ST2_CH1_V = ST2_CH1_V;
            this.ST2_CH2_A = ST2_CH2_A;
            this.ST2_CH2_V = ST2_CH2_V;

            this.ST1_CH1_A = ST1_CH1_A;
            this.ST1_CH2_A = ST1_CH2_A;
            this.ST1_CH1_V = ST1_CH1_V;
            this.ST1_CH2_V = ST1_CH2_V;

            PPS1.serialDisplay.RX = PPS1Rx;
            PPS1.serialDisplay.TX = PPS1Tx;
            PPS1.serialDisplay.IsOpenRect = PPS1Status;
            PPS1.OnConnected += PPS1_OnConnected;
            PPS1.OnSend += PPS1_OnSend;
            PPS1.OnRecive += PPS1_OnRecive;
            PPS1.OnChange += PPS1_OnChange;

            PPS2.serialDisplay.RX = PPS2Rx;
            PPS2.serialDisplay.TX = PPS2Tx;
            PPS2.serialDisplay.IsOpenRect = PPS2Status;
            PPS2.OnConnected += PPS2_OnConnected;
            PPS2.OnSend += PPS2_OnSend;
            PPS2.OnRecive += PPS2_OnRecive;
            PPS2.OnChange += PPS2_OnChange;
        }

        private void PPS2_OnChange(object sender, EventArgs e)
        {
            if (ST2_CH1_V != null) ST2_CH1_V.Dispatcher.BeginInvoke(new Action(delegate { ST2_CH1_V.Content = PPS2.Site1.ActualVolt.ToString() + "V"; }));
            if (ST2_CH1_A != null) ST2_CH1_A.Dispatcher.BeginInvoke(new Action(delegate { ST2_CH1_A.Content = PPS2.Site1.ActualAmpe.ToString() + "A"; }));
            if (ST2_CH2_V != null) ST2_CH2_V.Dispatcher.BeginInvoke(new Action(delegate { ST2_CH2_V.Content = PPS2.Site2.ActualVolt.ToString() + "V"; }));
            if (ST2_CH2_A != null) ST2_CH2_A.Dispatcher.BeginInvoke(new Action(delegate { ST2_CH2_A.Content = PPS2.Site2.ActualAmpe.ToString() + "A"; }));

            Console.WriteLine(PPS2.Site1.ActualVolt + "V");
            Console.WriteLine(PPS2.Site2.ActualVolt + "V");
            Console.WriteLine(PPS2.Site1.ActualAmpe + "A");
            Console.WriteLine(PPS2.Site2.ActualAmpe + "A");
        }

        private void PPS1_OnChange(object sender, EventArgs e)
        {
            if (ST1_CH1_V != null) ST1_CH1_V.Dispatcher.BeginInvoke(new Action(delegate { ST1_CH1_V.Content = PPS2.Site1.ActualVolt.ToString() + "V"; }));
            if (ST1_CH1_A != null) ST1_CH1_A.Dispatcher.BeginInvoke(new Action(delegate { ST1_CH1_A.Content = PPS2.Site1.ActualAmpe.ToString() + "A"; }));
            if (ST1_CH2_V != null) ST1_CH2_V.Dispatcher.BeginInvoke(new Action(delegate { ST1_CH2_V.Content = PPS2.Site2.ActualVolt.ToString() + "V"; }));
            if (ST1_CH2_A != null) ST1_CH2_A.Dispatcher.BeginInvoke(new Action(delegate { ST1_CH2_A.Content = PPS2.Site2.ActualAmpe.ToString() + "A"; }));

            Console.WriteLine(PPS1.Site1.ActualVolt + "V");
            Console.WriteLine(PPS1.Site2.ActualVolt + "V");
            Console.WriteLine(PPS1.Site1.ActualAmpe + "A");
            Console.WriteLine(PPS1.Site2.ActualAmpe + "A");
        }

        private void PPS1_OnRecive(object sender, EventArgs e)
        {
            PPS1.serialDisplay.BlinkRX();
        }

        private void PPS1_OnSend(object sender, EventArgs e)
        {
            PPS1.serialDisplay.BlinkTX();
        }

        private void PPS1_OnConnected(object sender, EventArgs e)
        {
            PPS1.serialDisplay.ShowCOMStatus((bool)sender);
        }

        private void PPS2_OnRecive(object sender, EventArgs e)
        {
            PPS2.serialDisplay.BlinkRX();
        }

        private void PPS2_OnSend(object sender, EventArgs e)
        {
            PPS2.serialDisplay.BlinkTX();
        }

        private void PPS2_OnConnected(object sender, EventArgs e)
        {
            PPS2.serialDisplay.ShowCOMStatus((bool)sender);
        }

        public async Task<bool> OnSite(PPS pps, bool IsOn)
        {
            bool OnOK = await pps.SetParam(PPS.SetFlag.SetOutput, 0, IsOn ? 1 : 0);
            return OnOK;
        }

        #endregion

        #region DMM
        public DMM DMM1 = new DMM() { SN = "GEQ840113" };
        public DMM DMM2 = new DMM() { SN = "GET857611" };

        Label MinVal_DMM1, MaxVal_DMM1, Arg_DMM1, Val_DMM1, MinVal_DMM2, MaxVal_DMM2, Arg_DMM2, Val_DMM2;


        public void DMM_UI_Init(
                    Label MinVal_DMM1, Label MaxVal_DMM1, Label Arg_DMM1, Label Val_DMM1, Label MinVal_DMM2, Label MaxVal_DMM2, Label Arg_DMM2, Label Val_DMM2,
                    Rectangle RxDMM1, Rectangle TxDMM1, Rectangle ConnectDMM1, Rectangle RxDMM2, Rectangle TxDMM2, Rectangle ConnectDMM2)
        {
            this.MinVal_DMM1 = MinVal_DMM1;
            this.MaxVal_DMM1 = MaxVal_DMM1;
            this.Arg_DMM1 = Arg_DMM1;
            this.Val_DMM1 = Val_DMM1;
            DMM1.serialDisplay.RX = RxDMM1;
            DMM1.serialDisplay.TX = TxDMM1;
            DMM1.serialDisplay.IsOpenRect = ConnectDMM1;

            DMM1.OnChange += DMM1_OnChange;
            DMM1.OnConnected += DMM1_OnConnected;
            DMM1.OnRecive += DMM1_OnRecive;
            DMM1.OnSend += DMM1_OnSend;

            this.MinVal_DMM2 = MinVal_DMM2;
            this.MaxVal_DMM2 = MaxVal_DMM2;
            this.Arg_DMM2 = Arg_DMM2;
            this.Val_DMM2 = Val_DMM2;
            DMM2.serialDisplay.RX = RxDMM2;
            DMM2.serialDisplay.TX = TxDMM2;
            DMM2.serialDisplay.IsOpenRect = ConnectDMM2;

            DMM2.OnChange += DMM2_OnChange;
            DMM2.OnConnected += DMM2_OnConnected;
            DMM2.OnRecive += DMM2_OnRecive;
            DMM2.OnSend += DMM2_OnSend;

        }

        private void DMM1_OnSend(object sender, EventArgs e)
        {
            DMM1.serialDisplay.BlinkTX();
        }

        private void DMM1_OnRecive(object sender, EventArgs e)
        {
            DMM1.serialDisplay.BlinkRX();
        }

        private void DMM1_OnConnected(object sender, EventArgs e)
        {
            DMM1.serialDisplay.ShowCOMStatus((bool)sender);
        }

        private void DMM1_OnChange(object sender, EventArgs e)
        {
            if (Val_DMM1 != null) Val_DMM1.Dispatcher.Invoke(new Action(delegate { Val_DMM1.Content = sender as string; }));
        }

        private void DMM2_OnSend(object sender, EventArgs e)
        {
            DMM2.serialDisplay.BlinkTX();
        }

        private void DMM2_OnRecive(object sender, EventArgs e)
        {
            DMM2.serialDisplay.BlinkRX();
        }

        private void DMM2_OnConnected(object sender, EventArgs e)
        {
            DMM2.serialDisplay.ShowCOMStatus((bool)sender);
        }

        private void DMM2_OnChange(object sender, EventArgs e)
        {
            if (Val_DMM2 != null) Val_DMM2.Dispatcher.Invoke(new Action(delegate { Val_DMM2.Content = sender as string; }));
        }

        public void DMM_ChangeMode(DMM_Mode mode)
        {
            Task.Run(async () =>
                {
                    await DMM1.SetMode(mode);
                    DMM1.GetValue();
                });
            Task.Run(async () =>
            {
                await DMM2.SetMode(mode);
                DMM2.GetValue();
            });
        }
        public void DMM_GetMeasure()
        {
            Task.Run(async () =>
            {
                DMM1.GetValue();
            });
            Task.Run(async () =>
            {
                DMM2.GetValue();
            });
        }

        public void DMM_ChangeRange(int SelectedRangeItem)
        {
            DMM1.ChangeRange(SelectedRangeItem);
            DMM2.ChangeRange(SelectedRangeItem);
        }

        public void DMM_ChangeRate(DMM_Rate _Rate)
        {
            DMM1.ChangeRate(_Rate);
            DMM2.ChangeRate(_Rate);
        }
        #endregion

        #region Barcode Scaner
        BarcodeReader BarcodeReader { get; set; } = new BarcodeReader();
        DataGrid BarcodelistGrid;
        public void BarcodeReaderInit(DataGrid BarcodesGrid, Rectangle RxRect, Rectangle ConnectedRect)
        {
            BarcodelistGrid = BarcodesGrid;
            BarcodelistGrid.Dispatcher.Invoke(new Action(delegate {
                BarcodelistGrid.ItemsSource = RootModel.Barcodes;
            }));

            BarcodeReader.SerialDisplay.RX = RxRect;
            BarcodeReader.SerialDisplay.IsOpenRect = ConnectedRect;
            BarcodeReader.OnReciverData += BarcodeReader_OnReciverData;
            BarcodeReader.OnConnected += BarcodeReader_OnConnected;
            BarcodeReader.SerialStartConnect();

        }

        private void BarcodeReader_OnReciverData(object sender, EventArgs e)
        {
            bool haveHandle = false;
            if (RootModel.Barcodes.Select(o => o.BarcodeData).Contains(BarcodeReader.BarcodeBuffer.Replace("\r", "")))
            {
                return;
            }

            for (int i = 0; i < RootModel.Barcodes.Count; i++)
            {
                if (RootModel.Barcodes[i].BarcodeData == "")
                {
                    RootModel.Barcodes[i].BarcodeData = BarcodeReader.BarcodeBuffer.Replace("\r", "");
                    haveHandle = true;
                    break;
                }
            }
            if (!haveHandle)
            {
                for (int i = 0; i < RootModel.BarcodesWaitting.Count; i++)
                {
                    RootModel.BarcodesWaitting[i] = BarcodeReader.BarcodeBuffer;
                    break;
                }
            }
            BarcodeReader.SerialDisplay.BlinkRX();
            BarcodelistGrid.Dispatcher.Invoke(new Action(delegate {
                BarcodelistGrid.ItemsSource = null;
                BarcodelistGrid.ItemsSource = RootModel.Barcodes;
            }));
        }

        private void BarcodeReader_OnConnected(object sender, EventArgs e)
        {
            BarcodeReader.SerialDisplay.ShowCOMStatus(BarcodeReader.serialPort.IsOpen);
        }

        #endregion

        #region Mux card
        public MuxCard MUX_CARD = new MuxCard();
        public DataGrid MuxCardGridP1, MuxCardGridP2;
        public void MuxUIInit(DataGrid MuxCardGrid1, DataGrid MuxCardGrid2)
        {
            this.MuxCardGridP1 = MuxCardGrid1;
            this.MuxCardGridP1.ItemsSource = MUX_CARD.ChanelsEdittingPart1;
            this.MuxCardGridP1 = MuxCardGrid2;
            this.MuxCardGridP1.ItemsSource = MUX_CARD.ChanelsEdittingPart2;
            MUX_CARD.UpdateMainMuxChannels();
        }
        #endregion
    }
}
