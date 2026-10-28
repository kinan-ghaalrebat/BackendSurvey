using SurveyManagement.Application.Common.Exceptions;
using SurveyManagement.Application.Surveys.Commands.CreateSurvey;
using SurveyManagement.Application.Surveys.Queries.GetSurveyById;
using SurveyManagement.UnitTests.Common;

namespace SurveyManagement.UnitTests.Surveys;

public class GetSurveyByIdQueryHandlerTests
{
    [Fact]
    public async Task Returns_survey_with_questions_and_options()
    {
        using var context = TestDbContextFactory.Create();
        var id = await new CreateSurveyCommandHandler(context).Handle(SurveyBuilder.ValidCommand(), CancellationToken.None);

        var result = await new GetSurveyByIdQueryHandler(context).Handle(new GetSurveyByIdQuery(id), CancellationToken.None);

        Assert.Equal(id, result.Id);
        Assert.Equal(3, result.Questions.Count);
        Assert.Equal(4, result.Questions[0].Options.Count);
    }

    [Fact]
    public async Task Unknown_id_throws_not_found()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new GetSurveyByIdQueryHandler(context);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new GetSurveyByIdQuery(999), CancellationToken.None));
    }
}
