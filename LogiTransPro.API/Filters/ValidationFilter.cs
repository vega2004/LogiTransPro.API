using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using LogiTransPro.API.Models.ViewModels;

namespace LogiTransPro.API.Filters
{
    public class ValidationFilter : IActionFilter
    {
        private readonly ILogger<ValidationFilter> _logger;

        public ValidationFilter(ILogger<ValidationFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e => new ValidationErrorResponse
                    {
                        Property = x.Key,
                        Error = e.ErrorMessage
                    }))
                    .ToList();

                var response = ApiResponse<object>.Error(
                    "Error de validación",
                    errors.Select(e => $"{e.Property}: {e.Error}").ToList()
                );

                _logger.LogWarning("Validación fallida: {@Errors}", errors);
                context.Result = new BadRequestObjectResult(response);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No se necesita implementación
        }
    }
}