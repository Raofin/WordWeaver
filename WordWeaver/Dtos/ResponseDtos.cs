using System.Net;

namespace WordWeaver.Dtos;

public class CommonResponse
{
    public string? Message { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}

public class ResponseHelper : CommonResponse
{
    public long? Id { get; set; }
}

public class ResponseHelper<T> : CommonResponse
{
    public T? Data { get; set; }
}

public class AuthResponse : CommonResponse
{
    public string? Token { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public UserDto? User { get; set; }
}