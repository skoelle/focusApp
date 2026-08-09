using FocusApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<FocusContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.UseUrls("http://0.0.0.0:5000");

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FocusContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReact");

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

// FALLBACK für SPA (alle nicht-API Routes >> index.html)
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
