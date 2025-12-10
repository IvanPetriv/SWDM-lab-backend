using System.ComponentModel.DataAnnotations;

namespace Api.Dtos;

public class TestSubmissionCreateDto
{
    [Required]
    public Guid TestId { get; set; }

    [Required]
    [MinLength(1)]
    public List<AnswerDto> Answers { get; set; } = [];
}

public class AnswerDto
{
    [Required]
    public Guid QuestionId { get; set; }

    [Required]
    public Guid SelectedOptionId { get; set; }
}
