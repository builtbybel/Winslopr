using Features;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Winslop.Help;

namespace Winslop
{
    /// <summary>
    /// Provides operations to load, analyze, fix, restore, and show help for FeatureNodes.
    /// </summary>
    public static class FeatureNodeManager
    {
        private static int totalChecked;
        private static int issuesFound;

        // Public properties to access the analysis results
        public static int TotalChecked => totalChecked;
        public static int IssuesFound => issuesFound;

        public static void ResetAnalysis()
        {
            totalChecked = 0;
            issuesFound = 0;
            Logger.Clear();
        }

        /// <summary>
        /// Loads all features into the TreeView.
        /// </summary>
        public static void LoadFeatures(TreeView tree)
        {
            // Hide the TreeView to avoid flickering and visible scroll jump
            tree.Visible = false;
            tree.BeginUpdate();

            try
            {
                var features = FeatureLoader.Load();
                tree.Nodes.Clear();

                foreach (var feature in features)
                    AddNode(tree.Nodes, feature);

                // root nodes (categories)
                foreach (TreeNode root in tree.Nodes)
                {
                    root.NodeFont = new Font(tree.Font, FontStyle.Bold);
                    root.ForeColor = Color.Black; // category color
                }

                tree.ExpandAll(); // expand all nodes

                // Ensure the first node is shown at the top (prevents auto-scroll to bottom)
                if (tree.Nodes.Count > 0) tree.TopNode = tree.Nodes[0];
            }
            finally
            {
                tree.EndUpdate();
                tree.Visible = true;
            }

            // Ensure top node after finished layout/paint (ExpandAll/Visible can override TopNode)
            tree.BeginInvoke((Action)(() =>
            {
                if (tree.IsDisposed || tree.Nodes.Count == 0) return;
                tree.TopNode = tree.Nodes[0];
            }));
        }

        /// <summary>
        /// Recursively adds a FeatureNode and its children into the TreeView.
        /// </summary>
        private static void AddNode(TreeNodeCollection treeNodes, FeatureNode featureNode)
        {
            string text = featureNode.IsCategory
                ? "  " + featureNode.Name + "  " // add extra space to avoid clipping
                : featureNode.Name;

            // ----------------------------------------------------------------------------
            // If the feature is not applicable, append a hint to the text.
            // The actual check is done during analysis, but this gives an early visual cue.
            if (!featureNode.IsCategory && featureNode.Feature != null && !featureNode.Feature.IsApplicable())
            {
                // Append applicability hint to the node text.
                text += " (not applicable)";
            }

            // ----------------------------------------------------------------------------

            TreeNode node = new TreeNode(text)
            {
                Tag = featureNode,
                Checked = featureNode.DefaultChecked,
            };
            treeNodes.Add(node);

            foreach (var child in featureNode.Children)
                AddNode(node.Nodes, child);
        }

        /// <summary>
        /// Analyzes all checked features recursively and logs only issues.
        /// </summary>
        public static async Task AnalyzeAll(TreeNodeCollection nodes)
        {
            ResetAnalysis();

            // Iterate through all nodes and analyze each one recursively
            foreach (TreeNode node in nodes)
            {
                // Recursively analyze each node and ensure async tasks are awaited
                await AnalyzeCheckedRecursive(node);
            }

            Logger.Log("🔎 ANALYSIS COMPLETE", LogLevel.Info);
            Logger.Log(new string('=', 50), LogLevel.Info);

            int ok = totalChecked - issuesFound;
            Logger.Log($"Summary: {ok} of {totalChecked} checked settings are OK; {issuesFound} require attention.",
                issuesFound > 0 ? LogLevel.Warning : LogLevel.Info);

            // Show message box with summary
            string title = issuesFound > 0
                ? "Analysis Complete – Attention Required"
                : "Analysis Complete";

            string message =
                $"✔ OK: {ok} of {totalChecked}\n" +
                $"⚠ Issues found: {issuesFound}\n\n" +
                "Before applying changes:\n" +
                "• Review the local log\n" +
                "• If you are unsure, use the Online Log Inspector\n" +
                "  to verify and understand each finding.";

            MessageBoxIcon icon = issuesFound > 0
                ? MessageBoxIcon.Warning
                : MessageBoxIcon.Information;

            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        /// <summary>
        /// Recursively checks all features and logs misconfigurations.
        /// </summary>
        private static async Task AnalyzeCheckedRecursive(TreeNode node)
        {
            if (node.Tag is FeatureNode fn)
            {
                // If the node is not a category, is checked, and has a feature to check
                if (!fn.IsCategory && node.Checked && fn.Feature != null)
                {
                    // ----------------------------------------------------------------------------
                    // Always analyze the selected leaf node, regardless of Checked state, but skip if not applicable.
                    if (!fn.Feature.IsApplicable())
                    {
                        // Mark as not applicable and skip checks.
                        node.ForeColor = Color.DarkGray;

                        string reason = fn.Feature.InapplicableReason();
                        if (!string.IsNullOrWhiteSpace(reason))
                            Logger.Log($"ℹ️ [{node.Parent?.Text ?? "General"}] {fn.Name} - Skipped: {reason}", LogLevel.Info);
                        else
                            Logger.Log($"ℹ️ [{node.Parent?.Text ?? "General"}] {fn.Name} - Skipped: Not applicable on this OS.", LogLevel.Info);

                        return;
                    }
                    // ----------------------------------------------------------------------------

                    totalChecked++;
                    bool isOk = await fn.Feature.CheckFeature();  // Await the async operation

                    if (!isOk)
                    {
                        issuesFound++;
                        node.ForeColor = Color.Red; // Mark as misconfigured
                        string category = node.Parent?.Text ?? "General";
                        Logger.Log($"❌ [{category}] {fn.Name} - Not configured as recommended.");
                        Logger.Log($"   ➤ {fn.Feature.GetFeatureDetails()}");
                        // Log a separator when an issue was found
                        Logger.Log(new string('-', 50), LogLevel.Info);
                    }
                    else
                    {
                        node.ForeColor = Color.Gray; // Mark as properly configured
                    }
                }

                // Recursively process child nodes and ensure awaiting the tasks
                foreach (TreeNode child in node.Nodes)
                {
                    await AnalyzeCheckedRecursive(child);  // Recursively call and await the result
                }
            }
        }

        /// <summary>
        /// Fixes all checked features recursively.
        /// </summary>
        public static async Task FixChecked(TreeNode node)
        {
            if (node.Tag is FeatureNode fn)
            {
                if (!fn.IsCategory && node.Checked && fn.Feature != null)
                {
                    // ----------------------------------------------------------------------------
                    // Always attempt to fix the selected leaf node, regardless of Checked state, but skip if not applicable.
                    if (!fn.Feature.IsApplicable())
                    {
                        Logger.Log($"ℹ️ {fn.Name} - Skipped: {fn.Feature.InapplicableReason() ?? "Not applicable on this OS."}", LogLevel.Info);
                        return;
                    }
                    // ----------------------------------------------------------------------------

                    bool result = await fn.Feature.DoFeature();
                    Logger.Log(result
                        ? $"🔧 {fn.Name} - Fixed"
                        : $"❌ {fn.Name} - ⚠️ Fix failed (This feature may require admin privileges)",
                        result ? LogLevel.Info : LogLevel.Error);
                }

                foreach (TreeNode child in node.Nodes)
                    await FixChecked(child);
            }
        }

        /// <summary>
        /// Restores all checked features recursively.
        /// </summary>
        public static void RestoreChecked(TreeNode node)
        {
            if (node.Tag is FeatureNode fn)
            {
                if (!fn.IsCategory && node.Checked && fn.Feature != null)
                {
                    // ----------------------------------------------------------------------------
                    // Always restore the selected leaf node, regardless of Checked state, but skip if not applicable.
                    if (!fn.Feature.IsApplicable())
                    {
                        Logger.Log($"ℹ️ {fn.Name} - Skipped restore: {fn.Feature.InapplicableReason() ?? "Not applicable on this OS."}", LogLevel.Info);
                        return;
                    }
                    // ----------------------------------------------------------------------------

                    bool ok = fn.Feature.UndoFeature();
                    string category = node.Parent?.Text ?? "General";
                    Logger.Log(ok
                        ? $"↩️ [{category}] {fn.Name} - Restored"
                        : $"❌ [{category}] {fn.Name} - Restore failed",
                        ok ? LogLevel.Info : LogLevel.Error);
                }

                foreach (TreeNode child in node.Nodes)
                    RestoreChecked(child);
            }
        }

        /// <summary>
        /// Analyzes a selected feature or, if it's a category, analyzes only checked child features.
        /// </summary>
        public static async void AnalyzeFeature(TreeNode node)
        {
            // Analyze this node if it's a leaf node (not a category)
            if (node.Tag is FeatureNode fn && !fn.IsCategory && fn.Feature != null)
            {
                // ----------------------------------------------------------------------------
                // Always analyze the selected leaf node, regardless of Checked state, but skip if not applicable.
                if (!fn.Feature.IsApplicable())
                {
                    Logger.Log($"ℹ️ {fn.Name} - Skipped: {fn.Feature.InapplicableReason() ?? "Not applicable on this OS."}", LogLevel.Info);
                    return;
                }
                // ----------------------------------------------------------------------------

                bool isOk = await fn.Feature.CheckFeature();
                node.ForeColor = isOk ? Color.Gray : Color.Red;

                if (isOk)
                {
                    Logger.Log($"✅ Feature: {fn.Name} is properly configured.", LogLevel.Info);
                }
                else
                {
                    string category = node.Parent?.Text ?? "General";
                    Logger.Log($"❌ Feature: {fn.Name} requires attention.", LogLevel.Warning);
                    Logger.Log($"   ➤ {fn.Feature.GetFeatureDetails()}");
                    Logger.Log(new string('-', 50), LogLevel.Info);
                }
            }
            else
            {
                // If it's a category node, analyze only checked child nodes
                foreach (TreeNode child in node.Nodes)
                {
                    if (child.Checked)
                        AnalyzeFeature(child);
                }
            }
        }

        /// <summary>
        /// Attempts to fix the selected feature or, if it is a category, fixes only checked child features.
        /// </summary>
        public static async Task FixFeature(TreeNode node)
        {
            // Try to fix this node if it is NOT a category (i.e., a leaf node)
            if (node.Tag is FeatureNode fn && !fn.IsCategory && fn.Feature != null)
            {
                // ----------------------------------------------------------------------------
                // Skip features that are not applicable on this OS/environment
                if (!fn.Feature.IsApplicable())
                {
                    Logger.Log(
                        $"ℹ️ {fn.Name} - Skipped: {fn.Feature.InapplicableReason() ?? "Not applicable on this OS."}",
                        LogLevel.Info);

                    return;
                }
                // ----------------------------------------------------------------------------

                // Always fix the selected leaf node, regardless of Checked
                bool result = await fn.Feature.DoFeature();
                Logger.Log(result
                    ? $"🔧 {fn.Name} - Fixed"
                    : $"❌ {fn.Name} - ⚠️ Fix failed (This feature may require admin privileges)",
                    result ? LogLevel.Info : LogLevel.Error);
            }
            else
            {
                // If it's a category node, fix only checked child nodes (recursively)
                foreach (TreeNode child in node.Nodes)
                {
                    if (child.Checked)
                        await FixFeature(child);
                }
            }
        }

        /// <summary>
        /// Restores a selected feature (always) or, if it's a category, only restores checked child features.
        /// Logs success or failure.
        /// </summary>
        public static void RestoreFeature(TreeNode node)
        {
            // Restore feature node regardless of Checked state
            if (node.Tag is FeatureNode fn && !fn.IsCategory && fn.Feature != null)
            {
                // ----------------------------------------------------------------------------
                // Skip features that are not applicable on this OS/environment.
                if (!fn.Feature.IsApplicable())
                    {
                    Logger.Log($"ℹ️ {fn.Name} - Skipped restore: {fn.Feature.InapplicableReason() ?? "Not applicable on this OS."}", LogLevel.Info);
                    return;
                }
                // ----------------------------------------------------------------------------

                bool ok = fn.Feature.UndoFeature();
                Logger.Log(ok
                    ? $"↩️ {fn.Name} - Restored"
                    : $"❌ {fn.Name} - Restore failed",
                    ok ? LogLevel.Info : LogLevel.Error);
            }
            else
            {
                // For category nodes, only restore checked children
                foreach (TreeNode child in node.Nodes)
                {
                    if (child.Checked)
                        RestoreFeature(child);
                }
            }
        }

        /// <summary>
        /// Opens help for the selected feature or plugin.
        /// Features: opens GitHub documentation.
        /// Plugins: uses PluginManager.ShowHelp(node).
        /// </summary>
        public static bool ShowHelp(TreeNode node)
        {
            if (node == null) return false;

            try
            {
                // If it is a feature node, use the feature URL.
                if (node.Tag is FeatureNode fn && fn.Feature != null)
                    FeatureHelp.OpenUrl(FeatureHelp.GetFeatureUrl(fn.Feature));
                else
                    // Otherwise treat it as plugin title.
                    FeatureHelp.OpenUrl(FeatureHelp.GetPluginUrl(node.Text));

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not open online help.\n\nDetails: " + ex.Message,
                    "Help",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }
        }
    }
}