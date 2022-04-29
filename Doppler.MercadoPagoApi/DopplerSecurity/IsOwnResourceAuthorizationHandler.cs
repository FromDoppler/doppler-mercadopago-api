
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace Doppler.HelloMicroservice.DopplerSecurity
{
    public class IsOwnResourceAuthorizationHandler : AuthorizationHandler<DopplerAuthorizationRequirement>
    {
        private readonly ILogger<IsOwnResourceAuthorizationHandler> _logger;

        public IsOwnResourceAuthorizationHandler(ILogger<IsOwnResourceAuthorizationHandler> logger)
        {
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DopplerAuthorizationRequirement requirement)
        {
            if (requirement.AllowOwnResource && IsOwnResource(context))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        private bool IsOwnResource(AuthorizationHandlerContext context)
        {
            if (!TryGetRouteData(context, out var routeData))
            {
                _logger.LogError("Is not possible access to Resource information. Type of context.Resource: {ResourceType}", context.Resource?.GetType().Name ?? "null");
                return false;
            }

            if (routeData.Values.TryGetValue("accountId", out var accountId) && accountId?.ToString() == GetTokenNameIdentifier(context.User))
            {
                // TODO: In case of using different public keys, for example Doppler and Relay,
                // it is necessary to check token Issuer information, to validate right origin.
                return true;
            }

            if (routeData.Values.TryGetValue("accountname", out var accountname) && accountname?.ToString() == GetTokenUniqueName(context.User))
            {
                // TODO: In case of using different public keys, for example Doppler and Relay,
                // it is necessary to check token Issuer information, to validate right origin.
                return true;
            }

            return false;
        }

        private static string GetTokenUniqueName(ClaimsPrincipal user) =>
            user.FindFirst(c => c.Type == ClaimTypes.Name)?.Value;

        private static string GetTokenNameIdentifier(ClaimsPrincipal user) =>
            user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        private static bool TryGetRouteData(AuthorizationHandlerContext context, out RouteData routeData)
        {
            // In my local environment with .NET 5
            if (context.Resource is HttpContext httpContext)
            {
                routeData = httpContext.GetRouteData();
                return true;
            }

            // ASP.NET Core 2?
            if (context.Resource is AuthorizationFilterContext authorizationFilterContext)
            {
                routeData = authorizationFilterContext.RouteData;
                return true;
            }

            routeData = null;
            return false;
        }
    }
}
