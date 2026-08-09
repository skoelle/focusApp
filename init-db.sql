-- FocusApp Datenbank initialisieren
-- Fuehren Sie dieses Script mit MariaDB Root-Berechtigung aus

CREATE DATABASE IF NOT EXISTS focusapp CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

CREATE USER IF NOT EXISTS 'focusapp'@'%' IDENTIFIED BY 'change-password';
GRANT ALL PRIVILEGES ON focusapp.* TO 'focusapp'@'%';

FLUSH PRIVILEGES;

USE focusapp;

CREATE TABLE IF NOT EXISTS FocusTasks (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Description VARCHAR(2000) NULL,
    `Order` INT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
