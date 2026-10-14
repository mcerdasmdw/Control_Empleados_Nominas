# R-A01 / T-A01 — Cuenta inicial

Utilidad puntual net8.0 con referencia a la aplicación existente; no se ejecuta al iniciar el sistema. No cambia el esquema ni inicializa bases reales.

Contrato: `--create <ruta existente>` crea únicamente `Administrador` activo y su relación con el rol 1 en una transacción, usando `Argon2Hasher` existente. Si el usuario ya existe (sin distinguir mayúsculas), conserva todos sus datos. `--verify <ruta existente>` autentica mediante `AuthService`, repositorios y sesión nueva; exige rol 1 Administrador y `AdminAll`. Ambos leen la contraseña de `PLANILLA_INITIAL_ADMIN_PASSWORD` y no la imprimen. No incluir la contraseña en argumentos, documentos o registros.

Entrada: archivo existente, contraseña obligatoria; creación exige las mismas condiciones de longitud/mayúscula/minúscula/dígito del servicio actual. Rol 1 y permisos deben coincidir con los existentes. Error de uso, rol, autenticación o SQL devuelve exit 1; éxito exit 0. Excepción SQL omite detalles; transacción fallida no deja inserción parcial. La verificación usa la fábrica actual que habilita WAL, por lo que puede generar archivos SQLite auxiliares.

```powershell
dotnet build verification/admin/AdminProvision.csproj --ignore-failed-sources -p:NuGetAudit=false -p:RestorePackagesPath='C:/Users/Maikol C/.nuget/packages'
dotnet verification/admin/bin/Debug/net8.0/AdminProvision.dll --self-test
```

El coordinador proporciona la variable de entorno y ruta de la base usada por App para `--create` y posteriormente `--verify`.

Verificación ejecutada: prueba sintética exit 0: primera inserción, repetición con contraseña distinta conserva ID/hash/salt/parámetros/rol y una sola cuenta; autenticación real y permisos correctos; contraseña de reemplazo rechazada. Base temporal inicializada únicamente para esta prueba y eliminada al terminar. No se ejecutó contra datos reales por el implementador.
