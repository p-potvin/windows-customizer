using System.Runtime.InteropServices;
using System.Text;

namespace ViewManager
{
    internal static class Shell
    {
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr ILCreateFromPathW(string pszPath);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SHGetPathFromIDListW(IntPtr pidl, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszPath);

        [DllImport("shell32.dll", SetLastError = true)]
        public static extern void ILFree(IntPtr pidl);

        public const int MAX_PATH = 260;
    }
}
