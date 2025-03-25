using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineExam.Domain.Entities;

namespace OnlineExam.Infrastructure.Data.EntitiesConfiguration
{
    public class ChooseQuestionConfiguration : IEntityTypeConfiguration<ChooseQuestion>
    {
        public void Configure(EntityTypeBuilder<ChooseQuestion> builder)
        {
            builder.ToTable("ChooseQuestions").HasKey(x => x.Id);

            builder.Property(x => x.Title)
                   .HasColumnType("varchar")
                   .HasMaxLength(250)
                   .IsRequired();

            builder.Property(x => x.GradeOfQuestion)
                   .HasColumnType("float")
                   .IsRequired();
        }
    }
}
