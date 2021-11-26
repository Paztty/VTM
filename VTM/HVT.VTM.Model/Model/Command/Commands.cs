using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HVT.VTM.Base
{
    public class Command
    {
        private CMDs privateCMD;
        public CMDs cmd
        {
            get { return privateCMD; }
            set
            {
                if (value != privateCMD)
                {
                    privateCMD = value;
                    CMD.Clear();
                    CMD.Add(Commands.SingleOrDefault(x => x.CMD == privateCMD));
                }
            }
        }

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

        public static ObservableCollection<CommandDescriptions> Commands = new ObservableCollection<CommandDescriptions>
        {
            new CommandDescriptions()
            {
            CMD = CMDs.NON,
            },

            new CommandDescriptions(){
            CMD = CMDs.PWR,
            Condition1 = "KIND",
            IsListCondition1 = true,
            Condition1List = CommandDescriptions.CommandMode_PowerKIND,
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

            //new CommandDescriptions()
            //{
            //CMD = CMDs.FRY,
            //Condition1 = "CH",
            //Oper = "STATUS",
            //Description = "Changes the condition of Channel which Fixture relay B/d is designated.\r"
            //            + "The case which will use Channel at multiple, '/', '~' With divides. (ex. 3/6/10~12)\r"
            //            + "The status uses ON/OFF or ON time(ms).\r"
            //            + "After relay operating, the delay time(ms) which is set waits.\r"
            //},

            new CommandDescriptions()
            {
            CMD = CMDs.MAK,
            Condition1 = "TEXT",
            Description = "Remark step"
            },

            new CommandDescriptions()
            {
            CMD = CMDs.DIS,
            Description = "Discharge command.\r\n" +
                          "The discharge which follows in discharge set executes.\r\n" +
                          "Will not be able to use from Power-ON conditions.\r\n"
            },

            new CommandDescriptions()
            {
            CMD = CMDs.END,
            Description = "Stops a test progress.\r\n"
            },

            new CommandDescriptions()
            {
            CMD = CMDs.ACV,
            Condition1 = "MUX(/RELAY)",
            Oper = "RANGE",
            IsListOper = true,
            OperList = CommandDescriptions.CommandMode_DMM_ACVrange,
            Condition2 = "RESOL",
            IsListCondition2 = true,
            Condition2List = CommandDescriptions.CommandMode_DMMresol,
            Spect = "SPEC",
            Min = "MIN",
            Max = "MAX",
            Mode = "MODE",
            IsListMode = true,
            ModeList = CommandDescriptions.CommandMode_DMM,
            Count = "COUNT/TIME",
            Description =" Uses DMM and measures AC voltage.\r\n"+
                        " The 'Range' selects DMM voltage measuring range. The 'M No' uses when storing a measurement result in the memory. There is a 'spec' and after comparison deciding, stores in the memory.\r\n"+
                        " mode : SPEC, CONT, MIN, AVR, MAX.\r\n"+
                        " SPEC : When the data which hits to an min-max limit scope comes in the within the set number of times just. (range : 1~1000 EA)\r\n"+
                        " CONT : As the set number of times continuously in min-max limit scope and when comes just.(range : 1~30 EA)\r\n"+
                        " MIN, AVR, MAX : For measurement time the minimum value which is measured, mean value and maximum extraction. (range : 300 ~ 10000ms)\r\n"

            },

            new CommandDescriptions()
            {
            CMD = CMDs.DCV,
            Condition1 = "MUX(/RELAY)",
            Oper = "RANGE",
            IsListOper = true,
            OperList= CommandDescriptions.CommandMode_DMM_DCVrange,
            Condition2 = "RESOL",
            IsListCondition2 = true,
            Condition2List= CommandDescriptions.CommandMode_DMMresol,
            Spect = "SPEC",
            Min = "MIN",
            Max = "MAX",
            Mode = "MODE",
            IsListMode = true,
            ModeList = CommandDescriptions.CommandMode_DMM,
            Count = "COUNT/TIME",
            Description =" Uses DMM and measures DC voltage.\r\n"+
                        " The 'Range' selects DMM voltage measuring range. The 'M No' uses when storing a measurement result in the memory. There is a 'spec' and after comparison deciding, stores in the memory.\r\n"+
                        " mode : SPEC, CONT, MIN, AVR, MAX.\r\n"+
                        " SPEC : When the data which hits to an min-max limit scope comes in the within the set number of times just. (range : 1~1000 EA)\r\n"+
                        " CONT : As the set number of times continuously in min-max limit scope and when comes just.(range : 1~30 EA)\r\n"+
                        " MIN, AVR, MAX : For measurement time the minimum value which is measured, mean value and maximum extraction. (range : 300 ~ 10000ms)\r\n"

            },

            new CommandDescriptions()
            {
            CMD = CMDs.FRQ,
            Condition1 = "MUX(/RELAY)",
            Oper = "RANGE",
            IsListOper = true,
            OperList= CommandDescriptions.CommandMode_DMM_ACVrange,
            Condition2 = "RESOL",
            IsListCondition2 = true,
            Condition2List= CommandDescriptions.CommandMode_DMMresol,
            Spect = "SPEC",
            Min = "MIN",
            Max = "MAX",
            Mode = "MODE",
            IsListMode = true,
            ModeList = CommandDescriptions.CommandMode_DMM,
            Count = "COUNT/TIME",
            Description =" Uses DMM and measures Frequency value.\r\n"+
                        " The 'Range' selects DMM voltage measuring range. The 'M No' uses when storing a measurement result in the memory. There is a 'spec' and after comparison deciding, stores in the memory.\r\n"+
                        " mode : SPEC, CONT, MIN, AVR, MAX.\r\n"+
                        " SPEC : When the data which hits to an min-max limit scope comes in the within the set number of times just. (range : 1~1000 EA)\r\n"+
                        " CONT : As the set number of times continuously in min-max limit scope and when comes just.(range : 1~30 EA)\r\n"+
                        " MIN, AVR, MAX : For measurement time the minimum value which is measured, mean value and maximum extraction. (range : 300 ~ 10000ms)\r\n"

            },

            new CommandDescriptions()
            {
            CMD = CMDs.RES,
            Condition1 = "MUX(/RELAY)",
            Oper = "RANGE",
            IsListOper = true,
            OperList = CommandDescriptions.CommandMode_DMM_RESrange,
            IsListCondition2 = true,
            Condition2List= CommandDescriptions.CommandMode_DMMresol,
            Condition2 = "RESOL",
            Spect = "SPEC",
            Min = "MIN",
            Max = "MAX",
            Mode = "MODE",
            IsListMode = true,
            ModeList = CommandDescriptions.CommandMode_DMM,
            Count = "COUNT/TIME",
            Description =" Uses DMM and measures Resistance value.\r\n"+
                        " The 'Range' selects DMM voltage measuring range. The 'M No' uses when storing a measurement result in the memory. There is a 'spec' and after comparison deciding, stores in the memory.\r\n"+
                        " mode : SPEC, CONT, MIN, AVR, MAX.\r\n"+
                        " SPEC : When the data which hits to an min-max limit scope comes in the within the set number of times just. (range : 1~1000 EA)\r\n"+
                        " CONT : As the set number of times continuously in min-max limit scope and when comes just.(range : 1~30 EA)\r\n"+
                        " MIN, AVR, MAX : For measurement time the minimum value which is measured, mean value and maximum extraction. (range : 300 ~ 10000ms)\r\n"

            },

            new CommandDescriptions()
            {
            CMD = CMDs.URD,
            Oper = "PORT",
            IsListOper = true,
            OperList= CommandDescriptions.CommandMode_UUT_Port,
            Condition2 = "COUNT",
            Description ="In the UUT port, data input of the port (P1/P2) which is set is a buffering.\r\n"+
                         "When inputs a 'Count', as the frame of the count which is set the bay restrictively is a buffering"
            },

            new CommandDescriptions()
            {
            CMD = CMDs.UTN,
            Condition1 = "NAMING",
            IsListCondition1 = true,
            Condition1List = CommandDescriptions.TXnaming,
            Oper = "PORT",
            IsListOper = true,
            OperList= CommandDescriptions.CommandMode_UUT_Port,
            Condition2 = "LIMIT TIME",
            Spect = "BUFFER",
            Min = "TRY COUNT",
            Mode = "MODE",
            IsListMode = true,
            ModeList= CommandDescriptions.CommandMode_UUT,
            Count = "SET TIME",
            Description ="In the UUT port, in the port (P1/P2) which is set outputs a Tx Naming data. The contents of data uses Hex format. there is not a data and uses the transmission memory buffer.\r\n"+
                        "mode : NORMAL/R-WAIT/SEND-R/TIMER  (Buffer only  R-WAIT or SEND-R modes will be able to use)\r\n"+
                        "NORMAL : Any restriction without data rightly transmission.\r\n"+
                        "R-WAIT : When the command execute is accomplished from, restrictive time (ms) periods the case which will be reception particulars of the corresponding pots, set time (ms) after, transmits a data.\r\n"+
                        "SEND-R : After Data transmitting, restrictive time (ms) periods confirms the data reception and delay for set time (ms). (Use the Try Cnt),  CHANGE-R : after UTD Data changing, restrictive time (ms) periods confirms the data reception and delay for set time (ms).\r\n"+
                        "TIMER  : Transmits a data at set time(ms) period. When cancels a transmission, sets a data in the blank Data and accomplishes or Set time is 0.\r\n"

            },

            new CommandDescriptions()
            {
            CMD = CMDs.UTX,
            Condition1 = "DATA (HEX)",
            Oper = "PORT",
            IsListOper = true,
            OperList= CommandDescriptions.CommandMode_UUT_Port,
            Condition2 = "LIMIT TIME",
            Spect = "BUFFER",
            Min = "TRY COUNT",
            Mode = "MODE",
            IsListMode = true,
            ModeList= CommandDescriptions.CommandMode_UUT,
            Count = "SET TIME",
            Description ="In the UUT port, in the port (P1/P2) which is set outputs a Tx Naming data. The contents of data uses Hex format. there is not a data and uses the transmission memory buffer.\r\n"+
                        "mode : NORMAL/R-WAIT/SEND-R/TIMER  (Buffer only  R-WAIT or SEND-R modes will be able to use)\r\n"+
                        "NORMAL : Any restriction without data rightly transmission.\r\n"+
                        "R-WAIT : When the command execute is accomplished from, restrictive time (ms) periods the case which will be reception particulars of the corresponding pots, set time (ms) after, transmits a data.\r\n"+
                        "SEND-R : After Data transmitting, restrictive time (ms) periods confirms the data reception and delay for set time (ms). (Use the Try Cnt),  CHANGE-R : after UTD Data changing, restrictive time (ms) periods confirms the data reception and delay for set time (ms).\r\n"+
                        "TIMER  : Transmits a data at set time(ms) period. When cancels a transmission, sets a data in the blank Data and accomplishes or Set time is 0.\r\n"

            },

            new CommandDescriptions()
            {
            CMD = CMDs.UCN,
            Condition1 = "RX DATA NAME",
            IsListCondition1 = true,
            Condition1List= CommandDescriptions.RXnaming,
            Oper = "PORT",
            IsListOper = true,
            OperList = CommandDescriptions.CommandMode_UUT_Port,
            Condition2 = "TX DATA NAME",
            IsListCondition2 = true,
            Condition2List= CommandDescriptions.TXnaming,
            Spect = "SPEC",
            Min = "MIN",
            Max = "MAX",
            Mode = "MODE",
            IsListMode = true,
            ModeList = CommandDescriptions.CommandMode_UUT,
            Count = "SET TIME",
            Description ="In the UUT port, data which has become the bufferring of the port which is set, seeks the Rx Data Name which same to a corresponding condition and compares, in Memory substitutes.\r\n"+
                            "Input the min & max spec with Rx Data Name table type format. (HEX , DEC or ASC)\r\n"+
                            "ASCII Rx Naming type is selected if the data input is compared to the 'spec'.  If the 'Tx Data Name' is input to the naming After the comparison, the transmission data is transmitted.\r\n"+
                            "Mode - NORMAL : recieved data compare.\r\n"+
                            "WAIT : To collect data in the same format, then compare the Rx Data Name\r\n"+
                            "W-DATA : Collecting data with the Rx Data Name format in the same upper and lower limit comparison data.\r\n"

            },

            new CommandDescriptions()
            {
            CMD = CMDs.UCP,
            Condition1 = "LOC-DATA",
            Oper = "PORT",
            IsListOper = true,
            OperList = CommandDescriptions.CommandMode_UUT_Port,
            Condition2 = "GET LOC",
            Spect = "SPEC",
            Min = "MIN",
            Max = "MAX",
            Description ="In the UUT port, data which has become the bufferring of the port which is set, seeks the data which same to a corresponding condition and compares, in Memory substitutes.\r\n"+
                            "The case which will use Channel at multiple, '/'.  (ex. 1-E5/2-00/3-11)\r\n"+
                            "When the buffer data where the data of condition agrees exists, the min-max limit and compares the data of collection location, substitutes in Memory.\r\n"+
                            "When data comparing, the case which will use a collection location at multiple,'/','~' with divides. (ex. 3/6/10~12)\r\n"+
                            "When substituting in Memory, will not be able to use a collection location at multiple."
            },

            new CommandDescriptions()
            {
            CMD = CMDs.STL,
            Oper = "SAMPLING",
            Condition2 = "LIMIT",
            Description ="Starts the incoming data collection of Level B/d. (Max 4000 count)\r\n"+
                            "The sampling speed sets with normality 100ms.\r\n"+
                            "When does not set a Limit-count, when EDL command are executed until, collects a data.\r\n"
            },

            new CommandDescriptions()
            {
            CMD = CMDs.EDL,
            Description ="Stop the incoming data collection of Level B/d.\r\n"
            },

            new CommandDescriptions()
            {
            CMD = CMDs.LCC,
            Condition1 = "CH",
            Oper = "Status",
            IsListOper = true,
            OperList= new List<string>(){"H", "L" },
            Condition2 = "SKIP",
            Description ="In incoming data of Level B/d, was the condition of data of corresponding Channel maintained compares in continuation.\r\n"+
                            "The Status selects H or L.\r\n"+
                            "The case which will use Channel at multiple, '/' With divides. (ex. 1/3/6)\r\n"+
                            "In incoming data, the number of skip-count which is set is excepted from the comparative object"
            },

            new CommandDescriptions()
            {
            CMD = CMDs.LEC,
            Condition1 = "CH",
            Oper = "Status",
            IsListOper = true,
            OperList= new List<string>(){"H", "L" },
            Condition2 = "SKIP",
            Spect = "SPEC",
            Min = "MIN",
            Max = "MAX",
            Description ="In incoming data of Level B/d, was the condition of data of corresponding Channel maintained compares in continuation.\r\n"+
                            "The Status selects H or L.\r\n"+
                            "The case which will use Channel at multiple, '/' With divides. (ex. 1/3/6)\r\n"+
                            "In incoming data, the number of skip-count which is set is excepted from the comparative object"
            },

            new CommandDescriptions()
            {
            CMD = CMDs.CAL,
            Condition1 = "CALCULATION",
            Oper = "MODE",
            IsListOper = true,
            OperList= new List<string>(){"REAL", "HEX" },
            Spect = "SPEC",
            Min = "MIN",
            Max = "MAX",
            Description ="After operation calculating, the min-max limit and compares, operation input in the memory.\r\n"+
                            "Mode : REAL or HEX\r\n"+
                            "When substituting in the memory, there is a 'spec' and after operation calculating, the min-max limit and comparison after deciding in the memory, substitutes.\r\n"+
                            "Calculation : numerical '+','-','*','/' ,absolute value '_',  logical '&','|','L','R' the single operation bay will be able to use.\r\n"+
                            "When use the logical operation AND(&), OR(|), The second factor uses Binary formats."

            },
            new CommandDescriptions()
            {
            CMD= CMDs.GLED,
            Oper = "GLED hex data",
            Description = "Read all Gled of PCB, return value as HEX number."
            },

            new CommandDescriptions()
            {
            CMD= CMDs.LED,
            Oper = "LED hex data",
            Description = "Read all LED of PCB, return value as HEX number."
            },

            new CommandDescriptions()
            {
            CMD= CMDs.FND,
            Oper = "String detected",
            Condition2 = "Scan time",
            Description = "Read SEGMENT value as string, compare with min value. Read until the same value with \"Oper\""
            },

            new CommandDescriptions()
            {
            CMD= CMDs.LCD,
            Oper = "String detected",
            Condition2 = "Scan time",
            Description = "Read all character in LCD of PCB as string, compare with min value."
            },

            new CommandDescriptions()
            {
            CMD = CMDs.PCB,
            Condition1 = "SELECT PCB",
            Description ="Command is for test only the selected PCB.\r\n"+
                            "select PCB : You can use any combination of 1, 2, 3, 4 PCB.\r\n"+
                            "ex) 1/2/3/4 or 1/2"

            },
        };

        public static void UpdateCommand()
        {
            foreach (var item in Commands)
            {
                item.TestContentsList = CommandDescriptions.QRnaming;
                if (item.Condition1 =="NAMING")
                {
                    item.Condition1List = CommandDescriptions.TXnaming;
                }
                else if (item.Condition1 == "RX DATA NAME")
                {
                    item.Condition1List = CommandDescriptions.RXnaming;
                }

                if (item.Condition2 == "TX DATA NAME")
                {
                    item.Condition2List = CommandDescriptions.TXnaming;
                }
            }
        }
    }
}
