using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Winslop.Views
{
    public partial class PluginsView : UserControl
    {
        private List<PluginEntry> plugins = new List<PluginEntry>();        // All plugins from the manifest, so full squad
        private List<PluginEntry> visiblePlugins = new List<PluginEntry>(); // Plugins currently shown in the UI (filtered or not)
        private HashSet<string> installedPlugins = new HashSet<string>();   // Names of plugins already installed on disk

        private const string manifestUrl = "https://raw.githubusercontent.com/builtbybel/Winslop/main/plugins/plugins_manifest.txt";

        public class PluginEntry
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Url { get; set; }
        }

        public PluginsView()
        {
            InitializeComponent();
        }

        private async void PluginsView_Load(object sender, EventArgs e)
        {
            await LoadPlugins();
        }

        /// <summary>
        /// Loads the currently installed plugins from disk into memory.
        /// </summary>
        private void LoadInstalledPlugins()
        {
            var pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
            if (Directory.Exists(pluginPath))
            {
                foreach (var file in Directory.GetFiles(pluginPath))
                {
                    installedPlugins.Add(Path.GetFileName(file));
                }
            }
        }

        /// <summary>
        /// Loads plugins from the remote manifest and displays them.
        /// </summary>
        private async Task LoadPlugins()
        {
            try
            {
                LoadInstalledPlugins(); // Load the plugins already installed on disk, gotta know whats up

                using (var client = new WebClient())
                {
                    // Download the plugin manifest
                    string content = await Task.Run(() => client.DownloadString(manifestUrl));

                    // Parse the manifest into our plugins list — all the available plugins decoded!
                    plugins = ParseManifest(content);

                    // Sync visiblePlugins with the full list to keep UI and data in perfect harmony
                    visiblePlugins = plugins.ToList();

                    // Update the ListBox so it shows all the plugins we're tracking right now
                    UpdateVisiblePlugins();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading plugins: " + ex.Message);
            }
        }

        /// <summary>
        /// Parses the remote plugin manifest into a list of PluginEntry objects.
        /// </summary>
        private List<PluginEntry> ParseManifest(string content)
        {
            var result = new List<PluginEntry>();
            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            PluginEntry current = null;
            string currentKey = null;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    if (current != null)
                        result.Add(current);

                    var name = trimmedLine.Substring(1, trimmedLine.Length - 2).Trim();
                    current = new PluginEntry { Name = name };
                    currentKey = null;
                }
                else if (!string.IsNullOrWhiteSpace(trimmedLine))
                {
                    if (trimmedLine.Contains("=") && current != null)
                    {
                        var parts = trimmedLine.Split(new[] { '=' }, 2);
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();

                        switch (key)
                        {
                            case "description":
                                current.Description = value;
                                currentKey = "description";
                                break;

                            case "url":
                                current.Url = value;
                                currentKey = "url";
                                break;

                            default:
                                currentKey = null;
                                break;
                        }
                    }
                    else if (currentKey == "description" && current != null)
                    {
                        // This line handles multi-line description values.
                        // If the previous key was "description" and the current line does not contain a new key=value pair,
                        // it is considered a continuation of the description.
                        // We append the new line to the existing description, preserving line breaks with "\n".
                        current.Description += "\n" + trimmedLine;
                    }
                }
            }

            if (current != null)
                result.Add(current);

            return result;
        }

        /// <summary>
        /// Downloads and installs checked plugins.
        /// If force is true, existing files will be overwritten.
        /// </summary>
        private async Task InstallPlugins(bool force = false)
        {
            var checkedItems = listPlugins.CheckedItems.Cast<ListViewItem>().ToList();
            if (checkedItems.Count == 0)
            {
                MessageBox.Show("Please check one or more plugins to download.");
                return;
            }

            string savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
            Directory.CreateDirectory(savePath);

            progressBarDownload.Visible = true;
            progressBarDownload.Value = 0;
            progressBarDownload.Maximum = checkedItems.Count;

            int done = 0;

            using (var client = new WebClient())
            {
                foreach (var item in checkedItems)
                {
                    var plugin = item.Tag as PluginEntry;
                    if (plugin == null)
                        continue;

                    string file = Path.Combine(savePath, Path.GetFileName(plugin.Url));

                    // Skip download if file exists and not forcing overwrite
                    if (!force && File.Exists(file))
                    {
                        progressBarDownload.Value = ++done;
                        continue;
                    }

                    try
                    {
                        await client.DownloadFileTaskAsync(new Uri(plugin.Url), file);
                        installedPlugins.Add(Path.GetFileName(plugin.Url));
                        item.SubItems[1].Text = "Yes";  // Update Installed column
                        //item.Checked = true;                // Ensure checked
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to download {plugin.Name}: {ex.Message}");
                    }

                    progressBarDownload.Value = ++done;
                }
            }

            progressBarDownload.Visible = false;
        }

        private async void btnPluginInstall_Click(object sender, EventArgs e)
        {
            await InstallPlugins(force: false);   // skip installed
        }

        /// <summary>
        /// Updates all plugins by selecting all and force-downloading them.
        /// </summary>
        private async void btnPluginUpdateAll_Click(object sender, EventArgs e)
        {
            // Check all items in the list
            foreach (ListViewItem item in listPlugins.Items)
            {
                item.Checked = true;
            }

            // Force install (overwrite even if already installed)
            await InstallPlugins(force: true);

            // Update the status of all plugins to "Updated"
            foreach (ListViewItem item in listPlugins.Items)
            {
                item.SubItems[1].Text = "Updated";
            }

            MessageBox.Show("All plugins updated.");
        }

        private void btnPluginRemove_Click(object sender, EventArgs e)
        {
            string pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");

            var checkedItems = listPlugins.CheckedItems.Cast<ListViewItem>().ToList();

            if (checkedItems.Count == 0)
            {
                MessageBox.Show("No plugins selected.");
                return;
            }

            foreach (var item in checkedItems)
            {
                var plugin = item.Tag as PluginEntry;
                if (plugin == null)
                    continue;

                string path = Path.Combine(pluginPath, Path.GetFileName(plugin.Url));

                if (File.Exists(path))
                    File.Delete(path);

                installedPlugins.Remove(Path.GetFileName(plugin.Url));
                item.SubItems[1].Text = "No";
                item.Checked = false;
            }

            MessageBox.Show("Selected plugins removed.");
        }

        private void btnPluginEdit_Click(object sender, EventArgs e)
        {
            if (listPlugins.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a plugin first.");
                return;
            }

            var plugin = listPlugins.SelectedItems[0].Tag as PluginEntry;
            if (plugin == null) return;

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins", Path.GetFileName(plugin.Url));

            if (!File.Exists(path))
            {
                MessageBox.Show("Plugin file not found. Please install the plugin first.");
                return;
            }

            try
            {
                var ext = Path.GetExtension(path).ToLower();
                var editor = ext == ".ps1" ? "powershell_ise.exe" : "notepad.exe";
                Process.Start(editor, $"\"{path}\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open plugin: " + ex.Message);
            }
        }

        /// <summary>
        /// Shows plugin information in a MessageBox.
        /// </summary>
        private void btnHelp_Click(object sender, EventArgs e)
        {
            if (listPlugins.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a plugin first.");
                return;
            }

            var plugin = listPlugins.SelectedItems[0].Tag as PluginEntry;
            if (plugin != null)
            {
                MessageBox.Show(plugin.Description, $"Info: {plugin.Name}");
            }
        }

        private void listPlugins_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listPlugins.SelectedItems.Count == 0)
                return;

            var plugin = listPlugins.SelectedItems[0].Tag as PluginEntry;
            if (plugin != null)
            {
                btnDescription.Text = plugin.Description;
            }
        }

        /// <summary>
        /// Updates the ListView with filtered plugin entries,
        /// including name, install status, and plugin type.
        /// Type is determined by plugin name ("(NX)" means native plugin),
        /// or file extension (.ps1 = Powershell; others = Other).
        /// </summary>
        private void UpdateVisiblePlugins(string query = "")
        {
            visiblePlugins = plugins
                .Where(p =>
                    p.Name.ToLower().Contains(query) ||
                    (p.Description?.ToLower() ?? "").Contains(query))
                .ToList();

            listPlugins.Items.Clear();

            foreach (var plugin in visiblePlugins)
            {
                var fileName = Path.GetFileName(plugin.Url);
                bool isInstalled = installedPlugins.Contains(fileName);

                // Determine plugin type
                string type;
                if (plugin.Name.Trim().EndsWith("(NX)", StringComparison.OrdinalIgnoreCase))
                {
                    type = "NX";
                }
                else if (Path.GetExtension(plugin.Url).Equals(".ps1", StringComparison.OrdinalIgnoreCase))
                {
                    type = "Powershell";
                }
                else
                {
                    type = "Other";
                }

                var item = new ListViewItem(plugin.Name);
                item.SubItems.Add(isInstalled ? "Yes" : "No");
                item.SubItems.Add(type);
                item.Tag = plugin;
                item.Checked = isInstalled;

                listPlugins.Items.Add(item);
            }

            // Auto-resize columns to fit header and content
            foreach (ColumnHeader column in listPlugins.Columns)
            {
                column.Width = -2;
            }
        }

        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            string query = textSearch.Text.Trim().ToLower();
            UpdateVisiblePlugins(query);
        }

        private void textSearch_Click(object sender, EventArgs e)
        {
            textSearch.Text = string.Empty;
        }

        private void btnPluginSubmit_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://github.com/builtbybel/Winslop/blob/main/plugins/plugins_manifest.txt",
                UseShellExecute = true
            });
        }

        private void linkPluginUsage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/builtbybel/Winslop/blob/main/docs/plugins.md");
        }
    }
}