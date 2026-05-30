namespace HardwareInspector.Models;

public class MonitorInfo : HardwareInfoBase
{
    public MonitorInfo() { HardwareType = "Monitor"; }

    public string MonitorType { get; set; } = string.Empty;
    public string MonitorId { get; set; } = string.Empty;
    public int PixelsPerXLogicalInch { get; set; }
    public int ScreenWidth { get; set; }
    public int ScreenHeight { get; set; }
    public string Resolution { get; set; } = string.Empty;
    public double DiagonalSize { get; set; }
    public string DisplayTechnology { get; set; } = string.Empty;
    public int RefreshRate { get; set; }
    public string ColorDepth { get; set; } = string.Empty;
    public string AspectRatio { get; set; } = string.Empty;
    public string ConnectionType { get; set; } = string.Empty;
    public DateTime ManufactureDate { get; set; }
    public string WeekOfManufacture { get; set; } = string.Empty;
    public string YearOfManufacture { get; set; } = string.Empty;
}
