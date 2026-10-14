using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Planilla_RRHH.Data;
using Planilla_RRHH.Data.Repositories;
using Planilla_RRHH.Security;
using Planilla_RRHH.Services;
using Planilla_RRHH.Services.Interfaces;
using Planilla_RRHH.ViewModels;
using Planilla_RRHH.ViewModels.Employees;
using Planilla_RRHH.Views;

namespace Planilla_RRHH;

public partial class App : Application
{
    public static IServiceProvider? Services { get; private set; }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    public override void OnFrameworkInitializationCompleted()
    {
        var services = ConfigureServices();
        Services = services.BuildServiceProvider();
        
        // Initialize database
        var db = Services.GetRequiredService<Database>();
        db.Initialize();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var dialogService = Services.GetRequiredService<IDialogService>();
            
            // Show login window first
            var loginWindow = Services.GetRequiredService<LoginWindow>();
            loginWindow.DataContext = ActivatorUtilities.CreateInstance<LoginViewModel>(Services, (Action)(() =>
            {
                var mainViewModel = Services.GetRequiredService<MainViewModel>();
                var mainWindow = Services.GetRequiredService<MainWindow>();
                mainWindow.DataContext = mainViewModel;
                mainViewModel.Initialize();
                dialogService.SetOwner(mainWindow);
                desktop.MainWindow = mainWindow;
                mainWindow.Show();
                loginWindow.Close();
            }));
            dialogService.SetOwner(loginWindow);
            desktop.MainWindow = loginWindow;
        }
        
        base.OnFrameworkInitializationCompleted();
    }
    
    private static ServiceCollection ConfigureServices()
    {
        var services = new ServiceCollection();
        
        // Data layer
        services.AddSingleton<SqliteConnectionFactory>(sp =>
        {
            var dbPath = GetDatabasePath();
            return new SqliteConnectionFactory(dbPath);
        });
        services.AddSingleton<Database>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<ISettingsRepository, SettingsRepository>();
        services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
        
        // Security
        services.AddSingleton<SessionContext>();
        
        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<ISettingsService, SettingsService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBackupService, BackupService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddSingleton<IDialogService, DialogService>();
        
        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<EmployeeListViewModel>();
        services.AddTransient<EmployeeFormViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<UsersViewModel>();
        services.AddTransient<BackupViewModel>();
        
        // Views
        services.AddTransient<MainWindow>();
        services.AddTransient<LoginWindow>();
        services.AddTransient<DashboardView>();
        services.AddTransient<EmployeeListView>();
        services.AddTransient<EmployeeFormView>();
        services.AddTransient<SettingsView>();
        services.AddTransient<UsersView>();
        services.AddTransient<BackupView>();
        
        return services;
    }
    
    private static string GetDatabasePath()
    {
        string baseDir;
        if (OperatingSystem.IsWindows())
        {
            baseDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
        else
        {
            baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }
        
        var appDir = Path.Combine(baseDir, "Planilla-RRHH");
        Directory.CreateDirectory(appDir);
        return Path.Combine(appDir, "planilla.db");
    }
}
