using DAPM.ClientApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

public class ActivityLogMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ActivityLogMiddleware> _logger;

    public ActivityLogMiddleware(RequestDelegate next, ILogger<ActivityLogMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        string username = "Anonymous"; // Default username
        string action = $"{context.Request.Method} {context.Request.Path}";
        string ticketId = "None"; // Default TicketId
        _logger.LogInformation("Authorization Header: {Authorization}", context.Request.Headers["Authorization"]);

        try
        {
            // Extract Authorization header
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var jwtHandler = new JwtSecurityTokenHandler();

                if (jwtHandler.CanReadToken(token))
                {
                    var jwtToken = jwtHandler.ReadJwtToken(token);

                    // Extract 'name' claim as username
                    username = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "Unknown";
                    _logger.LogInformation($"Extracted Username from Token: {username}");
                }
            }

            // Extract TicketId from query or headers
            ticketId = context.Request.Query["ticketId"].FirstOrDefault() ??
                       context.Request.Headers["ticketId"].FirstOrDefault() ??
                       "None";

            _logger.LogInformation($"Raw TicketId from Request: {ticketId}");

            //// If username is still "Anonymous", try resolving from TicketId
            //if (username == "Anonymous" && Guid.TryParse(ticketId, out var parsedTicketId))
            //{
            //    var ticketService = context.RequestServices.GetService<ITicketService>();
            //    if (ticketService != null)
            //    {
            //        username = ticketService.GetUsernameByTicket(parsedTicketId) ?? "Unknown";
            //        _logger.LogInformation($"Extracted Username from TicketId: {username}");
            //    }
            //}

            // Fallback to User.Identity.Name if still "Anonymous"
            if (username == "Anonymous" && context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                username = context.User.Identity.Name ?? "Unknown";
                _logger.LogInformation($"Extracted Username from Identity: {username}");
            }

            // Skip logging for the 'status' endpoint
            if (context.Request.Path.StartsWithSegments("/status", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context); // Skip further processing for this middleware
                return;
            }

            // Call the next middleware in the pipeline
            await _next(context);

            // Determine the result based on the HTTP response status code
            var result = context.Response.StatusCode switch
            {
                200 => "Success",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                _ => "Failed"
            };

            // Log the activity with TicketId
            var activityLogService = context.RequestServices.GetService<IActivityLogService>();
            if (activityLogService != null)
            {
                string detailedAction = $"{action})"; // Include TicketId in the action
                await activityLogService.LogUserActivity(username, detailedAction, result, DateTime.UtcNow);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in ActivityLogMiddleware: {ex.Message}");
            throw; // Re-throw the exception to avoid swallowing it
        }
    }
}
