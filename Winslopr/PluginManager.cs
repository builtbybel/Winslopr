using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Winslop;
using Winslop.Help;

/// <summary>
/// Provides functionality to load, execute, analyze, and fix external PowerShell-based plugins.
/// </summary>
public static class PluginManager
{
    private static int totalChecked;
    private static int issuesFound;

    // Public properties to access plugin analysis results
    public static int TotalChecked => totalChecked;

    public static int IssuesFound => issuesFound;

    private static void ResetAnalysis()
    {
        totalChecked = 0;
        issuesFound = 0;
    }

    /// 1. Execute a PowerShell script asynchronously and log output/errors.
    public static async Task ExecutePlugin(string pluginPath)
    {
        try
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = "powershell.exe";
                process.StartInfo.Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{pluginPath}\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        Logger.Log($"[PS Output] {e.Data}");
                };

                process.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        Logger.Log($"[PS Error] {e.Data}", LogLevel.Error);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await Task.Run(() => process.WaitForExit());
                Logger.Log($"✅ Script executed: {Path.GetFileName(pluginPath)}");
            }
        }
        catch (Exception ex)
        {
            Logger.Log($"❌ Error executing script: {ex.Message}", LogLevel.Error);
        }
    }

    /// 2. Load all .ps1 plugin files from the 'plugins' folder and return them as a FeatureTreeItem category.
    public static FeatureTreeItem LoadPlugins()
    {
        string pluginsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

        var pluginsCategory = new FeatureTreeItem("Plugins");
        // Start plugins category unchecked by default
        pluginsCategory.IsChecked = false;

        if (!Directory.Exists(pluginsFolder))
        {
            Directory.CreateDirectory(pluginsFolder);
            return pluginsCategory;
        }

        foreach (var scriptPath in Directory.GetFiles(pluginsFolder, "*.ps1"))
        {
            var scriptName = Path.GetFileNameWithoutExtension(scriptPath);
            // Reuse FeatureTreeItem; store script path in a lightweight wrapper
            // Create plugin as a non-category leaf so it participates in searches and behaves like a feature
            var pluginItem = new FeatureTreeItem(scriptName, isCategory: false, defaultChecked: false) { ScriptPath = scriptPath };
            pluginsCategory.Children.Add(pluginItem);
        }

        return pluginsCategory;
    }

    /// 3. Parse the [Commands] section from plugin content.
    private static Dictionary<string, string> ParseCommands(string pluginContent)
    {
        return ParseSection(pluginContent, "Commands");
    }

    /// 4. Parse the [Expect] section from plugin content.
    private static Dictionary<string, string> ParseExpect(string pluginContent)
    {
        return ParseSection(pluginContent, "Expect");
    }

    /// 5. Generic parser for named sections like [Commands] or [Expect].
    private static Dictionary<string, string> ParseSection(string content, string sectionName)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        bool insideSection = false;
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.Equals($"[{sectionName}]", StringComparison.OrdinalIgnoreCase))
            {
                insideSection = true;
                continue;
            }
            if (insideSection)
            {
                if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                    break;

                var idx = trimmed.IndexOf('=');
                if (idx > 0)
                {
                    var key = trimmed.Substring(0, idx).Trim();
                    var val = trimmed.Substring(idx + 1).Trim();
                    result[key] = val;
                }
            }
        }

        return result;
    }

    /// 6. Execute a shell command (CMD) and return exit code and output.
    private static async Task<(int exitCode, string output)> ExecuteCommand(string command)
    {
        var process = new Process();
        var outputBuilder = new StringBuilder();

        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = $"/c \"{command}\"";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.OutputDataReceived += (s, e) => { if (e.Data != null) outputBuilder.AppendLine(e.Data); };
        process.ErrorDataReceived += (s, e) => { if (e.Data != null) outputBuilder.AppendLine(e.Data); };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await Task.Run(() => process.WaitForExit());

        return (process.ExitCode, outputBuilder.ToString());
    }

    /// 7. Analyzes a single plugin item.
    public static async Task AnalyzePlugin(FeatureTreeItem item)
    {
        string path = item.ScriptPath;
        if (path == null || !File.Exists(path))
        {
            Logger.Log(new string('-', 50), LogLevel.Info);
            return;
        }

        string content = File.ReadAllText(path);
        var commands = ParseCommands(content);
        var expected = ParseExpect(content);

        if (!commands.ContainsKey("Check"))
        {
            item.Status = AnalysisStatus.Ok;
            Logger.Log($"🔎 Plugin ready: [PS] {Path.GetFileName(path)}");
            Logger.Log(new string('-', 50), LogLevel.Info);
            return;
        }

        string checkCmd = commands["Check"];
        var result = await ExecuteCommand(checkCmd);
        string output = result.output;

        bool allMatched = true;
        var mismatchDetails = new StringBuilder();

        foreach (var entry in expected)
        {
            var match = Regex.Match(output, $@"{Regex.Escape(entry.Key)}\s+REG_\w+\s+(\S+)", RegexOptions.IgnoreCase);

            if (match.Success)
            {
                string actual = match.Groups[1].Value;
                if (!entry.Value.Equals(actual, StringComparison.OrdinalIgnoreCase))
                {
                    allMatched = false;
                    mismatchDetails.AppendLine($"   ➤ {entry.Key}: expected '{entry.Value}', found '{actual}'");
                }
            }
            else
            {
                allMatched = false;
                mismatchDetails.AppendLine(
                    $"   ➤ Warning: The registry key '{entry.Key}' could not be located in the output.");
            }
        }

        // Update counters and item status
        totalChecked++;
        if (!allMatched) issuesFound++;
        item.Status = allMatched ? AnalysisStatus.Ok : AnalysisStatus.NeedsFix;

        Logger.Log(allMatched
            ? $"✅ Plugin: {item.Name} is properly configured."
            : $"❌ Plugin: {item.Name} requires attention.\n{mismatchDetails}",
            allMatched ? LogLevel.Info : LogLevel.Warning);

        Logger.Log(new string('-', 50), LogLevel.Info);
    }

    /// 8. Applies the fix to a single plugin item.
    public static async Task FixPlugin(FeatureTreeItem item)
    {
        string path = item.ScriptPath;
        if (path == null || !File.Exists(path)) return;

        var content = File.ReadAllText(path);
        var commands = ParseCommands(content);

        if (commands.TryGetValue("Do", out string doCmd))
        {
            Logger.Log($"🔧 Running Do command for plugin: {item.Name}");
            var (exitCode, output) = await ExecuteCommand(doCmd);
            Logger.Log($"Do Output:\n{output}");
            Logger.Log(exitCode == 0 ? "✅ Fix applied successfully." : "❌ Fix failed.");
        }
        else
        {
            Logger.Log($"🔧 No Do command found, executing full script.");
            await ExecutePlugin(path);
        }
    }

    /// 9. Reverts changes for a single plugin item.
    public static async Task RestorePlugin(FeatureTreeItem item)
    {
        string path = item.ScriptPath;
        if (path == null || !File.Exists(path)) return;

        var content = File.ReadAllText(path);
        var commands = ParseCommands(content);

        if (commands.TryGetValue("Undo", out string undoCmd))
        {
            Logger.Log($"♻️ Running Undo command for plugin: {item.Name}");
            var (exitCode, output) = await ExecuteCommand(undoCmd);
            Logger.Log($"Undo Output:\n{output}");
            Logger.Log(exitCode == 0 ? "✅ Restore successful." : "❌ Restore failed.");
        }
        else
        {
            Logger.Log($"⚠️ No Undo command found. Restore not possible.");
        }
    }

    /// 10. Recursively analyze all checked plugin items.
    public static async Task AnalyzeAll(FeatureTreeItem item)
    {
        if (!item.IsVisible) return; // Skip invisible items immediately

        if (item.IsChecked && item.IsPlugin)
            await AnalyzePlugin(item);

        foreach (var child in item.Children)
            await AnalyzeAll(child);
    }

    public static async Task AnalyzeAllPlugins(IEnumerable<FeatureTreeItem> items)
    {
        ResetAnalysis();

        Logger.Log("\n🔌 PLUGIN ANALYSIS", LogLevel.Info);
        Logger.Log(new string('=', 45), LogLevel.Info);

        // Count checked plugins before running analysis
        int checkedPlugins = 0;
        foreach (var item in items)
            foreach (var node in FeatureTreeItem.Flatten(new[] { item }))
                if (node.IsPlugin && node.IsChecked) checkedPlugins++;

        if (checkedPlugins == 0)
        {
            Logger.Log("No plugins selected – skipping plugin analysis.", LogLevel.Info);
            Logger.Log(string.Empty); // empty line for spacing
            return;
        }

        foreach (var item in items)
            await AnalyzeAll(item);
    }

    /// 11. Recursively apply fixes for all checked plugin items.
    public static async Task FixChecked(FeatureTreeItem item)
    {
        if (!item.IsVisible) return; // Skip invisible items immediately

        if (item.IsChecked && item.IsPlugin)
            await FixPlugin(item);

        foreach (var child in item.Children)
            await FixChecked(child);
    }

    /// 12. Recursively restore all checked plugin items.
    public static async Task RestoreChecked(FeatureTreeItem item)

    {
        if (!item.IsVisible) return; // Skip invisible items immediately

        if (item.IsChecked && item.IsPlugin)
            await RestorePlugin(item);

        foreach (var child in item.Children)
            await RestoreChecked(child);
    }

    /// 12. Return true if the item represents a PowerShell plugin.
    public static bool IsPluginNode(FeatureTreeItem item)
    {
        return item.IsPlugin;
    }

    /// 13. Open help for a plugin.
    public static bool ShowHelp(FeatureTreeItem item)
    {
        if (item == null) return false;
        FeatureHelp.OpenUrl(FeatureHelp.GetPluginUrl(item.Name));
        return true;
    }
}