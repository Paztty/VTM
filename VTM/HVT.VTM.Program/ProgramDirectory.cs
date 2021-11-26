using HVT.VTM.Base;
using System.Threading.Tasks;

namespace HVT.VTM.Program
{
    /// <summary>
    /// class for VTM directory manager
    /// </summary>
    public partial class Program
    {
        public static FolderMap MachineFolder = new FolderMap();
        public async void CreatMachineFolder()
        {
            await Task.Run(() => { MachineFolder.TryCreatFolderMap(); });
        }
    }
}
