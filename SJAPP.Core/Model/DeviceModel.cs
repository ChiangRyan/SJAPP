using System.ComponentModel;
using System.Windows.Input;

namespace SJAPP.Core.Model
{
    public class DeviceModel : INotifyPropertyChanged
    {
        private string _name;
        private string _ipAddress;
        private int _slaveId;
        private int _runCount;
        private string _status;
        private bool _isOperational;

        public event PropertyChangedEventHandler PropertyChanged;
        public event System.EventHandler<(string Name, string IpAddress, int SlaveId, int RunCount,bool IsOperational)> DataChanged;

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                    NotifyDataChanged();
                }
            }
        }

        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                if (_ipAddress != value)
                {
                    _ipAddress = value;
                    OnPropertyChanged(nameof(IpAddress));
                    NotifyDataChanged();
                }
            }
        }

        public int SlaveId
        {
            get => _slaveId;
            set
            {
                if (_slaveId != value)
                {
                    _slaveId = value;
                    OnPropertyChanged(nameof(SlaveId));
                    NotifyDataChanged();
                }
            }
        }

        public int RunCount
        {
            get => _runCount;
            set
            {
                if (_runCount != value)
                {
                    _runCount = value;
                    OnPropertyChanged(nameof(RunCount));
                    NotifyDataChanged(); // 觸發 DataChanged 事件
                }
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public bool IsOperational
        {
            get => _isOperational;
            set
            {
                if (_isOperational != value)
                {
                    _isOperational = value;
                    OnPropertyChanged(nameof(IsOperational));
                    NotifyDataChanged();
                }
            }
        }

        public ICommand StartCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand RecordCommand { get; set; }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void NotifyDataChanged()
        {
            DataChanged?.Invoke(this, (Name, IpAddress, SlaveId, RunCount,IsOperational));
        }
    }
}