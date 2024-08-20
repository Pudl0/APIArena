#define AUTH_TESTING

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using APIArena.Models;
using APIArena.Middleware;
// Required only when not in debug mode:
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using APIArena.Attributes;
using System.Data;

namespace APIArena.Services.Authorization
{
    public class AuthorizationHandler(
        IServiceProvider serviceProvider

#if !DEBUG || AUTH_TESTING
        , IHttpContextAccessor httpContextAccessor
#endif

    ) : IAuthorizationHandler
    {
#if !DEBUG || AUTH_TESTING
        private const string ApiKeyHeaderName = "X-API-Key";

        public async Task HandleAsync(AuthorizationHandlerContext context)
#else
        public Task HandleAsync(AuthorizationHandlerContext context)
#endif
        {
            if (context.User != null)
            {
                using var scope = serviceProvider.CreateScope();
                var pendingRequirements = context.PendingRequirements.ToList();
                var denyAnonRequirements = new List<DenyAnonymousAuthorizationRequirement>();
                var requestHasValidRequiredApiKey = false;

                var scopeServiceProvider = scope.ServiceProvider;

                foreach (var requirement in pendingRequirements)
                {
                    if (requirement is DenyAnonymousAuthorizationRequirement noAnon)
                        if (requestHasValidRequiredApiKey || context.User.Identity?.IsAuthenticated is true)
                            context.Succeed(noAnon);
                        else
                            denyAnonRequirements.Add(noAnon);

                    else if (requirement is ApiKeyRequired apiKeyRequirement)
                    {
#if !DEBUG || AUTH_TESTING
                        var httpContext = httpContextAccessor.HttpContext;

                        string? apiKey = httpContext?.Request.Headers[ApiKeyHeaderName];
                        var apiKeyValidator = scopeServiceProvider.GetRequiredService<IApiKeyValidator>();

                        if (apiKey is not null && apiKeyValidator.IsValid(apiKey, apiKeyRequirement.Scopes, apiKeyRequirement.RequiresAllScopes))
                        {
#endif
                        context.Succeed(requirement);


                        if (!apiKeyRequirement.RequireAuthentication)
                        {
                            // succeed any future authrentification requirements related to the microsoft login
                            requestHasValidRequiredApiKey = true;

                            // retroactively succeed authentification requirements to avoid the microsoft login
                            foreach (var denyAnonRequirement in denyAnonRequirements)
                                context.Succeed(denyAnonRequirement);
                        }
#if !DEBUG || AUTH_TESTING
                        }
#endif
                    }
                }
            }

#if DEBUG && !AUTH_TESTING
            return Task.CompletedTask;
#endif
        }
    }
}
