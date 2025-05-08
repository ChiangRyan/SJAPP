using SJAPP.Core.Model;

namespace SJAPP.Core.Service
{
    public interface ILoginDialogService
    {
        (bool Success, string Username, string Password) ShowLoginDialog();

        // 添加清除導航選擇的方法
        void ClearNavigationSelection();
    }
}
