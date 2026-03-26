using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using LogiTransPro.API.Models.ViewModels;

namespace LogiTransPro.API.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> _logger;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var response = new ApiResponse<object>();

            _logger.LogError(exception, "Error no controlado: {Message}", exception.Message);

            switch (exception)
            {
                case KeyNotFoundException _:
                    response = ApiResponse<object>.Error("Recurso no encontrado");
                    context.Result = new NotFoundObjectResult(response);
                    break;

                case InvalidOperationException _:
                    response = ApiResponse<object>.Error(exception.Message);
                    context.Result = new BadRequestObjectResult(response);
                    break;

                case UnauthorizedAccessException _:
                    response = ApiResponse<object>.Error("No autorizado");
                    context.Result = new UnauthorizedObjectResult(response);
                    break;

                case ArgumentException _:
                    response = ApiResponse<object>.Error(exception.Message);
                    context.Result = new BadRequestObjectResult(response);
                    break;

                default:
                    response = ApiResponse<object>.Error("Ocurrió un error interno en el servidor");
                    context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                    break;
            }

            context.ExceptionHandled = true;
        }
    }
}