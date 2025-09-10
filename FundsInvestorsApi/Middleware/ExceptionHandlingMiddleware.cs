using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace FundsInvestorsApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "An unexpected error occurred!",
                Detail = exception.Message
            };

            var result = JsonSerializer.Serialize(problemDetails);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = problemDetails.Status.Value;

            return context.Response.WriteAsync(result);
        }
    }
}
