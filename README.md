# Planilla RRHH — paquete para Codex
Fecha: 2026-10-12 (Costa Rica).

## Estado
Actualización 2026-10-13: esta carpeta contiene una aplicación .NET 8/Avalonia 11.2.3 y documentación SDD. La preparación de importación descrita abajo es histórica; no representa la ausencia actual de código. Los requisitos originales y sus aprobaciones siguen pendientes de reconciliación.

Las ocho imágenes de `Deseñosdelsistema/` se incorporaron como referencias para el diseño y los estilos. Consultar [la guía visual](docs/sdd/07-diseno-visual.md) antes de futuras tareas de interfaz. R-UI02/T-UI02 cubre solo revisión y documentación: no se programó ni se aprobaron módulos o reglas de nómina por las maquetas.

En la revisión histórica del paquete no se encontraron archivos en sources/ ni repositorio Git. La conversación recuperada contenía referencias opacas y no ofrecía adjuntos; no fue posible recuperar las definiciones originales de OpenCode ni documentos SDD previos.

## Importación
1. Extraer este paquete en una carpeta de trabajo independiente del espejo de ChatGPT.
2. Si se importa a un repositorio existente, revisar y combinar AGENTS.md y .codex/config.toml; no sobrescribir archivos existentes.
3. Abrir esa carpeta como proyecto local en Codex. La configuración de proyecto puede requerir que el proyecto sea de confianza según la instalación.
4. Leer AGENTS.md, memory.md y docs/sdd/README.md.
5. Recuperar el ZIP de OpenCode y documentos aprobados; incorporarlos como referencias inmutables en sources/ y reconciliar según docs/migracion.md.
6. Resolver los bloqueos de clarificación antes de implementar funcionalidades dependientes.
7. Verificar que la instalación reconoce los cuatro agentes. La validación de este paquete comprueba sintaxis y estructura, no ejecución real en otra instalación.

La configuración usa agentes TOML independientes en .codex/agents/. No fija modelo, stack, credenciales ni configuración global. memory.md se mantiene por instrucciones de AGENTS.md; no es una función automática de persistencia.

## Documentación oficial consultada
- https://learn.chatgpt.com/docs/agent-configuration/subagents
- https://learn.chatgpt.com/docs/config-file/config-reference
- https://learn.chatgpt.com/docs/agent-configuration/agents-md

