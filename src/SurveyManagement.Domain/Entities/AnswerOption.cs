namespace SurveyManagement.Domain.Entities;

// Join table: one row per option selected in an answer
public class AnswerOption
{
    public int AnswerId { get; set; }
    public int QuestionOptionId { get; set; }

    public Answer Answer { get; set; } = null!;
    public QuestionOption QuestionOption { get; set; } = null!;
}
