using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineExam.Domain.Entities;

namespace OnlineExam.Infrastructure.Data.EntitiesConfiguration
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students").HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                 .HasColumnType("varchar")
                 .HasMaxLength(450)
                 .IsRequired();
        }
    }
}
