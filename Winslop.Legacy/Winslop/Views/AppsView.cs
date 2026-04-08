using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using System.Windows.Forms;
using Winslop.Properties;

namespace Winslop.Views
{
    /// <summary>
    /// AppsView encapsulates:
    /// - pattern loading (Plugins\CFEnhancer.txt OR built-in resource list)
    /// - scanning installed Store apps
    /// - search/filter with checked-state preservation
    /// - uninstall of selected apps
    /// </summary>
    public partial class AppsView : UserControl, IMainActions, ISearchable, IView
    {
        // Keeps the full unfiltered app list for search reset/filtering.
        private string[] _allApps = new string[0];

        // Optional mapping if we want to show friendly names
        private readonly Dictionary<string, string> _fullNameToName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private enum AppDisplayMode
        {
            Recommended = 0,
            AllInstalled = 1,
            PluginOnly = 2,
            BuiltInOnly = 3
        }

        public AppsView()
        {
            InitializeComponent();
            InitializeDisplayModeCombo();
        }

        private void InitializeDisplayModeCombo()
        {
            if (comboDisplayMode == null)
                return;

            comboDisplayMode.Items.Clear();
            comboDisplayMode.Items.Add("Standard scan (recommended)");
            comboDisplayMode.Items.Add("Full scan (all installed apps)");
            comboDisplayMode.Items.Add("Scan using community list (CFEnhancer)");
            comboDisplayMode.Items.Add("Scan using built-in list");

            comboDisplayMode.DropDownStyle = ComboBoxStyle.DropDownList;
            comboDisplayMode.SelectedIndex = 0;

            //   comboDisplayMode.SelectedIndexChanged += async (s, e) => await AnalyzeAsync();
        }

        private AppDisplayMode SelectedMode
        {
            get
            {
                if (comboDisplayMode == null)
                    return AppDisplayMode.Recommended;

                switch (comboDisplayMode.SelectedIndex)
                {
                    case 1: return AppDisplayMode.AllInstalled;
                    case 2: return AppDisplayMode.PluginOnly;
                    case 3: return AppDisplayMode.BuiltInOnly;
                    default: return AppDisplayMode.Recommended;
                }
            }
        }

        // ---------------- IMainActions ----------------

        /// <summary>
        /// Scans installed apps based on the selected display mode and populates the list.
        /// </summary>
        public async Task AnalyzeAsync()
        {
            checkedListBoxApps.Items.Clear();
            checkedListBoxApps.Items.Add("Scanning installed apps, please wait...");
            _fullNameToName.Clear();

            // Load plugin and built-in patterns once.
            var plugin = LoadExternalBloatwarePatterns();
            var builtIn = LoadBuiltInPatterns();

            string[] bloat;
            string[] white;
            bool scanAll;

            // Decide behavior based on ComboBox mode.
            switch (SelectedMode)
            {
                case AppDisplayMode.AllInstalled:
                    // Show everything, but still respect whitelist if plugin exists.
                    bloat = plugin.bloatwarePatterns.Length > 0 ? plugin.bloatwarePatterns : builtIn.bloat;
                    white = plugin.whitelistPatterns.Length > 0 ? plugin.whitelistPatterns : builtIn.white;
                    scanAll = true;
                    Logger.Log("Displaying all installed applications.", LogLevel.Info);
                    break;

                case AppDisplayMode.PluginOnly:
                    // Only plugin list; if missing, show empty list with a friendly message.
                    if (plugin.bloatwarePatterns.Length == 0 && !plugin.scanAll)
                    {
                        Logger.Log("No plugin list found. Install CFEnhancer to use plugin recommendations.", LogLevel.Warning);
                        _allApps = new string[0];
                        FillList(_allApps);
                        return;
                    }
                    bloat = plugin.bloatwarePatterns;
                    white = plugin.whitelistPatterns;
                    scanAll = plugin.scanAll; // allow wildcard mode if plugin has it
                    Logger.Log("Displaying plugin recommendations (CFEnhancer).", LogLevel.Info);
                    break;

                case AppDisplayMode.BuiltInOnly:
                    bloat = builtIn.bloat;
                    white = builtIn.white;
                    scanAll = false;
                    Logger.Log("Displaying built-in recommendations.", LogLevel.Info);
                    break;

                default: // Recommended
                    if (plugin.bloatwarePatterns.Length > 0 || plugin.scanAll)
                    {
                        bloat = plugin.bloatwarePatterns;
                        white = plugin.whitelistPatterns;
                        scanAll = plugin.scanAll; // if plugin has wildcard, respect it in "Recommended"
                        Logger.Log("Displaying recommended applications (plugin).", LogLevel.Info);
                    }
                    else
                    {
                        bloat = builtIn.bloat;
                        white = builtIn.white;
                        scanAll = false;
                        Logger.Log("Displaying recommended applications (built-in).", LogLevel.Info);
                    }
                    break;
            }

            // Analyze installed apps.
            List<AppAnalysisResult> results = await AnalyzeAppsAsync(bloat, white, scanAll);

            // Log summary.
            LogAnalysisResults(results, scanAll);

            // Fill UI with FullName strings (as before).
            string[] fullNames = results.Select(r => r.FullName).ToArray();
            _allApps = fullNames;

            FillList(fullNames);
        }

        /// <summary>
        /// Uninstalls all checked apps and removes them from the list after uninstall.
        /// </summary>
        public async Task FixAsync()
        {
            Logger.OutputBox?.Clear();

            List<string> selected = checkedListBoxApps.CheckedItems.Cast<string>().ToList();
            if (selected.Count == 0)
                return;

            List<string> removed = await UninstallSelectedAppsAsync(selected);

            for (int i = 0; i < removed.Count; i++)
                checkedListBoxApps.Items.Remove(removed[i]);
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

        // ---------------- ISearchable ----------------

        /// <summary>
        /// Applies search to the apps list (empty query = reset).
        /// Preserves checked state for items that remain visible.
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

        // ---------------- Filtering ----------------

        private void ApplyAppsFilter(string query)
        {
            if (_allApps == null || _allApps.Length == 0)
                return;

            string q = (query ?? string.Empty).Trim();

            // Preserve checked state by app name (FullName string here)
            HashSet<string> checkedSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string s in checkedListBoxApps.CheckedItems.Cast<string>())
                checkedSet.Add(s);

            string[] items;
            if (string.IsNullOrEmpty(q))
            {
                items = _allApps;
            }
            else
            {
                items = _allApps
                    .Where(a => a.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToArray();
            }

            checkedListBoxApps.BeginUpdate();
            try
            {
                checkedListBoxApps.Items.Clear();

                for (int i = 0; i < items.Length; i++)
                {
                    string app = items[i];
                    int idx = checkedListBoxApps.Items.Add(app);

                    if (checkedSet.Contains(app))
                        checkedListBoxApps.SetItemChecked(idx, true);
                }
            }
            finally
            {
                checkedListBoxApps.EndUpdate();
            }
        }

        private void FillList(string[] items)
        {
            checkedListBoxApps.BeginUpdate();
            try
            {
                checkedListBoxApps.Items.Clear();
                checkedListBoxApps.Items.AddRange(items);
            }
            finally
            {
                checkedListBoxApps.EndUpdate();
            }
        }

        // ---------------- Core logic ----------------

        private class AppAnalysisResult
        {
            public string AppName { get; set; }
            public string FullName { get; set; }
        }

        /// <summary>
        /// Loads all installed Store apps using PowerShell Get-AppxPackage.
        /// Returns dictionary: appName > fullName.
        /// </summary>
        private static async Task<Dictionary<string, string>> LoadAppsAsync()
        {
            Dictionary<string, string> dir = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            await Task.Run(() =>
            {
                try
                {
                    var initialSessionState = InitialSessionState.CreateDefault();

                    using (var runspace = RunspaceFactory.CreateRunspace(initialSessionState))
                    {
                        runspace.Open();
                        using (var ps = PowerShell.Create())
                        {
                            ps.Runspace = runspace;
                            ps.AddCommand("Get-AppxPackage");
                            var results = ps.Invoke();

                            foreach (var result in results)
                            {
                                string name = result.Properties["Name"]?.Value?.ToString();
                                string fullName = result.Properties["PackageFullName"]?.Value?.ToString();

                                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(fullName))
                                {
                                    if (!dir.ContainsKey(name))
                                        dir[name] = fullName;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(string.Format("Error loading apps: {0}", ex.Message), LogLevel.Warning);
                }
            });

            Logger.Log(string.Format("(Checked against {0} apps from the system)", dir.Count));
            return dir;
        }

        /// <summary>
        /// Analyzes installed apps using bloatware patterns and whitelist.
        /// If scanAll is true, includes everything except whitelist.
        /// Otherwise includes only apps matching any bloatware pattern.
        /// </summary>
        private async Task<List<AppAnalysisResult>> AnalyzeAppsAsync(string[] bloatwarePatterns, string[] whitelistPatterns, bool scanAll)
        {
            Logger.Log("\n🧩 APPS ANALYSIS", LogLevel.Info);
            Logger.Log(new string('=', 50), LogLevel.Info);

            Dictionary<string, string> apps = await LoadAppsAsync();

            string[] bloat = Normalize(bloatwarePatterns);
            string[] white = Normalize(whitelistPatterns);

            List<AppAnalysisResult> results = new List<AppAnalysisResult>();

            foreach (var kvp in apps)
            {
                string name = kvp.Key;
                string full = kvp.Value;
                string lower = name.ToLowerInvariant();

                // Always skip whitelisted apps
                if (IsMatchAny(lower, white))
                    continue;

                // Show all if scanAll; otherwise only bloat matches
                if (scanAll || IsMatchAny(lower, bloat))
                {
                    results.Add(new AppAnalysisResult { AppName = name, FullName = full });
                    _fullNameToName[full] = name;
                }
            }

            return results;
        }

        private static bool IsMatchAny(string haystackLower, string[] patternsLower)
        {
            if (patternsLower == null || patternsLower.Length == 0)
                return false;

            for (int i = 0; i < patternsLower.Length; i++)
            {
                if (haystackLower.Contains(patternsLower[i]))
                    return true;
            }

            return false;
        }

        private static void LogAnalysisResults(List<AppAnalysisResult> results, bool scanAll)
        {
            if (scanAll)
            {
                Logger.Log(string.Format("Showing all apps (except whitelist): {0}", results.Count), LogLevel.Info);
                Logger.Log("");
                return;
            }

            if (results.Count > 0)
            {
                Logger.Log("Bloatware apps detected:", LogLevel.Info);
                for (int i = 0; i < results.Count; i++)
                {
                    var a = results[i];
                    Logger.Log(string.Format("❌ [ Bloatware ] {0} ({1})", a.AppName, a.FullName), LogLevel.Warning);
                }
            }
            else
            {
                Logger.Log("✅ No Microsoft Store bloatware apps found.", LogLevel.Info);
            }

            Logger.Log("");
        }


        /// <summary>
        /// Uninstalls selected apps using a single shared PowerShell Runspace
        /// and returns those removed successfully.
        /// </summary>
        private static async Task<List<string>> UninstallSelectedAppsAsync(List<string> selectedApps)
        {
            List<string> removed = new List<string>();

            await Task.Run(() =>
            {
                var initialSessionState = InitialSessionState.CreateDefault();

                using (var runspace = RunspaceFactory.CreateRunspace(initialSessionState))
                {
                    runspace.Open();

                    for (int i = 0; i < selectedApps.Count; i++)
                    {
                        string fullName = selectedApps[i];
                        Logger.Log(string.Format("🗑️ Removing app: {0}...", fullName));

                        try
                        {
                            using (var ps = PowerShell.Create())
                            {
                                ps.Runspace = runspace;
                                ps.AddCommand("Remove-AppxPackage")
                                  .AddParameter("Package", fullName);
                                ps.Invoke();

                                if (ps.HadErrors)
                                {
                                    foreach (var err in ps.Streams.Error)
                                        Logger.Log(string.Format("Failed to uninstall app {0}: {1}", fullName, err.ToString()), LogLevel.Warning);
                                }
                                else
                                {
                                    Logger.Log(string.Format("🗑️ Removed Store App: {0}", fullName));
                                    removed.Add(fullName);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(string.Format("Error uninstalling app {0}: {1}", fullName, ex.Message), LogLevel.Warning);
                        }
                    }
                }
            });

            List<string> failed = selectedApps.Except(removed).ToList();
            for (int i = 0; i < failed.Count; i++)
                Logger.Log(string.Format("⚠️ Failed to remove Store App: {0}", failed[i]), LogLevel.Warning);

            Logger.Log("App cleanup complete.");
            return removed;
        }

        /// <summary>
        /// Loads external bloatware and whitelist patterns from Plugins\CFEnhancer.txt.
        /// Supports:
        /// - "!" prefix => whitelist entry
        /// - "*" or "*.*" => scanAll wildcard
        /// - "#" comments
        /// </summary>
        private static (string[] bloatwarePatterns, string[] whitelistPatterns, bool scanAll)
            LoadExternalBloatwarePatterns(string fileName = "CFEnhancer.txt")
        {
            try
            {
                string exeDir = AppDomain.CurrentDomain.BaseDirectory;
                string fullPath = Path.Combine(exeDir, "Plugins", fileName);

                if (!File.Exists(fullPath))
                {
                    Logger.Log(
                        "⚠️ The bloatware radar stays basic for now 🧠. Get the enhanced detection list from Start > Manage plugins > CFEnhancer plugin",
                        LogLevel.Warning);

                    return (new string[0], new string[0], false);
                }

                List<string> bloat = new List<string>();
                List<string> white = new List<string>();
                bool scanAll = false;

                foreach (string raw in File.ReadLines(fullPath))
                {
                    // Strip comments and trim
                    string entry = raw.Split('#')[0].Trim();
                    if (string.IsNullOrWhiteSpace(entry))
                        continue;

                    // Wildcard: show all installed apps (except whitelist)
                    if (entry == "*" || entry == "*.*")
                    {
                        scanAll = true;
                        continue;
                    }

                    // Whitelist entry: leading "!"
                    if (entry.StartsWith("!"))
                    {
                        string w = entry.Substring(1).Trim().ToLowerInvariant();
                        if (!string.IsNullOrEmpty(w))
                            white.Add(w);
                    }
                    else
                    {
                        bloat.Add(entry.ToLowerInvariant());
                    }
                }

                return (Normalize(bloat), Normalize(white), scanAll);
            }
            catch (Exception ex)
            {
                Logger.Log("Error reading external bloatware file: " + ex.Message, LogLevel.Warning);
                return (new string[0], new string[0], false);
            }
        }

        /// <summary>
        /// Fallback bloatware list from Resources.PredefinedApps (comma-separated).
        /// </summary>
        private static (string[] bloat, string[] white) LoadBuiltInPatterns()
        {
            string raw = Resources.PredefinedApps ?? string.Empty;

            string[] bloat = raw
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim().ToLowerInvariant())
                .Where(s => s.Length > 0)
                .Distinct()
                .ToArray();

            return (bloat, new string[0]);
        }

        /// <summary>
        /// Normalize patterns: trim, lowercase, remove empty, distinct.
        /// </summary>
        private static string[] Normalize(IEnumerable<string> items)
        {
            if (items == null)
                return new string[0];

            return items
                .Select(s => (s ?? string.Empty).Trim().ToLowerInvariant())
                .Where(s => s.Length > 0)
                .Distinct()
                .ToArray();
        }
    }
}