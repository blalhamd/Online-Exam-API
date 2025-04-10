using Microsoft.AspNetCore.Mvc;
using OnlineExam.Core.Dtos.Auth.Requests;
using OnlineExam.Core.Dtos.Auth.Responses;
using OnlineExam.Core.IServices;

namespace OnlineExam.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        public async Task<LoginViewModel> LoginAsync(LoginRequest request)
        {
            return await _authenticationService.LoginAsync(request);
        }

    }
}
