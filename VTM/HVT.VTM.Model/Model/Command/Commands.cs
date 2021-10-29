using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.VTM.Base
{
    public class Command
    {

        private CMDs cmd = CMDs.PWR;

        public List<CommandDescriptions> CMD = new List<CommandDescriptions>
        {
            new CommandDescriptions()
                                    {
                                        CMD = CMDs.PWR,
                                        Condition1 = "KIND",
                                        Oper = "STATUS",
                                        Description = "Selected application of power ON/OFF"
                                    }
        };

        public static List<CommandDescriptions> Commands = new List<CommandDescriptions>
        {
            new CommandDescriptions(){
            CMD = CMDs.PWR,
            Condition1 = "KIND",
            Oper = "STATUS",
            Description = "Selected application of power ON/OFF"
            },

            new CommandDescriptions(){
            CMD = CMDs.DLY,
            Oper = "TIME",
            Description = "Waits the progress for the time(ms)."
            },

            new CommandDescriptions()
            {
            CMD = CMDs.GEN,
            Condition1 = "FREQ",
            Oper = "CH",
            Description = "Control the Device Load."
            },

            new CommandDescriptions()
            {
            CMD = CMDs.LOD,
            Condition1 = "KIND",
            Oper = "STATUS",
            Description = "Controls the output of Channel which Generator B/d is designated.\r\n Frequency range : 0~100kHz"
            },


            new CommandDescriptions()
            {
            CMD = CMDs.RLY,
            Condition1 = "CH",
            Oper = "STATUS",
            Description = "Changes the condition of Channel which relay B/d is designated.\r"
                        + "The case which will use Channel at multiple, '/', '~' With divides. (ex. 3/6/10~12)\r"
                        + "The status uses ON/OFF or ON time(ms).\r"
                        + "After relay operating, the delay time(ms) which is set waits.\r"
            },

            new CommandDescriptions()
            {
            CMD = CMDs.FRY,
            Condition1 = "CH",
            Oper = "STATUS",
            Description = "Changes the condition of Channel which Fixture relay B/d is designated.\r"
                        + "The case which will use Channel at multiple, '/', '~' With divides. (ex. 3/6/10~12)\r"
                        + "The status uses ON/OFF or ON time(ms).\r"
                        + "After relay operating, the delay time(ms) which is set waits.\r"
            },

            new CommandDescriptions()
            {
            CMD = CMDs.MAK,
            Condition1 = "TEXT",
            Description = "Remark step"
            },

        };
    }
}
