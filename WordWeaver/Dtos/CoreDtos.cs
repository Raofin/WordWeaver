namespace WordWeaver.Dtos;

public class EmailDto
{
    public string To { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
    public List<string>? CcRecipients { get; set; }
    public List<string>? BccRecipients { get; set; }
}

public class DecodedJwt
{
    public string? UserId { get; set; }
    public string? UniqueName { get; set; }
    public string? Email { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public DateTime? Expiration { get; set; }
    public DateTime? NotBefore { get; set; }
    public DateTime? IssuedAt { get; set; }
    public string? JwtId { get; set; }
    public List<string>? Roles { get; set; }
}

public class UploadedFile
{
    public string Name { get; set; } = null!;
    public string Extension { get; set; } = null!;
    public string Size { get; set; } = null!;
    public DateTime? UploadedAt { get; set; }
}
