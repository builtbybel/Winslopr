using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Winslopr.Helpers;

/// <summary>
/// Provides actions for interacting with and managing the log.
/// Also provides small helper log outputs for the Features tree (optional).
/// </summary>

namespace Winslopr.Services
{
    public sealed class LoggerActions
    {
        // Optional provider for the feature tree items
        private Func<IEnumerable<FeatureTreeItem>> _getFeaturesItems;

        /// <summary>
        /// Registers a callback that returns the root feature items.
        /// </summary>
        public void SetFeaturesItemsProvider(Func<IEnumerable<FeatureTreeItem>> getItems)
        {
            _getFeaturesItems = getItems;
        }

        /// <summary>Copies the whole log to the clipboard.</summary>
        public void CopyToClipboard()
        {
            var text = Logger.FullText;
            if (!string.IsNullOrWhiteSpace(text))
            {
                var dp = new Windows.ApplicationModel.DataTransfer.DataPackage();
                dp.SetText(text);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dp);
            }
        }

        /// <summary>Clears the logger output.</summary>
        public void Clear()
            => Logger.Clear();

        /// <summary>Opens Winslopr online inspector tool.</summary>
        public void AnalyzeOnline(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                return;

            var logText = Logger.FullText;
            if (string.IsNullOrWhiteSpace(logText))
                return;

            // Copy log to clipboard and open the inspector
            var dp = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dp.SetText(logText);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dp);

            Process.Start(new ProcessStartInfo(baseUrl) { UseShellExecute = true });
        }

        // ---------------- Feature tree log tools (optional) ----------------

        public void LogFeatureSummary()
        {
            var items = GetItemsOrNull();
            if (items == null)
            {
                Logger.Log(Localizer.Get("LogActions_NotAvailable"));
                return;
            }

            var all = FeatureTreeItem.Flatten(items).ToList();
            int total = all.Count;
            int checkedCount = all.Count(i => i.IsChecked);

            Logger.Log(Localizer.GetFormat("LogActions_Summary", total, checkedCount));
        }

        public void LogCheckedFeatures()
        {
            var items = GetItemsOrNull();
            if (items == null)
            {
                Logger.Log(Localizer.Get("LogActions_NotAvailable"));
                return;
            }

            var list = FeatureTreeItem.Flatten(items)
                .Where(i => i.IsChecked && i.Children.Count == 0)
                .Select(GetPath)
                .ToList();

            if (list.Count == 0)
            {
                Logger.Log(Localizer.Get("LogActions_NoChecked"));
                return;
            }

            Logger.Log(Localizer.GetFormat("LogActions_CheckedItems", list.Count));
            foreach (var path in list)
                Logger.Log("  - " + path);
        }

        public void LogUncheckedLeafFeatures(int maxLines = 200)
        {
            var items = GetItemsOrNull();
            if (items == null)
            {
                Logger.Log(Localizer.Get("LogActions_NotAvailable"));
                return;
            }

            var list = FeatureTreeItem.Flatten(items)
                .Where(i => !i.IsChecked && i.Children.Count == 0)
                .Select(GetPath)
                .ToList();

            Logger.Log(Localizer.GetFormat("LogActions_UncheckedItems", list.Count));
            foreach (var path in list.Take(maxLines))
                Logger.Log("  - " + path);

            if (list.Count > maxLines)
                Logger.Log("  ... (truncated)");
        }

        private IEnumerable<FeatureTreeItem> GetItemsOrNull()
            => _getFeaturesItems?.Invoke();

        private static string GetPath(FeatureTreeItem item)
        {
            var parts = new Stack<string>();
            var cur = item;
            while (cur != null)
            {
                parts.Push(cur.Name ?? string.Empty);
                cur = cur.Parent;
            }
            return string.Join("/", parts);
        }
    }
}