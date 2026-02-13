namespace MoneyMood.Exceptions;

public class ApiException(int statusCode, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;

    public static ApiException Conflict(string message) => new(409, message);
    public static ApiException NotFound(string message) => new(404, message);
    public static ApiException Unauthorized(string message) => new(401, message);
}