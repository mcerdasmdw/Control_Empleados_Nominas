# Checklist de tareas
- [x] T-UI02 (R-UI02): coordinador inspecciona los ocho PNG de `Deseñosdelsistema/` y documenta dirección visual, patrones por pantalla, hallazgos, límites y decisiones D-UI01 a D-UI04. Autorización: usuario, 2026-10-13, solo documentación. Rutas: `docs/sdd/*.md`, `README.md`, `DECISIONS.md`, `memory.md`. Aceptación: inventario 8/8, trazabilidad y enlaces, sin modificar código ni imágenes. Evidencia en `06-construccion.md` y guía `07-diseno-visual.md`.
- [ ] T-UI03 propuesta (R-UI02 como referencia, autorización de construcción pendiente): alcance y criterios preparados en [Revisión de T-UI03](08-revision-t-ui03.md) por solicitud de revisión del usuario, 2026-10-13. Resolver decisiones aplicables y aprobar criterios antes de asignar construcción al Frontend-Coder. La revisión documental no completa la implementación.
- [x] T-UI01 (R-UI01): Frontend-Coder revisa y mejora presentación en `Views/*.axaml`, `Resources/Styles/AppStyles.axaml` y `App.axaml`. Fuente/aprobación: solicitud actual del usuario 2026-10-12. Implementación y revisión estática entregadas; compilación final correcta. Conservación de bindings revisada manualmente, sin comparación automática completa. Inspección visual en ejecución pendiente; evidencia y límites en `06-construccion.md`. No depende de decisiones de nómina.
- [x] T-H01 (R-H01): instalar skill en `.agents/skills/ui-ux-pro-max`, adaptar rutas a Codex, configurar MCP en `.codex/config.toml` y verificar búsqueda e inicialización. Fuente y aprobación: solicitud del usuario 2026-10-12. Evidencia: `06-construccion.md`. Sesión real de navegador pendiente de disponibilidad de Chrome.
- [x] T-M01 Inspeccionar espejo y conversación. Evidencia: docs/migracion.md.
- [x] T-M02 Crear los cuatro roles Codex con criterios de uso.
- [x] T-M03 Crear instrucciones y memoria en paquete independiente.
- [x] T-M04 Organizar seis documentos SDD y decisiones.
- [ ] T-M05 Recuperar originales y reconciliar requisitos R-M05. Depende de D-01.
- [ ] T-M06 Importar y verificar carga real de agentes. Depende de D-07.
- [ ] T-P01 Completar especificación del MVP. Depende de D-02.
- [ ] T-P02 Aprobar plan técnico con reglas y ejemplos. Depende de D-03 a D-06.
- [ ] T-P03 Desglosar construcción en tareas aceptables. Depende de T-P02.
- [ ] T-P04 Implementar, probar y revisar aplicación. Depende de T-P03.

Cada futura tarea: ID, requisito, responsable, dependencias, rutas autorizadas, criterio de aceptación, verificación y evidencia. Una tarea no se completa solo porque exista un archivo.


- [ ] T-UI04 (R-UI04): ajuste visual del login autorizado por solicitud del usuario 2026-10-13. Propietario Frontend-Coder; ruta Views/LoginWindow.axaml. Criterios en R-UI04; sin dependencias de reglas salariales. Coordinador integra documentación y verificación; revisor en solo lectura.

Estado T-UI04: implementación, XML, comparación de bindings, compilación y revisión independiente terminadas. Validación visual en ejecución pendiente; casilla abierta hasta documentarla o aceptar expresamente su límite.

- [ ] T-UI05/R-UI05: corregir login por reporte del usuario 2026-10-13; rutas App.axaml.cs, ViewModels/LoginViewModel.cs y pruebas sintéticas. Criterios R-UI05. Documentar build, pruebas y límites visuales.

Rutas efectivas T-UI05: App.axaml.cs, ViewModels/LoginViewModel.cs, Views/LoginWindow.axaml (FallbackValue=False), Planilla-RRHH.csproj (excluir código del verificador del ejecutable principal), verification/login/Program.cs y LoginVerification.csproj.

Estado T-UI05: corrección, compilación y revisión independiente completadas; 14/14 pruebas sintéticas ejecutadas correctamente. Verificación visual de panel rojo y transición real de ventanas pendiente; no cerrar aceptación visual por prueba de ViewModel.

- [ ] T-A01/R-A01: crear cuenta inicial aprobada por usuario 2026-10-13. Backend-Coder propiedad verification/admin; coordinador documentación y ejecución BD local. Criterios R-A01; sin dependencias de nómina.

- [x] T-A01/R-A01 completada: herramienta puntual compilada, prueba sintética y revisión independiente; inserción real y autenticación con permisos verificadas exit0. Evidencia en construcción.
