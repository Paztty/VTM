using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HVT.VTM.Directorys;
using HVT.VTM.AppSetting;
using HVT.VTM.Core;
using HVT.StandantLocalUsers;

namespace HVT.VTM.Program
{
    public partial class Program
    {
        public Users users = new Users();

        public bool CheckPassWord(string PasswordInput, Users.Permissions permisssions)
        {
            return users.Password[(int)permisssions] == PasswordInput;
        }





    }
}
