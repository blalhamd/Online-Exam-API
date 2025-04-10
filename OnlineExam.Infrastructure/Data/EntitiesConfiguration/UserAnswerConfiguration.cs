using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineExam.Domain.Entities;

namespace OnlineExam.Infrastructure.Data.EntitiesConfiguration
{
    public partial class UserAnswerConfiguration : IEntityTypeConfiguration<UserAnswer>
    {
        public void Configure(EntityTypeBuilder<UserAnswer> builder)
        {
            builder.ToTable("UserAnswers").HasKey(x => x.Id);

            builder.HasOne(x => x.ChooseQuestion)
                   .WithMany(x => x.UserAnswers)
                   .HasForeignKey(x => x.ChooseQuestionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.SelectedChoice)
                   .WithMany(x => x.UserAnswers)
                   .HasForeignKey(x => x.SelectedChoiceId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
