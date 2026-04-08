using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Winslop;
using Winslop.Help;

/// <summary>
/// Provides functionality to load, execute, analyze, and fix external PowerShell-based plugins.
/// </summary>
///
public static class PluginManager
{
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

    /// 2. Load all .ps1 plugin files from the 'plugins' folder into a TreeView.
    public static void LoadPlugins(TreeView treeView)
    {
        string pluginsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");

        if (!Directory.Exists(pluginsFolder))
        {
            Directory.CreateDirectory(pluginsFolder);
            return;
        }

        var pluginsNode = new TreeNode("Plugins")
        {
            BackColor = Color.LightPink,
            ForeColor = Color.Black
        };

        foreach (var scriptPath in Directory.GetFiles(pluginsFolder, "*.ps1"))
        {
            var scriptName = Path.GetFileNameWithoutExtension(scriptPath);
            var scriptNode = new TreeNode
            {
                Text = $"{scriptName}", // [PS]
                ToolTipText = scriptPath,
                Tag = scriptPath,
                Checked = false
            };
            pluginsNode.Nodes.Add(scriptNode);
        }

        treeView.Nodes.Add(pluginsNode);
        pluginsNode.Expand();
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
    /// Lines must be in 'key = value' format.
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
            // Exit the section when another section begins
            if (insideSection)
            {
                if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                    break;

                // Parse lines of form: key = value
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

    /// <summary>
    /// 7. Analyzes a single plugin node by running its 'Check' command
    /// and comparing the output against expected values defined in the [Expect] section.
    /// Logs a summary indicating success if all checks pass, or warnings if mismatches occur.
    /// Only the specified plugin node is analyzed, regardless of its checked state.
    /// </summary>
    /// <param name="node">The TreeNode representing the plugin to analyze, with script path stored in Tag.</param>
    public static async Task AnalyzePlugin(TreeNode node)
    {
        if (node == null || node.Tag == null || !File.Exists(node.Tag.ToString()))
        {
            // Logger.Log($"❌ Script file not found for plugin: {node?.Text}", LogLevel.Error);
            Logger.Log(new string('-', 50), LogLevel.Info);
        }
        else
        {
            string pluginName = node.Text;
            string path = node.Tag.ToString();
            string content = File.ReadAllText(path);

            Dictionary<string, string> commands = ParseCommands(content);
            Dictionary<string, string> expected = ParseExpect(content);

            if (!commands.ContainsKey("Check"))
            {
                Logger.Log($"🔎 Plugin ready: [PS] {Path.GetFileName(path)}");
                Logger.Log(new string('-', 50), LogLevel.Info);
            }
            else
            {
                string checkCmd = commands["Check"];
                var result = await ExecuteCommand(checkCmd);
                string output = result.Item2;

                bool allMatched = true;
                StringBuilder mismatchDetails = new StringBuilder();

                foreach (var entry in expected)
                {
                    string key = entry.Key;
                    string expectedVal = entry.Value;

                    var match = Regex.Match(output, $@"{Regex.Escape(key)}\s+REG_\w+\s+(\S+)", RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        string actual = match.Groups[1].Value;

                        if (!expectedVal.Equals(actual, StringComparison.OrdinalIgnoreCase))
                        {
                            allMatched = false;
                            mismatchDetails.AppendLine($"   ➤ {key}: expected '{expectedVal}', found '{actual}'");
                        }
                    }
                    else
                    {
                        allMatched = false;
                        mismatchDetails.AppendLine(
                            $"   ➤ Warning: The registry key '{key}' could not be located in the output. " +
                            "This usually means the key is missing and the tweak will have to add it. " +
                            "[InternalCode: Could not be parsed from output]");
                    }
                }

                if (allMatched)
                {
                    Logger.Log($"✅ Plugin: {pluginName} is properly configured.", LogLevel.Info);
                    node.ForeColor = Color.Gray;
                }
                else
                {
                    Logger.Log($"❌ Plugin: {pluginName} requires attention.\n{mismatchDetails}", LogLevel.Warning);
                    node.ForeColor = Color.Red;
                }

                Logger.Log(new string('-', 50), LogLevel.Info);
            }
        }
    }

    /// <summary>
    /// 8. Applies the fix to a single plugin node.
    /// This method processes only the specified node regardless of its checked state.
    /// It attempts to run the "Do" command from the plugin script, or falls back to executing the entire script.
    /// </summary>
    public static async Task FixPlugin(TreeNode node)
    {
        if (node?.Tag is string path && File.Exists(path))
        {
            var content = File.ReadAllText(path);
            var commands = ParseCommands(content);

            if (commands.TryGetValue("Do", out string doCmd))
            {
                Logger.Log($"🔧 Running Do command for plugin: {node.Text}");
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
    }

    /// <summary>
    /// 9. Reverts changes for a single plugin node.
    /// </summary>
    public static async Task RestorePlugin(TreeNode node)
    {
        if (node?.Tag is string path && File.Exists(path))
        {
            var content = File.ReadAllText(path);
            var commands = ParseCommands(content);

            if (commands.TryGetValue("Undo", out string undoCmd))
            {
                Logger.Log($"♻️ Running Undo command for plugin: {node.Text}");
                var (exitCode, output) = await ExecuteCommand(undoCmd);
                Logger.Log($"Undo Output:\n{output}");

                Logger.Log(exitCode == 0 ? "✅ Restore successful." : "❌ Restore failed.");
            }
            else
            {
                Logger.Log($"⚠️ No Undo command found. Restore not possible.");
                MessageBox.Show("Restore is not possible for this plugin.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    /// 10. Recursively analyze all checked plugin nodes.
    public static async Task AnalyzeAll(TreeNode node)
    {
        if (node.Checked && node.Tag is string path && File.Exists(path))
            await AnalyzePlugin(node);

        foreach (TreeNode child in node.Nodes)
            await AnalyzeAll(child);
    }

    public static async Task AnalyzeAllPlugins(TreeNodeCollection nodes)
    {
        Logger.Log("\n🔌 PLUGIN ANALYSIS", LogLevel.Info);
        Logger.Log(new string('=', 50), LogLevel.Info);

        foreach (TreeNode node in nodes)
            await AnalyzeAll(node);
    }

    /// 11. Recursively apply fixes for all checked plugin nodes.
    public static async Task FixChecked(TreeNode node)
    {
        if (node.Checked && node.Tag is string pluginPath)
        {
            var pluginName = Path.GetFileName(pluginPath);
            var proceed = ShowPluginWarning(pluginName);
            if (!proceed) return;
            await FixPlugin(node);
        }

        foreach (TreeNode child in node.Nodes)
            await FixChecked(child);
    }

    /// 12. Return true if the node represents a PowerShell plugin file.
    public static bool IsPluginNode(TreeNode node)
    {
        return node?.Tag is string path && path.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase);
    }

    /// 13. Show a warning before executing external plugin code.
    public static bool ShowPluginWarning(string pluginName)
    {
        var result = MessageBox.Show(
            $"⚠️ WARNING: The plugin '{pluginName}' is an external script. Its execution is outside this app's responsibility and at your own risk.\n" +
            "Proceed only if you trust the source of this plugin. Do you want to continue?",
            "Plugin Activation Warning",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
        );

        return result == DialogResult.Yes;
    }

    public static bool ShowHelp(TreeNode node)
    {
        if (node == null) return false;

        // Open the plugin section in features.md (anchor based on node.Text).
        FeatureHelp.OpenUrl(FeatureHelp.GetPluginUrl(node.Text));
        return true;
    }
}