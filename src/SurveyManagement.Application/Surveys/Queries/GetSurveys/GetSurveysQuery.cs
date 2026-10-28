using MediatR;

namespace SurveyManagement.Application.Surveys.Queries.GetSurveys;

public record GetSurveysQuery : IRequest<List<SurveySummaryDto>>;

public record SurveySummaryDto(
    int Id,
    string Title,
    string? Description,
    DateTime CreatedAt,
    int QuestionCount);
