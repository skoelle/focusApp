// Copyright (c) 2026 Stefan Koelle (https://stefankoelle.de)
// Licensed under the MIT License. See LICENSE file in project root for details.
using FocusApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFilter("Microsoft.AspNetCore.HttpLogging", LogLevel.Information);

// Connection-String aus ENV-Variablen aufbauen (appsettings.json ist Fallback)
static string BuildConnectionString(IConfiguration config)
{
    return $"Server={config["DB_HOST"] ?? "localhost"};" +
           $"Port={config["DB_PORT"] ?? "3306"};" +
           $"Database={config["DB_NAME"] ?? "focusapp"};" +
           $"User={config["DB_USER"] ?? "focusapp"};" +
           $"Password={config["DB_PASSWORD"] ?? "change-password"}";
}

var connectionString = BuildConnectionString(builder.Configuration);

// Add services
builder.Services.AddDbContext<FocusContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
    options.RequestHeaders.Add("traceparent");
    options.RequestBodyLogLimit = 4096;
    options.ResponseBodyLogLimit = 4096;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.UseUrls("http://0.0.0.0:5000");

var app = builder.Build();

// Ensure database is created (mit Retry fuer gestartete MariaDB)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FocusContext>();
    var maxRetries = 5;
    for (int i = 1; i <= maxRetries; i++)
    {
        try
        {
            db.Database.EnsureCreated();
            Console.WriteLine("Database connected!");
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database connection attempt {i}/{maxRetries} failed: {ex.Message}");
            if (i == maxRetries)
            {
                Console.WriteLine("Could not connect to database after max retries. Exiting.");
                throw;
            }
            var delay = Math.Min(i * 5, 30);
            Console.WriteLine($"Retrying in {delay}s...");
            Thread.Sleep(delay * 1000);
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReact");
app.UseHttpLogging();

// STATIC FILES VOR UseRouting!
var clientPath = Path.Combine(app.Environment.ContentRootPath, "client", "build");
Console.WriteLine($"Looking for frontend at: {clientPath}");
Console.WriteLine($"Directory exists: {Directory.Exists(clientPath)}");

if (Directory.Exists(clientPath))
{
    Console.WriteLine("Serving React frontend from client/build/");

    app.UseDefaultFiles(new DefaultFilesOptions
    {
        FileProvider = new PhysicalFileProvider(clientPath)
    });

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(clientPath),
        RequestPath = ""
    });
}
else
{
    Console.WriteLine("client/build/ not found! Run: cd client && npm run build");
}

app.UseAuthorization();
app.MapControllers();

// FALLBACK f�r SPA (alle nicht-API Routes >> index.html)
app.MapFallback(async context =>
{
    var indexPath = Path.Combine(clientPath, "index.html");
    if (File.Exists(indexPath))
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(indexPath);
    }
    else
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Frontend not built. Run: cd client && npm run build");
    }
});

Console.WriteLine("FocusApp started!");
Console.WriteLine("Backend API: http://localhost:5000");
Console.WriteLine("Swagger:     http://localhost:5000/swagger");
Console.WriteLine("Frontend:    http://localhost:5000");

app.Run();
