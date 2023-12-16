namespace WordWeaver.Dtos;

public class PostPdfDto
{
    public long PostId { get; set; }

    public string? Username { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Text { get; set; }

    public DateTime? PublishedOn { get; set; }

    public List<long>? FileIds { get; set; }
}
