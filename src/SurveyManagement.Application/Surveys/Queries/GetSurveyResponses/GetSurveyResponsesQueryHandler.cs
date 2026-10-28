using MediatR;
using Microsoft.EntityFrameworkCore;
using SurveyManagement.Application.Common.Exceptions;
using SurveyManagement.Application.Common.Interfaces;

namespace SurveyManagement.Application.Surveys.Queries.GetSurveyResponses;

public class GetSurveyResponsesQueryHandler : IRequestHandler<GetSurveyResponsesQuery, List<SurveyResponseDto>>
{
    private readonly IAppDbContext _context;

    public GetSurveyResponsesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SurveyResponseDto>> Handle(GetSurveyResponsesQuery request, CancellationToken cancellationToken)
    {
        var surveyExists = await _context.Surveys.AnyAsync(s => s.Id == request.SurveyId, cancellationToken);
        if (!surveyExists)
            throw new NotFoundException("Survey", request.SurveyId);

        return await _context.SurveyResponses
            .AsNoTracking()
            .Where(r => r.SurveyId == request.SurveyId)
            .OrderByDescending(r => r.SubmittedAt)
            .Select(r => new SurveyResponseDto(
                r.Id,
                r.SubmittedAt,
                r.Answers
                    .OrderBy(a => a.Question.Order)
                    .Select(a => new AnswerDto(
                        a.QuestionId,
                        a.Question.Text,
                        a.Question.Type,
                        a.TextAnswer,
                        a.SelectedOptions
                            .OrderBy(ao => ao.QuestionOption.Order)
                            .Select(ao => new SelectedOptionDto(ao.QuestionOptionId, ao.QuestionOption.Text))
                            .ToList()))
                    .ToList()))
            .ToListAsync(cancellationToken);
    }
}
