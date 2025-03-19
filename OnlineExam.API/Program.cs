namespace OnlineExam.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
                 
            builder.Services.RegisterConfiguration(builder.Configuration);
            builder.Services.AddAuthentication(builder.Configuration);
            builder.Services.RegisterFluentValidationSettings();
            builder.Services.RegisterUserManager();
            builder.Services.OptionsPatternConfig(builder.Configuration);


            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Seed data after app is built
            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await Seed.Initialize(context, userManager, roleManager);
            }


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseCors(x => x.AllowAnyHeader().AllowCredentials().AllowAnyMethod());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRateLimiter();

            app.MapControllers();

            app.UseMiddleware<ErrorHandlingMiddleWare>();
            app.UseMiddleware<CalculateTimeOfRequest>();


            app.Run();
        }
    }
}
