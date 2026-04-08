using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Winslop.Features;
using Winslop.Help;
using Winslop.Helpers;

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
        /// Loads all features and returns them as FeatureTreeItems.
        /// </summary>
        public static List<FeatureTreeItem> LoadFeatures()
        {
            return FeatureLoader.Load();
        }

        /// <summary>
        /// Analyzes all checked features recursively and logs only issues.
        /// </summary>
        public static async Task AnalyzeAll(IEnumerable<FeatureTreeItem> items)
        {
            ResetAnalysis();
            FeatureTreeItem.ResetAllStatus(items);

            foreach (var item in items)
                await AnalyzeCheckedRecursive(item);
        }

        /// <summary>
        /// Recursively checks all features and logs misconfigurations.
        /// </summary>
        private static async Task AnalyzeCheckedRecursive(FeatureTreeItem item)
        {
            if (!item.IsVisible) return;

            if (!item.IsCategory && item.IsChecked && item.Feature != null)
            {
                if (!item.Feature.IsApplicable())
                {
                    item.Status = AnalysisStatus.NotApplicable;
                    string reason = item.Feature.InapplicableReason();
                    Logger.Log(
                        $"ℹ️ [{item.Parent?.Name ?? Localizer.Get("Log_General")}] {item.Name} - {Localizer.Get("Log_Skipped")}: {reason ?? Localizer.Get("Log_NotApplicableOS")}",
                        LogLevel.Info);
                    return;
                }

                totalChecked++;
                bool isOk = await item.Feature.CheckFeature();

                if (!isOk)
                {
                    issuesFound++;
                    item.Status = AnalysisStatus.NeedsFix;
                    string category = item.Parent?.Name ?? Localizer.Get("Log_General");
                    Logger.Log($"❌ [{category}] {item.Name} - {Localizer.Get("Log_NotConfigured")}");
                    Logger.Log($"   ➤ {item.Feature.GetFeatureDetails()}");
                    Logger.Log(new string('-', 50), LogLevel.Info);
                }
                else
                {
                    item.Status = AnalysisStatus.Ok;
                }
            }

            foreach (var child in item.Children)
                await AnalyzeCheckedRecursive(child);
        }

        /// <summary>
        /// Fixes all checked features recursively.
        /// </summary>
        public static async Task FixChecked(FeatureTreeItem item)
        {
            if (!item.IsVisible) return;

            if (!item.IsCategory && item.IsChecked && item.Feature != null)
            {
                if (!item.Feature.IsApplicable())
                {
                    item.Status = AnalysisStatus.NotApplicable;
                    Logger.Log(
                        $"ℹ️ {item.Name} - {Localizer.Get("Log_Skipped")}: {item.Feature.InapplicableReason() ?? Localizer.Get("Log_NotApplicableOS")}",
                        LogLevel.Info);
                    return;
                }

                bool result = await item.Feature.DoFeature();
                Logger.Log(result
                    ? $"🔧 {item.Name} - {Localizer.Get("Log_Fixed")}"
                    : $"❌ {item.Name} - ⚠️ {Localizer.Get("Log_FixFailed")}",
                    result ? LogLevel.Info : LogLevel.Error);
            }

            foreach (var child in item.Children)
                await FixChecked(child);
        }

        /// <summary>
        /// Restores all checked features recursively.
        /// </summary>
        public static void RestoreChecked(FeatureTreeItem item)
        {
            if (!item.IsVisible) return;

            if (!item.IsCategory && item.IsChecked && item.Feature != null)
            {
                if (!item.Feature.IsApplicable())
                {
                    item.Status = AnalysisStatus.NotApplicable;
                    Logger.Log(
                        $"ℹ️ {item.Name} - {Localizer.Get("Log_SkippedRestore")}: {item.Feature.InapplicableReason() ?? Localizer.Get("Log_NotApplicableOS")}",
                        LogLevel.Info);
                    return;
                }

                bool ok = item.Feature.UndoFeature();
                string category = item.Parent?.Name ?? Localizer.Get("Log_General");
                Logger.Log(ok
                    ? $"↩️ [{category}] {item.Name} - {Localizer.Get("Log_Restored")}"
                    : $"❌ [{category}] {item.Name} - {Localizer.Get("Log_RestoreFailed")}",
                    ok ? LogLevel.Info : LogLevel.Error);
            }

            foreach (var child in item.Children)
                RestoreChecked(child);
        }

        /// <summary>
        /// Analyzes a selected feature or, if it's a category, analyzes only checked child features.
        /// </summary>
        public static async Task AnalyzeFeature(FeatureTreeItem item)
        {
            if (!item.IsVisible) return; // Skip invisible items immediately

            if (!item.IsCategory && item.Feature != null)
            {
                if (!item.Feature.IsApplicable())
                {
                    item.Status = AnalysisStatus.NotApplicable;
                    Logger.Log(
                        $"ℹ️ {item.Name} - {Localizer.Get("Log_Skipped")}: {item.Feature.InapplicableReason() ?? Localizer.Get("Log_NotApplicableOS")}",
                        LogLevel.Info);
                    return;
                }

                bool isOk = await item.Feature.CheckFeature();

                if (isOk)
                {
                    item.Status = AnalysisStatus.Ok;
                    Logger.Log($"✅ {Localizer.GetFormat("Log_ProperlyConfigured", item.Name)}", LogLevel.Info);
                }
                else
                {
                    item.Status = AnalysisStatus.NeedsFix;
                    Logger.Log($"❌ {Localizer.GetFormat("Log_RequiresAttention", item.Name)}", LogLevel.Warning);
                    Logger.Log($"   ➤ {item.Feature.GetFeatureDetails()}");
                    Logger.Log(new string('-', 50), LogLevel.Info);
                }
            }
            else
            {
                foreach (var child in item.Children)
                {
                    if (child.IsChecked)
                        await AnalyzeFeature(child);
                }
            }
        }

        /// <summary>
        /// Attempts to fix the selected feature or, if it is a category, fixes only checked child features.
        /// </summary>
        public static async Task FixFeature(FeatureTreeItem item)
        {
            if (!item.IsVisible) return;

            if (!item.IsCategory && item.Feature != null)
            {
                if (!item.Feature.IsApplicable())
                {
                    item.Status = AnalysisStatus.NotApplicable;
                    Logger.Log(
                        $"ℹ️ {item.Name} - {Localizer.Get("Log_Skipped")}: {item.Feature.InapplicableReason() ?? Localizer.Get("Log_NotApplicableOS")}",
                        LogLevel.Info);
                    return;
                }

                bool result = await item.Feature.DoFeature();
                Logger.Log(result
                    ? $"🔧 {item.Name} - {Localizer.Get("Log_Fixed")}"
                    : $"❌ {item.Name} - ⚠️ {Localizer.Get("Log_FixFailed")}",
                    result ? LogLevel.Info : LogLevel.Error);
            }
            else
            {
                foreach (var child in item.Children)
                {
                    if (child.IsChecked)
                        await FixFeature(child);
                }
            }
        }

        /// <summary>
        /// Restores a selected feature (always) or, if it's a category, only restores checked child features.
        /// </summary>
        public static void RestoreFeature(FeatureTreeItem item)
        {
            if (!item.IsVisible) return;

            if (!item.IsCategory && item.Feature != null)
            {
                if (!item.Feature.IsApplicable())
                {
                    item.Status = AnalysisStatus.NotApplicable;
                    Logger.Log(
                        $"ℹ️ {item.Name} - {Localizer.Get("Log_SkippedRestore")}: {item.Feature.InapplicableReason() ?? Localizer.Get("Log_NotApplicableOS")}",
                        LogLevel.Info);
                    return;
                }

                bool ok = item.Feature.UndoFeature();
                Logger.Log(ok
                    ? $"↩️ {item.Name} - {Localizer.Get("Log_Restored")}"
                    : $"❌ {item.Name} - {Localizer.Get("Log_RestoreFailed")}",
                    ok ? LogLevel.Info : LogLevel.Error);
            }
            else
            {
                foreach (var child in item.Children)
                {
                    if (child.IsChecked)
                        RestoreFeature(child);
                }
            }
        }

        // -- Helpers -----------------------------------------------

        /// <summary>
        /// Opens help for the selected feature.
        /// </summary>
        public static bool ShowHelp(FeatureTreeItem item)
        {
            if (item == null) return false;

            try
            {
                if (item.Feature != null)
                    FeatureHelp.OpenUrl(FeatureHelp.GetFeatureUrl(item.Feature));
                else
                    FeatureHelp.OpenUrl(FeatureHelp.GetPluginUrl(item.Name));

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}