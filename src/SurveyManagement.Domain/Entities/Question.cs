using SurveyManagement.Domain.Enums;

namespace SurveyManagement.Domain.Entities;

public class Question
{
    public int Id { get; set; }
    public int SurveyId { get; set; }
    public string Text { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public int Order { get; set; }

    public Survey Survey { get; set; } = null!;
    public List<QuestionOption> Options { get; set; } = new();
}
