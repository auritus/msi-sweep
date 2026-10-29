// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

namespace MsiSweep.Inventory;

/// <summary>
/// Resolves security identifiers to user names.
/// </summary>
public interface IUserLookup
{
    /// <summary>
    /// Gets the SID used to represent a per-machine installation.
    /// </summary>
    string LocalSystemSid { get; }

    /// <summary>
    /// Gets the display name of the user with the specified SID.
    /// </summary>
    /// <param name="sid">The SID of the user, or <see langword="null"/> for a per-machine installation.</param>
    /// <returns>The account name of the user, or a placeholder text if the SID cannot be resolved.</returns>
    string GetUserName(string? sid);
}
