using Application.Contrato;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models;

namespace ApiProductos.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JwtAuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();
            var securitySettings = context.HttpContext.RequestServices.GetService<Microsoft.Extensions.Options.IOptions<Models.Utils.SecuritySettings>>()?.Value;

            if (jwtService == null)
            {
                context.Result = new JsonResult(RespuestaApi<object>.Error("Servicio de autenticación no disponible")) { StatusCode = 500 };
                return;
            }

            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new JsonResult(RespuestaApi<object>.Error("Token de autorización no proporcionado", 401)) { StatusCode = 401 };
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            try
            {
                var userId = jwtService.ValidateTokenAndGetUserId(token);
                if (userId == null)
                {
                    context.Result = new JsonResult(RespuestaApi<object>.Error("Token válido pero no se pudo extraer el usuario", 401)) { StatusCode = 401 };
                    return;
                }

                context.HttpContext.Items["UserId"] = userId.Value;
            }
            catch (Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException)
            {
                var now = DateTime.UtcNow;
                context.Result = new JsonResult(RespuestaApi<object>.Error($"El token ha expirado. Hora servidor (UTC): {now:yyyy-MM-dd HH:mm:ss}", 401)) { StatusCode = 401 };
            }
            catch (Exception ex)
            {
                context.Result = new JsonResult(RespuestaApi<object>.Error($"Token inválido: {ex.Message}", 401)) { StatusCode = 401 };
            }
        }
    }
}
