using MediatR;
using SurveyManagement.Domain.Enums;

namespace SurveyManagement.Application.Surveys.Commands.CreateSurvey;

// Returns the new survey's Id
public record CreateSurveyCommand(
    string Title,
    string? Description,
    List<CreateQuestionDto> Questions) : IRequest<int>;

public record CreateQuestionDto(
    string Text,
    QuestionType Type,
    List<string>? Options);
