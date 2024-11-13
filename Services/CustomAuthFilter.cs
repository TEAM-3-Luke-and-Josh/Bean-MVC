// Add this class to your project
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class CustomAuthorizeFilter : IAuthorizationFilter
{
    private readonly ILogger<CustomAuthorizeFilter> _logger;

    public CustomAuthorizeFilter(ILogger<CustomAuthorizeFilter> logger)
    {
        _logger = logger;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        _logger.LogInformation("Authorization check for: {Path}", context.HttpContext.Request.Path);
        
        if (!context.HttpContext.User.Identity!.IsAuthenticated)
        {
            _logger.LogWarning("User is not authenticated");
            context.Result = new UnauthorizedResult();
            return;
        }

        var endpoint = context.HttpContext.GetEndpoint();
        var authorizeData = endpoint?.Metadata.GetOrderedMetadata<IAuthorizeData>();
        
        if (authorizeData == null || !authorizeData.Any())
        {
            return;
        }

        foreach (var data in authorizeData)
        {
            if (!string.IsNullOrEmpty(data.Roles))
            {
                var roles = data.Roles.Split(',');
                if (!roles.Any(role => context.HttpContext.User.IsInRole(role.Trim())))
                {
                    _logger.LogWarning("User does not have required role");
                    context.Result = new ForbidResult();
                    return;
                }
            }
        }
    }
}