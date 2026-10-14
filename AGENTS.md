# Planilla RRHH — instrucciones para Codex
## Contexto y fuentes
Leer memory.md y docs/sdd/README.md al iniciar; consultar los documentos de la fase correspondiente.
sources/ es referencia de solo lectura: no editar, renombrar, mover ni eliminar sus archivos.
Este paquete no recupera los requisitos funcionales aprobados ni los prompts originales. No inventar aprobaciones ni sustituir documentos originales por estas plantillas.

## Método SDD
Seguir constitución → especificación → clarificación → plan técnico → tareas → construcción.
Registrar para cada requisito su ID, fuente, estado de aprobación y criterio de aceptación.
Distinguir requisitos confirmados, propuestas y decisiones pendientes. Una decisión bloquea solo el trabajo que dependa de ella.
Antes de construir, disponer de alcance y criterios aprobados, decisiones críticas resueltas y tarea trazable.
No escoger jurisdicción, fórmulas salariales, tasas, moneda, redondeos, tecnologías o despliegue sin evidencia de aprobación.
Registrar cambios y verificación en construcción; no declarar pruebas exitosas sin ejecutarlas.
Actualizar memory.md al cerrar trabajo significativo con estado, decisiones, aprendizajes y próximos pasos. El coordinador integra la memoria para evitar escrituras concurrentes.

## Subagentes
- Backend-Coder: usar para lógica del servidor, contratos de API, persistencia e integraciones de tareas aprobadas.
- Frontend-Coder: usar para interfaces, formularios y flujos conforme a contratos y criterios aprobados.
- Tester-datos: usar para pruebas, datos sintéticos, validación de cálculos y reconciliación contra resultados aprobados.
- revisor: usar para revisión independiente de cambios, riesgos y cumplimiento SDD; trabaja en solo lectura.
El coordinador asigna una tarea delimitada, requisitos, rutas que puede modificar y resultado esperado. Evitar que dos agentes editen el mismo archivo. Respetar los límites efectivos del entorno; cuatro roles no implican cuatro ejecuciones simultáneas.
Cada entrega incluye archivos cambiados, requisito/tarea, comprobaciones ejecutadas, resultados y bloqueos. El revisor devuelve hallazgos con ubicación, impacto y evidencia.

