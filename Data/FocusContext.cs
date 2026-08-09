// Copyright (c) 2026 Stefan Koelle (https://stefankoelle.de)
// Licensed under the MIT License. See LICENSE file in project root for details.
using FocusApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FocusApp.Data;

public class FocusContext : DbContext
{
    public DbSet<FocusTask> FocusTasks { get; set; }

    public FocusContext(DbContextOptions<FocusContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FocusTask>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Order).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });

        // Demo-Tasks fuer lokalen Test
        var now = DateTime.UtcNow;
        modelBuilder.Entity<FocusTask>().HasData(
            new FocusTask { Id = 1, Title = "Projektstruktur klären", Description = "README, AGENTS.md, Lizenz", Order = 1, CreatedAt = now, UpdatedAt = now },
            new FocusTask { Id = 2, Title = "API finalisieren", Description = "Health Endpoint, Validierung", Order = 2, CreatedAt = now, UpdatedAt = now },
            new FocusTask { Id = 3, Title = "Frontend polishieren", Description = "CSS, Responsive, Tests", Order = 3, CreatedAt = now, UpdatedAt = now },
            new FocusTask { Id = 4, Title = "Docker Setup prüfen", Description = "Healthcheck, CI/CD", Order = 4, CreatedAt = now, UpdatedAt = now },
            new FocusTask { Id = 5, Title = "Doku vervollständigen", Description = "API Docs, Deployment Guide", Order = 5, CreatedAt = now, UpdatedAt = now }
        );
    }
}
