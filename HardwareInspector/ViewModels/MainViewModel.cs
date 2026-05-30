using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HardwareInspector.Models;
using HardwareInspector.Services;
using Microsoft.Win32;

namespace HardwareInspector.ViewModels;

public partial class MainViewModel : ObservableObject, IDisposable
{
    private readonly WmiHardwareProvider _wmiProvider;
    private readonly SensorPollingService _sensorService;
    private readonly ReportExportService _reportService;

    [ObservableProperty]
    private SystemSummary _systemSummary = new();

    [ObservableProperty]
    private ObservableCollection<CpuInfo> _cpuList = new();

    [ObservableProperty]
    private ObservableCollection<MotherboardInfo> _motherboardList = new();

    [ObservableProperty]
    private ObservableCollection<MemoryInfo> _memoryList = new();

    [ObservableProperty]
    private ObservableCollection<GpuInfo> _gpuList = new();

    [ObservableProperty]
    private ObservableCollection<DiskInfo> _diskList = new();

    [ObservableProperty]
    private ObservableCollection<MonitorInfo> _monitorList = new();

    [ObservableProperty]
    private ObservableCollection<NetworkAdapterInfo> _networkList = new();

    [ObservableProperty]
    private ObservableCollection<SoundCardInfo> _soundList = new();

    [ObservableProperty]
    private int _selectedPageIndex;

    [ObservableProperty]
    private bool _isScanning;

    [ObservableProperty]
    private bool _isSensorRunning;

    [ObservableProperty]
    private string _statusText = "就绪";

    [ObservableProperty]
    private float _cpuTemperature;

    [ObservableProperty]
    private float _cpuLoad;

    [ObservableProperty]
    private float _gpuTemperature;

    [ObservableProperty]
    private float _gpuLoad;

    [ObservableProperty]
    private float _memoryUsagePercent;

    [ObservableProperty]
    private ObservableCollection<SensorData> _currentSensors = new();

    [ObservableProperty]
    private HardwareInfoBase? _selectedHardware;

    public MainViewModel()
    {
        _wmiProvider = new WmiHardwareProvider();
        _sensorService = new SensorPollingService(1000);
        _reportService = new ReportExportService();
        _sensorService.SensorUpdated += OnSensorUpdated;

        RunScanCommand.Execute(null);
    }

    [RelayCommand]
    private async Task RunScanAsync()
    {
        if (IsScanning) return;
        IsScanning = true;
        StatusText = "正在扫描硬件...";

        try
        {
            await Task.Run(() =>
            {
                SystemSummary = _wmiProvider.GetSystemSummary();

                List<CpuInfo> cpus = new();
                List<MotherboardInfo> mbs = new();
                List<MemoryInfo> mems = new();
                List<GpuInfo> gpus = new();
                List<DiskInfo> disks = new();
                List<MonitorInfo> monitors = new();
                List<NetworkAdapterInfo> networks = new();
                List<SoundCardInfo> sounds = new();

                try { cpus = _wmiProvider.GetCpuInfo(); } catch { }
                try { mbs = _wmiProvider.GetMotherboardInfo(); } catch { }
                try { mems = _wmiProvider.GetMemoryInfo(); } catch { }
                try { gpus = _wmiProvider.GetGpuInfo(); } catch { }
                try { disks = _wmiProvider.GetDiskInfo(); } catch { }
                try { monitors = _wmiProvider.GetMonitorInfo(); } catch { }
                try { networks = _wmiProvider.GetNetworkAdapterInfo(); } catch { }
                try { sounds = _wmiProvider.GetSoundCardInfo(); } catch { }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    CpuList = new ObservableCollection<CpuInfo>(cpus);
                    MotherboardList = new ObservableCollection<MotherboardInfo>(mbs);
                    MemoryList = new ObservableCollection<MemoryInfo>(mems);
                    GpuList = new ObservableCollection<GpuInfo>(gpus);
                    DiskList = new ObservableCollection<DiskInfo>(disks);
                    MonitorList = new ObservableCollection<MonitorInfo>(monitors);
                    NetworkList = new ObservableCollection<NetworkAdapterInfo>(networks);
                    SoundList = new ObservableCollection<SoundCardInfo>(sounds);
                });
            });

            if (!IsSensorRunning)
            {
                _sensorService.Start();
                IsSensorRunning = true;
            }

            StatusText = "扫描完成 - 传感器监控中";
        }
        catch (Exception ex)
        {
            StatusText = $"扫描出错: {ex.Message}";
        }
        finally
        {
            IsScanning = false;
        }
    }

    [RelayCommand]
    private void ToggleSensor()
    {
        if (IsSensorRunning)
        {
            _sensorService.Stop();
            IsSensorRunning = false;
            StatusText = "传感器监控已停止";
        }
        else
        {
            _sensorService.Start();
            IsSensorRunning = true;
            StatusText = "传感器监控中";
        }
    }

    [RelayCommand]
    private async Task ExportTxtAsync()
    {
        var dialog = new SaveFileDialog
        {
            Filter = "文本文件|*.txt",
            FileName = $"硬件检测报告_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
        };
        if (dialog.ShowDialog() == true)
        {
            await Task.Run(() => _reportService.ExportToTxt(
                dialog.FileName, SystemSummary,
                CpuList.ToList(), MotherboardList.ToList(), MemoryList.ToList(),
                GpuList.ToList(), DiskList.ToList(), MonitorList.ToList(),
                NetworkList.ToList(), SoundList.ToList()));
            StatusText = $"报告已导出: {dialog.FileName}";
        }
    }

    [RelayCommand]
    private async Task ExportHtmlAsync()
    {
        var dialog = new SaveFileDialog
        {
            Filter = "HTML文件|*.html",
            FileName = $"硬件检测报告_{DateTime.Now:yyyyMMdd_HHmmss}.html"
        };
        if (dialog.ShowDialog() == true)
        {
            await Task.Run(() => _reportService.ExportToHtml(
                dialog.FileName, SystemSummary,
                CpuList.ToList(), MotherboardList.ToList(), MemoryList.ToList(),
                GpuList.ToList(), DiskList.ToList(), MonitorList.ToList(),
                NetworkList.ToList(), SoundList.ToList()));
            StatusText = $"报告已导出: {dialog.FileName}";
        }
    }

    private void OnSensorUpdated(object? sender, SensorUpdateEventArgs e)
    {
        Application.Current?.Dispatcher.InvokeAsync(() =>
        {
            foreach (var cpu in CpuList)
            {
                _sensorService.ApplySensorsToHardware(cpu);
                var tempSensor = cpu.Sensors.FirstOrDefault(s => s.Type == SensorType.Temperature && s.Name.Contains("Core", StringComparison.OrdinalIgnoreCase) == false);
                if (tempSensor != null) CpuTemperature = tempSensor.Value;
                var loadSensor = cpu.Sensors.FirstOrDefault(s => s.Type == SensorType.Load && s.Name.Contains("Total", StringComparison.OrdinalIgnoreCase));
                if (loadSensor != null) CpuLoad = loadSensor.Value;
            }

            foreach (var gpu in GpuList)
            {
                _sensorService.ApplySensorsToHardware(gpu);
                var tempSensor = gpu.Sensors.FirstOrDefault(s => s.Type == SensorType.Temperature);
                if (tempSensor != null) GpuTemperature = tempSensor.Value;
                var loadSensor = gpu.Sensors.FirstOrDefault(s => s.Type == SensorType.Load && s.Name.Contains("Core", StringComparison.OrdinalIgnoreCase));
                if (loadSensor != null) GpuLoad = loadSensor.Value;
            }

            var memSensors = e.Sensors.FirstOrDefault(kvp => kvp.Key.Contains("ram", StringComparison.OrdinalIgnoreCase)).Value;
            if (memSensors != null)
            {
                var memLoad = memSensors.FirstOrDefault(s => s.Type == SensorType.Load);
                if (memLoad != null) MemoryUsagePercent = memLoad.Value;
            }

            if (SelectedHardware != null)
            {
                _sensorService.ApplySensorsToHardware(SelectedHardware);
                CurrentSensors = new ObservableCollection<SensorData>(SelectedHardware.Sensors);
            }
        });
    }

    partial void OnSelectedHardwareChanged(HardwareInfoBase? value)
    {
        if (value != null)
        {
            _sensorService.ApplySensorsToHardware(value);
            CurrentSensors = new ObservableCollection<SensorData>(value.Sensors);
        }
    }

    public void Dispose()
    {
        _sensorService.SensorUpdated -= OnSensorUpdated;
        _sensorService.Dispose();
    }
}
