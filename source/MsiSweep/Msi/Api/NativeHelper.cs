// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using MsiSweep.Extensions;
using static MsiSweep.Msi.Api.NativeMethods;

namespace MsiSweep.Msi.Api;

internal static class NativeHelper
{
    public static string? GetProductInfo(
        Guid product,
        string? sid,
        InstallContext context,
        string property)
    {
        return ReadString((buffer, ref length) =>
        {
            return MsiGetProductInfoEx(product.ToApiString(), sid, context, property, buffer, ref length);
        }, [ErrorCode.UnknownProduct, ErrorCode.UnknownProperty]);
    }

    public static string? GetPatchInfo(
        Guid patch,
        Guid product,
        string? sid,
        InstallContext context,
        string property)
    {
        return ReadString((buffer, ref length) =>
        {
            return MsiGetPatchInfoEx(patch.ToApiString(), product.ToApiString(), sid, context, property, buffer, ref length);
        }, [ErrorCode.UnknownPatch, ErrorCode.UnknownProduct, ErrorCode.UnknownProperty]);
    }

    public static string? GetPackageName(string path)
    {
        const nint OpenReadOnly = 0;    // MSIDBOPEN_READONLY
        const nint OpenPatchFile = 16;  // MSIDBOPEN_PATCHFILE (32 / sizeof(WCHAR))

        var isPatch = path.EndsWith(".msp", StringComparison.OrdinalIgnoreCase);
            var name = isPatch
                ? QueryValue(path, OpenPatchFile, "SELECT `Value` FROM `MsiPatchMetadata` WHERE `Property`='DisplayName'")
                : QueryValue(path, OpenReadOnly, "SELECT `Value` FROM `Property` WHERE `Property`='ProductName'");

        return string.IsNullOrEmpty(name) ? GetSubject(path) : name;
    }

    public static string? GetSubject(string path)
    {
        const uint PidSubject = 3;

        if (MsiGetSummaryInformation(0, path, 0, out var handle) != ErrorCode.Success)
        {
            return null;
        }

        try
        {
            return ReadString((buffer, ref length) =>
            {
                return MsiSummaryInfoGetProperty(handle, PidSubject, out _, out _, out _, buffer, ref length);
            }, [ErrorCode.UnknownProperty, ErrorCode.DatatypeMismatch]);
        }
        finally
        {
            _ = MsiCloseHandle(handle);
        }
    }

    #region Implementation
    private delegate uint StringGetter(Span<char> buffer, ref uint length);

    private static string? QueryValue(string path, nint persist, string query)
    {
        if (MsiOpenDatabase(path, persist, out var database) != ErrorCode.Success)
        {
            return null;
        }

        try
        {
            if (MsiDatabaseOpenView(database, query, out var view) != ErrorCode.Success)
            {
                return null;
            }

            try
            {
                if (MsiViewExecute(view, 0) != ErrorCode.Success)
                {
                    return null;
                }

                if (MsiViewFetch(view, out var record) != ErrorCode.Success)
                {
                    return null;
                }

                try
                {
                    return ReadString((buffer, ref length) =>
                    {
                        return MsiRecordGetString(record, 1, buffer, ref length);
                    });
                }
                finally
                {
                    _ = MsiCloseHandle(record);
                }
            }
            finally
            {
                _ = MsiCloseHandle(view);
            }
        }
        finally
        {
            _ = MsiCloseHandle(database);
        }
    }

    private static uint ReadStringLength(StringGetter getter, ReadOnlySpan<uint> ignore)
    {
        Span<char> buffer = stackalloc char[1];

        var length = 0u;
        var result = getter(buffer, ref length);

        if (ignore.Contains(result))
        {
            return 0;
        }

        if (result != ErrorCode.Success && result != ErrorCode.MoreData)
        {
            throw new InvalidOperationException($"Failed to read string length. Error code: {result}");
        }

        return length;
    }

    private static string? ReadString(StringGetter getter)
    {
        return ReadString(getter, []);
    }

    private static string? ReadString(StringGetter getter, ReadOnlySpan<uint> ignore)
    {
        var length = ReadStringLength(getter, ignore);

        if (length == 0)
        {
            return null;
        }

        return string.Create((int)length, getter, static (target, getter) =>
        {
            // Most API calls write the 0-terminator, so we need to allocate
            // an additional character for that, unfortunately.
            var intermediate = target.Length < 256
                ? stackalloc char[1 + target.Length]
                : new char[1 + target.Length];

            var length = (uint)intermediate.Length;
            var result = getter(intermediate, ref length);

            intermediate[..^1].CopyTo(target);

            if (result != ErrorCode.Success && result != ErrorCode.MoreData)
            {
                throw new InvalidOperationException($"Failed to read string. Error code: {result}");
            }
        });
    }
    #endregion
}
