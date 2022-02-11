using HVT.Utility;
using HVT.VTM.Base;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using static HVT.VTM.Base.Model;

namespace HVT.VTM.Program
{
    public partial class Program
    {
        public enum RunTestState
        {
            WAIT,
            READY,
            TESTTING,
            STOP,
            PAUSE,
            GOOD,
            FAIL,
            BUSY,
            DONE,
        }

        #region Varialble
        // Load new model and clone to application model edit.
        private Model rootModel = new Model();
        public Model RootModel
        {
            get { return rootModel; }
            set
            {
                rootModel = value;
                if (editAbleModel == null)
                {
                    EditAbleModel = (Model)value.Clone();

                }
            }
        }

        // EditAbleModel change notify
        private Model editAbleModel;
        public Model EditAbleModel
        {
            get { return editAbleModel; }
            set
            {
                editAbleModel = value;
                NoticeOfChange();
            }
        }

        public Command Command = new Command();
        #endregion

        #region Event
        // Event model actions
        public event EventHandler EditAbleModel_Change;
        private void NoticeOfChange()
        {
            EditAbleModel_Change?.Invoke(null, null);
        }

        public event EventHandler ModelChangeEvent;
        private void ModelChange()
        {
            ModelChangeEvent?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        // Model init 
        public void ModelInit()
        {
            TestState = Program.RunTestState.READY;
            //Task.Run(new Action(delegate { RootModel.RunTest(); }));
        }

        //save
        public void SaveModel()
        {
            RootModel.CameraSetting = cameraStreaming?.GetParammeter();
            GetCardChannel();
            if (RootModel.Path == null || RootModel.Name == null || !File.Exists(RootModel.Path))
            {
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.Filter = "VTM model files (*.vmdl)|*.vmdl";
                saveFile.Title = "Save VTM model file.";
                if (saveFile.ShowDialog() == true)
                {
                    RootModel.Name = saveFile.FileName.Split('\\').Last().Replace(".vmdl", "");
                    RootModel.Path = saveFile.FileName;
                    HVT.Utility.Extensions.SaveToFile(RootModel, saveFile.FileName);
                }
            }
            else
            {
                HVT.Utility.Extensions.SaveToFile(RootModel, RootModel.Path);
                HVT.Utility.Debug.Write("Model saved: " + RootModel.Path, Utility.Debug.ContentType.Notify);
            }
        }

        public void SaveModelAs()
        {
            RootModel.CameraSetting = cameraStreaming?.GetParammeter();
            GetCardChannel();

            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "VTM model files (*.vmdl)|*.vmdl";
            saveFile.Title = "Save VTM model file.";
            if (saveFile.ShowDialog() == true)
            {
                RootModel.Path = saveFile.FileName;
                HVT.Utility.Extensions.SaveToFile(RootModel, saveFile.FileName);
                HVT.Utility.Debug.Write("Model saved: " + saveFile.FileName, Utility.Debug.ContentType.Notify);

            }
        }

        public void LoadModel()
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
                HVT.Utility.Debug.Write("Load model:" + openFile.FileName, Utility.Debug.ContentType.Notify);
                //var fileInfor = new FileInfo(openFile.FileName);
                string modelStr = System.IO.File.ReadAllText(openFile.FileName);
                try
                {
                    string modelString = HVT.Utility.Extensions.Decoder(modelStr, System.Text.Encoding.UTF7);

                    RootModel = HVT.Utility.Extensions.ConvertFromJson<Model>(modelString);
                    RootModel = HVT.Utility.Extensions.ConvertFromJson<Model>(modelString);
                    RootModel.Path = openFile.FileName;

                    UpdateDataAfterLoad();
                }
                catch (Exception)
                {
                    HVT.Utility.Debug.Write("Load model fail, file not correct format. \n" +
                        "Model folder: " + openFile.FileName, Utility.Debug.ContentType.Error);
                }

            }
        }
        public void LoadModel(string path)
        {

            HVT.Utility.Debug.Write("Load model:" + path, Utility.Debug.ContentType.Notify);
            //var fileInfor = new FileInfo(openFile.FileName);
            string modelStr = System.IO.File.ReadAllText(path);
            try
            {
                string modelString = HVT.Utility.Extensions.Decoder(modelStr, System.Text.Encoding.UTF7);

                RootModel = HVT.Utility.Extensions.ConvertFromJson<Model>(modelString);
                RootModel = HVT.Utility.Extensions.ConvertFromJson<Model>(modelString);
                RootModel.Path = path;

                UpdateDataAfterLoad();
            }
            catch (Exception)
            {
                HVT.Utility.Debug.Write("Load model fail, file not correct format. \n" +
                    "Model folder: " + path, Utility.Debug.ContentType.Error);
            }

        }

        public void ImportModel()
        {
            RootModel.Load();
            LoadNamingFromModel();
            RootModel.CleanSteps();

            ModelChange();
        }

        public void UpdateDataAfterLoad()
        {
            ModelChange();
            RootModel.CleanSteps();
            RootModel.LoadFinishEvent();
            LoadNamingFromModel();
            SetCardChannel();
            Console.WriteLine("Load model done");
            HVT.Utility.Debug.Write("Load model ok", Utility.Debug.ContentType.Notify);

        }

        public event EventHandler StepTestChange;
        public event EventHandler TestRunFinish;
        public event EventHandler StateChange;

        public bool IsTestting = false;
        public int StepTesting = 0;

        private RunTestState testState;
        public RunTestState TestState
        {
            get { return testState; }
            set
            {
                if (value != testState)
                {
                    testState = value;
                    StateChange?.Invoke(value, null);
                }
            }
        }

        public async void RUN_TEST()
        {
            if (!IsTestting)
            {
                await Task.Run(RunTest);
            }
        }

        public async void RUN_MANUAL_TEST()
        {
            if (!IsTestting)
            {
                await Task.Run(Run_Manual_Test);
            }
        }

        public async void RunTest()
        {
            foreach (var item in RootModel.Steps)
            {
                item.ValueGet1 = "";
                item.ValueGet2 = "";
                item.ValueGet3 = "";
                item.ValueGet4 = "";
                item.Result1 = "";
                item.Result2 = "";
                item.Result3 = "";
                item.Result4 = "";
            }

            foreach (var item in RootModel.LEDs)
            {
                item.CalculatorOutput = 0;
            }

            foreach (var item in RootModel.GLEDs)
            {
                item.CalculatorOutput = 0;
            }

            if (RootModel.contruction.PBAs[0].IsWaiting) UUTs[0].OpenPort();
            if (RootModel.contruction.PBAs[1].IsWaiting) UUTs[1].OpenPort();
            if (RootModel.contruction.PBAs[2].IsWaiting) UUTs[2].OpenPort();
            if (RootModel.contruction.PBAs[3].IsWaiting) UUTs[3].OpenPort();

            IsTestting = true;
            StepTesting = 0;
            var Steps = RootModel.Steps;
            while (IsTestting)
            {
                switch (TestState)
                {
                    case RunTestState.WAIT:
                        
                        break;
                    case RunTestState.TESTTING:
                        if (StepTesting >= Steps.Count)
                        {
                            bool TestOK = true;
                            for (int i = 0; i < RootModel.contruction.PCB_Count; i++)
                            {
                                TestOK = TestOK && RootModel.contruction.PBAs[i].GetStepResult(RootModel.Steps.ToList<Step>());

                            }
                            TestState = TestOK ? RunTestState.GOOD : RunTestState.FAIL;
                        }
                        else
                        {

                            var stepTest = Steps[StepTesting];
                            if (stepTest != null)
                            {

                                if (stepTest.cmd != CMDs.NON || !stepTest.Skip)
                                {
                                    StepTestChange?.Invoke(StepTesting, null);
                                    RUN_FUNCTION_TEST(stepTest);
                                    await Task.Delay(5); // delay for data binding
                                }
                            }
                            StepTesting++;
                        }
                        break;
                    case RunTestState.PAUSE:
                        Task.Delay(500).Wait();
                        break;
                    case RunTestState.STOP:
                        IsTestting = false;
                        StepTesting = 0;
                        StepTestChange?.Invoke(StepTesting, null);
                        TestState = RunTestState.PAUSE;
                        break;
                    case RunTestState.GOOD:
                        IsTestting = false;
                        if (RootModel.contruction.PBAs[0].IsWaiting) UUTs[0].ClosePort();
                        if (RootModel.contruction.PBAs[1].IsWaiting) UUTs[1].ClosePort();
                        if (RootModel.contruction.PBAs[2].IsWaiting) UUTs[2].ClosePort();
                        if (RootModel.contruction.PBAs[3].IsWaiting) UUTs[3].ClosePort();

                        Print_Test(appSetting.QR);
                        IsTestting = false;
                        TestRunFinish?.Invoke("", null);
                        break;
                    case RunTestState.FAIL:
                        IsTestting = false;
                        if (RootModel.contruction.PBAs[0].IsWaiting) UUTs[0].ClosePort();
                        if (RootModel.contruction.PBAs[1].IsWaiting) UUTs[1].ClosePort();
                        if (RootModel.contruction.PBAs[2].IsWaiting) UUTs[2].ClosePort();
                        if (RootModel.contruction.PBAs[3].IsWaiting) UUTs[3].ClosePort();

                        Print_Test(appSetting.QR);
                        IsTestting = false;
                        TestRunFinish?.Invoke("", null);
                        break;
                    case RunTestState.READY:
                        break;
                    default:
                        break;
                }
            }

        }

        public async void Run_Manual_Test()
        {
            IsTestting = true;
            for (int i = 0; i < RootModel.Steps.Count; i++)
            {
                if (TestState == RunTestState.STOP)
                {
                    TestState = RunTestState.DONE;
                    break;
                }
                var stepTest = RootModel.Steps[i];
                    if (stepTest != null)
                    {
                    StepTesting = i;
                        if (stepTest.cmd != CMDs.NON || !stepTest.Skip)
                        {
                            StepTestChange?.Invoke(i, null);
                            RUN_FUNCTION_TEST(stepTest);
                            await Task.Delay(5); // delay for data binding
                        }
                    }
                while (TestState == RunTestState.PAUSE)
                {
                    Task.Delay(100).Wait();
                }


            }
            IsTestting = false;
            TestState = RunTestState.DONE;

        }

        public void RUN_FUNCTION_TEST(Step step)
        {
            if (RootModel.contruction.PBAs[0].IsWaiting) step.ValueGet1 = "exe";
            if (RootModel.contruction.PBAs[1].IsWaiting) step.ValueGet2 = "exe";
            if (RootModel.contruction.PBAs[2].IsWaiting) step.ValueGet3 = "exe";
            if (RootModel.contruction.PBAs[3].IsWaiting) step.ValueGet4 = "exe";

            step.Result1 = Step.DontCare;
            step.Result2 = Step.DontCare;
            step.Result3 = Step.DontCare;
            step.Result4 = Step.DontCare;

            var PCB_SKIP_CHECK = RootModel.contruction.PBAs;

            if (step.Skip)
            {
                return;
            }
            switch (step.cmd)
            {
                case CMDs.NON:
                    break;
                case CMDs.PWR:
                    break;
                case CMDs.DLY:
                    if (int.TryParse(step.Oper, out int delayTime))
                    {
                        Task.Delay(delayTime).Wait();
                    }
                    break;
                case CMDs.GEN:
                    break;
                case CMDs.LOD:
                    break;
                case CMDs.RLY:
                    break;
                case CMDs.MAK:
                    break;
                case CMDs.DIS:
                    break;
                case CMDs.END:
                    break;
                case CMDs.ACV:
                    DMM_Read(step, PCB_SKIP_CHECK);
                    break;
                case CMDs.DCV:
                    DMM_Read(step, PCB_SKIP_CHECK);
                    break;
                case CMDs.FRQ:
                    DMM_Read(step, PCB_SKIP_CHECK);
                    break;
                case CMDs.RES:
                    DMM_Read(step, PCB_SKIP_CHECK);
                    break;
                case CMDs.URD:
                    URD(step, PCB_SKIP_CHECK);
                    break;
                case CMDs.UTN:
                    UTN(step, PCB_SKIP_CHECK);
                    break;
                case CMDs.UTX:
                    UTX(step, PCB_SKIP_CHECK);
                    break;
                case CMDs.UCN:
                    UCN(step, PCB_SKIP_CHECK);
                    break;
                case CMDs.UCP:
                    break;
                case CMDs.STL:
                    break;
                case CMDs.EDL:
                    break;
                case CMDs.LCC:
                    break;
                case CMDs.LEC:
                    break;
                case CMDs.CAL:
                    break;
                case CMDs.GLED:
                    ReadGLED(step, PCB_SKIP_CHECK);
                    break;
                case CMDs.FND:
                    ReadFND(step, PCB_SKIP_CHECK);
                    break;
                case CMDs.LED:
                    ReadLED(step, PCB_SKIP_CHECK);
                    break;
                case CMDs.LCD:
                    ReadLCD(step, PCB_SKIP_CHECK);
                    break;
                case CMDs.PCB:
                    break;
                case CMDs.CAM:
                    CAM(step);
                    break;
                default:
                    break;
            }
        }

        #region Functions Code

        // DMM read 
        public async void DMM_Read(Step step, ObservableCollection<PBA> PBAs)
        {
            var start = DateTime.Now;
            switch (step.cmd)
            {
                case CMDs.ACV:
                    DMM_Rate modeAC = (DMM_Rate)Enum.Parse(typeof(DMM_Rate), step.Condition2, true);
                    MUX_CARD.SetChannels(step.Condition1);
                    switch (modeAC)
                    {
                        case DMM_Rate.NONE:
                            break;
                        case DMM_Rate.SLOW:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_slow_ACVFRQ).Wait();
                            break;
                        case DMM_Rate.MID:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_Mid_ACVFRQ).Wait();
                            break;
                        case DMM_Rate.FAST:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_Fast_ACVFRQ).Wait();
                            break;
                        default:
                            break;
                    }
                    DMM1.ChangeRate(modeAC);
                    DMM2.ChangeRate(modeAC);
                    DMM1.SetMode(DMM_Mode.ACV).Wait();
                    DMM2.SetMode(DMM_Mode.ACV).Wait();
                    break;
                case CMDs.DCV:
                    MUX_CARD.SetChannels(step.Condition1);
                    DMM_Rate modeDC = (DMM_Rate)Enum.Parse(typeof(DMM_Rate), step.Condition2, true);
                    switch (modeDC)
                    {
                        case DMM_Rate.NONE:
                            break;
                        case DMM_Rate.SLOW:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_slow_DCV).Wait();
                            break;
                        case DMM_Rate.MID:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_Mid_DCV).Wait();
                            break;
                        case DMM_Rate.FAST:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_Fast_DCV).Wait();
                            break;
                        default:
                            break;
                    }
                    DMM1.ChangeRate(modeDC);
                    DMM2.ChangeRate(modeDC);
                    DMM1.SetMode(DMM_Mode.DCV).Wait();
                    DMM2.SetMode(DMM_Mode.DCV).Wait();
                    break;
                case CMDs.FRQ:
                    MUX_CARD.SetChannels(step.Condition1);
                    DMM_Rate modeFQR = (DMM_Rate)Enum.Parse(typeof(DMM_Rate), step.Condition2, true);
                    switch (modeFQR)
                    {
                        case DMM_Rate.NONE:
                            break;
                        case DMM_Rate.SLOW:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_slow_ACVFRQ).Wait();
                            break;
                        case DMM_Rate.MID:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_Mid_ACVFRQ).Wait();
                            break;
                        case DMM_Rate.FAST:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_Fast_ACVFRQ).Wait();
                            break;
                        default:
                            break;
                    }
                    DMM1.ChangeRate(modeFQR);
                    DMM2.ChangeRate(modeFQR);
                    DMM1.SetMode(DMM_Mode.FREQ).Wait();
                    DMM2.SetMode(DMM_Mode.FREQ).Wait();
                    break;
                case CMDs.RES:
                    MUX_CARD.SetChannels(step.Condition1);
                    DMM_Rate modeRES = (DMM_Rate)Enum.Parse(typeof(DMM_Rate), step.Condition2, true);
                    switch (modeRES)
                    {
                        case DMM_Rate.NONE:
                            break;
                        case DMM_Rate.SLOW:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_slow_RES).Wait();
                            break;
                        case DMM_Rate.MID:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_Mid_RES).Wait();
                            break;
                        case DMM_Rate.FAST:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_Fast_RES).Wait();
                            break;
                        default:
                            break;
                    }
                    DMM1.ChangeRate(modeRES);
                    DMM2.ChangeRate(modeRES);
                    DMM1.SetMode(DMM_Mode.RES).Wait();
                    DMM2.SetMode(DMM_Mode.RES).Wait();
                    break;
            }

            step.ValueGet1 = (await DMM1.GetValue(true)).ToString();
            step.ValueGet2 = (await DMM2.GetValue(true)).ToString();



            Console.WriteLine(DateTime.Now.Subtract(start).TotalMilliseconds);
        }


        public void URD(Step step, ObservableCollection<PBA> PBAs)
        {
            if (PBAs[0].IsWaiting) step.Result1 = UUTs[0].OpenPort() ? Step.Ok : Step.Ng;
            if (PBAs[1].IsWaiting) step.Result2 = UUTs[1].OpenPort() ? Step.Ok : Step.Ng;
            if (PBAs[2].IsWaiting) step.Result3 = UUTs[2].OpenPort() ? Step.Ok : Step.Ng;
            if (PBAs[3].IsWaiting) step.Result4 = UUTs[3].OpenPort() ? Step.Ok : Step.Ng;

            if (PBAs[0].IsWaiting) step.ValueGet1 = step.Result1 == Step.Ok ? Step.Ok : "Sys";
            if (PBAs[1].IsWaiting) step.ValueGet2 = step.Result2 == Step.Ok ? Step.Ok : "Sys";
            if (PBAs[2].IsWaiting) step.ValueGet3 = step.Result3 == Step.Ok ? Step.Ok : "Sys";
            if (PBAs[3].IsWaiting) step.ValueGet4 = step.Result4 == Step.Ok ? Step.Ok : "Sys";

            bool isOk = true;

            if (PBAs[0].IsWaiting) isOk = isOk && step.Result1 == Step.Ok;
            if (PBAs[1].IsWaiting) isOk = isOk && step.Result2 == Step.Ok;
            if (PBAs[2].IsWaiting) isOk = isOk && step.Result3 == Step.Ok;
            if (PBAs[3].IsWaiting) isOk = isOk && step.Result4 == Step.Ok;
        }

        public void CAM(Step step)
        {
            CameraStreaming.VideoProperties properties;
            Enum.TryParse<CameraStreaming.VideoProperties>(step.Condition1, out properties);
            if (properties == CameraStreaming.VideoProperties.Reset)
            {
                cameraStreaming.SetParammeter(RootModel.CameraSetting);
                return;
            }

            int value = 0;
            Int32.TryParse(step.Oper, out value);
            cameraStreaming.SetParammeter(properties, value, true);
        }

        public void UTN(Step step, ObservableCollection<PBA> PBAs)
        {
            TxData txData = new TxData();
            txData = RootModel.Naming.TxDatas.Where(x => x.Name == step.Condition1).DefaultIfEmpty(null).FirstOrDefault();

            foreach (var item in UUTs)
            {
                if (step.Oper == "P1") item.Config = RootModel.P1_Config;
                else if (step.Oper == "P2") item.Config = RootModel.P2_Config;
            }

            var startTime = DateTime.Now;
            Int32 delay = 10;
            Int32 limittime = 10;
            int tryCount = 1;
            Int32.TryParse(step.Count, out delay);
            Int32.TryParse(step.Condition2, out limittime);
            Int32.TryParse(step.Min, out tryCount);
            if (txData == null)
            {
                if (PBAs[0].IsWaiting) step.ValueGet1 = "Naming";
                if (PBAs[1].IsWaiting) step.ValueGet2 = "Naming";
                if (PBAs[2].IsWaiting) step.ValueGet3 = "Naming";
                if (PBAs[3].IsWaiting) step.ValueGet4 = "Naming";

                if (PBAs[0].IsWaiting) step.Result1 = step.ValueGet1 == Step.Ok ? Step.Ok : Step.Ng;
                if (PBAs[1].IsWaiting) step.Result2 = step.ValueGet2 == Step.Ok ? Step.Ok : Step.Ng;
                if (PBAs[2].IsWaiting) step.Result3 = step.ValueGet3 == Step.Ok ? Step.Ok : Step.Ng;
                if (PBAs[3].IsWaiting) step.Result4 = step.ValueGet4 == Step.Ok ? Step.Ok : Step.Ng;

                return;
            }
            for (int i = 0; i <= tryCount; i++)
            {
                switch (step.Mode)
                {
                    case "NORMAL":
                        if (PBAs[0].IsWaiting) step.Result1 = UUTs[0].Send(txData) ? Step.Ok : Step.Ng;
                        if (PBAs[1].IsWaiting) step.Result2 = UUTs[1].Send(txData) ? Step.Ok : Step.Ng;
                        if (PBAs[2].IsWaiting) step.Result3 = UUTs[2].Send(txData) ? Step.Ok : Step.Ng;
                        if (PBAs[3].IsWaiting) step.Result4 = UUTs[3].Send(txData) ? Step.Ok : Step.Ng;

                        if (PBAs[0].IsWaiting) step.ValueGet1 = step.Result1 == Step.Ok ? Step.Ok : "Tx";
                        if (PBAs[1].IsWaiting) step.ValueGet2 = step.Result2 == Step.Ok ? Step.Ok : "Tx";
                        if (PBAs[2].IsWaiting) step.ValueGet3 = step.Result3 == Step.Ok ? Step.Ok : "Tx";
                        if (PBAs[3].IsWaiting) step.ValueGet4 = step.Result4 == Step.Ok ? Step.Ok : "Tx";

                        bool isOk = true;

                        if (PBAs[0].IsWaiting) isOk = isOk && step.Result1 == Step.Ok;
                        if (PBAs[1].IsWaiting) isOk = isOk && step.Result2 == Step.Ok;
                        if (PBAs[2].IsWaiting) isOk = isOk && step.Result3 == Step.Ok;
                        if (PBAs[3].IsWaiting) isOk = isOk && step.Result4 == Step.Ok;

                        if (isOk)
                        {
                            goto Finish;
                        }
                        break;

                    case "R_WAIT":
                        if (step.Condition1 != null)
                        {
                            if (PBAs[0].IsWaiting) step.ValueGet1 = UUTs[0].Send(txData) ? Step.Ok : "Tx";
                            if (PBAs[1].IsWaiting) step.ValueGet2 = UUTs[1].Send(txData) ? Step.Ok : "Tx";
                            if (PBAs[2].IsWaiting) step.ValueGet3 = UUTs[2].Send(txData) ? Step.Ok : "Tx";
                            if (PBAs[3].IsWaiting) step.ValueGet4 = UUTs[3].Send(txData) ? Step.Ok : "Tx";

                            step.Result = true;
                            if (PBAs[0].IsWaiting) step.Result = step.Result || (step.Result1 == Step.Ok);
                            if (PBAs[1].IsWaiting) step.Result = step.Result || (step.Result2 == Step.Ok);
                            if (PBAs[2].IsWaiting) step.Result = step.Result || (step.Result3 == Step.Ok);
                            if (PBAs[3].IsWaiting) step.Result = step.Result || (step.Result4 == Step.Ok);
                            if (step.Result) break;

                            Console.WriteLine();
                            bool areadyHaveBuffer = true;
                            var startWaitRx = DateTime.Now;
                            while (DateTime.Now.Subtract(startWaitRx).TotalMilliseconds < limittime)
                            {
                                for (int pcb = 0; pcb < RootModel.contruction.PCB_Count; pcb++)
                                {
                                    areadyHaveBuffer = areadyHaveBuffer ? UUTs[pcb].HaveBuffer() : false;
                                }

                                if (areadyHaveBuffer)
                                {
                                    if (PBAs[0].IsWaiting) step.ValueGet1 = Step.Ok;
                                    if (PBAs[1].IsWaiting) step.ValueGet2 = Step.Ok;
                                    if (PBAs[2].IsWaiting) step.ValueGet3 = Step.Ok;
                                    if (PBAs[3].IsWaiting) step.ValueGet4 = Step.Ok;
                                    break;
                                }
                                else
                                {
                                    if (PBAs[0].IsWaiting && step.ValueGet1 != "Tx") step.ValueGet1 = UUTs[0].HaveBuffer() ? Step.Ok : "Rx";
                                    if (PBAs[1].IsWaiting && step.ValueGet2 != "Tx") step.ValueGet2 = UUTs[1].HaveBuffer() ? Step.Ok : "Rx";
                                    if (PBAs[2].IsWaiting && step.ValueGet3 != "Tx") step.ValueGet3 = UUTs[2].HaveBuffer() ? Step.Ok : "Rx";
                                    if (PBAs[3].IsWaiting && step.ValueGet4 != "Tx") step.ValueGet4 = UUTs[3].HaveBuffer() ? Step.Ok : "Rx";
                                }
                            }
                            if (areadyHaveBuffer) goto Finish;
                        }
                        break;
                    case "SEND_R":
                        if (step.Condition1 != null)
                        {
                            Console.WriteLine();
                            bool areadyHaveBuffer = true;

                            if (PBAs[0].IsWaiting) step.ValueGet1 = UUTs[0].Send(txData) ? Step.Ok : "Tx";
                            if (PBAs[1].IsWaiting) step.ValueGet2 = UUTs[1].Send(txData) ? Step.Ok : "Tx";
                            if (PBAs[2].IsWaiting) step.ValueGet3 = UUTs[2].Send(txData) ? Step.Ok : "Tx";
                            if (PBAs[3].IsWaiting) step.ValueGet4 = UUTs[3].Send(txData) ? Step.Ok : "Tx";

                            Task.Delay(delay).Wait();

                            for (int pcb = 0; pcb < RootModel.contruction.PCB_Count; pcb++)
                            {
                                areadyHaveBuffer = areadyHaveBuffer && UUTs[pcb].HaveBuffer();
                            }

                            if (areadyHaveBuffer)
                            {
                                if (PBAs[0].IsWaiting) step.ValueGet1 = Step.Ok;
                                if (PBAs[1].IsWaiting) step.ValueGet2 = Step.Ok;
                                if (PBAs[2].IsWaiting) step.ValueGet3 = Step.Ok;
                                if (PBAs[3].IsWaiting) step.ValueGet4 = Step.Ok;
                            }
                            else
                            {
                                if (PBAs[0].IsWaiting && step.ValueGet1 != "Tx") step.ValueGet1 = UUTs[0].HaveBuffer() ? Step.Ok : "Rx";
                                if (PBAs[1].IsWaiting && step.ValueGet2 != "Tx") step.ValueGet2 = UUTs[1].HaveBuffer() ? Step.Ok : "Rx";
                                if (PBAs[2].IsWaiting && step.ValueGet3 != "Tx") step.ValueGet3 = UUTs[2].HaveBuffer() ? Step.Ok : "Rx";
                                if (PBAs[3].IsWaiting && step.ValueGet4 != "Tx") step.ValueGet4 = UUTs[3].HaveBuffer() ? Step.Ok : "Rx";
                            }

                            if (areadyHaveBuffer) goto Finish;
                            if (DateTime.Now.Subtract(startTime).TotalMilliseconds > limittime) goto Finish;
                        }
                        break;
                    case "SEND-R":
                        if (step.Condition1 != null)
                        {
                            Console.WriteLine();
                            bool areadyHaveBuffer = true;

                            if (PBAs[0].IsWaiting) step.ValueGet1 = UUTs[0].Send(txData) ? Step.Ok : "Tx";
                            if (PBAs[1].IsWaiting) step.ValueGet2 = UUTs[1].Send(txData) ? Step.Ok : "Tx";
                            if (PBAs[2].IsWaiting) step.ValueGet3 = UUTs[2].Send(txData) ? Step.Ok : "Tx";
                            if (PBAs[3].IsWaiting) step.ValueGet4 = UUTs[3].Send(txData) ? Step.Ok : "Tx";

                            step.Result = true;

                            if (PBAs[0].IsWaiting) step.Result = step.Result || (step.Result1 == Step.Ok);
                            if (PBAs[1].IsWaiting) step.Result = step.Result || (step.Result2 == Step.Ok);
                            if (PBAs[2].IsWaiting) step.Result = step.Result || (step.Result3 == Step.Ok);
                            if (PBAs[3].IsWaiting) step.Result = step.Result || (step.Result4 == Step.Ok);

                            if (!step.Result) break;
                            var startCheck = DateTime.Now;
                            while (DateTime.Now.Subtract(startCheck).TotalMilliseconds < delay)
                            {
                                for (int pcb = 0; pcb < RootModel.contruction.PCB_Count; pcb++)
                                {
                                    areadyHaveBuffer = areadyHaveBuffer && UUTs[pcb].HaveBuffer();
                                }

                                if (areadyHaveBuffer)
                                {
                                    if (PBAs[0].IsWaiting) step.ValueGet1 = Step.Ok;
                                    if (PBAs[1].IsWaiting) step.ValueGet2 = Step.Ok;
                                    if (PBAs[2].IsWaiting) step.ValueGet3 = Step.Ok;
                                    if (PBAs[3].IsWaiting) step.ValueGet4 = Step.Ok;
                                }
                                else
                                {
                                    if (PBAs[0].IsWaiting && step.ValueGet1 != "Tx") step.ValueGet1 = UUTs[0].HaveBuffer() ? Step.Ok : "Rx";
                                    if (PBAs[1].IsWaiting && step.ValueGet2 != "Tx") step.ValueGet2 = UUTs[1].HaveBuffer() ? Step.Ok : "Rx";
                                    if (PBAs[2].IsWaiting && step.ValueGet3 != "Tx") step.ValueGet3 = UUTs[2].HaveBuffer() ? Step.Ok : "Rx";
                                    if (PBAs[3].IsWaiting && step.ValueGet4 != "Tx") step.ValueGet4 = UUTs[3].HaveBuffer() ? Step.Ok : "Rx";
                                }

                                if (areadyHaveBuffer) goto Finish;
                                if (DateTime.Now.Subtract(startTime).TotalMilliseconds > limittime) goto Finish;
                            }
                        }
                        break;
                    case "TIMER":
                        if (int.TryParse(step.Count, out int time))
                        {
                            if (PBAs[0].IsWaiting) step.ValueGet1 = UUTs[0].SendTimer(txData, time) ? Step.Ok : "Sys";
                            if (PBAs[1].IsWaiting) step.ValueGet2 = UUTs[1].SendTimer(txData, time) ? Step.Ok : "Sys";
                            if (PBAs[2].IsWaiting) step.ValueGet3 = UUTs[2].SendTimer(txData, time) ? Step.Ok : "Sys";
                            if (PBAs[3].IsWaiting) step.ValueGet4 = UUTs[3].SendTimer(txData, time) ? Step.Ok : "Sys";
                        }
                        else
                        {
                            if (PBAs[0].IsWaiting) step.ValueGet1 = "Set time";
                            if (PBAs[1].IsWaiting) step.ValueGet2 = "Set time";
                            if (PBAs[2].IsWaiting) step.ValueGet3 = "Set time";
                            if (PBAs[3].IsWaiting) step.ValueGet4 = "Set time";
                        }

                        break;
                    default:
                        break;
                }
                Console.WriteLine("Try count" + i.ToString());
            }

        Finish:
            if (PBAs[0].IsWaiting) step.Result1 = step.ValueGet1 == Step.Ok ? Step.Ok : Step.Ng;
            if (PBAs[1].IsWaiting) step.Result2 = step.ValueGet2 == Step.Ok ? Step.Ok : Step.Ng;
            if (PBAs[2].IsWaiting) step.Result3 = step.ValueGet3 == Step.Ok ? Step.Ok : Step.Ng;
            if (PBAs[3].IsWaiting) step.Result4 = step.ValueGet4 == Step.Ok ? Step.Ok : Step.Ng;

            if (PBAs[0].IsWaiting) step.Result = step.Result || (step.Result1 == Step.Ok);
            if (PBAs[1].IsWaiting) step.Result = step.Result || (step.Result2 == Step.Ok);
            if (PBAs[2].IsWaiting) step.Result = step.Result || (step.Result3 == Step.Ok);
            if (PBAs[3].IsWaiting) step.Result = step.Result || (step.Result4 == Step.Ok);

            Console.WriteLine("UTN time: " + DateTime.Now.Subtract(startTime).TotalMilliseconds);
            //Task.Delay(delay).Wait();

        }

        public void UTX(Step step, ObservableCollection<PBA> PBAs)
        {
            foreach (var item in UUTs)
            {
                if (step.Oper == "P1") item.Config = RootModel.P1_Config;
                else if (step.Oper == "P2") item.Config = RootModel.P2_Config;
            }

            var startTime = DateTime.Now;
            Int32 delay = 10;
            Int32 limittime = 10;
            int tryCount = 1;
            Int32.TryParse(step.Count, out delay);
            Int32.TryParse(step.Condition2, out limittime);
            Int32.TryParse(step.Min, out tryCount);

            for (int i = 0; i <= tryCount; i++)
            {
                switch (step.Mode)
                {
                    case "NORMAL":
                        if (step.Condition1 != null)
                        {
                            if (PBAs[0].IsWaiting) step.Result1 = UUTs[0].Send(step.Condition1) ? Step.Ok : Step.Ng;
                            if (PBAs[1].IsWaiting) step.Result2 = UUTs[1].Send(step.Condition1) ? Step.Ok : Step.Ng;
                            if (PBAs[2].IsWaiting) step.Result3 = UUTs[2].Send(step.Condition1) ? Step.Ok : Step.Ng;
                            if (PBAs[3].IsWaiting) step.Result4 = UUTs[3].Send(step.Condition1) ? Step.Ok : Step.Ng;

                            if (PBAs[0].IsWaiting) step.ValueGet1 = step.Result1 == Step.Ok ? Step.Ok : "Tx";
                            if (PBAs[1].IsWaiting) step.ValueGet2 = step.Result2 == Step.Ok ? Step.Ok : "Tx";
                            if (PBAs[2].IsWaiting) step.ValueGet3 = step.Result3 == Step.Ok ? Step.Ok : "Tx";
                            if (PBAs[3].IsWaiting) step.ValueGet4 = step.Result4 == Step.Ok ? Step.Ok : "Tx";
                        }
                        bool isOk = true;
                        if (PBAs[0].IsWaiting) isOk = isOk && step.Result1 == Step.Ok;
                        if (PBAs[1].IsWaiting) isOk = isOk && step.Result2 == Step.Ok;
                        if (PBAs[2].IsWaiting) isOk = isOk && step.Result3 == Step.Ok;
                        if (PBAs[3].IsWaiting) isOk = isOk && step.Result4 == Step.Ok;

                        if (isOk)
                        {
                            goto Finish;
                        }
                        break;

                    case "R_WAIT":
                        if (step.Condition1 != null)
                        {
                            if (PBAs[0].IsWaiting) step.ValueGet1 = UUTs[0].Send(step.Condition1) ? Step.Ok : "Tx";
                            if (PBAs[1].IsWaiting) step.ValueGet2 = UUTs[1].Send(step.Condition1) ? Step.Ok : "Tx";
                            if (PBAs[2].IsWaiting) step.ValueGet3 = UUTs[2].Send(step.Condition1) ? Step.Ok : "Tx";
                            if (PBAs[3].IsWaiting) step.ValueGet4 = UUTs[3].Send(step.Condition1) ? Step.Ok : "Tx";

                            step.Result = true;
                            if (PBAs[0].IsWaiting) step.Result = step.Result || (step.Result1 == Step.Ok);
                            if (PBAs[1].IsWaiting) step.Result = step.Result || (step.Result2 == Step.Ok);
                            if (PBAs[2].IsWaiting) step.Result = step.Result || (step.Result3 == Step.Ok);
                            if (PBAs[3].IsWaiting) step.Result = step.Result || (step.Result4 == Step.Ok);
                            if (step.Result) break;

                            Console.WriteLine();
                            bool areadyHaveBuffer = true;
                            var startWaitRx = DateTime.Now;
                            while (DateTime.Now.Subtract(startWaitRx).TotalMilliseconds < limittime)
                            {
                                for (int pcb = 0; pcb < RootModel.contruction.PCB_Count; pcb++)
                                {
                                    areadyHaveBuffer = areadyHaveBuffer ? UUTs[pcb].HaveBuffer() : false;
                                }

                                if (areadyHaveBuffer)
                                {
                                    if (PBAs[0].IsWaiting) step.ValueGet1 = Step.Ok;
                                    if (PBAs[1].IsWaiting) step.ValueGet2 = Step.Ok;
                                    if (PBAs[2].IsWaiting) step.ValueGet3 = Step.Ok;
                                    if (PBAs[3].IsWaiting) step.ValueGet4 = Step.Ok;
                                    break;
                                }
                                else
                                {

                                    if (PBAs[0].IsWaiting) step.ValueGet1 = UUTs[0].HaveBuffer() ? Step.Ok : "Rx";
                                    if (PBAs[1].IsWaiting) step.ValueGet2 = UUTs[1].HaveBuffer() ? Step.Ok : "Rx";
                                    if (PBAs[2].IsWaiting) step.ValueGet3 = UUTs[2].HaveBuffer() ? Step.Ok : "Rx";
                                    if (PBAs[3].IsWaiting) step.ValueGet4 = UUTs[3].HaveBuffer() ? Step.Ok : "Rx";
                                }
                                Task.Delay(50).Wait();
                            }
                            if (areadyHaveBuffer) goto Finish;

                        }
                        break;
                    case "SEND_R":
                        if (step.Condition1 != null)
                        {
                            Console.WriteLine();
                            bool areadyHaveBuffer = true;

                            if (PBAs[0].IsWaiting) step.ValueGet1 = UUTs[0].Send(step.Condition1) ? Step.Ok : "Tx";
                            if (PBAs[1].IsWaiting) step.ValueGet2 = UUTs[1].Send(step.Condition1) ? Step.Ok : "Tx";
                            if (PBAs[2].IsWaiting) step.ValueGet3 = UUTs[2].Send(step.Condition1) ? Step.Ok : "Tx";
                            if (PBAs[3].IsWaiting) step.ValueGet4 = UUTs[3].Send(step.Condition1) ? Step.Ok : "Tx";

                            step.Result = true;
                            if (PBAs[0].IsWaiting) step.Result = step.Result || (step.Result1 == Step.Ok);
                            if (PBAs[1].IsWaiting) step.Result = step.Result || (step.Result2 == Step.Ok);
                            if (PBAs[2].IsWaiting) step.Result = step.Result || (step.Result3 == Step.Ok);
                            if (PBAs[3].IsWaiting) step.Result = step.Result || (step.Result4 == Step.Ok);
                            if (step.Result) break;

                            Task.Delay(delay).Wait();

                            for (int pcb = 0; pcb < RootModel.contruction.PCB_Count; pcb++)
                            {
                                areadyHaveBuffer = areadyHaveBuffer && UUTs[pcb].HaveBuffer();
                            }

                            if (areadyHaveBuffer)
                            {
                                if (PBAs[0].IsWaiting) step.ValueGet1 = Step.Ok;
                                if (PBAs[1].IsWaiting) step.ValueGet2 = Step.Ok;
                                if (PBAs[2].IsWaiting) step.ValueGet3 = Step.Ok;
                                if (PBAs[3].IsWaiting) step.ValueGet4 = Step.Ok;
                            }
                            else
                            {
                                if (PBAs[0].IsWaiting) step.ValueGet1 = UUTs[0].HaveBuffer() ? Step.Ok : "Rx";
                                if (PBAs[1].IsWaiting) step.ValueGet2 = UUTs[1].HaveBuffer() ? Step.Ok : "Rx";
                                if (PBAs[2].IsWaiting) step.ValueGet3 = UUTs[2].HaveBuffer() ? Step.Ok : "Rx";
                                if (PBAs[3].IsWaiting) step.ValueGet4 = UUTs[3].HaveBuffer() ? Step.Ok : "Rx";
                            }

                            if (areadyHaveBuffer) goto Finish;
                            if (DateTime.Now.Subtract(startTime).TotalMilliseconds > limittime) goto Finish;
                        }
                        break;
                    case "TIMER":
                        if (int.TryParse(step.Count, out int time))
                        {
                            if (PBAs[0].IsWaiting) step.ValueGet1 = UUTs[0].SendTimer(step.Condition1, time) ? Step.Ok : "Sys";
                            if (PBAs[1].IsWaiting) step.ValueGet2 = UUTs[1].SendTimer(step.Condition1, time) ? Step.Ok : "Sys";
                            if (PBAs[2].IsWaiting) step.ValueGet3 = UUTs[2].SendTimer(step.Condition1, time) ? Step.Ok : "Sys";
                            if (PBAs[3].IsWaiting) step.ValueGet4 = UUTs[3].SendTimer(step.Condition1, time) ? Step.Ok : "Sys";
                        }
                        else
                        {
                            if (PBAs[0].IsWaiting) step.ValueGet1 = "Set time";
                            if (PBAs[1].IsWaiting) step.ValueGet2 = "Set time";
                            if (PBAs[2].IsWaiting) step.ValueGet3 = "Set time";
                            if (PBAs[3].IsWaiting) step.ValueGet4 = "Set time";
                        }


                        break;
                    default:
                        break;
                }
                Console.WriteLine("Try count" + i.ToString());
            }

        Finish:
            if (PBAs[0].IsWaiting) step.Result1 = step.ValueGet1 == Step.Ok ? Step.Ok : Step.Ng;
            if (PBAs[1].IsWaiting) step.Result2 = step.ValueGet2 == Step.Ok ? Step.Ok : Step.Ng;
            if (PBAs[2].IsWaiting) step.Result3 = step.ValueGet3 == Step.Ok ? Step.Ok : Step.Ng;
            if (PBAs[3].IsWaiting) step.Result4 = step.ValueGet4 == Step.Ok ? Step.Ok : Step.Ng;
            step.Result = true;
            if (PBAs[0].IsWaiting) step.Result = step.Result || (step.Result1 == Step.Ok);
            if (PBAs[1].IsWaiting) step.Result = step.Result || (step.Result2 == Step.Ok);
            if (PBAs[2].IsWaiting) step.Result = step.Result || (step.Result3 == Step.Ok);
            if (PBAs[3].IsWaiting) step.Result = step.Result || (step.Result4 == Step.Ok);                                                
            //Task.Delay(delay).Wait();
        }

        public void UCN(Step step, ObservableCollection<PBA> PBAs)
        {
            RxData rxData = new RxData();
            rxData = RootModel.Naming.RxDatas.Where(x => x.Name == step.Condition1).DefaultIfEmpty(null).FirstOrDefault();

            if (rxData != null)
            {
                if (PBAs[0].IsWaiting) step.ValueGet1 = UUTs[0].CheckBufferString(rxData);
                if (PBAs[1].IsWaiting) step.ValueGet2 = UUTs[1].CheckBufferString(rxData);
                if (PBAs[2].IsWaiting) step.ValueGet3 = UUTs[2].CheckBufferString(rxData);
                if (PBAs[3].IsWaiting) step.ValueGet4 = UUTs[3].CheckBufferString(rxData);

                if (PBAs[0].IsWaiting) step.Result1 = step.ValueGet1 == step.Spect ? Step.Ok : Step.Ng;
                if (PBAs[1].IsWaiting) step.Result2 = step.ValueGet2 == step.Spect ? Step.Ok : Step.Ng;
                if (PBAs[2].IsWaiting) step.Result3 = step.ValueGet3 == step.Spect ? Step.Ok : Step.Ng;
                if (PBAs[3].IsWaiting) step.Result4 = step.ValueGet4 == step.Spect ? Step.Ok : Step.Ng;
            }
            else
            {
                if (PBAs[0].IsWaiting) step.Result1 = Step.Ng;
                if (PBAs[1].IsWaiting) step.Result2 = Step.Ng;
                if (PBAs[2].IsWaiting) step.Result3 = Step.Ng;
                if (PBAs[3].IsWaiting) step.Result4 = Step.Ng;
            }
        }

        public void ReadLCD(Step step, ObservableCollection<PBA> PBAs)
        {
            step.Result = true;
            if (int.TryParse(step.Condition2, out int scanTime))
            {
                DateTime start = DateTime.Now;
                while (DateTime.Now.Subtract(start).TotalMilliseconds < scanTime)
                {
                    if (PBAs[0].IsWaiting) step.ValueGet1 = step.Result1 != Step.Ok ? RootModel.LCDs[0].Data : step.ValueGet1;
                    if (PBAs[1].IsWaiting) step.ValueGet2 = step.Result2 != Step.Ok ? RootModel.LCDs[1].Data : step.ValueGet2;
                    if (PBAs[2].IsWaiting) step.ValueGet3 = step.Result3 != Step.Ok ? RootModel.LCDs[2].Data : step.ValueGet3;
                    if (PBAs[3].IsWaiting) step.ValueGet4 = step.Result4 != Step.Ok ? RootModel.LCDs[3].Data : step.ValueGet4;



                    if (PBAs[0].IsWaiting) step.Result1 = step.ValueGet1 == step.Oper ? Step.Ok : Step.Ng;
                    if (PBAs[1].IsWaiting) step.Result2 = step.ValueGet2 == step.Oper ? Step.Ok : Step.Ng;
                    if (PBAs[2].IsWaiting) step.Result3 = step.ValueGet3 == step.Oper ? Step.Ok : Step.Ng;
                    if (PBAs[3].IsWaiting) step.Result4 = step.ValueGet4 == step.Oper ? Step.Ok : Step.Ng;



                    if (PBAs[0].IsWaiting) step.Result &= (step.Result1 == Step.Ok);
                    if (PBAs[1].IsWaiting) step.Result &= (step.Result2 == Step.Ok);
                    if (PBAs[2].IsWaiting) step.Result &= (step.Result3 == Step.Ok);
                    if (PBAs[3].IsWaiting) step.Result &= (step.Result4 == Step.Ok);

                    if (step.Result)
                        break;

                    //Task.Delay(scanTime / 3).Wait();
                }
            }
            else
            {
                if (PBAs[0].IsWaiting) step.ValueGet1 = RootModel.LCDs[0].Data;
                if (PBAs[1].IsWaiting) step.ValueGet2 = RootModel.LCDs[1].Data;
                if (PBAs[2].IsWaiting) step.ValueGet3 = RootModel.LCDs[2].Data;
                if (PBAs[3].IsWaiting) step.ValueGet4 = RootModel.LCDs[3].Data;

                if (PBAs[0].IsWaiting) step.Result1 = step.ValueGet1 == step.Oper ? Step.Ok : Step.Ng;
                if (PBAs[1].IsWaiting) step.Result2 = step.ValueGet2 == step.Oper ? Step.Ok : Step.Ng;
                if (PBAs[2].IsWaiting) step.Result3 = step.ValueGet3 == step.Oper ? Step.Ok : Step.Ng;
                if (PBAs[3].IsWaiting) step.Result4 = step.ValueGet4 == step.Oper ? Step.Ok : Step.Ng;

                if (PBAs[0].IsWaiting) step.Result = step.Result & (step.Result1 == Step.Ok);
                if (PBAs[1].IsWaiting) step.Result = step.Result & (step.Result2 == Step.Ok);
                if (PBAs[2].IsWaiting) step.Result = step.Result & (step.Result3 == Step.Ok);
                if (PBAs[3].IsWaiting) step.Result = step.Result & (step.Result4 == Step.Ok);
            }

        }

        public void ReadFND(Step step, ObservableCollection<PBA> PBAs)
        {
            step.Result = true;
            if (int.TryParse(step.Condition2, out int scanTime))
            {
                DateTime start = DateTime.Now;
                while (DateTime.Now.Subtract(start).TotalMilliseconds < scanTime)
                {
                    step.ValueGet1 = step.Result1 != Step.Ok ? RootModel.FNDs[0].Data : step.ValueGet1;
                    step.ValueGet2 = step.Result2 != Step.Ok ? RootModel.FNDs[1].Data : step.ValueGet2;
                    step.ValueGet3 = step.Result3 != Step.Ok ? RootModel.FNDs[2].Data : step.ValueGet3;
                    step.ValueGet4 = step.Result4 != Step.Ok ? RootModel.FNDs[3].Data : step.ValueGet4;

                    step.Result1 = step.ValueGet1 == step.Oper ? Step.Ok : Step.Ng;
                    step.Result2 = step.ValueGet2 == step.Oper ? Step.Ok : Step.Ng;
                    step.Result3 = step.ValueGet3 == step.Oper ? Step.Ok : Step.Ng;
                    step.Result4 = step.ValueGet4 == step.Oper ? Step.Ok : Step.Ng;

                    if (PBAs[0].IsWaiting) step.Result = step.Result || (step.Result1 == Step.Ok);
                    if (PBAs[1].IsWaiting) step.Result = step.Result || (step.Result2 == Step.Ok);
                    if (PBAs[2].IsWaiting) step.Result = step.Result || (step.Result3 == Step.Ok);
                    if (PBAs[3].IsWaiting) step.Result = step.Result || (step.Result4 == Step.Ok);
                    if (step.Result) break;
                }
            }
            else
            {
                step.ValueGet1 = RootModel.FNDs[0].Data;
                step.ValueGet2 = RootModel.FNDs[1].Data;
                step.ValueGet3 = RootModel.FNDs[2].Data;
                step.ValueGet4 = RootModel.FNDs[3].Data;

                step.Result1 = step.ValueGet1 == step.Oper ? Step.Ok : Step.Ng;
                step.Result2 = step.ValueGet2 == step.Oper ? Step.Ok : Step.Ng;
                step.Result3 = step.ValueGet3 == step.Oper ? Step.Ok : Step.Ng;
                step.Result4 = step.ValueGet4 == step.Oper ? Step.Ok : Step.Ng;

                if (PBAs[0].IsWaiting) step.Result = step.Result || (step.Result1 == Step.Ok);
                if (PBAs[1].IsWaiting) step.Result = step.Result || (step.Result2 == Step.Ok);
                if (PBAs[2].IsWaiting) step.Result = step.Result || (step.Result3 == Step.Ok);
                if (PBAs[3].IsWaiting) step.Result = step.Result || (step.Result4 == Step.Ok);
            }

        }

        public void ReadLED(Step step, ObservableCollection<PBA> PBAs)
        {
            step.Result = true;

            step.ValueGet1 = PBAs[0].IsWaiting ? RootModel.LEDs[0].CalculatorOutputString : "";
            step.ValueGet2 = PBAs[1].IsWaiting ? RootModel.LEDs[1].CalculatorOutputString : "";
            step.ValueGet3 = PBAs[2].IsWaiting ? RootModel.LEDs[2].CalculatorOutputString : "";
            step.ValueGet4 = PBAs[3].IsWaiting ? RootModel.LEDs[3].CalculatorOutputString : "";

            step.Result1 = step.ValueGet1 == step.Oper ? Step.Ok : Step.Ng;
            step.Result2 = step.ValueGet2 == step.Oper ? Step.Ok : Step.Ng;
            step.Result3 = step.ValueGet3 == step.Oper ? Step.Ok : Step.Ng;
            step.Result4 = step.ValueGet4 == step.Oper ? Step.Ok : Step.Ng;

            if (PBAs[0].IsWaiting) step.Result = step.Result || (step.Result1 == Step.Ok);
            if (PBAs[1].IsWaiting) step.Result = step.Result || (step.Result2 == Step.Ok);
            if (PBAs[2].IsWaiting) step.Result = step.Result || (step.Result3 == Step.Ok);
            if (PBAs[3].IsWaiting) step.Result = step.Result || (step.Result4 == Step.Ok);
        }

        public void ReadGLED(Step step, ObservableCollection<PBA> PBAs)
        {
            step.Result = true;

            step.ValueGet1 = PBAs[0].IsWaiting ? RootModel.GLEDs[0].CalculatorOutputString : "";
            step.ValueGet2 = PBAs[1].IsWaiting ? RootModel.GLEDs[1].CalculatorOutputString : "";
            step.ValueGet3 = PBAs[2].IsWaiting ? RootModel.GLEDs[2].CalculatorOutputString : "";
            step.ValueGet4 = PBAs[3].IsWaiting ? RootModel.GLEDs[3].CalculatorOutputString : "";

            step.Result1 = step.ValueGet1 == step.Oper ? Step.Ok : Step.Ng;
            step.Result2 = step.ValueGet2 == step.Oper ? Step.Ok : Step.Ng;
            step.Result3 = step.ValueGet3 == step.Oper ? Step.Ok : Step.Ng;
            step.Result4 = step.ValueGet4 == step.Oper ? Step.Ok : Step.Ng;

            if (PBAs[0].IsWaiting) step.Result = step.Result || (step.Result1 == Step.Ok);
            if (PBAs[1].IsWaiting) step.Result = step.Result || (step.Result2 == Step.Ok);
            if (PBAs[2].IsWaiting) step.Result = step.Result || (step.Result3 == Step.Ok);
            if (PBAs[3].IsWaiting) step.Result = step.Result || (step.Result4 == Step.Ok);

        }
        #endregion

    }
}
