using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WordWeaver.Helper;

namespace WordWeaver.Dtos;

public class PostDto
{
    public long PostId { get; set; }

    public long UserId { get; set; }

    [MinLength(10)]
    public string Title { get; set; } = null!;

    [MinLength(100)]
    public string Description { get; set; } = null!;

    [MinLength(200)]
    public string Text { get; set; } = null!;

    public List<long>? FileIds { get; set; }

    public bool IsPublished { get; set; }
}

public class PostPreviewDto
{
    public long PostId { get; set; }

    public long? UserId { get; set; }

    [MinLength(10)]
    public string Title { get; set; } = null!;

    [MinLength(100)]
    public string Description { get; set; } = null!;
}

public class UpdatePostDto
{
    [Range(1, long.MaxValue, ErrorMessage = "PostId is required.")]
    public long PostId { get; set; }

    [MinLength(10)]
    public string? Title { get; set; }

    [MinLength(100)]
    public string? Description { get; set; }

    [MinLength(200)]
    public string? Text { get; set; }

    public List<long>? FileIds { get; set; }
    
    public bool? IsPublished { get; set; }
    
    public bool? IsActive { get; set; }
}

public class ReactDto
{
    public long ReactId { get; set; }

    public long? UserId { get; set; }

    public long? BlogId { get; set; }

    public long? CommentId { get; set; }

    public ReactTypes ReactEnumId { get; set; }

    public bool? IsActive { get; set; }
}
