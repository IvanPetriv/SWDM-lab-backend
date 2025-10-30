namespace Api.Dtos;
public class CourseGetDto {
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";

    public Guid? TeacherId { get; set; }
    public int Code { get; set; }
}
