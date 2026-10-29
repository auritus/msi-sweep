// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace MsiSweep.Msi.Api;

/// <summary>
/// Declarations of native methods exported by <c>MSI.DLL</c>, see <a href="https://learn.microsoft.com/en-us/windows/win32/api/msi/">msi.h</a>
/// for references.
/// </summary>
internal static partial class NativeMethods
{
    public const string Dll = "msi.dll";

    [LibraryImport(Dll, EntryPoint = "MsiEnumProductsExW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial uint MsiEnumProductsEx(
        string? szProductCode,
        string? szUserSid,
        InstallContext dwContext,
        uint dwIndex,
        Span<char> szInstalledProductCode,
        out InstallContext pdwInstalledContext,
        Span<char> szSid,
        ref uint pcchSid);

    [LibraryImport(Dll, EntryPoint = "MsiGetProductInfoExW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial uint MsiGetProductInfoEx(
        ReadOnlySpan<char> szProductCode,
        string? szUserSid,
        InstallContext dwContext,
        string szProperty,
        Span<char> lpValue,
        ref uint pcchValue);

    [LibraryImport(Dll, EntryPoint = "MsiEnumPatchesExW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial uint MsiEnumPatchesEx(
        string? szProductCode,
        string? szUserSid,
        InstallContext dwContext,
        PatchState dwFilter,
        uint dwIndex,
        Span<char> szPatchCode,
        Span<char> szTargetProductCode,
        out InstallContext pdwTargetProductContext,
        Span<char> szTargetUserSid,
        ref uint pcchTargetUserSid);

    [LibraryImport(Dll, EntryPoint = "MsiGetPatchInfoExW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial uint MsiGetPatchInfoEx(
        ReadOnlySpan<char> szPatchCode,
        ReadOnlySpan<char> szProductCode,
        string? szUserSid,
        InstallContext dwContext,
        string szProperty,
        Span<char> lpValue,
        ref uint pcchValue);

    [LibraryImport(Dll, EntryPoint = "MsiOpenDatabaseW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial uint MsiOpenDatabase(
        string szDatabasePath,
        nint szPersist,
        out nint phDatabase);

    [LibraryImport(Dll, EntryPoint = "MsiDatabaseOpenViewW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial uint MsiDatabaseOpenView(
        nint hDatabase,
        string szQuery,
        out nint phView);

    [LibraryImport(Dll, EntryPoint = "MsiRecordGetStringW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial uint MsiRecordGetString(
        nint hRecord,
        uint iField,
        Span<char> szValueBuf,
        ref uint pcchValueBuf);

    [LibraryImport(Dll, EntryPoint = "MsiGetSummaryInformationW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial uint MsiGetSummaryInformation(
        nint hDatabase,
        string szDatabasePath,
        uint uiUpdateCount,
        out nint phSummaryInfo);

    [LibraryImport(Dll, EntryPoint = "MsiSummaryInfoGetPropertyW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial uint MsiSummaryInfoGetProperty(
        nint hSummaryInfo,
        uint uiProperty,
        out uint puiDataType,
        out int piValue,
        out long pftValue,
        Span<char> szValueBuf,
        ref uint pcchValueBuf);

    [LibraryImport(Dll)]
    public static partial uint MsiCloseHandle(nint hAny);

    [LibraryImport(Dll)]
    public static partial uint MsiViewExecute(nint hView, nint hRecord);

    [LibraryImport(Dll)]
    public static partial uint MsiViewFetch(nint hView, out nint phRecord);
}
