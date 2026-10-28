using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyManagement.Domain.Entities;

namespace SurveyManagement.Infrastructure.Persistence.Configurations;

public class AnswerOptionConfiguration : IEntityTypeConfiguration<AnswerOption>
{
    public void Configure(EntityTypeBuilder<AnswerOption> builder)
    {
        // Composite key: the same option can't be selected twice in one answer
        builder.HasKey(ao => new { ao.AnswerId, ao.QuestionOptionId });

        builder.HasOne(ao => ao.Answer)
            .WithMany(a => a.SelectedOptions)
            .HasForeignKey(ao => ao.AnswerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ao => ao.QuestionOption)
            .WithMany()
            .HasForeignKey(ao => ao.QuestionOptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
