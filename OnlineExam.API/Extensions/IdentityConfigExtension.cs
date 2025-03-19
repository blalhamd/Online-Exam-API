namespace OnlineExam.API.Extensions
{
    public static class IdentityConfigExtension
    {
        public static IServiceCollection RegisterUserManager(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

            return services;
        }
    }
}
