using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineExam.Domain.Entities;

namespace OnlineExam.Infrastructure.Data.EntitiesConfiguration
{
    public partial class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.ToTable("Teachers").HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                  .HasColumnType("varchar")
                  .HasMaxLength(450)
                  .IsRequired();
        }
    }
}
