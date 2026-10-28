using MediatR;
using SurveyManagement.Application.Common.Interfaces;
using SurveyManagement.Domain.Entities;

namespace SurveyManagement.Application.Surveys.Commands.CreateSurvey;

public class CreateSurveyCommandHandler : IRequestHandler<CreateSurveyCommand, int>
{
    private readonly IAppDbContext _context;

    public CreateSurveyCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateSurveyCommand request, CancellationToken cancellationToken)
    {
        var survey = new Survey
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Questions = request.Questions.Select((q, qIndex) => new Question
            {
                Text = q.Text.Trim(),
                Type = q.Type,
                Order = qIndex + 1,
                Options = (q.Options ?? new List<string>()).Select((text, oIndex) => new QuestionOption
                {
                    Text = text.Trim(),
                    Order = oIndex + 1
                }).ToList()
            }).ToList()
        };

        // EF Core inserts the survey, its questions and options in one transaction
        _context.Surveys.Add(survey);
        await _context.SaveChangesAsync(cancellationToken);

        return survey.Id;
    }
}
