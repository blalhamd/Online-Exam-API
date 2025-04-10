using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MosefakApp.API.MiddleWares;
using OnlineExam.API.Extensions;
using OnlineExam.API.Filters.Authentication;
using OnlineExam.DependencyInjection;
using OnlineExam.Domain.Entities.Identity;
using OnlineExam.Infrastructure.Data.context;
using OnlineExam.Infrastructure.SeedData;

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

            // for permission based authorization

            builder.Services.AddTransient(typeof(IAuthorizationHandler), typeof(PermissionAuthorizationHandler));
            builder.Services.AddTransient(typeof(IAuthorizationPolicyProvider), typeof(PermissionAuthorizationPolicyProvider));

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            /*
             * ? Option 2: Set Global Fallback to JWT (Recommended for APIs)
               If you want to avoid repeating [Authorize(...)] on every controller,
               add a global fallback policy in your Program.cs:
             * 
             * 
            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            Now every controller and endpoint is secured by default unless you explicitly decorate with [AllowAnonymous].

            */

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

            app.UseMiddleware<ErrorHandlingMiddleWare>(); // Early error handling for all subsequent middleware
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors(x => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.UseMiddleware<CalculateTimeOfRequest>(); // If it captures request timing, it can go here

            app.Run();
        }
    }
}
