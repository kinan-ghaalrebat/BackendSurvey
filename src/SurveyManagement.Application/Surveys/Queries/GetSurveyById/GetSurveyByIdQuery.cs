using MediatR;
using SurveyManagement.Domain.Enums;

namespace SurveyManagement.Application.Surveys.Queries.GetSurveyById;

public record GetSurveyByIdQuery(int Id) : IRequest<SurveyDetailDto>;

public record SurveyDetailDto(
    int Id,
    string Title,
    string? Description,
    DateTime CreatedAt,
    List<QuestionDto> Questions);

public record QuestionDto(
    int Id,
    string Text,
    QuestionType Type,
    List<OptionDto> Options);

public record OptionDto(int Id, string Text);
