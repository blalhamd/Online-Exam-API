using Microsoft.AspNetCore.Identity;

namespace OnlineExam.Domain.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = null!;
        public string RoleType { get; set; } = null!;
    }
}
