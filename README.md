# FocusApp - Todo Application

Eine moderne, minimalistische Todo-Anwendung mit Drag & Drop, gebaut mit React und ASP.NET Core 8.0.

## 🚀 Features

- ✅ **Todo-Verwaltung** - Erstellen, Bearbeiten, Loschen von Aufgaben
- 🎯 **Drag & Drop** - Intuitive Neuordnung der Aufgaben
- 📱 **Responsive Design** - Optimiert fur Desktop und Mobile
- 🔄 **RESTful API** - Saubere Backend-Architektur

## 📋 Voraussetzungen

- **Docker** und **Docker Compose**
- **MariaDB** (externer Host, z.B. LXC Container)
- **Git**

## 🐳 Deployment

### 1. MariaDB initialisieren (einmalig)

Die FocusApp benötigt eine MariaDB Datenbank. Das Init-Script erstellt die Datenbank und den User auf dem externen MariaDB-Server.

```bash
# .env aus Template erstellen und editieren
cp .env.example .env
vim .env

# Init-Script als MariaDB Root ausfuehren
mysql -h $DB_HOST -u root -p < init-db.sql
```

Die `.env.example` enthalt:
```
DB_HOST=maria-db-server.domain.local
DB_PORT=3306
DB_NAME=focusapp
DB_USER=focusapp
DB_PASSWORD=<CHANGE_ME>
```

### 2. Docker Image bauen

```bash
docker compose build
```

Oder direkt aus dem GitHub Container Registry pullen (nach dem ersten CI-Run):

```bash
docker compose pull
```

### 3. App starten

```bash
docker compose up -d
```

Die App laeuft auf: `http://localhost:5000`

### 4. Logs anzeigen

```bash
docker compose logs -f
```

## 🔄 CI/CD mit GitHub Actions

Bei jedem Push auf `main` wird automatisch:

1. Das Docker-Image gebaut (Multi-Stage: Node + .NET)
2. Nach `ghcr.io` (GitHub Container Registry) gepusht
3. Alte Images aufgeraeumt (letzte 4 bleiben erhalten)

### Manuelles Deployment auf dem Host

```bash
# Neuestes Image pullen
docker compose pull

# Container neustarten
docker compose up -d
```

## ⚙️ Konfiguration

Die App wird ueber Umgebungsvariablen konfiguriert, die in `docker-compose.yml` gesetzt werden:

| Variable | Beschreibung |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` oder `Development` |
| `DB_HOST` | MariaDB Host |
| `DB_PORT` | MariaDB Port |
| `DB_NAME` | Datenbankname |
| `DB_USER` | Datenbank-User |
| `DB_PASSWORD` | Datenbank-Passwort |

## 📡 API Endpoints

### Todos abrufen
```http
GET /api/focustasks
```

### Todo erstellen
```http
POST /api/focustasks
Content-Type: application/json

{
  "title": "Neue Aufgabe",
  "description": "Optional"
}
```

### Todo aktualisieren
```http
PUT /api/focustasks/{id}
Content-Type: application/json

{
  "id": 1,
  "title": "Geaendert",
  "description": "Neue Beschreibung"
}
```

### Todo loeschen
```http
DELETE /api/focustasks/{id}
```

### Reihenfolge aktualisieren
```http
POST /api/focustasks/reorder
Content-Type: application/json

[1, 3, 2, 4]
```

### Health Check
```http
GET /api/health
```

Response (200 OK):
```json
{
  "status": "healthy",
  "database": "connected",
  "timestamp": "2026-08-09T12:00:00Z",
  "version": "2.0.0"
}
```

Response (503 Service Unavailable):
```json
{
  "status": "unhealthy",
  "database": "disconnected",
  "timestamp": "2026-08-09T12:00:00Z",
  "version": "2.0.0"
}
```

**Uptime Kuma Konfiguration:**
- URL: `http://localhost:5000/api/health`
- Methode: GET
- Erwarteter Status: 200
- Intervall: 60 Sekunden

**Docker Healthcheck:**
- Automatisch in `docker-compose.yml` konfiguriert
- Prüft alle 30 Sekunden den Health Endpoint
- Container wird als `healthy`/`unhealthy` markiert

## 🎯 Entwicklung

### Backend (Development)

```bash
dotnet run
```

API laeuft auf: `http://localhost:5000`

### Frontend (Development)

```bash
cd client
npm install
npm run dev
```

React Dev Server laeuft auf: `http://localhost:5173`
API-Calls werden automatisch an `http://localhost:5000` weitergeleitet (siehe `vite.config.ts`).

## 📦 Projektstruktur

```
FocusApp/
├── client/                    # React Frontend (TypeScript, Vite)
│   ├── src/
│   │   ├── components/        # React Komponenten
│   │   │   ├── TaskCard.tsx
│   │   │   └── TaskForm.tsx
│   │   ├── styles/            # CSS Design System
│   │   ├── App.tsx            # Haupt-App
│   │   ├── api.ts             # Axios API Client
│   │   ├── types.ts           # TypeScript Types
│   │   └── main.tsx           # Entry Point
│   ├── build/                 # Production Build
│   └── package.json
├── Controllers/
│   └── FocusTasksController.cs
├── Data/
│   └── FocusContext.cs        # EF Core DbContext
├── Models/
│   ├── FocusTask.cs           # Domain Model
│   └── Dtos.cs                # API Data Transfer Objects
├── Properties/
├── Program.cs                 # ASP.NET Startup
├── FocusApp.csproj            # Projekt-Datei
├── FocusApp.sln               # Solution File
├── Directory.Build.props
├── appsettings.json           # Config
├── Dockerfile                 # Multi-Stage Docker Build
├── docker-compose.yml         # Docker Compose Konfiguration
├── .env.example               # Environment Template
├── init-db.sql                # MariaDB Init Script
├── LICENSE                    # MIT License
└── renovate.json              # Dependency Updates
```

## 🐛 Troubleshooting

### Container startet nicht

```bash
# Logs pruefen
docker compose logs app

# Container Status
docker compose ps
```

### Datenbank-Fehler

```bash
# Pruefen ob MariaDB erreichbar ist
mysql -h $DB_HOST -u $DB_USER -p

# DB neu initialisieren
mysql -h $DB_HOST -u root -p < init-db.sql
```

### Port bereits belegt

Port in `docker-compose.yml` aendern:
```yaml
ports:
  - "5001:5000"
```

## 🔐 Sicherheit

### Authentifizierung mit Authelia

Die FocusApp hat **keine eigene Authentifizierung**. Alle Endpoints sind offen zugänglich. Für einen Produktiveinsatz **muss** die App hinter einem Authentifizierungs-Proxy betrieben werden.

[Authelia](https://www.authelia.com/) stellt Single Sign-On (SSO) bereit und schützt die App mit einem Reverse Proxy.

**Architektur:**
```
Browser → nginx + Authelia → FocusApp (Docker)
                ↓
         MariaDB (extern)
```

**Voraussetzungen:**
- Authelia läuft als eigener Container/Dienst
- nginx als Reverse Proxy mit Authelia-Integration

**nginx-Konfiguration mit Authelia:**

```nginx
# Authelia snippet einbinden
include /etc/nginx/authelia/authelia-location.conf;

server {
    listen 443 ssl;
    server_name focus.example.com;

    ssl_certificate     /etc/ssl/certs/focus.crt;
    ssl_certificate_key /etc/ssl/private/focus.key;

    # Schuetzt alle Routes hinter Authelia
    location / {
        include /etc/nginx/authelia/authelia-authrequest.conf;
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header Remote-User $remote_user;
    }
}
```

**Authelia-Konfiguration (`configuration.yml`):**

```yaml
access_control:
  default_policy: one_factor
  rules:
    # API-Endpoints ebenfalls schuetzen
    - domain: focus.example.com
      resources:
        - "^/api/.*$"
      policy: one_factor

session:
  name: focusapp_session
  secret: "your-session-secret"
  cookies:
    - domain: focus.example.com
      authelia_url: https://auth.example.com
      default_redirection_url: https://focus.example.com

identity_providers:
  - id: ldap
    # oder: - id: openid_connect
```

**Wichtig:**
- Ohne Authelia ist die App komplett offen — kein Schutz für API oder Frontend
- Authelia prüft vor jedem Request die Sitzung
- Der `/api/` Pfad muss ebenfalls geschützt werden (nicht nur `/`)
- Für API-Clients (z.B. Mobile Apps) kann eine API-Key-Lösung implementiert werden

### Reverse Proxy (nginx) ohne Authelia

Für lokale Entwicklung/Tests ohne Auth:

```nginx
server {
    listen 443 ssl;
    server_name focus.example.com;

    ssl_certificate     /etc/ssl/certs/focus.crt;
    ssl_certificate_key /etc/ssl/private/focus.key;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

---

## License

Licensed under the [MIT License](LICENSE) - Copyright (c) 2026 Stefan Koelle (https://stefankoelle.de)

