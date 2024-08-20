using APIArena.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIArena", Version = "v1" });
});

builder.Services.AddDbContextWithPooledFactory<DataContext>(options =>
{
    string? connstring = builder.Configuration.GetConnectionString("APIArena");

    // Get the Connection string based on the multi-connection string setup
#if DEBUG
    connstring = builder.Configuration.GetConnectionString(builder.Configuration.GetConnectionString("Current") ?? "APIArena") ?? connstring;
#endif

    // Error if none is present
    ArgumentException.ThrowIfNullOrWhiteSpace(connstring);

    options.UseMySql(connstring, ServerVersion.AutoDetect(connstring));

#if DEBUG
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
#endif

    options.LogTo(m => System.Diagnostics.Debug.WriteLine(m));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
