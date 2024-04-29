namespace HVT.VTM.Program
{
    public class AppSettingParam
    {
        public Operations Operations { get; set; } = new Operations();
        public Communication Communication { get; set; } = new Communication();
        public ETCSetting ETCSetting { get; set; } = new ETCSetting();
        public SystemAccess SystemAccess { get; set; } = new SystemAccess();
    }
}
