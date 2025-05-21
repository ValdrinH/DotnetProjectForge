using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace DotnetProjectForge.API.Middleware
{
    public class StandardResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public StandardResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);

                // Reset stream position
                responseBody.Seek(0, SeekOrigin.Begin);

                // Check for FileResult - skip wrapping
                if (context.Items.TryGetValue("__ActionResult", out var result) && result is FileResult)
                {
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                    return;
                }

                // Skip wrapping for 204 No Content
                if (context.Response.StatusCode == StatusCodes.Status204NoContent)
                {
                    return;
                }

                var responseContent = await new StreamReader(responseBody).ReadToEndAsync();

                var standardResponse = new
                {
                    success = true,
                    data = string.IsNullOrWhiteSpace(responseContent) ? null : JsonSerializer.Deserialize<object>(responseContent),
                    message = (string)null
                };

                var jsonResponse = JsonSerializer.Serialize(standardResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                context.Response.ContentType = "application/json";
                context.Response.ContentLength = jsonResponse.Length;

                context.Response.Body = originalBodyStream;
                await context.Response.WriteAsync(jsonResponse);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, originalBodyStream);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, Stream originalBodyStream)
        {
            var standardResponse = new
            {
                success = false,
                data = (object)null,
                message = exception.Message
            };

            var jsonResponse = JsonSerializer.Serialize(standardResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentLength = jsonResponse.Length;

            context.Response.Body = originalBodyStream;
            await context.Response.WriteAsync(jsonResponse);
        }
    }

    public static class StandardResponseMiddlewareExtensions
    {
        public static IApplicationBuilder UseStandardResponse(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<StandardResponseMiddleware>();
        }
    }
}
