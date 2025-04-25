using PropertyChanged;
using SJAPP.Core.Model;
using SJAPP.Core.Services.Communication;
using System.Windows.Input;
using SJAPP.Core.Helpers;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace SJAPP.Core.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class ManualOperationViewModel : ViewModelBase
    {
        private readonly ICommunicationService _communicationService;
        private readonly SqliteDataService _dataService;

        // UI 繫結屬性
        public string IpAddress { get; set; } = "192.168.64.87";
        public int ReadSlaveId { get; set; } = 1;
        public int ReadAddress { get; set; } = 0;
        public int ReadQuantity { get; set; } = 2;
        public int ReadFunctionCode { get; set; } = 3; // 預設功能碼
        public int WriteSlaveId { get; set; } = 1;
        public int WriteAddress { get; set; } = 0;
        public int WriteValue { get; set; } = 100;
        public string StatusText { get; set; } = "";

        // 命令
        public ICommand TurnOnLedCommand { get; }
        public ICommand TurnOffLedCommand { get; }
        public ICommand ReadModbusCommand { get; }
        public ICommand WriteModbusCommand { get; }
        public ICommand ViewHistoryCommand { get; }

        public ManualOperationViewModel(ICommunicationService communicationService, SqliteDataService dataService)
        {
            Debug.WriteLine("Creating HomeViewModel...");
            _communicationService = communicationService;
            _dataService = dataService;

            // 初始化命令
            TurnOnLedCommand = new RelayCommand(async () => await ExecuteTurnOnLed());
            TurnOffLedCommand = new RelayCommand(async () => await ExecuteTurnOffLed());
            ReadModbusCommand = new RelayCommand(async () => await ExecuteReadModbus());
            WriteModbusCommand = new RelayCommand(async () => await ExecuteWriteModbus());

            Debug.WriteLine("ManualOperationViewModel created.");
        }

        private async Task ExecuteTurnOnLed()
        {
            if (string.IsNullOrEmpty(IpAddress))
            {
                StatusText = $"[{DateTime.Now}] 錯誤: 請輸入 ESP32 的 IP 位址\n" + StatusText;
                return;
            }

            try
            {
                string response = await _communicationService.SendLedCommandAsync(IpAddress, "ON");
                StatusText = $"[{DateTime.Now}] LED 控制回應: {response}\n" + StatusText;
            }
            catch (Exception ex)
            {
                StatusText = $"[{DateTime.Now}] LED 控制錯誤: {ex.Message}\n" + StatusText;
            }
        }

        private async Task ExecuteTurnOffLed()
        {
            if (string.IsNullOrEmpty(IpAddress))
            {
                StatusText = $"[{DateTime.Now}] 錯誤: 請輸入 ESP32 的 IP 位址\n" + StatusText;
                return;
            }

            try
            {
                string response = await _communicationService.SendLedCommandAsync(IpAddress, "OFF");
                StatusText = $"[{DateTime.Now}] LED 控制回應: {response}\n" + StatusText;
            }
            catch (Exception ex)
            {
                StatusText = $"[{DateTime.Now}] LED 控制錯誤: {ex.Message}\n" + StatusText;
            }
        }

        private async Task ExecuteReadModbus()
        {
            if (string.IsNullOrEmpty(IpAddress))
            {
                StatusText = $"[{DateTime.Now}] 錯誤: 請輸入 ESP32 的 IP 位址\n" + StatusText;
                return;
            }

            try
            {
                var modbusResult = await _communicationService.ReadModbusAsync(IpAddress, ReadSlaveId, ReadAddress, ReadQuantity, ReadFunctionCode);
                if (modbusResult.Status == "success")
                {
                    try
                    {
                        StatusText = $"[{DateTime.Now}] Modbus 讀取成功 (從站 {modbusResult.SlaveId}): {string.Join(", ", modbusResult.Data)}\n" + StatusText;
                    }
                    catch (Exception ex)
                    {
                        StatusText = $"[{DateTime.Now}] 儲存 Modbus 數據失敗: {ex.Message}\n詳細資訊: {ex}\n" + StatusText;
                    }
                }
                else
                {
                    StatusText = $"[{DateTime.Now}] Modbus 讀取失敗: {JsonSerializer.Serialize(modbusResult)}\n" + StatusText;
                }
            }
            catch (Exception ex)
            {
                StatusText = $"[{DateTime.Now}] Modbus 讀取錯誤: {ex.Message}\n詳細資訊: {ex}\n" + StatusText;
            }
        }

        private async Task ExecuteWriteModbus()
        {
            if (string.IsNullOrEmpty(IpAddress))
            {
                StatusText = $"[{DateTime.Now}] 錯誤: 請輸入 ESP32 的 IP 位址\n" + StatusText;
                return;
            }

            try
            {
                string response = await _communicationService.WriteModbusAsync(IpAddress, WriteSlaveId, WriteAddress, WriteValue);
                StatusText = $"[{DateTime.Now}] Modbus 寫入回應: {response}\n" + StatusText;
            }
            catch (Exception ex)
            {
                StatusText = $"[{DateTime.Now}] Modbus 寫入錯誤: {ex.Message}\n" + StatusText;
            }
        }

    }
}