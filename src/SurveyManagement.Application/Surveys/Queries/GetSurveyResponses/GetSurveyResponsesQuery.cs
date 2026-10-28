using MediatR;
using SurveyManagement.Domain.Enums;

namespace SurveyManagement.Application.Surveys.Queries.GetSurveyResponses;

public record GetSurveyResponsesQuery(int SurveyId) : IRequest<List<SurveyResponseDto>>;

// One submission, with its answers grouped under it
public record SurveyResponseDto(
    int Id,
    DateTime SubmittedAt,
    List<AnswerDto> Answers);

public record AnswerDto(
    int QuestionId,
    string QuestionText,
    QuestionType QuestionType,
    string? TextAnswer,
    List<SelectedOptionDto> SelectedOptions);

public record SelectedOptionDto(int Id, string Text);
