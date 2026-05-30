namespace HardwareInspector.Models;

public class GpuInfo : HardwareInfoBase
{
    public GpuInfo() { HardwareType = "GPU"; }

    public string AdapterCompatibility { get; set; } = string.Empty;
    public string DriverVersion { get; set; } = string.Empty;
    public string DriverDate { get; set; } = string.Empty;
    public long DedicatedVRAM { get; set; }
    public long SharedMemory { get; set; }
    public string VideoArchitecture { get; set; } = string.Empty;
    public string VideoProcessor { get; set; } = string.Empty;
    public uint CurrentRefreshRate { get; set; }
    public uint CurrentHorizontalResolution { get; set; }
    public uint CurrentVerticalResolution { get; set; }
}
