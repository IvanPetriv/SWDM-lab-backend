namespace Api.Dtos;

public class TestWithCorrectAnswersDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<QuestionWithCorrectAnswerDto> Questions { get; set; } = [];
}

public class QuestionWithCorrectAnswerDto
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public List<QuestionOptionWithCorrectDto> Options { get; set; } = [];
}

public class QuestionOptionWithCorrectDto
{
    public Guid Id { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int OrderIndex { get; set; }
}
