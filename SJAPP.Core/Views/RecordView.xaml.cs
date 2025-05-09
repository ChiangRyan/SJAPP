using SJAPP.Core.Model;
using SJAPP.Core.ViewModel;
using System.Collections.Generic;
using System.Windows;

namespace SJAPP.Views
{
    public partial class RecordView : Window
    {
        private RecordViewModel _viewModel;

        public RecordView(int deviceId, string deviceName, string currentUser, SqliteDataService dataService)
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine($"RecordView 初始化: DeviceId={deviceId}, DeviceName={deviceName}, Username={currentUser}");
            var deviceRecords = dataService.GetDeviceRecords(deviceId);
            _viewModel = new RecordViewModel(deviceRecords, currentUser, deviceId, deviceName, dataService);
            this.DataContext = _viewModel;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}