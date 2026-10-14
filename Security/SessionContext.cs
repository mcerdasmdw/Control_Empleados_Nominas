using CommunityToolkit.Mvvm.ComponentModel;
using Planilla_RRHH.Models;
using Planilla_RRHH.Models.Enums;

namespace Planilla_RRHH.Security;

public partial class SessionContext : ObservableObject
{
    [ObservableProperty]
    private User? _currentUser;
    
    [ObservableProperty]
    private Role? _currentRole;
    
    [ObservableProperty]
    private PermissionFlags _permissions;
    
    public bool IsAuthenticated => CurrentUser != null;
    public int? UserId => CurrentUser?.Id;
    public string? Username => CurrentUser?.Username;
    public string? UserEmail => CurrentUser?.Email;
    
    public void SetUser(User user, Role role)
    {
        CurrentUser = user;
        CurrentRole = role;
        Permissions = role.Permissions;
    }
    
    public void Clear()
    {
        CurrentUser = null;
        CurrentRole = null;
        Permissions = PermissionFlags.None;
    }
    
    public bool HasPermission(PermissionFlags permission)
        => Permissions.HasPermission(permission);
    
    public bool CanCreateEmployee => HasPermission(PermissionFlags.EmployeeCreate);
    public bool CanReadEmployee => HasPermission(PermissionFlags.EmployeeRead);
    public bool CanUpdateEmployee => HasPermission(PermissionFlags.EmployeeUpdate);
    public bool CanDeleteEmployee => HasPermission(PermissionFlags.EmployeeDelete);
    public bool CanViewSalary => HasPermission(PermissionFlags.EmployeeViewSalary);
    public bool CanManageUsers => HasPermission(PermissionFlags.UserManageRoles);
    public bool CanManageSettings => HasPermission(PermissionFlags.SettingsUpdate);
    public bool CanCreateBackup => HasPermission(PermissionFlags.BackupCreate);
    public bool CanRestoreBackup => HasPermission(PermissionFlags.BackupRestore);
    public bool CanManageBackups => HasPermission(PermissionFlags.BackupCreate) || HasPermission(PermissionFlags.BackupRestore);
}