using LogiTransPro.API.Data;
using LogiTransPro.API.Extensions;
using LogiTransPro.API.Filters;
using LogiTransPro.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ======================================================
// CONFIGURAR PUERTO PARA RAILWAY
// ======================================================
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

// ======================================================
// CONFIGURAR SERILOG (LOGGING)
// ======================================================
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration)
          .Enrich.FromLogContext()
          .WriteTo.Console()
          .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day);
});

// ======================================================
// CONFIGURAR CORS (PARA FRONTEND VUE.JS Y PRODUCCIÓN)
// ======================================================
builder.Services.AddCors(options =>
{
    // Política para desarrollo con orígenes específicos
    options.AddPolicy("AllowVueFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:8080",
                "http://localhost:3000",
                "http://localhost:5173",
                "https://localhost:5173",
                "https://logitransproapi-production.up.railway.app"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });

    // Política para pruebas en producción (permite cualquier origen)
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ======================================================
// AGREGAR SERVICIOS CON FILTROS
// ======================================================
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<ApiExceptionFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LogiTransPro API",
        Version = "v1",
        Description = "API para Sistema de Gestión Logística Inteligente"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// ======================================================
// CONFIGURAR BASE DE DATOS POSTGRESQL
// ======================================================
builder.Services.AddDbContext<LogiTransProDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ======================================================
// CONFIGURAR AUTENTICACIÓN JWT
// ======================================================
var jwtKey = builder.Configuration["Jwt:Key"] ?? "LogiTransPro-SecretKey-2024-Minimum32CharactersLong!";
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

// ======================================================
// CONFIGURAR AUTORIZACIÓN
// ======================================================
builder.Services.AddAuthorization();

// ======================================================
// REGISTRAR FILTROS EN EL CONTENEDOR DE DEPENDENCIAS
// ======================================================
builder.Services.AddScoped<ValidationFilter>();
builder.Services.AddScoped<ApiExceptionFilter>();

// ======================================================
// AGREGAR SERVICIOS PERSONALIZADOS
// ======================================================
builder.Services.AddScoped<LogiTransProDbContext>();
builder.Services.AddCustomServices();

// ======================================================
// CONFIGURAR AUTOMAPPER
// ======================================================
builder.Services.AddAutoMapper(typeof(Program));

// ======================================================
// CONSTRUIR LA APLICACIÓN
// ======================================================
var app = builder.Build();

// ======================================================
// MIDDLEWARE GLOBAL
// ======================================================
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

// ======================================================
// CONFIGURAR PIPELINE HTTP - SWAGGER HABILITADO EN TODOS LOS ENTORNOS
// ======================================================
// Swagger habilitado para producción también
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LogiTransPro API v1");
    c.RoutePrefix = string.Empty;
});

// ======================================================
// USAR CORS - Usar AllowAll para producción (permite cualquier origen)
// ======================================================
app.UseCors("AllowAll");  // Cambiado para permitir cualquier origen en producción

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ======================================================
// VERIFICAR CONEXIÓN A LA BASE DE DATOS
// ======================================================
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LogiTransProDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var canConnect = await dbContext.Database.CanConnectAsync();
        if (canConnect)
        {
            var databaseName = dbContext.Database.GetDbConnection().Database;
            logger.LogInformation("✅ Conexión exitosa a PostgreSQL");
            logger.LogInformation("📊 Base de datos: {DatabaseName}", databaseName);

            try
            {
                var vehiculosCount = await dbContext.Vehiculos.CountAsync();
                logger.LogInformation("📋 Total de vehículos en BD: {Count}", vehiculosCount);
            }
            catch (Exception ex)
            {
                logger.LogWarning("⚠️ No se pudo acceder a la tabla Vehiculos: {Message}", ex.Message);
            }
        }
        else
        {
            logger.LogError("❌ No se pudo conectar a la base de datos");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Error al conectar con PostgreSQL");
        logger.LogError("Detalles: {Message}", ex.Message);
    }
}

// ======================================================
// EJECUTAR LA APLICACIÓN
// ======================================================
app.Run();