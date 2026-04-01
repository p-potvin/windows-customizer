using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace ThumbnailRefresher.Native;

[ComImport]
[Guid("e357fccd-a995-4576-b01f-234630154e96")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IThumbnailProvider
{
    void GetThumbnail(int cx, out IntPtr phbmp, out WTS_ALPHATYPE pdwAlpha);
}

[ComImport]
[Guid("f676c109-14a0-491a-8bb7-915479ea266c")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IThumbnailCache
{
    void GetThumbnail(
        [In] IShellItem pShellItem,
        [In] uint cxyRequestedThumbSize,
        [In] WTS_FLAGS flags,
        [Out] out ISharedBitmap ppvThumb,
        [Out, Optional] out WTS_CACHEFLAGS pOutFlags,
        [Out, Optional] out WTS_THUMBNAILID pThumbnailID);

    void GetThumbnailByID(
        [In] WTS_THUMBNAILID thumbnailID,
        [In] uint cxyRequestedThumbSize,
        [Out] out ISharedBitmap ppvThumb,
        [Out, Optional] out WTS_CACHEFLAGS pOutFlags);
}

[ComImport]
[Guid("09117b01-576a-4974-9130-975949d26b01")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ISharedBitmap
{
    void GetBitmap([Out] out IntPtr phbm);
    void GetSize([Out] out SIZE pSize);
    void GetFormat([Out] out WTS_ALPHATYPE pdwAlpha);
    void InitializeBitmap([In] IntPtr hbm, [In] WTS_ALPHATYPE wtsAlpha);
    void Detach([Out] out IntPtr phbm);
}

[ComImport]
[Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IShellItem
{
    void BindToHandler([In, Optional] IntPtr pbc, [In] ref Guid bhid, [In] ref Guid riid, [Out] out IntPtr ppv);
    void GetParent([Out] out IShellItem ppsi);
    void GetDisplayName([In] SIGDN sigdnName, [Out] out IntPtr ppszName);
    void GetAttributes([In] uint sfgaoMask, [Out] out uint psfgaoAttribs);
    void Compare([In] IShellItem psi, [In] uint hint, [Out] out int piOrder);
}

public enum WTS_FLAGS : uint
{
    WTS_EXTRACT = 0x00000000,
    WTS_INCACHEONLY = 0x00000001,
    WTS_FASTEXTRACT = 0x00000002,
    WTS_FORCEEXTRACTION = 0x00000004,
    WTS_SLOWRECLAIM = 0x00000008,
    WTS_EXTRACTDONOTRETRIEVE = 0x00000010,
    WTS_SCALETOREQUESTEDSIZE = 0x00000020,
    WTS_SKIPFASTEXTRACT = 0x00000040,
    WTS_EXTRACTROOTSONLY = 0x00000080
}

public enum WTS_CACHEFLAGS : uint
{
    WTS_DEFAULT = 0x00000000,
    WTS_LOWQUALITY = 0x00000001,
    WTS_CACHED = 0x00000002
}

public enum WTS_ALPHATYPE : int
{
    WTSAT_UNKNOWN = 0,
    WTSAT_RGB = 1,
    WTSAT_ARGB = 2
}

[StructLayout(LayoutKind.Sequential, Size = 16)]
public struct WTS_THUMBNAILID
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public byte[] rgbKey;
}

[StructLayout(LayoutKind.Sequential)]
public struct SIZE
{
    public int cx;
    public int cy;
}

public enum SIGDN : uint
{
    SIGDN_FILESYSPATH = 0x80058000
}

public static class ShellNativeMethods
{
    [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
    public static extern void SHCreateItemFromParsingName(
        [In, MarshalAs(UnmanagedType.LPWStr)] string pszPath,
        [In] IntPtr pbc,
        [In] ref Guid riid,
        [Out] out IShellItem ppv);

    public static readonly Guid CLSID_LocalThumbnailCache = new Guid("43362143-6ef2-4770-96d7-97554659aff2");
    // Standard LocalThumbnailCache: {43362143-6ef2-4770-96d7-97554659aff2}
    // Shared LocalThumbnailCache: {50238124-7431-4824-8f43-4ff716f9f584}
    public static readonly Guid IID_IThumbnailCache = new Guid("f676c109-14a0-491a-8bb7-915479ea266c");
    public static readonly Guid IID_IShellItem = new Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe");
    public static readonly Guid BHID_ThumbnailHandler = new Guid("f676c109-14a0-491a-8bb7-915479ea266c");

    public const uint SFGAO_OFFLINE = 0x00001000;
    public const uint SFGAO_STORAGE = 0x00000008;
    public const uint SFGAO_STREAM = 0x00400000;

    [DllImport("gdi32.dll")]
    public static extern bool DeleteObject(IntPtr hObject);
}
