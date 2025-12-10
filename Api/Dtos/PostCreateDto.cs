using System.ComponentModel.DataAnnotations;

namespace Api.Dtos;

public class PostCreateDto
{
    [Required]
    public Guid CourseId { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(10000, MinimumLength = 1)]
    public string TextContent { get; set; } = string.Empty;

    public IFormFile? Image { get; set; }
}
