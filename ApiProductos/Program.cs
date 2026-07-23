using Application.Contrato;
using Domain.Services;
using Domain.Utils;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.Utils;
using Serilog;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando la API de Productos...");
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // Configuración de AppSettings
    ConfigurationManager configuration = builder.Configuration;

    // Add services to the container.
    builder.Services.AddControllers();

    // Personalizar respuesta de error de validación de modelo
    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var errors = context.ModelState
                .Where(e => e.Value != null && e.Value.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(y => y.ErrorMessage))
                .ToList();

            var errorMessage = string.Join(" | ", errors);
            var requestPath = context.HttpContext.Request.Path;

            logger.LogWarning("Fallo de validación de modelo en {Path}: {Errors}", requestPath, errorMessage);

            return new BadRequestObjectResult(new { mensaje = string.Join(", ", errors) });
        };
    });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContext<InventarioContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSQL"));
    });

    // Configuración de SecuritySettings
    builder.Services.Configure<SecuritySettings>(configuration.GetSection("Security"));
    SecuritySettings securitySettings = new();
    configuration.GetSection("Security").Bind(securitySettings);

    // Configuración de servicios
    builder.Services.AddRouting(options => options.LowercaseUrls = true);

    builder.Services.AddScoped<IUsuarioService, UsuarioService>();
    builder.Services.AddScoped<IProductoService, ProductoService>();
    builder.Services.AddScoped<IProveedorService, ProveedorService>();
    builder.Services.AddScoped<ILoteService, LoteService>();

    // Inyección de dependencias - Servicios de Seguridad
    builder.Services.AddSingleton<IAesEncryptionService, AesEncryptionService>();
    builder.Services.AddSingleton<IJwtService, JwtService>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación terminó inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}
