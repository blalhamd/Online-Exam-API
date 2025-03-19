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

            //Register Services

            services.RegisterServices();

            // Register AutoMappper

            services.AddAutoMapper(typeof(Mapping));

            // Register RateLimiting

            services.RegisterConcurrencyRateLimitingConfig();

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

            return services;
        }


        private static IServiceCollection RegisterConcurrencyRateLimitingConfig(this IServiceCollection services)
        {
            services.AddRateLimiter(RateLimiterOptions =>
            {
                RateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                RateLimiterOptions.AddConcurrencyLimiter(RateLimiterType.Concurrency, ConcurrencyLimiterOptions =>
                {
                    ConcurrencyLimiterOptions.PermitLimit = 1000;
                    ConcurrencyLimiterOptions.QueueLimit = 100; // will go to waiting list..
                    ConcurrencyLimiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // will exist empty place to accept request, will move oldest waited request from Queue to execute..
                });
            });

            return services;
        }


    }
}
