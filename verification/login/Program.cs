using Avalonia.Controls;
using Planilla_RRHH.Models;
using Planilla_RRHH.Security;
using Planilla_RRHH.Services.Interfaces;
using Planilla_RRHH.ViewModels;

// R-UI05 / T-UI05: expected results derive from the approved login correction scope.
// Synthetic services avoid UI startup, database, network, and real credentials.
var auth = new FakeAuth();
var dialogs = new FakeDialogs();
var successCount = 0;
var vm = new LoginViewModel(auth, dialogs, auth.Session, () => successCount++);
var notifications = new List<string?>();
var commandEvents = 0;
vm.PropertyChanged += (_, e) => notifications.Add(e.PropertyName);
vm.LoginCommand.CanExecuteChanged += (_, _) => commandEvents++;
var checks = 0;
void Check(bool condition, string id, string expected)
{
    if (!condition) throw new InvalidOperationException($"FAIL {id}: expected {expected}");
    checks++;
    Console.WriteLine($"PASS {id}: actual = expected ({expected})");
}

Check(!vm.CanLogin && !vm.LoginCommand.CanExecute(null) && vm.ErrorMessage == "", "L01", "empty form disables login and starts without error");
Check(vm.ForgotPasswordCommand.CanExecute(null), "L02", "recovery available on empty form");
await vm.ForgotPasswordCommand.ExecuteAsync(null);
Check(dialogs.WarningCount == 1 && auth.ResetCount == 0, "L03", "empty username warns without requesting reset");
notifications.Clear();
vm.Username = "synthetic.user";
Check(!vm.CanLogin && notifications.Contains(nameof(vm.CanLogin)), "L04", "username alone remains disabled and notifies CanLogin");
notifications.Clear();
vm.Password = "synthetic-password";
Check(vm.CanLogin && vm.LoginCommand.CanExecute(null) && notifications.Contains(nameof(vm.CanLogin)), "L05", "both credentials enable login and notify CanLogin");
vm.Password = "   ";
Check(!vm.CanLogin && !vm.LoginCommand.CanExecute(null), "L06", "whitespace password disables login");
vm.Password = "synthetic-password";
notifications.Clear();
var eventsBeforeLoading = commandEvents;
vm.IsLoading = true;
Check(!vm.CanLogin && !vm.LoginCommand.CanExecute(null) && notifications.Contains(nameof(vm.CanLogin)) && commandEvents > eventsBeforeLoading, "L07", "loading disables login with property and command notifications");
notifications.Clear();
var eventsBeforeRestore = commandEvents;
vm.IsLoading = false;
Check(vm.CanLogin && vm.LoginCommand.CanExecute(null) && notifications.Contains(nameof(vm.CanLogin)) && commandEvents > eventsBeforeRestore, "L08", "ending loading restores login and notifications");

auth.PendingLogin = new(TaskCreationOptions.RunContinuationsAsynchronously);
var login = vm.LoginCommand.ExecuteAsync(null);
Check(vm.IsLoading && !vm.CanLogin && !vm.LoginCommand.CanExecute(null), "L09", "pending synthetic authentication disables login");
auth.PendingLogin.SetResult((false, "Synthetic rejected credentials", null));
await login;
Check(!vm.IsLoading && vm.CanLogin && vm.ErrorMessage == "Synthetic rejected credentials" && successCount == 0, "L10", "failure displays service error and restores login without success callback");
auth.PendingLogin = null;
auth.Succeed = true;
await vm.LoginCommand.ExecuteAsync(null);
Check(successCount == 1 && vm.ErrorMessage == "" && !vm.IsLoading && auth.LastUsername == "synthetic.user" && auth.LastPassword == "synthetic-password", "L11", "success invokes callback once, clears error and forwards credentials");
auth.ThrowLogin = true;
await vm.LoginCommand.ExecuteAsync(null);
Check(vm.ErrorMessage == "Error: Synthetic service failure" && !vm.IsLoading && vm.CanLogin && successCount == 1, "L12", "exception displays error and restores login without callback");
await vm.ForgotPasswordCommand.ExecuteAsync(null);
Check(auth.ResetCount == 1 && auth.LastResetUsername == "synthetic.user" && dialogs.InfoCount == 1, "L13", "username recovery requests service and reports success");
auth.ResetSucceed = false;
await vm.ForgotPasswordCommand.ExecuteAsync(null);
Check(auth.ResetCount == 2 && dialogs.ErrorCount == 1 && dialogs.LastError == "Synthetic reset failure", "L14", "recovery failure displays service error");
Console.WriteLine($"R-UI05/T-UI05: {checks}/{checks} checks passed; synthetic services only, no UI or database startup.");

sealed class FakeAuth : IAuthService
{
    public SessionContext Session { get; } = new();
    public TaskCompletionSource<(bool success, string? error, User? user)>? PendingLogin;
    public bool Succeed;
    public bool ThrowLogin;
    public bool ResetSucceed = true;
    public int ResetCount;
    public string? LastUsername, LastPassword, LastResetUsername;
    public Task<(bool success, string? error, User? user)> LoginAsync(string username, string password)
    {
        LastUsername = username;
        LastPassword = password;
        if (ThrowLogin) throw new InvalidOperationException("Synthetic service failure");
        return PendingLogin?.Task ?? Task.FromResult<(bool, string?, User?)>((Succeed, Succeed ? null : "Synthetic rejected credentials", null));
    }
    public Task<(bool success, string? error)> RequestPasswordResetAsync(string username)
    {
        ResetCount++;
        LastResetUsername = username;
        return Task.FromResult<(bool, string?)>((ResetSucceed, ResetSucceed ? null : "Synthetic reset failure"));
    }
    public Task<(bool success, string? error)> ChangePasswordAsync(int id, string current, string next) => throw new NotSupportedException();
    public Task<(bool success, string? error, User? user)> ResetPasswordAsync(string token, string next) => throw new NotSupportedException();
    public Task<bool> ValidateSessionAsync() => throw new NotSupportedException();
    public void Logout() => throw new NotSupportedException();
}

sealed class FakeDialogs : IDialogService
{
    public int WarningCount, InfoCount, ErrorCount;
    public string? LastError;
    public Task ShowWarningAsync(string title, string message) { WarningCount++; return Task.CompletedTask; }
    public Task ShowInfoAsync(string title, string message) { InfoCount++; return Task.CompletedTask; }
    public Task ShowErrorAsync(string title, string message) { ErrorCount++; LastError = message; return Task.CompletedTask; }
    public void SetOwner(Window owner) => throw new NotSupportedException();
    public Task<bool> ShowConfirmAsync(string title, string message, string confirmText = "Confirmar", string cancelText = "Cancelar") => throw new NotSupportedException();
    public Task<string?> ShowInputAsync(string title, string message, string placeholder = "") => throw new NotSupportedException();
    public Task<string?> ShowFileOpenAsync(string title, string? filter = null) => throw new NotSupportedException();
    public Task<string?> ShowFileSaveAsync(string title, string? filter = null, string? defaultName = null) => throw new NotSupportedException();
    public Task<string?> ShowFolderSelectAsync(string title) => throw new NotSupportedException();
}
