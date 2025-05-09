using SJAPP.Core.Model;
using SJAPP.Core.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;


namespace SJAPP.Views
{
    public partial class RecordView : Window
    {
        public RecordView(List<DeviceRecord> records)
        {
            InitializeComponent();
            this.DataContext = new RecordViewModel(records);
        }
    }

}
