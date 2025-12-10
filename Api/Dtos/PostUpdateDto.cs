using System.ComponentModel.DataAnnotations;

namespace Api.Dtos;

public class PostUpdateDto
{
    [StringLength(200, MinimumLength = 1)]
    public string? Title { get; set; }

    [StringLength(10000, MinimumLength = 1)]
    public string? TextContent { get; set; }

    public IFormFile? Image { get; set; }

    public bool? RemoveImage { get; set; }
}
