using OnlineExam.Core.Dtos.Auth.Requests;
using OnlineExam.Core.Dtos.Auth.Responses;

namespace OnlineExam.Core.IServices
{
    public interface IAuthenticationService
    {
        Task<LoginViewModel> LoginAsync(LoginRequest request);
    }
}
