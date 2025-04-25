using Microsoft.Extensions.DependencyInjection;
using SJAPP.Core.Services.Communication;
using SJAPP.Core.ViewModel;
using SJAPP.Core.Model;
using System;
using System.Windows;
using SQLitePCL;

namespace SJAPP.Views
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Entering OnStartup...");

            try
            {
                // 初始化 SQLite 提供者
                Batteries.Init();
                System.Diagnostics.Debug.WriteLine("SQLite provider initialized.");

                System.Diagnostics.Debug.WriteLine("Calling base.OnStartup...");
                base.OnStartup(e);
                System.Diagnostics.Debug.WriteLine("base.OnStartup called successfully.");

                System.Diagnostics.Debug.WriteLine("Starting application...");

                // 設置 DI 容器
                var services = new ServiceCollection();
                System.Diagnostics.Debug.WriteLine("Adding services to DI container...");
                services.AddSingleton<ICommunicationService, CommunicationService>();
                services.AddSingleton<SqliteDataService>();
                services.AddSingleton<HomeViewModel>();
                services.AddSingleton<ManualOperationViewModel>();
                System.Diagnostics.Debug.WriteLine("Services added to DI container.");

                System.Diagnostics.Debug.WriteLine("Building service provider...");
                ServiceProvider = services.BuildServiceProvider();
                System.Diagnostics.Debug.WriteLine("Service provider built successfully.");

                // 創建並顯示主視窗
                System.Diagnostics.Debug.WriteLine("Creating MainWindow...");
                var mainWindow = new MainWindow();
                System.Diagnostics.Debug.WriteLine("MainWindow created. Showing window...");
                mainWindow.Show();
                System.Diagnostics.Debug.WriteLine("MainWindow shown.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Application startup failed: {ex}");
                MessageBox.Show($"應用程式啟動失敗：{ex.Message}\n\n詳細資訊：{ex}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Application exiting...");
            var communicationService = ServiceProvider?.GetService<ICommunicationService>();
            communicationService?.CleanupConnections(); // 改為調用 CleanupConnections
            base.OnExit(e);
        }
    }
}