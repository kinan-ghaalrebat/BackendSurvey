using FluentValidation;
using SurveyManagement.Domain.Enums;

namespace SurveyManagement.Application.Surveys.Commands.CreateSurvey;

public class CreateSurveyCommandValidator : AbstractValidator<CreateSurveyCommand>
{
    public CreateSurveyCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);

        RuleFor(x => x.Questions)
            .NotEmpty().WithMessage("At least one question is required.");

        RuleForEach(x => x.Questions).SetValidator(new CreateQuestionDtoValidator());
    }
}

public class CreateQuestionDtoValidator : AbstractValidator<CreateQuestionDto>
{
    public CreateQuestionDtoValidator()
    {
        RuleFor(q => q.Text).NotEmpty().MaximumLength(500);
        RuleFor(q => q.Type).IsInEnum().WithMessage("Question type must be SingleSelect, MultiSelect or Text.");

        When(q => q.Type is QuestionType.SingleSelect or QuestionType.MultiSelect, () =>
        {
            RuleFor(q => q.Options)
                .NotNull().WithMessage("Select questions must have options.")
                .Must(o => o!.Count >= 2).WithMessage("Select questions must have at least two options.")
                .When(q => q.Options is not null, ApplyConditionTo.CurrentValidator);

            RuleForEach(q => q.Options).NotEmpty().MaximumLength(200);
        });

        When(q => q.Type == QuestionType.Text, () =>
        {
            RuleFor(q => q.Options)
                .Must(o => o is null || o.Count == 0)
                .WithMessage("Text questions must not have options.");
        });
    }
}
