-- FocusApp Datenbank initialisieren
-- Fuehren Sie dieses Script mit MariaDB Root-Berechtigung aus

CREATE DATABASE IF NOT EXISTS focusapp CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

CREATE USER IF NOT EXISTS 'focusapp'@'%' IDENTIFIED BY 'change-password';
GRANT ALL PRIVILEGES ON focusapp.* TO 'focusapp'@'%';

FLUSH PRIVILEGES;
