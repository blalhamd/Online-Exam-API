namespace OnlineExam.Business.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJwtProvider _jwtProvider;
        public AuthenticationService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IJwtProvider jwtProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtProvider = jwtProvider;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
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

            var roles = await _userManager.GetRolesAsync(user);

            (var token, var expireAt) = _jwtProvider.GenerateToken(user, roles);

            return new LoginResponse
            {
                Token = token,
                ExpireAt = expireAt,
            };
        }
    }
}
