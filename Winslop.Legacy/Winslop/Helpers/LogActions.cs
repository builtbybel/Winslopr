using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

/// <summary>
/// Provides actions for interacting with and managing the content of a log displayed in a RichTextBox.
/// /// <remarks>This class allows users to perform common operations on a log, such as copying its content to the
/// clipboard, analyzing it using the Winslop online inspector tool, or clearing the log. It is designed to work with a <see
/// cref="RichTextBox"/> control that serves as the log display.</remarks>
/// 
/// Also provides small helper log outputs for the Features tree (optional).
/// </summary>
public sealed class LogActions
{
    private readonly RichTextBox _logBox;

    // Optional provider for the FeaturesView TreeView 
    private Func<TreeView> _getFeaturesTree;

    public LogActions(RichTextBox logBox)
    {
        _logBox = logBox ?? throw new ArgumentNullException(nameof(logBox));
    }

    /// <summary>
    /// Registers a callback that returns the FeaturesView TreeView (can be null).
    /// </summary>
    public void SetFeaturesTreeProvider(Func<TreeView> getTree)
    {
        _getFeaturesTree = getTree;
    }

    /// <summary>Copies the whole log to the clipboard.</summary>
    public void CopyToClipboard()
    {
        var text = _logBox.Text;
        if (!string.IsNullOrWhiteSpace(text))
            Clipboard.SetText(text);
    }

    /// <summary>Clears the logger output.</summary>
    public void Clear()
        => _logBox.Clear();

    /// <summary>Opens Winslop online inspector tool and passes the log via URL‑encoded GET parameter.</summary>
    public void AnalyzeOnline(string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            return;

        var logText = _logBox.Text;

        if (string.IsNullOrWhiteSpace(logText))
        {
            MessageBox.Show(
                "There's nothing to analyze yet.\n\nRun an inspection first, then try again.",
                "Nothing to analyze (yet)",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        Clipboard.SetText(logText);
        Process.Start(baseUrl);

        MessageBox.Show(
            "The log has been copied to the clipboard.\n" +
            "Click “Paste log from clipboard” on the log inspector page, or press CTRL+V.",
            "Log copied",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    // ---------------- Feature tree log tools (optional) ----------------

    /// <summary>
    /// Logs a short summary for the current feature tree (total + checked).
    /// </summary>
    public void LogFeatureSummary()
    {
        var tree = GetTreeOrNull();
        if (tree == null)
        {
            AppendLine("[Features] Not available.");
            return;
        }

        int total = EnumerateNodes(tree.Nodes).Count();
        int checkedCount = EnumerateNodes(tree.Nodes).Count(n => n.Checked);

        AppendLine($"[Features] Total nodes: {total}, Checked: {checkedCount}");
    }

    /// <summary>
    /// Logs all checked leaf nodes (actual features/plugins, not categories).
    /// </summary>
    public void LogCheckedFeatures()
    {
        var tree = GetTreeOrNull();
        if (tree == null)
        {
            AppendLine("[Features] Not available.");
            return;
        }

        var list = EnumerateNodes(tree.Nodes)
            .Where(n => n.Checked && n.Nodes.Count == 0)
            .Select(GetNodePath)
            .ToList();

        if (list.Count == 0)
        {
            AppendLine("[Features] No checked items.");
            return;
        }

        AppendLine($"[Features] Checked items ({list.Count}):");
        foreach (var item in list)
            AppendLine("  - " + item);
    }

    /// <summary>
    /// Logs unchecked leaf nodes (can be useful to see what is NOT selected).
    /// Output is truncated to avoid flooding the log.
    /// </summary>
    public void LogUncheckedLeafFeatures(int maxLines = 200)
    {
        var tree = GetTreeOrNull();
        if (tree == null)
        {
            AppendLine("[Features] Not available.");
            return;
        }

        var list = EnumerateNodes(tree.Nodes)
            .Where(n => !n.Checked && n.Nodes.Count == 0)
            .Select(GetNodePath)
            .ToList();

        AppendLine($"[Features] Unchecked leaf items ({list.Count}):");
        foreach (var item in list.Take(maxLines))
            AppendLine("  - " + item);

        if (list.Count > maxLines)
            AppendLine("  ... (truncated)");
    }

    private TreeView GetTreeOrNull()
        => _getFeaturesTree != null ? _getFeaturesTree() : null;

    private void AppendLine(string text)
        => _logBox.AppendText(text + Environment.NewLine);

    private static IEnumerable<TreeNode> EnumerateNodes(TreeNodeCollection nodes)
    {
        foreach (TreeNode n in nodes)
        {
            yield return n;
            foreach (var c in EnumerateNodes(n.Nodes))
                yield return c;
        }
    }

    private static string GetNodePath(TreeNode node)
    {
        var stack = new Stack<string>();
        var cur = node;
        while (cur != null)
        {
            stack.Push((cur.Text ?? string.Empty).Trim());
            cur = cur.Parent;
        }
        return string.Join("/", stack);
    }
}
