namespace HardwareInspector.Models;

public class MotherboardInfo : HardwareInfoBase
{
    public MotherboardInfo() { HardwareType = "Motherboard"; }

    public string Version { get; set; } = string.Empty;
    public string BIOSVersion { get; set; } = string.Empty;
    public string BIOSDate { get; set; } = string.Empty;
    public string BIOSVendor { get; set; } = string.Empty;
    public string Chipset { get; set; } = string.Empty;
    public string FormFactor { get; set; } = string.Empty;
}
