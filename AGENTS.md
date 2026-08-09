# Projekt-Richtlinien

## CSS / Frontend

- Nach jeder CSS-Anpassung (`*.css` Dateien) immer `cd client && npm run build` ausfuehren um zu pruefen ob der Build fehlerfrei durchlaeuft
- Nach Aenderungen an Komponenten-Dateien (`*.tsx`) ebenfalls `npm run build` ausfuehren

## Docker

- Nach Aenderungen am `Dockerfile` oder `docker-compose.yml` immer `docker build .` ausfuehren
