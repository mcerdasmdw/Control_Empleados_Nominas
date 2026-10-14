namespace Planilla_RRHH.Models.Enums;

[Flags]
public enum PermissionFlags
{
    None = 0,
    
    // Employee permissions
    EmployeeCreate = 1 << 0,
    EmployeeRead = 1 << 1,
    EmployeeUpdate = 1 << 2,
    EmployeeDelete = 1 << 3,
    EmployeeViewSalary = 1 << 4,
    
    // User management permissions
    UserCreate = 1 << 5,
    UserRead = 1 << 6,
    UserUpdate = 1 << 7,
    UserDelete = 1 << 8,
    UserManageRoles = 1 << 9,
    
    // Settings permissions
    SettingsRead = 1 << 10,
    SettingsUpdate = 1 << 11,
    
    // Backup permissions
    BackupCreate = 1 << 12,
    BackupRestore = 1 << 13,
    
    // Combined role permissions
    AdminAll = ~0,
    RRHH = EmployeeCreate | EmployeeRead | EmployeeUpdate | EmployeeDelete,
    Supervisor = EmployeeRead,
    Contabilidad = EmployeeRead | EmployeeViewSalary
}

public static class PermissionExtensions
{
    public static bool HasPermission(this PermissionFlags flags, PermissionFlags permission)
        => (flags & permission) == permission;
}