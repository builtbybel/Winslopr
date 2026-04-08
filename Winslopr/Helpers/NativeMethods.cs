using System;
using System.Collections.Generic;
using System.Text;

namespace Winslopr.Helpers
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// P/Invoke wrappers for the Win32 common dialog functions.
    /// Used as a fallback for WinUI 3's StoragePicker, which fails
    /// when the app is running elevated (as administrator).
    /// </summary>
    ///
    internal static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct OPENFILENAME
        {
            public int lStructSize;
            public IntPtr hwndOwner;
            public IntPtr hInstance;
            public string lpstrFilter;
            public string lpstrCustomFilter;
            public int nMaxCustFilter;
            public int nFilterIndex;
            public string lpstrFile;
            public int nMaxFile;
            public string lpstrFileTitle;
            public int nMaxFileTitle;
            public string lpstrInitialDir;
            public string lpstrTitle;
            public int Flags;
            public short nFileOffset;
            public short nFileExtension;
            public string lpstrDefExt;
            public IntPtr lCustData;
            public IntPtr lpfnHook;
            public string lpTemplateName;
            public IntPtr pvReserved;
            public int dwReserved;
            public int FlagsEx;
        }

        [DllImport("comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetSaveFileName(ref OPENFILENAME ofn);

        [DllImport("comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetOpenFileName(ref OPENFILENAME ofn);

        public static string? ShowSaveDialog()
        {
            var ofn = new OPENFILENAME();
            ofn.lStructSize = Marshal.SizeOf(ofn);
            ofn.lpstrFilter = "Winslopr Selection\0*.sel\0\0";
            ofn.lpstrFile = new string('\0', 260);
            ofn.nMaxFile = 260;
            ofn.lpstrDefExt = "sel";
            ofn.Flags = 0x00000002; // OFN_OVERWRITEPROMPT
            return GetSaveFileName(ref ofn) ? ofn.lpstrFile.TrimEnd('\0') : null;
        }

        /// <summary>
        /// Shows a Win32 Open File dialog filtered to .sel files.
        /// </summary>
        public static string? ShowOpenDialog()
        {
            var ofn = new OPENFILENAME();
            ofn.lStructSize = Marshal.SizeOf(ofn);
            ofn.lpstrFilter = "Winslopr Selection\0*.sel\0\0";
            ofn.lpstrFile = new string('\0', 260);
            ofn.nMaxFile = 260;
            ofn.lpstrDefExt = "sel";
            ofn.Flags = 0x00001000; // OFN_FILEMUSTEXIST
            return GetOpenFileName(ref ofn) ? ofn.lpstrFile.TrimEnd('\0') : null;
        }
    }
}