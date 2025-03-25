using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineExam.Domain.Entities;

namespace OnlineExam.Infrastructure.Data.EntitiesConfiguration
{
    public partial class SubjectConfiguration
    {
        public class ChoiceConfiguration : IEntityTypeConfiguration<Choice>
        {
            public void Configure(EntityTypeBuilder<Choice> builder)
            {
                builder.ToTable("Choices").HasKey(x => x.Id);

                builder.Property(x => x.Text)
                       .HasColumnType("varchar")
                       .HasMaxLength(250)
                       .IsRequired();
            }
        }
    }
}
