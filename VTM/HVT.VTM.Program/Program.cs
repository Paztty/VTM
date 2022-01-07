using HVT.VTM.Base;
using HVT.Utility;

namespace HVT.VTM.Program
{
    public partial class Program
    {
        public AppSettingParam appSetting = new AppSettingParam();

        public void Machine_Init()
        {
            appSetting = Extensions.OpenFromFile<AppSettingParam>("Config.cfg");
            appSetting = appSetting ?? new AppSettingParam();
        }
    }
}
