using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

/// <summary>
/// TreeView selection export/import (text file).
/// Exports ONLY checked nodes to keep the file small and readable.
///
/// Format:
///   WINSLOP_SELECTION_V1
///   1;Issues/Basic Disk Cleanup
///   1;MS Edge/Disable Start Boost
/// </summary>
public static class TreeSelectionTransferV1
{
    private const string Header = "WINSLOP_SELECTION_V1";
    private const char Sep = ';';
    private const char PathSep = '/';

    // ComboBox actions
    private const string ActionSelect = "<Select profile...>";
    private const string ActionImport = "<Import...>";
    private const string ActionExport = "<Export...>";

    // ---------------- Basic Export / Import ----------------

    public static void ExportChecked(string filePath, TreeView tree)
    {
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("filePath is required.", nameof(filePath));
        if (tree == null) throw new ArgumentNullException(nameof(tree));

        using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            writer.WriteLine(Header);

            // Write only checked nodes (leaves and/or parents whatever is checked)
            foreach (TreeNode node in EnumerateNodes(tree.Nodes).Where(n => n.Checked))
            {
                writer.WriteLine($"1{Sep}{GetNormalizedNodePath(node)}");
            }
        }
    }

    public static void ImportChecked(string filePath, TreeView tree, bool clearFirst = true)
    {
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("filePath is required.", nameof(filePath));
        if (tree == null) throw new ArgumentNullException(nameof(tree));
        if (!File.Exists(filePath)) throw new FileNotFoundException("Selection file not found.", filePath);

        var lines = File.ReadAllLines(filePath, Encoding.UTF8);
        if (lines.Length == 0 || !string.Equals(lines[0].Trim(), Header, StringComparison.Ordinal))
            throw new InvalidOperationException("Invalid selection file format.");

        // Build lookup: normalized path > node
        var lookup = EnumerateNodes(tree.Nodes)
            .ToDictionary(n => GetNormalizedNodePath(n), n => n, StringComparer.OrdinalIgnoreCase);

        tree.BeginUpdate();
        try
        {
            if (clearFirst)
            {
                foreach (TreeNode n in EnumerateNodes(tree.Nodes))
                    n.Checked = false;
            }

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (line.Length == 0) continue;

                int sepIndex = line.IndexOf(Sep);
                if (sepIndex <= 0) continue;

                // Expect: "1;path"
                string flag = line.Substring(0, sepIndex).Trim();
                if (flag != "1") continue;

                string path = NormalizePath(line.Substring(sepIndex + 1));

                if (lookup.TryGetValue(path, out var node))
                    node.Checked = true;
            }
        }
        finally
        {
            tree.EndUpdate();
        }
    }

    // ---------------- ComboBox Profiles ----------------

    private sealed class ComboState
    {
        public string Directory;
        public TreeView Tree;
        public Form Owner;
        public bool ClearFirst;
        public Action<string> Log;
        public Action AfterApply;       // e.g. sync search baseline in FeaturesView
        public bool IsInternalUpdate;
    }

    /// <summary>
    /// Wires a regular ComboBox as a profile switcher for *.sel files in a directory.
    /// Items: Import...;Export...;then all *.sel profile names.
    /// Selecting a profile imports it immediately.
    /// </summary>
    public static void WireProfileComboBox(
        ComboBox combo,
        TreeView tree,
        Form owner,
        string directory = null,
        bool clearFirst = true,
        Action<string> log = null,
        Action afterApply = null)
    {
        if (combo == null) throw new ArgumentNullException(nameof(combo));
        if (tree == null) throw new ArgumentNullException(nameof(tree));
        if (owner == null) throw new ArgumentNullException(nameof(owner));

        var state = new ComboState
        {
            Directory = directory ?? AppDomain.CurrentDomain.BaseDirectory,
            Tree = tree,
            Owner = owner,
            ClearFirst = clearFirst,
            Log = log,
            AfterApply = afterApply
        };

        combo.Tag = state;
        combo.DropDownStyle = ComboBoxStyle.DropDownList;

        combo.SelectedIndexChanged -= Combo_SelectedIndexChanged;
        combo.SelectedIndexChanged += Combo_SelectedIndexChanged;

        RefreshProfileComboBox(combo, keepSelection: false);
    }

    public static void RefreshProfileComboBox(ComboBox combo, bool keepSelection = true)
    {
        if (combo == null) throw new ArgumentNullException(nameof(combo));

        var state = combo.Tag as ComboState;
        if (state == null) throw new InvalidOperationException("ComboBox is not wired. Call WireProfileComboBox first.");

        var prev = keepSelection ? combo.SelectedItem as string : null;

        state.IsInternalUpdate = true;
        combo.BeginUpdate();
        try
        {
            combo.Items.Clear();
            combo.Items.Add(ActionSelect);
            combo.Items.Add(ActionImport);
            combo.Items.Add(ActionExport);

            if (Directory.Exists(state.Directory))
            {
                var profiles = Directory.GetFiles(state.Directory, "*.sel")
                    .Select(Path.GetFileNameWithoutExtension)
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(n => n, StringComparer.OrdinalIgnoreCase);

                foreach (var p in profiles)
                    combo.Items.Add(p);
            }

            //if (keepSelection && !string.IsNullOrWhiteSpace(prev) && combo.Items.Contains(prev))
            //    combo.SelectedItem = prev;
            //else
            //    combo.SelectedIndex = combo.Items.Count > 2 ? 2 : 0; // first real profile, else Import

            // Always default to "Select..." so nothing auto-runs
            combo.SelectedIndex = 0;

        }
        finally
        {
            combo.EndUpdate();
            state.IsInternalUpdate = false;
        }
    }

    private static void Combo_SelectedIndexChanged(object sender, EventArgs e)
    {
        var combo = sender as ComboBox;
        if (combo == null) return;

        var state = combo.Tag as ComboState;
        if (state == null) return;
        if (state.IsInternalUpdate) return;

        var selected = combo.SelectedItem as string;
        if (string.IsNullOrWhiteSpace(selected)) return;

        try
        {
            if (string.Equals(selected, ActionSelect, StringComparison.Ordinal))
                return;

            if (string.Equals(selected, ActionImport, StringComparison.Ordinal))
            {
                DoImportDialog(combo, state);
                return;
            }

            if (string.Equals(selected, ActionExport, StringComparison.Ordinal))
            {
                DoExportDialog(combo, state);
                return;
            }

            // Profile selected
            var path = Path.Combine(state.Directory, selected + ".sel");
            if (!File.Exists(path))
            {
                state.Log?.Invoke("⚠ Profile file not found: " + path);
                return;
            }

            ImportChecked(path, state.Tree, state.ClearFirst);
            state.Log?.Invoke("✅ Profile loaded: " + Path.GetFileName(path));
            state.AfterApply?.Invoke();
        }
        catch (Exception ex)
        {
            MessageBox.Show(state.Owner, ex.Message, "Profile action failed",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static void DoImportDialog(ComboBox combo, ComboState state)
    {
        using (var dlg = new OpenFileDialog())
        {
            dlg.Title = "Import selection";
            dlg.InitialDirectory = state.Directory;
            dlg.Filter = "Selection files (*.sel)|*.sel|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            dlg.Multiselect = false;

            if (dlg.ShowDialog(state.Owner) != DialogResult.OK)
                return;

            ImportChecked(dlg.FileName, state.Tree, state.ClearFirst);
            state.Log?.Invoke("✅ Selection imported: " + Path.GetFileName(dlg.FileName));
            state.AfterApply?.Invoke();

            RefreshProfileComboBox(combo, keepSelection: false);

            var name = Path.GetFileNameWithoutExtension(dlg.FileName);
            if (!string.IsNullOrWhiteSpace(name) && combo.Items.Contains(name))
            {
                state.IsInternalUpdate = true;
                try { combo.SelectedItem = name; }
                finally { state.IsInternalUpdate = false; }
            }
        }
    }

    private static void DoExportDialog(ComboBox combo, ComboState state)
    {
        var current = combo.SelectedItem as string;
        var suggested = (!string.IsNullOrWhiteSpace(current) &&
                         !string.Equals(current, ActionImport, StringComparison.Ordinal) &&
                         !string.Equals(current, ActionExport, StringComparison.Ordinal))
            ? (current + ".sel")
            : "winslop-selection.sel";

        using (var dlg = new SaveFileDialog())
        {
            dlg.Title = "Export selection";
            dlg.InitialDirectory = state.Directory;
            dlg.Filter = "Selection files (*.sel)|*.sel|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            dlg.DefaultExt = "sel";
            dlg.AddExtension = true;
            dlg.FileName = suggested;

            if (dlg.ShowDialog(state.Owner) != DialogResult.OK)
                return;

            ExportChecked(dlg.FileName, state.Tree);
            state.Log?.Invoke("✅ Selection exported: " + Path.GetFileName(dlg.FileName));
            state.AfterApply?.Invoke();

            RefreshProfileComboBox(combo, keepSelection: false);

            var name = Path.GetFileNameWithoutExtension(dlg.FileName);
            if (!string.IsNullOrWhiteSpace(name) && combo.Items.Contains(name))
            {
                state.IsInternalUpdate = true;
                try { combo.SelectedItem = name; }
                finally { state.IsInternalUpdate = false; }
            }
        }
    }

    // ---------------- Helpers ----------------

    private static IEnumerable<TreeNode> EnumerateNodes(TreeNodeCollection nodes)
    {
        foreach (TreeNode n in nodes)
        {
            yield return n;
            foreach (var c in EnumerateNodes(n.Nodes))
                yield return c;
        }
    }

    private static string GetNormalizedNodePath(TreeNode node)
    {
        // Build a stable path from root to leaf: Root/Child/Leaf
        var stack = new Stack<string>();
        var cur = node;
        while (cur != null)
        {
            stack.Push((cur.Text ?? string.Empty).Trim());
            cur = cur.Parent;
        }
        return string.Join(PathSep.ToString(), stack);
    }

    private static string NormalizePath(string raw)
    {
        // Normalize user file input: trim and trim each segment
        var parts = (raw ?? string.Empty)
            .Trim()
            .Split(new[] { PathSep }, StringSplitOptions.None)
            .Select(p => p.Trim())
            .Where(p => p.Length > 0);

        return string.Join(PathSep.ToString(), parts);
    }
}