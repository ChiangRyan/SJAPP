using PropertyChanged;
using SJAPP.Core.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SJAPP.Core.Helpers;


namespace SJAPP.Core.ViewModel
    
    {
    [AddINotifyPropertyChangedInterface]
    public class RecordViewModel : ViewModelBase
    {
        public ObservableCollection<DeviceRecord> DeviceRecords { get; set; }

        public RecordViewModel(List<DeviceRecord> records)
        {
            DeviceRecords = new ObservableCollection<DeviceRecord>(records);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

}
