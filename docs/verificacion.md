# Verificación del paquete
Fecha: 2026-10-12.

- Sintaxis TOML válida: configuración y cuatro agentes.
- Cuatro nombres y campos obligatorios presentes; revisor configurado en solo lectura.
- Seis fases SDD presentes; enlaces locales resueltos.
- ZIP incluye archivos ocultos .codex; integridad CRC y correspondencia de bytes comprobadas.
- SHA-256 de cada archivo registrado en manifest.json.

Límites: no se verificó carga real en Codex, no hay aplicación ni pruebas funcionales. No se puede certificar conservación de requisitos ausentes. El espejo y sources/ no se modificaron.
