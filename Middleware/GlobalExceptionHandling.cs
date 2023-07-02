using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace CRUD_Migration_Logging_XunitTesting.Middleware
{
    public class GlobalExceptionHandling : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandling> _logger;

        public GlobalExceptionHandling(ILogger<GlobalExceptionHandling> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                ProblemDetails problem = new()
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Type = "Server error",
                    Title = "Server error",
                    Detail = "An internal server has occured"
                };

                string json = JsonSerializer.Serialize(problem);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            }
        }
    }
}
