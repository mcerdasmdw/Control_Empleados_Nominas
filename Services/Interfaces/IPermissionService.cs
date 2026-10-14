using Planilla_RRHH.Models.Enums;
using Planilla_RRHH.Security;

namespace Planilla_RRHH.Services.Interfaces;

public interface IPermissionService
{
    PermissionFlags GetRolePermissions(int roleId);
    bool HasPermission(PermissionFlags permission);
    bool CanAccessEmployeeManagement();
    bool CanViewSalaries();
    bool CanManageUsers();
    bool CanManageSettings();
    bool CanManageBackups();
    IEnumerable<string> GetVisibleNavigationItems();
}