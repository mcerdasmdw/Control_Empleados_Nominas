# Revisión de T-UI03

Fecha: 2026-10-13. Fuente: usuario en este chat, «continua con la revision de la tarea T-UI03». Autoriza esta revisión documental; no aporta aprobación de construcción ni respuestas a D-UI01 a D-UI04. Requisito de referencia: R-UI02; antecedente: R-UI01. Estado: propuesta de adaptación, pendiente de aprobación.

## Resultado y alcance propuesto

T-UI03 no está lista para construcción: falta aprobar alcance, criterios y decisiones visuales aplicables. Se propone una primera adaptación de Login, ventana principal (encabezado y lateral), Dashboard y Empleados, conservando sus datos y acciones existentes. Comparar con los cuatro PNG correspondientes de [Diseño visual](07-diseno-visual.md); adaptar jerarquía, superficies, selección, tarjetas y tablas sin exigir copia literal.

Rutas propuestas para una futura tarea Frontend-Coder: `App.axaml`, `Resources/Styles/AppStyles.axaml`, `Views/LoginWindow.axaml`, `Views/MainWindow.axaml`, `Views/DashboardView.axaml` y `Views/EmployeeListView.axaml`. La revisión actual solo modifica documentación. Los estilos compartidos requieren comprobar también EmployeeForm, Usuarios, Configuración y Backups, aunque no se rediseñen.

No incorporar pantallas, columnas, indicadores ni comandos por aparecer en los PNG. Planilla, Vacaciones, Aguinaldos, Liquidaciones y Reportes requieren tareas y requisitos propios. Conservar marca, menú, stack, bindings, permisos y reglas existentes como límite de esta propuesta, sin certificar su aprobación funcional histórica.

## Decisiones por dependencia

| Decisión | Propuesta para revisar | Trabajo dependiente |
|---|---|---|
| D-UI01 | Conservar nombre y recursos actuales; no adoptar marca o logo de las maquetas | Solo un cambio de identidad requiere resolver derechos y marca |
| D-UI02 | Preparar valores concretos de recursos semánticos y ejemplos Light/Dark antes de construir | Estilos finales; aún no hay paleta, fuente ni medidas aprobadas |
| D-UI03 | Conservar menú y comandos existentes | Nuevos módulos o reagrupación requieren resolución propia |
| D-UI04 | Priorizar las cuatro vistas indicadas y fidelidad de patrones, con diferencias justificadas | Alcance de construcción y tamaños objetivo pendientes de aprobación |

La inspección estática confirma MainWindow de 1200×800 y LoginWindow de 420×600; estos valores son configuración actual, no tamaños objetivo aprobados. Proponer para validación esos tamaños y una ventana principal reducida de 960×640, con escala 100% y 150%; confirmar esta matriz antes de construir.

## Criterios de aceptación propuestos

| ID | Criterio verificable | Evidencia requerida |
|---|---|---|
| CA-UI03-01 | Las cuatro vistas siguen los patrones acordados; cada diferencia con el PNG se justifica por contenido existente, legibilidad o tamaño | Capturas sintéticas en ambos temas y comparación por vista |
| CA-UI03-02 | No se pierden ni agregan bindings, comandos, campos o condiciones de permisos | Inventario previo y comparación después; comprobación de navegación y visibilidad con perfiles sintéticos disponibles |
| CA-UI03-03 | Texto, acciones y foco se distinguen en superficies y filas seleccionadas | Medición de contraste propuesta: 4,5:1 para texto normal, 3:1 para texto grande y señales visuales necesarias; comprobación real de foco y estados |
| CA-UI03-04 | Teclado alcanza controles y acciones en orden usable; etiquetas persistentes y nombres accesibles en controles de icono | Recorrido con Tab/Shift+Tab y activación por teclado; inspección de nombres accesibles |
| CA-UI03-05 | En la matriz aprobada no se recortan acciones ni importes; el desplazamiento permite consultar tablas y formularios completos | Capturas con nombres largos, error de login largo y tablas extensas; desplazamiento y DatePicker comprobados |
| CA-UI03-06 | Estados vacíos, error y carga existentes siguen legibles; estados inexistentes se registran como no aplicables | Casos sintéticos identificados y resultados por vista, sin implementar flujos nuevos |
| CA-UI03-07 | Los recursos compartidos no introducen defectos visuales en las otras cuatro vistas existentes | Inspección de EmployeeForm, Usuarios, Configuración y Backups en Light/Dark |
| CA-UI03-08 | AXAML válido y compilación correcta; revisión independiente sin hallazgos pendientes dentro del alcance | Comandos, resultados y revisión registrados en construcción; advertencias existentes identificadas |

Estos criterios no están aprobados ni ejecutados. La guía local ui-ux-pro-max aporta prioridades generales de contraste, teclado, etiquetas y adaptación; no se generó un sistema visual alternativo ni se eligieron valores de estilo.

## Secuencia propuesta

1. Resolver D-UI02 y D-UI04 para este alcance; confirmar límites de D-UI01/D-UI03 y aprobar criterios.
2. Registrar una tarea de construcción trazable con rutas, propietario y decisiones aprobadas; capturar inventario de contratos antes de editar.
3. Frontend-Coder adapta las vistas y recursos; Tester-datos verifica casos sintéticos aprobados; revisor inspecciona en solo lectura. Coordinador integra evidencias y memoria.
4. Registrar compilación e inspección visual de aplicación; cerrar solo con criterios satisfechos o límites expresamente aceptados.

## Verificación de esta revisión

Leídos memoria, constitución, especificación, clarificación, plan, tareas, construcción, guía visual y SKILL.md de ui-ux-pro-max. Listadas las vistas existentes e inspeccionados estáticamente tamaños y comandos de las cuatro vistas propuestas. No se ejecutó aplicación, compilación ni pruebas funcionales; no se repitió la inspección de PNG de T-UI02. Revisión independiente y comprobación de enlaces se registran en `06-construccion.md`.
