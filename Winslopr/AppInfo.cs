using System.Reflection;

namespace Winslop
{
    public static class AppInfo
    {
        // Returns version as v26.03.40
        public static string VersionString
        {
            get
            {
                var ver = Assembly.GetExecutingAssembly().GetName().Version;
                if (ver == null)
                    return "v0.00.00";
                return $"{ver.Major:D2}.{ver.Minor:D2}.{ver.Build:D2}";
            }
        }

        // Returns version as 26.03.40 (for Update URLs etc.)
        public static string RawVersion
        {
            get
            {
                var ver = Assembly.GetExecutingAssembly().GetName().Version;
                if (ver == null)
                    return "0.00.00";
                return $"{ver.Major:D2}.{ver.Minor:D2}.{ver.Build:D2}";
            }
        }
    }
}
