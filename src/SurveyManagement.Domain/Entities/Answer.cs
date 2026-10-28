namespace SurveyManagement.Domain.Entities;

// The answer to one question inside a submission
public class Answer
{
    public int Id { get; set; }
    public int SurveyResponseId { get; set; }
    public int QuestionId { get; set; }
    public string? TextAnswer { get; set; }

    public SurveyResponse SurveyResponse { get; set; } = null!;
    public Question Question { get; set; } = null!;
    public List<AnswerOption> SelectedOptions { get; set; } = new();
}
