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
            new FocusTask { Id = 1, Title = "Schrank im Schlafzimmer montieren", Description = "IKEA PAX - Anleitung und Werkzeug bereitlegen", Order = 1, CreatedAt = now, UpdatedAt = now },
            new FocusTask { Id = 2, Title = "Winterreifen Termin vereinbaren", Description = "Reifencenter anrufen, Wochentag abstimmen", Order = 2, CreatedAt = now, UpdatedAt = now },
            new FocusTask { Id = 3, Title = "Zahnarzt Vorsorge terminieren", Description = "Praxis anrufen, halbjährigen Termin ausmachen", Order = 3, CreatedAt = now, UpdatedAt = now },
            new FocusTask { Id = 4, Title = "Gartenzaun streichen", Description = "Holzschutzfarbe kaufen, Wetter abwarten", Order = 4, CreatedAt = now, UpdatedAt = now }
        );
    }
}
