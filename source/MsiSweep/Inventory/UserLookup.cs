// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using System.Collections.Concurrent;
using System.Security.Principal;

namespace MsiSweep.Inventory;

/// <summary>
/// Resolves security identifiers to account names and caches the results.
/// </summary>
public sealed class UserLookup : IUserLookup
{
    #region IUserLookup
    /// <inheritdoc/>
    public string LocalSystemSid { get; } = "S-1-5-18";

    /// <inheritdoc/>
    public string GetUserName(string? sid)
    {
        if (string.IsNullOrEmpty(sid) || sid.Equals(LocalSystemSid, StringComparison.OrdinalIgnoreCase))
        {
            return "(per-machine)";
        }

        if (_cache.TryGetValue(sid, out var userName))
        {
            return userName;
        }

        try
        {
            userName = new SecurityIdentifier(sid)
                .Translate(typeof(NTAccount))
                .Value;
        }
        catch (Exception ex) when (
            ex is ArgumentException or IdentityNotMappedException ||
            ex.GetType() == typeof(SystemException)
        )
        {
            userName = $"({sid})";
        }

        _cache[sid] = userName;

        return userName;
    }
    #endregion

    #region Implementation
    private readonly ConcurrentDictionary<string, string> _cache = new();
    #endregion
}
