using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineExam.Domain.Entities;

namespace OnlineExam.Infrastructure.Data.EntitiesConfiguration
{
    public partial class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            builder.ToTable("Subjects").HasKey(x => x.Id);

            builder.Property(x => x.Name)
                  .HasColumnType("varchar")
                  .HasMaxLength(50)
                  .IsRequired();

            builder.Property(x => x.Code)
                 .HasColumnType("varchar")
                 .HasMaxLength(50)
                 .IsRequired(false);


            builder.Property(x => x.Description)
                 .HasColumnType("varchar")
                 .HasMaxLength(250)
                 .IsRequired(false);
        }
    }
}
