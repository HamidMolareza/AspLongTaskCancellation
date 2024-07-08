namespace AspLongTaskCancellation.Middlewares;

public class OperationCanceledExceptionMiddleware(RequestDelegate next) {
    public async Task InvokeAsync(HttpContext context) {
        try {
            await next(context);
        }
        catch (OperationCanceledException) {
            if (!context.Response.HasStarted) {
                context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
                await context.Response.WriteAsync("Request was cancelled.");
            }
        }
    }
}

public static class OperationCanceledExceptionExtensions {
    public static void UseOperationCanceledException(this WebApplication app) =>
        app.UseMiddleware<OperationCanceledExceptionMiddleware>();
}