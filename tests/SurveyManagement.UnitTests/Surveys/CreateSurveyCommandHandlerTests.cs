using Microsoft.EntityFrameworkCore;
using SurveyManagement.Application.Surveys.Commands.CreateSurvey;
using SurveyManagement.Domain.Enums;
using SurveyManagement.UnitTests.Common;

namespace SurveyManagement.UnitTests.Surveys;

public class CreateSurveyCommandHandlerTests
{
    [Fact]
    public async Task Saves_survey_with_questions_and_options_in_order()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new CreateSurveyCommandHandler(context);

        var id = await handler.Handle(SurveyBuilder.ValidCommand(), CancellationToken.None);

        var survey = await context.Surveys
            .Include(s => s.Questions).ThenInclude(q => q.Options)
            .SingleAsync(s => s.Id == id);

        Assert.Equal("Customer Satisfaction Survey", survey.Title);
        Assert.Equal(3, survey.Questions.Count);

        var first = survey.Questions.Single(q => q.Order == 1);
        Assert.Equal(QuestionType.SingleSelect, first.Type);
        Assert.Equal(new[] { "Excellent", "Good", "Average", "Poor" },
            first.Options.OrderBy(o => o.Order).Select(o => o.Text));

        var text = survey.Questions.Single(q => q.Order == 3);
        Assert.Empty(text.Options);
    }
}
