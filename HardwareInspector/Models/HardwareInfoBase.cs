namespace HardwareInspector.Models;

public abstract class HardwareInfoBase
{
    public string Name { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string Status { get; set; } = "Unknown";
    public string HardwareType { get; set; } = string.Empty;
    public List<SensorData> Sensors { get; set; } = new();
}
