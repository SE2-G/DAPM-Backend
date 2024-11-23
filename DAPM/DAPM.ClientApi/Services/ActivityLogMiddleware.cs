using DAPM.ClientApi.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

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
        string username = "Anonymous"; // Default if no username is found
        string action = $"{context.Request.Method} {context.Request.Path}";

        try
        {
            // Extract Authorization header
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();

                // Decode JWT token
                var jwtHandler = new JwtSecurityTokenHandler();
                if (jwtHandler.CanReadToken(token))
                {
                    var jwtToken = jwtHandler.ReadJwtToken(token);
                    var payload = jwtToken.Payload;

                    // Extract the username claim (replace "name" with the actual claim key)
                    if (payload.TryGetValue("name", out var name))
                    {
                        username = name?.ToString() ?? "Unknown";
                    }
                }
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

            // Log the activity
            var activityLogService = context.RequestServices.GetService<IActivityLogService>();
            await activityLogService?.LogUserActivity(username, action, result, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in ActivityLogMiddleware: {ex.Message}");
            throw; // Re-throw the exception to avoid swallowing it
        }
    }
}
