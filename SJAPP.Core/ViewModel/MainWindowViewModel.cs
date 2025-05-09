using SJAPP.Core.Helpers;
using SJAPP.Core.Model;
using SJAPP.Core.Service;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System;


namespace SJAPP.Core.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ILoginDialogService _loginDialogService;
        private readonly PermissionService _permissionService;
        private readonly Frame _mainFrame;
        private string _username;
        private string _password;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public bool IsLoggedIn => _permissionService.IsLoggedIn;
        public bool CanViewHome => _permissionService.HasPermission(Permission.ViewHome);

        public bool CanViewManualOperation => _permissionService.HasPermission(Permission.ViewManualOperation);
        public bool CanViewMonitor => _permissionService.HasPermission(Permission.ViewMonitor);
        public bool CanViewWarning => _permissionService.HasPermission(Permission.ViewWarning);
        public bool CanViewSettings => _permissionService.HasPermission(Permission.ViewSettings);
        public bool CanControlDevice => _permissionService.HasPermission(Permission.ControlDevice);

        public bool CanLogin => !IsLoggedIn;
        public bool CanLogout => IsLoggedIn;

        public ICommand LoginCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand NavigateCommand { get; }
        public ICommand ShowLoginCommand { get; }

        public MainWindowViewModel(PermissionService permissionService, Frame mainFrame, ILoginDialogService DialogService)
        {
            _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
            _mainFrame = mainFrame ?? throw new ArgumentNullException(nameof(mainFrame));
            _loginDialogService = DialogService;
            _username = string.Empty;
            _password = string.Empty;

            _permissionService.PermissionsChanged += (s, e) =>
            {
                UpdatePermissionProperties();
                OnPropertyChanged(nameof(IsLoggedIn));
                OnPropertyChanged(nameof(CanLogin));
                OnPropertyChanged(nameof(CanLogout));

                System.Diagnostics.Debug.WriteLine("PermissionsChanged event received.");
            };

            LoginCommand = new RelayCommand(ExecuteLogin, () => CanLogin);
            LogoutCommand = new RelayCommand(ExecuteLogout, () => CanLogout);
            NavigateCommand = new RelayCommand<string>(ExecuteNavigate);
            ShowLoginCommand = new RelayCommand(ExecuteShowLogin);

            System.Diagnostics.Debug.WriteLine("MainWindowViewModel initialized, IsLoggedIn: " + IsLoggedIn);
        }

        private void ExecuteLogin()
        {
            try
            {
                if (_permissionService.Login(Username, Password))
                {
                    UpdatePermissionProperties();
                    OnPropertyChanged(nameof(IsLoggedIn));
                    OnPropertyChanged(nameof(CanLogin));
                    OnPropertyChanged(nameof(CanLogout));
                    Username = string.Empty;
                    Password = string.Empty;
                    (LogoutCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    System.Diagnostics.Debug.WriteLine($"Login successful, user: {_permissionService.CurrentUser.Username}");
                }
                else
                {
                    System.Windows.MessageBox.Show("登入失敗：使用者名稱或密碼錯誤。", "登入錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                    // 僅在需要時重新顯示
                    if (!string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Password))
                    {
                        ExecuteShowLogin();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login failed: {ex.Message}\nStackTrace: {ex.StackTrace}");
                System.Windows.MessageBox.Show($"登入失敗：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                // 僅在需要時重新顯示
                if (!string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Password))
                {
                    ExecuteShowLogin();
                }
            }
        }

        private void ExecuteLogout()
        {
            _permissionService.Logout();
            _mainFrame.Navigate(null);
            UpdatePermissionProperties();
            OnPropertyChanged(nameof(IsLoggedIn));
            OnPropertyChanged(nameof(CanLogin));
            OnPropertyChanged(nameof(CanLogout));
            // 使用接口調用方法
            _loginDialogService.ClearNavigationSelection();
            System.Diagnostics.Debug.WriteLine("Logout successful.");
        }

        private void ExecuteNavigate(string viewUri)
        {
            if (string.IsNullOrEmpty(viewUri)) return;
            try
            {
                _mainFrame.Navigate(new Uri(viewUri, UriKind.Relative));
                System.Diagnostics.Debug.WriteLine($"Navigated to: {viewUri}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation failed: {ex.Message}\nStackTrace: {ex.StackTrace}");
                System.Windows.MessageBox.Show($"導航失敗：{ex.Message}", "錯誤", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        public void ExecuteShowLogin()
        {
            try
            {
                var (success, username, password) = _loginDialogService.ShowLoginDialog();
                if (success)
                {
                    Username = username;
                    Password = password;
                    ExecuteLogin();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"登入窗口錯誤: {ex.Message}");
            }
        }


        private void UpdatePermissionProperties()
        {
            OnPropertyChanged(nameof(CanViewHome));
            OnPropertyChanged(nameof(CanViewManualOperation));
            OnPropertyChanged(nameof(CanViewMonitor));
            OnPropertyChanged(nameof(CanViewWarning));
            OnPropertyChanged(nameof(CanViewSettings));
            OnPropertyChanged(nameof(CanControlDevice));
            System.Diagnostics.Debug.WriteLine("Permission properties updated.");
        }

        public void NotifyPermissionsChanged()
        {
            UpdatePermissionProperties();
            System.Diagnostics.Debug.WriteLine("NotifyPermissionsChanged called.");
        }
    }
}