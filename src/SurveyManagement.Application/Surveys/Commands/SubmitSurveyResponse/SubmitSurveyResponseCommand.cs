using MediatR;

namespace SurveyManagement.Application.Surveys.Commands.SubmitSurveyResponse;

// Returns the new response's Id
public record SubmitSurveyResponseCommand(
    int SurveyId,
    List<SubmitAnswerDto> Answers) : IRequest<int>;

// Request body for POST /api/surveys/{id}/submit (the survey id comes from the route)
public record SubmitSurveyResponseRequest(List<SubmitAnswerDto> Answers);

public record SubmitAnswerDto(
    int QuestionId,
    List<int>? SelectedOptionIds,
    string? TextAnswer);
