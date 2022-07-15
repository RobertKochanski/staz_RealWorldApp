using Newtonsoft.Json;
using RealWorldApp.Commons.Exceptions;
using System.Net;

namespace RealWebAppAPI
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var exceptionType = exception.GetType();
            var statusCode = HttpStatusCode.InternalServerError;
            var status = string.Empty;
            var payload = string.Empty;

            switch (exception)
            {
                case NotFoundException e:
                    statusCode = HttpStatusCode.NotFound;
                    status = "NOT_FOUND";
                    context.Response.StatusCode = (int)statusCode;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync(payload);

                case BadRequestException e:
                    statusCode = HttpStatusCode.BadRequest;
                    status = "BAD_REQUEST";
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    context.Response.StatusCode = (int)statusCode;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync(exception.Message);
            }

            var response = new
            {
                error = exception.Data["error"],
                status,
                timeStamp = DateTime.Now,
                message = exception.Source + " error",
            };

            payload = JsonConvert.SerializeObject(response);
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsync(payload);
        }
    }
}
