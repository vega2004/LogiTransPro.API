using Microsoft.AspNetCore.Mvc;

namespace LogiTransPro.API.Filters
{
    public class AuthorizeRoleAttribute : TypeFilterAttribute
    {
        public AuthorizeRoleAttribute(params string[] roles) : base(typeof(AuthorizeRoleFilter))
        {
            Arguments = new object[] { roles };
        }
    }
}