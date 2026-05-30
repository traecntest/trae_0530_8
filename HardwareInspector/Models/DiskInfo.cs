namespace HardwareInspector.Models;

public class DiskInfo : HardwareInfoBase
{
    public DiskInfo() { HardwareType = "Disk"; }

    public string InterfaceType { get; set; } = string.Empty;
    public long Capacity { get; set; }
    public string MediaType { get; set; } = string.Empty;
    public string FirmwareRevision { get; set; } = string.Empty;
    public string PartitionStyle { get; set; } = string.Empty;
    public int Partitions { get; set; }
    public long TotalSectors { get; set; }
    public long FreeSpace { get; set; }
    public string FileSystem { get; set; } = string.Empty;
    public double HealthPercentage { get; set; }
    public double Temperature { get; set; }
    public long ReadSpeed { get; set; }
    public long WriteSpeed { get; set; }
}
