
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace FlowerShop.Utility
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Allow request continue to Controller/Service
                await _next(context);
            }
            catch (Exception ex)
            {
                // Catch every exception and handle it
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // default Server (500)
            var response = new ApiResult<string>("Internal Server Error");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            switch (exception)
            {
                case NotFoundException ex:
                    // Error 404
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = ex.Message;
                    break;

                case ValidationException ex:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Validation Failed";
                    response.Errors = ex.Errors
                        .SelectMany(x => x.Value.Select(err => $"{x.Key}: {err}"))
                        .ToList();
                    break;

                case BadRequestException ex:
                    // Error 400
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = ex.Message;
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = "An unexpected error occurred.";
                    _logger.LogError(exception, "Unhandled Exception");
                    break;
            }

            // Object to JSON
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }
    }
}
