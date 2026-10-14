# Inventario y límites de migración
Fecha: 2026-10-12.
- Espejo inspeccionado: únicamente AGENTS.md sincronizado (392 bytes); sources/ vacío.
- No existe repositorio Git local en el directorio de trabajo.
- Historial leído: cinco turnos disponibles, sin adjuntos; respuestas representadas mediante referencias opacas.
- Confirmados: nombres de roles, secciones de memoria, intención de SDD y destino Codex.
- Ausentes: ZIP/configuración OpenCode, instrucciones originales de roles, SDD anteriores, código y requisitos funcionales.

## Correspondencia preparada
| Origen solicitado | Destino |
|---|---|
| Backend-Coder | .codex/agents/Backend-Coder.toml |
| Frontend-Coder | .codex/agents/Frontend-Coder.toml |
| Tester-datos | .codex/agents/Tester-datos.toml |
| revisor | .codex/agents/revisor.toml |
| Instrucciones del proyecto | AGENTS.md |
| Memoria | memory.md |
| SDD | docs/sdd/01 a 06 |

Los prompts actuales son definiciones nuevas basadas en las responsabilidades de los nombres; no equivalencias verificadas con prompts de OpenCode.

## Reconciliación al recibir originales
Preservar fuentes inmutables; listar archivos y hashes; mapear cada original a destino; comparar requisito por requisito, instrucciones y decisiones; mantener IDs y aprobaciones; registrar omisiones/conflictos; consultar diferencias no resolubles; verificar enlaces, configuración y carga de agentes.
No trasladar claves, proveedores o permisos de OpenCode como si fueran opciones Codex.
No modificar el AGENTS.md sincronizado del espejo. Este paquete tiene su propia raíz importable.

