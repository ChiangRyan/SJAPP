using SJAPP.Core.Model;
using SJAPP.Core.Services.Communication;
using SJAPP.Core.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Timers;
using PropertyChanged;
using System.Linq;

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

            // 先從資料庫載入設備數據
            LoadDevicesFromDatabase();

            // 如果資料庫中沒有設備數據，則初始化預設設備
            if (!Devices.Any())
            {
                InitializeDefaultDevices();
            }

            _updateTimer = new Timer(5000);
            _updateTimer.Elapsed += async (s, e) => await UpdateDeviceData();
            _updateTimer.AutoReset = true;
        }

        private void LoadDevicesFromDatabase()
        {
            var deviceDataList = _dataService.GetDeviceData();
            foreach (var data in deviceDataList)
            {
                string ipAddress;
                int slaveId;
                if (deviceDataList.IndexOf(data) < 10)
                {
                    ipAddress = "192.168.64.52";
                    slaveId = deviceDataList.IndexOf(data) + 1;
                }
                else if (deviceDataList.IndexOf(data) == 10)
                {
                    ipAddress = "192.168.64.87";
                    slaveId = 1;
                }
                else
                {
                    ipAddress = "192.168.64.89";
                    slaveId = 1;
                }
                
                var device = new DeviceModel
                {
                    Name = data.Name ?? $"設備 {deviceDataList.IndexOf(data) + 1}", // 使用資料庫名稱，無則用預設
                    IpAddress = data.IpAddress ?? ipAddress,
                    SlaveId = data.SlaveId > 0 ? data.SlaveId : slaveId,
                    RunCount = data.RunCount,
                    Status = "未知",
                    IsOperational = data.IsOperational
                };
                
                int deviceIndex = deviceDataList.IndexOf(data);
                device.StartCommand = new RelayCommand(async () => await ExecuteStart(deviceIndex));
                device.StopCommand = new RelayCommand(async () => await ExecuteStop(deviceIndex));

                device.DataChanged += (sender, e) => DeviceDataChanged(deviceIndex, e.Name, e.IpAddress, e.SlaveId, e.IsOperational, e.RunCount);

                Devices.Add(device);
                System.Diagnostics.Debug.WriteLine($" Name={device.Name}, IP={device.IpAddress}, SlaveId={device.SlaveId}, IsOperational={device.IsOperational}, RunCount={device.RunCount}");
            }
        }

        private void InitializeDefaultDevices()
        {
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

                var device = new DeviceModel
                {
                    Name = $"設備 {i + 1}",
                    IpAddress = ipAddress,
                    SlaveId = slaveId,
                    RunCount = 0,
                    Status = "未知",
                    IsOperational = false
                };

                int deviceIndex = i;
                device.StartCommand = new RelayCommand(async () => await ExecuteStart(deviceIndex));
                device.StopCommand = new RelayCommand(async () => await ExecuteStop(deviceIndex));

                device.DataChanged += (sender, e) => DeviceDataChanged(deviceIndex, e.Name, e.IpAddress, e.SlaveId, e.IsOperational, e.RunCount);

                Devices.Add(device);
                _dataService.SaveDeviceData(deviceIndex, device.Name, device.IpAddress, device.SlaveId, device.IsOperational, device.RunCount);
                System.Diagnostics.Debug.WriteLine($"Initialized default device {i + 1}: Name={device.Name}, IP={device.IpAddress}, SlaveId={device.SlaveId}, IsOperational={device.IsOperational}, RunCount={device.RunCount}");
            }
        }

        [SuppressPropertyChangedWarnings]
        private void DeviceDataChanged(int deviceIndex, string name, string ipAddress, int slaveId, bool isOperational, int runCount)
        {
            var device = Devices[deviceIndex];
            if (device != null)
            {
                device.Name = name;
                device.IpAddress = ipAddress;
                device.SlaveId = slaveId;
                device.IsOperational = isOperational;
                device.RunCount = runCount;
                _dataService.SaveDeviceData(deviceIndex, name, ipAddress, slaveId, isOperational, runCount);
                System.Diagnostics.Debug.WriteLine($"Saved changes for device {deviceIndex + 1}: Name={name}, IP={ipAddress}, SlaveId={slaveId}, IsOperational={isOperational}, RunCount={runCount}");
            }
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
                await _communicationService.WriteModbusAsync(device.IpAddress, device.SlaveId, _controlAddress, 1, 6);
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