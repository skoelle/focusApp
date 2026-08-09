# FocusApp - Todo Application

Eine moderne, minimalistische Todo-Anwendung mit Drag & Drop, gebaut mit React und ASP.NET Core.

## 🚀 Features

- ✅ **Todo-Verwaltung** - Erstellen, Bearbeiten, Löschen von Aufgaben
- 🎯 **Drag & Drop** - Intuitive Neuordnung der Aufgaben
- 🎨 **Dark/Light Mode** - Automatische Theme-Erkennung
- 📱 **Responsive Design** - Optimiert für Desktop und Mobile
- 💾 **Persistente Speicherung** - Daten bleiben nach Neustart erhalten
- 🔄 **RESTful API** - Saubere Backend-Architektur

## 📋 Voraussetzungen

- **.NET 9.0 SDK** oder höher
- **Node.js 18+** und **npm** (für React-Entwicklung)
- **Linux Server** (Ubuntu/Debian empfohlen) für Deployment

## 🛠️ Installation & Setup

### 1. Projekt klonen/kopieren

```bash
# Projektverzeichnis erstellen
sudo mkdir -p /opt/tools/FocusApp
sudo chown $USER:$USER /opt/tools/FocusApp
```

### 2. .NET Runtime installieren

```bash
# .NET 9.0 Runtime herunterladen
cd /tmp
wget https://download.visualstudio.microsoft.com/download/pr/...dotnet-runtime-9.0.0-linux-x64.tar.gz

# Entpacken nach /opt/dotnet
sudo mkdir -p /opt/dotnet
sudo tar -xzf dotnet-runtime-9.0.0-linux-x64.tar.gz -C /opt/dotnet

# PATH setzen
echo 'export PATH=$PATH:/opt/dotnet' >> ~/.bashrc
source ~/.bashrc
```

### 3. React Frontend bauen

```bash
cd /pfad/zu/deinem/projekt/FocusApp/ClientApp

# Dependencies installieren
npm install

# Production Build erstellen
npm run build
```

**Wichtig:** Der Build landet in `ClientApp/build/` und wird automatisch vom Backend ausgeliefert.

### 4. ASP.NET Backend veröffentlichen

```bash
cd /pfad/zu/deinem/projekt/FocusApp

# Release Build erstellen
dotnet publish -c Release -o /opt/tools/FocusApp
```

## ⚙️ Konfiguration

### appsettings.Production.json

Erstelle `/opt/tools/FocusApp/appsettings.Production.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

**Log Levels:**
- `Error` - Nur Fehler
- `Warning` - Warnungen + Fehler (empfohlen)
- `Information` - Mehr Details
- `None` - Kein Logging

## 🔧 Systemd Service Setup

### Service-Datei erstellen

```bash
sudo vim /etc/systemd/system/focusapp.service
```

**Inhalt:**

```ini
[Unit]
Description=FocusApp Todo Application
After=network.target

[Service]
Type=simple
WorkingDirectory=/opt/tools/FocusApp
ExecStart=/opt/dotnet/dotnet /opt/tools/FocusApp/FocusApp.dll
Restart=always
RestartSec=10
User=stefan
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_ROOT=/opt/dotnet
Environment=ASPNETCORE_URLS=http://0.0.0.0:5000
SyslogIdentifier=focusapp
StandardOutput=journal
StandardError=journal

[Install]
WantedBy=multi-user.target
```

**Wichtig:** 
- `Type=simple` verwenden (nicht `notify`)
- `User` anpassen auf deinen Linux-User
- Port `5000` ist der Standard, kann angepasst werden

### Service aktivieren

```bash
# Service neu laden
sudo systemctl daemon-reload

# Service starten
sudo systemctl start focusapp.service

# Service beim Boot aktivieren
sudo systemctl enable focusapp.service

# Status prüfen
sudo systemctl status focusapp.service
```

## 📡 API Endpoints

### Todos abrufen
```http
GET /api/todos
```

**Response:**
```json
[
  {
    "id": 1,
    "title": "Erste Aufgabe",
    "description": "Beschreibung",
    "createdAt": "2026-01-29T22:00:00Z",
    "order": 0
  }
]
```

### Todo erstellen
```http
POST /api/todos
Content-Type: application/json

{
  "title": "Neue Aufgabe",
  "description": "Optional"
}
```

### Todo aktualisieren
```http
PUT /api/todos/{id}
Content-Type: application/json

{
  "id": 1,
  "title": "Geändert",
  "description": "Neue Beschreibung"
}
```

### Todo löschen
```http
DELETE /api/todos/{id}
```

### Reihenfolge aktualisieren
```http
POST /api/todos/reorder
Content-Type: application/json

[1, 3, 2, 4]
```

## 🎯 Entwicklung

### Backend starten (Development)

```bash
cd FocusApp
dotnet run
```

API läuft auf: `http://localhost:5000`

### Frontend starten (Development)

```bash
cd FocusApp/ClientApp
npm start
```

React Dev Server läuft auf: `http://localhost:3000`

**Proxy:** API-Calls werden automatisch an `http://localhost:5000` weitergeleitet (siehe `package.json`).

### Datenbank

Todos werden in einer **SQLite-Datenbank** gespeichert:
- Datei: `/opt/tools/FocusApp/todos.db`
- Automatische Erstellung beim ersten Start
- Entity Framework Core mit Code-First Migrations

## 🐛 Troubleshooting

### Service startet nicht

```bash
# Logs ansehen
sudo journalctl -u focusapp.service -n 50 --no-pager

# Manuell testen
cd /opt/tools/FocusApp
/opt/dotnet/dotnet FocusApp.dll
```

### Service hängt bei "starting"

**Problem:** `Type=notify` statt `Type=simple` in Service-Datei.

**Lösung:** Service-Datei editieren, `Type=simple` verwenden, dann:
```bash
sudo systemctl daemon-reload
sudo systemctl restart focusapp.service
```

### Port bereits belegt

```bash
# Port-Verwendung prüfen
sudo netstat -tlnp | grep 5000

# Anderen Port in Service-Datei setzen
Environment=ASPNETCORE_URLS=http://0.0.0.0:5001
```

### Datenbank-Fehler

```bash
# Datenbank löschen und neu erstellen lassen
rm /opt/tools/FocusApp/todos.db
sudo systemctl restart focusapp.service
```

## 📦 Projektstruktur

```
FocusApp/
├── ClientApp/                 # React Frontend
│   ├── public/
│   ├── src/
│   │   ├── components/       # React Komponenten
│   │   ├── App.js           # Haupt-App
│   │   ├── App.css          # Styles
│   │   └── index.js         # Entry Point
│   ├── package.json
│   └── build/               # Production Build (nach npm run build)
├── Controllers/
│   └── TodosController.cs   # API Controller
├── Models/
│   ├── TodoItem.cs          # Todo Model
│   └── TodoContext.cs       # EF Core DbContext
├── Program.cs               # ASP.NET Startup
├── FocusApp.csproj          # Projekt-Datei
├── appsettings.json         # Basis-Config
├── appsettings.Production.json  # Production-Config
└── todos.db                 # SQLite Datenbank (runtime)
```

## 🔐 Sicherheit

### Firewall-Regeln

```bash
# Nur lokalen Zugriff erlauben (Standard)
sudo ufw deny 5000

# Für Netzwerkzugriff:
sudo ufw allow 5000/tcp
```

### Reverse Proxy (nginx)

Für Production empfohlen:

```nginx
server {
    listen 80;
    server_name focus.example.com;

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

## 📊 Monitoring

### Service-Status prüfen

```bash
# Aktiver Status
sudo systemctl status focusapp.service

# Letzte Logs
sudo journalctl -u focusapp.service -n 50

# Live-Logs folgen
sudo journalctl -u focusapp.service -f
```

### Ressourcen-Nutzung

```bash
# Prozess finden
ps aux | grep FocusApp

# Speicher/CPU-Nutzung
top -p $(pgrep -f FocusApp.dll)
```

## 🚀 Updates & Deployment

### 1. Code aktualisieren

```bash
# Frontend neu bauen
cd ClientApp
npm run build

# Backend neu veröffentlichen
cd ..
dotnet publish -c Release -o /opt/tools/FocusApp
```

### 2. Service neu starten

```bash
sudo systemctl restart focusapp.service
sudo systemctl status focusapp.service
```

### 3. Datenbank-Migration (bei Schema-Änderungen)

```bash
cd FocusApp

# Migration erstellen
dotnet ef migrations add MigrationName

# Migration anwenden
dotnet ef database update

# Oder automatisch beim Start (bereits konfiguriert in Program.cs)
```

## 📝 Nützliche Befehle

```bash
# Service-Befehle
sudo systemctl start focusapp.service      # Starten
sudo systemctl stop focusapp.service       # Stoppen
sudo systemctl restart focusapp.service    # Neustarten
sudo systemctl status focusapp.service     # Status
sudo systemctl enable focusapp.service     # Auto-Start aktivieren
sudo systemctl disable focusapp.service    # Auto-Start deaktivieren

# Logs
sudo journalctl -u focusapp.service        # Alle Logs
sudo journalctl -u focusapp.service -f     # Live-Logs
sudo journalctl -u focusapp.service --since "1 hour ago"  # Letzte Stunde

# Datenbank
sqlite3 /opt/tools/FocusApp/todos.db       # DB öffnen
.tables                                     # Tabellen anzeigen
SELECT * FROM TodoItems;                    # Alle Todos
.quit                                       # Beenden
```

## 🎨 Design System

Die App verwendet ein eigenes Design System mit:
- CSS Custom Properties für theming
- Responsive Design (Mobile-First)
- Accessibility-Features (ARIA, Keyboard-Navigation)
- Dark/Light Mode Support


---

**Version:** 1.0.0  
**Letzte Aktualisierung:** 29. Januar 2026  
**Status:** ✅ Production Ready
