using HVT.VTM.Base;
using Microsoft.Win32;
using System;
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
        //event 
        public event EventHandler ModelChangeEvent;
        private void ModelChange()
        {
            ModelChangeEvent?.Invoke(this, EventArgs.Empty);
        }

        // Enum and constance
        public enum RunTestState
        {
            WAIT,
            READY,
            TESTTING,
            STOP,
            Pause,
            GOOD,
            FAIL,
            BUSY
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
            if (RootModel.Path == null || RootModel.Name == null)
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
            }
        }

        public void SaveModelAs()
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "VTM model files (*.vmdl)|*.vmdl";
            saveFile.Title = "Save VTM model file.";
            if (saveFile.ShowDialog() == true)
            {
                RootModel.Path = saveFile.FileName;
                HVT.Utility.Extensions.SaveToFile(RootModel, saveFile.FileName);
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
                //var fileInfor = new FileInfo(openFile.FileName);
                string modelStr = System.IO.File.ReadAllText(openFile.FileName);
                string modelString = HVT.Utility.Extensions.Decoder(modelStr, System.Text.Encoding.UTF7);
                RootModel = HVT.Utility.Extensions.ConvertFromJson<Model>(modelString);
                //RootModel.Steps.Clear();
                //foreach (var step in modelLoaded.Steps)
                //{
                //    RootModel.Steps.Add(HVT.Utility.Extensions.Clone(step));
                //}
                //RootModel.Naming = modelLoaded.Naming;
                //RootModel.Path = modelLoaded.Path;
                //RootModel.Name = modelLoaded.Name;
                ModelChange();
                UpdateDataAfterLoad();
            }
        }

        public void UpdateDataAfterLoad()
        {
            LoadNamingFromModel();
            RootModel.ReplaceComponent(DrawingCanvas, DisplayCanvas);
            Console.WriteLine("Load model done");
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
        Task RUNTEST;

        public void RUN_TEST()
        {
            if (RUNTEST == null)
            {
                RUNTEST = new Task(RunTest);
                RUNTEST.Start();
            }
            else if (RUNTEST.Status != TaskStatus.Running)
            {
                RUNTEST = new Task(RunTest);
                RUNTEST.Start();
            }
        }


        public void RunTest()
        {
            foreach (var item in RootModel.Steps)
            {
                item.ValueGet1 = "";
                item.ValueGet2 = "";
                item.ValueGet3 = "";
                item.ValueGet4 = "";
                item.Result1 = Step.DontCare;
                item.Result2 = Step.DontCare;
                item.Result3 = Step.DontCare;
                item.Result4 = Step.DontCare;
            }

            StepTesting = 0;
            IsTestting = true;
            IsTestting = true;
            var rand = new Random(100);
            StepTesting = 0;
            var Steps = RootModel.Steps;
            while (true)
            {
                switch (TestState)
                {
                    case RunTestState.WAIT:
                        TestState = RunTestState.READY;
                        break;
                    case RunTestState.TESTTING:
                        if (StepTesting == Steps.Count)
                        {
                            TestState = RunTestState.GOOD;
                        }
                        else
                        {
                            IsTestting = true;
                            StepTestChange?.Invoke(StepTesting, null);
                            Console.WriteLine(StepTesting + ":" + Steps[StepTesting].CMD + " " + Steps[StepTesting].Min_Max);
                            var stepTest = Steps[StepTesting];
                            if (stepTest != null)
                            {
                                RUN_FUNCTION_TEST(stepTest);
                            }

                            StepTesting++;

                            if (StepTesting == Steps.Count)
                            {
                                TestState = RunTestState.GOOD;
                            }
                        }
                        break;
                    case RunTestState.Pause:
                        break;
                    case RunTestState.STOP:
                        IsTestting = false;
                        StepTesting = 0;
                        StepTestChange?.Invoke(StepTesting, null);
                        TestState = RunTestState.Pause;
                        break;
                    case RunTestState.GOOD:
                        IsTestting = false;
                        TestRunFinish?.Invoke("Good", null);
                        Thread.Sleep(rand.Next(1000));
                        TestState = RunTestState.READY;
                        break;
                    case RunTestState.FAIL:
                        IsTestting = false;
                        TestRunFinish?.Invoke("Fail", null);
                        Thread.Sleep(rand.Next(1000));
                        TestState = RunTestState.READY;
                        break;
                    case RunTestState.READY:
                        goto Finish;
                    default:
                        break;
                }
            }
        Finish: IsTestting = false;
        }

        public void RUN_FUNCTION_TEST(Step step)
        {
            step.ValueGet1 = "exe";
            step.ValueGet2 = "exe";
            step.ValueGet3 = "exe";
            step.ValueGet4 = "exe";

            step.Result1 = Step.DontCare;
            step.Result2 = Step.DontCare;
            step.Result3 = Step.DontCare;
            step.Result4 = Step.DontCare;
            switch (step.cmd)
            {
                case CMDs.NON:
                    break;
                case CMDs.PWR:
                    break;
                case CMDs.DLY:
                    if (int.TryParse(step.Oper, out int delayTime))
                    {
                        Thread.Sleep(delayTime);
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
                    break;
                case CMDs.DCV:
                    break;
                case CMDs.FRQ:
                    break;
                case CMDs.RES:
                    break;
                case CMDs.URD:
                    break;
                case CMDs.UTN:
                    break;
                case CMDs.UTX:
                    break;
                case CMDs.UCN:
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
                    step.ValueGet1 = RootModel.GLEDs[0].CalculatorOutputString;
                    step.ValueGet2 = RootModel.GLEDs[1].CalculatorOutputString;
                    step.ValueGet3 = RootModel.GLEDs[2].CalculatorOutputString;
                    step.ValueGet4 = RootModel.GLEDs[3].CalculatorOutputString;
                    break;
                case CMDs.FND:
                    ReadFND(step);
                    break;
                case CMDs.LED:
                    step.ValueGet1 = RootModel.LEDs[0].CalculatorOutputString;
                    step.ValueGet2 = RootModel.LEDs[1].CalculatorOutputString;
                    step.ValueGet3 = RootModel.LEDs[2].CalculatorOutputString;
                    step.ValueGet4 = RootModel.LEDs[3].CalculatorOutputString;
                    break;
                case CMDs.LCD:
                    ReadLCD(step);
                    break;
                case CMDs.PCB:
                    break;
                default:

                    break;
            }





        }




        #region Functions Code

        public void ReadLCD(Step step)
        {
            step.Result = false;
            StartUpdateLCD();
            if (int.TryParse(step.Condition2, out int scanTime))
            {
                DateTime start = DateTime.Now;
                while (DateTime.Now.Subtract(start).TotalMilliseconds < scanTime)
                {
                    step.ValueGet1 = step.Result1 != Step.Ok ? RootModel.LCDs[0].Data : step.ValueGet1;
                    step.ValueGet2 = step.Result2 != Step.Ok ? RootModel.LCDs[1].Data : step.ValueGet1;
                    step.ValueGet3 = step.Result3 != Step.Ok ? RootModel.LCDs[2].Data : step.ValueGet1;
                    step.ValueGet4 = step.Result4 != Step.Ok ? RootModel.LCDs[3].Data : step.ValueGet1;

                    step.Result1 = step.ValueGet1 == step.Oper ? Step.Ok : Step.Ng;
                    step.Result2 = step.ValueGet2 == step.Oper ? Step.Ok : Step.Ng;
                    step.Result3 = step.ValueGet3 == step.Oper ? Step.Ok : Step.Ng;
                    step.Result4 = step.ValueGet4 == step.Oper ? Step.Ok : Step.Ng;

                    if (step.Result1 == Step.Ok && step.Result2 == Step.Ok && step.Result3 == Step.Ok && step.Result4 == Step.Ok)
                    {
                        step.Result = true;
                        break;
                    }
                    Task.Delay(scanTime / 3);
                    StartUpdateLCD();
                }
            }
            else
            {
                step.ValueGet1 = RootModel.LCDs[0].Data;
                step.ValueGet2 = RootModel.LCDs[1].Data;
                step.ValueGet3 = RootModel.LCDs[2].Data;
                step.ValueGet4 = RootModel.LCDs[3].Data;

                step.Result1 = step.ValueGet1 == step.Oper ? Step.Ok : Step.Ng;
                step.Result2 = step.ValueGet2 == step.Oper ? Step.Ok : Step.Ng;
                step.Result3 = step.ValueGet3 == step.Oper ? Step.Ok : Step.Ng;
                step.Result4 = step.ValueGet4 == step.Oper ? Step.Ok : Step.Ng;

                if (step.Result1 == Step.Ok && step.Result2 == Step.Ok && step.Result3 == Step.Ok && step.Result4 == Step.Ok)
                {
                    step.Result = true;
                }
            }

        }

        public void ReadFND(Step step)
        {
            step.Result = false;
            if (int.TryParse(step.Condition2, out int scanTime))
            {
                DateTime start = DateTime.Now;
                while (DateTime.Now.Subtract(start).TotalMilliseconds < scanTime)
                {
                    step.ValueGet1 = step.Result1 != Step.Ok ? RootModel.FNDs[0].Data : step.ValueGet1;
                    step.ValueGet2 = step.Result2 != Step.Ok ? RootModel.FNDs[1].Data : step.ValueGet1;
                    step.ValueGet3 = step.Result3 != Step.Ok ? RootModel.FNDs[2].Data : step.ValueGet1;
                    step.ValueGet4 = step.Result4 != Step.Ok ? RootModel.FNDs[3].Data : step.ValueGet1;

                    step.Result1 = step.ValueGet1 == step.Oper ? Step.Ok : Step.Ng;
                    step.Result2 = step.ValueGet2 == step.Oper ? Step.Ok : Step.Ng;
                    step.Result3 = step.ValueGet3 == step.Oper ? Step.Ok : Step.Ng;
                    step.Result4 = step.ValueGet4 == step.Oper ? Step.Ok : Step.Ng;

                    if (step.Result1 == Step.Ok && step.Result2 == Step.Ok && step.Result3 == Step.Ok && step.Result4 == Step.Ok)
                    {
                        step.Result = true;
                        break;
                    }
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

                if (step.Result1 == Step.Ok && step.Result2 == Step.Ok && step.Result3 == Step.Ok && step.Result4 == Step.Ok)
                {
                    step.Result = true;
                }
            }

        }
        #endregion

    }
}
