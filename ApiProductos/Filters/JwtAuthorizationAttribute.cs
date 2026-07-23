using Application.Contrato;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Linq;

namespace ApiProductos.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JwtAuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var logger = context.HttpContext.RequestServices.GetService<ILogger<JwtAuthorizationAttribute>>();
            var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();
            var securitySettings = context.HttpContext.RequestServices.GetService<Microsoft.Extensions.Options.IOptions<Models.Utils.SecuritySettings>>()?.Value;

            if (jwtService == null)
            {
                logger?.LogError("Servicio de autenticación (IJwtService) no está disponible en RequestServices.");
                context.Result = new JsonResult(RespuestaApi<object>.Error("Servicio de autenticación no disponible")) { StatusCode = 500 };
                return;
            }

            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                logger?.LogWarning("Acceso no autorizado: Token de autorización no proporcionado o formato inválido.");
                context.Result = new JsonResult(RespuestaApi<object>.Error("Token de autorización no proporcionado", 401)) { StatusCode = 401 };
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            try
            {
                var userId = jwtService.ValidateTokenAndGetUserId(token);
                if (userId == null)
                {
                    logger?.LogWarning("Acceso no autorizado: Token válido pero no se pudo extraer el ID del usuario.");
                    context.Result = new JsonResult(RespuestaApi<object>.Error("Token válido pero no se pudo extraer el usuario", 401)) { StatusCode = 401 };
                    return;
                }

                context.HttpContext.Items["UserId"] = userId.Value;
                logger?.LogInformation("Usuario {UserId} autenticado exitosamente vía JWT.", userId.Value);
            }
            catch (Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException ex)
            {
                var now = DateTime.UtcNow;
                logger?.LogWarning(ex, "Intento de acceso fallido: El token JWT ha expirado. Hora servidor: {ServerTime}", now);
                context.Result = new JsonResult(RespuestaApi<object>.Error($"El token ha expirado. Hora servidor (UTC): {now:yyyy-MM-dd HH:mm:ss}", 401)) { StatusCode = 401 };
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "Intento de acceso fallido con token inválido.");
                context.Result = new JsonResult(RespuestaApi<object>.Error($"Token inválido: {ex.Message}", 401)) { StatusCode = 401 };
            }
        }
    }
}
