using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineExam.Domain.Entities.Identity;

namespace OnlineExam.Infrastructure.Data.EntitiesConfiguration
{
    public partial class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.ToTable("Users", schema: "Security").HasKey(x => x.Id);

            builder.Property(x => x.FullName)
                  .HasColumnType("varchar")
                  .HasMaxLength(50)
                  .IsRequired();

            builder.Property(x => x.RoleType)
                 .HasColumnType("varchar")
                 .HasMaxLength(50)
                 .IsRequired();

            builder.Property(x => x.PasswordHash)
                 .HasColumnType("varchar")
                 .HasMaxLength(250)
                 .IsRequired();

            builder.Property(x => x.SecurityStamp)
                 .HasColumnType("varchar")
                 .HasMaxLength(250)
                 .IsRequired();

            builder.Property(x => x.ConcurrencyStamp)
                 .HasColumnType("varchar")
                 .HasMaxLength(250)
                 .IsRequired();

            builder.Property(x => x.PhoneNumber)
                .HasColumnType("varchar")
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}

