namespace OnlineExam.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting(RateLimiterType.Concurrency)]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public async Task<ActionResult<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var query = await _authenticationService.LoginAsync(request);
           
            return Ok(query);
        }

    }
}
