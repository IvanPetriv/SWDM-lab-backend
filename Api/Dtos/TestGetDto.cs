namespace Api.Dtos;

public class TestGetDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<QuestionGetDto> Questions { get; set; } = [];
}

public class QuestionGetDto
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public List<QuestionOptionGetDto> Options { get; set; } = [];
}

public class QuestionOptionGetDto
{
    public Guid Id { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    // IsCorrect is intentionally excluded for students
}
