namespace OnlineExam.Core.Dtos.Auth.Responses
{
    public class LoginResponse
    {
        public string Token { get; set; } = null!;
        public DateTime? ExpireAt { get; set; }
    }
}
