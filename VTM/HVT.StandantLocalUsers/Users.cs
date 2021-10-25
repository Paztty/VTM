using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.StandantLocalUsers
{
    public class Users
    {
        public enum Permissions
        {
            None = 0,
            OP = 1,
            Tech = 2,
            Manager = 3,
        }

        public string[] Password =
        {
                "",
                "1",
                "1234",
                "dyelc123"
        };
    }
}
