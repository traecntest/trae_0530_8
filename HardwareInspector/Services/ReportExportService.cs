using System.IO;
using System.Text;
using HardwareInspector.Models;

namespace HardwareInspector.Services;

public class ReportExportService
{
    public void ExportToTxt(string filePath, SystemSummary summary, List<CpuInfo> cpus,
        List<MotherboardInfo> motherboards, List<MemoryInfo> memories,
        List<GpuInfo> gpus, List<DiskInfo> disks, List<MonitorInfo> monitors,
        List<NetworkAdapterInfo> networks, List<SoundCardInfo> sounds)
    {
        var sb = new StringBuilder();

        sb.AppendLine("═══════════════════════════════════════════════════════");
        sb.AppendLine("           硬件检测报告 - Hardware Inspector");
        sb.AppendLine("═══════════════════════════════════════════════════════");
        sb.AppendLine();
        sb.AppendLine($"生成时间: {summary.ScanTime:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"计算机名: {summary.ComputerName}");
        sb.AppendLine();

        sb.AppendLine("──────────────── 系统概览 ────────────────");
        sb.AppendLine($"操作系统:  {summary.OperatingSystem}");
        sb.AppendLine($"系统版本:  {summary.OSVersion} (Build {summary.OSBuild})");
        sb.AppendLine($"系统制造商: {summary.SystemManufacturer}");
        sb.AppendLine($"系统型号:  {summary.SystemModel}");
        sb.AppendLine($"系统类型:  {summary.SystemType}");
        sb.AppendLine($"处理器:   {summary.Processor}");
        sb.AppendLine($"总物理内存: {summary.TotalPhysicalMemory}");
        sb.AppendLine($"BIOS版本:  {summary.BIOSVersion}");
        sb.AppendLine($"运行时间:  {summary.Uptime}");
        sb.AppendLine();

        if (cpus.Count > 0)
        {
            sb.AppendLine("──────────────── CPU 信息 ────────────────");
            foreach (var cpu in cpus)
            {
                sb.AppendLine($"名称:     {cpu.Name}");
                sb.AppendLine($"制造商:    {cpu.Manufacturer}");
                sb.AppendLine($"架构:     {cpu.Architecture}");
                sb.AppendLine($"核心数:    {cpu.CoreCount}");
                sb.AppendLine($"线程数:    {cpu.ThreadCount}");
                sb.AppendLine($"插槽:     {cpu.Socket}");
                sb.AppendLine($"最大频率:   {cpu.MaxClockSpeed} MHz");
                sb.AppendLine($"当前频率:   {cpu.CurrentClockSpeed} MHz");
                sb.AppendLine($"序列号:    {cpu.SerialNumber}");
                sb.AppendLine($"状态:     {cpu.Status}");
                sb.AppendLine($"虚拟化:    {cpu.VirtualizationEnabled}");
                AppendSensors(sb, cpu.Sensors);
                sb.AppendLine();
            }
        }

        if (motherboards.Count > 0)
        {
            sb.AppendLine("──────────────── 主板信息 ────────────────");
            foreach (var mb in motherboards)
            {
                sb.AppendLine($"名称:     {mb.Name}");
                sb.AppendLine($"制造商:    {mb.Manufacturer}");
                sb.AppendLine($"型号:     {mb.Model}");
                sb.AppendLine($"版本:     {mb.Version}");
                sb.AppendLine($"序列号:    {mb.SerialNumber}");
                sb.AppendLine($"BIOS版本:  {mb.BIOSVersion}");
                sb.AppendLine($"BIOS厂商:  {mb.BIOSVendor}");
                sb.AppendLine($"BIOS日期:  {mb.BIOSDate}");
                AppendSensors(sb, mb.Sensors);
                sb.AppendLine();
            }
        }

        if (memories.Count > 0)
        {
            sb.AppendLine("──────────────── 内存信息 ────────────────");
            foreach (var mem in memories)
            {
                sb.AppendLine($"插槽:     {mem.BankLabel}");
                sb.AppendLine($"制造商:    {mem.Manufacturer}");
                sb.AppendLine($"型号:     {mem.PartNumber}");
                sb.AppendLine($"容量:     {FormatBytes(mem.Capacity)}");
                sb.AppendLine($"类型:     {mem.MemoryType}");
                sb.AppendLine($"速度:     {mem.Speed} MHz");
                sb.AppendLine($"序列号:    {mem.SerialNumber}");
                sb.AppendLine($"电压:     {mem.Voltage} V");
                AppendSensors(sb, mem.Sensors);
                sb.AppendLine();
            }
        }

        if (gpus.Count > 0)
        {
            sb.AppendLine("──────────────── 显卡信息 ────────────────");
            foreach (var gpu in gpus)
            {
                sb.AppendLine($"名称:     {gpu.Name}");
                sb.AppendLine($"制造商:    {gpu.Manufacturer}");
                sb.AppendLine($"显存:     {FormatBytes(gpu.DedicatedVRAM)}");
                sb.AppendLine($"驱动版本:   {gpu.DriverVersion}");
                sb.AppendLine($"分辨率:    {gpu.CurrentHorizontalResolution}x{gpu.CurrentVerticalResolution}");
                sb.AppendLine($"刷新率:    {gpu.CurrentRefreshRate} Hz");
                sb.AppendLine($"状态:     {gpu.Status}");
                AppendSensors(sb, gpu.Sensors);
                sb.AppendLine();
            }
        }

        if (disks.Count > 0)
        {
            sb.AppendLine("──────────────── 硬盘信息 ────────────────");
            foreach (var disk in disks)
            {
                sb.AppendLine($"名称:     {disk.Name}");
                sb.AppendLine($"制造商:    {disk.Manufacturer}");
                sb.AppendLine($"容量:     {FormatBytes(disk.Capacity)}");
                sb.AppendLine($"接口:     {disk.InterfaceType}");
                sb.AppendLine($"介质类型:   {disk.MediaType}");
                sb.AppendLine($"序列号:    {disk.SerialNumber}");
                sb.AppendLine($"固件版本:   {disk.FirmwareRevision}");
                sb.AppendLine($"状态:     {disk.Status}");
                AppendSensors(sb, disk.Sensors);
                sb.AppendLine();
            }
        }

        if (monitors.Count > 0)
        {
            sb.AppendLine("──────────────── 显示器信息 ────────────────");
            foreach (var mon in monitors)
            {
                sb.AppendLine($"名称:     {mon.Name}");
                sb.AppendLine($"制造商:    {mon.Manufacturer}");
                sb.AppendLine($"型号:     {mon.Model}");
                sb.AppendLine($"序列号:    {mon.SerialNumber}");
                sb.AppendLine($"生产年份:   {mon.YearOfManufacture}");
                sb.AppendLine();
            }
        }

        if (networks.Count > 0)
        {
            sb.AppendLine("──────────────── 网卡信息 ────────────────");
            foreach (var net in networks)
            {
                sb.AppendLine($"名称:     {net.Name}");
                sb.AppendLine($"制造商:    {net.Manufacturer}");
                sb.AppendLine($"MAC地址:   {net.MACAddress}");
                sb.AppendLine($"速度:     {net.Speed}");
                sb.AppendLine($"IP地址:   {net.IPAddress}");
                sb.AppendLine($"状态:     {net.ConnectionStatus}");
                sb.AppendLine();
            }
        }

        if (sounds.Count > 0)
        {
            sb.AppendLine("──────────────── 声卡信息 ────────────────");
            foreach (var sound in sounds)
            {
                sb.AppendLine($"名称:     {sound.Name}");
                sb.AppendLine($"制造商:    {sound.Manufacturer}");
                sb.AppendLine($"驱动版本:   {sound.DriverVersion}");
                sb.AppendLine($"状态:     {sound.Status}");
                sb.AppendLine();
            }
        }

        sb.AppendLine("═══════════════════════════════════════════════════════");
        sb.AppendLine("           报告结束 - Generated by Hardware Inspector");
        sb.AppendLine("═══════════════════════════════════════════════════════");

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
    }

    public void ExportToHtml(string filePath, SystemSummary summary, List<CpuInfo> cpus,
        List<MotherboardInfo> motherboards, List<MemoryInfo> memories,
        List<GpuInfo> gpus, List<DiskInfo> disks, List<MonitorInfo> monitors,
        List<NetworkAdapterInfo> networks, List<SoundCardInfo> sounds)
    {
        var sb = new StringBuilder();

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang='zh-CN'>");
        sb.AppendLine("<head>");
        sb.AppendLine("<meta charset='UTF-8'>");
        sb.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        sb.AppendLine($"<title>硬件检测报告 - {summary.ComputerName}</title>");
        sb.AppendLine("<style>");
        sb.AppendLine("  * { margin: 0; padding: 0; box-sizing: border-box; }");
        sb.AppendLine("  body { font-family: 'Segoe UI', system-ui, sans-serif; background: #0E1621; color: #E8ECF1; padding: 32px; }");
        sb.AppendLine("  .header { text-align: center; margin-bottom: 40px; }");
        sb.AppendLine("  .header h1 { font-size: 28px; color: #2D7FF9; margin-bottom: 8px; }");
        sb.AppendLine("  .header .meta { color: #8899AA; font-size: 14px; }");
        sb.AppendLine("  .card { background: #1E2D3D; border: 1px solid #2A3A4E; border-radius: 12px; padding: 24px; margin-bottom: 24px; }");
        sb.AppendLine("  .card h2 { font-size: 18px; color: #00C9A7; margin-bottom: 16px; padding-bottom: 12px; border-bottom: 1px solid #2A3A4E; }");
        sb.AppendLine("  .grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 16px; }");
        sb.AppendLine("  .info-row { display: flex; justify-content: space-between; padding: 6px 0; border-bottom: 1px solid rgba(42,58,78,0.5); }");
        sb.AppendLine("  .info-label { color: #8899AA; font-size: 13px; }");
        sb.AppendLine("  .info-value { color: #E8ECF1; font-size: 13px; font-weight: 500; }");
        sb.AppendLine("  .sensor { display: inline-block; background: #1A2332; border: 1px solid #2A3A4E; border-radius: 8px; padding: 8px 14px; margin: 4px; font-size: 12px; }");
        sb.AppendLine("  .sensor-temp { border-color: #FF4757; color: #FF4757; }");
        sb.AppendLine("  .sensor-volt { border-color: #FFB020; color: #FFB020; }");
        sb.AppendLine("  .sensor-fan { border-color: #2D7FF9; color: #2D7FF9; }");
        sb.AppendLine("  .sensor-load { border-color: #00C9A7; color: #00C9A7; }");
        sb.AppendLine("  .summary-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(200px, 1fr)); gap: 16px; }");
        sb.AppendLine("  .summary-item { text-align: center; padding: 16px; background: #1A2332; border-radius: 8px; }");
        sb.AppendLine("  .summary-item .label { color: #8899AA; font-size: 12px; margin-bottom: 4px; }");
        sb.AppendLine("  .summary-item .value { color: #E8ECF1; font-size: 16px; font-weight: 600; }");
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");

        sb.AppendLine("<div class='header'>");
        sb.AppendLine($"<h1>🖥️ 硬件检测报告</h1>");
        sb.AppendLine($"<div class='meta'>计算机: {summary.ComputerName} | 生成时间: {summary.ScanTime:yyyy-MM-dd HH:mm:ss} | 运行时间: {summary.Uptime}</div>");
        sb.AppendLine("</div>");

        sb.AppendLine("<div class='card'><h2>📊 系统概览</h2>");
        sb.AppendLine("<div class='summary-grid'>");
        AppendSummaryItem(sb, "操作系统", summary.OperatingSystem);
        AppendSummaryItem(sb, "系统版本", $"{summary.OSVersion} (Build {summary.OSBuild})");
        AppendSummaryItem(sb, "制造商", summary.SystemManufacturer);
        AppendSummaryItem(sb, "型号", summary.SystemModel);
        AppendSummaryItem(sb, "处理器", summary.Processor);
        AppendSummaryItem(sb, "总内存", summary.TotalPhysicalMemory);
        AppendSummaryItem(sb, "BIOS", summary.BIOSVersion);
        sb.AppendLine("</div></div>");

        AppendHardwareSectionHtml(sb, "CPU 信息", "🧠", cpus, c =>
        {
            var items = new List<(string, string)>
            {
                ("名称", c.Name), ("制造商", c.Manufacturer), ("架构", c.Architecture),
                ("核心/线程", $"{c.CoreCount}C/{c.ThreadCount}T"),
                ("最大频率", $"{c.MaxClockSpeed} MHz"), ("序列号", c.SerialNumber),
                ("虚拟化", c.VirtualizationEnabled), ("状态", c.Status)
            };
            return items;
        });

        AppendHardwareSectionHtml(sb, "主板信息", "🔧", motherboards, m =>
        {
            var items = new List<(string, string)>
            {
                ("名称", m.Name), ("制造商", m.Manufacturer), ("型号", m.Model),
                ("版本", m.Version), ("序列号", m.SerialNumber),
                ("BIOS版本", m.BIOSVersion), ("BIOS厂商", m.BIOSVendor)
            };
            return items;
        });

        AppendHardwareSectionHtml(sb, "内存信息", "💾", memories, m =>
        {
            var items = new List<(string, string)>
            {
                ("插槽", m.BankLabel), ("制造商", m.Manufacturer), ("型号", m.PartNumber),
                ("容量", FormatBytes(m.Capacity)), ("类型", m.MemoryType),
                ("速度", $"{m.Speed} MHz"), ("序列号", m.SerialNumber)
            };
            return items;
        });

        AppendHardwareSectionHtml(sb, "显卡信息", "🎮", gpus, g =>
        {
            var items = new List<(string, string)>
            {
                ("名称", g.Name), ("制造商", g.Manufacturer),
                ("显存", FormatBytes(g.DedicatedVRAM)),
                ("驱动版本", g.DriverVersion),
                ("分辨率", $"{g.CurrentHorizontalResolution}x{g.CurrentVerticalResolution}"),
                ("状态", g.Status)
            };
            return items;
        });

        AppendHardwareSectionHtml(sb, "硬盘信息", "📀", disks, d =>
        {
            var items = new List<(string, string)>
            {
                ("名称", d.Name), ("制造商", d.Manufacturer), ("容量", FormatBytes(d.Capacity)),
                ("接口", d.InterfaceType), ("类型", d.MediaType),
                ("序列号", d.SerialNumber), ("状态", d.Status)
            };
            return items;
        });

        AppendHardwareSectionHtml(sb, "显示器信息", "🖥️", monitors, m =>
        {
            var items = new List<(string, string)>
            {
                ("制造商", m.Manufacturer), ("型号", m.Model),
                ("序列号", m.SerialNumber), ("生产年份", m.YearOfManufacture)
            };
            return items;
        });

        AppendHardwareSectionHtml(sb, "网卡信息", "🌐", networks, n =>
        {
            var items = new List<(string, string)>
            {
                ("名称", n.Name), ("制造商", n.Manufacturer),
                ("MAC地址", n.MACAddress), ("速度", n.Speed),
                ("IP地址", n.IPAddress), ("状态", n.ConnectionStatus)
            };
            return items;
        });

        AppendHardwareSectionHtml(sb, "声卡信息", "🔊", sounds, s =>
        {
            var items = new List<(string, string)>
            {
                ("名称", s.Name), ("制造商", s.Manufacturer),
                ("驱动版本", s.DriverVersion), ("状态", s.Status)
            };
            return items;
        });

        sb.AppendLine("</body></html>");

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
    }

    private static void AppendSensors(StringBuilder sb, List<SensorData> sensors)
    {
        if (sensors.Count == 0) return;
        sb.AppendLine("  传感器数据:");
        foreach (var s in sensors)
        {
            sb.AppendLine($"    {s.Name}: {s.Value:F1} {s.Unit}" +
                          (s.MinValue.HasValue ? $" (Min: {s.MinValue:F1})" : "") +
                          (s.MaxValue.HasValue ? $" (Max: {s.MaxValue:F1})" : ""));
        }
    }

    private static void AppendSummaryItem(StringBuilder sb, string label, string value)
    {
        sb.AppendLine($"<div class='summary-item'><div class='label'>{label}</div><div class='value'>{EscapeHtml(value)}</div></div>");
    }

    private static void AppendHardwareSectionHtml<T>(StringBuilder sb, string title, string icon,
        List<T> items, Func<T, List<(string label, string value)>> getProperties) where T : HardwareInfoBase
    {
        if (items.Count == 0) return;

        sb.AppendLine($"<div class='card'><h2>{icon} {title}</h2>");

        foreach (var item in items)
        {
            var props = getProperties(item);
            sb.AppendLine("<div class='grid'><div>");
            foreach (var (label, value) in props)
            {
                sb.AppendLine($"<div class='info-row'><span class='info-label'>{label}</span><span class='info-value'>{EscapeHtml(value)}</span></div>");
            }
            sb.AppendLine("</div></div>");

            if (item.Sensors.Count > 0)
            {
                sb.AppendLine("<div style='margin-top:12px'>");
                foreach (var sensor in item.Sensors)
                {
                    var cssClass = sensor.Type switch
                    {
                        SensorType.Temperature => "sensor-temp",
                        SensorType.Voltage => "sensor-volt",
                        SensorType.FanSpeed => "sensor-fan",
                        SensorType.Load => "sensor-load",
                        _ => ""
                    };
                    sb.AppendLine($"<span class='sensor {cssClass}'>{sensor.Name}: {sensor.Value:F1} {sensor.Unit}</span>");
                }
                sb.AppendLine("</div>");
            }
        }

        sb.AppendLine("</div>");
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

    private static string EscapeHtml(string text)
    {
        return text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
    }
}
