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
