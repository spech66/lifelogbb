using LifelogBb.Models;
using Microsoft.EntityFrameworkCore;

namespace LifelogBb.Utilities;

/// <summary>
/// Reads the OAuth toggle from the app config row.
/// </summary>
public static class OAuthSettings
{
    /// <summary>
    /// Read only. Config.GetConfig would insert and save a row as a side effect, which must not
    /// happen on the anonymous OAuth endpoints. No row yet means no toggle, which is the correct
    /// "disabled" default.
    /// </summary>
    public static Task<bool> IsEnabledAsync(LifelogBbContext context, CancellationToken cancellationToken = default) =>
        context.Configs.AsNoTracking().Select(c => c.McpOAuthEnabled).FirstOrDefaultAsync(cancellationToken);
}
