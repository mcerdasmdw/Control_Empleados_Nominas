# Especificación
## Referencias visuales — R-UI02 (2026-10-13)

| ID | Fuente | Estado de aprobación | Requisito y criterio de aceptación |
|---|---|---|---|
| R-UI02 | Solicitud del usuario en este chat, 2026-10-13; ocho PNG en `Deseñosdelsistema/` | Aprobado para lectura, incorporación como referencia y actualización documental; programación expresamente excluida | Inspeccionar las ocho imágenes, registrar estilos y patrones por pantalla, diferencias y decisiones pendientes; enlazar la guía con SDD y memoria. Solo modificar Markdown y preservar imágenes y código. |

Guía: [Diseño visual](07-diseno-visual.md). La autorización de referencia visual no aprueba módulos, cálculos, tasas, moneda, jurisdicción, marca nueva ni todas las acciones mostradas. Los criterios de implementación de la guía son propuestas para una futura tarea aprobada.

## Requisitos de migración confirmados
Fuente: solicitud actual del usuario y mensajes visibles de conversación 6ac9070a-ed74-83e8-8149-7a45e8e2cff2.

| ID | Requisito | Criterio de aceptación |
|---|---|---|
| R-M01 | Preparar destino Codex | Configuración de proyecto y cuatro agentes TOML |
| R-M02 | Definir cuándo usar los cuatro roles | Cada agente tiene descripción e instrucciones |
| R-M03 | AGENTS.md y memory.md | Instrucciones y cuatro secciones de memoria |
| R-M04 | Organizar seis fases SDD | Seis documentos enlazados |
| R-M05 | Preservar requisitos aprobados | No inventar requisitos; recuperar y reconciliar originales |
| R-M06 | Identificar decisiones pendientes | Registro explícito de bloqueos |
| R-M07 | Preparar importación sin repositorio | Paquete ZIP, instrucciones y verificación |

R-M05 no está completado: faltan las fuentes que permitirían comprobar conservación.

## Producto
## Herramientas autorizadas — 2026-10-12
### Presentación de la interfaz — R-UI01
Fuente: solicitud del usuario del 2026-10-12 a Frontend-Coder: «revisa la interfaz y utiliza ui-ux-pro-max para mejorar la presentacion». Estado: mejora de presentación autorizada por el usuario. Alcance operativo: estilos y vistas Avalonia existentes, jerarquía tipográfica, espaciado, contraste y consistencia de controles. Criterios de verificación derivados de ese alcance: recursos y AXAML válidos; conservar bindings, comandos, permisos y campos; documentar resultados de compilación y límites de inspección visual. No autoriza cambios de reglas salariales ni flujos funcionales.

| ID | Fuente | Estado de aprobación | Requisito y criterio de aceptación |
|---|---|---|---|
| R-H01 | Solicitud del usuario en este chat del 2026-10-12 | Aprobado por el usuario | Agregar UI UX Pro Max al proyecto y configurar Context7 y Chrome DevTools MCP; skill con recursos y búsqueda ejecutable, servidores habilitados en configuración del proyecto y comprobación de inicialización MCP. |

Esta autorización cubre herramientas de desarrollo; no aprueba requisitos funcionales, tecnologías del producto ni cambios de interfaz.

Nombre conocido: Planilla RRHH; contexto: MVP de nóminas.
No hay detalle funcional recuperable. No están confirmados módulos, usuarios, jurisdicción, cálculos ni criterios de aceptación.

## Registro para requisitos por recuperar
Cada entrada debe incluir: ID R-P*, texto original, archivo/sección fuente, estado (aprobado/propuesto/pendiente), aprobación verificable, criterio de aceptación, decisiones relacionadas y pruebas.


## Ajuste del login — R-UI04 (2026-10-13)
Fuente y aprobación: usuario en este chat, «revisa y modifica la interfaz del login». Autoriza construcción de presentación del login existente. Criterios derivados del alcance: mejorar jerarquía y espaciado; etiquetas persistentes y nombres accesibles; controles de al menos 44 de alto; error legible con ajuste de líneas y scroll; conservar bindings, comandos, marca, tamaños de ventana, paleta y temas existentes; AXAML válido y compilación verificada. No añade funciones de la maqueta. D-UI02/D-UI04 no bloquean esta tarea limitada porque reutiliza recursos y ventana actuales; T-UI03 global sigue pendiente. La validación visual en ejecución debe distinguirse de la estática.

## R-UI05 — Corrección de login (2026-10-13)
Fuente/aprobación: usuario reporta elemento rojo y acciones desactivadas y solicita verificar. Alcance: corregir conexión runtime del LoginViewModel, actualización de habilitación y visibilidad del error vacío. Criterios derivados: DataContext asignado con servicios existentes; login habilitado con credenciales y fuera de carga; recuperación disponible; error oculto inicialmente y visible tras fallo; callback de éxito abre ventana principal con su ViewModel. Sin alterar reglas de autenticación.

## R-A01 — Cuenta inicial (2026-10-13)
Fuente/aprobación: solicitud actual del usuario crear Administrador con contraseña inicial indicada en chat. Criterios: insertar cuenta activa con rol Administrador en base local usada por App; hash/salt Argon2 existentes, sin guardar texto plano en BD; operación atómica e idempotente sin sobrescribir cuentas existentes; verificar autenticación real y permisos. No crear cuenta de nuevo al arrancar aplicación ni restablecer contraseñas automáticamente.
