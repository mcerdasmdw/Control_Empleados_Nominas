# Estado actual
2026-10-13: R-A01/T-A01 cuenta Administrador creada en BD local AppData/Roaming/Planilla-RRHH/planilla.db por petición explícita del usuario. Verificada autenticación real con contraseña inicial solicitada y AdminAll, exit0. Herramienta puntual verification/admin compilada y selftest correcto; sin seed automático ni cambios schema. Revisión sin bloqueo de creación.
2026-10-13: R-UI05/T-UI05 corrige reporte de acciones de login desactivadas: asignación DataContext runtime faltante, callback de ventana principal y notificaciones CanLogin/CanExecute. Panel error añade fallback oculto. Build 0 errores y CS8603 conocida; revisión independiente sin hallazgos nuevos. Pruebas sintéticas en ejecución; inspección visual pendiente.
2026-10-13: R-UI04/T-UI04 login implementado por solicitud actual del usuario. Solo Views/LoginWindow.axaml: tarjeta, controles amplios, nombres accesibles, error temático y scroll. Ocho bindings conservados según comparación automática Frontend-Coder; XML válido y build final 0 errores, CS8603 preexistente. Revisión independiente en curso; inspección visual en ejecución pendiente.
2026-10-13: revisión documental de T-UI03 continuada por solicitud del usuario. Creado `docs/sdd/08-revision-t-ui03.md` con cuatro vistas propuestas, rutas, ocho criterios, matriz de tamaños propuesta y dependencias. Revisor independiente sin hallazgos comprobados pendientes; 13 enlaces SDD verificados, cero rotos. Sin código, compilación ni aplicación ejecutada. T-UI03 sigue pendiente de aprobación de construcción.
2026-10-13: R-UI02/T-UI02 documental: inspeccionados visualmente los ocho PNG de `Deseñosdelsistema/` (nombre real conservado). Guía `docs/sdd/07-diseno-visual.md` incorporada y enlazada a SDD; README corregido sobre existencia actual de aplicación. Enlaces verificados y revisión independiente documental sin hallazgos comprobados pendientes. Solo cambios Markdown; sin programación ni ejecución de la aplicación.
2026-10-12: R-UI01 / T-UI01 implementada por Frontend-Coder con ui-ux-pro-max y revisada por revisor en solo lectura. Mejoras en App.axaml, AppStyles y ocho vistas: colores por tema, contraste, espaciado, acciones textuales, login flexible con scroll, tablas con altura acotada y foco. XML 10/10 y compilación final correctos (0 errores; CS8603 en BoolConverters.cs:72 sin modificar). Sin inspección visual en ejecución.
2026-10-12: T-H01 / R-H01 completada por solicitud del usuario: UI UX Pro Max en `.agents/skills`, Context7 remoto y Chrome DevTools local configurados en `.codex/config.toml`. Búsqueda de la skill e inicialización de ambos MCP verificadas; Chrome DevTools anunció 30 herramientas. Navegación pendiente de disponibilidad de Chrome. Esta carpeta sí contiene código Avalonia, csproj y bin/obj: la descripción histórica siguiente no representa el contenido actual.
2026-10-12: estructura Codex y SDD preparada para importación. No hay Git ni aplicación en el espejo. sources/ está vacío. Prompts originales, ZIP OpenCode y requisitos funcionales no recuperados.

# Decisiones
2026-10-13: usuario autoriza cuenta inicial Administrador y contraseña proporcionada en chat; se inserta una vez sin sobrescribir existentes y se guarda hash Argon2/salt en BD.
2026-10-13: usuario autoriza verificar y corregir elementos del login reportados; se conectan contratos existentes, sin reglas de auth nuevas ni nuevas dependencias.
2026-10-13: solicitud «revisa y modifica la interfaz del login» autoriza construcción visual limitada R-UI04/T-UI04; conservar identidad, recursos y tamaño actuales evita depender de decisiones globales de T-UI03. Criterios técnicos derivados del alcance, no aprobación individual de criterios ni autorización de nuevas funciones.
2026-10-13: «continua con la revision de la tarea T-UI03» autoriza revisión documental; no aprueba construcción, criterios ni D-UI01 a D-UI04. Alcance sugerido: Login, ventana principal, Dashboard y Empleados; conservar identidad, menú y contratos como límites propuestos.
2026-10-13: usuario autoriza usar la carpeta de diseños como referencia y actualizar Markdown, pero excluye programación. Referencia: lateral marino, selección azul, tarjetas claras, acento menta, tablas oscuras y formularios etiquetados. Valores exactos, identidad, tema oscuro, menú y alcance de construcción pendientes D-UI01 a D-UI04; las imágenes no aprueban funcionalidad ni cálculos.
2026-10-12: usuario autorizó revisar y mejorar presentación usando Frontend-Coder y ui-ux-pro-max. Alcance visual trazado como R-UI01/T-UI01, conservando stack existente .NET 8/Avalonia 11.2.3, lógica y contratos. Revisor verificó correcciones de contraste de selección y scroll; no se aprobó funcionalidad nueva.
2026-10-12: usuario autorizó las tres herramientas de desarrollo para el proyecto. No implica aprobación de reglas de nómina o rediseño. MCP con alcance de proyecto; sin claves guardadas; estadísticas Chrome DevTools desactivadas.
Confirmado por solicitud del usuario: destino Codex; cuatro roles Backend-Coder, Frontend-Coder, Tester-datos y revisor; metodología SDD y este archivo con cuatro secciones.
Decisión de organización del paquete: agentes TOML y seis documentos numerados; no constituye aprobación del producto.
Pendientes: ver docs/sdd/03-clarificacion.md.

# Aprendizajes y errores a evitar
Design.DataContext no asigna DataContext runtime. Revisar composición/DI antes de dar por funcional una vista compilada; LoginViewModel requiere Action explícito y CanLogin debe notificar cambios. Panel rojo vacío puede ser superficie de error con binding sin fuente; no llamarlo botón confirmado sin evidencia visual.
T-UI03 distingue revisión documental terminada de construcción pendiente. Tamaños actuales 1200×800 y 420×600 no son aprobación de tamaños objetivo; estilos compartidos requieren comprobar también las cuatro vistas fuera del rediseño.
Los PNG contienen recortes de columnas, nombres fragmentados, legibilidad por comprobar y menús inconsistentes: adaptar, no copiar defectos. `DECISIONS.md` existe con decisiones históricas autodenominadas confirmadas, pero falta evidencia de aprobación; preservarlo y reconciliarlo, sin asumir que no hay antecedentes ni que R-UI02 los aprueba.
Para compilar se requirió DOTNET_CLI_HOME temporal y restore de dependencias ya declaradas. No modificar csproj para resolver referencias por suposición: el build resultó válido. Los datos Avalonia de la skill citan versión 12; evaluar compatibilidad con versión real 11.2.3. Un respaldo en tmp no persistió entre ejecuciones, por lo que no afirmar comparación automática completa de bindings. El contraste de acciones necesita considerarse también sobre filas seleccionadas.
Runtime Python y Node disponibles en la caché de Codex; alias python de Windows no es el runtime usado. npm auxiliar y servidor Chrome DevTools instalados dentro de `.tools`. Rutas MCP locales dependen de esta máquina y carpeta. No confundir inicialización MCP con navegación verificada.
No interpretar referencias opacas del historial como contenido recuperado.
No confundir configuración estructural verificada con agentes ejecutados o aplicación probada.
No asumir que Costa Rica, zona horaria del usuario, es la jurisdicción de la nómina.
No alterar archivos sincronizados del espejo.

# Próximos pasos
Revisar y aprobar propuesta `docs/sdd/08-revision-t-ui03.md`; concretar D-UI02 (valores visuales y ambos temas) y D-UI04 (alcance, fidelidad y matriz de ventanas) antes de asignar construcción. No marcar T-UI03 completada por terminar la revisión documental.
Antes de modificar interfaz, consultar `docs/sdd/07-diseno-visual.md`, concretar tarea y criterios aprobados y resolver las decisiones visuales aplicables. T-UI03 es propuesta sin autorización de construcción. Mantener los ocho PNG intactos.
Inspeccionar interfaz en ejecución: temas claro/oscuro, foco de teclado, login con error largo, acciones en selección y DatePicker en formularios pequeños. No certificar apariencia visual solo por XML/compilación. Evidencia de T-UI01 en docs/sdd/06-construccion.md.
Recargar MCP o reiniciar Codex; skill disponible en el próximo turno. Para navegación Chrome DevTools, localizar/instalar Chrome y probar sesión real. Ver evidencia de herramientas en docs/sdd/06-construccion.md.
Recuperar originales y requisitos aprobados; reconciliar sin pérdida; resolver decisiones bloqueantes; completar y aprobar el plan; construir por tareas y documentar evidencia.



2026-10-13 — Próximo paso para T-UI04: validar login en ejecución Light/Dark, Tab/Shift+Tab, Enter y error largo; T-UI03 general conserva aprobación pendiente. Revisor señaló riesgo preexistente de notificación CanLogin en LoginViewModel; comprobar con prueba runtime antes de atribuir un defecto al cambio visual.

2026-10-13: revisión independiente T-UI04 terminada, sin hallazgos comprobados del cambio visual. Implementación y validación estática finalizadas; tarea conserva comprobación visual pendiente.


2026-10-13: T-UI05 pruebas sintéticas finalizadas exit 0, 14/14 PASS; ver verification/login/README.md. Caché NuGet permanente C:/Users/Maikol C/.nuget/packages permitió restore sin nuevos paquetes (auditoría NU1900 por falta de red). Corrección verificada en ViewModel y compilación; ejecutar versión recompilada y verificar ambos botones, error y transición en pantalla como próximo paso.


2026-10-13 — Próximo paso: revisor detectó defecto preexistente UserRepository.UpdateAsync omite hash/salt/parámetros; antes de certificar cambio/reset de contraseña, corregir persistencia y probar. No afecta login inicial verificado. No registrar contraseña inicial en documentos.

2026-10-13: R-G01/T-G01 conexión GitHub autorizada. Git inicializado en main, origin https://github.com/mcerdasmdw/Control_Empleados_Nominas.git, fetch/ls-remote exitosos sin refs (remoto vacío). README local preservado. .gitignore excluye .env y .venv. Pendientes identidad de autor, commit Inicio del proyecto y push verificado.

