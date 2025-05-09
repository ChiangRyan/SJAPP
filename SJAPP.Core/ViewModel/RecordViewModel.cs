using PropertyChanged;
using SJAPP.Core.Model;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using SJAPP.Core.Helpers;

namespace SJAPP.Core.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class RecordViewModel : ViewModelBase
    {
        private readonly SqliteDataService _dataService;
        private readonly string _currentUsername;
        private readonly int _deviceId;
        private readonly string _deviceName;

        // 記錄集合
        public ObservableCollection<DeviceRecord> DeviceRecords { get; set; }

        // 記錄內容
        public string RecordContent { get; set; }

        // 命令
        public ICommand AddRecordCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public RecordViewModel(List<DeviceRecord> records, string username, int deviceId, string deviceName, SqliteDataService dataService)
        {
            _dataService = dataService;
            _currentUsername = username;
            _deviceId = deviceId;
            _deviceName = deviceName;
            Debug.WriteLine($"RecordViewModel 初始化: DeviceId={_deviceId}, DeviceName={_deviceName}, Username={_currentUsername}");
            if (!_dataService.DeviceExists(_deviceId))
            {
                System.Diagnostics.Debug.WriteLine($"無效的 DeviceId: {_deviceId}");
                MessageBox.Show($"設備 ID {_deviceId} 不存在於資料庫中，請選擇有效設備", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // 初始化記錄集合
            DeviceRecords = new ObservableCollection<DeviceRecord>(records.OrderByDescending(r => r.Timestamp));

            // 初始化命令
            AddRecordCommand = new RelayCommand(AddRecord, CanAddRecord);
            RefreshCommand = new RelayCommand(RefreshRecords);
        }

        private bool CanAddRecord()
        {
            return !string.IsNullOrWhiteSpace(RecordContent);
        }

        private void AddRecord()
        {
            try
            {
                Debug.WriteLine(
                   $" AddRecord: DeviceId={_deviceId}," +
                   $" DeviceName={_deviceName}," +
                   $" Username={_currentUsername}," +
                   $" Content={RecordContent}"
                );

                // 建立新記錄
                var newRecord = new DeviceRecord
                {
                    DeviceId = _deviceId,
                    DeviceName = _deviceName,
                    Username = _currentUsername,
                    Content = RecordContent.Trim(),
                    Timestamp = DateTime.Now
                };

                // 保存到資料庫
                _dataService.AddDeviceRecord(newRecord);

                // 刷新顯示
                RefreshRecords();

                // 清空輸入
                RecordContent = string.Empty;

                MessageBox.Show("記錄已成功添加！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AddRecord failed: {ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"添加記錄時發生錯誤: {ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshRecords()
        {
            try
            {
                // 從資料庫中重新讀取此設備的記錄
                var records = _dataService.GetDeviceRecords(_deviceId);

                // 更新記錄集合
                DeviceRecords.Clear();
                foreach (var record in records.OrderByDescending(r => r.Timestamp))
                {
                    DeviceRecords.Add(record);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"刷新記錄時發生錯誤: {ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
