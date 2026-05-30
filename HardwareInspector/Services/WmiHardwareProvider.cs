using System.Management;
using HardwareInspector.Models;

namespace HardwareInspector.Services;

public class WmiHardwareProvider
{
    public SystemSummary GetSystemSummary()
    {
        var summary = new SystemSummary
        {
            ComputerName = Environment.MachineName,
            ScanTime = DateTime.Now
        };

        using var osQuery = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
        foreach (var obj in osQuery.Get())
        {
            summary.OperatingSystem = SafeString(obj["Caption"]);
            summary.OSVersion = SafeString(obj["Version"]);
            summary.OSBuild = SafeString(obj["BuildNumber"]);
            var totalMem = SafeUlong(obj["TotalVisibleMemorySize"]);
            summary.TotalPhysicalMemory = FormatBytes(totalMem * 1024);
        }

        using var csQuery = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
        foreach (var obj in csQuery.Get())
        {
            summary.SystemManufacturer = SafeString(obj["Manufacturer"]);
            summary.SystemModel = SafeString(obj["Model"]);
            summary.SystemType = SafeString(obj["SystemType"]);
        }

        using var biosQuery = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
        foreach (var obj in biosQuery.Get())
        {
            summary.BIOSVersion = SafeString(obj["SMBIOSBIOSVersion"]);
        }

        using var cpuQuery = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
        foreach (var obj in cpuQuery.Get())
        {
            summary.Processor = SafeString(obj["Name"]);
            break;
        }

        try
        {
            var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
            summary.Uptime = $"{(int)uptime.TotalDays}d {uptime.Hours}h {uptime.Minutes}m";
        }
        catch
        {
            summary.Uptime = "N/A";
        }

        return summary;
    }

    public List<CpuInfo> GetCpuInfo()
    {
        var list = new List<CpuInfo>();
        using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
        foreach (var obj in searcher.Get())
        {
            var cpu = new CpuInfo
            {
                Name = SafeString(obj["Name"]),
                Manufacturer = SafeString(obj["Manufacturer"]),
                Model = SafeString(obj["Name"]),
                SerialNumber = SafeString(obj["ProcessorId"]),
                Status = SafeString(obj["Status"]),
                Architecture = GetCpuArchitecture(SafeUshort(obj["Architecture"])),
                CoreCount = SafeInt(obj["NumberOfCores"]),
                ThreadCount = SafeInt(obj["NumberOfLogicalProcessors"]),
                Socket = SafeString(obj["SocketDesignation"]),
                MaxClockSpeed = SafeUint(obj["MaxClockSpeed"]),
                CurrentClockSpeed = SafeUint(obj["CurrentClockSpeed"]),
                Stepping = SafeString(obj["Stepping"]),
                Revision = SafeString(obj["Revision"]),
                L2CacheSize = SafeUint(obj["L2CacheSize"]),
                L3CacheSize = SafeUint(obj["L3CacheSize"]),
                VirtualizationEnabled = SafeString(obj["VirtualizationFirmwareEnabled"])
            };
            list.Add(cpu);
        }
        return list;
    }

    public List<MotherboardInfo> GetMotherboardInfo()
    {
        var list = new List<MotherboardInfo>();
        using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
        foreach (var obj in searcher.Get())
        {
            var mb = new MotherboardInfo
            {
                Name = SafeString(obj["Product"]),
                Manufacturer = SafeString(obj["Manufacturer"]),
                Model = SafeString(obj["Product"]),
                SerialNumber = SafeString(obj["SerialNumber"]),
                Status = SafeString(obj["Status"]),
                Version = SafeString(obj["Version"])
            };
            list.Add(mb);
        }

        using var biosSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
        foreach (var obj in biosSearcher.Get())
        {
            if (list.Count > 0)
            {
                list[0].BIOSVersion = SafeString(obj["SMBIOSBIOSVersion"]);
                list[0].BIOSDate = SafeString(obj["ReleaseDate"]);
                list[0].BIOSVendor = SafeString(obj["Manufacturer"]);
            }
        }

        return list;
    }

    public List<MemoryInfo> GetMemoryInfo()
    {
        var list = new List<MemoryInfo>();
        using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
        foreach (var obj in searcher.Get())
        {
            var mem = new MemoryInfo
            {
                Name = SafeString(obj["DeviceLocator"]),
                Manufacturer = SafeString(obj["Manufacturer"]),
                Model = SafeString(obj["PartNumber"]),
                SerialNumber = SafeString(obj["SerialNumber"]),
                Capacity = SafeLong(obj["Capacity"]),
                MemoryType = GetMemoryType(SafeUshort(obj["SMBIOSMemoryType"])),
                Speed = SafeUint(obj["Speed"]),
                FormFactor = GetMemoryFormFactor(SafeUshort(obj["FormFactor"])),
                PartNumber = SafeString(obj["PartNumber"]),
                DataWidth = SafeInt(obj["DataWidth"]),
                BankLabel = SafeString(obj["BankLabel"]),
                ConfiguredClockSpeed = SafeString(obj["ConfiguredClockSpeed"]),
                Voltage = SafeString(obj["ConfiguredVoltage"])
            };
            list.Add(mem);
        }
        return list;
    }

    public List<GpuInfo> GetGpuInfo()
    {
        var list = new List<GpuInfo>();
        using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
        foreach (var obj in searcher.Get())
        {
            var gpu = new GpuInfo
            {
                Name = SafeString(obj["Name"]),
                Manufacturer = SafeString(obj["AdapterCompatibility"]),
                Model = SafeString(obj["VideoProcessor"]),
                SerialNumber = string.Empty,
                Status = SafeString(obj["Status"]),
                AdapterCompatibility = SafeString(obj["AdapterCompatibility"]),
                DriverVersion = SafeString(obj["DriverVersion"]),
                DriverDate = SafeString(obj["DriverDate"]),
                DedicatedVRAM = SafeLong(obj["AdapterRAM"]),
                VideoArchitecture = GetVideoArchitecture(SafeUshort(obj["VideoArchitecture"])),
                VideoProcessor = SafeString(obj["VideoProcessor"]),
                CurrentRefreshRate = SafeUint(obj["CurrentRefreshRate"]),
                CurrentHorizontalResolution = SafeUint(obj["CurrentHorizontalResolution"]),
                CurrentVerticalResolution = SafeUint(obj["CurrentVerticalResolution"])
            };
            list.Add(gpu);
        }
        return list;
    }

    public List<DiskInfo> GetDiskInfo()
    {
        var list = new List<DiskInfo>();
        using var diskSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
        foreach (var obj in diskSearcher.Get())
        {
            var disk = new DiskInfo
            {
                Name = SafeString(obj["Model"]),
                Manufacturer = SafeString(obj["Manufacturer"]),
                Model = SafeString(obj["Model"]),
                SerialNumber = SafeString(obj["SerialNumber"]),
                Status = SafeString(obj["Status"]),
                InterfaceType = SafeString(obj["InterfaceType"]),
                Capacity = SafeLong(obj["Size"]),
                MediaType = SafeString(obj["MediaType"]),
                FirmwareRevision = SafeString(obj["FirmwareRevision"]),
                Partitions = SafeInt(obj["Partitions"]),
                TotalSectors = SafeLong(obj["TotalSectors"])
            };

            using var partSearcher = new ManagementObjectSearcher(
                $"ASSOCIATORS OF {{Win32_DiskDrive.DeviceID='{SafeString(obj["DeviceID"]}'}} WHERE AssocClass=Win32_DiskDriveToDiskPartition");
            foreach (var part in partSearcher.Get())
            {
                using var logicalSearcher = new ManagementObjectSearcher(
                    $"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{SafeString(part["DeviceID"]}'}} WHERE AssocClass=Win32_LogicalDiskToPartition");
                foreach (var logical in logicalSearcher.Get())
                {
                    disk.FileSystem = SafeString(logical["FileSystem"]);
                    disk.FreeSpace = SafeLong(logical["FreeSpace"]);
                }
            }

            list.Add(disk);
        }
        return list;
    }

    public List<MonitorInfo> GetMonitorInfo()
    {
        var list = new List<MonitorInfo>();
        using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DesktopMonitor");
        foreach (var obj in searcher.Get())
        {
            var monitor = new MonitorInfo
            {
                Name = SafeString(obj["Name"]),
                Manufacturer = SafeString(obj["MonitorManufacturer"]),
                MonitorType = SafeString(obj["MonitorType"]),
                SerialNumber = SafeString(obj["SerialNumber"]),
                PixelsPerXLogicalInch = SafeInt(obj["PixelsPerXLogicalInch"])
            };
            list.Add(monitor);
        }

        try
        {
            using var wmiSearcher = new ManagementObjectSearcher("SELECT * FROM WmiMonitorID");
            foreach (var obj in wmiSearcher.Get())
            {
                var monitor = new MonitorInfo
                {
                    Manufacturer = ReadWmiString(obj, "ManufacturerName"),
                    Model = ReadWmiString(obj, "ProductCodeID"),
                    SerialNumber = ReadWmiString(obj, "SerialNumberID"),
                    MonitorId = SafeString(obj["InstanceName"]),
                    YearOfManufacture = SafeString(obj["YearOfManufacture"]),
                    WeekOfManufacture = SafeString(obj["WeekOfManufacture"])
                };
                list.Add(monitor);
            }
        }
        catch
        {
        }

        return list;
    }

    public List<NetworkAdapterInfo> GetNetworkAdapterInfo()
    {
        var list = new List<NetworkAdapterInfo>();
        using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter WHERE PhysicalAdapter=True");
        foreach (var obj in searcher.Get())
        {
            var adapter = new NetworkAdapterInfo
            {
                Name = SafeString(obj["Name"]),
                Manufacturer = SafeString(obj["Manufacturer"]),
                Model = SafeString(obj["ProductName"]),
                SerialNumber = string.Empty,
                Status = SafeString(obj["NetConnectionStatus"]) == "2" ? "Connected" : "Disconnected",
                MACAddress = SafeString(obj["MACAddress"]),
                AdapterType = SafeString(obj["AdapterType"]),
                Speed = FormatSpeed(SafeLong(obj["Speed"])),
                ConnectionStatus = GetNetConnectionStatus(SafeShort(obj["NetConnectionStatus"]))
            };

            var configSearcher = new ManagementObjectSearcher(
                $"SELECT * FROM Win32_NetworkAdapterConfiguration WHERE Index={SafeUint(obj["Index"])}");
            foreach (var config in configSearcher.Get())
            {
                adapter.DHCPEnabled = SafeBool(config["DHCPEnabled"]);
                adapter.IPAddress = SafeStringArray(config["IPAddress"]);
                adapter.SubnetMask = SafeStringArray(config["IPSubnet"]);
                adapter.DefaultGateway = SafeStringArray(config["DefaultIPGateway"]);
                adapter.DNSServers = SafeStringArray(config["DNSServerSearchOrder"]);
            }

            list.Add(adapter);
        }
        return list;
    }

    public List<SoundCardInfo> GetSoundCardInfo()
    {
        var list = new List<SoundCardInfo>();
        using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_SoundDevice");
        foreach (var obj in searcher.Get())
        {
            var sound = new SoundCardInfo
            {
                Name = SafeString(obj["Name"]),
                Manufacturer = SafeString(obj["Manufacturer"]),
                Model = SafeString(obj["ProductName"]),
                ProductName = SafeString(obj["ProductName"]),
                Status = SafeString(obj["Status"]),
                StatusInfo = SafeString(obj["StatusInfo"]),
                DriverVersion = SafeString(obj["DriverVersion"]),
                DriverDate = SafeString(obj["DriverDate"]),
                DriverProvider = SafeString(obj["DriverProvider"])
            };
            list.Add(sound);
        }
        return list;
    }

    private static string SafeString(object? obj)
    {
        return obj?.ToString() ?? string.Empty;
    }

    private static int SafeInt(object? obj)
    {
        return obj is int i ? i : 0;
    }

    private static uint SafeUint(object? obj)
    {
        return obj is uint u ? u : 0u;
    }

    private static ushort SafeUshort(object? obj)
    {
        return obj is ushort u ? u : (ushort)0;
    }

    private static short SafeShort(object? obj)
    {
        return obj is short s ? s : (short)0;
    }

    private static long SafeLong(object? obj)
    {
        return obj is long l ? l : 0L;
    }

    private static ulong SafeUlong(object? obj)
    {
        return obj is ulong u ? u : 0UL;
    }

    private static bool SafeBool(object? obj)
    {
        return obj is bool b && b;
    }

    private static string SafeStringArray(object? obj)
    {
        if (obj is string[] arr)
            return string.Join(", ", arr);
        return string.Empty;
    }

    private static string ReadWmiString(ManagementObject obj, string propertyName)
    {
        try
        {
            if (obj[propertyName] is ushort[] chars)
                return new string(chars.Where(c => c > 0).Select(c => (char)c).ToArray());
        }
        catch { }
        return string.Empty;
    }

    private static string FormatBytes(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < suffixes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {suffixes[order]}";
    }

    private static string FormatSpeed(long bps)
    {
        if (bps <= 0) return "N/A";
        if (bps >= 1_000_000_000) return $"{bps / 1_000_000_000.0:0.##} Gbps";
        if (bps >= 1_000_000) return $"{bps / 1_000_000.0:0.##} Mbps";
        return $"{bps / 1_000.0:0.##} Kbps";
    }

    private static string GetCpuArchitecture(ushort arch)
    {
        return arch switch
        {
            0 => "x86",
            5 => "ARM",
            9 => "x64",
            12 => "ARM64",
            _ => $"Unknown ({arch})"
        };
    }

    private static string GetMemoryType(ushort type)
    {
        return type switch
        {
            20 => "DDR",
            21 => "DDR2",
            22 => "DDR2 FB-DIMM",
            24 => "DDR3",
            26 => "DDR4",
            34 => "DDR5",
            _ => $"Unknown ({type})"
        };
    }

    private static string GetMemoryFormFactor(ushort ff)
    {
        return ff switch
        {
            8 => "DIMM",
            12 => "SODIMM",
            _ => $"Unknown ({ff})"
        };
    }

    private static string GetVideoArchitecture(ushort arch)
    {
        return arch switch
        {
            1 => "Other",
            2 => "Unknown",
            3 => "CGA",
            4 => "EGA",
            5 => "VGA",
            6 => "SVGA",
            7 => "MDA",
            8 => "HGC",
            9 => "MCGA",
            10 => "8514A",
            _ => $"Unknown ({arch})"
        };
    }

    private static string GetNetConnectionStatus(short status)
    {
        return status switch
        {
            0 => "Disconnected",
            1 => "Connecting",
            2 => "Connected",
            3 => "Disconnecting",
            4 => "Hardware not present",
            5 => "Hardware disabled",
            6 => "Hardware malfunction",
            7 => "Media disconnected",
            8 => "Authenticating",
            9 => "Authentication succeeded",
            10 => "Authentication failed",
            11 => "Invalid address",
            12 => "Credentials required",
            _ => $"Unknown ({status})"
        };
    }
}
