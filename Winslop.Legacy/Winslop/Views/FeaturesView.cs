using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Winslop.Views
{
    public partial class FeaturesView : UserControl, IMainActions, ISearchable, IView
    {
        // Expose the TreeView so MainForm/LogActions can access it
        public TreeView Tree => treeFeatures;

        /// <summary>
        /// FeatureView hosts the Windows feature/plugin tree and encapsulates all
        /// analyze/fix/restore actions for that tree.
        /// </summary>
        private TreeNode[] _originalNodes;
        private bool _profilesWired;

        public FeaturesView()
        {
            InitializeComponent();
        }

        private void FeaturesView_Load(object sender, EventArgs e)
        {
            if (_profilesWired) return;

            TreeSelectionTransferV1.WireProfileComboBox(
              comboProfiles,
                treeFeatures,
                this.FindForm(),
                directory: AppDomain.CurrentDomain.BaseDirectory,
                clearFirst: true,
                log: msg => Logger.Log(msg, LogLevel.Info)
            );

            _profilesWired = true;
        }

        public void InitializeAppState()
        {
            // Load features and plugins into the tree view
            FeatureNodeManager.LoadFeatures(treeFeatures);
            PluginManager.LoadPlugins(treeFeatures);
            // Keep a copy of the original nodes for live filtering
            _originalNodes = treeFeatures.Nodes.Cast<TreeNode>()
                .Select(n => (TreeNode)n.Clone())
                .ToArray();
        }

        /// <summary>
        /// Analyze all features/plugins in the tree.
        /// This is called by MainForm's global Analyze button via IMainActions.
        /// </summary>
        public async Task AnalyzeAsync()
        {
            // Clear log output (global logger output is hosted in MainForm)
            Logger.OutputBox?.Clear();

            // Analyze features
            await FeatureNodeManager.AnalyzeAll(treeFeatures.Nodes);

            // Analyze plugins
            await PluginManager.AnalyzeAllPlugins(treeFeatures.Nodes);
        }

        /// <summary>
        /// Fix checked features/plugins in the tree.
        /// This is called by MainForm's global Fix button via IMainActions.
        /// </summary>
        public async Task FixAsync()
        {
            // Fix all checked features
            foreach (TreeNode node in treeFeatures.Nodes)
                await FeatureNodeManager.FixChecked(node);

            // Fix all checked plugins
            foreach (TreeNode node in treeFeatures.Nodes)
                await PluginManager.FixChecked(node);
        }

        /// <summary>
        /// Toggles check state of all nodes in the tree (select all / select none).
        /// MainForm can call this for a global "Selection" menu item.
        /// </summary>
        private bool _treeChecked = false;

        public void ToggleSelection()
        {
            foreach (TreeNode node in treeFeatures.Nodes)
            {
                node.Checked = _treeChecked;

                foreach (TreeNode child in node.Nodes)
                {
                    child.Checked = _treeChecked;

                    foreach (TreeNode grandChild in child.Nodes)
                        grandChild.Checked = _treeChecked;
                }
            }

            _treeChecked = !_treeChecked;
        }

        /// <summary>
        /// Checks or unchecks all child nodes when a parent node is checked/unchecked.
        /// </summary>
        private void treeFeatures_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // Ignore programmatic changes to avoid recursion issues
            if (e.Action == TreeViewAction.Unknown)
                return;

            foreach (TreeNode child in e.Node.Nodes)
                child.Checked = e.Node.Checked;
        }

        private void treeFeatures_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            // Get the node under the mouse cursor
            TreeNode nodeUnderMouse = treeFeatures.GetNodeAt(e.X, e.Y);
            if (nodeUnderMouse == null)
                return;

            treeFeatures.SelectedNode = nodeUnderMouse;

            // Show the context menu at the mouse position
            contextMenuStrip.Show(treeFeatures, e.Location);
        }

        /// <summary>
        /// Context menu: analyze selected node (plugin vs feature logic).
        /// </summary>

        private async void analyzeMarkedFeatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(treeFeatures.SelectedNode is TreeNode selectedNode))
                return;

            Logger.OutputBox?.Clear();
            Logger.Log($"🔎 Analyzing Feature: {selectedNode.Text}", LogLevel.Info);

            // If a leaf node is selected, analyze that plugin directly.
            // If a parent node is selected, analyze recursively.
            if (selectedNode.Nodes.Count == 0)
            {
                await PluginManager.AnalyzePlugin(selectedNode);
            }
            else
            {
                await PluginManager.AnalyzeAll(selectedNode);
            }

            // Perform feature-specific analysis (non-plugin)
            FeatureNodeManager.AnalyzeFeature(selectedNode);
        }

        /// <summary>
        /// Context menu: fix selected node (plugin + feature).
        /// </summary>
        private async void fixMarkedFeatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(treeFeatures.SelectedNode is TreeNode selectedNode))
                return;

            Logger.OutputBox?.Clear();
            Logger.Log($"🔧 Fixing Feature: {selectedNode.Text}", LogLevel.Info);

            // Recursively fix all checked feature nodes (non-plugin)
            await FeatureNodeManager.FixFeature(selectedNode);

            // Recursively fix all checked plugin nodes starting from the selected node
            await PluginManager.FixPlugin(selectedNode);
        }

        /// <summary>
        /// Context menu: restore selected node (plugin restore if available, then feature restore).
        /// </summary>
        private async void restoreMarkedFeatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(treeFeatures.SelectedNode is TreeNode selectedNode))
                return;

            if (PluginManager.IsPluginNode(selectedNode))
            {
                // Restore the plugin using its Undo command if available
                await PluginManager.RestorePlugin(selectedNode);
            }
            else
            {
                Logger.OutputBox?.Clear();
                Logger.Log($"↩️ Restoring Feature: {selectedNode.Text}", LogLevel.Info);
            }

            // Perform feature-specific restore (non-plugin)
            FeatureNodeManager.RestoreFeature(selectedNode);
        }

        /// <summary>
        /// Context menu: show help for the selected node.
        /// </summary>
        private void helpMarkedFeatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeFeatures.SelectedNode is TreeNode selectedNode)
                FeatureNodeManager.ShowHelp(selectedNode);
        }

        // ---------------- SearchInterface ----------------

        public void ApplySearch(string query)
        {
            if (_originalNodes == null)
                return;

            string q = (query ?? string.Empty).Trim();

            treeFeatures.BeginUpdate();
            try
            {
                treeFeatures.Nodes.Clear();

                // Empty query = restore original tree
                if (q.Length == 0)
                {
                    foreach (var n in _originalNodes.Select(n => (TreeNode)n.Clone()))
                        treeFeatures.Nodes.Add(n);

                    treeFeatures.ExpandAll();
                    if (treeFeatures.Nodes.Count > 0) treeFeatures.TopNode = treeFeatures.Nodes[0];
                    return;
                }

                // Build filtered tree (keeps parent path)
                foreach (var root in _originalNodes)
                {
                    var filtered = FilterNode(root, q);
                    if (filtered != null)
                        treeFeatures.Nodes.Add(filtered);
                }

                treeFeatures.ExpandAll();
                if (treeFeatures.Nodes.Count > 0) treeFeatures.TopNode = treeFeatures.Nodes[0];
            }
            finally
            {
                treeFeatures.EndUpdate();
            }
        }

        /// <summary>
        /// Returns a filtered clone of the node (and matching children), or null if nothing matches.
        /// Keeps parent nodes if any descendant matches.
        /// </summary>
        private TreeNode FilterNode(TreeNode source, string query)
        {
            bool selfMatch = source.Text?.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;

            // Clone node without children first
            var clone = new TreeNode(source.Text)
            {
                Name = source.Name,
                Tag = source.Tag,
                Checked = source.Checked,
                ImageKey = source.ImageKey,
                SelectedImageKey = source.SelectedImageKey
            };

            // Copy formatting if you use it
            clone.NodeFont = source.NodeFont;
            clone.ForeColor = source.ForeColor;

            // Recursively add matching children
            foreach (TreeNode child in source.Nodes)
            {
                var filteredChild = FilterNode(child, query);
                if (filteredChild != null)
                    clone.Nodes.Add(filteredChild);
            }

            // Keep node if it matches itself OR any child matched
            return (selfMatch || clone.Nodes.Count > 0) ? clone : null;
        }

        // ---------------- IView Interface ----------------
        public void RefreshView()
        {
            InitializeAppState();
            Logger.Clear();
        }

        // ---------------- Import / Export ----------------

        public void ExportSelection(string filePath) => TreeSelectionTransferV1.ExportChecked(filePath, treeFeatures);

        public int ImportSelection(string filePath)
        {
            // Import checked nodes from file (clears first)
            TreeSelectionTransferV1.ImportChecked(filePath, treeFeatures, clearFirst: true);

            // Return how many nodes are checked after import (for logging/UI feedback)
            return CountCheckedLeafNodes(treeFeatures.Nodes);
        }

        // Counts checked leaf nodes only (not category/root nodes)
        private int CountCheckedLeafNodes(TreeNodeCollection nodes)
        {
            int count = 0;

            foreach (TreeNode n in nodes)
            {
                if (n.Nodes.Count == 0)
                {
                    // Leaf = actual feature/plugin
                    if (n.Checked) count++;
                }
                else
                {
                    // Parent = category/group
                    count += CountCheckedLeafNodes(n.Nodes);
                }
            }

            return count;
        }

        /// <summary>
        /// Restore all checked features to original state.
        /// </summary>
        public void RestoreSelection()
        {
            // Count how many nodes are currently checked
            int checkedCount = CountCheckedLeafNodes(treeFeatures.Nodes);

            if (checkedCount == 0)
            {
                MessageBox.Show(
                    "No items are selected.\n\nPlease check one or more features/plugins in the tree first.",
                    "Restore selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                $"⚠️ This will restore ONLY the {checkedCount} selected (checked) item(s) to their original state.\n" +
                "Any previous tweaks for those items may be reverted.\n\n" +
                "Do you want to continue?",
                "Restore selected items",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            Logger.OutputBox?.Clear();

            foreach (TreeNode node in treeFeatures.Nodes)
                FeatureNodeManager.RestoreChecked(node);

            Logger.Log($"↩️ Restored {checkedCount} selected item(s).", LogLevel.Info);
        }


    }
}