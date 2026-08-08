using Microsoft.AspNetCore.Mvc;

namespace HPParkingAPI.Common;

public class ApiErrorResponse : ProblemDetails
{
    public string? TraceId { get; set; }
    public IDictionary<string, string[]>? Errors { get; set; }

    public static ApiErrorResponse Create(
        string title,
        int status,
        string detail,
        string instance,
        string traceId,
        IDictionary<string, string[]>? errors = null)
    {
        return new ApiErrorResponse
        {
            Type = $"https://tools.ietf.org/html/rfc9110#section-{GetRfcSection(status)}",
            Title = title,
            Status = status,
            Detail = detail,
            Instance = instance,
            TraceId = traceId,
            Errors = errors
        };
    }

    private static string GetRfcSection(int status) => status switch
    {
        400 => "15.5.1",
        500 => "15.6.1",
        _ => "15"
    };
}