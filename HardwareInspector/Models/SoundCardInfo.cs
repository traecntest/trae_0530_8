namespace HardwareInspector.Models;

public class SoundCardInfo : HardwareInfoBase
{
    public SoundCardInfo() { HardwareType = "SoundCard"; }

    public string ProductName { get; set; } = string.Empty;
    public string DriverVersion { get; set; } = string.Empty;
    public string DriverDate { get; set; } = string.Empty;
    public string DriverProvider { get; set; } = string.Empty;
    public string StatusInfo { get; set; } = string.Empty;
    public bool DMASupport { get; set; }
    public string AudioCodec { get; set; } = string.Empty;
    public int SampleRate { get; set; }
    public int BitDepth { get; set; }
    public int Channels { get; set; }
}
