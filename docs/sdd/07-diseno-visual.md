# Referencias de diseño y estilos

Fecha: 2026-10-13. Requisito R-UI02; tarea documental T-UI02.
Fuente de autorización: solicitud del usuario en este chat para leer la carpeta de diseños, tomarla en cuenta para la aplicación y actualizar Markdown, sin programar todavía.
La carpeta real se llama `Deseñosdelsistema/`; se conserva su nombre y sus ocho PNG sin modificación. Este documento se consulta antes de futuras tareas de interfaz junto con la especificación y los contratos aprobados.

## Alcance y aprobación

Confirmado: utilizar estas imágenes como referencia de diseño y estilos y preparar documentación. No se autoriza implementar en esta tarea. La apariencia observada orienta el trabajo futuro; los valores exactos de color, fuente, medidas e identidad no están definidos por archivos de diseño editables.
Los textos, cifras, personas, fechas, tasas, moneda, reglas legales, módulos y acciones que aparecen en los PNG son contenido de maqueta, no requisitos funcionales aprobados ni resultados esperados para pruebas de nómina. Consultar D-02 a D-06 y D-UI01 a D-UI04 antes de construir trabajo dependiente.

## Inventario inspeccionado visualmente

| Fuente | Diseño observado | Límite funcional |
|---|---|---|
| `../../Deseñosdelsistema/Login.png` | Tarjeta blanca, marca centrada, icono menta sobre azul oscuro, etiquetas visibles, campos con iconos, botón principal oscuro y acciones secundarias | Recordarme, mostrar contraseña, recuperación y solicitar acceso necesitan alcance y contratos aprobados |
| `../../Deseñosdelsistema/Dashboard.png` | Barra lateral oscura, encabezado claro, cuatro indicadores, tabla de próximos pagos, panel de solicitudes y alertas inferiores | Indicadores, aprobaciones y alertas necesitan definiciones y permisos |
| `../../Deseñosdelsistema/Empleados.png` | Título y descripción, acción superior derecha, búsqueda amplia, tabla con encabezado oscuro, estados y paginación | Columnas, acciones y salarios deben respetar contratos y permisos existentes; no añadir Jornada por la imagen |
| `../../Deseñosdelsistema/Planilla.png` | Selectores de período, acción de cálculo, tabla con filas alternas, resumen de totales y exportación | No aprobar CCSS 10.67%, deducciones, períodos, moneda ni exportación bancaria por la maqueta |
| `../../Deseñosdelsistema/Vacaciones.png` | Indicadores, búsqueda y tabla a la izquierda; formulario y resumen de proyección a la derecha | Saldos, vencimiento, promedio salarial, solicitudes y registro requieren requisitos aprobados |
| `../../Deseñosdelsistema/Aguinaldos.png` | Período visible, acciones superiores, tres tarjetas, progreso y tabla de desglose | Acumulación, proyección, cierre y exportación pendientes de definición funcional |
| `../../Deseñosdelsistema/Liquidaciones.png` | Formulario izquierdo y panel oscuro de desglose/total a la derecha; aviso informativo | Motivos, prestaciones, topes y notas legales no son autoridad normativa ni fórmulas aprobadas |
| `../../Deseñosdelsistema/Reportes.png` | Filtros laterales, tarjetas de categorías y vista previa con gráfico, tabla y paginación | Catálogo, gráficos, PDF, Excel, correo e impresión requieren alcance aprobado |

## Dirección visual de referencia

- Estructura de escritorio: barra lateral azul marino con marca arriba y configuración abajo; encabezado horizontal claro con título y usuario; contenido principal sobre fondo gris muy claro.
- Navegación: fila activa azul intenso con texto e icono blancos; filas inactivas con iconos lineales y texto atenuado. La selección debe reconocerse también por su forma o indicador y conservar foco visible.
- Superficies: tarjetas blancas o grises, bordes finos, radios moderados y sombras discretas. Reservar paneles oscuros para resúmenes destacados.
- Color: azul marino para estructura y botones principales, azul vivo para selección/enlaces, menta/verde para marca y estados favorables, rojo sobre superficie clara para alertas, gris para estados neutros. Son familias observadas, no valores hexadecimales aprobados.
- Tipografía: sans serif, títulos y cifras jerarquizados; etiquetas y descripciones legibles. Se observa apariencia monoespaciada en algunos importes; su adopción y la familia exacta quedan pendientes. Alinear columnas numéricas y conservar el formato monetario que se apruebe.
- Componentes: títulos con descripción breve; acción principal junto al título o formulario; búsqueda y filtros antes de tablas; encabezados oscuros, filas alternas, estados con texto, pie de conteo/paginación; formularios con etiquetas persistentes y acciones agrupadas.
- Iconografía: trazo consistente y tamaño uniforme, acompañada de texto en acciones relevantes. No deducir biblioteca de iconos ni licencia del logotipo a partir de las imágenes.

## Hallazgos que deben corregirse al adaptar las referencias

La inspección es de PNG, no de una interfaz ejecutable; no demuestra comportamiento, permisos ni accesibilidad real.

1. Planilla, Aguinaldos, Vacaciones y Reportes muestran columnas, cifras o controles recortados. Diseñar tablas con desplazamiento dentro de su región, tamaños de columna y adaptación del formulario; no reproducir recortes ni desbordamiento de la ventana.
2. Dashboard presenta nombres fragmentados y acciones ajustadas al borde en solicitudes. Permitir texto largo y espacio para acciones sin ocultarlas.
3. En varios paneles oscuros y montos claros la legibilidad necesita comprobación. No copiar colores sin calcular contraste y comprobar los estados seleccionados, deshabilitados y con foco.
4. Reportes mantiene Dashboard seleccionado y el menú agrupa Vacaciones/Aguinaldos en unas imágenes y los separa en otras. La futura navegación requiere una estructura acordada y selección coherente con la pantalla actual.
5. Login incluye funciones que pueden no existir en el flujo actual. Reutilizar su jerarquía visual sin introducir comandos sin contrato aprobado.
6. Los PNG muestran una interfaz clara con lateral oscuro, no un tema oscuro completo. Conservar los temas existentes; la adaptación exacta de las referencias al tema oscuro queda pendiente.

## Relación con la aplicación existente

Inspección estática de `Resources/Styles/AppStyles.axaml` y `Views/MainWindow.axaml`: ya existen recursos semánticos Light/Dark, superficies claras, acentos azules, radios y estructura de encabezado/lateral. El lateral actual usa SurfaceBrush; la referencia propone lateral marino. R-UI01 queda como antecedente verificado estáticamente y por compilación, sin validación visual en ejecución.

En una futura tarea aprobada, reutilizar recursos semánticos y estilos compartidos, conservar .NET 8/Avalonia 11.2.3, bindings, comandos, permisos y lógica; adaptar encabezado, lateral, tarjetas y tablas conforme a este documento. No migrar a tecnologías web por la apariencia de las maquetas. Login, Dashboard y Empleados tienen vistas existentes; las demás imágenes no bastan para autorizar nuevos módulos. Configuración, Usuarios, Backups y formulario de empleado carecen de PNG específico: aplicar coherencia visual como propuesta, sin inventar pantallas de referencia.

## Criterios propuestos para una futura implementación

Estos criterios son preparación técnica y deben incorporarse a una tarea de construcción aprobada; aún no se han ejecutado.

- Comparar Login, encabezado/lateral, Dashboard y Empleados con sus PNG, documentando adaptaciones y diferencias justificadas.
- Validar contraste, etiquetas accesibles, teclado y foco; los estados y gráficos deben poder entenderse sin depender solo del color. Guía general consultada: prioridades de accesibilidad, formularios y consistencia de `ui-ux-pro-max/SKILL.md`; no se generó una paleta alternativa con su buscador.
- Verificar ventana reducida, textos largos, tablas extensas, estados vacíos/error/carga y ambos temas sin recortar controles o importes.
- Comprobar que no cambia la visibilidad de salarios, permisos, navegación autorizada, bindings ni comandos.
- Validar AXAML, compilar y revisar visualmente en ejecución antes de afirmar fidelidad o accesibilidad. Las imágenes no sustituyen pruebas funcionales.

## Decisiones pendientes

| ID | Decisión | Trabajo que depende de ella |
|---|---|---|
| D-UI01 | Identidad: Planilla-RRHH frente a Maestro Planillas / Maestro Planillas CR / Planilla Digital; logotipo y derechos de uso | Cambio de nombre y recursos de marca |
| D-UI02 | Valores exactos de paleta, fuente, medidas y adaptación al tema oscuro | Especificación final de estilos |
| D-UI03 | Menú final: agrupación de Vacaciones/Aguinaldos y ubicación de Reportes; alcance de módulos y acciones | Nuevas pantallas y cambios de navegación |
| D-UI04 | Prioridad y grado de fidelidad, tamaños de ventana objetivo y alcance de la futura construcción | Tarea aprobada de implementación visual |

Estas decisiones no bloquean la incorporación documental ya autorizada.
