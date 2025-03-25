namespace OnlineExam.Core.Dtos.Auth.Responses
{
    public class LoginViewModel
    {
        public string Token { get; set; } = null!;
        public DateTime? ExpireAt { get; set; }
    }
}
