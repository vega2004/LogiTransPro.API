using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace LogiTransPro.API.Filters
{
    public class AuthorizeRoleFilter : IAuthorizationFilter
    {
        private readonly string[] _allowedRoles;
        private readonly ILogger<AuthorizeRoleFilter> _logger;

        public AuthorizeRoleFilter(string[] allowedRoles, ILogger<AuthorizeRoleFilter> logger)
        {
            _allowedRoles = allowedRoles;
            _logger = logger;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                _logger.LogWarning("Usuario no autenticado");
                context.Result = new UnauthorizedResult();
                return;
            }

            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userRole) || !_allowedRoles.Contains(userRole))
            {
                _logger.LogWarning("Usuario con rol {UserRole} no autorizado para este recurso", userRole);
                context.Result = new ForbidResult();
            }
        }
    }
}