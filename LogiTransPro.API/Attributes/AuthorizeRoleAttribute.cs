using Microsoft.AspNetCore.Authorization;
using LogiTransPro.API.Constants;

namespace LogiTransPro.API.Attributes
{
    /// <summary>
    /// Atributo personalizado para autorización basada en roles
    /// </summary>
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Constructor que acepta uno o más roles
        /// </summary>
        /// <param name="roles">Lista de roles permitidos</param>
        public AuthorizeRoleAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }

        /// <summary>
        /// Constructor que acepta un rol específico
        /// </summary>
        /// <param name="role">Rol permitido</param>
        public AuthorizeRoleAttribute(string role)
        {
            Roles = role;
        }

        /// <summary>
        /// Constructor por defecto - permite cualquier usuario autenticado
        /// </summary>
        public AuthorizeRoleAttribute()
        {
            // Solo requiere autenticación, sin restricción de roles específicos
        }
    }

    /// <summary>
    /// Atributo para acciones que solo pueden ser ejecutadas por Administradores
    /// </summary>
    public class AdminOnlyAttribute : AuthorizeRoleAttribute
    {
        public AdminOnlyAttribute() : base(RolesConstants.Admin)
        {
        }
    }

    /// <summary>
    /// Atributo para acciones que pueden ser ejecutadas por Administradores y Supervisores
    /// </summary>
    public class AdminOrSupervisorAttribute : AuthorizeRoleAttribute
    {
        public AdminOrSupervisorAttribute() : base(RolesConstants.Admin, RolesConstants.Supervisor)
        {
        }
    }

    /// <summary>
    /// Atributo para acciones que pueden ser ejecutadas por Administradores, Supervisores y Operadores
    /// </summary>
    public class AdminSupervisorOrOperadorAttribute : AuthorizeRoleAttribute
    {
        public AdminSupervisorOrOperadorAttribute() : base(RolesConstants.Admin, RolesConstants.Supervisor, RolesConstants.Operador)
        {
        }
    }

    /// <summary>
    /// Atributo para acciones exclusivas de Choferes
    /// </summary>
    public class ChoferOnlyAttribute : AuthorizeRoleAttribute
    {
        public ChoferOnlyAttribute() : base(RolesConstants.Chofer)
        {
        }
    }
}