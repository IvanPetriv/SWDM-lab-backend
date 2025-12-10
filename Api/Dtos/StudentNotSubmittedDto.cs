namespace Api.Dtos;

public class StudentNotSubmittedDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
}
