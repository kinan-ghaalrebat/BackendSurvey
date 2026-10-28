using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SurveyManagement.Application.Common.Exceptions;
using SurveyManagement.Application.Common.Interfaces;
using SurveyManagement.Domain.Entities;
using SurveyManagement.Domain.Enums;

namespace SurveyManagement.Application.Surveys.Commands.SubmitSurveyResponse;

public class SubmitSurveyResponseCommandHandler : IRequestHandler<SubmitSurveyResponseCommand, int>
{
    private readonly IAppDbContext _context;

    public SubmitSurveyResponseCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(SubmitSurveyResponseCommand request, CancellationToken cancellationToken)
    {
        var survey = await _context.Surveys
            .AsNoTracking()
            .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(s => s.Id == request.SurveyId, cancellationToken)
            ?? throw new NotFoundException("Survey", request.SurveyId);

        var questions = survey.Questions.ToDictionary(q => q.Id);
        var errors = new List<ValidationFailure>();

        for (var i = 0; i < request.Answers.Count; i++)
        {
            var answer = request.Answers[i];
            var field = $"Answers[{i}]";
            var selected = answer.SelectedOptionIds ?? new List<int>();

            if (!questions.TryGetValue(answer.QuestionId, out var question))
            {
                errors.Add(new ValidationFailure($"{field}.QuestionId",
                    $"Question {answer.QuestionId} does not belong to survey {survey.Id}."));
                continue;
            }

            if (question.Type == QuestionType.Text)
            {
                if (selected.Count > 0)
                    errors.Add(new ValidationFailure($"{field}.SelectedOptionIds", "Text questions do not accept options."));

                if (string.IsNullOrWhiteSpace(answer.TextAnswer))
                    errors.Add(new ValidationFailure($"{field}.TextAnswer", "Text questions require a text answer."));

                continue;
            }

            // SingleSelect or MultiSelect
            if (!string.IsNullOrWhiteSpace(answer.TextAnswer))
                errors.Add(new ValidationFailure($"{field}.TextAnswer", "Select questions do not accept a text answer."));

            if (selected.Count == 0)
                errors.Add(new ValidationFailure($"{field}.SelectedOptionIds", "At least one option must be selected."));

            if (question.Type == QuestionType.SingleSelect && selected.Count > 1)
                errors.Add(new ValidationFailure($"{field}.SelectedOptionIds", "Single select questions accept only one option."));

            var validOptionIds = question.Options.Select(o => o.Id).ToHashSet();
            foreach (var optionId in selected.Where(id => !validOptionIds.Contains(id)))
                errors.Add(new ValidationFailure($"{field}.SelectedOptionIds",
                    $"Option {optionId} does not belong to question {question.Id}."));
        }

        // Same 400 response shape as the FluentValidation rules
        if (errors.Count > 0)
            throw new ValidationException(errors);

        var response = new SurveyResponse
        {
            SurveyId = survey.Id,
            Answers = request.Answers.Select(a => new Answer
            {
                QuestionId = a.QuestionId,
                TextAnswer = a.TextAnswer?.Trim(),
                SelectedOptions = (a.SelectedOptionIds ?? new List<int>())
                    .Select(optionId => new AnswerOption { QuestionOptionId = optionId })
                    .ToList()
            }).ToList()
        };

        _context.SurveyResponses.Add(response);
        await _context.SaveChangesAsync(cancellationToken);

        return response.Id;
    }
}
