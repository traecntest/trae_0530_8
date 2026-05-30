namespace HardwareInspector.Models;

public class SensorData
{
    public string Name { get; set; } = string.Empty;
    public SensorType Type { get; set; }
    public float Value { get; set; }
    public float? MinValue { get; set; }
    public float? MaxValue { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public enum SensorType
{
    Temperature,
    Voltage,
    FanSpeed,
    Load,
    Power,
    Clock,
    Data
}
