using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Winslop.Views
{
    public partial class InstallView : UserControl, IMainActions, ISearchable, IView
    {
        private const string AppsFileName = "winget-apps.ini";

        // All apps from file
        private readonly List<AppEntry> _allApps = new List<AppEntry>();

        // Current filtered view
        private List<AppEntry> _filteredApps = new List<AppEntry>();

        // Preserve checked state across filtering by WingetId
        private readonly HashSet<string> _checkedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Status (filled by AnalyzeAsync)
        private readonly HashSet<string> _installedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private readonly HashSet<string> _upgradeIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private bool _isBusy;

        // Base texts for checkboxes to which counts will be appended
        private const string InstalledOnlyLabel = "Installed";

        private const string UpgradeableLabel = "Upgradeable";

        public InstallView()
        {
            InitializeComponent();

            // Show installed apps only when the checkbox is checked
            chkInstalledOnly.CheckedChanged += (s, e) => ApplyAppsFilter(_lastSearchQuery);
            // Show apps with available upgrades only when the checkbox is checked
            chkUpgradesOnly.CheckedChanged += (s, e) => ApplyAppsFilter(_lastSearchQuery);
            // Wire events for existing designer controls
            comboDisplayMode.SelectedIndexChanged += (s, e) => ApplyAppsFilter(_lastSearchQuery);

            btnInstall.Click += BtnInstall_Click;
            btnUpgradeSelected.Click += BtnUpgradeSelected_Click;
            btnUpgradeAll.Click += BtnUpgradeAll_Click;
            btnUninstall.Click += BtnUninstall_Click;

            // Load and render
            LoadAppsFromWingetTxt();
            PopulateCategories();
            ApplyAppsFilter("");

            // Analyze on startup to check installed/upgradable apps (optional, can be triggered manually by "Inspect system"/Refresh button)
            if (!DesignMode)
                _ = AnalyzeAsync();
        }

        private string _lastSearchQuery = "";

        // ---------------- ISearchable ----------------

        /// <summary>
        /// Applies search to the apps list (empty query = reset).
        /// Preserves selection state for items that remain visible.
        /// </summary>
        public void ApplySearch(string query)
        {
            ApplyAppsFilter(query);
        }

        // ---------------- IView ----------------

        /// <summary>
        /// Re-analyze apps when refreshing the view.
        /// </summary>
        public void RefreshView()
        {
            // Fire-and-forget is okay for UI refresh
            _ = AnalyzeAsync();
            Logger.Clear();
        }

        // ---------------- IMainActions ----------------

        /// <summary>
        /// Analyze installed packages and check for upgrades using winget list/upgrade.
        /// </summary>
        public async Task AnalyzeAsync()
        {
            if (_isBusy) return;
            SetBusy(true);

            try
            {
                Logger.BeginSection("Winget Analyze");

                _installedIds.Clear();
                _upgradeIds.Clear();

                Logger.Log("Running: winget list", LogLevel.Info);
                var listLines = await RunWingetCaptureLinesAsync("list");

                foreach (var a in _allApps)
                {
                    if (listLines.Any(l => l.IndexOf(a.WingetId, StringComparison.OrdinalIgnoreCase) >= 0))
                        _installedIds.Add(a.WingetId);
                }

                Logger.Log("Running: winget upgrade (to check for available app updates)", LogLevel.Info);
                var upgradeLines = await RunWingetCaptureLinesAsync("upgrade");

                foreach (var a in _allApps)
                {
                    if (upgradeLines.Any(l => l.IndexOf(a.WingetId, StringComparison.OrdinalIgnoreCase) >= 0))
                        _upgradeIds.Add(a.WingetId);
                }

                foreach (var a in _allApps)
                {
                    a.IsInstalled = _installedIds.Contains(a.WingetId);
                    a.HasUpgrade = _upgradeIds.Contains(a.WingetId);
                }
                UpdateCheckboxCounts(); // Update counts in filter checkboxes

                Logger.Log("Analyze finished.", LogLevel.Info);

                Logger.BeginSection("Ready");
                // Summary counts
                int installedCount = _allApps.Count(x => x.IsInstalled);
                int updatesAvailableCount = _allApps.Count(x => x.HasUpgrade);

                Logger.Log($"Installed (from catalog): {installedCount}/{_allApps.Count}", LogLevel.Info);
                Logger.Log($"Updates available (from catalog): {updatesAvailableCount}", updatesAvailableCount > 0 ? LogLevel.Warning : LogLevel.Info);

                Logger.Log("Apps loaded. Tick apps in the list and click 'Apply selected changes' to install apps.", LogLevel.Info);
                Logger.Log("'Inspect system' checks installed apps + available updates.", LogLevel.Info);
                ApplyAppsFilter(_lastSearchQuery);
            }
            catch (Exception ex)
            {
                Logger.Log("Analyze failed: " + ex.Message, LogLevel.Error);
            }
            finally
            {
                SetBusy(false);
            }
        }

        /// <summary>
        /// Fix/apply action:
        /// - For selected apps: install if not installed, else upgrade if upgrade available.
        /// </summary>
        public async Task FixAsync()
        {
            var apps = GetSelectedApps();
            if (apps.Count == 0) return;

            if (_isBusy) return;
            SetBusy(true);

            try
            {
                Logger.BeginSection("Winget Fix");

                foreach (var a in apps)
                {
                    if (!a.IsInstalled)
                        await InstallOneAsync(a);
                    else if (a.HasUpgrade)
                        await UpgradeOneAsync(a);
                    else
                        Logger.Log($"Skip (no action): {a.Name} ({a.WingetId})", LogLevel.Info);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Fix failed: " + ex.Message, LogLevel.Error);
            }
            finally
            {
                SetBusy(false);
                await AnalyzeAsync();
            }
        }

        // Count and display the number of installed and upgradable apps in the respective filter checkboxes.
        private void UpdateCheckboxCounts()
        {
            int installedCount = _allApps.Count(a => a.IsInstalled);
            int updatesCount = _allApps.Count(a => a.IsInstalled && a.HasUpgrade);

            // Always show counts
            chkInstalledOnly.Text = $"{InstalledOnlyLabel} ({installedCount})";
            chkUpgradesOnly.Text = $"{UpgradeableLabel} ({updatesCount})";

            // Upgradeable (show count only if > 0) + color hint
            chkUpgradesOnly.Text = updatesCount > 0
                ? $"{UpgradeableLabel} ({updatesCount})"
                : UpgradeableLabel;

            chkUpgradesOnly.ForeColor = updatesCount > 0
                ? System.Drawing.Color.OrangeRed
                : System.Drawing.Color.Black;
        }

        // ---------------- Filtering / Rendering ----------------

        private void ApplyAppsFilter(string query)
        {
            _lastSearchQuery = (query ?? "").Trim();

            // Preserve selection before changing list content
            RememberSelectionFromListBox();

            var cat = (comboDisplayMode.SelectedItem as string) ?? "All";
            IEnumerable<AppEntry> q = _allApps;

            // Category filter
            if (!string.Equals(cat, "All", StringComparison.OrdinalIgnoreCase))
                q = q.Where(a => string.Equals(a.Category, cat, StringComparison.OrdinalIgnoreCase));

            // Installed-only filter
            if (chkInstalledOnly.Checked)
                q = q.Where(a => a.IsInstalled);

            // Updates-only filter
            if (chkUpgradesOnly.Checked)
                q = q.Where(a => a.HasUpgrade);

            // Search filter (by name or winget id)
            if (!string.IsNullOrWhiteSpace(_lastSearchQuery))
            {
                q = q.Where(a =>
                    a.Name.IndexOf(_lastSearchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    a.WingetId.IndexOf(_lastSearchQuery, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            _filteredApps = q.OrderBy(a => a.Name).ToList();
            RenderListBox();
        }

        private void RenderListBox()
        {
            checkedListBoxApps.BeginUpdate();
            try
            {
                checkedListBoxApps.Items.Clear();

                foreach (var a in _filteredApps)
                    checkedListBoxApps.Items.Add(new AppListItem(a, BuildDisplayText(a)));

                // Restore checked state by WingetId
                for (int i = 0; i < checkedListBoxApps.Items.Count; i++)
                {
                    var it = checkedListBoxApps.Items[i] as AppListItem;
                    if (it != null && _checkedIds.Contains(it.Entry.WingetId))
                        checkedListBoxApps.SetItemChecked(i, true);
                }
            }
            finally
            {
                checkedListBoxApps.EndUpdate();
            }
        }

        private string BuildDisplayText(AppEntry a)
        {
            // Example: "7-Zip  [Installed]  [Upgrade]  -  7zip.7zip"
            var sb = new StringBuilder();
            sb.Append(a.Name);

            if (a.IsInstalled) sb.Append("  [Installed]");
            if (a.HasUpgrade) sb.Append("  [Update available]");

            sb.Append("  -  ");
            sb.Append(a.WingetId);
            return sb.ToString();
        }

        private void RememberSelectionFromListBox()
        {
            // preserve checked items
            _checkedIds.Clear();

            for (int i = 0; i < checkedListBoxApps.Items.Count; i++)
            {
                if (!checkedListBoxApps.GetItemChecked(i))
                    continue;

                var item = checkedListBoxApps.Items[i] as AppListItem;
                if (item?.Entry?.WingetId != null)
                    _checkedIds.Add(item.Entry.WingetId);
            }
        }

        private List<AppEntry> GetSelectedApps()
        {
            // operate on checked apps
            var list = new List<AppEntry>();

            foreach (var obj in checkedListBoxApps.CheckedItems)
            {
                var item = obj as AppListItem;
                if (item?.Entry != null)
                    list.Add(item.Entry);
            }

            return list;
        }

        private void PopulateCategories()
        {
            var cats = _allApps
                .Select(a => a.Category)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c)
                .ToList();

            comboDisplayMode.BeginUpdate();
            try
            {
                comboDisplayMode.Items.Clear();
                comboDisplayMode.Items.Add("All");
                foreach (var c in cats)
                    comboDisplayMode.Items.Add(c);
            }
            finally
            {
                comboDisplayMode.EndUpdate();
            }

            comboDisplayMode.SelectedIndex = 0; // All
        }

        // ---------------- Buttons / Actions ----------------

        private async void BtnInstall_Click(object sender, EventArgs e)
        {
            var apps = GetSelectedApps();
            if (apps.Count == 0) return;

            if (_isBusy) return;
            SetBusy(true);

            try
            {
                Logger.BeginSection("Winget Install Selected");
                foreach (var a in apps)
                    await InstallOneAsync(a);
            }
            catch (Exception ex)
            {
                Logger.Log("Install failed: " + ex.Message, LogLevel.Error);
            }
            finally
            {
                SetBusy(false);
                await AnalyzeAsync();
            }
        }

        private async void BtnUpgradeSelected_Click(object sender, EventArgs e)
        {
            var apps = GetSelectedApps();
            if (apps.Count == 0) return;

            if (_isBusy) return;
            SetBusy(true);

            try
            {
                Logger.BeginSection("Winget Upgrade Selected");
                foreach (var a in apps)
                    await UpgradeOneAsync(a);
            }
            catch (Exception ex)
            {
                Logger.Log("Upgrade failed: " + ex.Message, LogLevel.Error);
            }
            finally
            {
                SetBusy(false);
                await AnalyzeAsync();
            }
        }

        private async void BtnUpgradeAll_Click(object sender, EventArgs e)
        {
            if (_isBusy) return;
            SetBusy(true);

            try
            {
                Logger.BeginSection("Winget Upgrade All");
                Logger.Log("Running: winget upgrade --all", LogLevel.Info);

                await RunWingetStreamingAsync("upgrade --all --accept-package-agreements --accept-source-agreements");

                Logger.Log("Upgrade all finished.", LogLevel.Info);
            }
            catch (Exception ex)
            {
                Logger.Log("Upgrade all failed: " + ex.Message, LogLevel.Error);
            }
            finally
            {
                SetBusy(false);
                await AnalyzeAsync();
            }
        }

        private async void BtnUninstall_Click(object sender, EventArgs e)
        {
            var apps = GetSelectedApps();
            if (apps.Count == 0) return;

            if (_isBusy) return;
            SetBusy(true);

            try
            {
                Logger.BeginSection("Winget Uninstall Selected");

                foreach (var a in apps)
                {
                    Logger.Log($"Uninstall: {a.Name} ({a.WingetId})", LogLevel.Warning);
                    await RunWingetStreamingAsync($"uninstall --id \"{a.WingetId}\" -e");
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Uninstall failed: " + ex.Message, LogLevel.Error);
            }
            finally
            {
                SetBusy(false);
                await AnalyzeAsync();
            }
        }

        /// <summary>
        /// Toggles all items in the app list (select all / select none).
        /// </summary>
        public void ToggleSelection()
        {
            bool shouldCheck = false;

            for (int i = 0; i < checkedListBoxApps.Items.Count; i++)
            {
                if (!checkedListBoxApps.GetItemChecked(i))
                {
                    shouldCheck = true;
                    break;
                }
            }

            for (int i = 0; i < checkedListBoxApps.Items.Count; i++)
                checkedListBoxApps.SetItemChecked(i, shouldCheck);
        }

        private async Task InstallOneAsync(AppEntry a)
        {
            Logger.Log($"Install: {a.Name} ({a.WingetId})", LogLevel.Info);

            await RunWingetStreamingAsync(
                $"install --id \"{a.WingetId}\" -e --accept-package-agreements --accept-source-agreements");
        }

        private async Task UpgradeOneAsync(AppEntry a)
        {
            Logger.Log($"Upgrade: {a.Name} ({a.WingetId})", LogLevel.Info);

            await RunWingetStreamingAsync(
                $"upgrade --id \"{a.WingetId}\" -e --accept-package-agreements --accept-source-agreements");
        }

        // ---------------- File Loading / Parsing ----------------

        private void LoadAppsFromWingetTxt()
        {
            _allApps.Clear();
            _installedIds.Clear();
            _upgradeIds.Clear();
            _checkedIds.Clear();

            var path = Path.Combine(
          AppDomain.CurrentDomain.BaseDirectory,
          "Plugins",
          AppsFileName);

            if (!File.Exists(path))
            {
                Logger.Log($"Missing {AppsFileName} next to EXE: {path}", LogLevel.Error);
                return;
            }

            var text = File.ReadAllText(path, Encoding.UTF8);
            _allApps.AddRange(ParseIniApps(text));

            Logger.BeginSection("Winget Apps");
            Logger.Log($"Loaded {_allApps.Count} apps from {AppsFileName}", LogLevel.Info);
        }

        /// <summary>
        /// Parses INI-like text:
        /// [Category]
        /// Name=Winget.Id
        /// Skips comments (# or ;) and Winget "na".
        /// </summary>
        private static List<AppEntry> ParseIniApps(string iniText)
        {
            var list = new List<AppEntry>();
            string currentCategory = "Uncategorized";

            using (var sr = new StringReader(iniText ?? ""))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.Length == 0) continue;
                    if (line.StartsWith("#") || line.StartsWith(";")) continue;

                    // Category header
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        var c = line.Substring(1, line.Length - 2).Trim();
                        currentCategory = string.IsNullOrWhiteSpace(c) ? "Uncategorized" : c;
                        continue;
                    }

                    // Entry: Name=WingetId
                    var eq = line.IndexOf('=');
                    if (eq <= 0 || eq >= line.Length - 1) continue;

                    var name = line.Substring(0, eq).Trim();
                    var winget = line.Substring(eq + 1).Trim();

                    if (string.IsNullOrWhiteSpace(name)) continue;
                    if (string.IsNullOrWhiteSpace(winget)) continue;
                    if (string.Equals(winget, "na", StringComparison.OrdinalIgnoreCase)) continue;

                    list.Add(new AppEntry
                    {
                        Category = currentCategory,
                        Name = name,
                        WingetId = winget
                    });
                }
            }

            return list;
        }

        // ---------------- Winget Helpers ----------------

        /// <summary>
        /// Runs winget and streams output to Logger.
        /// </summary>
        private async Task<int> RunWingetStreamingAsync(string arguments)
        {
            return await RunProcessStreamingAsync("winget", arguments, (line) =>
            {
                // Keep output readable: errors often appear in stderr but we handle both the same way yeah!!
                Logger.Log(line, LogLevel.Info);
            });
        }

        /// <summary>
        /// Runs winget and returns captured output lines (stdout+stderr mixed).
        /// </summary>
        private async Task<List<string>> RunWingetCaptureLinesAsync(string arguments)
        {
            var lines = new List<string>();
            await RunProcessStreamingAsync("winget", arguments, line => lines.Add(line));
            return lines;
        }

        private static async Task<int> RunProcessStreamingAsync(string fileName, string arguments, Action<string> onLine)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            var tcs = new TaskCompletionSource<int>();
            var p = new Process { StartInfo = psi, EnableRaisingEvents = true };

            p.OutputDataReceived += (s, e) => { if (e.Data != null) onLine?.Invoke(e.Data); };
            p.ErrorDataReceived += (s, e) => { if (e.Data != null) onLine?.Invoke(e.Data); };
            p.Exited += (s, e) =>
            {
                try { tcs.TrySetResult(p.ExitCode); }
                finally { p.Dispose(); }
            };

            if (!p.Start())
                return -1;

            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            return await tcs.Task.ConfigureAwait(false);
        }

        // ---------------- Busy ----------------

        private void SetBusy(bool busy)
        {
            _isBusy = busy;

            // Disable actions while winget is running
            btnInstall.Enabled = !busy;
            btnUpgradeSelected.Enabled = !busy;
            btnUpgradeAll.Enabled = !busy;
            btnUninstall.Enabled = !busy;
            chkInstalledOnly.Enabled = !busy;
            chkUpgradesOnly.Enabled = !busy;

            comboDisplayMode.Enabled = !busy;
            checkedListBoxApps.Enabled = !busy;
        }

        // ---------------- Internal types ----------------

        private sealed class AppEntry
        {
            public string Category;
            public string Name;
            public string WingetId;

            public bool IsInstalled;
            public bool HasUpgrade;
        }

        /// <summary>
        /// Wraps an AppEntry for ListBox display. ListBox uses ToString().
        /// </summary>
        private sealed class AppListItem
        {
            public readonly AppEntry Entry;
            private readonly string _text;

            public AppListItem(AppEntry entry, string text)
            {
                Entry = entry;
                _text = text ?? "";
            }

            public override string ToString() => _text;
        }
    }
}