using Microsoft.AspNetCore.Authorization;

namespace OnlineExam.API.Filters.Authentication
{
    public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission)
    {

    }
}
