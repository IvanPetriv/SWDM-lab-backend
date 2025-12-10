using System.ComponentModel.DataAnnotations;

namespace Api.Dtos;

public class TestCreateDto
{
    [Required]
    public Guid CourseId { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    [MinLength(1)]
    public List<QuestionCreateDto> Questions { get; set; } = [];
}

public class QuestionCreateDto
{
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string QuestionText { get; set; } = string.Empty;

    [Required]
    [MinLength(2)]
    public List<QuestionOptionCreateDto> Options { get; set; } = [];
}

public class QuestionOptionCreateDto
{
    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string OptionText { get; set; } = string.Empty;

    [Required]
    public bool IsCorrect { get; set; }
}
