using System;
using System.Windows;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using SJAPP.Core.ViewModel;
using SJAPP.Properties;

namespace SJAPP.Views
{
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private Home _homePage; // 可選：用於緩存 Home 頁面
        private ManualOperation _manualoperationPage; // 可選：用於緩存 ManualOperation 頁面

        public MainWindow()
        {
            Debug.WriteLine("Creating MainWindow...");
            InitializeComponent();

            // 從 App.xaml.cs 獲取 IServiceProvider
            _serviceProvider = (App.Current as App)?.ServiceProvider;
            if (_serviceProvider == null)
            {
                throw new InvalidOperationException("ServiceProvider is not initialized.");
            }

            // 創建 Home 頁面並設置 DataContext
            _homePage = new Home
            {
                DataContext = _serviceProvider.GetService<HomeViewModel>()
            };


            // 預設導航到 Home 頁面
            HomeButton.IsChecked = true;
            MainFrame.Navigate(_homePage);

            Debug.WriteLine("MainWindow created.");
        }

        // 首頁
        private void HomeButton_Checked(object sender, RoutedEventArgs e)
        {
            // 使用已緩存的 Home 頁面，或者創建新實例
            if (_homePage == null)
            {
                _homePage = new Home
                {
                    DataContext = _serviceProvider.GetService<HomeViewModel>()
                };
            }
            MainFrame.Navigate(_homePage);
        }

        // 手動操作
        private void ManualOperationButton_Checked(object sender, RoutedEventArgs e)
        {
            // 使用已緩存的 Home 頁面，或者創建新實例
            if (_manualoperationPage == null)
            {
                _manualoperationPage = new ManualOperation
                {
                    DataContext = _serviceProvider.GetService<ManualOperationViewModel>()
                };
            }
            MainFrame.Navigate(_manualoperationPage);

        }

        // 監控
        private void MonitorButton_Checked(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Monitor());
        }

        // 預警
        private void WarningButton_Checked(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AlarmWarning());
        }

        // 設置
        private void SettingsButton_Checked(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Settings());
        }

        // 最小化窗口
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // 關閉窗口
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}