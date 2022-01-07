namespace HVT.VTM.Base
{
    public class AppSettingParam
    {
        public Operations Operations { get; set; } = new Operations();
        public Communication Communication { get; set; } = new Communication();
        public ETCSetting ETCSetting { get; set; } = new ETCSetting();
        public QR_Code QR { get; set; } = new QR_Code();
        public SystemAccess SystemAccess { get; set; } = new SystemAccess();
    }
}
