using Microsoft.AspNetCore.Identity;
using OnlineExam.Domain.Entities.Identity;
using OnlineExam.Infrastructure.Data.context;

namespace OnlineExam.API.Extensions
{
    public static class IdentityConfigExtension
    {
        public static IServiceCollection RegisterUserManager(this IServiceCollection services)
        {   
                 services.AddIdentity<AppUser, IdentityRole>(options =>
                 {
                     options.User.RequireUniqueEmail = true;

                     options.Lockout.AllowedForNewUsers = true;
                     options.Lockout.MaxFailedAccessAttempts = 5;
                     options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

                     options.Password.RequiredLength = 8;
                 })
                  .AddEntityFrameworkStores<AppDbContext>()
                  .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider);

            return services;
        }
    }
}
