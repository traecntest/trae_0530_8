namespace HardwareInspector.Models;

public class SystemSummary
{
    public string ComputerName { get; set; } = string.Empty;
    public string OperatingSystem { get; set; } = string.Empty;
    public string OSVersion { get; set; } = string.Empty;
    public string OSBuild { get; set; } = string.Empty;
    public string SystemManufacturer { get; set; } = string.Empty;
    public string SystemModel { get; set; } = string.Empty;
    public string SystemType { get; set; } = string.Empty;
    public string Processor { get; set; } = string.Empty;
    public string TotalPhysicalMemory { get; set; } = string.Empty;
    public string BIOSVersion { get; set; } = string.Empty;
    public DateTime ScanTime { get; set; } = DateTime.Now;
    public string Uptime { get; set; } = string.Empty;
}
