// Copyright (c) 2026 Stefan Koelle (https://stefankoelle.de)
// Licensed under the MIT License. See LICENSE file in project root for details.
using FocusApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FocusApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly FocusContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(FocusContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetHealth()
    {
        var health = new
        {
            status = "healthy",
            database = "unknown",
            timestamp = DateTime.UtcNow,
            version = "2.0.0"
        };

        try
        {
            await _context.Database.CanConnectAsync();
            health = new
            {
                status = "healthy",
                database = "connected",
                timestamp = DateTime.UtcNow,
                version = "2.0.0"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            health = new
            {
                status = "unhealthy",
                database = "disconnected",
                timestamp = DateTime.UtcNow,
                version = "2.0.0"
            };
            return StatusCode(503, health);
        }

        return Ok(health);
    }
}
