using System.ComponentModel;

namespace SJAPP.Core.Helpers
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            System.Diagnostics.Debug.WriteLine($"OnPropertyChanged called for property: {propertyName}, Has subscribers: {PropertyChanged != null}");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}