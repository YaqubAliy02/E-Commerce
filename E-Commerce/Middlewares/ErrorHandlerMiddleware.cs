using Serilog; // Replace with your logging library if different
using System.Net;
using System.Text.Json;

namespace E_Commerce.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            // Log the exception with additional context
            Log.Error(ex, "An error occurred processing request for {path} by user {user}", context.Request.Path, context.User?.Identity?.Name);

            var response = context.Response;
            response.ContentType = "application/json";

            int statusCode;
            string message;

            if (ex is ArgumentException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = "Invalid request parameters.";
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = "An unexpected error occurred.";
            }

            var result = JsonSerializer.Serialize(new
            {
                Message = message,
                Details = ex.Message // Consider filtering sensitive details here
            });

            await response.WriteAsync(result);
            response.StatusCode = statusCode;
        }
    }
}