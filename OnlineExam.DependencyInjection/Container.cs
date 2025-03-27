using Microsoft.AspNetCore.Identity.UI.Services;
using OnlineExam.Business.Services.Email;
using OnlineExam.Core.IRepositories.Non_Generic;
using OnlineExam.Core.IServices.Email;
using OnlineExam.Infrastructure.Repositories.Non_Generic;

namespace OnlineExam.DependencyInjection
{
    public static class Container
    {
        public static IServiceCollection RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {

            // Register AppDbContext 

            services.RegisterConnectionString(configuration);

            // Register Unit Of Work

            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            services.AddScoped(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            services.AddScoped(typeof(IExamRepositoryAsync), typeof(ExamRepositoryAsync));
            services.AddScoped(typeof(IStudentRepositoryAsync), typeof(StudentRepositoryAsync));
            services.AddScoped(typeof(ITeacherRepositoryAsync), typeof(TeacherRepositoryAsync));

            //Register Services

            services.RegisterServices();

            // Register AutoMappper

            services.AddAutoMapper(typeof(Mapping));


            return services;
        }

        private static IServiceCollection RegisterConnectionString(this IServiceCollection services, IConfiguration configuration)
        {

            var connection = configuration["ConnectionStrings:DefaultConnectionString"];
            services.AddDbContext<AppDbContext>(x => x.UseSqlServer(connection, options =>
            {
                options.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                options.CommandTimeout(60);

            }));
            services.AddScoped<AppDbContext, AppDbContext>();
            return services;
        }

        private static IServiceCollection RegisterServices(this IServiceCollection services)
        {

            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<IEmailBodyBuilder, EmailBodyBuilder>();
            services.AddScoped<IEmailSender, EmailSender>();

            return services;
        }

    }
}
