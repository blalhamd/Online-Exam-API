namespace OnlineExam.Core.IServices.Provider
{
    public interface IJwtProvider
    {
        (string token, DateTime? expireAt) GenerateToken(AppUser applicationUser, IEnumerable<string> roles);
    }
}
