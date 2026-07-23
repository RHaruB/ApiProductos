using Application.Contrato;
using Domain.Services;
using Domain.Utils;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.Utils;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Configuración de AppSettings
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
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


//Configuración de servicios
builder.Services.AddRouting(options => options.LowercaseUrls = true);


builder.Services.AddScoped<IUsuarioService, UsuarioService>(); 
builder.Services.AddScoped<IAesEncryptionService, AesEncryptionService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
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
