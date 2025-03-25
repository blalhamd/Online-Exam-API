using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OnlineExam.Core.Constants;
using System.Text;

namespace OnlineExam.API.Extensions
{
    public static class AuthExtension
    {
        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var JwtOptions = configuration.GetSection("Jwt").Get<JwtSetting>();

            services.AddSingleton(JwtOptions!);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).
            AddJwtBearer(options =>
            {
                options.SaveToken = true;

                // here will validate parameters of Token.
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = JwtOptions!.Issuer,
                    ValidateAudience = true,
                    ValidAudience = JwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.Key))
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Bearer", policy => policy.RequireAuthenticatedUser());
            });

            return services;
        }
    }
}
