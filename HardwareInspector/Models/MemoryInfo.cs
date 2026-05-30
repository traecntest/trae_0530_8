namespace HardwareInspector.Models;

public class MemoryInfo : HardwareInfoBase
{
    public MemoryInfo() { HardwareType = "Memory"; }

    public long Capacity { get; set; }
    public string MemoryType { get; set; } = string.Empty;
    public double Speed { get; set; }
    public string FormFactor { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public int DataWidth { get; set; }
    public string BankLabel { get; set; } = string.Empty;
    public string ConfiguredClockSpeed { get; set; } = string.Empty;
    public string Voltage { get; set; } = string.Empty;
}
