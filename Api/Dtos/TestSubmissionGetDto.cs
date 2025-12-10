namespace Api.Dtos;

public class TestSubmissionGetDto
{
    public Guid Id { get; set; }
    public Guid TestId { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public double Grade { get; set; }
    public List<SubmissionAnswerGetDto> Answers { get; set; } = [];
}

public class SubmissionAnswerGetDto
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public Guid SelectedOptionId { get; set; }
    public string SelectedOptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
