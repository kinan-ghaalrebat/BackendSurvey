using MediatR;
using Microsoft.EntityFrameworkCore;
using SurveyManagement.Application.Common.Interfaces;

namespace SurveyManagement.Application.Surveys.Queries.GetSurveys;

public class GetSurveysQueryHandler : IRequestHandler<GetSurveysQuery, List<SurveySummaryDto>>
{
    private readonly IAppDbContext _context;

    public GetSurveysQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SurveySummaryDto>> Handle(GetSurveysQuery request, CancellationToken cancellationToken)
    {
        // Select straight into the DTO so SQL only fetches the columns we need
        return await _context.Surveys
            .AsNoTracking()
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SurveySummaryDto(
                s.Id,
                s.Title,
                s.Description,
                s.CreatedAt,
                s.Questions.Count))
            .ToListAsync(cancellationToken);
    }
}
