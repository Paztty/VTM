using HVT.VTM.Base;
using HVT.Utility;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using HVT.Controls;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using OpenCvSharp;
using System.Runtime.Remoting.Channels;

namespace HVT.VTM.Program
{
    public partial class Program
    {
        bool IsloadModel = false;
        private Model testModel = new Model();
        public Model TestModel
        {
            get { return testModel; }
            set
            {
                TestState = RunTestState.STOP;
                if (value != testModel)
                {
                    testModel = value;
                    SetBoards();
                    IsloadModel = true;
                    Debug.Appent("New model loaded: " + value.Path, Debug.ContentType.Notify);
                    Debug.Appent("\tUsing barcode input: " + (testModel.BarcodeOption.UseBarcodeInput ? "YES" : "NO"), testModel.BarcodeOption.UseBarcodeInput ? Debug.ContentType.Notify : Debug.ContentType.Warning);
                    Debug.Appent("\tDischarge before test: " + (testModel.Discharge.CheckBeforeTest ? "YES" : "NO"), testModel.Discharge.CheckBeforeTest ? Debug.ContentType.Notify : Debug.ContentType.Warning);
                }
                TestState = RunTestState.WAIT;
            }
        }

        public FolderMap AppFolder = new FolderMap();

        public event EventHandler StepTestChange;
        public event EventHandler TestRunFinish;
        public event EventHandler StateChange;
        public event EventHandler EscapTimeChange;
        public event EventHandler TesttingStateChange;

        private bool _IsTestting;
        public bool IsTestting
        {
            get { return _IsTestting; }
            set
            {
                if (value != _IsTestting)
                {
                    _IsTestting = value;
                    TesttingStateChange?.Invoke(value, null);
                }
            }
        }

        public int StepTesting = 0;

        private int FailReTestStep = 0;

        private double _EscapTime;
        public double EscapTime
        {
            get { return _EscapTime; }
            set
            {
                _EscapTime = value;
                EscapTimeChange?.Invoke(_EscapTime, null);
            }
        }
        System.Timers.Timer EscapTimer = new System.Timers.Timer()
        {
            Interval = 100,
            Enabled = true,
        };


        public enum RunTestState
        {
            WAIT,
            READY,
            TESTTING,
            MANUALTEST,
            STOP,
            PAUSE,
            GOOD,
            FAIL,
            BUSY,
            DONE,
        }

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
                    switch (testState)
                    {
                        case RunTestState.WAIT:
                            if (TestModel.BarcodeOption.UseBarcodeInput)
                            {
                                Debug.Write("Waiting barcode", Debug.ContentType.Warning);
                            }
                            EscapTimer.Stop();
                            EscapTime = 0;
                            break;
                        case RunTestState.READY:
                            Debug.Write("Ready", Debug.ContentType.Notify, 15);
                            EscapTimer.Stop();
                            EscapTime = 0;
                            break;
                        case RunTestState.TESTTING:
                            Debug.Write("Test start", Debug.ContentType.Warning, 20);
                            EscapTimer.Start();
                            break;
                        case RunTestState.MANUALTEST:
                            EscapTimer.Stop();
                            EscapTime = 0;
                            break;
                        case RunTestState.STOP:
                            EscapTimer.Stop();
                            EscapTime = 0;
                            break;
                        case RunTestState.PAUSE:
                            break;
                        case RunTestState.GOOD:
                            Debug.Write("Test done: GOOD", Debug.ContentType.Notify, 15);
                            EscapTimer.Stop();
                            break;
                        case RunTestState.FAIL:
                            Debug.Write("Test done: FAIL", Debug.ContentType.Error, 15);
                            EscapTimer.Stop();
                            EscapTime = 0;
                            break;
                        case RunTestState.BUSY:
                            EscapTimer.Stop();
                            EscapTime = 0;
                            break;
                        case RunTestState.DONE:
                            EscapTimer.Stop();
                            EscapTime = 0;
                            break;
                        default:
                            EscapTimer.Stop();
                            EscapTime = 0;
                            break;
                    }
                }
            }
        }

        public enum PageActive
        {
            AutoPage,
            ManualPage,
            ModelPage,
            VistionPage
        }
        public PageActive pageActive;

        public async void START()
        {
            TestState = RunTestState.BUSY;
            await Task.Run(ProgramState);
        }
        private async Task ProgramState()
        {
            while (true)
            {
                switch (TestState)
                {
                    case RunTestState.WAIT:
                        if (TestModel.BarcodeOption.UseBarcodeInput)
                        {
                            foreach (var item in Boards)
                            {
                                item.Skip = item.UserSkip;
                            }
                            bool Realdy = true;
                            if (Boards.Count >= 1) if (!Boards[0].Skip) Realdy &= Boards[0].BarcodeReady;
                            if (Boards.Count >= 2) if (!Boards[1].Skip) Realdy &= Boards[1].BarcodeReady;
                            if (Boards.Count >= 3) if (!Boards[2].Skip) Realdy &= Boards[2].BarcodeReady;
                            if (Boards.Count >= 4) if (!Boards[3].Skip) Realdy &= Boards[3].BarcodeReady;
                            if (Realdy)
                            {
                                SYSTEM.System_Board.MachineIO.BUZZER = false;
                                SYSTEM.System_Board.MachineIO.MainDOWN = false;
                                SYSTEM.System_Board.MachineIO.MainUP = true;
                                SYSTEM.System_Board.PowerRelease();
                                RELAY.Card.Release();
                                Solenoid.Card.Release();
                                MuxCard.Card.ReleaseChannels();
                                TestState = RunTestState.READY;
                            }
                            else if (IsTestting)
                            {
                                IsTestting = false;
                                Debug.Write("No barcode.", Debug.ContentType.Error, 30);
                                await Task.Delay(500);
                                SYSTEM.System_Board.MachineIO.BUZZER = false;
                                SYSTEM.System_Board.MachineIO.MainUP = true;
                                SYSTEM.System_Board.SendControl();
                            }
                        }
                        else
                        {
                            SYSTEM.System_Board.PowerRelease();
                            RELAY.Card.Release();
                            Solenoid.Card.Release();
                            MuxCard.Card.ReleaseChannels();
                            TestState = RunTestState.READY;
                        }
                        break;
                    case RunTestState.TESTTING:
                        //Clear boar detail
                        foreach (var item in Boards)
                        {
                            item.BoardDetail = "";
                            item.Skip = item.UserSkip;
                        }
                        //Delay before start
                        SYSTEM.System_Board.PowerRelease();
                        RELAY.Card.Release();
                        Solenoid.Card.Release();
                        MuxCard.Card.ReleaseChannels();
                        await Task.Delay(appSetting.Operations.StartDelaytime);

                        // Cleaning steps and set start parametter to boards
                        TestModel.CleanSteps();
                        IsTestting = true;
                        StepTesting = 0;
                        var Steps = TestModel.Steps;
                        if (Boards.Count >= 1) if (!Boards[0].Skip) Boards[0].StartTest = DateTime.Now;
                        if (Boards.Count >= 2) if (!Boards[1].Skip) Boards[1].StartTest = DateTime.Now;
                        if (Boards.Count >= 3) if (!Boards[2].Skip) Boards[2].StartTest = DateTime.Now;
                        if (Boards.Count >= 4) if (!Boards[3].Skip) Boards[3].StartTest = DateTime.Now;

                        //Discharge
                        if (TestModel.Discharge.CheckBeforeTest || appSetting.ETCSetting.UseDischargeTestStart)
                        {
                            if (!DisCharge() && appSetting.ETCSetting.UseDischargeError)
                            {
                                TestState = RunTestState.FAIL;
                                IsTestting = false;
                            }
                        }
                        //Start Test
                        while (IsTestting)
                        {
                            //Test done without END command
                            if (StepTesting >= Steps.Count)
                            {
                                bool TestOK = true;
                                if (Boards.Count >= 1) { Boards[0].Result = Boards[0].UserSkip ? "SKIP" : Steps.Select(x => x.Result1).Contains(Step.Ng) ? "FAIL" : "OK"; }
                                if (Boards.Count >= 2) { Boards[1].Result = Boards[1].UserSkip ? "SKIP" : Steps.Select(x => x.Result2).Contains(Step.Ng) ? "FAIL" : "OK"; }
                                if (Boards.Count >= 3) { Boards[2].Result = Boards[2].UserSkip ? "SKIP" : Steps.Select(x => x.Result3).Contains(Step.Ng) ? "FAIL" : "OK"; }
                                if (Boards.Count >= 4) { Boards[3].Result = Boards[3].UserSkip ? "SKIP" : Steps.Select(x => x.Result4).Contains(Step.Ng) ? "FAIL" : "OK"; }

                                foreach (var item in Boards)
                                {
                                    if (!item.UserSkip)
                                    {
                                        item.EndTest = DateTime.Now;
                                    }
                                }

                                SYSTEM.System_Board.MachineIO.ADSC = true;
                                SYSTEM.System_Board.MachineIO.BDSC = true;
                                SYSTEM.System_Board.MachineIO.MainUP = true;
                                SYSTEM.System_Board.MachineIO.LPG = true;

                                SYSTEM.System_Board.SendControl();
                                TestOK = Boards.Select(x => x.Result).Contains("FAIL");
                                TestState = TestOK ? RunTestState.FAIL : RunTestState.GOOD;
                                ResultPanel.ShowResult(Boards.ToList());
                                IsTestting = false;
                                break;
                            }
                            else
                            {
                                var stepTest = Steps[StepTesting];
                                if (stepTest != null)
                                {
                                    StepTestChange?.Invoke(StepTesting, null);
                                    if (stepTest.cmd != CMDs.NON && Steps[StepTesting].cmd != CMDs.END && !stepTest.Skip)
                                    {
                                        bool IsPass = RUN_FUNCTION_TEST(stepTest);

                                        //Test pass and ejump
                                        if (!IsPass && stepTest.E_Jump != 0)
                                        {
                                            FailReTestStep = stepTest.E_Jump - 1;
                                            int StepResetErr = stepTest.No - 1;
                                            for (int i = 0; i < appSetting.Operations.ErrorJumpCount; i++)
                                            {
                                                for (int stepRetest = FailReTestStep; stepRetest <= StepResetErr; stepRetest++)
                                                {
                                                    StepTesting = stepRetest;
                                                    StepTestChange?.Invoke(StepTesting, null);
                                                    stepTest = Steps[stepRetest];
                                                    IsPass = RUN_FUNCTION_TEST(stepTest);
                                                    if (IsPass && stepRetest == StepResetErr)
                                                    {
                                                        break;
                                                    }
                                                    if (!IsTestting || TestState != RunTestState.TESTTING)
                                                    {
                                                        break;
                                                    }
                                                    await Task.Delay(100);
                                                }
                                                if (IsPass)
                                                {
                                                    break;
                                                }
                                                if (!IsTestting || TestState != RunTestState.TESTTING)
                                                {
                                                    break;
                                                }
                                            }
                                        }

                                        //Stop with res test fail
                                        if (appSetting.Operations.FailResistanceStopAll && stepTest.cmd == CMDs.RES && !IsPass)
                                        {
                                            Debug.Write("RES step test fail -> Fail RES stop all enable -> Force stop all", Debug.ContentType.Error);
                                            IsTestting = false;
                                            TestState = RunTestState.STOP;
                                        }

                                        //Stop with stop all when fail 
                                        if (appSetting.Operations.FailStopAll && !IsPass)
                                        {
                                            Debug.Write("Step test fail -> Fail stop all enable -> Force stop all", Debug.ContentType.Error);
                                            IsTestting = false;
                                            TestState = RunTestState.STOP;
                                        }

                                        //Skip fail Step
                                        if (appSetting.Operations.FailStopPCB && !IsPass)
                                        {
                                            if (Boards.Count >= 1) { Boards[0].Result = Boards[0].Skip ? "SKIP" : Steps.Select(x => x.Result1).Contains(Step.Ng) ? "FAIL" : "OK"; }
                                            if (Boards.Count >= 2) { Boards[1].Result = Boards[1].Skip ? "SKIP" : Steps.Select(x => x.Result2).Contains(Step.Ng) ? "FAIL" : "OK"; }
                                            if (Boards.Count >= 3) { Boards[2].Result = Boards[2].Skip ? "SKIP" : Steps.Select(x => x.Result3).Contains(Step.Ng) ? "FAIL" : "OK"; }
                                            if (Boards.Count >= 4) { Boards[3].Result = Boards[3].Skip ? "SKIP" : Steps.Select(x => x.Result4).Contains(Step.Ng) ? "FAIL" : "OK"; }

                                            bool TestOK = true;

                                            if (Boards.Count >= 1) { Boards[0].Result = Boards[0].UserSkip ? "SKIP" : Steps.Select(x => x.Result1).Contains(Step.Ng) ? "FAIL" : "OK"; }
                                            if (Boards.Count >= 2) { Boards[1].Result = Boards[1].UserSkip ? "SKIP" : Steps.Select(x => x.Result2).Contains(Step.Ng) ? "FAIL" : "OK"; }
                                            if (Boards.Count >= 3) { Boards[2].Result = Boards[2].UserSkip ? "SKIP" : Steps.Select(x => x.Result3).Contains(Step.Ng) ? "FAIL" : "OK"; }
                                            if (Boards.Count >= 4) { Boards[3].Result = Boards[3].UserSkip ? "SKIP" : Steps.Select(x => x.Result4).Contains(Step.Ng) ? "FAIL" : "OK"; }

                                            TestOK = Boards.Select(x => x.Result).Contains("OK");
                                            if (!TestOK)
                                            {
                                                foreach (var item in Boards)
                                                {
                                                    if (!item.UserSkip)
                                                    {
                                                        item.EndTest = DateTime.Now;
                                                    }
                                                }

                                                SYSTEM.System_Board.MachineIO.ADSC = true;
                                                SYSTEM.System_Board.MachineIO.BDSC = true;
                                                SYSTEM.System_Board.MachineIO.MainUP = true;
                                                SYSTEM.System_Board.MachineIO.LPG = true;
                                                SYSTEM.System_Board.SendControl();
                                                TestState = RunTestState.FAIL;
                                                ResultPanel.ShowResult(Boards.ToList());
                                                IsTestting = false;
                                                break;
                                            }
                                            else
                                            {
                                                if (Boards.Count >= 1) if (!Boards[0].Skip) Boards[0].Skip = Boards[0].Result == "FAIL";
                                                if (Boards.Count >= 2) if (!Boards[1].Skip) Boards[1].Skip = Boards[1].Result == "FAIL";
                                                if (Boards.Count >= 3) if (!Boards[2].Skip) Boards[2].Skip = Boards[2].Result == "FAIL";
                                                if (Boards.Count >= 4) if (!Boards[3].Skip) Boards[3].Skip = Boards[3].Result == "FAIL";

                                                Debug.Write("Step test fail -> Skip sites:", Debug.ContentType.Warning);
                                                if (Boards.Count >= 1) if (Boards[0].Skip) Debug.Appent("\t\tSite A", Debug.ContentType.Warning);
                                                if (Boards.Count >= 2) if (Boards[1].Skip) Debug.Appent("\t\tSite B", Debug.ContentType.Warning);
                                                if (Boards.Count >= 3) if (Boards[2].Skip) Debug.Appent("\t\tSite C", Debug.ContentType.Warning);
                                                if (Boards.Count >= 4) if (Boards[3].Skip) Debug.Appent("\t\tSite D", Debug.ContentType.Warning);
                                            }
                                        }
                                    }

                                    if (stepTest.cmd == CMDs.END)
                                    {
                                        bool TestOK = true;
                                        if (Boards.Count >= 1) { Boards[0].Result = Boards[0].UserSkip ? "SKIP" : Steps.Select(x => x.Result1).Contains(Step.Ng) ? "FAIL" : "OK"; }
                                        if (Boards.Count >= 2) { Boards[1].Result = Boards[1].UserSkip ? "SKIP" : Steps.Select(x => x.Result2).Contains(Step.Ng) ? "FAIL" : "OK"; }
                                        if (Boards.Count >= 3) { Boards[2].Result = Boards[2].UserSkip ? "SKIP" : Steps.Select(x => x.Result3).Contains(Step.Ng) ? "FAIL" : "OK"; }
                                        if (Boards.Count >= 4) { Boards[3].Result = Boards[3].UserSkip ? "SKIP" : Steps.Select(x => x.Result4).Contains(Step.Ng) ? "FAIL" : "OK"; }

                                        foreach (var item in Boards)
                                        {
                                            if (!item.UserSkip)
                                            {
                                                item.EndTest = DateTime.Now;
                                            }
                                        }

                                        SYSTEM.System_Board.MachineIO.ADSC = true;
                                        SYSTEM.System_Board.MachineIO.BDSC = true;

                                        SYSTEM.System_Board.SendControl();
                                        TestOK = Boards.Select(x => x.Result).Contains("FAIL");
                                        TestState = TestOK ? RunTestState.FAIL : RunTestState.GOOD;
                                        ResultPanel.ShowResult(Boards.ToList());
                                        IsTestting = false;
                                        break;
                                    }

                                    if (stepTest.cmd == CMDs.UCN)
                                    {
                                        if (Boards.Count >= 1) if (!Boards[0].Skip && CommandDescriptions.CommandRemark_Version.Contains(stepTest.Remark)) Boards[0].BoardDetail += stepTest.Remark + ": " + stepTest.ValueGet1 + " ";
                                        if (Boards.Count >= 2) if (!Boards[1].Skip && CommandDescriptions.CommandRemark_Version.Contains(stepTest.Remark)) Boards[1].BoardDetail += stepTest.Remark + ": " + stepTest.ValueGet2 + " ";
                                        if (Boards.Count >= 3) if (!Boards[2].Skip && CommandDescriptions.CommandRemark_Version.Contains(stepTest.Remark)) Boards[2].BoardDetail += stepTest.Remark + ": " + stepTest.ValueGet3 + " ";
                                        if (Boards.Count >= 4) if (!Boards[3].Skip && CommandDescriptions.CommandRemark_Version.Contains(stepTest.Remark)) Boards[3].BoardDetail += stepTest.Remark + ": " + stepTest.ValueGet4 + " ";
                                    }

                                    await Task.Delay(10); // delay for data binding
                                }
                                StepTesting++;
                            }
                        }
                        break;
                    case RunTestState.MANUALTEST:
                        StepTesting = 0;
                        Steps = TestModel.Steps;
                        foreach (var item in Boards)
                        {
                            item.Skip = item.UserSkip;
                        }
                        //Start Test
                        while (IsTestting)
                        {
                            while (TestState == RunTestState.PAUSE)
                            {
                                await Task.Delay(500);
                            }

                            //Test done without END command
                            if (StepTesting >= Steps.Count)
                            {
                                SYSTEM.System_Board.MachineIO.ADSC = true;
                                SYSTEM.System_Board.MachineIO.BDSC = true;
                                SYSTEM.System_Board.SendControl();
                                await Task.Delay(1000);

                                SYSTEM.System_Board.MachineIO.ADSC = false;
                                SYSTEM.System_Board.MachineIO.BDSC = false;
                                SYSTEM.System_Board.SendControl();
                                IsTestting = false;
                                TestRunFinish?.Invoke(null, null);
                                break;
                            }
                            else
                            {
                                var lastStep = StepTesting;
                                var stepTest = Steps[StepTesting];
                                if (stepTest != null)
                                {
                                    StepTestChange?.Invoke(StepTesting, null);
                                    if (stepTest.cmd != CMDs.NON && !stepTest.Skip)
                                    {
                                        bool IsPass = RUN_FUNCTION_TEST(stepTest);

                                        //Test pass and ejump
                                        if (!IsPass && stepTest.E_Jump != 0)
                                        {
                                            FailReTestStep = stepTest.E_Jump - 1;
                                            int StepResetErr = stepTest.No - 1;
                                            for (int i = 0; i < appSetting.Operations.ErrorJumpCount; i++)
                                            {
                                                for (int stepRetest = FailReTestStep; stepRetest <= StepResetErr; stepRetest++)
                                                {
                                                    while (TestState == RunTestState.PAUSE)
                                                    {
                                                        await Task.Delay(500);
                                                    }

                                                    StepTesting = stepRetest;
                                                    StepTestChange?.Invoke(StepTesting, null);
                                                    stepTest = Steps[stepRetest];
                                                    IsPass = RUN_FUNCTION_TEST(stepTest);
                                                    if (IsPass && stepRetest == StepResetErr)
                                                    {
                                                        StepTesting = lastStep;
                                                        break;
                                                    }
                                                    if (!IsTestting)
                                                    {
                                                        StepTesting = lastStep;
                                                        break;
                                                    }
                                                    await Task.Delay(100);
                                                }
                                                if (!IsTestting)
                                                {
                                                    StepTesting = lastStep;
                                                    break;
                                                }
                                                StepTesting = lastStep;
                                            }
                                        }

                                        //Stop with res test fail
                                        if (appSetting.Operations.FailResistanceStopAll && stepTest.cmd == CMDs.RES && !IsPass)
                                        {
                                            Debug.Write("RES step test fail -> Fail RES stop all enable -> Force stop all", Debug.ContentType.Error);
                                            IsTestting = false;
                                            TestState = RunTestState.STOP;
                                        }

                                        //Stop with stop all when fail 
                                        if (appSetting.Operations.FailStopAll && !IsPass)
                                        {
                                            Debug.Write("Step test fail -> Fail stop all enable -> Force stop all", Debug.ContentType.Error);
                                            IsTestting = false;
                                            TestState = RunTestState.STOP;
                                        }

                                        //Skip fail Step
                                        if (appSetting.Operations.FailStopPCB && !IsPass)
                                        {
                                            if (Boards.Count >= 1) { Boards[0].Result = Boards[0].Skip ? "SKIP" : Steps.Select(x => x.Result1).Contains(Step.Ng) ? "FAIL" : "OK"; }
                                            if (Boards.Count >= 2) { Boards[1].Result = Boards[1].Skip ? "SKIP" : Steps.Select(x => x.Result2).Contains(Step.Ng) ? "FAIL" : "OK"; }
                                            if (Boards.Count >= 3) { Boards[2].Result = Boards[2].Skip ? "SKIP" : Steps.Select(x => x.Result3).Contains(Step.Ng) ? "FAIL" : "OK"; }
                                            if (Boards.Count >= 4) { Boards[3].Result = Boards[3].Skip ? "SKIP" : Steps.Select(x => x.Result4).Contains(Step.Ng) ? "FAIL" : "OK"; }

                                            if (Boards.Count >= 1) if (!Boards[0].Skip) Boards[0].Skip = Boards[0].Result != "OK";
                                            if (Boards.Count >= 2) if (!Boards[1].Skip) Boards[1].Skip = Boards[1].Result != "OK";
                                            if (Boards.Count >= 3) if (!Boards[2].Skip) Boards[2].Skip = Boards[2].Result != "OK";
                                            if (Boards.Count >= 4) if (!Boards[3].Skip) Boards[3].Skip = Boards[3].Result != "OK";
                                            Debug.Write("Step test fail -> Skip sites:", Debug.ContentType.Warning);
                                            if (Boards.Count >= 1) if (Boards[0].Skip) Debug.Appent("\t\tSite A", Debug.ContentType.Warning);
                                            if (Boards.Count >= 2) if (Boards[1].Skip) Debug.Appent("\t\tSite B", Debug.ContentType.Warning);
                                            if (Boards.Count >= 3) if (Boards[2].Skip) Debug.Appent("\t\tSite C", Debug.ContentType.Warning);
                                            if (Boards.Count >= 4) if (Boards[3].Skip) Debug.Appent("\t\tSite D", Debug.ContentType.Warning);
                                        }

                                        if (!IsTestting)
                                        {
                                            StepTesting = lastStep;
                                            break;
                                        }
                                    }

                                    if (stepTest.cmd == CMDs.END)
                                    {
                                        SYSTEM.System_Board.MachineIO.ADSC = true;
                                        SYSTEM.System_Board.MachineIO.BDSC = true;
                                        SYSTEM.System_Board.SendControl();

                                        await Task.Delay(1000);

                                        SYSTEM.System_Board.MachineIO.ADSC = false;
                                        SYSTEM.System_Board.MachineIO.BDSC = false;
                                        SYSTEM.System_Board.SendControl();
                                        IsTestting = false;
                                        TestRunFinish?.Invoke(null, null);
                                        break;
                                    }

                                    await Task.Delay(10); // delay for data binding
                                }
                                StepTesting++;
                            }
                        }
                        break;
                    case RunTestState.PAUSE:
                        await Task.Delay(100);
                        break;
                    case RunTestState.STOP:

                        IsTestting = false;
                        StepTesting = 0;
                        StepTestChange?.Invoke(StepTesting, null);
                        SYSTEM.System_Board.MachineIO.ADSC = true;
                        SYSTEM.System_Board.MachineIO.BDSC = true;
                        SYSTEM.System_Board.MachineIO.LPR = true;
                        SYSTEM.System_Board.SendControl();
                        await Task.Delay(2000);
                        SYSTEM.System_Board.MachineIO.ADSC = false;
                        SYSTEM.System_Board.MachineIO.BDSC = false;
                        SYSTEM.System_Board.MachineIO.MainUP = true;
                        SYSTEM.System_Board.MachineIO.LPR = true;
                        SYSTEM.System_Board.SendControl();
                        await Task.Delay(1000);

                        if (TestModel.BarcodeOption.UseBarcodeInput)
                        {
                            TestState = RunTestState.WAIT;
                        }
                        else
                        {
                            TestState = RunTestState.READY;
                        }
                        foreach (var item in Boards)
                        {
                            item.Barcode = "";
                            item.Result = "";
                            item.Skip = item.UserSkip;
                        }
                        break;
                    case RunTestState.GOOD:
                        TestRunFinish?.Invoke("", null);
                        if (Boards.Count >= 1)
                        {
                            if (!Boards[0].Skip && Boards[0].Result == "OK")
                            {
                                if (TestModel.BarcodeOption.UseBarcodeInput)
                                {
                                    Printer.GT800.SendStringToPrinter(Printer.QRcode.GenerateCode("A", Boards[0].Barcode, Boards[0].StartTest, Boards[0].EndTest,
                                        TestModel.Steps.Select(x => x.IMQSCode).ToList(), TestModel.Steps.Select(x => x.Min).ToList(), TestModel.Steps.Select(x => x.Max).ToList(),
                                        TestModel.Steps.Select(x => x.ValueGet1).ToList(), TestModel.Steps.Where(x => x.Remark == "MAIN VERSION").DefaultIfEmpty(new Step()).FirstOrDefault().ValueGet1,
                                        TestModel.Steps.Where(x => x.Remark == "SUB VERSION").DefaultIfEmpty(new Step()).FirstOrDefault().ValueGet1, out string barcodeOut));
                                    Boards[0].QRout = barcodeOut;
                                    Debug.Appent("\t\tBoard A: GOOD - qr printed:" + Boards[0].QRout, Debug.ContentType.Notify);
                                }
                            }
                        }
                        if (Boards.Count >= 2)
                        {
                            if (!Boards[1].Skip && Boards[1].Result == "OK")
                            {
                                if (TestModel.BarcodeOption.UseBarcodeInput)
                                {
                                    Printer.GT800.SendStringToPrinter(Printer.QRcode.GenerateCode("B", Boards[1].Barcode, Boards[1].StartTest, Boards[1].EndTest,
                                        TestModel.Steps.Select(x => x.IMQSCode).ToList(), TestModel.Steps.Select(x => x.Min).ToList(), TestModel.Steps.Select(x => x.Max).ToList(),
                                        TestModel.Steps.Select(x => x.ValueGet2).ToList(), TestModel.Steps.Where(x => x.Remark == "MAIN VERSION").DefaultIfEmpty(new Step()).FirstOrDefault().ValueGet2,
                                        TestModel.Steps.Where(x => x.Remark == "SUB VERSION").DefaultIfEmpty(new Step()).FirstOrDefault().ValueGet2, out string barcodeOut));
                                    Boards[1].QRout = barcodeOut;
                                    Debug.Appent("\t\tBoard B: GOOD - qr printed:" + Boards[1].QRout, Debug.ContentType.Notify);
                                }
                            }
                        }
                        if (Boards.Count >= 3)
                        {
                            if (!Boards[2].Skip && Boards[2].Result == "OK")
                            {
                                if (TestModel.BarcodeOption.UseBarcodeInput)
                                {
                                    Printer.GT800.SendStringToPrinter(Printer.QRcode.GenerateCode("C", Boards[2].Barcode, Boards[2].StartTest, Boards[2].EndTest,
                                        TestModel.Steps.Select(x => x.IMQSCode).ToList(), TestModel.Steps.Select(x => x.Min).ToList(), TestModel.Steps.Select(x => x.Max).ToList(),
                                        TestModel.Steps.Select(x => x.ValueGet3).ToList(), TestModel.Steps.Where(x => x.Remark == "MAIN VERSION").DefaultIfEmpty(new Step()).FirstOrDefault().ValueGet3,
                                        TestModel.Steps.Where(x => x.Remark == "SUB VERSION").DefaultIfEmpty(new Step()).FirstOrDefault().ValueGet3, out string barcodeOut));
                                    Boards[2].QRout = barcodeOut;
                                    Debug.Appent("\t\tBoard C: GOOD - qr printed:" + Boards[2].QRout, Debug.ContentType.Notify);

                                }
                            }
                        }
                        if (Boards.Count >= 4)
                        {
                            if (!Boards[3].Skip && Boards[3].Result == "OK")
                            {
                                if (TestModel.BarcodeOption.UseBarcodeInput)
                                {
                                    Printer.GT800.SendStringToPrinter(Printer.QRcode.GenerateCode("D", Boards[3].Barcode, Boards[3].StartTest, Boards[3].EndTest,
                                        TestModel.Steps.Select(x => x.IMQSCode).ToList(), TestModel.Steps.Select(x => x.Min).ToList(), TestModel.Steps.Select(x => x.Max).ToList(),
                                        TestModel.Steps.Select(x => x.ValueGet4).ToList(), TestModel.Steps.Where(x => x.Remark == "MAIN VERSION").DefaultIfEmpty(new Step()).FirstOrDefault().ValueGet4,
                                        TestModel.Steps.Where(x => x.Remark == "SUB VERSION").DefaultIfEmpty(new Step()).FirstOrDefault().ValueGet4, out string barcodeOut));
                                    Boards[3].QRout = barcodeOut;
                                    Debug.Appent("\t\tBoard D: GOOD - qr printed:" + Boards[3].QRout, Debug.ContentType.Notify);
                                }
                            }
                        }
                        foreach (var item in Boards)
                        {
                            if (!item.UserSkip)
                            {
                                item.TestStep = TestModel.Steps.ToList();
                                if (appSetting.Operations.SaveFailPCB)
                                {
                                    AppFolder.SaveHistory(item);
                                }
                                else
                                {
                                    if (item.Result == "OK")
                                    {
                                        AppFolder.SaveHistory(item);
                                    }
                                }
                            }
                        }
                        RELAY.Card.Release();
                        Solenoid.Card.Release();
                        MuxCard.Card.ReleaseChannels();
                        SYSTEM.System_Board.MachineIO.ADSC = true;
                        SYSTEM.System_Board.MachineIO.BDSC = true;
                        SYSTEM.System_Board.MachineIO.LPG = true;
                        SYSTEM.System_Board.SendControl();
                        await Task.Delay(1000);
                        SYSTEM.System_Board.MachineIO.ADSC = false;
                        SYSTEM.System_Board.MachineIO.BDSC = false;
                        SYSTEM.System_Board.MachineIO.MainUP = true;

                        SYSTEM.System_Board.SendControl();
                        await Task.Delay(1000);
                        foreach (var item in Boards)
                        {
                            item.Barcode = "";
                            item.Skip = item.UserSkip;
                        }
                        if (TestModel.BarcodeOption.UseBarcodeInput)
                        {
                            TestState = RunTestState.WAIT;
                        }
                        else
                        {
                            TestState = RunTestState.READY;
                        }
                        break;

                    case RunTestState.FAIL:
                        TestRunFinish?.Invoke("", null);
                        if (Printer.QRcode.TestPCBPassPrint)
                        {
                            if (Boards.Count >= 1)
                            {
                                if (!Boards[0].Skip && Boards[0].Result == "OK")
                                {
                                    if (TestModel.BarcodeOption.UseBarcodeInput)
                                    {
                                        Printer.GT800.SendStringToPrinter(Printer.QRcode.GenerateCode("A", Boards[0].Barcode, Boards[0].StartTest, Boards[0].EndTest,
                                            TestModel.Steps.Select(x => x.IMQSCode).ToList(), TestModel.Steps.Select(x => x.Min).ToList(), TestModel.Steps.Select(x => x.Max).ToList(),
                                            TestModel.Steps.Select(x => x.ValueGet1).ToList(), TestModel.Steps.Where(x => x.Remark == "MAIN VERSION").DefaultIfEmpty(new Step()).FirstOrDefault().ValueGet1,
                                            TestModel.Steps.Where(x => x.Remark == "SUB VERSION").DefaultIfEmpty(new Step()).FirstOrDefault().ValueGet1, out string barcodeOut));
                                        Boards[0].QRout = barcodeOut;
                                        Debug.Write("\t\tBoard A: GOOD - qr printed:" + Boards[0].QRout, Debug.ContentType.Notify);
                                    }
                                }
                                else
                                {
                                    Debug.Appent(String.Format("\t\tBoard A: {0}", Boards[0].Result), Debug.ContentType.Error);
                                }

                            }
                            if (Boards.Count >= 2)
                            {
                                if (!Boards[1].Skip && Boards[1].Result == "OK")
                                {
                                    if (TestModel.BarcodeOption.UseBarcodeInput)
                                    {
                                        Printer.GT800.SendStringToPrinter(Printer.QRcode.GenerateCode("B", Boards[1].Barcode, Boards[1].StartTest, Boards[1].EndTest,
                                            TestModel.Steps.Select(x => x.IMQSCode).ToList(), TestModel.Steps.Select(x => x.Min).ToList(), TestModel.Steps.Select(x => x.Max).ToList(),
                                            TestModel.Steps.Select(x => x.ValueGet2).ToList(), TestModel.Steps.Where(x => x.Remark == "MAIN VERSION").DefaultIfEmpty(new Step()).FirstOrDefault().ValueGet2,
                                            TestModel.Steps.Where(x => x.Remark == "SUB VERSION").DefaultIfEmpty(new Step()).FirstOrDefault().ValueGet2, out string barcodeOut));
                                        Boards[1].QRout = barcodeOut;
                                        Debug.Appent("\t\tBoard B: GOOD - qr printed:" + Boards[1].QRout, Debug.ContentType.Notify);
                                    }
                                }
                                else
                                {
                                    Debug.Appent(String.Format("\t\tBoard B: {0}", Boards[1].Result), Debug.ContentType.Error);
                                }
                            }
                            if (Boards.Count >= 3)
                            {
                                if (!Boards[2].Skip && Boards[2].Result == "OK")
                                {
                                    if (TestModel.BarcodeOption.UseBarcodeInput)
                                    {
                                        Printer.GT800.SendStringToPrinter(Printer.QRcode.GenerateCode("C", Boards[2].Barcode, Boards[2].StartTest, Boards[2].EndTest,
                                            TestModel.Steps.Select(x => x.IMQSCode).ToList(), TestModel.Steps.Select(x => x.Min).ToList(), TestModel.Steps.Select(x => x.Max).ToList(),
                                            TestModel.Steps.Select(x => x.ValueGet3).ToList(), TestModel.Steps.Where(x => x.Remark == "MAIN VERSION").DefaultIfEmpty(new Step()).FirstOrDefault().ValueGet3,
                                            TestModel.Steps.Where(x => x.Remark == "SUB VERSION").DefaultIfEmpty(new Step()).FirstOrDefault().ValueGet3, out string barcodeOut));
                                        Boards[2].QRout = barcodeOut;
                                        Debug.Appent("\t\tBoard C: GOOD - qr printed:" + Boards[2].QRout, Debug.ContentType.Notify);

                                    }
                                }
                                else
                                {
                                    Debug.Appent(String.Format("\t\tBoard C: {0}", Boards[2].Result), Debug.ContentType.Error);

                                }
                                if (Boards.Count >= 4)
                                {
                                    if (!Boards[3].Skip && Boards[3].Result == "OK")
                                    {
                                        if (TestModel.BarcodeOption.UseBarcodeInput)
                                        {
                                            Printer.GT800.SendStringToPrinter(Printer.QRcode.GenerateCode("D", Boards[3].Barcode, Boards[3].StartTest, Boards[3].EndTest,
                                                TestModel.Steps.Select(x => x.IMQSCode).ToList(), TestModel.Steps.Select(x => x.Min).ToList(), TestModel.Steps.Select(x => x.Max).ToList(),
                                                TestModel.Steps.Select(x => x.ValueGet4).ToList(), TestModel.Steps.Where(x => x.Remark == "MAIN VERSION").DefaultIfEmpty(new Step()).FirstOrDefault().ValueGet4,
                                                TestModel.Steps.Where(x => x.Remark == "SUB VERSION").DefaultIfEmpty(new Step()).FirstOrDefault().ValueGet4, out string barcodeOut));
                                            Boards[3].QRout = barcodeOut;
                                            Debug.Appent("\t\tBoard D: GOOD - qr printed:" + Boards[3].QRout, Debug.ContentType.Notify);
                                        }
                                    }
                                    else
                                    {
                                        Debug.Appent(String.Format("\t\tBoard D: {0}", Boards[3].Result), Debug.ContentType.Error);

                                    }
                                }
                            }
                            foreach (var item in Boards)
                            {
                                if (!item.UserSkip)
                                {
                                    item.TestStep = TestModel.Steps.ToList();
                                    if (appSetting.Operations.SaveFailPCB)
                                    {
                                        AppFolder.SaveHistory(item);
                                    }
                                    else
                                    {
                                        if (item.Result == "OK")
                                        {
                                            AppFolder.SaveHistory(item);
                                        }
                                    }
                                }
                            }
                        }
                        RELAY.Card.Release();
                        Solenoid.Card.Release();
                        MuxCard.Card.ReleaseChannels();
                        SYSTEM.System_Board.MachineIO.ADSC = true;
                        SYSTEM.System_Board.MachineIO.BDSC = true;
                        SYSTEM.System_Board.MachineIO.LPR = true;
                        SYSTEM.System_Board.MachineIO.BUZZER = true;
                        SYSTEM.System_Board.SendControl();
                        await Task.Delay(1000);
                        SYSTEM.System_Board.MachineIO.ADSC = false;
                        SYSTEM.System_Board.MachineIO.BDSC = false;
                        SYSTEM.System_Board.MachineIO.MainUP = true;
                        SYSTEM.System_Board.SendControl();
                        await Task.Delay(2000);
                        SYSTEM.System_Board.MachineIO.BUZZER = false;
                        SYSTEM.System_Board.SendControl();
                        foreach (var item in Boards)
                        {
                            item.Barcode = "";
                            item.Result = "";
                            item.Skip = item.UserSkip;
                        }
                        if (TestModel.BarcodeOption.UseBarcodeInput)
                        {
                            TestState = RunTestState.WAIT;
                        }
                        else
                        {
                            TestState = RunTestState.READY;
                        }
                        break;

                    case RunTestState.READY:
                        if (IsTestting)
                        {
                            if (TestModel.BarcodeOption.UseBarcodeInput)
                            {
                                bool Realdy = true;
                                if (Boards.Count >= 1) if (!Boards[0].Skip) Realdy &= Boards[0].BarcodeReady;
                                if (Boards.Count >= 2) if (!Boards[1].Skip) Realdy &= Boards[1].BarcodeReady;
                                if (Boards.Count >= 3) if (!Boards[2].Skip) Realdy &= Boards[2].BarcodeReady;
                                if (Boards.Count >= 4) if (!Boards[3].Skip) Realdy &= Boards[3].BarcodeReady;
                                if (Realdy)
                                {
                                    TestState = RunTestState.TESTTING;
                                    SYSTEM.System_Board.MachineIO.BUZZER = false;
                                    SYSTEM.System_Board.MachineIO.LPG = true;
                                    SYSTEM.System_Board.MachineIO.MainDOWN = false;
                                    SYSTEM.System_Board.SendControl();
                                }
                                else
                                {
                                    IsTestting = false;
                                    TestState = RunTestState.WAIT;
                                    Debug.Write("Nothings barcode", Debug.ContentType.Error, 30);
                                }
                            }
                            else
                            {
                                TestState = RunTestState.TESTTING;
                                SYSTEM.System_Board.MachineIO.BUZZER = false;
                                SYSTEM.System_Board.MachineIO.LPG = true;
                                SYSTEM.System_Board.SendControl();
                            }
                        }
                        else if (appSetting.Operations.UseRetryUpdown)
                        {
                            SYSTEM.System_Board.MachineIO.BUZZER = false;
                            SYSTEM.System_Board.MachineIO.MainUP = true;
                            SYSTEM.System_Board.SendControl();
                            await Task.Delay(appSetting.Operations.TestPressUpTime);
                            SYSTEM.System_Board.MachineIO.MainDOWN = true;
                            SYSTEM.System_Board.SendControl();
                            await Task.Delay(appSetting.Operations.TestPressUpTime + 2000);
                        }
                        break;
                    default:
                        break;
                }
                await Task.Delay(500);
            }
        }

        private void EscapTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            EscapTime += 0.1;
        }

        public void RUN_MANUAL_TEST()
        {
            if (!IsTestting)
            {
                TestState = RunTestState.MANUALTEST;
                IsTestting = true;
            }
        }

        public async void Run_Manual_Test()
        {
            if (TestModel.Steps.Count < 2)
            {
                return;
            }
            TestModel.CleanSteps();
            for (int i = 0; i < TestModel.Steps.Count; i++)
            {
                if (TestState == RunTestState.STOP)
                {
                    TestState = RunTestState.DONE;
                    break;
                }
                var stepTest = TestModel.Steps[i];
                if (stepTest != null)
                {
                    StepTesting = i;
                    if (stepTest.cmd != CMDs.NON || !stepTest.Skip)
                    {
                        StepTestChange?.Invoke(i, null);
                        RUN_FUNCTION_TEST(stepTest);
                        await Task.Delay(10); // delay for data binding
                    }
                }
                while (TestState == RunTestState.PAUSE)
                {
                    Task.Delay(100).Wait();
                }
            }
            TestState = RunTestState.DONE;
            TestRunFinish?.Invoke(null, null);
        }

        public void ResetTest()
        {
            TestState = RunTestState.WAIT;
            //SYSTEM.System_Board.MachineIO.ADSC = true;
            //SYSTEM.System_Board.MachineIO.BDSC = true;
            //SYSTEM.System_Board.MachineIO.ADSC = false;
            //SYSTEM.System_Board.MachineIO.BDSC = false;
            //SYSTEM.System_Board.SendControl();
            TestModel.CleanSteps();
            RELAY.Card.Release();
            MuxCard.Card.ReleaseChannels();
            Solenoid.Card.Release();
        }

        public Step currentStep = new Step();

        public async void RunStep()
        {
            if (!FunctionTesting)
            {
                await Task.Run(RunFunctionsTest);
            }
        }

        bool FunctionTesting = false;
        public async void RunFunctionsTest()
        {
            FunctionTesting = true;
            try
            {
                currentStep.ValueGet1 = "";
                currentStep.ValueGet2 = "";
                currentStep.ValueGet3 = "";
                currentStep.ValueGet4 = "";
                RUN_FUNCTION_TEST(currentStep);
            }
            catch (Exception err)
            {
                Debug.Write(string.Format("{0} : {1}", currentStep.TestContent, err.StackTrace), Debug.ContentType.Error);
            }
            await Task.Delay(10);
            FunctionTesting = false;
        }

        public bool RUN_FUNCTION_TEST(Step step)
        {
            step.Result1 = Step.DontCare;
            step.Result2 = Step.DontCare;
            step.Result3 = Step.DontCare;
            step.Result4 = Step.DontCare;

            step.ValueGet1 = "";
            step.ValueGet2 = "";
            step.ValueGet3 = "";
            step.ValueGet4 = "";

            bool isSkipAll = Boards.Where(x => x.Skip).Count() == Boards.Count;
            if (isSkipAll) return false;

            if (!step.Skip)

                switch (step.cmd)
                {
                    case CMDs.NON:
                        break;
                    case CMDs.PWR:
                        PWR(step);
                        break;
                    case CMDs.DLY:
                        DLY(step);
                        break;
                    case CMDs.GEN:
                        GEN(step);
                        break;
                    case CMDs.BUZ:
                        Buzzer(step);
                        break;
                    case CMDs.RLY:
                        RELAY_CONTROL(step);
                        Task.Delay(50).Wait();
                        break;
                    case CMDs.KEY:
                        KEY(step);
                        Task.Delay(50).Wait();
                        break;
                    case CMDs.MAK:
                        break;
                    case CMDs.DIS:
                        DIS(step);
                        break;
                    case CMDs.END:
                        END(step);
                        break;
                    case CMDs.ACV:
                        ACV(step);
                        break;
                    case CMDs.DCV:
                        DCV(step);
                        break;
                    case CMDs.FRQ:
                        FREQ(step);
                        break;
                    case CMDs.RES:
                        RES(step);
                        break;
                    case CMDs.URD:
                        //URD(step, PCB_SKIP_CHECK); update late
                        break;
                    case CMDs.UTN:
                        UTN(step);
                        break;
                    case CMDs.UTX:
                        UTX(step);
                        break;
                    case CMDs.UCN:
                        UCN(step);
                        break;
                    //case CMDs.UCP:
                    //    break;
                    case CMDs.STL:
                        STL(step);
                        break;
                    case CMDs.EDL:
                        EDL(step);
                        break;
                    case CMDs.LCC:
                        LCC(step);
                        break;
                    case CMDs.LEC:
                        LEC(step);
                        break;
                    case CMDs.CAL:
                        break;
                    case CMDs.GLED:
                        ReadGLED(step);
                        break;
                    case CMDs.FND:
                        ReadFND(step);
                        break;
                    case CMDs.LED:
                        ReadLED(step);
                        break;
                    case CMDs.LCD:
                        ReadLCD(step);
                        break;
                    case CMDs.PCB:
                        PCB(step);
                        break;
                    case CMDs.CAM:
                        CAM(step);
                        Task.Delay(1000).Wait();
                        break;
                    case CMDs.MOT:
                        MOT(step);
                        break;
                    default:
                        break;
                }
            return StepTestResult(step);
        }

        #region Functions Code

        public void GEN(Step step)
        {
            if (!Double.TryParse(step.Condition1, out double frequency))
            {
                functionsParameterError("Condition", step);
                return;
            }

            List<string> Channels = step.Oper.Split('/').ToList();
            List<int> ChannelsInt = new List<int>();
            foreach (var Channel in Channels)
            {
                if (!Int32.TryParse(Channel, out int channel))
                {
                    functionsParameterError("Oper", step);
                    return;
                }
                else
                {
                    if (channel == 0 || channel > 4)
                    {
                        functionsParameterError("Oper", step);
                        return;
                    }
                    else
                    {
                        ChannelsInt.Add(channel);
                    }
                }
            }

            if (!SYSTEM.System_Board.SerialPort.Port.IsOpen)
            {
                functionsParameterError("Sys", step);
                return;
            }

            if (!SYSTEM.System_Board.GEN((int)frequency, ChannelsInt))
            {
                functionsParameterError("Sys", step);
            }
            else
            {
                if (Boards.Count >= 1) if (!Boards[0].Skip) step.ValueGet1 = "exe";
                if (Boards.Count >= 2) if (!Boards[1].Skip) step.ValueGet2 = "exe";
                if (Boards.Count >= 3) if (!Boards[2].Skip) step.ValueGet3 = "exe";
                if (Boards.Count >= 4) if (!Boards[3].Skip) step.ValueGet4 = "exe";
            }
        }

        public void Buzzer(Step step)
        {
            if (Boards.Count > 2)
            {
                functionsParameterError("Site number", step);
                return;
            }
            if (!SYSTEM.System_Board.SerialPort.Port.IsOpen)
            {
                functionsParameterError("Sys", step);
                return;
            }

            double minValue;
            if (!Double.TryParse(step.Min, out minValue))
            {
                functionsParameterError("Min", step);
                return;
            }

            double maxValue;
            if (!Double.TryParse(step.Max, out maxValue))
            {
                if (step.Max == "L")
                {
                    maxValue = Double.MaxValue;
                }
                else
                {
                    functionsParameterError("Max", step);
                    return;
                }
            }

            SYSTEM.System_Board.GetInput();
            Task.Delay(100).Wait();
            if (Boards.Count >= 1)
            {
                if (!Boards[0].Skip)
                {
                    int Mic_Level = SYSTEM.System_Board.MachineIO.MIC_A;
                    step.ValueGet1 = Mic_Level.ToString();
                    if (Mic_Level <= maxValue & Mic_Level >= minValue)
                    {
                        step.Result1 = Step.Ok;
                    }
                    else
                    {
                        step.Result1 = Step.Ng;
                    }
                }
            }
            if (Boards.Count >= 2)
            {
                if (!Boards[1].Skip)
                {
                    int Mic_Level = SYSTEM.System_Board.MachineIO.MIC_B;
                    step.ValueGet2 = Mic_Level.ToString();
                    if (Mic_Level <= maxValue & Mic_Level >= minValue)
                    {
                        step.Result2 = Step.Ok;
                    }
                    else
                    {
                        step.Result2 = Step.Ng;
                    }
                }
            }
        }

        public void CAM(Step step)
        {
            if (Capture == null)
            {
                functionsParameterError("no cam", step);
                return;
            }

            Camera.CameraControl.VideoProperties properties;

            if (Enum.TryParse<Camera.CameraControl.VideoProperties>(step.Condition1, out properties))
            {
                if (properties == Camera.CameraControl.VideoProperties.Reset)
                {
                    Capture?.SetParammeter(TestModel.CameraSetting);
                    return;
                }

                int value = 0;

                if (Int32.TryParse(step.Oper, out value))
                {
                    Capture?.SetParammeter(properties, value, true);
                }
                else
                {
                    functionsParameterError("Oper", step);
                }
            }
            else
            {
                functionsParameterError("condition", step);
                return;
            }

        }

        public void UTN(Step step)
        {
            TxData txData = new TxData();
            txData = TestModel.Naming.TxDatas.Where(x => x.Name == step.Condition1).DefaultIfEmpty(null).FirstOrDefault();
            if (txData == null)
            {
                functionsParameterError("Naming", step);
                return;
            }
            foreach (var item in UUTs)
            {
                if (step.Oper == "P1" && item.Config != TestModel.P1_Config)
                {
                    item.Config = TestModel.P1_Config;
                }
                else if (step.Oper == "P2")
                {
                    item.Config = TestModel.P2_Config;
                }
            }

            var startTime = DateTime.Now;
            Int32 delay = 10;
            Int32 limittime = 10;
            int tryCount = 1;
            Int32.TryParse(step.Count, out delay);
            Int32.TryParse(step.Condition2, out limittime);
            Int32.TryParse(step.Min, out tryCount);

            switch (step.Mode)
            {
                case "NORMAL":
                    UTN_NORMAL(step, txData);
                    break;
                case "SEND-R":
                    var listTask1 = new List<Task<bool>>();

                    if (Boards.Count >= 1) if (!Boards[0].Skip) listTask1.Add(UTN_SEND_R(step, UUTs[0], 1, txData, delay, limittime, tryCount));
                    if (Boards.Count >= 2) if (!Boards[1].Skip) listTask1.Add(UTN_SEND_R(step, UUTs[1], 2, txData, delay, limittime, tryCount));
                    if (Boards.Count >= 3) if (!Boards[2].Skip) listTask1.Add(UTN_SEND_R(step, UUTs[2], 3, txData, delay, limittime, tryCount));
                    if (Boards.Count >= 4) if (!Boards[3].Skip) listTask1.Add(UTN_SEND_R(step, UUTs[3], 4, txData, delay, limittime, tryCount));

                    try
                    {
                        // Wait for all the tasks to finish.
                        Task.WaitAll(listTask1.ToArray());

                        // We should never get to this point
                        Console.WriteLine("WaitAll() has not thrown exceptions. THIS WAS NOT EXPECTED.");
                    }
                    catch (AggregateException e)
                    {
                        Console.WriteLine("\nThe following exceptions have been thrown by WaitAll(): (THIS WAS EXPECTED)");
                        for (int j = 0; j < e.InnerExceptions.Count; j++)
                        {
                            Console.WriteLine("\n-------------------------------------------------\n{0}", e.InnerExceptions[j].ToString());
                        }
                    }
                    break;
                case "SEND_R":
                    var listTask2 = new List<Task<bool>>();

                    if (Boards.Count >= 1) if (!Boards[0].Skip) listTask2.Add(UTN_SEND_R(step, UUTs[0], 1, txData, delay, limittime, tryCount));
                    if (Boards.Count >= 2) if (!Boards[1].Skip) listTask2.Add(UTN_SEND_R(step, UUTs[1], 2, txData, delay, limittime, tryCount));
                    if (Boards.Count >= 3) if (!Boards[2].Skip) listTask2.Add(UTN_SEND_R(step, UUTs[2], 3, txData, delay, limittime, tryCount));
                    if (Boards.Count >= 4) if (!Boards[3].Skip) listTask2.Add(UTN_SEND_R(step, UUTs[3], 4, txData, delay, limittime, tryCount));

                    try
                    {
                        // Wait for all the tasks to finish.
                        Task.WaitAll(listTask2.ToArray());

                        // We should never get to this point
                        Console.WriteLine("WaitAll() has not thrown exceptions. THIS WAS NOT EXPECTED.");
                    }
                    catch (AggregateException e)
                    {
                        Console.WriteLine("\nThe following exceptions have been thrown by WaitAll(): (THIS WAS EXPECTED)");
                        for (int j = 0; j < e.InnerExceptions.Count; j++)
                        {
                            Console.WriteLine("\n-------------------------------------------------\n{0}", e.InnerExceptions[j].ToString());
                        }
                    }
                    break;
                case "TIMER":
                    UTN_SendTimer(step, txData);
                    break;
                default:
                    break;
            }
        }

        public void UTX(Step step)
        {
            string txData = step.Condition1;
            if (txData == null || txData.Length < 2)
            {
                functionsParameterError("Condition", step);
                return;
            }
            foreach (var item in UUTs)
            {
                if (step.Oper == "P1" && item.Config != TestModel.P1_Config)
                {
                    item.Config = TestModel.P1_Config;
                }
                else if (step.Oper == "P2")
                {
                    item.Config = TestModel.P2_Config;
                }
            }

            var startTime = DateTime.Now;
            Int32 delay = 10;
            Int32 limittime = 10;
            int tryCount = 1;
            Int32.TryParse(step.Count, out delay);
            Int32.TryParse(step.Condition2, out limittime);
            Int32.TryParse(step.Min, out tryCount);

            switch (step.Mode)
            {
                case "NORMAL":
                    UTN_NORMAL(step, txData);
                    break;
                case "SEND-R":
                    var listTask1 = new List<Task<bool>>();

                    if (Boards.Count >= 1) if (!Boards[0].Skip) listTask1.Add(UTN_SEND_R(step, UUTs[0], 1, txData, delay, limittime, tryCount));
                    if (Boards.Count >= 2) if (!Boards[1].Skip) listTask1.Add(UTN_SEND_R(step, UUTs[1], 2, txData, delay, limittime, tryCount));
                    if (Boards.Count >= 3) if (!Boards[2].Skip) listTask1.Add(UTN_SEND_R(step, UUTs[2], 3, txData, delay, limittime, tryCount));
                    if (Boards.Count >= 4) if (!Boards[3].Skip) listTask1.Add(UTN_SEND_R(step, UUTs[3], 4, txData, delay, limittime, tryCount));

                    try
                    {
                        // Wait for all the tasks to finish.
                        Task.WaitAll(listTask1.ToArray());

                        // We should never get to this point
                        Console.WriteLine("WaitAll() has not thrown exceptions. THIS WAS NOT EXPECTED.");
                    }
                    catch (AggregateException e)
                    {
                        Console.WriteLine("\nThe following exceptions have been thrown by WaitAll(): (THIS WAS EXPECTED)");
                        for (int j = 0; j < e.InnerExceptions.Count; j++)
                        {
                            Console.WriteLine("\n-------------------------------------------------\n{0}", e.InnerExceptions[j].ToString());
                        }
                    }
                    break;
                case "SEND_R":
                    var listTask2 = new List<Task<bool>>();

                    if (Boards.Count >= 1) if (!Boards[0].Skip) listTask2.Add(UTN_SEND_R(step, UUTs[0], 1, txData, delay, limittime, tryCount));
                    if (Boards.Count >= 2) if (!Boards[1].Skip) listTask2.Add(UTN_SEND_R(step, UUTs[1], 2, txData, delay, limittime, tryCount));
                    if (Boards.Count >= 3) if (!Boards[2].Skip) listTask2.Add(UTN_SEND_R(step, UUTs[2], 3, txData, delay, limittime, tryCount));
                    if (Boards.Count >= 4) if (!Boards[3].Skip) listTask2.Add(UTN_SEND_R(step, UUTs[3], 4, txData, delay, limittime, tryCount));

                    try
                    {
                        // Wait for all the tasks to finish.
                        Task.WaitAll(listTask2.ToArray());

                        // We should never get to this point
                        Console.WriteLine("WaitAll() has not thrown exceptions. THIS WAS NOT EXPECTED.");
                    }
                    catch (AggregateException e)
                    {
                        Console.WriteLine("\nThe following exceptions have been thrown by WaitAll(): (THIS WAS EXPECTED)");
                        for (int j = 0; j < e.InnerExceptions.Count; j++)
                        {
                            Console.WriteLine("\n-------------------------------------------------\n{0}", e.InnerExceptions[j].ToString());
                        }
                    }
                    break;
                case "TIMER":
                    UTN_SendTimer(step, txData);
                    break;
                default:
                    break;
            }
        }

        private void UTN_NORMAL(Step step, TxData txData)
        {
            if (Boards.Count >= 1) if (!Boards[0].Skip) step.Result1 = UUTs[0].Send(txData) ? Step.Ok : Step.Ng;
            if (Boards.Count >= 2) if (!Boards[1].Skip) step.Result2 = UUTs[1].Send(txData) ? Step.Ok : Step.Ng;
            if (Boards.Count >= 3) if (!Boards[2].Skip) step.Result3 = UUTs[2].Send(txData) ? Step.Ok : Step.Ng;
            if (Boards.Count >= 4) if (!Boards[3].Skip) step.Result4 = UUTs[3].Send(txData) ? Step.Ok : Step.Ng;

            if (Boards.Count >= 1) if (!Boards[0].Skip) step.ValueGet1 = step.Result1 == Step.Ok ? Step.Ok : "Tx";
            if (Boards.Count >= 2) if (!Boards[1].Skip) step.ValueGet2 = step.Result2 == Step.Ok ? Step.Ok : "Tx";
            if (Boards.Count >= 3) if (!Boards[2].Skip) step.ValueGet3 = step.Result3 == Step.Ok ? Step.Ok : "Tx";
            if (Boards.Count >= 4) if (!Boards[3].Skip) step.ValueGet4 = step.Result4 == Step.Ok ? Step.Ok : "Tx";
        }

        private void UTN_NORMAL(Step step, string txData)
        {
            if (Boards.Count >= 1) if (!Boards[0].Skip) step.Result1 = UUTs[0].Send(txData) ? Step.Ok : Step.Ng;
            if (Boards.Count >= 2) if (!Boards[1].Skip) step.Result2 = UUTs[1].Send(txData) ? Step.Ok : Step.Ng;
            if (Boards.Count >= 3) if (!Boards[2].Skip) step.Result3 = UUTs[2].Send(txData) ? Step.Ok : Step.Ng;
            if (Boards.Count >= 4) if (!Boards[3].Skip) step.Result4 = UUTs[3].Send(txData) ? Step.Ok : Step.Ng;

            if (Boards.Count >= 1) if (!Boards[0].Skip) step.ValueGet1 = step.Result1 == Step.Ok ? Step.Ok : "Tx";
            if (Boards.Count >= 2) if (!Boards[1].Skip) step.ValueGet2 = step.Result2 == Step.Ok ? Step.Ok : "Tx";
            if (Boards.Count >= 3) if (!Boards[2].Skip) step.ValueGet3 = step.Result3 == Step.Ok ? Step.Ok : "Tx";
            if (Boards.Count >= 4) if (!Boards[3].Skip) step.ValueGet4 = step.Result4 == Step.Ok ? Step.Ok : "Tx";
        }

        private void UTN_SendTimer(Step step, TxData txData)
        {
            if (int.TryParse(step.Count, out int time))
            {
                if (Boards.Count >= 1) if (!Boards[0].Skip) step.ValueGet1 = UUTs[0].SendTimer(txData, time) ? Step.Ok : "Sys";
                if (Boards.Count >= 2) if (!Boards[1].Skip) step.ValueGet2 = UUTs[1].SendTimer(txData, time) ? Step.Ok : "Sys";
                if (Boards.Count >= 3) if (!Boards[2].Skip) step.ValueGet3 = UUTs[2].SendTimer(txData, time) ? Step.Ok : "Sys";
                if (Boards.Count >= 4) if (!Boards[3].Skip) step.ValueGet4 = UUTs[3].SendTimer(txData, time) ? Step.Ok : "Sys";
            }
            else
            {
                if (Boards.Count >= 1) if (!Boards[0].Skip) step.ValueGet1 = "Set time";
                if (Boards.Count >= 2) if (!Boards[1].Skip) step.ValueGet2 = "Set time";
                if (Boards.Count >= 3) if (!Boards[2].Skip) step.ValueGet3 = "Set time";
                if (Boards.Count >= 4) if (!Boards[3].Skip) step.ValueGet4 = "Set time";
            }
        }

        private void UTN_SendTimer(Step step, string txData)
        {
            if (int.TryParse(step.Count, out int time))
            {
                if (Boards.Count >= 1) if (!Boards[0].Skip) step.ValueGet1 = UUTs[0].SendTimer(txData, time) ? Step.Ok : "Sys";
                if (Boards.Count >= 2) if (!Boards[1].Skip) step.ValueGet2 = UUTs[1].SendTimer(txData, time) ? Step.Ok : "Sys";
                if (Boards.Count >= 3) if (!Boards[2].Skip) step.ValueGet3 = UUTs[2].SendTimer(txData, time) ? Step.Ok : "Sys";
                if (Boards.Count >= 4) if (!Boards[3].Skip) step.ValueGet4 = UUTs[3].SendTimer(txData, time) ? Step.Ok : "Sys";
            }
            else
            {
                if (Boards.Count >= 1) if (!Boards[0].Skip) step.ValueGet1 = "Set time";
                if (Boards.Count >= 2) if (!Boards[1].Skip) step.ValueGet2 = "Set time";
                if (Boards.Count >= 3) if (!Boards[2].Skip) step.ValueGet3 = "Set time";
                if (Boards.Count >= 4) if (!Boards[3].Skip) step.ValueGet4 = "Set time";
            }
        }

        private async Task<bool> UTN_SEND_R(Step step, UUTPort UUT, int boardIndex, TxData txData, int DelayTime, int limitTime, int tryCount)
        {
            var start = DateTime.Now;
            for (int i = 0; i < tryCount; i++)
            {
                if (DateTime.Now.Subtract(start).TotalMilliseconds > limitTime)
                {
                    if (UUT.HaveBuffer())
                    {
                        SetValue(step, boardIndex, "OK");
                        return true;
                    }
                    else
                    {
                        SetValue(step, boardIndex, "Timeout", true);
                        return false;
                    }
                }
                else
                {
                    var sendOK = UUT.Send(txData);
                    if (!sendOK)
                    {
                        SetValue(step, boardIndex, "Tx", true);
                    }
                    await Task.Delay(DelayTime);
                    if (UUT.HaveBuffer())
                    {
                        SetValue(step, boardIndex, "OK");
                        return true;
                    }
                    else
                    {
                        SetValue(step, boardIndex, "Rx", true);
                    }
                }
            }
            return false;
        }

        private async Task<bool> UTN_SEND_R(Step step, UUTPort UUT, int boardIndex, string txData, int DelayTime, int limitTime, int tryCount)
        {
            var start = DateTime.Now;
            while (true)
            {
                var sendOK = UUT.Send(txData);
                if (sendOK)
                {
                    SetValue(step, boardIndex, "OK");
                }
                else
                {
                    SetValue(step, boardIndex, "Tx", true);
                }

                await Task.Delay(DelayTime);

                if (UUT.HaveBuffer())
                {
                    SetValue(step, boardIndex, "OK");
                    return true;
                }
                else
                {
                    SetValue(step, boardIndex, "Rx", true);
                    if (DateTime.Now.Subtract(start).TotalMilliseconds < limitTime)
                        return false;
                }
            }
        }

        private void SetValue(Step step, int Index, string value, bool IsFail = false)
        {
            switch (Index)
            {
                case 1:
                    if (Boards.Count > 0)
                    {
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = value;
                            if (IsFail)
                            {
                                step.Result1 = Step.Ng;
                            }
                            else
                            {
                                step.Result1 = Step.DontCare;
                            }
                        }
                    }
                    break;
                case 2:
                    if (Boards.Count > 1)
                    {
                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = value;
                            if (IsFail)
                            {
                                step.Result2 = Step.Ng;
                            }
                            else
                            {
                                step.Result2 = Step.DontCare;
                            }
                        }
                    }
                    break;
                case 3:
                    if (Boards.Count > 2)
                    {
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = value;
                            if (IsFail)
                            {
                                step.Result3 = Step.Ng;
                            }
                            else
                            {
                                step.Result3 = Step.DontCare;
                            }
                        }
                    }
                    break;
                case 4:
                    if (Boards.Count > 3)
                    {
                        if (!Boards[3].Skip)
                        {
                            step.ValueGet4 = value;
                            if (IsFail)
                            {
                                step.Result4 = Step.Ng;
                            }
                            else
                            {
                                step.Result4 = Step.DontCare;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public void UCN(Step step)
        {
            RxData rxData = new RxData();
            rxData = TestModel.Naming.RxDatas.Where(x => x.Name == step.Condition1).DefaultIfEmpty(null).FirstOrDefault();

            if (rxData != null)
            {
                if (Boards.Count >= 1) if (!Boards[0].Skip) step.ValueGet1 = UUTs[0].CheckBufferString(rxData);
                if (Boards.Count >= 2) if (!Boards[1].Skip) step.ValueGet2 = UUTs[1].CheckBufferString(rxData);
                if (Boards.Count >= 3) if (!Boards[2].Skip) step.ValueGet3 = UUTs[2].CheckBufferString(rxData);
                if (Boards.Count >= 4) if (!Boards[3].Skip) step.ValueGet4 = UUTs[3].CheckBufferString(rxData);

                if (Boards.Count >= 1) if (!Boards[0].Skip) step.Result1 = step.ValueGet1 == step.Spect ? Step.Ok : Step.Ng;
                if (Boards.Count >= 2) if (!Boards[1].Skip) step.Result2 = step.ValueGet2 == step.Spect ? Step.Ok : Step.Ng;
                if (Boards.Count >= 3) if (!Boards[2].Skip) step.Result3 = step.ValueGet3 == step.Spect ? Step.Ok : Step.Ng;
                if (Boards.Count >= 4) if (!Boards[3].Skip) step.Result4 = step.ValueGet4 == step.Spect ? Step.Ok : Step.Ng;
            }
            else
            {
                if (Boards.Count >= 1) if (!Boards[0].Skip) step.Result1 = Step.Ng;
                if (Boards.Count >= 2) if (!Boards[1].Skip) step.Result2 = Step.Ng;
                if (Boards.Count >= 3) if (!Boards[2].Skip) step.Result3 = Step.Ng;
                if (Boards.Count >= 4) if (!Boards[3].Skip) step.Result4 = Step.Ng;
            }
        }


        public void ReadLCD(Step step)
        {
            step.Result = true;
            if (int.TryParse(step.Condition2, out int scanTime))
            {
                DateTime start = DateTime.Now;
                while (DateTime.Now.Subtract(start).TotalMilliseconds < scanTime)
                {
                    foreach (var LCDitem in VisionTester.Models.LCDs)
                    {
                        LCDitem.TestImage(Capture.LastMatFrame, step.Oper);
                    }

                    step.Result = true;

                    if (Boards.Count >= 1) if (!Boards[0].Skip) step.ValueGet1 = step.Result1 != Step.Ok ?
                                    (VisionTester.Models.LCDs[0].DetectedString.Replace("B", "8") == step.Oper.Replace("B", "8") ? step.Oper :
                                    VisionTester.Models.LCDs[0].DetectedString) : step.ValueGet1;

                    if (Boards.Count >= 2) if (!Boards[1].Skip) step.ValueGet2 = step.Result2 != Step.Ok ?
                                    (VisionTester.Models.LCDs[0].DetectedString.Replace("B", "8") == step.Oper.Replace("B", "8") ? step.Oper :
                                    VisionTester.Models.LCDs[1].DetectedString): step.ValueGet2;

                    if (Boards.Count >= 3) if (!Boards[2].Skip) step.ValueGet3 = step.Result3 != Step.Ok ?
                                    (VisionTester.Models.LCDs[0].DetectedString.Replace("B", "8") == step.Oper.Replace("B", "8") ? step.Oper :
                                    VisionTester.Models.LCDs[2].DetectedString): step.ValueGet3;

                    if (Boards.Count >= 4) if (!Boards[3].Skip) step.ValueGet4 = step.Result4 != Step.Ok ?
                                    (VisionTester.Models.LCDs[0].DetectedString.Replace("B", "8") == step.Oper.Replace("B", "8") ? step.Oper :
                                    VisionTester.Models.LCDs[3].DetectedString): step.ValueGet4;

                    if (Boards.Count >= 1) if (!Boards[0].Skip) step.Result1 = step.ValueGet1 == step.Oper ? Step.Ok : Step.Ng;
                    if (Boards.Count >= 2) if (!Boards[1].Skip) step.Result2 = step.ValueGet2 == step.Oper ? Step.Ok : Step.Ng;
                    if (Boards.Count >= 3) if (!Boards[2].Skip) step.Result3 = step.ValueGet3 == step.Oper ? Step.Ok : Step.Ng;
                    if (Boards.Count >= 4) if (!Boards[3].Skip) step.Result4 = step.ValueGet4 == step.Oper ? Step.Ok : Step.Ng;

                    if (Boards.Count >= 1) if (!Boards[0].Skip) step.Result &= (step.Result1 == Step.Ok);
                    if (Boards.Count >= 2) if (!Boards[1].Skip) step.Result &= (step.Result2 == Step.Ok);
                    if (Boards.Count >= 3) if (!Boards[2].Skip) step.Result &= (step.Result3 == Step.Ok);
                    if (Boards.Count >= 4) if (!Boards[3].Skip) step.Result &= (step.Result4 == Step.Ok);

                    if (step.Result)
                        break;
                    Task.Delay(300).Wait();
                }
            }
            else
            {
                if (Boards.Count >= 1) if (!Boards[0].Skip) step.ValueGet1 = VisionTester.Models.LCDs[0].DetectedString;
                if (Boards.Count >= 2) if (!Boards[1].Skip) step.ValueGet2 = VisionTester.Models.LCDs[1].DetectedString;
                if (Boards.Count >= 3) if (!Boards[2].Skip) step.ValueGet3 = VisionTester.Models.LCDs[2].DetectedString;
                if (Boards.Count >= 4) if (!Boards[3].Skip) step.ValueGet4 = VisionTester.Models.LCDs[3].DetectedString;

                if (Boards.Count >= 1) if (!Boards[0].Skip) step.Result1 = step.ValueGet1 == step.Oper ? Step.Ok : Step.Ng;
                if (Boards.Count >= 2) if (!Boards[1].Skip) step.Result2 = step.ValueGet2 == step.Oper ? Step.Ok : Step.Ng;
                if (Boards.Count >= 3) if (!Boards[2].Skip) step.Result3 = step.ValueGet3 == step.Oper ? Step.Ok : Step.Ng;
                if (Boards.Count >= 4) if (!Boards[3].Skip) step.Result4 = step.ValueGet4 == step.Oper ? Step.Ok : Step.Ng;

                if (Boards.Count >= 1) if (!Boards[0].Skip) step.Result &= (step.Result1 == Step.Ok);
                if (Boards.Count >= 2) if (!Boards[1].Skip) step.Result &= (step.Result2 == Step.Ok);
                if (Boards.Count >= 3) if (!Boards[2].Skip) step.Result &= (step.Result3 == Step.Ok);
                if (Boards.Count >= 4) if (!Boards[3].Skip) step.Result &= (step.Result4 == Step.Ok);
            }

        }

        public void ReadFND(Step step)
        {
            step.Result = true;
            if (int.TryParse(step.Condition2, out int scanTime))
            {
                DateTime start = DateTime.Now;
                while (DateTime.Now.Subtract(start).TotalMilliseconds < scanTime)
                {
                    step.Result = true;

                    if (Boards.Count >= 1) if (!Boards[0].Skip) step.ValueGet1 = step.Result1 != Step.Ok ? VisionTester.Models.FNDs[0].DetectedString : step.ValueGet1;
                    if (Boards.Count >= 2) if (!Boards[1].Skip) step.ValueGet2 = step.Result2 != Step.Ok ? VisionTester.Models.FNDs[1].DetectedString : step.ValueGet2;
                    if (Boards.Count >= 3) if (!Boards[2].Skip) step.ValueGet3 = step.Result3 != Step.Ok ? VisionTester.Models.FNDs[2].DetectedString : step.ValueGet3;
                    if (Boards.Count >= 4) if (!Boards[3].Skip) step.ValueGet4 = step.Result4 != Step.Ok ? VisionTester.Models.FNDs[3].DetectedString : step.ValueGet4;

                    if (Boards.Count >= 1) if (!Boards[0].Skip) step.Result1 = step.ValueGet1 == step.Oper ? Step.Ok : Step.Ng;
                    if (Boards.Count >= 2) if (!Boards[1].Skip) step.Result2 = step.ValueGet2 == step.Oper ? Step.Ok : Step.Ng;
                    if (Boards.Count >= 3) if (!Boards[2].Skip) step.Result3 = step.ValueGet3 == step.Oper ? Step.Ok : Step.Ng;
                    if (Boards.Count >= 4) if (!Boards[3].Skip) step.Result4 = step.ValueGet4 == step.Oper ? Step.Ok : Step.Ng;

                    if (Boards.Count >= 1) if (!Boards[0].Skip) step.Result &= (step.Result1 == Step.Ok);
                    if (Boards.Count >= 2) if (!Boards[1].Skip) step.Result &= (step.Result2 == Step.Ok);
                    if (Boards.Count >= 3) if (!Boards[2].Skip) step.Result &= (step.Result3 == Step.Ok);
                    if (Boards.Count >= 4) if (!Boards[3].Skip) step.Result &= (step.Result4 == Step.Ok);
                    if (step.Result)
                        break;
                    Task.Delay(200).Wait();
                }
            }
            else
            {
                if (Boards.Count >= 1) if (!Boards[0].Skip) step.ValueGet1 = VisionTester.Models.FNDs[0].DetectedString;
                if (Boards.Count >= 2) if (!Boards[1].Skip) step.ValueGet2 = VisionTester.Models.FNDs[1].DetectedString;
                if (Boards.Count >= 3) if (!Boards[2].Skip) step.ValueGet3 = VisionTester.Models.FNDs[2].DetectedString;
                if (Boards.Count >= 4) if (!Boards[3].Skip) step.ValueGet4 = VisionTester.Models.FNDs[3].DetectedString;

                if (Boards.Count >= 1) if (!Boards[0].Skip) step.Result1 = step.ValueGet1 == step.Oper ? Step.Ok : Step.Ng;
                if (Boards.Count >= 2) if (!Boards[1].Skip) step.Result2 = step.ValueGet2 == step.Oper ? Step.Ok : Step.Ng;
                if (Boards.Count >= 3) if (!Boards[2].Skip) step.Result3 = step.ValueGet3 == step.Oper ? Step.Ok : Step.Ng;
                if (Boards.Count >= 4) if (!Boards[3].Skip) step.Result4 = step.ValueGet4 == step.Oper ? Step.Ok : Step.Ng;

                if (Boards.Count >= 1) if (!Boards[0].Skip) step.Result &= (step.Result1 == Step.Ok);
                if (Boards.Count >= 2) if (!Boards[1].Skip) step.Result &= (step.Result2 == Step.Ok);
                if (Boards.Count >= 3) if (!Boards[2].Skip) step.Result &= (step.Result3 == Step.Ok);
                if (Boards.Count >= 4) if (!Boards[3].Skip) step.Result &= (step.Result4 == Step.Ok);
            }

        }

        public void ReadLED(Step step)
        {
            step.Result = true;
            if (Boards.Count >= 1) if (!Boards[0].Skip) step.ValueGet1 = VisionTester.Models.LED[0].CalculatorOutputString;
            if (Boards.Count >= 2) if (!Boards[1].Skip) step.ValueGet2 = VisionTester.Models.LED[1].CalculatorOutputString;
            if (Boards.Count >= 3) if (!Boards[2].Skip) step.ValueGet3 = VisionTester.Models.LED[2].CalculatorOutputString;
            if (Boards.Count >= 4) if (!Boards[3].Skip) step.ValueGet4 = VisionTester.Models.LED[3].CalculatorOutputString;

            if (Boards.Count >= 1) if (!Boards[0].Skip) step.Result1 = step.ValueGet1 == step.Oper ? Step.Ok : Step.Ng;
            if (Boards.Count >= 2) if (!Boards[1].Skip) step.Result2 = step.ValueGet2 == step.Oper ? Step.Ok : Step.Ng;
            if (Boards.Count >= 3) if (!Boards[2].Skip) step.Result3 = step.ValueGet3 == step.Oper ? Step.Ok : Step.Ng;
            if (Boards.Count >= 4) if (!Boards[3].Skip) step.Result4 = step.ValueGet4 == step.Oper ? Step.Ok : Step.Ng;

            if (Boards.Count >= 1) if (!Boards[0].Skip) step.Result &= (step.Result1 == Step.Ok);
            if (Boards.Count >= 2) if (!Boards[1].Skip) step.Result &= (step.Result2 == Step.Ok);
            if (Boards.Count >= 3) if (!Boards[2].Skip) step.Result &= (step.Result3 == Step.Ok);
            if (Boards.Count >= 4) if (!Boards[3].Skip) step.Result &= (step.Result4 == Step.Ok);
        }

        public void ReadGLED(Step step)
        {
            step.Result = true;

            if (Boards.Count >= 1) if (!Boards[0].Skip) step.ValueGet1 = VisionTester.Models.GLED[0].CalculatorOutputString;
            if (Boards.Count >= 2) if (!Boards[1].Skip) step.ValueGet2 = VisionTester.Models.GLED[1].CalculatorOutputString;
            if (Boards.Count >= 3) if (!Boards[2].Skip) step.ValueGet3 = VisionTester.Models.GLED[2].CalculatorOutputString;
            if (Boards.Count >= 4) if (!Boards[3].Skip) step.ValueGet4 = VisionTester.Models.GLED[3].CalculatorOutputString;

            if (Boards.Count >= 1) if (!Boards[0].Skip) step.Result1 = step.ValueGet1 == step.Oper ? Step.Ok : Step.Ng;
            if (Boards.Count >= 2) if (!Boards[1].Skip) step.Result2 = step.ValueGet2 == step.Oper ? Step.Ok : Step.Ng;
            if (Boards.Count >= 3) if (!Boards[2].Skip) step.Result3 = step.ValueGet3 == step.Oper ? Step.Ok : Step.Ng;
            if (Boards.Count >= 4) if (!Boards[3].Skip) step.Result4 = step.ValueGet4 == step.Oper ? Step.Ok : Step.Ng;

            if (Boards.Count >= 1) if (!Boards[0].Skip) step.Result &= (step.Result1 == Step.Ok);
            if (Boards.Count >= 2) if (!Boards[1].Skip) step.Result &= (step.Result2 == Step.Ok);
            if (Boards.Count >= 3) if (!Boards[2].Skip) step.Result &= (step.Result3 == Step.Ok);
            if (Boards.Count >= 4) if (!Boards[3].Skip) step.Result &= (step.Result4 == Step.Ok);

        }


        public void RELAY_CONTROL(Step step)
        {
            List<String> Channel = new List<string>();
            if (step.Condition1.Contains("/"))
            {
                Channel = step.Condition1.Split('/').ToList();
            }
            else
            {
                Channel.Add(step.Condition1);
            }
            List<int> numberChannel = new List<int>();
            foreach (var item in Channel)
            {
                if (item.Contains('~'))
                {
                    int startChannel = Convert.ToInt32(item.Split('~')[0]);
                    int endChannel = Convert.ToInt32(item.Split('~')[1]);
                    for (int i = startChannel; i < endChannel; i++)
                    {
                        numberChannel.Add(i - 1);
                    }
                }
                else
                {
                    numberChannel.Add(Convert.ToInt32(item) - 1);
                }
            }

            bool SetOK = RELAY.SetChannels(numberChannel, step.Oper == "ON");
            switch (Boards.Count)
            {
                case 1:
                    if (!Boards[0].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet1 = "Sys";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "exe";
                            step.Result1 = Step.Ok;
                        }
                    }
                    break;
                case 2:
                    if (!Boards[0].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet1 = "Sys";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "exe";
                            step.Result1 = Step.Ok;
                        }
                    }

                    if (!Boards[1].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet2 = "Sys";
                            step.Result2 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet2 = "exe";
                            step.Result2 = Step.Ok;
                        }
                    }
                    break;
                case 3:
                    if (!Boards[0].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet1 = "Sys";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "exe";
                            step.Result1 = Step.Ok;
                        }
                    }

                    if (!Boards[1].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet2 = "Sys";
                            step.Result2 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet2 = "exe";
                            step.Result2 = Step.Ok;
                        }
                    }
                    if (!Boards[2].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet3 = "Sys";
                            step.Result3 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet3 = "exe";
                            step.Result3 = Step.Ok;
                        }
                    }
                    break;
                case 4:
                    if (!Boards[0].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet1 = "Sys";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "exe";
                            step.Result1 = Step.Ok;
                        }
                    }

                    if (!Boards[1].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet2 = "Sys";
                            step.Result2 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet2 = "exe";
                            step.Result2 = Step.Ok;
                        }
                    }
                    if (!Boards[2].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet3 = "Sys";
                            step.Result3 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet3 = "exe";
                            step.Result3 = Step.Ok;
                        }
                    }
                    if (!Boards[3].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet4 = "Sys";
                            step.Result4 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet4 = "exe";
                            step.Result4 = Step.Ok;
                        }
                    }
                    break;
                default:
                    break;
            }
            if (Int32.TryParse(step.Condition2, out int pressDelayTime))
            {
                if (pressDelayTime > 0)
                {
                    Task.Delay(pressDelayTime).Wait();
                    SetOK = RELAY.SetChannels(numberChannel, false);
                    if (!SetOK)
                    {
                        functionsParameterError("Sys", step);
                    }
                }
            }

            if (Int32.TryParse(step.Count, out int delayTime))
            {
                Task.Delay(delayTime).Wait();
            }
        }

        public void KEY(Step step)
        {

            List<String> Channel = new List<string>();
            if (step.Condition1 == null)
            {
                functionsParameterError("Condition 1", step);
                return;
            }
            if (step.Condition1.Contains("/"))
            {
                Channel = step.Condition1.Split('/').ToList();
            }
            else
            {
                Channel.Add(step.Condition1);
            }
            List<int> numberChannel = new List<int>();
            foreach (var item in Channel)
            {
                if (item.Contains('~'))
                {
                    if (!int.TryParse(item.Split('~')[0], out int startChannel))
                    {
                        functionsParameterError("Condition start", step);
                        return;
                    }
                    if (!int.TryParse(item.Split('~')[0], out int endChannel))
                    {
                        functionsParameterError("Condition start", step);
                        return;
                    }
                    for (int i = startChannel; i < endChannel; i++)
                    {
                        numberChannel.Add(i);
                    }
                }
                else
                {
                    if (int.TryParse(item, out int channelNumber))
                    {
                        numberChannel.Add(Convert.ToInt32(channelNumber));
                    }
                    else
                    {
                        functionsParameterError("Condition format", step);
                        return;
                    }
                }
            }

            bool SetOK = Solenoid.SetChannels(numberChannel, step.Oper == "ON");
            switch (Boards.Count)
            {
                case 1:
                    if (!Boards[0].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet1 = "Sys";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "exe";
                            step.Result1 = Step.Ok;
                        }
                    }
                    break;
                case 2:
                    if (!Boards[0].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet1 = "Sys";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "exe";
                            step.Result1 = Step.Ok;
                        }
                    }

                    if (!Boards[1].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet2 = "Sys";
                            step.Result2 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet2 = "exe";
                            step.Result2 = Step.Ok;
                        }
                    }
                    break;
                case 3:
                    if (!Boards[0].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet1 = "Sys";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "exe";
                            step.Result1 = Step.Ok;
                        }
                    }

                    if (!Boards[1].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet2 = "Sys";
                            step.Result2 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet2 = "exe";
                            step.Result2 = Step.Ok;
                        }
                    }
                    if (!Boards[2].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet3 = "Sys";
                            step.Result3 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet3 = "exe";
                            step.Result3 = Step.Ok;
                        }
                    }
                    break;
                case 4:
                    if (!Boards[0].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet1 = "Sys";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "exe";
                            step.Result1 = Step.Ok;
                        }
                    }

                    if (!Boards[1].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet2 = "Sys";
                            step.Result2 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet2 = "exe";
                            step.Result2 = Step.Ok;
                        }
                    }
                    if (!Boards[2].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet3 = "Sys";
                            step.Result3 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet3 = "exe";
                            step.Result3 = Step.Ok;
                        }
                    }
                    if (!Boards[3].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet4 = "Sys";
                            step.Result4 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet4 = "exe";
                            step.Result4 = Step.Ok;
                        }
                    }
                    break;
                default:
                    break;
            }
            if (Int32.TryParse(step.Condition2, out int pressDelayTime))
            {
                if (pressDelayTime > 0)
                {
                    Task.Delay(pressDelayTime).Wait();
                    SetOK = Solenoid.SetChannels(numberChannel, false);
                    if (!SetOK)
                    {
                        functionsParameterError("Sys", step);
                    }
                }
            }

            if (Int32.TryParse(step.Count, out int delayTime))
            {
                Task.Delay(delayTime).Wait();
            }
        }

        public void RES(Step step)
        {
            DMM.DMM_Rate rate;
            DMM.DMM_RES_Range range;
            try
            {
                range = (DMM.DMM_RES_Range)Enum.Parse(typeof(DMM.DMM_RES_Range), step.Oper, true);
            }
            catch (Exception)
            {
                switch (Boards.Count)
                {
                    case 1:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Oper";
                            step.Result1 = Step.Ng;
                        }
                        break;
                    case 2:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Oper";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Oper";
                            step.Result2 = Step.Ng;
                        }
                        break;
                    case 3:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Oper";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {

                            step.ValueGet2 = "Oper";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Oper";
                            step.Result3 = Step.Ng;
                        }
                        break;
                    case 4:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Oper";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Oper";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Oper";
                            step.Result3 = Step.Ng;
                        }
                        if (!Boards[3].Skip)
                        {
                            step.ValueGet4 = "Oper";
                            step.Result4 = Step.Ng;
                        }
                        break;
                    default:
                        break;
                }

                return;
            }
            try
            {
                rate = (DMM.DMM_Rate)Enum.Parse(typeof(DMM.DMM_Rate), step.Condition2, true);
                Console.WriteLine(rate.ToString());
            }
            catch (Exception)
            {
                switch (Boards.Count)
                {
                    case 1:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }
                        break;
                    case 2:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Condition";
                            step.Result2 = Step.Ng;
                        }
                        break;
                    case 3:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {

                            step.ValueGet2 = "Condition";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Condition";
                            step.Result3 = Step.Ng;
                        }
                        break;
                    case 4:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Condition";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Condition";
                            step.Result3 = Step.Ng;
                        }
                        if (!Boards[3].Skip)
                        {
                            step.ValueGet4 = "Condition";
                            step.Result4 = Step.Ng;
                        }
                        break;
                    default:
                        break;
                }

                return;
            }

            _DMM.SetModeRES(range, rate);
            DMM_BOARD_TEST(step);

        }

        public void ACV(Step step)
        {
            DMM.DMM_Rate rate;
            DMM.DMM_ACV_Range range;

            try
            {
                range = (DMM.DMM_ACV_Range)Enum.Parse(typeof(DMM.DMM_ACV_Range), step.Oper, true);
                Console.WriteLine(range.ToString());
            }
            catch (Exception)
            {
                functionsParameterError("Oper", step);

                return;
            }
            try
            {
                rate = (DMM.DMM_Rate)Enum.Parse(typeof(DMM.DMM_Rate), step.Condition2, true);
                Console.WriteLine(rate.ToString());
            }
            catch (Exception)
            {
                functionsParameterError("Condition", step);

                return;
            }

            _DMM.SetModeAC(range, rate);

            DMM_BOARD_TEST(step);
        }

        public void DCV(Step step)
        {
            DMM.DMM_Rate rate;
            DMM.DMM_DCV_Range range;

            try
            {
                range = (DMM.DMM_DCV_Range)Enum.Parse(typeof(DMM.DMM_DCV_Range), step.Oper, true);
                Console.WriteLine(range.ToString());
            }
            catch (Exception)
            {
                switch (Boards.Count)
                {
                    case 1:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Oper";
                            step.Result1 = Step.Ng;
                        }
                        break;
                    case 2:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Oper";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Oper";
                            step.Result2 = Step.Ng;
                        }
                        break;
                    case 3:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Oper";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {

                            step.ValueGet2 = "Oper";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Oper";
                            step.Result3 = Step.Ng;
                        }
                        break;
                    case 4:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Oper";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Oper";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Oper";
                            step.Result3 = Step.Ng;
                        }
                        if (!Boards[3].Skip)
                        {
                            step.ValueGet4 = "Oper";
                            step.Result4 = Step.Ng;
                        }
                        break;
                    default:
                        break;
                }

                return;
            }
            try
            {
                rate = (DMM.DMM_Rate)Enum.Parse(typeof(DMM.DMM_Rate), step.Condition2, true);
                Console.WriteLine(rate.ToString());
            }
            catch (Exception)
            {
                switch (Boards.Count)
                {
                    case 1:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }
                        break;
                    case 2:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Condition";
                            step.Result2 = Step.Ng;
                        }
                        break;
                    case 3:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {

                            step.ValueGet2 = "Condition";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Condition";
                            step.Result3 = Step.Ng;
                        }
                        break;
                    case 4:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Condition";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Condition";
                            step.Result3 = Step.Ng;
                        }
                        if (!Boards[3].Skip)
                        {
                            step.ValueGet4 = "Condition";
                            step.Result4 = Step.Ng;
                        }
                        break;
                    default:
                        break;
                }

                return;
            }

            _DMM.SetModeDC(range, rate);
            DMM_BOARD_TEST(step);

        }

        public void FREQ(Step step)
        {
            DMM.DMM_Rate rate;
            DMM.DMM_ACV_Range range;

            try
            {
                range = (DMM.DMM_ACV_Range)Enum.Parse(typeof(DMM.DMM_ACV_Range), step.Oper, true);
                Console.WriteLine(range.ToString());
            }
            catch (Exception)
            {
                switch (Boards.Count)
                {
                    case 1:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Oper";
                            step.Result1 = Step.Ng;
                        }
                        break;
                    case 2:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Oper";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Oper";
                            step.Result2 = Step.Ng;
                        }
                        break;
                    case 3:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Oper";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {

                            step.ValueGet2 = "Oper";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Oper";
                            step.Result3 = Step.Ng;
                        }
                        break;
                    case 4:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Oper";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Oper";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Oper";
                            step.Result3 = Step.Ng;
                        }
                        if (!Boards[3].Skip)
                        {
                            step.ValueGet4 = "Oper";
                            step.Result4 = Step.Ng;
                        }
                        break;
                    default:
                        break;
                }

                return;
            }
            try
            {
                rate = (DMM.DMM_Rate)Enum.Parse(typeof(DMM.DMM_Rate), step.Condition2, true);
                Console.WriteLine(rate.ToString());
            }
            catch (Exception)
            {
                switch (Boards.Count)
                {
                    case 1:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }
                        break;
                    case 2:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Condition";
                            step.Result2 = Step.Ng;
                        }
                        break;
                    case 3:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {

                            step.ValueGet2 = "Condition";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Condition";
                            step.Result3 = Step.Ng;
                        }
                        break;
                    case 4:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Condition";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Condition";
                            step.Result3 = Step.Ng;
                        }
                        if (!Boards[3].Skip)
                        {
                            step.ValueGet4 = "Condition";
                            step.Result4 = Step.Ng;
                        }
                        break;
                    default:
                        break;
                }

                return;
            }

            _DMM.SetModeFREQ(range, rate);
            DMM_BOARD_TEST(step);
        }

        public void DIODE(Step step)
        {
            DMM.DMM_Rate rate;

            try
            {
                rate = (DMM.DMM_Rate)Enum.Parse(typeof(DMM.DMM_Rate), step.Condition2, true);
                Console.WriteLine(rate.ToString());
            }
            catch (Exception)
            {
                switch (Boards.Count)
                {
                    case 1:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }
                        break;
                    case 2:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Condition";
                            step.Result2 = Step.Ng;
                        }
                        break;
                    case 3:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {

                            step.ValueGet2 = "Condition";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Condition";
                            step.Result3 = Step.Ng;
                        }
                        break;
                    case 4:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Condition";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Condition";
                            step.Result3 = Step.Ng;
                        }
                        if (!Boards[3].Skip)
                        {
                            step.ValueGet4 = "Condition";
                            step.Result4 = Step.Ng;
                        }
                        break;
                    default:
                        break;
                }

                return;
            }

            _DMM.SetModeDiode(rate);
            DMM_BOARD_TEST(step);

        }

        private void DMM_BOARD_TEST(Step step)
        {
            if (!_DMM.DMM1.SerialPort.Port.IsOpen & !_DMM.DMM2.SerialPort.Port.IsOpen)
            {
                switch (Boards.Count)
                {
                    case 1:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Sys";
                            step.Result1 = Step.Ng;
                        }
                        break;
                    case 2:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Sys";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Sys";
                            step.Result2 = Step.Ng;
                        }
                        break;
                    case 3:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Sys";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {

                            step.ValueGet2 = "Sys";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Sys";
                            step.Result3 = Step.Ng;
                        }
                        break;
                    case 4:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Sys";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Sys";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Sys";
                            step.Result3 = Step.Ng;
                        }
                        if (!Boards[3].Skip)
                        {
                            step.ValueGet4 = "Sys";
                            step.Result4 = Step.Ng;
                        }
                        break;
                    default:
                        break;
                }
                return;
            }
            Task.Delay(100).Wait();

            bool IsMux2WhenTest1Board = false;
            switch (Boards.Count)
            {
                case 1:
                    if (Boards[0].Skip) return;
                    if (!SetBoardMux(step.Condition1, 1, out IsMux2WhenTest1Board))
                    {
                        step.ValueGet1 = "condition1";
                        step.Result1 = Step.Ng;
                        return;
                    }
                    DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                    ReadDMMAndCompareBoard1(step, IsMux2WhenTest1Board);
                    break;

                case 2:
                    if (Boards[0].Skip && Boards[1].Skip) return;

                    if (!Boards[0].Skip)
                    {
                        if (!SetBoardMux(step.Condition1, 1, out _))
                        {
                            step.ValueGet1 = "condition1";
                            step.Result1 = Step.Ng;
                        }
                    }
                    if (!Boards[1].Skip)
                    {
                        if (!SetBoardMux(step.Condition1, 2, out _))
                        {
                            step.ValueGet2 = "condition1";
                            step.Result2 = Step.Ng;
                        }
                    }

                    if (step.Result1 != Step.Ng || step.Result2 != Step.Ng)
                        DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);

                    if (!Boards[0].Skip && !Boards[1].Skip)
                    {
                        if (!(step.Result1 == Step.Ng || step.Result2 == Step.Ng))
                            ReadDMMAndCompareBoard12(step);
                        else if (step.Result1 != Step.Ng)
                            ReadDMMAndCompareBoard1(step, false);
                        else if (step.Result2 != Step.Ng)
                            ReadDMMAndCompareBoard2(step);
                    }
                    else if (!Boards[0].Skip)
                    {
                        if (step.Result1 != Step.Ng)
                            ReadDMMAndCompareBoard1(step, false);
                    }
                    else if (!Boards[1].Skip)
                    {
                        if (step.Result2 != Step.Ng)
                            ReadDMMAndCompareBoard2(step);
                    }

                    break;

                case 3:
                    if (Boards[0].Skip && Boards[1].Skip && Boards[2].Skip) return;
                    if (!Boards[0].Skip)
                    {
                        if (!SetBoardMux(step.Condition1, 1, out _))
                        {
                            step.ValueGet1 = "condition1";
                            step.Result1 = Step.Ng;
                        }
                    }
                    if (!Boards[1].Skip)
                    {
                        if (!SetBoardMux(step.Condition1, 2, out _))
                        {
                            step.ValueGet2 = "condition1";
                            step.Result2 = Step.Ng;
                        }
                    }

                    if (!Boards[0].Skip || !Boards[1].Skip)
                    {
                        if (step.Result1 != Step.Ng || step.Result2 != Step.Ng)
                            DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                    }

                    if (!Boards[0].Skip && !Boards[1].Skip)
                    {
                        if (!(step.Result1 == Step.Ng || step.Result2 == Step.Ng))
                            ReadDMMAndCompareBoard12(step);
                        else if (step.Result1 != Step.Ng)
                            ReadDMMAndCompareBoard1(step, false);
                        else if (step.Result2 != Step.Ng)
                            ReadDMMAndCompareBoard2(step);
                    }
                    else if (!Boards[0].Skip)
                    {
                        if (step.Result1 != Step.Ng)
                            ReadDMMAndCompareBoard1(step, false);
                    }
                    else if (!Boards[1].Skip)
                    {
                        if (step.Result2 != Step.Ng)
                            ReadDMMAndCompareBoard2(step);
                    }

                    if (!Boards[2].Skip)
                    {
                        if (!SetBoardMux(step.Condition1, 2, out _))
                        {
                            step.ValueGet3 = "condition1";
                            step.Result3 = Step.Ng;
                        }
                        else
                        {
                            DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                        }
                    }

                    if (!Boards[2].Skip)
                        if (step.Result3 != Step.Ng)
                            ReadDMMAndCompareBoard3(step);
                        else
                            goto Out;

                    break;
                case 4:
                    if (Boards[0].Skip && Boards[1].Skip && Boards[2].Skip && Boards[3].Skip) return;
                    if (!Boards[0].Skip)
                    {
                        if (!SetBoardMux(step.Condition1, 1, out _))
                        {
                            step.ValueGet1 = "condition1";
                            step.Result1 = Step.Ng;
                        }
                    }
                    if (!Boards[1].Skip)
                    {
                        if (!SetBoardMux(step.Condition1, 2, out _))
                        {
                            step.ValueGet2 = "condition1";
                            step.Result2 = Step.Ng;
                        }
                    }

                    if (!Boards[0].Skip || !Boards[1].Skip)
                    {
                        if (!(step.Result1 == Step.Ng || step.Result2 == Step.Ng))
                            DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                    }


                    if (!Boards[0].Skip && !Boards[1].Skip)
                    {
                        if (!(step.Result1 == Step.Ng || step.Result2 == Step.Ng))
                            ReadDMMAndCompareBoard12(step);
                        else if (step.Result1 != Step.Ng)
                            ReadDMMAndCompareBoard1(step, false);
                        else if (step.Result2 != Step.Ng)
                            ReadDMMAndCompareBoard2(step);
                    }
                    else if (!Boards[0].Skip)
                    {
                        if (step.Result1 != Step.Ng)
                            ReadDMMAndCompareBoard1(step, false);
                    }
                    else if (!Boards[1].Skip)
                    {
                        if (step.Result2 != Step.Ng)
                            ReadDMMAndCompareBoard2(step);
                    }



                    if (!Boards[2].Skip)
                    {
                        if (!SetBoardMux(step.Condition1, 3, out _))
                        {
                            step.ValueGet3 = "condition1";
                            step.Result3 = Step.Ng;
                        }
                    }
                    if (!Boards[3].Skip)
                    {
                        if (!SetBoardMux(step.Condition1, 4, out _))
                        {
                            step.ValueGet4 = "condition1";
                            step.Result4 = Step.Ng;
                        }
                    }

                    if (!Boards[2].Skip || !Boards[3].Skip)
                    {
                        if (!(step.Result3 == Step.Ng || step.Result4 == Step.Ng))
                            DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                    }


                    if (!Boards[2].Skip && !Boards[3].Skip)
                    {
                        if (!(step.Result3 == Step.Ng || step.Result4 == Step.Ng))
                            ReadDMMAndCompareBoard34(step);
                        else if (step.Result3 != Step.Ng)
                            ReadDMMAndCompareBoard3(step);
                        else if (step.Result4 != Step.Ng)
                            ReadDMMAndCompareBoard4(step);
                    }
                    else if (!Boards[2].Skip)
                    {
                        if (step.Result3 != Step.Ng)
                            ReadDMMAndCompareBoard3(step);
                    }
                    else if (!Boards[3].Skip)
                    {
                        if (step.Result4 != Step.Ng)
                            ReadDMMAndCompareBoard4(step);
                    }
                    break;
                default:
                    break;
            }
        Out:
            Task.Delay(appSetting.ETCSetting.DelayDMMRead).Wait();
            MuxCard.Card.ReleaseChannels();
        }

        private bool ReadDMMAndCompareBoard1(Step step, bool ReadByDmm2)
        {
            var retry = Int32.Parse(step.Count);
            retry = retry < 2 ? 1 : retry;

            if (step.Mode != "Spect")
            {
                _DMM.DMM1.RequestValues(retry);
                _DMM.DMM2.RequestValues(retry);
            }

            double minValue = 0;
            double maxValue = 0;

            if (!Double.TryParse(step.Min, out minValue))
            {
                step.ValueGet1 = "Min";

                step.Result1 = Step.Ng;

                return false;
            }

            if (!Double.TryParse(step.Max, out maxValue))
            {
                if (step.Max == "L")
                {
                    maxValue = Double.MaxValue;
                }
                else
                {
                    step.ValueGet1 = "Max";

                    step.Result1 = Step.Ng;

                    return false;
                }
            }

            if (step.Result1 == Step.Ng) return false;

            switch (step.Mode)
            {
                case "SPEC":
                    for (int i = 0; i < retry; i++)
                    {
                        if (step.Result1 != Step.Ok & Boards.Count > 0)
                        {
                            if (ReadByDmm2) _DMM.DMM2.GetValue();
                            else _DMM.DMM1.GetValue();
                            step.ValueGet1 = _DMM.DMM1.LastStringValue;
                            if (_DMM.DMM1.LastDoubleValue <= maxValue & _DMM.DMM1.LastDoubleValue >= minValue)
                            {
                                step.Result1 = Step.Ok;
                                break;
                            }
                            else
                            {
                                step.Result1 = Step.Ng;
                            }
                        }
                        Task.Delay(appSetting.ETCSetting.DelayDMMRead).Wait();
                    }
                    break;
                case "CONT":
                    for (int i = 0; i < 40; i++)
                    {
                        if (ReadByDmm2) _DMM.DMM2.GetValue(i, 1);
                        else _DMM.DMM1.GetValue(i, 1);
                        Task.Delay(appSetting.ETCSetting.DelayDMMRead).Wait();
                    }
                    var DMM1_PassValues = _DMM.DMM1.ValuesCount1.Where(x => (x >= minValue && x <= maxValue)).ToList();
                    if (DMM1_PassValues.Count == retry)
                    {
                        step.ValueGet1 = _DMM.DMM1.LastStringValue;
                        step.Result1 = Step.Ng;
                    }
                    else
                    {
                        step.Result1 = Step.Ok;
                        step.ValueGet1 = _DMM.DMM1.GetStringValue(DMM1_PassValues[0]);
                    }


                    break;
                case "MIN":
                    for (int i = 0; i < retry; i++)
                    {
                        if (ReadByDmm2) _DMM.DMM2.GetValue(i, 1);
                        else _DMM.DMM1.GetValue(i, 1);
                    }

                    step.Result1 = Step.Ok;

                    step.ValueGet1 = _DMM.DMM1.GetStringValue(_DMM.DMM1.MIN_1);

                    if (_DMM.DMM1.MIN_1 <= maxValue && _DMM.DMM1.MIN_1 >= minValue)
                    {
                        step.Result1 = Step.Ng;
                    }
                    break;

                case "AVR":
                    for (int i = 0; i < retry; i++)
                    {
                        if (ReadByDmm2) _DMM.DMM2.GetValue(i, 1);
                        else _DMM.DMM1.GetValue(i, 1);
                    }
                    step.Result1 = Step.Ok;

                    step.ValueGet1 = _DMM.DMM1.GetStringValue(_DMM.DMM1.AVR_1);

                    if (_DMM.DMM1.AVR_1 > maxValue || _DMM.DMM1.AVR_1 < minValue)
                    {
                        step.Result1 = Step.Ng;
                    }
                    step.Result2 = Step.Ng;

                    break;
                case "MAX":
                    for (int i = 0; i < retry; i++)
                    {
                        if (ReadByDmm2) _DMM.DMM2.GetValue(i, 1);
                        else _DMM.DMM1.GetValue(i, 1);
                    }

                    step.Result1 = Step.Ok;

                    step.ValueGet1 = _DMM.DMM1.GetStringValue(_DMM.DMM1.MAX_1);
                    if (_DMM.DMM1.MAX_1 <= maxValue && _DMM.DMM1.MAX_1 >= minValue)
                    {
                        step.Result1 = Step.Ng;
                    }
                    break;
                default:
                    break;
            }


            return StepTestResult(step);
        }

        private bool ReadDMMAndCompareBoard2(Step step)
        {
            var retry = Int32.Parse(step.Count);
            retry = retry < 2 ? 1 : retry;

            if (step.Mode != "Spect")
            {
                _DMM.DMM2.RequestValues(retry);
            }

            double minValue = 0;
            double maxValue = 0;

            if (!Double.TryParse(step.Min, out minValue))
            {
                step.ValueGet2 = "Min";

                step.Result2 = Step.Ng;

                return false;
            }

            if (!Double.TryParse(step.Max, out maxValue))
            {
                if (step.Max == "L")
                {
                    maxValue = Double.MaxValue;
                }
                else
                {
                    step.ValueGet2 = "Max";

                    step.Result2 = Step.Ng;

                    return false;
                }

            }

            if (step.Result2 == Step.Ng) return false;

            switch (step.Mode)
            {
                case "SPEC":
                    for (int i = 0; i < retry; i++)
                    {
                        if (step.Result2 != Step.Ok & Boards.Count > 1)
                        {
                            _DMM.DMM2.GetValue();
                            step.ValueGet2 = _DMM.DMM2.LastStringValue;
                            if (_DMM.DMM2.LastDoubleValue <= maxValue & _DMM.DMM2.LastDoubleValue >= minValue)
                            {
                                step.Result2 = Step.Ok;
                                break;
                            }
                            else
                            {
                                step.Result2 = Step.Ng;
                            }
                        }
                        Task.Delay(appSetting.ETCSetting.DelayDMMRead).Wait();

                    }
                    break;
                case "CONT":
                    for (int i = 0; i < retry; i++)
                    {
                        _DMM.DMM2.GetValue(i, 1);
                        Task.Delay(appSetting.ETCSetting.DelayDMMRead).Wait();

                    }
                    var DMM2_PassValues = _DMM.DMM2.ValuesCount1.Where(x => (x >= minValue && x <= maxValue)).ToList();

                    if (DMM2_PassValues.Count == 0)
                    {
                        step.ValueGet2 = _DMM.DMM2.LastStringValue;
                        step.Result2 = Step.Ng;
                    }
                    else
                    {
                        step.ValueGet2 = _DMM.DMM2.GetStringValue(DMM2_PassValues[0]);
                        step.Result2 = Step.Ok;
                    }

                    break;
                case "MIN":
                    for (int i = 0; i < retry; i++)
                    {
                        _DMM.DMM2.GetValue(i, 1);
                    }

                    step.Result2 = Step.Ok;

                    step.ValueGet2 = _DMM.DMM2.GetStringValue(_DMM.DMM2.MIN_1);

                    if (_DMM.DMM2.MIN_1 <= maxValue && _DMM.DMM2.MIN_1 >= minValue)
                    {
                        step.Result2 = Step.Ng;
                    }
                    break;

                case "AVR":
                    for (int i = 0; i < retry; i++)
                    {
                        _DMM.DMM2.GetValue(i, 1);
                    }
                    step.Result2 = Step.Ok;
                    step.ValueGet2 = _DMM.DMM2.GetStringValue(_DMM.DMM2.AVR_1);

                    if (_DMM.DMM2.AVR_1 > maxValue || _DMM.DMM2.AVR_1 < minValue)
                    {
                        step.Result2 = Step.Ng;
                    }
                    break;
                case "MAX":
                    for (int i = 0; i < retry; i++)
                    {
                        _DMM.DMM2.GetValue(i, 1);
                    }

                    step.Result2 = Step.Ok;

                    step.ValueGet2 = _DMM.DMM2.GetStringValue(_DMM.DMM2.MAX_1);

                    if (_DMM.DMM2.MAX_1 <= maxValue && _DMM.DMM2.MAX_1 >= minValue)
                    {
                        step.Result2 = Step.Ng;
                    }
                    break;
                default:
                    break;
            }


            return StepTestResult(step);
        }

        private bool ReadDMMAndCompareBoard3(Step step)
        {
            var retry = Int32.Parse(step.Count);
            retry = retry < 2 ? 2 : retry;

            if (step.Mode != "Spect")
            {
                _DMM.DMM1.RequestValues(retry);
            }

            double minValue = 0;
            double maxValue = 0;

            if (!Double.TryParse(step.Min, out minValue))
            {
                step.ValueGet3 = "Min";

                step.Result3 = Step.Ng;

                return false;
            }

            if (!Double.TryParse(step.Max, out maxValue))
            {
                if (step.Max == "L")
                {
                    maxValue = Double.MaxValue;
                }
                else
                {
                    step.ValueGet3 = "Max";

                    step.Result3 = Step.Ng;

                    return false;
                }

            }

            if (step.Result3 == Step.Ng) return false;


            switch (step.Mode)
            {
                case "SPEC":
                    for (int i = 0; i < retry; i++)
                    {
                        if (step.Result3 != Step.Ok & Boards.Count > 0)
                        {
                            _DMM.DMM1.GetValue();
                            step.ValueGet3 = _DMM.DMM1.LastStringValue;
                            if (_DMM.DMM1.LastDoubleValue <= maxValue & _DMM.DMM1.LastDoubleValue >= minValue)
                            {
                                step.Result3 = Step.Ok;
                                break;
                            }
                            else
                            {
                                step.Result3 = Step.Ng;
                            }
                        }
                        Task.Delay(appSetting.ETCSetting.DelayDMMRead).Wait();

                    }
                    break;
                case "CONT":
                    for (int i = 0; i < retry; i++)
                    {
                        _DMM.DMM1.GetValue(i, 1);
                    }
                    var DMM1_PassValues = _DMM.DMM1.ValuesCount1.Where(x => (x >= minValue && x <= maxValue)).ToList();
                    if (DMM1_PassValues.Count == 0)
                    {
                        step.ValueGet3 = _DMM.DMM1.LastStringValue;
                        step.Result3 = Step.Ng;
                    }
                    else
                    {
                        step.Result3 = Step.Ok;
                        step.ValueGet3 = _DMM.DMM1.GetStringValue(DMM1_PassValues[0]);
                    }

                    break;
                case "MIN":
                    for (int i = 0; i < retry; i++)
                    {
                        _DMM.DMM1.GetValue(i, 1);
                    }

                    step.Result3 = Step.Ok;

                    step.ValueGet3 = _DMM.DMM1.GetStringValue(_DMM.DMM1.MIN_1);

                    if (_DMM.DMM1.MIN_1 <= maxValue && _DMM.DMM1.MIN_1 >= minValue)
                    {
                        step.Result3 = Step.Ng;
                    }
                    break;

                case "AVR":
                    for (int i = 0; i < retry; i++)
                    {
                        _DMM.DMM1.GetValue(i, 1);
                    }
                    step.Result3 = Step.Ok;

                    step.ValueGet3 = _DMM.DMM1.GetStringValue(_DMM.DMM1.AVR_1);

                    if (_DMM.DMM1.AVR_1 > maxValue || _DMM.DMM1.AVR_1 < minValue)
                    {
                        step.Result3 = Step.Ng;
                    }
                    break;
                case "MAX":
                    for (int i = 0; i < retry; i++)
                    {
                        _DMM.DMM1.GetValue(i, 1);
                    }

                    step.Result3 = Step.Ok;

                    step.ValueGet3 = _DMM.DMM1.GetStringValue(_DMM.DMM1.MAX_1);
                    if (_DMM.DMM1.MAX_1 <= maxValue && _DMM.DMM1.MAX_1 >= minValue)
                    {
                        step.Result3 = Step.Ng;
                    }
                    break;
                default:
                    break;
            }


            return StepTestResult(step);
        }

        private bool ReadDMMAndCompareBoard4(Step step)
        {
            var retry = Int32.Parse(step.Count);
            retry = retry < 2 ? 2 : retry;

            if (step.Mode != "Spect")
            {
                _DMM.DMM1.RequestValues(retry);
                _DMM.DMM2.RequestValues(retry);
            }

            double minValue = 0;
            double maxValue = 0;

            if (!Double.TryParse(step.Min, out minValue))
            {
                step.ValueGet4 = "Min";

                step.Result4 = Step.Ng;

                return false;
            }

            if (!Double.TryParse(step.Max, out maxValue))
            {
                if (step.Max == "L")
                {
                    maxValue = Double.MaxValue;
                }
                else
                {
                    step.ValueGet4 = "Max";

                    step.Result4 = Step.Ng;

                    return false;
                }

            }

            if (step.Result4 == Step.Ng) return false;

            switch (step.Mode)
            {
                case "SPEC":
                    for (int i = 0; i < retry; i++)
                    {
                        if (step.Result4 != Step.Ok & Boards.Count > 1)
                        {
                            _DMM.DMM2.GetValue();
                            step.ValueGet4 = _DMM.DMM2.LastStringValue;
                            if (_DMM.DMM2.LastDoubleValue <= maxValue & _DMM.DMM2.LastDoubleValue >= minValue)
                            {
                                step.Result4 = Step.Ok;
                            }
                            else
                            {
                                step.Result4 = Step.Ng;
                            }
                        }
                    }
                    break;
                case "CONT":
                    for (int i = 0; i < retry; i++)
                    {
                        _DMM.DMM2.GetValue(i, 1);
                    }
                    var DMM2_PassValues = _DMM.DMM2.ValuesCount1.Where(x => (x >= minValue && x <= maxValue)).ToList();

                    if (DMM2_PassValues.Count == 0)
                    {
                        step.ValueGet4 = _DMM.DMM2.LastStringValue;
                        step.Result4 = Step.Ng;
                    }
                    else
                    {
                        step.Result4 = Step.Ok;
                        step.ValueGet4 = _DMM.DMM2.GetStringValue(DMM2_PassValues[0]);
                    }

                    break;
                case "MIN":
                    for (int i = 0; i < retry; i++)
                    {
                        _DMM.DMM2.GetValue(i, 1);
                    }

                    step.Result4 = Step.Ok;

                    step.ValueGet4 = _DMM.DMM2.GetStringValue(_DMM.DMM2.MIN_1);

                    if (_DMM.DMM2.MIN_1 <= maxValue && _DMM.DMM2.MIN_1 >= minValue)
                    {
                        step.Result4 = Step.Ng;
                    }
                    break;

                case "AVR":
                    for (int i = 0; i < retry; i++)
                    {
                        _DMM.DMM2.GetValue(i, 1);
                    }
                    step.Result4 = Step.Ok;

                    step.ValueGet4 = _DMM.DMM2.GetStringValue(_DMM.DMM2.AVR_1);

                    if (_DMM.DMM2.AVR_1 > maxValue || _DMM.DMM2.AVR_1 < minValue)
                    {
                        step.Result4 = Step.Ng;
                    }
                    break;
                case "MAX":
                    for (int i = 0; i < retry; i++)
                    {
                        _DMM.DMM2.GetValue(i, 1);
                    }

                    step.Result4 = Step.Ok;

                    step.ValueGet4 = _DMM.DMM2.GetStringValue(_DMM.DMM2.MAX_1);

                    if (_DMM.DMM2.MAX_1 <= maxValue && _DMM.DMM2.MAX_1 >= minValue)
                    {
                        step.Result4 = Step.Ng;
                    }
                    break;
                default:
                    break;
            }


            return StepTestResult(step);
        }

        private bool ReadDMMAndCompareBoard12(Step step)
        {
            var retry = Int32.Parse(step.Count);
            retry = retry < 2 ? 1 : retry;

            if (step.Mode != "Spect")
            {
                _DMM.DMM1.RequestValues(retry);
                _DMM.DMM2.RequestValues(retry);
            }

            double minValue = 0;
            double maxValue = 0;

            if (!Double.TryParse(step.Min, out minValue))
            {
                step.ValueGet1 = "Min";
                step.ValueGet2 = "Min";
                step.ValueGet3 = "Min";
                step.ValueGet4 = "Min";

                step.Result1 = Step.Ng;
                step.Result2 = Step.Ng;
                step.Result3 = Step.Ng;
                step.Result4 = Step.Ng;

                return false;
            }

            if (!Double.TryParse(step.Max, out maxValue))
            {
                if (step.Max == "L")
                {
                    maxValue = Double.MaxValue;
                }
                else
                {
                    step.ValueGet1 = "Max";
                    step.ValueGet2 = "Max";
                    step.ValueGet3 = "Max";
                    step.ValueGet4 = "Max";

                    step.Result1 = Step.Ng;
                    step.Result2 = Step.Ng;
                    step.Result3 = Step.Ng;
                    step.Result4 = Step.Ng;

                    return false;
                }

            }

            switch (step.Mode)
            {
                case "SPEC":
                    for (int i = 0; i < retry; i++)
                    {
                        if (step.Result1 != Step.Ok & Boards.Count > 0)
                        {
                            _DMM.DMM1.GetValue();
                            step.ValueGet1 = _DMM.DMM1.LastStringValue;
                            if (_DMM.DMM1.LastDoubleValue <= maxValue & _DMM.DMM1.LastDoubleValue >= minValue)
                            {
                                step.Result1 = Step.Ok;
                            }
                            else
                            {
                                step.Result1 = Step.Ng;
                            }
                        }
                        if (step.Result2 != Step.Ok & Boards.Count > 1)
                        {
                            _DMM.DMM2.GetValue();
                            step.ValueGet2 = _DMM.DMM2.LastStringValue;
                            if (_DMM.DMM2.LastDoubleValue <= maxValue & _DMM.DMM2.LastDoubleValue >= minValue)
                            {
                                step.Result2 = Step.Ok;
                            }
                            else
                            {
                                step.Result2 = Step.Ng;
                            }
                        }
                        if (step.Result1 == Step.Ok && step.Result2 == Step.Ok)
                        {
                            break;
                        }
                        Task.Delay(appSetting.ETCSetting.DelayDMMRead).Wait();
                    }
                    break;
                case "CONT":
                    for (int i = 0; i < retry; i++)
                    {
                        _DMM.DMM1.GetValue(i, 1);
                        _DMM.DMM2.GetValue(i, 1);
                        Task.Delay(appSetting.ETCSetting.DelayDMMRead).Wait();

                    }
                    var DMM1_PassValues = _DMM.DMM1.ValuesCount1.Where(x => (x >= minValue && x <= maxValue)).ToList();
                    var DMM2_PassValues = _DMM.DMM2.ValuesCount1.Where(x => (x >= minValue && x <= maxValue)).ToList();
                    if (DMM1_PassValues.Count == 0)
                    {
                        step.ValueGet1 = _DMM.DMM1.LastStringValue;
                        step.Result1 = Step.Ng;
                    }
                    else
                    {
                        step.Result1 = Step.Ok;
                        step.ValueGet1 = _DMM.DMM1.GetStringValue(DMM1_PassValues[0]);
                    }

                    if (DMM2_PassValues.Count == 0)
                    {
                        step.ValueGet2 = _DMM.DMM2.LastStringValue;
                        step.Result2 = Step.Ng;
                    }
                    else
                    {
                        step.Result2 = Step.Ok;
                        step.ValueGet2 = _DMM.DMM2.GetStringValue(DMM2_PassValues[0]);
                    }

                    break;
                case "MIN":
                    for (int i = 0; i < retry; i++)
                    {
                        _DMM.DMM1.GetValue(i, 1);
                        _DMM.DMM2.GetValue(i, 1);
                    }

                    step.Result1 = Step.Ok;
                    step.Result2 = Step.Ok;

                    step.ValueGet1 = _DMM.DMM1.GetStringValue(_DMM.DMM1.MIN_1);
                    step.ValueGet2 = _DMM.DMM2.GetStringValue(_DMM.DMM2.MIN_1);

                    if (_DMM.DMM1.MIN_1 <= maxValue && _DMM.DMM1.MIN_1 >= minValue)
                    {
                        step.Result1 = Step.Ng;
                    }
                    if (_DMM.DMM2.MIN_1 <= maxValue && _DMM.DMM2.MIN_1 >= minValue)
                    {
                        step.Result2 = Step.Ng;
                    }
                    break;

                case "AVR":
                    for (int i = 0; i < retry; i++)
                    {
                        _DMM.DMM1.GetValue(i, 1);
                        _DMM.DMM2.GetValue(i, 1);
                    }
                    step.Result1 = Step.Ok;
                    step.Result2 = Step.Ok;

                    step.ValueGet1 = _DMM.DMM1.GetStringValue(_DMM.DMM1.AVR_1);
                    step.ValueGet2 = _DMM.DMM2.GetStringValue(_DMM.DMM2.AVR_1);

                    if (_DMM.DMM1.AVR_1 > maxValue || _DMM.DMM1.AVR_1 < minValue)
                    {
                        step.Result1 = Step.Ng;
                    }
                    if (_DMM.DMM2.AVR_1 > maxValue || _DMM.DMM2.AVR_1 < minValue)
                    {
                        step.Result2 = Step.Ng;
                    }
                    break;
                case "MAX":
                    for (int i = 0; i < retry; i++)
                    {
                        _DMM.DMM1.GetValue(i, 1);
                        _DMM.DMM2.GetValue(i, 1);
                    }

                    step.Result1 = Step.Ok;
                    step.Result2 = Step.Ok;

                    step.ValueGet1 = _DMM.DMM1.GetStringValue(_DMM.DMM1.MAX_1);
                    step.ValueGet2 = _DMM.DMM2.GetStringValue(_DMM.DMM2.MAX_1);
                    if (_DMM.DMM1.MAX_1 <= maxValue && _DMM.DMM1.MAX_1 >= minValue)
                    {
                        step.Result1 = Step.Ng;
                    }
                    if (_DMM.DMM2.MAX_1 <= maxValue && _DMM.DMM2.MAX_1 >= minValue)
                    {
                        step.Result2 = Step.Ng;
                    }
                    break;
                default:
                    break;
            }


            return StepTestResult(step);
        }

        private bool ReadDMMAndCompareBoard34(Step step)
        {
            var retry = Int32.Parse(step.Count);
            retry = retry < 2 ? 2 : retry;

            _DMM.DMM1.RequestValues(retry);
            _DMM.DMM2.RequestValues(retry);

            double minValue = 0;
            double maxValue = 0;

            if (!Double.TryParse(step.Min, out minValue))
            {
                step.ValueGet3 = "Min";
                step.ValueGet4 = "Min";

                step.Result3 = Step.Ng;
                step.Result4 = Step.Ng;

                return false;
            }

            if (!Double.TryParse(step.Max, out maxValue))
            {
                if (step.Max == "L")
                {
                    maxValue = Double.MaxValue;
                }
                else
                {
                    step.ValueGet3 = "Max";
                    step.ValueGet4 = "Max";

                    step.Result3 = Step.Ng;
                    step.Result4 = Step.Ng;

                    return false;
                }

            }

            step.Result3 = Step.DontCare;
            step.Result4 = Step.DontCare;

            switch (step.Mode)
            {
                case "SPEC":
                    for (int i = 0; i < retry; i++)
                    {
                        if (step.Result3 != Step.Ok & Boards.Count > 0)
                        {
                            _DMM.DMM1.GetValue();
                            step.ValueGet3 = _DMM.DMM1.LastStringValue;
                            if (_DMM.DMM1.LastDoubleValue <= maxValue & _DMM.DMM1.LastDoubleValue >= minValue)
                            {
                                step.Result3 = Step.Ok;
                            }
                            else
                            {
                                step.Result3 = Step.Ng;
                            }
                        }
                        if (step.Result4 != Step.Ok & Boards.Count > 1)
                        {
                            _DMM.DMM2.GetValue();
                            step.ValueGet4 = _DMM.DMM2.LastStringValue;
                            if (_DMM.DMM2.LastDoubleValue <= maxValue & _DMM.DMM2.LastDoubleValue >= minValue)
                            {
                                step.Result4 = Step.Ok;
                            }
                            else
                            {
                                step.Result4 = Step.Ng;
                            }
                        }
                    }
                    break;
                case "CONT":
                    for (int i = 0; i < retry; i++)
                    {
                        _DMM.DMM1.GetValue(i, 1);
                        _DMM.DMM2.GetValue(i, 1);
                    }
                    var DMM1_PassValues = _DMM.DMM1.ValuesCount1.Where(x => (x >= minValue && x <= maxValue)).ToList();
                    var DMM2_PassValues = _DMM.DMM2.ValuesCount1.Where(x => (x >= minValue && x <= maxValue)).ToList();
                    if (DMM1_PassValues.Count == 0)
                    {
                        step.ValueGet3 = _DMM.DMM1.LastStringValue;
                        step.Result3 = Step.Ng;
                    }
                    else
                    {
                        step.Result3 = Step.Ok;
                        step.ValueGet3 = _DMM.DMM1.GetStringValue(DMM1_PassValues[0]);
                    }

                    if (DMM2_PassValues.Count == 0)
                    {
                        step.ValueGet4 = _DMM.DMM2.LastStringValue;
                        step.Result4 = Step.Ng;
                    }
                    else
                    {
                        step.Result4 = Step.Ok;
                        step.ValueGet4 = _DMM.DMM2.GetStringValue(DMM2_PassValues[0]);
                    }

                    break;
                case "MIN":
                    for (int i = 0; i < retry; i++)
                    {
                        _DMM.DMM1.GetValue(i, 1);
                        _DMM.DMM2.GetValue(i, 1);
                    }

                    step.Result3 = Step.Ok;
                    step.Result4 = Step.Ok;

                    step.ValueGet3 = _DMM.DMM1.GetStringValue(_DMM.DMM1.MIN_1);
                    step.ValueGet4 = _DMM.DMM2.GetStringValue(_DMM.DMM2.MIN_1);

                    if (_DMM.DMM1.MIN_1 <= maxValue && _DMM.DMM1.MIN_1 >= minValue)
                    {
                        step.Result3 = Step.Ng;
                    }
                    if (_DMM.DMM2.MIN_1 <= maxValue && _DMM.DMM2.MIN_1 >= minValue)
                    {
                        step.Result4 = Step.Ng;
                    }
                    break;

                case "AVR":
                    for (int i = 0; i < retry; i++)
                    {
                        _DMM.DMM1.GetValue(i, 1);
                        _DMM.DMM2.GetValue(i, 1);
                    }
                    step.Result3 = Step.Ok;
                    step.Result4 = Step.Ok;

                    step.ValueGet2 = _DMM.DMM1.GetStringValue(_DMM.DMM1.AVR_1);
                    step.ValueGet4 = _DMM.DMM2.GetStringValue(_DMM.DMM2.AVR_1);

                    if (_DMM.DMM1.AVR_1 > maxValue || _DMM.DMM1.AVR_1 < minValue)
                    {
                        step.Result3 = Step.Ng;
                    }
                    if (_DMM.DMM2.AVR_1 > maxValue || _DMM.DMM2.AVR_1 < minValue)
                    {
                        step.Result4 = Step.Ng;
                    }
                    break;
                case "MAX":
                    for (int i = 0; i < retry; i++)
                    {
                        _DMM.DMM1.GetValue(i, 1);
                        _DMM.DMM2.GetValue(i, 1);
                    }

                    step.Result3 = Step.Ok;
                    step.Result4 = Step.Ok;

                    step.ValueGet2 = _DMM.DMM1.GetStringValue(_DMM.DMM1.MAX_1);
                    step.ValueGet4 = _DMM.DMM2.GetStringValue(_DMM.DMM2.MAX_1);
                    if (_DMM.DMM1.MAX_1 <= maxValue && _DMM.DMM1.MAX_1 >= minValue)
                    {
                        step.Result3 = Step.Ng;
                    }
                    if (_DMM.DMM2.MAX_1 <= maxValue && _DMM.DMM2.MAX_1 >= minValue)
                    {
                        step.Result4 = Step.Ng;
                    }
                    break;
                default:
                    break;
            }


            return StepTestResult(step);
        }

        private void DelayAfterMuxSellect(DMM.DMM_Mode mode, DMM.DMM_Rate rate)
        {
            switch (mode)
            {
                case DMM.DMM_Mode.NONE:
                    break;
                case DMM.DMM_Mode.DCV:
                    switch (rate)
                    {
                        case DMM.DMM_Rate.NONE:
                            break;
                        case DMM.DMM_Rate.SLOW:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_slow_DCV).Wait();
                            break;
                        case DMM.DMM_Rate.MID:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_Mid_DCV).Wait();
                            break;
                        case DMM.DMM_Rate.FAST:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_Fast_DCV).Wait();
                            break;
                        default:
                            break;
                    }
                    break;
                case DMM.DMM_Mode.ACV:
                    switch (rate)
                    {
                        case DMM.DMM_Rate.NONE:
                            break;
                        case DMM.DMM_Rate.SLOW:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_slow_ACVFRQ).Wait();
                            break;
                        case DMM.DMM_Rate.MID:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_Mid_ACVFRQ).Wait();
                            break;
                        case DMM.DMM_Rate.FAST:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_Fast_ACVFRQ).Wait();
                            break;
                        default:
                            break;
                    }
                    break;
                case DMM.DMM_Mode.FREQ:
                    switch (rate)
                    {
                        case DMM.DMM_Rate.NONE:
                            break;
                        case DMM.DMM_Rate.SLOW:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_slow_ACVFRQ).Wait();
                            break;
                        case DMM.DMM_Rate.MID:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_Mid_ACVFRQ).Wait();
                            break;
                        case DMM.DMM_Rate.FAST:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_Fast_ACVFRQ).Wait();
                            break;
                        default:
                            break;
                    }
                    break;
                case DMM.DMM_Mode.RES:
                    switch (rate)
                    {
                        case DMM.DMM_Rate.NONE:
                            break;
                        case DMM.DMM_Rate.SLOW:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_slow_RES).Wait();
                            break;
                        case DMM.DMM_Rate.MID:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_Mid_RES).Wait();
                            break;
                        case DMM.DMM_Rate.FAST:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_Fast_RES).Wait();
                            break;
                        default:
                            break;
                    }
                    break;
                case DMM.DMM_Mode.DIODE:
                    switch (rate)
                    {
                        case DMM.DMM_Rate.NONE:
                            break;
                        case DMM.DMM_Rate.SLOW:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_slow_DCV).Wait();
                            break;
                        case DMM.DMM_Rate.MID:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_Mid_DCV).Wait();
                            break;
                        case DMM.DMM_Rate.FAST:
                            Task.Delay(appSetting.ETCSetting.MUXdelay_Fast_DCV).Wait();
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        private bool DisCharge()
        {
            if (!_DMM.DMM1.SerialPort.Port.IsOpen & !_DMM.DMM2.SerialPort.Port.IsOpen)
            {
                return false;
            }
            SYSTEM.System_Board.MachineIO.ADSC = true;
            SYSTEM.System_Board.MachineIO.BDSC = true;
            SYSTEM.System_Board.SendControl();


            //Discharge item 1
            bool DisChargeItem1Pass = false;
            bool DisChargeItem2Pass = false;
            bool DisChargeItem3Pass = false;

            DateTime StartDisChargeTime = DateTime.Now;
            if (TestModel.Discharge.DischargeItem1 != 0) // check none discharge mode
            {
                switch (TestModel.Discharge.DischargeItem1)
                {
                    case 1:
                        _DMM.SetModeDC(DMM.DMM_DCV_Range.DC1000V, DMM.DMM_Rate.MID);
                        break;
                    case 2:
                        _DMM.SetModeAC(DMM.DMM_ACV_Range.AC750V, DMM.DMM_Rate.MID);
                        break;
                    default:
                        break;
                }
                switch (Boards.Count)
                {
                    case 1:
                        if (Boards[0].Skip) return true;
                        if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 1, out bool IsMux2WhenTest1Board))
                        {
                            return false;
                        }
                        DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                        StartDisChargeTime = DateTime.Now;
                        while (DateTime.Now.Subtract(StartDisChargeTime).TotalMilliseconds < TestModel.Discharge.DischargeTime)
                        {
                            if (IsMux2WhenTest1Board)
                            {
                                _DMM.DMM1.GetValue();
                                if (_DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow)
                                {
                                    DisChargeItem1Pass = true;
                                    break;
                                }
                            }
                            else
                            {
                                _DMM.DMM2.GetValue();
                                if (_DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow)
                                {
                                    DisChargeItem1Pass = true;
                                    break;
                                }
                            }
                        }
                        break;
                    case 2:
                        if (Boards[0].Skip && Boards[1].Skip) return true;
                        if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 1, out _))
                        {
                            return false;
                        }
                        if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 2, out _))
                        {
                            return false;
                        }
                        DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                        StartDisChargeTime = DateTime.Now;
                        while (DateTime.Now.Subtract(StartDisChargeTime).TotalMilliseconds < TestModel.Discharge.DischargeTime)
                        {
                            _DMM.DMM1.GetValue();
                            _DMM.DMM2.GetValue();
                            if (!Boards[0].Skip && !Boards[1].Skip)
                            {
                                DisChargeItem1Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow
                                    & _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                if (DisChargeItem1Pass) break;
                            }
                            else if (!Boards[0].Skip)
                            {
                                DisChargeItem1Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                if (DisChargeItem1Pass) break;
                            }
                            else if (!Boards[1].Skip)
                            {
                                DisChargeItem1Pass = _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                if (DisChargeItem1Pass) break;
                            }
                        }
                        break;
                    case 3:
                        if (!Boards[0].Skip || !Boards[1].Skip)
                        {
                            if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 1, out _))
                            {
                                return false;
                            }
                            if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 2, out _))
                            {
                                return false;
                            }
                            DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                            StartDisChargeTime = DateTime.Now;
                            while (DateTime.Now.Subtract(StartDisChargeTime).TotalMilliseconds < TestModel.Discharge.DischargeTime)
                            {
                                _DMM.DMM1.GetValue();
                                _DMM.DMM2.GetValue();
                                if (!Boards[0].Skip && !Boards[1].Skip)
                                {
                                    DisChargeItem1Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow
                                        & _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem1Pass) break;
                                }
                                else if (!Boards[0].Skip)
                                {
                                    DisChargeItem1Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem1Pass) break;
                                }
                                else if (!Boards[1].Skip)
                                {
                                    DisChargeItem1Pass = _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem1Pass) break;
                                }
                            }
                        }

                        if (!Boards[2].Skip)
                        {
                            if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 3, out _))
                            {
                                return false;
                            }
                            DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                            StartDisChargeTime = DateTime.Now;
                            while (DateTime.Now.Subtract(StartDisChargeTime).TotalMilliseconds < TestModel.Discharge.DischargeTime)
                            {
                                _DMM.DMM1.GetValue();
                                if (_DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow)
                                {
                                    DisChargeItem1Pass &= true;
                                    break;
                                }
                            }
                        }
                        break;
                    case 4:
                        if (!Boards[0].Skip || !Boards[1].Skip)
                        {
                            if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 1, out _))
                            {
                                return false;
                            }
                            if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 2, out _))
                            {
                                return false;
                            }
                            DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                            StartDisChargeTime = DateTime.Now;
                            while (DateTime.Now.Subtract(StartDisChargeTime).TotalMilliseconds < TestModel.Discharge.DischargeTime)
                            {
                                _DMM.DMM1.GetValue();
                                _DMM.DMM2.GetValue();
                                if (!Boards[0].Skip && !Boards[1].Skip)
                                {
                                    DisChargeItem1Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow
                                        & _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem1Pass) break;
                                }
                                else if (!Boards[0].Skip)
                                {
                                    DisChargeItem1Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem1Pass) break;
                                }
                                else if (!Boards[1].Skip)
                                {
                                    DisChargeItem1Pass = _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem1Pass) break;
                                }
                            }
                        }

                        if (!Boards[3].Skip || !Boards[2].Skip)
                        {
                            if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 3, out _))
                            {
                                return false;
                            }
                            if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 4, out _))
                            {
                                return false;
                            }
                            DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                            StartDisChargeTime = DateTime.Now;
                            while (DateTime.Now.Subtract(StartDisChargeTime).TotalMilliseconds < TestModel.Discharge.DischargeTime)
                            {
                                _DMM.DMM1.GetValue();
                                _DMM.DMM2.GetValue();
                                if (!Boards[2].Skip && !Boards[3].Skip)
                                {
                                    DisChargeItem1Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow
                                        & _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem1Pass) break;
                                }
                                else if (!Boards[2].Skip)
                                {
                                    DisChargeItem1Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem1Pass) break;
                                }
                                else if (!Boards[3].Skip)
                                {
                                    DisChargeItem1Pass = _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem1Pass) break;
                                }
                            }
                        }
                        break;
                }
            }


            if (TestModel.Discharge.DischargeItem2 != 0 && DisChargeItem1Pass) // check none discharge mode
            {
                switch (TestModel.Discharge.DischargeItem2)
                {
                    case 1:
                        _DMM.SetModeDC(DMM.DMM_DCV_Range.DC1000V, DMM.DMM_Rate.MID);
                        break;
                    case 2:
                        _DMM.SetModeAC(DMM.DMM_ACV_Range.AC750V, DMM.DMM_Rate.MID);
                        break;
                    default:
                        break;
                }
                switch (Boards.Count)
                {
                    case 1:
                        if (Boards[0].Skip) return true;
                        if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 1, out bool IsMux2WhenTest1Board))
                        {
                            return false;
                        }
                        DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                        StartDisChargeTime = DateTime.Now;
                        while (DateTime.Now.Subtract(StartDisChargeTime).TotalMilliseconds < TestModel.Discharge.DischargeTime)
                        {
                            if (IsMux2WhenTest1Board)
                            {
                                _DMM.DMM1.GetValue();
                                if (_DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow)
                                {
                                    DisChargeItem2Pass = true;
                                    break;
                                }
                            }
                            else
                            {
                                _DMM.DMM2.GetValue();
                                if (_DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow)
                                {
                                    DisChargeItem2Pass = true;
                                    break;
                                }
                            }
                        }
                        break;
                    case 2:
                        if (Boards[0].Skip && Boards[1].Skip) return true;
                        if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 1, out _))
                        {
                            return false;
                        }
                        if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 2, out _))
                        {
                            return false;
                        }
                        DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                        StartDisChargeTime = DateTime.Now;
                        while (DateTime.Now.Subtract(StartDisChargeTime).TotalMilliseconds < TestModel.Discharge.DischargeTime)
                        {
                            _DMM.DMM1.GetValue();
                            _DMM.DMM2.GetValue();
                            if (!Boards[0].Skip && !Boards[1].Skip)
                            {
                                DisChargeItem2Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow
                                    & _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                if (DisChargeItem2Pass) break;
                            }
                            else if (!Boards[0].Skip)
                            {
                                DisChargeItem2Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                if (DisChargeItem2Pass) break;
                            }
                            else if (!Boards[1].Skip)
                            {
                                DisChargeItem2Pass = _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                if (DisChargeItem2Pass) break;
                            }
                        }
                        break;
                    case 3:
                        if (!Boards[0].Skip || !Boards[1].Skip)
                        {
                            if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 1, out _))
                            {
                                return false;
                            }
                            if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 2, out _))
                            {
                                return false;
                            }
                            DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                            StartDisChargeTime = DateTime.Now;
                            while (DateTime.Now.Subtract(StartDisChargeTime).TotalMilliseconds < TestModel.Discharge.DischargeTime)
                            {
                                _DMM.DMM1.GetValue();
                                _DMM.DMM2.GetValue();
                                if (!Boards[0].Skip && !Boards[1].Skip)
                                {
                                    DisChargeItem2Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow
                                        & _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem2Pass) break;
                                }
                                else if (!Boards[0].Skip)
                                {
                                    DisChargeItem2Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem2Pass) break;
                                }
                                else if (!Boards[1].Skip)
                                {
                                    DisChargeItem2Pass = _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem2Pass) break;
                                }
                            }
                        }

                        if (!Boards[2].Skip)
                        {
                            if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 3, out _))
                            {
                                return false;
                            }
                            DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                            StartDisChargeTime = DateTime.Now;
                            while (DateTime.Now.Subtract(StartDisChargeTime).TotalMilliseconds < TestModel.Discharge.DischargeTime)
                            {
                                _DMM.DMM1.GetValue();
                                if (_DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow)
                                {
                                    DisChargeItem2Pass &= true;
                                    break;
                                }
                            }
                        }
                        break;
                    case 4:
                        if (!Boards[0].Skip || !Boards[1].Skip)
                        {
                            if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 1, out _))
                            {
                                return false;
                            }
                            if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 2, out _))
                            {
                                return false;
                            }
                            DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                            StartDisChargeTime = DateTime.Now;
                            while (DateTime.Now.Subtract(StartDisChargeTime).TotalMilliseconds < TestModel.Discharge.DischargeTime)
                            {
                                _DMM.DMM1.GetValue();
                                _DMM.DMM2.GetValue();
                                if (!Boards[0].Skip && !Boards[1].Skip)
                                {
                                    DisChargeItem2Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow
                                        & _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem2Pass) break;
                                }
                                else if (!Boards[0].Skip)
                                {
                                    DisChargeItem2Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem2Pass) break;
                                }
                                else if (!Boards[1].Skip)
                                {
                                    DisChargeItem2Pass = _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem2Pass) break;
                                }
                            }
                        }

                        if (!Boards[3].Skip || !Boards[2].Skip)
                        {
                            if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 3, out _))
                            {
                                return false;
                            }
                            if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 4, out _))
                            {
                                return false;
                            }
                            DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                            StartDisChargeTime = DateTime.Now;
                            while (DateTime.Now.Subtract(StartDisChargeTime).TotalMilliseconds < TestModel.Discharge.DischargeTime)
                            {
                                _DMM.DMM1.GetValue();
                                _DMM.DMM2.GetValue();
                                if (!Boards[2].Skip && !Boards[3].Skip)
                                {
                                    DisChargeItem2Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow
                                        & _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem2Pass) break;
                                }
                                else if (!Boards[2].Skip)
                                {
                                    DisChargeItem2Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem2Pass) break;
                                }
                                else if (!Boards[3].Skip)
                                {
                                    DisChargeItem2Pass = _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem2Pass) break;
                                }
                            }
                        }
                        break;
                }
            }


            if (TestModel.Discharge.DischargeItem3 != 0 && DisChargeItem2Pass) // check none discharge mode
            {
                switch (TestModel.Discharge.DischargeItem3)
                {
                    case 1:
                        _DMM.SetModeDC(DMM.DMM_DCV_Range.DC1000V, DMM.DMM_Rate.MID);
                        break;
                    case 2:
                        _DMM.SetModeAC(DMM.DMM_ACV_Range.AC750V, DMM.DMM_Rate.MID);
                        break;
                    default:
                        break;
                }
                switch (Boards.Count)
                {
                    case 1:
                        if (Boards[0].Skip) return true;
                        if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 1, out bool IsMux2WhenTest1Board))
                        {
                            return false;
                        }
                        DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                        StartDisChargeTime = DateTime.Now;
                        while (DateTime.Now.Subtract(StartDisChargeTime).TotalMilliseconds < TestModel.Discharge.DischargeTime)
                        {
                            if (IsMux2WhenTest1Board)
                            {
                                _DMM.DMM1.GetValue();
                                if (_DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow)
                                {
                                    DisChargeItem3Pass = true;
                                    break;
                                }
                            }
                            else
                            {
                                _DMM.DMM2.GetValue();
                                if (_DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow)
                                {
                                    DisChargeItem3Pass = true;
                                    break;
                                }
                            }
                        }
                        break;
                    case 2:
                        if (Boards[0].Skip && Boards[1].Skip) return true;
                        if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 1, out _))
                        {
                            return false;
                        }
                        if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 2, out _))
                        {
                            return false;
                        }
                        DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                        StartDisChargeTime = DateTime.Now;
                        while (DateTime.Now.Subtract(StartDisChargeTime).TotalMilliseconds < TestModel.Discharge.DischargeTime)
                        {
                            _DMM.DMM1.GetValue();
                            _DMM.DMM2.GetValue();
                            if (!Boards[0].Skip && !Boards[1].Skip)
                            {
                                DisChargeItem3Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow
                                    & _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                if (DisChargeItem3Pass) break;
                            }
                            else if (!Boards[0].Skip)
                            {
                                DisChargeItem3Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                if (DisChargeItem3Pass) break;
                            }
                            else if (!Boards[1].Skip)
                            {
                                DisChargeItem3Pass = _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                if (DisChargeItem3Pass) break;
                            }
                        }
                        break;
                    case 3:
                        if (!Boards[0].Skip || !Boards[1].Skip)
                        {
                            if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 1, out _))
                            {
                                return false;
                            }
                            if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 2, out _))
                            {
                                return false;
                            }
                            DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                            StartDisChargeTime = DateTime.Now;
                            while (DateTime.Now.Subtract(StartDisChargeTime).TotalMilliseconds < TestModel.Discharge.DischargeTime)
                            {
                                _DMM.DMM1.GetValue();
                                _DMM.DMM2.GetValue();
                                if (!Boards[0].Skip && !Boards[1].Skip)
                                {
                                    DisChargeItem3Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow
                                        & _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem3Pass) break;
                                }
                                else if (!Boards[0].Skip)
                                {
                                    DisChargeItem3Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem3Pass) break;
                                }
                                else if (!Boards[1].Skip)
                                {
                                    DisChargeItem3Pass = _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem3Pass) break;
                                }
                            }
                        }

                        if (!Boards[2].Skip)
                        {
                            if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 3, out _))
                            {
                                return false;
                            }
                            DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                            StartDisChargeTime = DateTime.Now;
                            while (DateTime.Now.Subtract(StartDisChargeTime).TotalMilliseconds < TestModel.Discharge.DischargeTime)
                            {
                                _DMM.DMM1.GetValue();
                                if (_DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow)
                                {
                                    DisChargeItem3Pass &= true;
                                    break;
                                }
                            }
                        }
                        break;
                    case 4:
                        if (!Boards[0].Skip || !Boards[1].Skip)
                        {
                            if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 1, out _))
                            {
                                return false;
                            }
                            if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 2, out _))
                            {
                                return false;
                            }
                            DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                            StartDisChargeTime = DateTime.Now;
                            while (DateTime.Now.Subtract(StartDisChargeTime).TotalMilliseconds < TestModel.Discharge.DischargeTime)
                            {
                                _DMM.DMM1.GetValue();
                                _DMM.DMM2.GetValue();
                                if (!Boards[0].Skip && !Boards[1].Skip)
                                {
                                    DisChargeItem3Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow
                                        & _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem3Pass) break;
                                }
                                else if (!Boards[0].Skip)
                                {
                                    DisChargeItem3Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem3Pass) break;
                                }
                                else if (!Boards[1].Skip)
                                {
                                    DisChargeItem3Pass = _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem3Pass) break;
                                }
                            }
                        }

                        if (!Boards[3].Skip || !Boards[2].Skip)
                        {
                            if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 3, out _))
                            {
                                return false;
                            }
                            if (!SetBoardMux(String.Format("{0}/{1}", TestModel.Discharge.Item1ChannelP, TestModel.Discharge.Item1ChannelN), 4, out _))
                            {
                                return false;
                            }
                            DelayAfterMuxSellect(_DMM.Mode, _DMM.Rate);
                            StartDisChargeTime = DateTime.Now;
                            while (DateTime.Now.Subtract(StartDisChargeTime).TotalMilliseconds < TestModel.Discharge.DischargeTime)
                            {
                                _DMM.DMM1.GetValue();
                                _DMM.DMM2.GetValue();
                                if (!Boards[2].Skip && !Boards[3].Skip)
                                {
                                    DisChargeItem3Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow
                                        & _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem3Pass) break;
                                }
                                else if (!Boards[2].Skip)
                                {
                                    DisChargeItem3Pass = _DMM.DMM1.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem3Pass) break;
                                }
                                else if (!Boards[3].Skip)
                                {
                                    DisChargeItem3Pass = _DMM.DMM2.LastDoubleValue < TestModel.Discharge.Item1VoltageBelow;
                                    if (DisChargeItem3Pass) break;
                                }
                            }
                        }
                        break;
                }
            }

            return DisChargeItem1Pass & DisChargeItem2Pass & DisChargeItem3Pass;
        }

        private void PCB(Step step)
        {
            List<string> BoardSellect = step.Condition1.Split(',').ToList();
            foreach (var item in Boards)
            {
                item.Skip = true;
            }
            foreach (var item in BoardSellect)
            {
                switch (item)
                {
                    case "1":
                        if (Boards.Count > 1) Boards[0].Skip = false;
                        break;
                    case "2":
                        if (Boards.Count > 2) Boards[1].Skip = false;
                        break;
                    case "3":
                        if (Boards.Count > 3) Boards[2].Skip = false;
                        break;
                    case "4":
                        if (Boards.Count > 4) Boards[3].Skip = false;
                        break;
                    default:
                        break;
                }
            }
        }

        private void STL(Step step)
        {
            if (!LEVEL.SerialPort.Port.IsOpen)
            {
                functionsParameterError("Sys", step);
                return;
            }
            LEVEL.StartGetSample(100);
            bool SetOK = true;
            switch (Boards.Count)
            {
                case 1:
                    if (!Boards[0].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet1 = "Sys";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "exe";
                            step.Result1 = Step.Ok;
                        }
                    }
                    break;
                case 2:
                    if (!Boards[0].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet1 = "Sys";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "exe";
                            step.Result1 = Step.Ok;
                        }
                    }

                    if (!Boards[1].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet2 = "Sys";
                            step.Result2 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet2 = "exe";
                            step.Result2 = Step.Ok;
                        }
                    }
                    break;
                case 3:
                    if (!Boards[0].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet1 = "Sys";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "exe";
                            step.Result1 = Step.Ok;
                        }
                    }

                    if (!Boards[1].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet2 = "Sys";
                            step.Result2 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet2 = "exe";
                            step.Result2 = Step.Ok;
                        }
                    }
                    if (!Boards[2].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet3 = "Sys";
                            step.Result3 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet3 = "exe";
                            step.Result3 = Step.Ok;
                        }
                    }
                    break;
                case 4:
                    if (!Boards[0].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet1 = "Sys";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "exe";
                            step.Result1 = Step.Ok;
                        }
                    }

                    if (!Boards[1].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet2 = "Sys";
                            step.Result2 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet2 = "exe";
                            step.Result2 = Step.Ok;
                        }
                    }
                    if (!Boards[2].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet3 = "Sys";
                            step.Result3 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet3 = "exe";
                            step.Result3 = Step.Ok;
                        }
                    }
                    if (!Boards[3].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet4 = "Sys";
                            step.Result4 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet4 = "exe";
                            step.Result4 = Step.Ok;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void EDL(Step step)
        {
            if (!LEVEL.SerialPort.Port.IsOpen)
            {
                functionsParameterError("Sys", step);
                return;
            }
            LEVEL.StopGetSample();
            bool SetOK = true;
            switch (Boards.Count)
            {
                case 1:
                    if (!Boards[0].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet1 = "Sys";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "exe";
                            step.Result1 = Step.Ok;
                        }
                    }
                    break;
                case 2:
                    if (!Boards[0].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet1 = "Sys";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "exe";
                            step.Result1 = Step.Ok;
                        }
                    }

                    if (!Boards[1].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet2 = "Sys";
                            step.Result2 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet2 = "exe";
                            step.Result2 = Step.Ok;
                        }
                    }
                    break;
                case 3:
                    if (!Boards[0].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet1 = "Sys";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "exe";
                            step.Result1 = Step.Ok;
                        }
                    }

                    if (!Boards[1].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet2 = "Sys";
                            step.Result2 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet2 = "exe";
                            step.Result2 = Step.Ok;
                        }
                    }
                    if (!Boards[2].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet3 = "Sys";
                            step.Result3 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet3 = "exe";
                            step.Result3 = Step.Ok;
                        }
                    }
                    break;
                case 4:
                    if (!Boards[0].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet1 = "Sys";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "exe";
                            step.Result1 = Step.Ok;
                        }
                    }

                    if (!Boards[1].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet2 = "Sys";
                            step.Result2 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet2 = "exe";
                            step.Result2 = Step.Ok;
                        }
                    }
                    if (!Boards[2].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet3 = "Sys";
                            step.Result3 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet3 = "exe";
                            step.Result3 = Step.Ok;
                        }
                    }
                    if (!Boards[3].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet4 = "Sys";
                            step.Result4 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet4 = "exe";
                            step.Result4 = Step.Ok;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void LCC(Step step)
        {
            if (!LEVEL.SerialPort.Port.IsOpen)
            {
                functionsParameterError("Sys", step);
                return;
            }
            int channel = 0;
            try
            {
                channel = Int32.Parse(step.Condition1) - 1;
            }
            catch (Exception)
            {
                switch (Boards.Count)
                {
                    case 1:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }
                        break;
                    case 2:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Condition";
                            step.Result2 = Step.Ng;
                        }
                        break;
                    case 3:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {

                            step.ValueGet2 = "Condition";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Condition";
                            step.Result3 = Step.Ng;
                        }
                        break;
                    case 4:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Condition";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Condition";
                            step.Result3 = Step.Ng;
                        }
                        if (!Boards[3].Skip)
                        {
                            step.ValueGet4 = "Condition";
                            step.Result4 = Step.Ng;
                        }
                        break;
                    default:
                        break;
                }

                return;
            }

            if (channel >= Boards[0].LevelChannels.Count())
            {
                switch (Boards.Count)
                {
                    case 1:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }
                        break;
                    case 2:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Condition";
                            step.Result2 = Step.Ng;
                        }
                        break;
                    case 3:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {

                            step.ValueGet2 = "Condition";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Condition";
                            step.Result3 = Step.Ng;
                        }
                        break;
                    case 4:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Condition";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Condition";
                            step.Result3 = Step.Ng;
                        }
                        if (!Boards[3].Skip)
                        {
                            step.ValueGet4 = "Condition";
                            step.Result4 = Step.Ng;
                        }
                        break;
                    default:
                        break;
                }

                return;
            }

            bool IsHigh = step.Oper.Contains("H");

            switch (Boards.Count)
            {
                case 1:
                    if (!Boards[0].Skip)
                    {
                        if (Boards[0].LevelChannels[channel].Samples.Where(x => x.Level != IsHigh).Count() > 0)
                        {
                            var failChannels = Boards[0].LevelChannels[channel].Samples.Where(x => x.Level != IsHigh).ToList();
                            for (int i = 0; i < failChannels.Count; i++)
                            {
                                Console.WriteLine("{0}->{1}", i, failChannels[i].Level);
                            }
                            step.ValueGet1 = "X";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "O";
                            step.Result1 = Step.Ok;
                        }
                    }
                    break;
                case 2:
                    if (!Boards[0].Skip)
                    {
                        if (Boards[0].LevelChannels[channel].Samples.Where(x => x.Level != IsHigh).Count() > 0)
                        {
                            step.ValueGet1 = "X";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "O";
                            step.Result1 = Step.Ok;
                        }
                    }

                    if (!Boards[1].Skip)
                    {
                        if (Boards[1].LevelChannels[channel].Samples.Where(x => x.Level != IsHigh).Count() > 0)
                        {
                            step.ValueGet2 = "X";
                            step.Result2 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet2 = "O";
                            step.Result2 = Step.Ok;
                        }
                    }
                    break;
                case 3:
                    if (!Boards[0].Skip)
                    {
                        if (Boards[0].LevelChannels[channel].Samples.Where(x => x.Level != IsHigh).Count() > 0)
                        {
                            step.ValueGet1 = "X";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "O";
                            step.Result1 = Step.Ok;
                        }
                    }

                    if (!Boards[1].Skip)
                    {
                        if (Boards[1].LevelChannels[channel].Samples.Where(x => x.Level != IsHigh).Count() > 0)
                        {
                            step.ValueGet2 = "X";
                            step.Result2 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet2 = "O";
                            step.Result2 = Step.Ok;
                        }
                    }
                    if (!Boards[2].Skip)
                    {
                        if (Boards[2].LevelChannels[channel].Samples.Where(x => x.Level != IsHigh).Count() > 0)
                        {
                            step.ValueGet3 = "X";
                            step.Result3 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet3 = "O";
                            step.Result3 = Step.Ok;
                        }
                    }
                    break;
                case 4:
                    if (!Boards[0].Skip)
                    {
                        if (Boards[0].LevelChannels[channel].Samples.Where(x => x.Level != IsHigh).Count() > 0)
                        {
                            step.ValueGet1 = "X";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "O";
                            step.Result1 = Step.Ok;
                        }
                    }

                    if (!Boards[1].Skip)
                    {
                        if (Boards[1].LevelChannels[channel].Samples.Where(x => x.Level != IsHigh).Count() > 0)
                        {
                            step.ValueGet2 = "X";
                            step.Result2 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet2 = "O";
                            step.Result2 = Step.Ok;
                        }
                    }
                    if (!Boards[2].Skip)
                    {
                        if (Boards[2].LevelChannels[channel].Samples.Where(x => x.Level != IsHigh).Count() > 0)
                        {
                            step.ValueGet3 = "X";
                            step.Result3 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet3 = "O";
                            step.Result3 = Step.Ok;
                        }
                    }
                    if (!Boards[3].Skip)
                    {
                        if (Boards[3].LevelChannels[channel].Samples.Where(x => x.Level != IsHigh).Count() > 0)
                        {
                            step.ValueGet4 = "X";
                            step.Result4 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet4 = "O";
                            step.Result4 = Step.Ok;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void LEC(Step step)
        {
            if (!LEVEL.SerialPort.Port.IsOpen)
            {
                functionsParameterError("Sys", step);
                return;
            }
            int channel = 0;
            try
            {
                channel = Int32.Parse(step.Condition1);
            }
            catch (Exception)
            {
                switch (Boards.Count)
                {
                    case 1:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }
                        break;
                    case 2:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Condition";
                            step.Result2 = Step.Ng;
                        }
                        break;
                    case 3:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {

                            step.ValueGet2 = "Condition";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Condition";
                            step.Result3 = Step.Ng;
                        }
                        break;
                    case 4:
                        if (!Boards[0].Skip)
                        {
                            step.ValueGet1 = "Condition";
                            step.Result1 = Step.Ng;
                        }

                        if (!Boards[1].Skip)
                        {
                            step.ValueGet2 = "Condition";
                            step.Result2 = Step.Ng;
                        }
                        if (!Boards[2].Skip)
                        {
                            step.ValueGet3 = "Condition";
                            step.Result3 = Step.Ng;
                        }
                        if (!Boards[3].Skip)
                        {
                            step.ValueGet4 = "Condition";
                            step.Result4 = Step.Ng;
                        }
                        break;
                    default:
                        break;
                }

                return;
            }

            if (channel >= Boards[0].LevelChannels.Count())
            {
                functionsParameterError("Condition", step);

                return;
            }

            if (step.Oper != "H" && step.Oper != "L")
                functionsParameterError("Oper", step);

            bool IsHigh = step.Oper == "H";

            double minValue;
            double maxValue;

            if (!Double.TryParse(step.Min, out minValue))
            {
                functionsParameterError("Min", step);
                return;
            }

            if (!Double.TryParse(step.Max, out maxValue))
            {
                if (step.Max == "L")
                {
                    maxValue = Double.MaxValue;
                }
                else
                {
                    functionsParameterError("Max", step);
                    return;
                }
            }
            int skipSamples = 0;
            if (step.Condition2 != null)
            {
                if (step.Condition2.Length >= 1)
                {
                    if (!Int32.TryParse(step.Condition2, out skipSamples))
                    {
                        functionsParameterError("Condition2", step);
                        return;
                    }
                }
            }
            if (!Double.TryParse(step.Max, out maxValue))
            {
                if (step.Max == "L")
                {
                    maxValue = Double.MaxValue;
                }
                else
                {
                    functionsParameterError("Max", step);
                    return;
                }
            }

            if (Boards.Count >= 1)
            {
                if (!Boards[0].Skip)
                {
                    int countA = Boards[0].LEVEL_COUNT(IsHigh, channel, skipSamples);
                    step.ValueGet1 = countA.ToString();
                    if (countA >= minValue && countA <= maxValue)
                    {
                        step.Result1 = Step.Ok;
                    }
                    else
                    {
                        step.Result1 = Step.Ng;
                    }
                }
            }

            if (Boards.Count >= 2)
            {
                if (!Boards[1].Skip)
                {
                    int countA = Boards[1].LEVEL_COUNT(IsHigh, channel, skipSamples);
                    step.ValueGet2 = countA.ToString();
                    if (countA >= minValue && countA <= maxValue)
                    {
                        step.Result2 = Step.Ok;
                    }
                    else
                    {
                        step.Result2 = Step.Ng;
                    }
                }
            }

            if (Boards.Count >= 3)
            {
                if (!Boards[2].Skip)
                {
                    int countA = Boards[2].LEVEL_COUNT(IsHigh, channel, skipSamples);
                    step.ValueGet3 = countA.ToString();
                    if (countA >= minValue && countA <= maxValue)
                    {
                        step.Result3 = Step.Ok;
                    }
                    else
                    {
                        step.Result3 = Step.Ng;
                    }
                }
            }

            if (Boards.Count >= 4)
            {
                if (!Boards[3].Skip)
                {
                    int countA = Boards[3].LEVEL_COUNT(IsHigh, channel, skipSamples);
                    step.ValueGet4 = countA.ToString();
                    if (countA >= minValue && countA <= maxValue)
                    {
                        step.Result4 = Step.Ok;
                    }
                    else
                    {
                        step.Result4 = Step.Ng;
                    }
                }
            }
        }

        private void DLY(Step step)
        {
            bool SetOK = false;
            if (int.TryParse(step.Oper, out int delayTime))
            {
                SetOK = true;
                if(delayTime > 100)
                {
                    int delay = 0;
                    while (delay + 100<= delayTime)
                    {
                        Task.Delay(90).Wait();
                        delay += 100;
                        step.ValueGet1 = delay.ToString();
                        step.ValueGet2 = delay.ToString();
                        step.ValueGet3 = delay.ToString();
                        step.ValueGet4 = delay.ToString();
                    }
                }
                else
                {
                    Task.Delay(delayTime).Wait();
                }
            }

            switch (Boards.Count)
            {
                case 1:
                    if (!Boards[0].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet1 = "Oper";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "exe";
                            step.Result1 = Step.Ok;
                        }
                    }
                    break;
                case 2:
                    if (!Boards[0].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet1 = "Oper";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "exe";
                            step.Result1 = Step.Ok;
                        }
                    }

                    if (!Boards[1].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet2 = "Oper";
                            step.Result2 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet2 = "exe";
                            step.Result2 = Step.Ok;
                        }
                    }
                    break;
                case 3:
                    if (!Boards[0].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet1 = "Oper";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "exe";
                            step.Result1 = Step.Ok;
                        }
                    }

                    if (!Boards[1].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet2 = "Oper";
                            step.Result2 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet2 = "exe";
                            step.Result2 = Step.Ok;
                        }
                    }
                    if (!Boards[2].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet3 = "Oper";
                            step.Result3 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet3 = "exe";
                            step.Result3 = Step.Ok;
                        }
                    }
                    break;
                case 4:
                    if (!Boards[0].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet1 = "Oper";
                            step.Result1 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet1 = "exe";
                            step.Result1 = Step.Ok;
                        }
                    }

                    if (!Boards[1].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet2 = "Oper";
                            step.Result2 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet2 = "exe";
                            step.Result2 = Step.Ok;
                        }
                    }
                    if (!Boards[2].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet3 = "Oper";
                            step.Result3 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet3 = "exe";
                            step.Result3 = Step.Ok;
                        }
                    }
                    if (!Boards[3].Skip)
                    {
                        if (!SetOK)
                        {
                            step.ValueGet4 = "Oper";
                            step.Result4 = Step.Ng;
                        }
                        else
                        {
                            step.ValueGet4 = "exe";
                            step.Result4 = Step.Ok;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void PWR(Step step)
        {
            bool IsON = step.Oper == "ON";
            bool Is220V = step.Condition1 == "220VAC";
            bool Is110V = step.Condition1 == "110VAC";

            if (!SYSTEM.System_Board.SerialPort.Port.IsOpen)
            {
                functionsParameterError("Sys", step);
                return;
            }

            switch (Boards.Count)
            {
                case 1:
                    if (!Boards[0].Skip)
                    {
                        if (Is110V)
                            SYSTEM.System_Board.MachineIO.AC110 = IsON;
                        if (Is220V)
                            SYSTEM.System_Board.MachineIO.AC220 = IsON;
                        SYSTEM.System_Board.SendControl();

                        step.ValueGet1 = "exe";
                        step.Result1 = Step.Ok;
                    }
                    break;
                case 2:
                    if (!Boards[0].Skip)
                    {
                        if (Is110V)
                            SYSTEM.System_Board.MachineIO.AC110 = IsON;
                        if (Is220V)
                            SYSTEM.System_Board.MachineIO.AC220 = IsON;

                        step.ValueGet1 = "exe";
                        step.Result1 = Step.Ok;
                    }

                    if (!Boards[1].Skip)
                    {
                        if (Is110V)
                            SYSTEM.System_Board.MachineIO.BC110 = IsON;
                        if (Is220V)
                            SYSTEM.System_Board.MachineIO.BC220 = IsON;

                        step.ValueGet2 = "exe";
                        step.Result2 = Step.Ok;
                    }

                    SYSTEM.System_Board.SendControl();
                    break;
                case 3:
                    if (!Boards[0].Skip)
                    {
                        if (Is110V)
                            SYSTEM.System_Board.MachineIO.AC110 = IsON;
                        if (Is220V)
                            SYSTEM.System_Board.MachineIO.AC220 = IsON;

                        step.ValueGet1 = "exe";
                        step.Result1 = Step.Ok;
                    }

                    if (!Boards[1].Skip)
                    {
                        if (Is110V)
                            SYSTEM.System_Board.MachineIO.BC110 = IsON;
                        if (Is220V)
                            SYSTEM.System_Board.MachineIO.BC220 = IsON;

                        step.ValueGet2 = "exe";
                        step.Result2 = Step.Ok;
                    }
                    if (!Boards[2].Skip)
                    {
                        if (Is110V)
                            SYSTEM.System_Board.MachineIO.AC110 = IsON;
                        if (Is220V)
                            SYSTEM.System_Board.MachineIO.AC220 = IsON;

                        step.ValueGet3 = "exe";
                        step.Result3 = Step.Ok;
                    }
                    SYSTEM.System_Board.SendControl();
                    break;
                case 4:
                    if (!Boards[0].Skip)
                    {
                        if (Is110V)
                            SYSTEM.System_Board.MachineIO.AC110 = IsON;
                        if (Is220V)
                            SYSTEM.System_Board.MachineIO.AC220 = IsON;

                        step.ValueGet1 = "exe";
                        step.Result1 = Step.Ok;
                    }

                    if (!Boards[1].Skip)
                    {
                        if (Is110V)
                            SYSTEM.System_Board.MachineIO.BC110 = IsON;
                        if (Is220V)
                            SYSTEM.System_Board.MachineIO.BC220 = IsON;

                        step.ValueGet2 = "exe";
                        step.Result2 = Step.Ok;
                    }
                    if (!Boards[2].Skip)
                    {
                        if (Is110V)
                            SYSTEM.System_Board.MachineIO.AC110 = IsON;
                        if (Is220V)
                            SYSTEM.System_Board.MachineIO.AC220 = IsON;

                        step.ValueGet3 = "exe";
                        step.Result3 = Step.Ok;
                    }

                    if (!Boards[4].Skip)
                    {
                        if (Is110V)
                            SYSTEM.System_Board.MachineIO.BC110 = IsON;
                        if (Is220V)
                            SYSTEM.System_Board.MachineIO.BC220 = IsON;

                        step.ValueGet4 = "exe";
                        step.Result4 = Step.Ok;
                    }
                    SYSTEM.System_Board.SendControl();
                    break;
                default:
                    break;
            }


        }

        private void DIS(Step step)
        {
            if (SYSTEM.System_Board.SerialPort.Port.IsOpen)
            {
                if (step.Condition1 == "ON")
                {
                    SYSTEM.System_Board.MachineIO.ADSC = true;
                    SYSTEM.System_Board.MachineIO.BDSC = true;
                }
                else
                {
                    SYSTEM.System_Board.MachineIO.ADSC = false;
                    SYSTEM.System_Board.MachineIO.BDSC = false;
                }

                SYSTEM.System_Board.SendControl();
                if (Boards.Count >= 1) if (!Boards[0].Skip) step.ValueGet1 = "exe";
                if (Boards.Count >= 2) if (!Boards[1].Skip) step.ValueGet2 = "exe";
                if (Boards.Count >= 3) if (!Boards[2].Skip) step.ValueGet3 = "exe";
                if (Boards.Count >= 4) if (!Boards[3].Skip) step.ValueGet4 = "exe";
            }
            else
            {
                functionsParameterError("sys", step);
            }
        }
        public void MOT(Step step)
        {
            double minValue = 0;
            double maxValue = 0;

            if (!Double.TryParse(step.Min, out minValue))
            {
                functionsParameterError("Min", step);
                return;
            }

            if (!Double.TryParse(step.Max, out maxValue))
            {
                if (step.Max == "L")
                {
                    maxValue = Double.MaxValue;
                }
                else
                {
                    functionsParameterError("Max", step);
                    return;
                }
            }
            if (!PowerMetter.SerialPort.Port.IsOpen)
            {
                functionsParameterError("sys", step);
            }

            if (step.Condition1 == "READ")
            {
                if (Boards.Count >= 1) if (!Boards[0].Skip) if (PowerMetter.Read('A')) step.ValueGet1 = "exe"; else { step.ValueGet1 = "sys"; step.Result1 = Step.Ng; }
                if (Boards.Count >= 2) if (!Boards[1].Skip) if (PowerMetter.Read('B')) step.ValueGet2 = "exe"; else { step.ValueGet2 = "sys"; step.Result2 = Step.Ng; }
                if (Boards.Count >= 3) if (!Boards[2].Skip) if (PowerMetter.Read('C')) step.ValueGet3 = "exe"; else { step.ValueGet3 = "sys"; step.Result3 = Step.Ng; }
                if (Boards.Count >= 4) if (!Boards[3].Skip) if (PowerMetter.Read('D')) step.ValueGet4 = "exe"; else { step.ValueGet4 = "sys"; step.Result4 = Step.Ng; }
            }
            else
            {
                //"READ", "CMP UU", "CMP UW", "CMP UV", "CMP UUW", "CMP UWV", "CMP UVU", "CMP IU", "CMP IW", "CMP IV"
                switch (step.Condition1)
                {
                    case "CMP UU":
                        CheckStepMinMax(step, PowerMetter.ValueHolders.Select(x => x.UU).ToArray(), minValue, maxValue);
                        break;
                    case "CMP UW":
                        CheckStepMinMax(step, PowerMetter.ValueHolders.Select(x => x.UW).ToArray(), minValue, maxValue);
                        break;
                    case "CMP UV":
                        CheckStepMinMax(step, PowerMetter.ValueHolders.Select(x => x.UV).ToArray(), minValue, maxValue);
                        break;

                    case "CMP UUW":
                        CheckStepMinMax(step, PowerMetter.ValueHolders.Select(x => x.UUW).ToArray(), minValue, maxValue);
                        break;
                    case "CMP UWV":
                        CheckStepMinMax(step, PowerMetter.ValueHolders.Select(x => x.UWV).ToArray(), minValue, maxValue);
                        break;
                    case "CMP UVU":
                        CheckStepMinMax(step, PowerMetter.ValueHolders.Select(x => x.UVU).ToArray(), minValue, maxValue);
                        break;

                    case "CMP IU":
                        CheckStepMinMax(step, PowerMetter.ValueHolders.Select(x => x.IU).ToArray(), minValue, maxValue);
                        break;
                    case "CMP IW":
                        CheckStepMinMax(step, PowerMetter.ValueHolders.Select(x => x.IW).ToArray(), minValue, maxValue);
                        break;
                    case "CMP IV":
                        CheckStepMinMax(step, PowerMetter.ValueHolders.Select(x => x.IV).ToArray(), minValue, maxValue);
                        break;
                    default:
                        functionsParameterError("condition", step);
                        break;
                }
            }
        }
        private void CheckStepMinMax(Step step, double[] values, double minValue, double maxValue)
        {
            if (Boards.Count >= 1)
            {
                if (!Boards[0].Skip)
                {
                    double countA = values[0];
                    step.ValueGet1 = countA.ToString("N3");
                    if (countA >= minValue && countA <= maxValue)
                    {
                        step.Result1 = Step.Ok;
                    }
                    else
                    {
                        step.Result1 = Step.Ng;
                    }
                }
            }

            if (Boards.Count >= 2)
            {
                if (!Boards[1].Skip)
                {
                    double countA = values[1];
                    step.ValueGet2 = countA.ToString("N3");
                    if (countA >= minValue && countA <= maxValue)
                    {
                        step.Result2 = Step.Ok;
                    }
                    else
                    {
                        step.Result2 = Step.Ng;
                    }
                }
            }

            if (Boards.Count >= 3)
            {
                if (!Boards[2].Skip)
                {
                    double countA = values[2];
                    step.ValueGet3 = countA.ToString("N3");
                    if (countA >= minValue && countA <= maxValue)
                    {
                        step.Result3 = Step.Ok;
                    }
                    else
                    {
                        step.Result3 = Step.Ng;
                    }
                }
            }

            if (Boards.Count >= 4)
            {
                if (!Boards[3].Skip)
                {
                    double countA = values[3];
                    step.ValueGet4 = countA.ToString("N3");
                    if (countA >= minValue && countA <= maxValue)
                    {
                        step.Result4 = Step.Ok;
                    }
                    else
                    {
                        step.Result4 = Step.Ng;
                    }
                }
            }

        }
        public void END(Step step)
        {
            SYSTEM.System_Board.PowerRelease();
            RELAY.Card.Release();
            Solenoid.Card.Release();
            MuxCard.Card.ReleaseChannels();
        }

        private void functionsParameterError(string nameOfFunc, Step step)
        {
            switch (Boards.Count)
            {
                case 1:
                    if (!Boards[0].Skip)
                    {
                        step.ValueGet1 = nameOfFunc;
                        step.Result1 = Step.Ng;
                    }
                    break;
                case 2:
                    if (!Boards[0].Skip)
                    {
                        step.ValueGet1 = nameOfFunc;
                        step.Result1 = Step.Ng;
                    }

                    if (!Boards[1].Skip)
                    {
                        step.ValueGet2 = nameOfFunc;
                        step.Result2 = Step.Ng;
                    }
                    break;
                case 3:
                    if (!Boards[0].Skip)
                    {
                        step.ValueGet1 = nameOfFunc;
                        step.Result1 = Step.Ng;
                    }

                    if (!Boards[1].Skip)
                    {

                        step.ValueGet2 = nameOfFunc;
                        step.Result2 = Step.Ng;
                    }
                    if (!Boards[2].Skip)
                    {
                        step.ValueGet3 = nameOfFunc;
                        step.Result3 = Step.Ng;
                    }
                    break;
                case 4:
                    if (!Boards[0].Skip)
                    {
                        step.ValueGet1 = nameOfFunc;
                        step.Result1 = Step.Ng;
                    }

                    if (!Boards[1].Skip)
                    {
                        step.ValueGet2 = nameOfFunc;
                        step.Result2 = Step.Ng;
                    }
                    if (!Boards[2].Skip)
                    {
                        step.ValueGet3 = nameOfFunc;
                        step.Result3 = Step.Ng;
                    }
                    if (!Boards[3].Skip)
                    {
                        step.ValueGet4 = nameOfFunc;
                        step.Result4 = Step.Ng;
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

        private bool StepTestResult(Step step)
        {
            bool isOk = true;
            switch (Boards.Count)
            {
                case 1:
                    if (!Boards[0].Skip) isOk = isOk && step.Result1 != Step.Ng;
                    return isOk;
                case 2:
                    if (!Boards[0].Skip) isOk = isOk && step.Result1 != Step.Ng;
                    if (!Boards[1].Skip) isOk = isOk && step.Result2 != Step.Ng;
                    return isOk;
                case 3:
                    if (!Boards[0].Skip) isOk = isOk && step.Result1 != Step.Ng;
                    if (!Boards[1].Skip) isOk = isOk && step.Result2 != Step.Ng;
                    if (!Boards[2].Skip) isOk = isOk && step.Result3 != Step.Ng;
                    return isOk;
                case 4:
                    if (!Boards[0].Skip) isOk = isOk && step.Result1 != Step.Ng;
                    if (!Boards[1].Skip) isOk = isOk && step.Result2 != Step.Ng;
                    if (!Boards[2].Skip) isOk = isOk && step.Result3 != Step.Ng;
                    if (!Boards[3].Skip) isOk = isOk && step.Result4 != Step.Ng;
                    return isOk;
                default:
                    return false;
            }
        }

    }
}
