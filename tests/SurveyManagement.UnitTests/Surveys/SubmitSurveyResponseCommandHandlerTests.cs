using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SurveyManagement.Application.Common.Exceptions;
using SurveyManagement.Application.Surveys.Commands.CreateSurvey;
using SurveyManagement.Application.Surveys.Commands.SubmitSurveyResponse;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Infrastructure.Persistence;
using SurveyManagement.UnitTests.Common;

namespace SurveyManagement.UnitTests.Surveys;

public class SubmitSurveyResponseCommandHandlerTests : IDisposable
{
    private readonly AppDbContext _context = TestDbContextFactory.Create();
    private readonly SubmitSurveyResponseCommandHandler _handler;

    private int _surveyId;
    private Question _single = null!;
    private Question _multi = null!;
    private Question _text = null!;

    public SubmitSurveyResponseCommandHandlerTests()
    {
        _handler = new SubmitSurveyResponseCommandHandler(_context);
        SeedSurvey().GetAwaiter().GetResult();
    }

    private async Task SeedSurvey()
    {
        _surveyId = await new CreateSurveyCommandHandler(_context)
            .Handle(SurveyBuilder.ValidCommand(), CancellationToken.None);

        var questions = await _context.Questions.Include(q => q.Options)
            .Where(q => q.SurveyId == _surveyId)
            .OrderBy(q => q.Order)
            .ToListAsync();

        (_single, _multi, _text) = (questions[0], questions[1], questions[2]);
    }

    private int OptionId(Question question, int index) => question.Options.OrderBy(o => o.Order).ElementAt(index).Id;

    private Task<int> Submit(params SubmitAnswerDto[] answers) =>
        _handler.Handle(new SubmitSurveyResponseCommand(_surveyId, answers.ToList()), CancellationToken.None);

    [Fact]
    public async Task Valid_submission_saves_one_answer_per_question_and_one_row_per_selected_option()
    {
        var responseId = await Submit(
            new SubmitAnswerDto(_single.Id, new List<int> { OptionId(_single, 1) }, null),
            new SubmitAnswerDto(_multi.Id, new List<int> { OptionId(_multi, 0), OptionId(_multi, 1) }, null),
            new SubmitAnswerDto(_text.Id, null, "The service was very good."));

        var response = await _context.SurveyResponses
            .Include(r => r.Answers).ThenInclude(a => a.SelectedOptions)
            .SingleAsync(r => r.Id == responseId);

        Assert.Equal(3, response.Answers.Count);
        Assert.Equal(2, response.Answers.Single(a => a.QuestionId == _multi.Id).SelectedOptions.Count);
        Assert.Equal("The service was very good.", response.Answers.Single(a => a.QuestionId == _text.Id).TextAnswer);
    }

    [Fact]
    public async Task Unknown_survey_throws_not_found()
    {
        var command = new SubmitSurveyResponseCommand(999,
            new List<SubmitAnswerDto> { new(_single.Id, new List<int> { OptionId(_single, 0) }, null) });

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Single_select_with_two_options_is_rejected()
    {
        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            Submit(new SubmitAnswerDto(_single.Id, new List<int> { OptionId(_single, 0), OptionId(_single, 1) }, null)));

        Assert.Contains(ex.Errors, e => e.ErrorMessage.Contains("only one option"));
    }

    [Fact]
    public async Task Option_from_another_question_is_rejected()
    {
        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            Submit(new SubmitAnswerDto(_single.Id, new List<int> { OptionId(_multi, 0) }, null)));

        Assert.Contains(ex.Errors, e => e.ErrorMessage.Contains("does not belong to question"));
    }

    [Fact]
    public async Task Question_from_another_survey_is_rejected()
    {
        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            Submit(new SubmitAnswerDto(9999, new List<int> { 1 }, null)));

        Assert.Contains(ex.Errors, e => e.ErrorMessage.Contains("does not belong to survey"));
    }

    [Fact]
    public async Task Text_question_with_options_is_rejected()
    {
        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            Submit(new SubmitAnswerDto(_text.Id, new List<int> { OptionId(_single, 0) }, "Hi")));

        Assert.Contains(ex.Errors, e => e.ErrorMessage.Contains("Text questions do not accept options"));
    }

    [Fact]
    public async Task Rejected_submission_saves_nothing()
    {
        await Assert.ThrowsAsync<ValidationException>(() =>
            Submit(new SubmitAnswerDto(_single.Id, new List<int>(), null)));

        Assert.Empty(_context.SurveyResponses);
    }

    public void Dispose() => _context.Dispose();
}
