# Plan técnico
## Incorporación documental R-UI02 (2026-10-13)

T-UI02 registra las ocho referencias de `Deseñosdelsistema/` en [Diseño visual](07-diseno-visual.md), enlaza especificación, clarificación y tareas, y actualiza memoria. No incluye implementación, generación de assets, instalación de dependencias ni pruebas de aplicación.

Antes de futuras tareas de interfaz, leer esta guía y resolver solo las decisiones de las que dependa su alcance. Definir criterios y rutas autorizadas; Frontend-Coder implementará tareas aprobadas, Tester-datos validará lo que corresponda y revisor revisará en solo lectura. Conservar los recursos y contratos Avalonia existentes; ningún nuevo módulo se autoriza por aparecer en una imagen.


## Mejora de presentación R-UI01 (2026-10-12)
Usar la tecnología existente Avalonia 11 / .NET 8. Frontend-Coder revisa vistas y estilos, consulta ui-ux-pro-max y mejora jerarquía, espaciado y contraste con recursos compartidos. Conservar contratos de bindings y lógica. Validar AXAML y compilar si el entorno lo permite. El coordinador integra evidencia y memoria; revisión independiente en solo lectura al terminar los cambios.
Estado histórico del paquete SDD: preparación documental; requisitos funcionales originales pendientes de reconciliación. Para T-UI01 se constató código existente .NET 8 / Avalonia 11.2.3; se conserva ese stack sin decidir migraciones.

## Secuencia
1. Recuperar fuentes D-01; inventariar y comparar contenidos.
2. Transcribir requisitos conservando texto, IDs y aprobaciones.
3. Resolver D-02 a D-06 solo cuando los originales no establezcan respuesta.
4. Definir componentes, contratos, persistencia, acceso e integraciones con trazabilidad.
5. Definir pruebas y resultados esperados antes de asignar implementación.
6. Dividir tareas con propietario, rutas, dependencias y aceptación.
7. Implementar, probar y revisar; registrar evidencia.

## Coordinación
Backend-Coder entrega lógica y contratos; Frontend-Coder consume contratos acordados; Tester-datos verifica resultados aprobados; revisor inspecciona cambios de forma independiente.
La concurrencia requiere tareas independientes y rutas distintas.

## Secciones por completar
Arquitectura; modelo de datos; contratos; seguridad y permisos; cálculos y precisión; estrategia de pruebas; despliegue; migración de datos; recuperación ante fallos.
No decidir nuevas dependencias o arquitectura del producto sin evidencia de aprobación. Restaurar las dependencias existentes para verificar T-UI01 forma parte de su validación.


## R-UI04 / T-UI04 — Login
Frontend-Coder modifica únicamente Views/LoginWindow.axaml; reutiliza recursos semánticos existentes, conserva inventario de bindings y comandos y no modifica autenticación. Coordinador valida XML y build; revisor inspecciona en solo lectura. Evidencia y limitaciones en construcción.

R-UI05/T-UI05: coordinador conecta DataContext/callback en App.axaml.cs; Backend-Coder corrige notificaciones en LoginViewModel; Tester-datos verifica estados con servicios sintéticos; revisor revisa solo lectura.

R-A01/T-A01: herramienta puntual verification/admin con servicios/repositorios existentes y transacción SQL; coordinador ejecuta contra %APPDATA%/Planilla-RRHH/planilla.db; Backend-Coder implementa, revisor solo lectura. Verificación sintética y autenticación posterior; no cambiar schema ni startup.
