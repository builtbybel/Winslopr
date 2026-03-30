using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Winslop.Helpers;

namespace Winslop
{
    public class PluginEntry : INotifyPropertyChanged
    {
        private bool _isChecked;
        private string _installedText = "";
        private SolidColorBrush _installedColor = new(Colors.Gray);

        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Url { get; set; } = "";
        public string Type { get; set; } = "";

        public bool IsChecked
        {
            get => _isChecked;
            set { if (_isChecked != value) { _isChecked = value; OnPropertyChanged(); } }
        }

        public string InstalledText
        {
            get => _installedText;
            set { if (_installedText != value) { _installedText = value; OnPropertyChanged(); } }
        }

        public SolidColorBrush InstalledColor
        {
            get => _installedColor;
            set { if (_installedColor != value) { _installedColor = value; OnPropertyChanged(); } }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public sealed partial class PluginsDialog : ContentDialog
    {
        private const string ManifestUrl =
            "https://raw.githubusercontent.com/builtbybel/Winslop/main/plugins/plugins_manifest.txt";

        // Shared HttpClient — never create one per request
        private static readonly HttpClient _http = new();

        private readonly string _pluginsFolder;
        private List<PluginEntry> _allPlugins = new();
        private readonly List<PluginEntry> _visiblePlugins = new();
        private readonly HashSet<string> _installedFiles = new(StringComparer.OrdinalIgnoreCase);

        public PluginsDialog()
        {
            InitializeComponent();
            _pluginsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

            // Load plugins once the dialog is loaded
            Loaded += async (_, _) => await LoadPluginsAsync();
        }

        // -- Load -------------------------------------------------

        private async Task LoadPluginsAsync()
        {
            LoadInstalledFiles();
            try
            {
                string content = await _http.GetStringAsync(ManifestUrl);
                _allPlugins = ParseManifest(content);
            }
            catch (Exception ex)
            {
                _allPlugins = new List<PluginEntry>();
                txtDescription.Text = "Error loading manifest: " + ex.Message;
            }
            UpdateList();
        }

        // Scan the plugins folder and cache all installed file names
        private void LoadInstalledFiles()
        {
            _installedFiles.Clear();
            if (!Directory.Exists(_pluginsFolder)) return;
            foreach (var f in Directory.GetFiles(_pluginsFolder))
                _installedFiles.Add(Path.GetFileName(f));
        }

        // -- Manifest parser --------------------------------------

        private static List<PluginEntry> ParseManifest(string content)
        {
            var result = new List<PluginEntry>();
            PluginEntry current = null;
            string currentKey = null;

            foreach (var line in content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None))
            {
                var t = line.Trim();
                if (t.StartsWith('[') && t.EndsWith(']'))
                {
                    if (current != null) result.Add(current);
                    current = new PluginEntry { Name = t[1..^1].Trim() };
                    currentKey = null;
                }
                else if (!string.IsNullOrWhiteSpace(t) && current != null)
                {
                    if (t.Contains('='))
                    {
                        var parts = t.Split('=', 2);
                        switch (parts[0].Trim())
                        {
                            case "description": current.Description = parts[1].Trim(); currentKey = "description"; break;
                            case "url": current.Url = parts[1].Trim(); currentKey = "url"; break;
                            default: currentKey = null; break;
                        }
                    }
                    else if (currentKey == "description")
                        current.Description += "\n" + t;
                }
            }
            if (current != null) result.Add(current);
            return result;
        }

        // -- Update list ------------------------------------------

        private void UpdateList(string query = "")
        {
            _visiblePlugins.Clear();

            foreach (var p in _allPlugins)
            {
                if (query.Length > 0 &&
                    !p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) &&
                    !(p.Description?.Contains(query, StringComparison.OrdinalIgnoreCase) == true))
                    continue;

                var fileName = Path.GetFileName(p.Url);
                bool installed = _installedFiles.Contains(fileName);
                p.InstalledText = installed ? Localizer.Get("Plugins_Installed") : "";
                p.InstalledColor = Brush(installed ? Microsoft.UI.Colors.Green : Microsoft.UI.Colors.Gray);
                p.IsChecked = installed;
                p.Type = p.Name.EndsWith("(NX)", StringComparison.OrdinalIgnoreCase) ? "NX"
                    : Path.GetExtension(p.Url).Equals(".ps1", StringComparison.OrdinalIgnoreCase) ? "PS1"
                    : "Other";

                _visiblePlugins.Add(p);
            }

            listPlugins.ItemsSource = null;
            listPlugins.ItemsSource = _visiblePlugins;
        }

        private static Microsoft.UI.Xaml.Media.SolidColorBrush Brush(Windows.UI.Color color)
            => new(color);

        // -- Install / Update / Remove ----------------------------

        private async Task InstallPlugins(bool force)
        {
            var toInstall = _visiblePlugins.Where(p => p.IsChecked).ToList();
            if (toInstall.Count == 0) return;

            Directory.CreateDirectory(_pluginsFolder);
            progressBar.Visibility = Visibility.Visible;
            progressBar.Maximum = toInstall.Count;
            progressBar.Value = 0;

            foreach (var p in toInstall)
            {
                var fileName = Path.GetFileName(p.Url);
                var filePath = Path.Combine(_pluginsFolder, fileName);

                if (!force && File.Exists(filePath)) { progressBar.Value++; continue; }

                try
                {
                    var data = await _http.GetByteArrayAsync(p.Url);
                    await File.WriteAllBytesAsync(filePath, data);
                    _installedFiles.Add(fileName);
                    p.InstalledText = Localizer.Get("Plugins_Installed");
                    p.InstalledColor = Brush(Microsoft.UI.Colors.Green);
                }
                catch { /* skip failed downloads silently */ }

                progressBar.Value++;
            }

            progressBar.Visibility = Visibility.Collapsed;
        }

        private async Task UpdateAll()
        {
            foreach (var p in _visiblePlugins) p.IsChecked = true;
            await InstallPlugins(force: true);
            foreach (var p in _visiblePlugins) p.InstalledText = Localizer.Get("Plugins_Updated");
        }

        private void RemoveChecked()
        {
            foreach (var p in _visiblePlugins.Where(p => p.IsChecked).ToList())
            {
                var filePath = Path.Combine(_pluginsFolder, Path.GetFileName(p.Url));
                if (File.Exists(filePath)) File.Delete(filePath);
                _installedFiles.Remove(Path.GetFileName(p.Url));
                p.InstalledText = "";
                p.InstalledColor = Brush(Microsoft.UI.Colors.Gray);
                p.IsChecked = false;
            }
        }

        // -- UI handlers ------------------------------------------

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
            => UpdateList(txtSearch.Text.Trim());

        private void listPlugins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listPlugins.SelectedItem is PluginEntry p)
                txtDescription.Text = p.Description ?? "";
        }

        private async void btnInstall_Click(object sender, RoutedEventArgs e) => await InstallPlugins(force: false);

        private async void btnUpdateAll_Click(object sender, RoutedEventArgs e) => await UpdateAll();

        private void btnRemove_Click(object sender, RoutedEventArgs e) => RemoveChecked();

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (listPlugins.SelectedItem is not PluginEntry plugin) return;
            var path = Path.Combine(_pluginsFolder, Path.GetFileName(plugin.Url));
            if (!File.Exists(path)) return;
            OpenProcess("notepad.exe", $"\"{path}\"");
        }

        private void btnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(_pluginsFolder);
            OpenProcess(_pluginsFolder);
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
            => OpenProcess("https://github.com/builtbybel/Winslop/blob/main/plugins/plugins_manifest.txt");

        private static void OpenProcess(string target, string args = null)
            => System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = target,
                Arguments = args ?? "",
                UseShellExecute = true
            });
    }
}