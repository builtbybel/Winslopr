using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Winslop.Extensions
{
    public partial class ExtensionsView : UserControl, ISearchable, IView
    {
        private ExtensionsCategory _category = ExtensionsCategory.All;

        // Full list loaded from scripts
        private readonly List<ExtensionsDefinition> _allTools = new List<ExtensionsDefinition>();

        // Filtered list shown in the ListBox
        private readonly BindingList<ExtensionsDefinition> _visibleTools = new BindingList<ExtensionsDefinition>();

        private readonly BindingSource _bs = new BindingSource();

        private string _searchQuery = string.Empty;

        public ExtensionsView(ExtensionsCategory category = ExtensionsCategory.All)
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            _category = category;

            // Setup filter dropdown
            comboFilter.Items.Clear();
            comboFilter.Items.AddRange(new object[] { "All", "Tool", "Pre", "Mid", "Post" });

            comboFilter.SelectedIndexChanged -= comboFilter_SelectedIndexChanged;
            comboFilter.SelectedIndex = 0;
            comboFilter.SelectedIndexChanged += comboFilter_SelectedIndexChanged;

            // Setup ListBox binding (left side)
            _bs.DataSource = _visibleTools;
            listTools.DataSource = _bs;
            listTools.DisplayMember = nameof(ExtensionsDefinition.Title);
            listTools.SelectedIndexChanged += listTools_SelectedIndexChanged;

            // Create the right panel (details) in code
            //CreateDetailsPanel();

            // Hook uninstall event
            detailsControl.ToolUninstalled += (s, e) => LoadToolsAsync();

            // Start with empty detailsControl panel
            detailsControl.SetTool(null);

            // Load scripts
            LoadToolsAsync();
        }

        ///// <summary>
        ///// Creates the detailsControl panel and docks it to fill remaining space.
        ///// This avoids needing to drag custom controls in the designer.
        ///// </summary>
        //private void CreatedetailsControlPanel()
        //{
        //    detailsControl = new ExtensionsItemControl();
        //    detailsControl.Dock = DockStyle.Right;

        //    // Add after listTools so it fills the remaining area
        //    Controls.Add(detailsControl);
        //    detailsControl.BringToFront();

        //    // When user uninstalls, reload list
        //    detailsControl.ToolUninstalled += (s, e) => LoadToolsAsync();

        //    // Start with nothing selected
        //    detailsControl.SetTool(null);
        //}

        private async void LoadToolsAsync()
        {
            lblStatus.Visible = true;
            lblStatus.Text = "Loading extensions...";

            _allTools.Clear();
            _visibleTools.Clear();
            detailsControl.SetTool(null);

            string scriptDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scripts");

            if (!Directory.Exists(scriptDirectory))
            {
                Directory.CreateDirectory(scriptDirectory);
                lblStatus.Text = "Scripts folder was missing, created it.";
                return;
            }

            // Get scripts
            string[] scriptFiles = await Task.Run(() => Directory.GetFiles(scriptDirectory, "*.ps1"));

            // Parse metadata in background
            var loadedTools = await Task.Run(() =>
            {
                var list = new List<ExtensionsDefinition>();

                foreach (var scriptPath in scriptFiles)
                {
                    string title = Path.GetFileNameWithoutExtension(scriptPath);
                    string icon = PickIconForScript(title);
                    var meta = ReadMetadataFromScript(scriptPath);

                    var tool = new ExtensionsDefinition(title, meta.description, icon, scriptPath)
                    {
                        Category = meta.category,
                        UseConsole = meta.useConsole,
                        UseLog = meta.useLog,
                        SupportsInput = meta.inputEnabled,
                        InputPlaceholder = meta.inputPh,
                        PoweredByText = meta.poweredByText,
                        PoweredByUrl = meta.poweredByUrl
                    };

                    tool.Options.AddRange(meta.options);
                    list.Add(tool);
                }

                return list;
            });

            _allTools.AddRange(loadedTools);

            ApplyFilterAndSearch();

            lblStatus.Visible = false;

            // Auto-select first item
            if (listTools.Items.Count > 0 && listTools.SelectedIndex < 0)
                listTools.SelectedIndex = 0;
        }

        private void listTools_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tool = listTools.SelectedItem as ExtensionsDefinition;
            detailsControl.SetTool(tool);
        }

        private void comboFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboFilter.SelectedItem?.ToString())
            {
                case "Tool": _category = ExtensionsCategory.Tool; break;
                case "Pre": _category = ExtensionsCategory.Pre; break;
                case "Mid": _category = ExtensionsCategory.Mid; break;
                case "Post": _category = ExtensionsCategory.Post; break;
                default: _category = ExtensionsCategory.All; break;
            }

            ApplyFilterAndSearch();
        }

        public void ApplySearch(string query)
        {
            _searchQuery = query ?? string.Empty;
            ApplyFilterAndSearch();
        }

        /// <summary>
        /// Applies category + search filters and refreshes the left ListBox.
        /// </summary>
        private void ApplyFilterAndSearch()
        {
            string q = (_searchQuery ?? "").Trim().ToLowerInvariant();

            var filtered = _allTools
                .Where(t =>
                    (_category == ExtensionsCategory.All || t.Category == _category) &&
                    (string.IsNullOrEmpty(q) ||
                     (t.Title ?? "").ToLowerInvariant().Contains(q) ||
                     (t.Description ?? "").ToLowerInvariant().Contains(q)))
                .OrderBy(t => t.Title)
                .ToList();

            _visibleTools.RaiseListChangedEvents = false;
            _visibleTools.Clear();
            foreach (var t in filtered)
                _visibleTools.Add(t);
            _visibleTools.RaiseListChangedEvents = true;
            _visibleTools.ResetBindings();

            // If nothing matches, clear detailsControl panel
            if (_visibleTools.Count == 0)
            {
                detailsControl.SetTool(null);
            }
        }

        public void RefreshView()
        {
            Logger.Clear();
            LoadToolsAsync();
        }

        // ---------------- Metadata parsing ----------------

        private (string description,
                 List<string> options,
                 ExtensionsCategory category,
                 bool useConsole,
                 bool useLog,
                 bool inputEnabled,
                 string inputPh,
                 string poweredByText,
                 string poweredByUrl)
            ReadMetadataFromScript(string scriptPath)
        {
            string description = "No description available.";
            var options = new List<string>();
            ExtensionsCategory category = ExtensionsCategory.All;
            bool useConsole = false;
            bool useLog = false;
            bool inputEnabled = false;
            string inputPh = string.Empty;
            string poweredByText = string.Empty;
            string poweredByUrl = string.Empty;

            try
            {
                // Only scan first lines for metadata
                var lines = File.ReadLines(scriptPath).Take(15);

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    if (line.StartsWith("# Description:", StringComparison.OrdinalIgnoreCase))
                    {
                        description = line.Substring(14).Trim();
                    }
                    else if (line.StartsWith("# Category:", StringComparison.OrdinalIgnoreCase))
                    {
                        string raw = line.Substring(11).Trim().ToLowerInvariant();
                        if (raw == "pre") category = ExtensionsCategory.Pre;
                        else if (raw == "mid") category = ExtensionsCategory.Mid;
                        else if (raw == "tool") category = ExtensionsCategory.Tool;
                        else if (raw == "post") category = ExtensionsCategory.Post;
                        else category = ExtensionsCategory.All;
                    }
                    else if (line.StartsWith("# Options:", StringComparison.OrdinalIgnoreCase))
                    {
                        options = line.Substring(10)
                            .Split(';')
                            .Select(x => x.Trim())
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToList();
                    }
                    else if (line.StartsWith("# Host:", StringComparison.OrdinalIgnoreCase))
                    {
                        var raw = line.Substring(7).Trim().ToLowerInvariant();
                        if (raw == "console") useConsole = true;
                        else if (raw == "log") useLog = true;
                    }
                    else if (line.StartsWith("# Input:", StringComparison.OrdinalIgnoreCase))
                    {
                        var raw = line.Substring(8).Trim().ToLowerInvariant();
                        inputEnabled = (raw == "true" || raw == "yes" || raw == "1");
                    }
                    else if (line.StartsWith("# InputPlaceholder:", StringComparison.OrdinalIgnoreCase))
                    {
                        inputPh = line.Substring(19).Trim();
                    }
                    else if (line.StartsWith("# PoweredBy:", StringComparison.OrdinalIgnoreCase))
                    {
                        poweredByText = line.Substring(12).Trim();
                    }
                    else if (line.StartsWith("# PoweredUrl:", StringComparison.OrdinalIgnoreCase))
                    {
                        poweredByUrl = line.Substring(13).Trim();
                    }
                    else if (line.StartsWith("#"))
                    {
                        if (description == "No description available.")
                            description = line.TrimStart('#').Trim();
                    }
                }
            }
            catch
            {
                // Keep defaults
            }

            return (description, options, category, useConsole, useLog, inputEnabled, inputPh, poweredByText, poweredByUrl);
        }

        private string PickIconForScript(string name)
        {
            name = (name ?? "").ToLowerInvariant();

            if (name.Contains("debloat")) return "\uE74D";
            if (name.Contains("network")) return "\uE701";
            if (name.Contains("explorer")) return "\uE8B7";
            if (name.Contains("update")) return "\uE895";
            if (name.Contains("context")) return "\uE8A5";

            if (name.Contains("backup")) return "\uE8C7";
            if (name.Contains("security")) return "\uE72E";
            if (name.Contains("performance")) return "\uE7B8";
            if (name.Contains("privacy")) return "\uF552";
            if (name.Contains("app")) return "\uED35";
            if (name.Contains("setup")) return "\uE8FB";

            return "\uE7C5";
        }
    }
}