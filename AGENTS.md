# Projekt-Richtlinien

## CSS / Frontend

- Nach jeder CSS-Anpassung (`*.css` Dateien) immer `cd client && npm run build` ausfuehren um zu pruefen ob der Build fehlerfrei durchlaeuft
- Nach Aenderungen an Komponenten-Dateien (`*.tsx`) ebenfalls `npm run build` ausfuehren

## .NET / Backend

- Nach Aenderungen an `.cs` Dateien immer `dotnet build` ausfuehren
- Anschliessend `dotnet test` im `FocusApp.Tests` Verzeichnis ausfuehren

## Docker

- Nach Aenderungen am `Dockerfile` oder `docker-compose.yml` immer `docker build .` ausfuehren

## License

MIT License - Copyright (c) 2026 Stefan Koelle (https://stefankoelle.de)
- Full text in `LICENSE`
- License headers in all source code files
