namespace HardwareInspector.Models;

public class NetworkAdapterInfo : HardwareInfoBase
{
    public NetworkAdapterInfo() { HardwareType = "NetworkAdapter"; }

    public string MACAddress { get; set; } = string.Empty;
    public string AdapterType { get; set; } = string.Empty;
    public string Speed { get; set; } = string.Empty;
    public bool DHCPEnabled { get; set; }
    public string IPAddress { get; set; } = string.Empty;
    public string SubnetMask { get; set; } = string.Empty;
    public string DefaultGateway { get; set; } = string.Empty;
    public string DNSServers { get; set; } = string.Empty;
    public string ConnectionStatus { get; set; } = string.Empty;
    public long BytesSent { get; set; }
    public long BytesReceived { get; set; }
    public string LinkSpeed { get; set; } = string.Empty;
}
