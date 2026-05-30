namespace HardwareInspector.Models;

public class CpuInfo : HardwareInfoBase
{
    public CpuInfo() { HardwareType = "CPU"; }

    public string Architecture { get; set; } = string.Empty;
    public int CoreCount { get; set; }
    public int ThreadCount { get; set; }
    public string Socket { get; set; } = string.Empty;
    public double MaxClockSpeed { get; set; }
    public double CurrentClockSpeed { get; set; }
    public string Stepping { get; set; } = string.Empty;
    public string Revision { get; set; } = string.Empty;
    public uint L2CacheSize { get; set; }
    public uint L3CacheSize { get; set; }
    public string VirtualizationEnabled { get; set; } = string.Empty;
}
