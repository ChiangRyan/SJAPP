using SJAPP.Core.Service;
using SJAPP.Core.Views;
using SJAPP.Views;
using System;
using System.Windows;

namespace SJAPP.Core.Model
{
    public class RecordService : IRecordDialogService
    {
        private readonly SqliteDataService _dataService;

        public RecordService(SqliteDataService dataService)
        {
            _dataService = dataService;
        }

        public (string deviceName, string username, int deviceId) ShowRecordDialog(int deviceId, string deviceName, string username)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"ShowRecordDialog: DeviceId={deviceId}, DeviceName={deviceName}, Username={username}");
                if (!_dataService.DeviceExists(deviceId))
                {
                    System.Diagnostics.Debug.WriteLine($"無效的 DeviceId: {deviceId}");
                    MessageBox.Show($"設備 ID {deviceId} 不存在於資料庫中", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                    return (deviceName, username, deviceId);
                }

                var recordView = new RecordView(deviceId, deviceName, username, _dataService);
                recordView.ShowDialog();
                return (deviceName, username, deviceId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ShowRecordDialog failed: {ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"顯示記錄視窗失敗: {ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                return (deviceName, username, deviceId);
            }
        }
    }
}
