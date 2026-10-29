// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using System.Buffers;

namespace MsiSweep.Extensions;

/// <summary>
/// Extension methods for the <see cref="Guid"/> type.
/// </summary>
internal static class GuidExtensions
{
    /// <summary>
    /// Gets the length of a GUID in the packed form used within the registry.
    /// </summary>
    public const int PackedLength = 32;

    extension(Guid guid)
    {
        /// <summary>
        /// Gets a string representing the <see cref="Guid"/> in the form expected
        /// by the MSI API, which is <c>{XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX}</c>.
        /// </summary>
        /// <returns>A string representation of the <see cref="Guid"/>.</returns>
        public string ToApiString() => guid.ToString("B").ToUpperInvariant();

        /// <summary>
        /// Converts the <see cref="Guid"/> into the packed form used within the registry. The <paramref name="target"/>
        /// must provide space for 32 characters.
        /// </summary>
        /// <param name="target">The buffer to copy the packed string form of the <see cref="Guid"/> to.</param>
        /// <returns>
        /// <see langword="true"/>, if the conversion succeeded. Otherwise, <see langword="false"/>.
        /// </returns>
        public bool TryWritePackedString(Span<char> target)
        {
            Span<byte> bytes = stackalloc byte[16];

            guid.TryWriteBytes(bytes);
            SwapNibbles(bytes);

            return Convert.TryToHexString(bytes, target, out _);
        }

        /// <summary>
        /// Converts the <see cref="Guid"/> into the packed form used within the registry.
        /// </summary>
        /// <returns>
        /// The 32-character packed form of the <see cref="Guid"/>.
        /// </returns>
        public string ToPackedString() =>
            string.Create(PackedLength, guid, static (target, value) => value.TryWritePackedString(target));

        /// <summary>
        /// Parses a string containing a packed <see cref="Guid"/>.
        /// </summary>
        /// <param name="packed">The string representation of the <see cref="Guid"/>.</param>
        /// <param name="result">The resulting <see cref="Guid"/>, if the operation succeeded.</param>
        /// <returns>
        /// <see langword="true"/>, if the conversion succeeded. Otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TryParsePackedString(ReadOnlySpan<char> packed, out Guid result)
        {
            Span<byte> bytes = stackalloc byte[16];

            if (packed.Length != PackedLength ||
                Convert.FromHexString(packed, bytes, out _, out _) != OperationStatus.Done)
            {
                result = default;
                return false;
            }

            SwapNibbles(bytes);
            result = new Guid(bytes);
            return true;
        }
    }

    #region Implementation
    private static void SwapNibbles(Span<byte> bytes)
    {
        foreach (ref var b in bytes)
        {
            b = (byte)((b << 4) | (b >> 4));
        }
    }
    #endregion
}
