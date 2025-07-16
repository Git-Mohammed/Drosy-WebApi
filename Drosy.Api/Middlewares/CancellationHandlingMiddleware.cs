using Drosy.Application.Interfaces.Common;
namespace Drosy.Api.Middlewares
{
    public class CancellationHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Application.Interfaces.Common.ILogger<CancellationHandlingMiddleware> _logger;

        public CancellationHandlingMiddleware(RequestDelegate next, Application.Interfaces.Common.ILogger<CancellationHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); 
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex.Message);
                context.Response.StatusCode = 499;
                await context.Response.WriteAsync("Reqeust canceld by the user");
            }
        }
    }

}
