using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Winslop;

/// <summary>
/// FeatureTreeItem selection export/import (text file).
/// Exports ONLY checked items to keep the file small and readable.
///
/// Format:
///   WINSLOP_SELECTION_V2
///   1;feature:Disable Widgets
///   1;plugin:Create Restore Point
/// </summary>
public static class TreeSelectionTransferV1
{
    private const string Header = "WINSLOP_SELECTION_V2";
    private const char Sep = ';';
    private const string FeaturePrefix = "feature:";
    private const string PluginPrefix = "plugin:";

    // ---------------- Basic Export / Import ----------------

    public static void ExportChecked(string filePath, IEnumerable<FeatureTreeItem> items)
    {
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("filePath is required.", nameof(filePath));
        using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
        writer.WriteLine(Header);
        foreach (var item in FeatureTreeItem.Flatten(items).Where(i => i.IsChecked && !i.IsCategory))
        {
            string key = GetSelectionKey(item);
            if (!string.IsNullOrWhiteSpace(key))
                writer.WriteLine($"1{Sep}{key}");
        }
    }

    public static void ImportChecked(string filePath, IEnumerable<FeatureTreeItem> items, bool clearFirst = true)
    {
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("filePath is required.", nameof(filePath));
        if (!File.Exists(filePath)) throw new FileNotFoundException("Selection file not found.", filePath);

        var lines = File.ReadAllLines(filePath, Encoding.UTF8);
        if (lines.Length == 0 || !string.Equals(lines[0].Trim(), Header, StringComparison.Ordinal))
            throw new InvalidOperationException("Invalid selection file format.");

        var flattened = FeatureTreeItem.Flatten(items).ToList();

        if (clearFirst)
        {
            foreach (var item in flattened)
                item.IsChecked = false;
        }

        var lookup = flattened
            .Where(i => !i.IsCategory)
            .Select(i => new { Key = GetSelectionKey(i), Item = i })
            .Where(x => !string.IsNullOrWhiteSpace(x.Key))
            .GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().Item, StringComparer.OrdinalIgnoreCase);

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (line.Length == 0) continue;

            int sepIndex = line.IndexOf(Sep);
            if (sepIndex <= 0) continue;

            string flag = line.Substring(0, sepIndex).Trim();
            if (flag != "1") continue;

            string key = NormalizeSelectionKey(line.Substring(sepIndex + 1));

            if (lookup.TryGetValue(key, out var item))
                item.IsChecked = true;
        }
    }

    // ---------------- Helpers ----------------

    private static string GetSelectionKey(FeatureTreeItem item)
    {
        if (item == null || item.IsCategory) return null;

        if (item.Feature != null)
            return FeaturePrefix + NormalizeKey(item.Feature.ID());

        if (item.IsPlugin)
        {
            string pluginId = Path.GetFileNameWithoutExtension(item.ScriptPath);
            if (string.IsNullOrWhiteSpace(pluginId))
                pluginId = item.Name;

            return PluginPrefix + NormalizeKey(pluginId);
        }

        return NormalizeKey(item.Name);
    }

    private static string NormalizeSelectionKey(string raw)
    {
        string key = NormalizeKey(raw);

        if (key.StartsWith(FeaturePrefix, StringComparison.OrdinalIgnoreCase))
            return FeaturePrefix + NormalizeKey(key.Substring(FeaturePrefix.Length));

        if (key.StartsWith(PluginPrefix, StringComparison.OrdinalIgnoreCase))
            return PluginPrefix + NormalizeKey(key.Substring(PluginPrefix.Length));

        return key;
    }

    private static string NormalizeKey(string raw)
        => (raw ?? string.Empty).Trim();
}
