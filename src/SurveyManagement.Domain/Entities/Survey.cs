namespace SurveyManagement.Domain.Entities;

public class Survey
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Question> Questions { get; set; } = new();
    public List<SurveyResponse> Responses { get; set; } = new();
}
