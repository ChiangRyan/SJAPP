using SJAPP.Core.Model;
using SJAPP.Core.Services.Communication;
using SJAPP.Core.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Timers;
using PropertyChanged;

namespace SJAPP.Core.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class HomeViewModel : ViewModelBase
    {
        private readonly ICommunicationService _communicationService;
        private readonly SqliteDataService _dataService;
        private readonly Timer _updateTimer;
        private readonly int _runCountAddress = 10;
        private readonly int _statusAddress = 1;
        private readonly int _controlAddress = 0;

        public ObservableCollection<DeviceModel> Devices { get; set; }

        public HomeViewModel(ICommunicationService communicationService, SqliteDataService dataService)
        {
            _communicationService = communicationService;
            _dataService = dataService;

            Devices = new ObservableCollection<DeviceModel>();
            var deviceData = _dataService.GetDeviceData();

            for (int i = 0; i < 12; i++)
            {
                string ipAddress;
                int slaveId;
                if (i < 10)
                {
                    ipAddress = "192.168.64.52";
                    slaveId = i + 1;
                }
                else if (i == 10)
                {
                    ipAddress = "192.168.64.87";
                    slaveId = 1;
                }
                else
                {
                    ipAddress = "192.168.64.89";
                    slaveId = 1;
                }

                string name = $"設備 {i + 1}";
                bool isOperational = true;
                int runCount = 0;
                if (i < deviceData.Count)
                {
                    if (!string.IsNullOrEmpty(deviceData[i].Name))
                    {
                        name = deviceData[i].Name;
                    }
                    if (!string.IsNullOrEmpty(deviceData[i].IpAddress))
                    {
                        ipAddress = deviceData[i].IpAddress;
                    }
                    if (deviceData[i].SlaveId > 0)
                    {
                        slaveId = deviceData[i].SlaveId;
                    }
                    isOperational = deviceData[i].IsOperational;
                    runCount = deviceData[i].RunCount;
                }

                var device = new DeviceModel
                {
                    Name = name,
                    IpAddress = ipAddress,
                    SlaveId = slaveId,
                    RunCount = runCount,
                    Status = "未知",
                    IsOperational = isOperational
                };
                int deviceIndex = i;
                device.StartCommand = new RelayCommand(async () => await ExecuteStart(deviceIndex));
                device.StopCommand = new RelayCommand(async () => await ExecuteStop(deviceIndex));
                device.DataChanged += (sender, e) => DeviceDataChanged(deviceIndex, e.Name, e.IpAddress, e.SlaveId, e.IsOperational, e.RunCount);

                Devices.Add(device);
                System.Diagnostics.Debug.WriteLine($"Added device {i + 1}: Name={device.Name}, IP={device.IpAddress}, SlaveId={device.SlaveId}, IsOperational={device.IsOperational}, RunCount={device.RunCount}");

                _dataService.SaveDeviceData(deviceIndex, device.Name, device.IpAddress, device.SlaveId, device.IsOperational, device.RunCount);
            }

            _updateTimer = new Timer(5000);
            _updateTimer.Elapsed += async (s, e) => await UpdateDeviceData();
            _updateTimer.AutoReset = true;
        }

        [SuppressPropertyChangedWarnings]
        private void DeviceDataChanged(int deviceIndex, string name, string ipAddress, int slaveId, bool isOperational, int runCount)
        {
            _dataService.SaveDeviceData(deviceIndex, name, ipAddress, slaveId, isOperational, runCount);
        }

        private async Task UpdateDeviceData()
        {
            foreach (var device in Devices)
            {
                if (!device.IsOperational)
                {
                    System.Diagnostics.Debug.WriteLine($"Skipping update for device {device.Name}: Device is not operational.");
                    continue;
                }

                try
                {
                    var statusResult = await _communicationService.ReadModbusAsync(device.IpAddress, device.SlaveId, _statusAddress, 1, 3);
                    if (statusResult.Status == "success" && statusResult.Data.Count > 0)
                    {
                        int statusValue = statusResult.Data[0];
                        switch (statusValue)
                        {
                            case 0:
                                device.Status = "閒置";
                                break;
                            case 1:
                                device.Status = "運行中";
                                break;
                            case 2:
                                device.Status = "故障";
                                break;
                            default:
                                device.Status = "未知";
                                break;
                        }
                        System.Diagnostics.Debug.WriteLine($"Device {device.Name} status updated: {device.Status}");
                    }
                    else
                    {
                        device.Status = "通訊失敗";
                        device.IsOperational = false; // 通訊失敗時，自動設為不運作
                        System.Diagnostics.Debug.WriteLine($"Failed to read status for device {device.Name}: {statusResult.Message}");
                    }

                    if (device.IsOperational) // 只有在仍運作時才繼續讀取跑合次數
                    {
                        var runCountResult = await _communicationService.ReadModbusAsync(device.IpAddress, device.SlaveId, _runCountAddress, 2, 3);
                        if (runCountResult.Status == "success" && runCountResult.Data.Count >= 2)
                        {
                            int lowWord = runCountResult.Data[0];
                            int highWord = runCountResult.Data[1];
                            device.RunCount = (highWord << 16) | (lowWord & 0xFFFF);
                            System.Diagnostics.Debug.WriteLine($"Device {device.Name} run count updated: {device.RunCount}");
                        }
                        else
                        {
                            device.Status = "通訊失敗";
                            device.IsOperational = false; // 通訊失敗時，自動設為不運作
                            System.Diagnostics.Debug.WriteLine($"Failed to read run count for device {device.Name}: {runCountResult.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to update device {device.Name}: {ex.Message}");
                    device.Status = "通訊失敗";
                    device.IsOperational = false; // 異常時也設為不運作
                }
            }
        }

        private async Task ExecuteStart(int deviceIndex)
        {
            var device = Devices[deviceIndex];
            if (!device.IsOperational || device.Status == "運行中" || device.Status == "通訊失敗")
            {
                System.Diagnostics.Debug.WriteLine($"Cannot start device {device.Name}: Device is not operational, already running, or communication failed.");
                return;
            }

            device.Status = "啟動中...";
            try
            {
                _updateTimer.Stop();
                System.Diagnostics.Debug.WriteLine("Update timer stopped for ExecuteStart.");
                await _communicationService.WriteModbusAsync(device.IpAddress, device.SlaveId, _controlAddress, 1,6);
                await UpdateDeviceData();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ExecuteStart failed: {ex.Message}");
                device.Status = "啟動失敗";
            }
            finally
            {
                _updateTimer.Start();
                System.Diagnostics.Debug.WriteLine("Update timer restarted after ExecuteStart.");
            }
        }

        private async Task ExecuteStop(int deviceIndex)
        {
            var device = Devices[deviceIndex];
            if (!device.IsOperational || device.Status == "閒置" || device.Status == "通訊失敗")
            {
                System.Diagnostics.Debug.WriteLine($"Cannot stop device {device.Name}: Device is not operational, already idle, or communication failed.");
                return;
            }

            device.Status = "停止中...";
            try
            {
                _updateTimer.Stop();
                System.Diagnostics.Debug.WriteLine("Update timer stopped for ExecuteStop.");
                await _communicationService.WriteModbusAsync(device.IpAddress, device.SlaveId, _controlAddress, 0, 6);
                await UpdateDeviceData();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ExecuteStop failed: {ex.Message}");
                device.Status = "停止失敗";
            }
            finally
            {
                _updateTimer.Start();
                System.Diagnostics.Debug.WriteLine("Update timer restarted after ExecuteStop.");
            }
        }

        public void StopPolling()
        {
            _updateTimer.Stop();
            _communicationService.CleanupConnections();
            System.Diagnostics.Debug.WriteLine("Update timer stopped and connections cleaned up due to page unload.");
        }

        public async void StartPolling()
        {
            await UpdateDeviceData();
            if (!_updateTimer.Enabled)
            {
                _updateTimer.Start();
                System.Diagnostics.Debug.WriteLine("Update timer started due to page load.");
            }
        }
    }
}