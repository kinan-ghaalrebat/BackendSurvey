using Microsoft.EntityFrameworkCore;
using SurveyManagement.Domain.Entities;

namespace SurveyManagement.Application.Common.Interfaces;

// Handlers depend on this interface, not on the Infrastructure AppDbContext
public interface IAppDbContext
{
    DbSet<Survey> Surveys { get; }
    DbSet<Question> Questions { get; }
    DbSet<QuestionOption> QuestionOptions { get; }
    DbSet<SurveyResponse> SurveyResponses { get; }
    DbSet<Answer> Answers { get; }
    DbSet<AnswerOption> AnswerOptions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
