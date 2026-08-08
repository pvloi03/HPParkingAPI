using Microsoft.AspNetCore.Diagnostics;

namespace HPParkingAPI.Common;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IHostEnvironment _env;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(IHostEnvironment env, ILogger<GlobalExceptionHandler> logger)
    {
        _env = env;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred");

        var response = ApiErrorResponse.Create(
            title: "Internal Server Error",
            status: StatusCodes.Status500InternalServerError,
            detail: _env.IsDevelopment()
                ? exception.Message
                : "Đã có lỗi xảy ra ở server, vui lòng thử lại sau.",
            instance: httpContext.Request.Path,
            traceId: httpContext.TraceIdentifier
        );

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}