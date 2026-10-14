# Decisiones de Diseño - Planilla-RRHH MVP

## Referencias visuales incorporadas — 2026-10-13

El usuario autorizó leer `Deseñosdelsistema/` y tomar sus ocho PNG como referencia de diseño y estilos, actualizando documentación sin programar (R-UI02/T-UI02). Guía e inventario: [docs/sdd/07-diseno-visual.md](docs/sdd/07-diseno-visual.md). Identidad, tokens exactos, tema oscuro, menú y alcance de implementación permanecen pendientes en D-UI01 a D-UI04. Los valores salariales y notas legales de las imágenes no son reglas aprobadas.

Las entradas históricas siguientes se preservan con su título original; no incluyen evidencia de responsable, fecha y referencia de aprobación. Deben reconciliarse con los originales conforme a `docs/sdd/03-clarificacion.md`; esta actualización no confirma ni revoca sus decisiones funcionales.

## Decisiones Confirmadas

### 1. Target Framework
- **.NET 8 LTS + Avalonia 11.x** (migración desde .NET 10 actual)

### 2. Password Hashing
- **Argon2id** via `Konscious.Security.Cryptography.Argon2`
- Parámetros: memory=64MB, iterations=3, parallelism=4, salt=16 bytes, hash=32 bytes

### 3. Primer Administrador
- **Script SQL de migración inicial** - inserta admin con hash pregenerado
- Requiere ejecución manual del script o herramienta de migración

### 4. Modelo Usuario-Rol
- **Un solo rol por usuario** - tabla `UserRoles` con Unique Constraint en `UserId`

### 5. Campos Obligatorios Empleado
- **Solo**: Nombre completo, Cédula
- Resto opcionales: teléfono, email, dirección, puesto, salario, tipo salario, fecha contratación, estado, datos bancarios, observaciones

### 6. Tipos de Salario (Catálogo)
1. Mensual
2. Quincenal
3. Semanal
4. Diario
5. Por hora

### 7. Datos Bancarios Empleado
- Banco (texto)
- Tipo de cuenta (Ahorri/Corriente)
- Número de cuenta
- IBAN

### 8. Validación Cédula Costarricense
- **Completa**: formato + dígito verificador (algoritmo oficial físico/jurídica)

### 9. Logotipo Restaurante
- **PNG/JPG** en disco (`AppData/Logos/`) + ruta relativa en BD

### 10. Restauración Backup
- **BD completa** - usuarios, roles, configuración, empleados todo se restaura

### 11. Recuperación Contraseña
- **Solo generación de token** (sin email real)
- Token mostrado en consola/logs para testing
- Email real = fase posterior

### 12. Matriz de Permisos

| Acción | Admin | RRHH | Supervisor | Contabilidad |
|--------|-------|------|------------|--------------|
| Crear empleado | ✓ | ✓ | ✗ | ✗ |
| Editar empleado | ✓ | ✓ | ✗ | ✗ |
| Eliminar/Desactivar empleado | ✓ | ✓ | ✗ | ✗ |
| Ver empleados (lista) | ✓ | ✓ | ✓ | ✓ |
| Ver salarios | ✓ | ✗ | ✗ | ✓ |
| Gestionar usuarios/roles | ✓ | ✗ | ✗ | ✗ |
| Configuración restaurante | ✓ | ✗ | ✗ | ✗ |
| Crear backup | ✓ | ✗ | ✗ | ✗ |
| Restaurar backup | ✓ | ✗ | ✗ | ✗ |

---

## Pendientes de Definición (No Bloqueantes para Inicio)

1. **Longitudes máximas exactas** para campos de texto (nombres, direcciones, etc.)
2. **Estados de empleado** exactos (Activo, Inactivo, Suspendido, etc.)
3. **Validación IBAN** costarricense (formato específico)
4. **Ubicación exacta AppData** multiplataforma para logs, DB, logos, backups
5. **Política de expiración de sesión** (timeout, remember me)
6. **Auditoría/logging** de acciones sensibles (opcional MVP)

---

## Esquema BD Propuesto (SQLite)

```sql
-- Users
CREATE TABLE Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE COLLATE NOCASE,
    PasswordHash BLOB NOT NULL,      -- Argon2id hash (32 bytes)
    PasswordSalt BLOB NOT NULL,      -- 16 bytes
    Argon2Params TEXT NOT NULL,      -- JSON con parámetros usados
    Email TEXT,
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now'))
);

-- Roles
CREATE TABLE Roles (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,
    Description TEXT
);

-- UserRoles (1 rol por usuario)
CREATE TABLE UserRoles (
    UserId INTEGER NOT NULL,
    RoleId INTEGER NOT NULL,
    PRIMARY KEY (UserId),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE RESTRICT
);

-- Employees
CREATE TABLE Employees (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    FullName TEXT NOT NULL,
    Cedula TEXT NOT NULL UNIQUE,
    Phone TEXT,
    Email TEXT,
    Address TEXT,
    Position TEXT,
    BaseSalary NUMERIC(18,2),        -- decimal para precisión monetaria
    SalaryType TEXT CHECK (SalaryType IN ('Mensual','Quincenal','Semanal','Diario','Por hora')),
    HireDate TEXT,                   -- ISO8601 date
    Status TEXT NOT NULL DEFAULT 'Activo' CHECK (Status IN ('Activo','Inactivo')),
    BankName TEXT,
    BankAccountType TEXT CHECK (BankAccountType IN ('Ahorros','Corriente')),
    BankAccountNumber TEXT,
    BankIBAN TEXT,
    Notes TEXT,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now'))
);

-- RestaurantSettings (singleton, Id=1)
CREATE TABLE RestaurantSettings (
    Id INTEGER PRIMARY KEY CHECK (Id = 1),
    CommercialName TEXT,
    LegalId TEXT,
    Phone TEXT,
    Email TEXT,
    Address TEXT,
    Currency TEXT NOT NULL DEFAULT 'CRC',
    LogoPath TEXT,                   -- ruta relativa a AppData/Logos/
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now'))
);

-- PasswordResetTokens (para recuperación sin email)
CREATE TABLE PasswordResetTokens (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    TokenHash BLOB NOT NULL,         -- hash del token (Argon2id o SHA256)
    ExpiresAt TEXT NOT NULL,         -- ISO8601
    UsedAt TEXT,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- SchemaVersion (para migraciones)
CREATE TABLE SchemaVersion (
    Version INTEGER PRIMARY KEY,
    AppliedAt TEXT NOT NULL DEFAULT (datetime('now')),
    Description TEXT
);
```

---

## Estructura de Proyecto Propuesta

```
Planilla-RRHH.sln
├── Planilla-RRHH.csproj                 # Proyecto principal (Avalonia Desktop)
├── Planilla-RRHH.Tests.csproj           # Proyecto de pruebas (xUnit)
│
├── App.axaml / App.axaml.cs             # App entry, DI setup, theme
├── Program.cs                           # Main entry point
│
├── Models/
│   ├── User.cs
│   ├── Role.cs
│   ├── Employee.cs
│   ├── RestaurantSettings.cs
│   ├── Permission.cs (flags enum)
│   └── Enums/
│       ├── SalaryType.cs
│       ├── EmployeeStatus.cs
│       ├── BankAccountType.cs
│       └── PermissionFlags.cs
│
├── ViewModels/
│   ├── ViewModelBase.cs
│   ├── MainViewModel.cs                 # Shell + navegación
│   ├── LoginViewModel.cs
│   ├── DashboardViewModel.cs
│   ├── Employees/
│   │   ├── EmployeeListViewModel.cs
│   │   ├── EmployeeFormViewModel.cs
│   │   └── EmployeeSearchViewModel.cs
│   ├── SettingsViewModel.cs
│   ├── UsersViewModel.cs
│   ├── BackupViewModel.cs
│   └── PasswordResetViewModel.cs
│
├── Views/
│   ├── MainWindow.axaml / .cs           # Shell con navegación lateral
│   ├── LoginWindow.axaml / .cs
│   ├── DashboardView.axaml
│   ├── Employees/
│   │   ├── EmployeeListView.axaml
│   │   ├── EmployeeFormView.axaml
│   │   └── EmployeeSearchView.axaml (UserControl)
│   ├── SettingsView.axaml
│   ├── UsersView.axaml
│   ├── BackupView.axaml
│   └── PasswordResetView.axaml
│
├── Services/
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── IEmployeeService.cs
│   │   ├── ISettingsService.cs
│   │   ├── IUserService.cs
│   │   ├── IBackupService.cs
│   │   ├── IPermissionService.cs
│   │   └── IDialogService.cs
│   ├── AuthService.cs
│   ├── EmployeeService.cs
│   ├── SettingsService.cs
│   ├── UserService.cs
│   ├── BackupService.cs
│   ├── PermissionService.cs
│   └── DialogService.cs
│
├── Data/
│   ├── Database.cs                      # Conexión, init, migraciones
│   ├── Repositories/
│   │   ├── UserRepository.cs
│   │   ├── RoleRepository.cs
│   │   ├── EmployeeRepository.cs
│   │   ├── SettingsRepository.cs
│   │   └── PasswordResetRepository.cs
│   ├── Migrations/
│   │   ├── 001_InitialSchema.sql
│   │   ├── 002_SeedAdmin.sql            -- Hash pregenerado
│   │   └── MigrationRunner.cs
│   └── SqliteConnectionFactory.cs
│
├── Security/
│   ├── Argon2Hasher.cs                  # Wrapper Konscious.Argon2
│   ├── PasswordValidator.cs
│   ├── CedulaValidator.cs               # Algoritmo oficial CR
│   ├── TokenGenerator.cs                # Secure random tokens
│   └── SessionContext.cs                # Usuario actual + permisos
│
├── Resources/
│   ├── Styles/
│   │   ├── AppStyles.axaml
│   │   ├── Colors.axaml
│   │   └── Controls.axaml
│   ├── Icons/                           # FontAwesome + custom
│   └── Strings/
│       └── Resources.resx               # Localización ES
│
├── Converters/
│   ├── BoolToVisibilityConverter.cs
│   ├── EnumToStringConverter.cs
│   ├── CurrencyConverter.cs             # CRC formatting
│   └── InverseBoolConverter.cs
│
├── Extensions/
│   ├── ServiceCollectionExtensions.cs   # DI registration
│   └── StringExtensions.cs
│
└── Tests/
    ├── Unit/
    │   ├── Security/
    │   │   ├── Argon2HasherTests.cs
    │   │   ├── CedulaValidatorTests.cs
    │   │   └── TokenGeneratorTests.cs
    │   ├── Services/
    │   │   ├── AuthServiceTests.cs
    │   │   ├── EmployeeServiceTests.cs
    │   │   ├── SettingsServiceTests.cs
    │   │   ├── UserServiceTests.cs
    │   │   ├── BackupServiceTests.cs
    │   │   └── PermissionServiceTests.cs
    │   ├── Repositories/
    │   │   ├── EmployeeRepositoryTests.cs
    │   │   └── UserRepositoryTests.cs
    │   └── ViewModels/
    │       ├── LoginViewModelTests.cs
    │       ├── EmployeeFormViewModelTests.cs
    │       └── DashboardViewModelTests.cs
    │
    ├── Integration/
    │   ├── DatabaseIntegrationTests.cs
    │   ├── MigrationTests.cs
    │   └── BackupRestoreTests.cs
    │
    └── TestFixtures/
        ├── TestDatabaseFixture.cs
        └── TestDataBuilder.cs
```

---

## Navegación Lateral (Según Permisos)

```
┌─────────────────────┐
│  Planilla-RRHH      │
├─────────────────────┤
│  📊 Dashboard       │  ← Todos
├─────────────────────┤
│  👥 Empleados       │  ← Admin, RRHH, Supervisor, Contabilidad
├─────────────────────┤
│  ⚙️ Configuración   │  ← Solo Admin
├─────────────────────┤
│  👤 Usuarios        │  ← Solo Admin
├─────────────────────┤
│  💾 Backups         │  ← Solo Admin
├─────────────────────┤
│  📋 Nómina (próxim.)│  ← Visible solo informativa
└─────────────────────┘
```

---

## Comandos de Desarrollo

```bash
# Restaurar dependencias
dotnet restore

# Compilar
dotnet build

# Ejecutar (Debug)
dotnet run

# Ejecutar tests
dotnet test

# Publicar Windows (self-contained)
dotnet publish -c Release -r win-x64 --self-contained true

# Publicar Linux (self-contained)
dotnet publish -c Release -r linux-x64 --self-contained true

# Generar hash Argon2id para admin inicial (herramienta auxiliar)
dotnet run --project Tools/GenerateAdminHash
```

---

## Ubicaciones de Datos (Multiplataforma)

| Dato | Windows | Linux/macOS |
|------|---------|-------------|
| Base de datos | `%APPDATA%\Planilla-RRHH\planilla.db` | `~/.local/share/Planilla-RRHH/planilla.db` |
| Logos | `%APPDATA%\Planilla-RRHH\Logos\` | `~/.local/share/Planilla-RRHH/Logos/` |
| Backups | `%USERPROFILE%\Documents\Planilla-RRHH\Backups\` | `~/Documents/Planilla-RRHH/Backups/` |
| Logs | `%LOCALAPPDATA%\Planilla-RRHH\Logs\` | `~/.local/share/Planilla-RRHH/Logs/` |
| Config app | `%APPDATA%\Planilla-RRHH\appsettings.json` | `~/.config/Planilla-RRHH/appsettings.json` |

---

## Próximos Pasos (Stage 2)

1. **Migrar proyecto a .NET 8 + Avalonia 11**
2. **Crear solución con proyectos principal + tests**
3. **Configurar DI, navegación, tema Fluent**
4. **Implementar `Database.cs` + migraciones + seed admin**
5. **Verificar arranque en Windows y Linux**

¿Procedo con Stage 2 (Esqueleto Funcional)?
