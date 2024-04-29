using HVT.VTM.Base;
using HVT.Utility;

namespace HVT.VTM.Program
{
    public partial class Program
    {
        public StepViewer StepViewer = new StepViewer();
        public async void Machine_Init()
        {
            CreatAppFolder();
            LoadAppSetting();
            CheckComnunication();
            EscapTimer.Elapsed += EscapTimer_Elapsed;
            EscapTimer.Stop();
            await System.Threading.Tasks.Task.Delay(50);
        }
    }
}
