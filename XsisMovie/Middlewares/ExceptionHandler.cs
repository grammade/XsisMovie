using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

public class ExceptionHandlingMiddleware {
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _env;
    public ExceptionHandlingMiddleware(RequestDelegate next, IWebHostEnvironment env) {
        _next = next;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context) {
        try {
            await _next(context);
        }
        catch (Exception ex) {
            Console.WriteLine($"An unhandled exception occurred: {ex}");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var errorResponse = new ErrorResponse {
                Message = "An error occurred",
                ErrorCode = "500"
            };

            // Include the stack trace only in the development environment
            if (_env.IsDevelopment()) {
                errorResponse.AdditionalInfo = new Dictionary<string, string>
                {
                    { "ExceptionType", ex.GetType().FullName },
                    { "StackTrace", ex.StackTrace }
                };
            }

            context.Response.ContentType = "application/json";
            var json = JsonConvert.SerializeObject( errorResponse );
            await context.Response.WriteAsync(json);
        }
    }
    public class ErrorResponse {
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public Dictionary<string, string> AdditionalInfo { get; set; }
    }
}
