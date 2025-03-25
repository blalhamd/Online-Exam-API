﻿using Microsoft.AspNetCore.Authorization;

namespace OnlineExam.API.Filters.Authentication
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; } = null!;

        public PermissionRequirement(string permission)
        {
            this.Permission = permission;
        }
    }
}
