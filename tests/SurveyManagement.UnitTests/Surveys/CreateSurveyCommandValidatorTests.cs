using SurveyManagement.Application.Surveys.Commands.CreateSurvey;
using SurveyManagement.Domain.Enums;
using SurveyManagement.UnitTests.Common;

namespace SurveyManagement.UnitTests.Surveys;

public class CreateSurveyCommandValidatorTests
{
    private readonly CreateSurveyCommandValidator _validator = new();

    [Fact]
    public void Valid_survey_passes()
    {
        var result = _validator.Validate(SurveyBuilder.ValidCommand());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Empty_title_fails()
    {
        var command = SurveyBuilder.ValidCommand() with { Title = "" };

        var result = _validator.Validate(command);

        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }

    [Fact]
    public void No_questions_fails()
    {
        var command = SurveyBuilder.ValidCommand() with { Questions = new List<CreateQuestionDto>() };

        var result = _validator.Validate(command);

        Assert.Contains(result.Errors, e => e.PropertyName == "Questions");
    }

    [Theory]
    [InlineData(QuestionType.SingleSelect)]
    [InlineData(QuestionType.MultiSelect)]
    public void Select_question_with_one_option_fails(QuestionType type)
    {
        var command = SurveyBuilder.ValidCommand() with
        {
            Questions = new List<CreateQuestionDto> { new("Pick one", type, new List<string> { "Only option" }) }
        };

        var result = _validator.Validate(command);

        Assert.Contains(result.Errors, e => e.PropertyName == "Questions[0].Options");
    }

    [Fact]
    public void Text_question_with_options_fails()
    {
        var command = SurveyBuilder.ValidCommand() with
        {
            Questions = new List<CreateQuestionDto> { new("Comments", QuestionType.Text, new List<string> { "A", "B" }) }
        };

        var result = _validator.Validate(command);

        Assert.Contains(result.Errors, e => e.PropertyName == "Questions[0].Options");
    }

    [Fact]
    public void Invalid_question_type_fails()
    {
        var command = SurveyBuilder.ValidCommand() with
        {
            Questions = new List<CreateQuestionDto> { new("Bad type", (QuestionType)99, null) }
        };

        var result = _validator.Validate(command);

        Assert.Contains(result.Errors, e => e.PropertyName == "Questions[0].Type");
    }
}
