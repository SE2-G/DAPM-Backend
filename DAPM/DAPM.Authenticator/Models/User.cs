﻿// Author: s205135

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DAPM.Authenticator.Models
{
    public class User : IdentityUser<int>
    {
        public ICollection<UserRole> UserRoles { get; set; }
        public string FullName { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; } = "No affiliation";
    }
}
