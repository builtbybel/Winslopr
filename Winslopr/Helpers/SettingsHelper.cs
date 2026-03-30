using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Winslopr.Helpers
{
    /// <summary>
    /// Central read/write for all #key=value entries in Winslopr.txt.
    /// Used by DonationHelper, MigrationHelper, Localizer, and SettingsPage.
    /// </summary>
    public static class SettingsHelper
    {
        private static readonly string FilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Winslopr.txt");

        public static string? Get(string key)
        {
            if (!File.Exists(FilePath)) return null;
            string prefix = $"#{key}=";
            foreach (var line in File.ReadAllLines(FilePath))
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    return trimmed[prefix.Length..].Trim();
            }
            return null;
        }

        public static void Set(string key, string? value)
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            var lines = File.Exists(FilePath)
                ? File.ReadAllLines(FilePath).ToList()
                : new List<string>();

            string prefix = $"#{key}";
            lines.RemoveAll(l => l.Trim().StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            if (value != null)
                lines.Add($"#{key}={value}");
            File.WriteAllLines(FilePath, lines);
        }

        public static bool HasFlag(string key)
            => string.Equals(Get(key), "true", StringComparison.OrdinalIgnoreCase);

        public static void SetFlag(string key, bool value)
            => Set(key, value ? "true" : null);

        // -- Convenience shortcuts --------------------------------

        public static string? GetLanguage() => Get("language");
        public static void SetLanguage(string code) => Set("language", code);
    }
}
