using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace LogiTransPro.API.Filters
{
    public class LoggingActionFilter : IActionFilter
    {
        private readonly ILogger<LoggingActionFilter> _logger;

        public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            context.HttpContext.Items["Stopwatch"] = stopwatch;

            var controller = context.Controller.GetType().Name;
            var action = context.ActionDescriptor.RouteValues["action"];

            _logger.LogInformation("Iniciando {Controller}.{Action}", controller, action);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.Items["Stopwatch"] is Stopwatch stopwatch)
            {
                stopwatch.Stop();
                var controller = context.Controller.GetType().Name;
                var action = context.ActionDescriptor.RouteValues["action"];
                var duration = stopwatch.ElapsedMilliseconds;

                _logger.LogInformation("Finalizando {Controller}.{Action} - Duration: {Duration}ms",
                    controller, action, duration);
            }
        }
    }
}