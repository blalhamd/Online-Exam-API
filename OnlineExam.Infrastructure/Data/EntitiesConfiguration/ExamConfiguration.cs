using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineExam.Domain.Entities;

namespace OnlineExam.Infrastructure.Data.EntitiesConfiguration
{
    public partial class SubjectConfiguration
    {
        public class ExamConfiguration : IEntityTypeConfiguration<Exam>
        {
            public void Configure(EntityTypeBuilder<Exam> builder)
            {
                builder.ToTable("Exams").HasKey(x => x.Id);

                builder.Property(x => x.Description)
                       .HasColumnType("varchar")
                       .HasMaxLength(250)
                       .IsRequired();

                builder.Property(x => x.Duration).HasColumnType("time").IsRequired();

                builder.HasMany(x => x.ChooseQuestions)
                       .WithOne(x => x.Exam)
                       .HasForeignKey(x => x.ExamId)
                       .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}
