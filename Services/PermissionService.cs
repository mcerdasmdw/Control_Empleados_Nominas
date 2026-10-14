using Planilla_RRHH.Models.Enums;
using Planilla_RRHH.Security;
using Planilla_RRHH.Services.Interfaces;

namespace Planilla_RRHH.Services;

public sealed class PermissionService : IPermissionService
{
    private readonly SessionContext _session;
    
    public PermissionService(SessionContext session)
    {
        _session = session;
    }
    
    public PermissionFlags GetRolePermissions(int roleId)
    {
        // This would typically query the database
        // For now, return based on known role IDs
        return roleId switch
        {
            1 => PermissionFlags.AdminAll,      // Administrador
            2 => PermissionFlags.RRHH,          // Recursos Humanos
            3 => PermissionFlags.Supervisor,    // Supervisor
            4 => PermissionFlags.Contabilidad,  // Contabilidad
            _ => PermissionFlags.None
        };
    }
    
    public bool HasPermission(PermissionFlags permission) => _session.HasPermission(permission);
    public bool CanAccessEmployeeManagement() => _session.CanReadEmployee;
    public bool CanViewSalaries() => _session.CanViewSalary;
    public bool CanManageUsers() => _session.CanManageUsers;
    public bool CanManageSettings() => _session.CanManageSettings;
    public bool CanManageBackups() => _session.CanCreateBackup;
    
    public IEnumerable<string> GetVisibleNavigationItems()
    {
        var items = new List<string> { "Dashboard", "Empleados" };
        
        if (_session.CanManageSettings)
            items.Add("Configuración");
        
        if (_session.CanManageUsers)
            items.Add("Usuarios");
        
        if (_session.CanManageBackups)
            items.Add("Backups");
        
        // Nómina futura - siempre visible como informativa
        items.Add("Nómina (próximamente)");
        
        return items;
    }
}