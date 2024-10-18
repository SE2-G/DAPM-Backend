﻿namespace UtilLibrary
{
    public class UserDto
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string OrganizationName { get; set; }

        public Guid OrganizationId { get; set; }

        public List<string> Roles { get; set; }
        public string Token { get; set; }

    }
}
