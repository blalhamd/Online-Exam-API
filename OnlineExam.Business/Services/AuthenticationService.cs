using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineExam.Core.Dtos.Auth.Requests;
using OnlineExam.Core.Dtos.Auth.Responses;
using OnlineExam.Core.IServices;
using OnlineExam.Core.IServices.Provider;
using OnlineExam.Domain.Entities.Identity;

namespace OnlineExam.Business.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJwtProvider _jwtProvider;
        private readonly ILogger<AuthenticationService> _logger;
        public AuthenticationService(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<AppUser> signInManager,
            IJwtProvider jwtProvider,
            ILogger<AuthenticationService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtProvider = jwtProvider;
            _logger = logger;
        }

        public async Task<LoginViewModel> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
                throw new ArgumentException("Email cannot be empty");

            if (string.IsNullOrEmpty(request.Password))
                throw new ArgumentException("Password cannot be empty");

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials");

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, true);

            if (!result.Succeeded)
            {
                throw result.IsNotAllowed ? new BadRequest("You must confirm your email.")
                     : result.IsLockedOut ? new BadRequest("You are Locked Out")
                     : new BadRequest("Invalid Email Or Password");
            }

            (var roles, var permissions) = await GetRolesAndPermissions(user);

            (var token, var expireAt) = _jwtProvider.GenerateToken(user, roles, permissions);

            return new LoginViewModel
            {
                Token = token,
                ExpireAt = expireAt,
            };
        }

        private async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetRolesAndPermissions(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var permissions = new List<string>();

            foreach (var item in roles)
            {
                var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Name!.ToLower() == item.ToLower());

                if (role != null)
                {
                    var claimsRole = await _roleManager.GetClaimsAsync(role);

                    if (claimsRole != null)
                    {
                        foreach (var claim in claimsRole)
                        {
                            permissions.Add(claim.Value);
                        }
                    }
                }
            }

            return (roles, permissions);
        }
    }
}
