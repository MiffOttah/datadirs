using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox.DataDirs
{
    internal static class Native
    {
        [DllImport("shell32.dll")]
        static extern int SHGetKnownFolderPath(
            [MarshalAs(UnmanagedType.LPStruct)] Guid rfid,
            uint dwFlags,
            IntPtr hToken,
            out IntPtr pszPath  // API uses CoTaskMemAlloc
        );

        internal static string GetKnownFolderPath(Guid rfid, string fallback = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6)
            {
                if (SHGetKnownFolderPath(rfid, 0, IntPtr.Zero, out var pszPath) == 0)
                {
                    string result = Marshal.PtrToStringUni(pszPath);
                    Marshal.FreeCoTaskMem(pszPath);
                    if (!string.IsNullOrEmpty(result)) return result;
                }
            }

            return fallback;
        }
    }
}
