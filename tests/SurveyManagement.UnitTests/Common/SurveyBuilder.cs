using SurveyManagement.Application.Surveys.Commands.CreateSurvey;
using SurveyManagement.Domain.Enums;

namespace SurveyManagement.UnitTests.Common;

public static class SurveyBuilder
{
    // The example survey from the assessment guide
    public static CreateSurveyCommand ValidCommand() => new(
        "Customer Satisfaction Survey",
        "Please provide your feedback.",
        new List<CreateQuestionDto>
        {
            new("How satisfied are you?", QuestionType.SingleSelect, new List<string> { "Excellent", "Good", "Average", "Poor" }),
            new("Which channels did you use?", QuestionType.MultiSelect, new List<string> { "Website", "Mobile App", "Call Center" }),
            new("Additional comments", QuestionType.Text, null)
        });
}
