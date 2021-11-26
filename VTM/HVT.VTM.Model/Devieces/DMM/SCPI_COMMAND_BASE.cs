namespace HVT.VTM.Base
{
    public static class SCPI_COMMAND_BASE
    {
        public enum Mode
        {
            CONFigure,
            MEASure,
            SENse,
            CALCulate,
            TRIGger,
            STATus,
            Sample,
            Trace,
            SYSTem,
            Star,
            ROUTe,
            INPut,
            INITiate,
            FETCh,
            DATA
        }

        public enum Header
        {
            VOLTage,
            CURRent,
            RESistance,
            FREQuecy,
            PERiod,
            CONTinuity,
            DIODe,
            TEMPerature,
            FUNCtion,
            RANGe,
            AUTO,
            OFF,

        }
        public enum Action
        {

        }
    }
}
