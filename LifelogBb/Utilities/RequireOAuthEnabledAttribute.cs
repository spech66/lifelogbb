using LifelogBb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LifelogBb.Utilities;

/// <summary>
/// Hides the whole OAuth surface while <see cref="Models.Entities.Config.McpOAuthEnabled"/> is off.
/// A 404 keeps a disabled instance indistinguishable from one that never had OAuth.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireOAuthEnabledAttribute : Attribute, IAsyncResourceFilter
{
    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        // Resolved per request instead of through the constructor so the attribute needs no DI registration.
        var dbContext = context.HttpContext.RequestServices.GetRequiredService<LifelogBbContext>();

        if (!await OAuthSettings.IsEnabledAsync(dbContext, context.HttpContext.RequestAborted))
        {
            context.Result = new NotFoundResult();
            return;
        }

        await next();
    }
}
