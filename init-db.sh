#!/bin/bash
# FocusApp Datenbank Initialisierung
# Fuehrt init-db.sql gegen den externen MariaDB Server aus

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# .env laden
if [ -f "$SCRIPT_DIR/.env" ]; then
    export $(grep -v '^#' "$SCRIPT_DIR/.env" | xargs)
else
    echo "Fehler: .env Datei nicht gefunden unter $SCRIPT_DIR/.env"
    exit 1
fi

# Pruefen ob notwendige Variablen gesetzt sind
if [ -z "$DB_HOST" ] || [ -z "$DB_ROOT_PASSWORD" ]; then
    echo "Fehler: DB_HOST und DB_ROOT_PASSWORD muessen in .env gesetzt sein"
    exit 1
fi

DB_PORT=${DB_PORT:-3306}

echo "Verbinde zu MariaDB auf $DB_HOST:$DB_PORT..."

# Pruefen ob mysql/mariadb Client verfuegbar ist
if command -v mariadb &> /dev/null; then
    MYSQL_CMD="mariadb"
elif command -v mysql &> /dev/null; then
    MYSQL_CMD="mysql"
else
    echo "Fehler: weder 'mariadb' noch 'mysql' Client gefunden"
    echo "Installieren Sie: sudo apt install mariadb-client"
    exit 1
fi

# SQL ausfuehren
$MYSQL_CMD -h "$DB_HOST" -P "$DB_PORT" -u root -p"$DB_ROOT_PASSWORD" < "$SCRIPT_DIR/init-db.sql"

echo "Datenbank focusapp erfolgreich initialisiert!"
