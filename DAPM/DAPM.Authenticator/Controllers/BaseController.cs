﻿using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UtilLibrary;

namespace DAPM.Authenticator.Controllers
{

    [ApiController]
    [EnableCors("AllowAllOrigins")]
    public class BaseController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public BaseController(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }
        public List<string> userRoles => _contextAccessor.HttpContext
                                                            .User
                                                            .Claims
                                                            .Where(c => c.Type == ClaimTypes.Role)?.Select(x => x.Value).ToList();

        public string userId => _contextAccessor.HttpContext
                                                    .User
                                                    .Claims
                                                    .First(c => c.Type == CustomTokenTypeConstants.Id)?.Value;
        public string Organization => _contextAccessor.HttpContext
                                                    .User
                                                    .Claims
                                                    .First(c => c.Type == CustomTokenTypeConstants.OrganisationName)?.Value;
        public string OrganizationID => _contextAccessor.HttpContext
                                            .User
                                            .Claims
                                            .First(c => c.Type == CustomTokenTypeConstants.OrganisationId)?.Value;
    }
}