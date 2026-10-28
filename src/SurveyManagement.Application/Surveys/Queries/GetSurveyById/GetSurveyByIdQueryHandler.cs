using MediatR;
using Microsoft.EntityFrameworkCore;
using SurveyManagement.Application.Common.Exceptions;
using SurveyManagement.Application.Common.Interfaces;

namespace SurveyManagement.Application.Surveys.Queries.GetSurveyById;

public class GetSurveyByIdQueryHandler : IRequestHandler<GetSurveyByIdQuery, SurveyDetailDto>
{
    private readonly IAppDbContext _context;

    public GetSurveyByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<SurveyDetailDto> Handle(GetSurveyByIdQuery request, CancellationToken cancellationToken)
    {
        var survey = await _context.Surveys
            .AsNoTracking()
            .Where(s => s.Id == request.Id)
            .Select(s => new SurveyDetailDto(
                s.Id,
                s.Title,
                s.Description,
                s.CreatedAt,
                s.Questions
                    .OrderBy(q => q.Order)
                    .Select(q => new QuestionDto(
                        q.Id,
                        q.Text,
                        q.Type,
                        q.Options
                            .OrderBy(o => o.Order)
                            .Select(o => new OptionDto(o.Id, o.Text))
                            .ToList()))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);

        return survey ?? throw new NotFoundException("Survey", request.Id);
    }
}
