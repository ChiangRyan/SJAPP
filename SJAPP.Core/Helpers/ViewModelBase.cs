using System.ComponentModel;
using System.Diagnostics;
namespace SJAPP.Core.Helpers
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            Debug.WriteLine($"OnPropertyChanged called for property: {propertyName}, Has subscribers: {PropertyChanged != null}");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}