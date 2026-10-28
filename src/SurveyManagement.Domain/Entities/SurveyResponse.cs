namespace SurveyManagement.Domain.Entities;

// One submission of a survey by one respondent
public class SurveyResponse
{
    public int Id { get; set; }
    public int SurveyId { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public Survey Survey { get; set; } = null!;
    public List<Answer> Answers { get; set; } = new();
}
