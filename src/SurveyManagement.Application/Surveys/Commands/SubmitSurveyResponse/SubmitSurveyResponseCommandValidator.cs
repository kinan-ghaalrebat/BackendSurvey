using FluentValidation;

namespace SurveyManagement.Application.Surveys.Commands.SubmitSurveyResponse;

// Shape-only rules. Rules that need the database live in the handler.
public class SubmitSurveyResponseCommandValidator : AbstractValidator<SubmitSurveyResponseCommand>
{
    public SubmitSurveyResponseCommandValidator()
    {
        RuleFor(x => x.Answers)
            .NotEmpty().WithMessage("At least one answer is required.")
            .Must(a => a.Select(x => x.QuestionId).Distinct().Count() == a.Count)
            .WithMessage("Each question can only be answered once.")
            .When(x => x.Answers is not null, ApplyConditionTo.CurrentValidator);

        RuleForEach(x => x.Answers).ChildRules(answer =>
        {
            answer.RuleFor(a => a.QuestionId).GreaterThan(0);

            answer.RuleFor(a => a.SelectedOptionIds)
                .Must(ids => ids!.Distinct().Count() == ids!.Count)
                .WithMessage("The same option can't be selected twice.")
                .When(a => a.SelectedOptionIds is not null);

            answer.RuleFor(a => a.TextAnswer).MaximumLength(2000);
        });
    }
}
