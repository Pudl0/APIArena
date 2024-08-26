using APIArena.Middleware;
using APIArena.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using APIArena.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using APIArena.Services;
using APIArena.Attributes;
using Tellerando.Infrastructure.Services;
using APIArena.Controllers;
using APIArena.DTO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication().AddBearerToken();
builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIArena", Version = "v1" });
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "X-API-KEY",
        Type = SecuritySchemeType.ApiKey,
        Description = "API Key needed to access the endpoints"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    }
                },
                Array.Empty<string>()
            }
        });
});

builder.Services.AddSingleton<IAuthorizationHandler, AuthorizationHandler>();
builder.Services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();

builder.Services.AddScoped<ApiKeyService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<MapService>();
builder.Services.AddScoped<PlayerService>();

builder.Services.AddScoped<SettingService>();

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

// always set default values for the settings
using (var scope = app.Services.CreateScope())
{
    var settingsService = scope.ServiceProvider.GetRequiredService<SettingService>();

    settingsService.SetSetting("GameMode", GameDTO.GameMode.PvE);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
