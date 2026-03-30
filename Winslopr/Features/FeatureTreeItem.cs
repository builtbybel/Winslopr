using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI;
using Winslop.Helpers;

/// <summary>
/// Represents a single node in the Features hierarchy.
///
/// <para>
/// <see cref="FeatureTreeItem"/> is the core data model behind the Features tree.
/// Each instance can describe a category, a configurable feature, or a plugin,
/// including its current state, metadata, and nested child nodes.
/// </para>
///
/// <para>
/// The UI layer (TreeView / TreeViewItem) only renders this structure.
/// In other words: <see cref="FeatureTreeItem"/> defines what a node is,
/// while XAML controls define how that node is displayed.
/// </para>
/// </summary>

namespace Winslop
{
    /// <summary>
    /// Result of a feature analysis. Controls the text color in the TreeView.
    /// Replaces TreeNode.ForeColor from the WinForms version.
    /// </summary>
    public enum AnalysisStatus
    {
        /// <summary>Not yet analyzed (default text color).</summary>
        None,

        /// <summary>Feature is already configured as recommended (gray).</summary>
        Ok,

        /// <summary>Feature needs attention (red).</summary>
        NeedsFix,

        /// <summary>Feature does not apply to this OS version (orange).</summary>
        NotApplicable
    }

    /// <summary>
    /// ViewModel for a single node in the features TreeView.
    /// Replaces WinForms TreeNode. WinUI has no built-in tree node,
    /// so this class provides the data binding surface.
    ///
    /// Two kinds of nodes exist:
    ///   - Category (IsCategory = true)  : grouping node, e.g. "Taskbar", "Privacy"
    ///   - Feature  (IsCategory = false) : leaf node backed by a FeatureBase instance
    ///
    /// Created directly in FeatureLoader.Load() via collection initializer.
    /// Parent is set automatically when a child is added to Children
    /// (via CollectionChanged), so log messages like
    ///   "[Taskbar] Disable Widgets" show the correct category.
    /// </summary>
    public class FeatureTreeItem : INotifyPropertyChanged
    {
        private bool _isChecked;
        private AnalysisStatus _status = AnalysisStatus.None;

        // Controls visibility during search/filter operations (see FilteredChildren).
        private bool _isVisible = true;

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged();
                }
            }
        }

/// <summary>Display name shown in the tree (feature ID or category name).</summary>
        public string Name { get; }

        /// <summary>True for grouping nodes (e.g. "Privacy"), false for leaf features.</summary>
        public bool IsCategory { get; }

        /// <summary>The underlying feature logic. Null for categories and plugins.</summary>
        public FeatureBase Feature { get; }

        /// <summary>
        /// Parent category node. Set automatically by <see cref="OnChildrenChanged"/>
        /// when this item is added to another item's Children collection.
        /// Used by FeatureManager for log messages like "[Taskbar] Feature X".
        /// </summary>
        public FeatureTreeItem Parent { get; set; }

        /// <summary>Child nodes. Adding items automatically sets their Parent.</summary>
        public ObservableCollection<FeatureTreeItem> Children { get; } = new();

        /// <summary>
        /// Filtered subset of Children for display in the TreeView.
        /// Never set Visibility on TreeViewItem directly — WinUI 3 crashes when
        /// the virtualizing panel handles Visibility changes on tree nodes.
        /// Instead, rebuild this collection after each filter pass via RefreshFilteredChildren().
        /// </summary>
        public ObservableCollection<FeatureTreeItem> FilteredChildren { get; } = new();

        /// <summary>
        /// Rebuilds FilteredChildren to only contain currently-visible direct children.
        /// Recurses so the entire subtree is refreshed in one call.
        /// </summary>
        public void RefreshFilteredChildren()
        {
            FilteredChildren.Clear();
            foreach (var child in Children)
            {
                if (child.IsVisible)
                {
                    FilteredChildren.Add(child);
                    child.RefreshFilteredChildren();
                }
            }
        }

        /// <summary>Path to the PowerShell plugin script (only set for plugins, otherwise null). Replaces TreeNode.Tag.</summary>
        public string ScriptPath { get; set; }

        /// <summary>True if the feature is applicable on the current OS. Plugins are always applicable.</summary>
        public bool IsApplicableOnOS => Feature?.IsApplicable() ?? true;

        /// <summary>Shows the reason when a feature is not applicable (e.g., "Windows 11 only").</summary>
        public string ApplicabilityText
        {
            get
            {
                if (IsApplicableOnOS || Feature == null) return "";
                var reason = Feature.InapplicableReason();
                if (string.IsNullOrWhiteSpace(reason))
                    return $"({Localizer.Get("InapplicableReason_NotApplicable")})";
                // Try to resolve known reason keys
                string localized = reason switch
                {
                    "Windows 10 only" => Localizer.Get("InapplicableReason_Win10Only"),
                    "Windows 11 only" => Localizer.Get("InapplicableReason_Win11Only"),
                    _ => reason
                };
                return $"({localized})";
            }
        }

        /// <summary>Analysis result. Drives the text color via StatusToBrushConverter in XAML.</summary>
        public AnalysisStatus Status
        {
            get => _status;
            set { if (_status != value) { _status = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Checkbox state bound two-way in the TreeView template.
        /// When a category is checked/unchecked, all children follow
        /// (same behavior as treeFeatures_AfterCheck in my Winslop WinForms version).
        /// </summary>
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked == value) return;
                _isChecked = value;
                OnPropertyChanged();

                if (IsCategory)
                {
                    foreach (var child in Children)
                        child.IsChecked = value;
                }
            }
        }

        /// <summary>Category constructor.</summary>
        public FeatureTreeItem(string name) : this(name, isCategory: true, defaultChecked: true)
        {
        }

        /// <summary>General constructor allowing creation of either category or a non-category (used for plugins).</summary>
        public FeatureTreeItem(string name, bool isCategory, bool defaultChecked = true)
        {
            Name = name;
            IsCategory = isCategory;
            _isChecked = defaultChecked;
            Children.CollectionChanged += OnChildrenChanged;
        }

        /// <summary>Feature (leaf) constructor.</summary>
        public FeatureTreeItem(FeatureBase feature, bool defaultChecked = true)
        {
            Feature = feature;

            // Use the feature class name to generate the resource key automatically.
            // Example:
            //   SettingsAds      > Feature_SettingsAds
            //   FileExplorerAds  > Feature_FileExplorerAds
            string className = feature.GetType().Name;
            string resourceKey = $"Feature_{className}";

            // Try to resolve the localized label from the RESW resources.
            Name = Localizer.Get(resourceKey);

            // If the resource key is missing, Localizer returns the key itself.
            // Fall back to the feature's original hardcoded ID text in that case.
            if (Name == resourceKey)
                Name = feature.ID();

            IsCategory = false;
            _isChecked = defaultChecked;
            Children.CollectionChanged += OnChildrenChanged;
        }

        /// <summary>
        /// Automatically sets Parent on every child that is added to Children.
        /// This ensures FeatureManager always knows the category for logging,
        /// even when items are created via collection initializer in FeatureLoader.
        /// </summary>
        private void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (FeatureTreeItem child in e.NewItems)
                    child.Parent = this;
            }
        }

        // -- Helpers -----------------------------------------------

        /// <summary>Recursively yields every node in the tree (depth-first).</summary>
        public static IEnumerable<FeatureTreeItem> Flatten(IEnumerable<FeatureTreeItem> items)
        {
            foreach (var item in items)
            {
                yield return item;
                foreach (var descendant in Flatten(item.Children))
                    yield return descendant;
            }
        }

        /// <summary>Counts how many visible leaf features are currently checked.</summary>
        public static int CountCheckedLeaves(IEnumerable<FeatureTreeItem> items)
       => Flatten(items).Count(i => !i.IsCategory && i.Feature != null && i.IsChecked && i.IsVisible);

        /// <summary>Resets all visible nodes to AnalysisStatus.None before a new analysis run.</summary>
        public static void ResetAllStatus(IEnumerable<FeatureTreeItem> items)
        {
            foreach (var item in Flatten(items).Where(i => i.IsVisible))
                item.Status = AnalysisStatus.None;
        }

        /// <summary>Returns true if this item is a plugin (has a ScriptPath).</summary>
        public bool IsPlugin => ScriptPath != null;

        // -- INotifyPropertyChanged --------------------------------

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // -- Converters -----------------

    /// <summary>
    /// XAML converter: IsCategory (bool) > FontWeight.
    /// Categories are shown semi-bold, features normal.
    /// In my WinForms this was just node.NodeFont = bold.
    /// </summary>
    public class BoolToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => value is true ? FontWeights.SemiBold : FontWeights.SemiLight;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// XAML converter: AnalysisStatus > dot Fill color.
    /// None = Transparent (dot hidden), Ok = Green, NeedsFix = Red, NotApplicable = Orange.
    /// </summary>
    public class StatusToBrushConverter : IValueConverter
    {
        private static readonly SolidColorBrush _transparent = new(Colors.Transparent);
        private static readonly SolidColorBrush _green  = new(Color.FromArgb(255, 108, 203, 95));
        private static readonly SolidColorBrush _red    = new(Color.FromArgb(255, 232, 17, 35));
        private static readonly SolidColorBrush _orange = new(Colors.Orange);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is AnalysisStatus status)
            {
                return status switch
                {
                    AnalysisStatus.Ok            => _green,
                    AnalysisStatus.NeedsFix      => _red,
                    AnalysisStatus.NotApplicable => _orange,
                    _                            => _transparent
                };
            }
            return _transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}