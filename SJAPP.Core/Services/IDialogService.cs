using SJAPP.Core.Model;

namespace SJAPP.Core.Service
{
    public interface ILoginDialogService
    {
        (bool Success, string Username, string Password) ShowLoginDialog();
        void ClearNavigationSelection();
    }

    public interface IRecordDialogService
    {
        (string deviceName, string username, int deviceId) ShowRecordDialog(int deviceId, string deviceName, string username);
    }
}
