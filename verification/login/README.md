# Verificación R-UI05 / T-UI05

Fuente: solicitud del usuario 2026-10-13 de verificar elemento rojo y acciones de login desactivadas. Criterios: `docs/sdd/02-especificacion.md`, R-UI05. Expectativas derivadas de esos criterios y del contrato existente; no añade reglas de autenticación.

Procedimiento: proyecto consola net8.0 con referencia a la aplicación, `LoginViewModel` real y servicios sintéticos. No inicia Avalonia, SQLite, red ni autenticación real. Cada comprobación falla con excepción y código de salida distinto de cero si el resultado real difiere del esperado.

```powershell
$env:DOTNET_CLI_HOME = Join-Path $env:TEMP 'planilla-dotnet'
$env:NUGET_PACKAGES = 'C:/Users/Maikol C/.nuget/packages'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
dotnet run --project verification/login/LoginVerification.csproj --no-restore
```

Para primera restauración desde dependencias existentes: `dotnet restore verification/login/LoginVerification.csproj --ignore-failed-sources --verbosity minimal`. No se añaden paquetes.

| Casos | Procedimiento y esperado | Resultado real ejecutado |
|---|---|---|
| L01–L03 | Formulario vacío: login deshabilitado/error vacío; recuperación disponible, advierte sin llamar servicio | Coincide, 3 PASS |
| L04–L06 | Asignar usuario, contraseña y espacios: notifica CanLogin; habilita con ambos y deshabilita contraseña en blanco | Coincide, 3 PASS |
| L07–L09 | Activar/desactivar IsLoading y dejar servicio pendiente: CanLogin/CanExecute y sus notificaciones se actualizan | Coincide, 3 PASS |
| L10–L12 | Respuesta rechazada, exitosa y excepción: error, restauración de habilitación, credenciales transmitidas y callback solo tras éxito | Coincide, 3 PASS |
| L13–L14 | Recuperación con usuario y resultado éxito/fallo: llama servicio y muestra diálogo adecuado | Coincide, 2 PASS |

Evidencia ejecutada 2026-10-13: salida `R-UI05/T-UI05: 14/14 checks passed; synthetic services only, no UI or database startup.`, exit code 0. Compilación con advertencia CS8603 previa y NU1900 (auditoría NuGet sin conectividad). Primer intento de restore sin cache explícita falló NU1301; repetir con NUGET_PACKAGES permanente permitió restaurar y ejecutar.

Límite: esta prueba valida comportamiento y notificaciones del ViewModel; no certifica DataContext de una ventana real, visibilidad de AXAML, aspecto visual, teclado ni integración con autenticación real. La conexión runtime App/AXAML requiere revisión independiente y/o inspección visual del coordinador.
