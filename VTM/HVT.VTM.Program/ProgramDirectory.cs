using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HVT.VTM.Base;

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
           await Task.Run(() => { MachineFolder.TryCreatFolderMap();});
        }
    }
}
