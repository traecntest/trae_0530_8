using System.Collections.Concurrent;
using HardwareInspector.Models;
using LibreHardwareMonitor.Hardware;

namespace HardwareInspector.Services;

public class SensorPollingService : IDisposable
{
    private readonly Computer _computer;
    private readonly ConcurrentDictionary<string, List<SensorData>> _sensorCache = new();
    private readonly CancellationTokenSource _cts = new();
    private Task? _pollingTask;
    private readonly int _pollingIntervalMs;
    private bool _isRunning;

    public event EventHandler<SensorUpdateEventArgs>? SensorUpdated;
    public bool IsRunning => _isRunning;

    public SensorPollingService(int pollingIntervalMs = 1000)
    {
        _pollingIntervalMs = pollingIntervalMs;
        _computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = true,
            IsStorageEnabled = true,
            IsMotherboardEnabled = true,
            IsNetworkEnabled = true
        };
    }

    public void Start()
    {
        if (_isRunning) return;

        _computer.Open();
        _isRunning = true;
        _pollingTask = Task.Run(PollingLoopAsync);
    }

    public void Stop()
    {
        if (!_isRunning) return;
        _isRunning = false;
        _cts.Cancel();
        _computer.Close();
    }

    private async Task PollingLoopAsync()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            try
            {
                _computer.Accept(new UpdateVisitor());

                foreach (var hardware in _computer.Hardware)
                {
                    var sensors = CollectSensors(hardware);
                    var key = hardware.Identifier.ToString();
                    _sensorCache.AddOrUpdate(key, sensors, (_, _) => sensors);

                    foreach (var subHardware in hardware.SubHardware)
                    {
                        var subSensors = CollectSensors(subHardware);
                        var subKey = subHardware.Identifier.ToString();
                        _sensorCache.AddOrUpdate(subKey, subSensors, (_, _) => subSensors);
                    }
                }

                SensorUpdated?.Invoke(this, new SensorUpdateEventArgs(_sensorCache.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)));

                await Task.Delay(_pollingIntervalMs, _cts.Token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception)
            {
                await Task.Delay(2000, _cts.Token);
            }
        }
    }

    private static List<SensorData> CollectSensors(IHardware hardware)
    {
        var list = new List<SensorData>();

        foreach (var sensor in hardware.Sensors)
        {
            var sensorData = new SensorData
            {
                Name = sensor.Name ?? string.Empty,
                Value = sensor.Value ?? 0f,
                MinValue = sensor.Min,
                MaxValue = sensor.Max,
                Type = MapSensorType(sensor.SensorType),
                Unit = GetUnit(sensor.SensorType),
                Timestamp = DateTime.Now
            };
            list.Add(sensorData);
        }

        return list;
    }

    private static SensorType MapSensorType(SensorType lhmsType)
    {
        return lhmsType switch
        {
            LibreHardwareMonitor.Hardware.SensorType.Temperature => SensorType.Temperature,
            LibreHardwareMonitor.Hardware.SensorType.Voltage => SensorType.Voltage,
            LibreHardwareMonitor.Hardware.SensorType.Fan => SensorType.FanSpeed,
            LibreHardwareMonitor.Hardware.SensorType.Load => SensorType.Load,
            LibreHardwareMonitor.Hardware.SensorType.Power => SensorType.Power,
            LibreHardwareMonitor.Hardware.SensorType.Clock => SensorType.Clock,
            LibreHardwareMonitor.Hardware.SensorType.Data => SensorType.Data,
            _ => SensorType.Data
        };
    }

    private static string GetUnit(SensorType lhmsType)
    {
        return lhmsType switch
        {
            LibreHardwareMonitor.Hardware.SensorType.Temperature => "°C",
            LibreHardwareMonitor.Hardware.SensorType.Voltage => "V",
            LibreHardwareMonitor.Hardware.SensorType.Fan => "RPM",
            LibreHardwareMonitor.Hardware.SensorType.Load => "%",
            LibreHardwareMonitor.Hardware.SensorType.Power => "W",
            LibreHardwareMonitor.Hardware.SensorType.Clock => "MHz",
            LibreHardwareMonitor.Hardware.SensorType.Data => "GB",
            _ => ""
        };
    }

    public List<SensorData> GetSensorsForHardware(string hardwareIdentifier)
    {
        if (_sensorCache.TryGetValue(hardwareIdentifier, out var sensors))
            return sensors;
        return new List<SensorData>();
    }

    public Dictionary<string, List<SensorData>> GetAllSensors()
    {
        return _sensorCache.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public void ApplySensorsToHardware(HardwareInfoBase hardware)
    {
        var allSensors = GetAllSensors();
        var matched = new List<SensorData>();

        foreach (var kvp in allSensors)
        {
            var identifier = kvp.Key.ToLowerInvariant();
            var hwType = hardware.HardwareType.ToLowerInvariant();

            bool isMatch = (hwType == "cpu" && identifier.Contains("/cpu")) ||
                          (hwType == "gpu" && (identifier.Contains("/gpu") || identifier.Contains("/atigpu") || identifier.Contains("/nvgpu"))) ||
                          (hwType == "motherboard" && identifier.Contains("/mainboard")) ||
                          (hwType == "disk" && identifier.Contains("/hdd")) ||
                          (hwType == "networkadapter" && identifier.Contains("/nic"));

            if (isMatch)
            {
                matched.AddRange(kvp.Value.Where(s => s.Value > 0 || s.MinValue > 0 || s.MaxValue > 0));
            }
        }

        hardware.Sensors = matched;
    }

    public void Dispose()
    {
        Stop();
        _cts.Dispose();
        _computer.Close();
    }
}

public class SensorUpdateEventArgs : EventArgs
{
    public Dictionary<string, List<SensorData>> Sensors { get; }

    public SensorUpdateEventArgs(Dictionary<string, List<SensorData>> sensors)
    {
        Sensors = sensors;
    }
}
