using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

namespace Winslopr
{
    internal static class WindowsVersion
    {
        private const string CurrentVersionKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion";

        public static bool IsWindows10()
        {
            Version v = GetRealOSVersion();

            // Windows 11 is still 10.0, but starts at build 22000.
            return v.Major == 10 && v.Minor == 0 && v.Build < 22000;
        }

        public static bool IsWindows11OrLater()
        {
            Version v = GetRealOSVersion();
            return v.Major == 10 && v.Minor == 0 && v.Build >= 22000;
        }

        // Uses RtlGetVersion to avoid incorrect version values when app manifest is missing.
        public static Version GetRealOSVersion()
        {
            var os = new OSVERSIONINFOEX();
            os.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));

            int status = RtlGetVersion(ref os);
            if (status != 0)
                return Environment.OSVersion.Version;

            return new Version(os.dwMajorVersion, os.dwMinorVersion, os.dwBuildNumber);
        }

        // Returns the Update Build Revision (the part after the dot), e.g. 7921 in 26200.7921.
        public static int GetUBR()
        {
            object v = Registry.GetValue(CurrentVersionKey, "UBR", null);
            return (v is int i) ? i : 0;
        }

        public static string GetWindowsName()
        {
            var v = GetRealOSVersion();
            if (v.Major == 10 && v.Minor == 0 && v.Build >= 22000) return "Windows 11";
            if (v.Major == 10 && v.Minor == 0) return "Windows 10";
            return "Windows";
        }

        // Returns e.g. "24H2" (preferred) or older "ReleaseId" fallback.
        public static string? GetFeatureUpdate()
        {
            // DisplayVersion is used on modern Windows 10/11 (e.g. 22H2, 23H2, 24H2).
            string displayVersion = ReadString(CurrentVersionKey, "DisplayVersion");
            if (!string.IsNullOrWhiteSpace(displayVersion))
                return displayVersion;

            // ReleaseId is older (e.g. 2004, 21H2).
            string releaseId = ReadString(CurrentVersionKey, "ReleaseId");
            return string.IsNullOrWhiteSpace(releaseId) ? null : releaseId;
        }

        // Returns e.g. "Windows 11 24H2 26200.7921"
        public static string GetDisplayString()
        {
            var v = GetRealOSVersion();
            int ubr = GetUBR();
            string fu = GetFeatureUpdate();

            string build = $"{v.Build}.{ubr}";
            if (string.IsNullOrWhiteSpace(fu))
                return $"{GetWindowsName()} {build}";

            return $"{GetWindowsName()} {fu} {build}";
        }

        private static string ReadString(string keyPath, string valueName)
        {
            object v = Registry.GetValue(keyPath, valueName, null);
            return v as string;
        }

        // P/Invoke: retrieves the real Windows version from ntdll,
        // bypassing the compatibility shim that Environment.OSVersion uses.
        [DllImport("ntdll.dll", CharSet = CharSet.Unicode)]
        private static extern int RtlGetVersion(ref OSVERSIONINFOEX lpVersionInformation);

        // Maps to the Win32 OSVERSIONINFOEX struct used by RtlGetVersion.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct OSVERSIONINFOEX
        {
            public int dwOSVersionInfoSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformId;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;   // Service pack string, e.g. "Service Pack 1"

            public ushort wServicePackMajor;
            public ushort wServicePackMinor;
            public ushort wSuiteMask;
            public byte wProductType;    // e.g. VER_NT_WORKSTATION
            public byte wReserved;
        }
    }
}