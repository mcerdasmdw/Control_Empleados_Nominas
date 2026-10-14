# Clarificación
## Referencias visuales — 2026-10-13

R-UI02 autoriza documentación y uso futuro de los PNG como referencia. D-UI01 (identidad y logo), D-UI02 (tokens exactos y tema oscuro), D-UI03 (menú y módulos) y D-UI04 (alcance de implementación y fidelidad) siguen pendientes; detalle y dependencias en [Diseño visual](07-diseno-visual.md). No bloquean T-UI02 documental. Las cifras y notas legales de las imágenes no resuelven D-03/D-04.

`DECISIONS.md` contiene entradas tituladas «Decisiones Confirmadas» y propuestas funcionales; se encontró y leyó como antecedente local, pero su texto no aporta persona, fecha ni referencia verificable de aprobación. D-02 a D-06 requieren reconciliar este antecedente con las fuentes originales; no tratarlo como ausente ni como aprobación nueva derivada de R-UI02.

Todas las entradas están abiertas; ninguna respuesta ha sido asumida.

| ID | Información o decisión pendiente | Evidencia necesaria | Trabajo dependiente |
|---|---|---|---|
| D-01 | ZIP OpenCode, prompts y SDD originales | Archivos originales | Migración fiel y comparación |
| D-02 | Requisitos y alcance MVP aprobados | Especificación y aprobación | Funcionalidades y aceptación |
| D-03 | Jurisdicción y reglas de nómina | Requisitos aprobados y fuentes normativas aplicables | Cálculos |
| D-04 | Moneda, períodos, precisión y redondeo | Decisión y ejemplos esperados | Modelo y pruebas numéricas |
| D-05 | Stack y arquitectura previamente elegidos | Plan técnico original o aprobación nueva | Código y dependencias |
| D-06 | Usuarios, permisos, datos e integraciones | Requisitos de acceso y contratos | Autenticación e integración |
| D-07 | Repositorio y entorno objetivo | Ruta/URL y configuración existente | Integración y comprobación de carga |

Al resolver: conservar pregunta, respuesta exacta, responsable, fecha, fuente, requisitos afectados y cambios derivados. Consultar primero los originales: estas preguntas pueden haber sido respondidas allí.


2026-10-13: R-UI04 autoriza ajustar únicamente presentación de LoginWindow. Se conservan identidad, recursos de ambos temas y 420×600; D-UI01 a D-UI04 permanecen abiertas para el rediseño general y no bloquean este alcance. Criterios técnicos derivados registrados en especificación; no se presentan como aprobación individual del usuario.

R-UI05: reparación autorizada por reporte actual de botones desactivados y elemento rojo. Se conectan servicios y callback existentes sin nuevas reglas, marca ni dependencias; no requiere resolver decisiones del rediseño general.

2026-10-13 R-A01: nombre Administrador y contraseña inicial proporcionados explícitamente por usuario; se autoriza inserción puntual en base local de App con rol Administrador existente. No se infiere un mecanismo permanente de cuentas predeterminadas ni se resuelven otros requisitos D-06.
