using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.VTM.Program
{
    public class SystemAccess
    {
        public bool UseAdminPass { get; set; } = true;
        public string AdminPass { get; set; } = "1234af";

        public bool UseOperator { get; set; } = true;
        public string OperationPass { get; set; } = "OP";
    }
}
