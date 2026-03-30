using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Winslopr.Helpers;
using Winslopr.Services;
using Winslopr.Views;
using Winsloprr.Services;

namespace Winslopr
{
    public sealed partial class MainWindow : Window
    {
        private readonly Dictionary<string, Type> _pages = new()
        {
            { "Home",    typeof(FeaturesPage) },
            { "Apps",    typeof(AppsPage) },
            { "Install", typeof(InstallPage) },
            { "Tools",   typeof(ToolsPage) }
            // Settings is an overlay panel, not a Frame page
        };

        // Services initialized in constructor
        private NavigationService _nav = null!;

        private MenuActionRouter _router = null!;
        private LoggerDisplay _loggerDisplay = null!;
        private LoggerActions? _logActions;

        private bool _closeHandled;

        public MainWindow()
        {
            InitializeComponent();

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);

            // Mark gear button as passthrough so it receives clicks (not treated as drag region)
            AppTitleBar.Loaded += (_, _) => UpdatePassthrough();
            AppTitleBar.SizeChanged += (_, _) => UpdatePassthrough();

            // -- Services -----------------------------------------
            var navButtons = new[] { navBtnFeatures, navBtnApps, navBtnInstall, navBtnTools };

            _nav = new NavigationService(
                ContentFrame, navButtons, _pages, (FrameworkElement)Content);
            _router = new MenuActionRouter(ContentFrame);
            _loggerDisplay = new LoggerDisplay(txtLogger, scrollLogger, DispatcherQueue);
            _logActions = new LoggerActions();

            // Wire log actions to FeaturesPage tree after each navigation
            ContentFrame.Navigated += OnContentFrameNavigated;

            // Donation dialog on close (unless opted out)
            Closed += MainWindow_Closed;

            // Navigate to the default page after everything is set up
            DispatcherQueue.TryEnqueue(
                Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,
                () => _nav.NavigateToDefault("Home"));
        }

        // Marks the gear button rect as Passthrough
        private void UpdatePassthrough()
        {
            if (btnTitleSettings.XamlRoot == null) return;
            double scale = btnTitleSettings.XamlRoot.RasterizationScale;
            var pos = btnTitleSettings.TransformToVisual(null)
                                      .TransformPoint(new Windows.Foundation.Point(0, 0));
            Microsoft.UI.Input.InputNonClientPointerSource.GetForWindowId(AppWindow.Id)
                .SetRegionRects(Microsoft.UI.Input.NonClientRegionKind.Passthrough,
                [new Windows.Graphics.RectInt32(
                    (int)(pos.X * scale), (int)(pos.Y * scale),
                    (int)(btnTitleSettings.ActualWidth  * scale),
                    (int)(btnTitleSettings.ActualHeight * scale))]);
        }

        // -- Navigation -------------------------------------------

        // Handle nav button clicks and navigate and update visual state
        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
                _nav.NavigateTo(tag);
        }

        private void NavSettings_Click(object sender, RoutedEventArgs e)
        {
            bool isOpening = settingsOverlayBackdrop.Visibility != Visibility.Visible;

            // Load SettingsPage into the overlay only once (so lazy again)
            if (isOpening)
                settingsFrame.Navigate(typeof(SettingsPage), null,
                    new Microsoft.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());

            // Toggle: opens on first click, closes on second click
            settingsOverlayBackdrop.Visibility = isOpening ? Visibility.Visible : Visibility.Collapsed;
        }

        // Used by SettingsPage to navigate to another page and close the overlay
        public void NavigateToPage(string tag)
        {
            settingsOverlayBackdrop.Visibility = Visibility.Collapsed;
            _nav.NavigateTo(tag);
        }

        /// <summary>
        /// Called after every page navigation.
        /// Wires up LogActions and adjusts menu/button state per page.
        /// Settings is a separate overlay panel and never navigates through ContentFrame.
        /// </summary>
        private void OnContentFrameNavigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var page = ContentFrame.Content;

            // Give LogActions access to the feature tree when FeaturesPage is active
            if (page is FeaturesPage fp)
                _logActions?.SetFeaturesItemsProvider(() => fp.RootItems);

            bool isFeatures = page is FeaturesPage;
            bool showButtons = page is not ToolsPage; // Tools has its own action UI

            // Menu items: Undo only on FeaturesPage, Toggle on all except Tools
            MenuUndo.IsEnabled = isFeatures;
            MenuToggle.IsEnabled = showButtons;

            // Inspect/Apply buttons hidden on Tools (handled by ToolsPage itself)
            bottomButtons.Visibility = showButtons ? Visibility.Visible : Visibility.Collapsed;
        }

        // -- Button handlers --------------------------------------

        private async void btnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            var actions = _router.CurrentActions();
            if (actions == null) return;
            btnAnalyze.IsEnabled = btnFix.IsEnabled = false;
            try { await actions.AnalyzeAsync(); }
            finally { btnAnalyze.IsEnabled = btnFix.IsEnabled = true; }
        }

        private async void btnFix_Click(object sender, RoutedEventArgs e)
        {
            var actions = _router.CurrentActions();
            if (actions == null) return;
            btnAnalyze.IsEnabled = btnFix.IsEnabled = false;
            try { await actions.FixAsync(); }
            finally { btnAnalyze.IsEnabled = btnFix.IsEnabled = true; }
        }

        // -- Search -----------------------------------------------

        private void textSearch_TextChanged(object sender, TextChangedEventArgs e)
            => _router.CurrentSearchable()?.ApplySearch(textSearch.Text);

        private void textSearch_GotFocus(object sender, RoutedEventArgs e)
            => textSearch.Text = string.Empty;

        // -- More options menu ---------------------------------------

        private void MenuToggleAll_Click(object sender, RoutedEventArgs e)
            => _router.ToggleAll();

        private void MenuUndo_Click(object sender, RoutedEventArgs e)
            => _router.Undo();

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
            => _router.Refresh();

        // -- Log actions ------------------------------------------
        private void MenuInspectLog_Click(object sender, RoutedEventArgs e)
            => _logActions?.AnalyzeOnline(ExternalLinks.LogInspectorUrl);

        private void MenuCopyLog_Click(object sender, RoutedEventArgs e)
            => _logActions?.CopyToClipboard();

        private void MenuClearLog_Click(object sender, RoutedEventArgs e)
            => _logActions?.Clear();

        private void MenuLogChecked_Click(object sender, RoutedEventArgs e)
            => _logActions?.LogCheckedFeatures();

        private void MenuLogUnchecked_Click(object sender, RoutedEventArgs e)
            => _logActions?.LogUncheckedLeafFeatures();

        private void MenuLogSummary_Click(object sender, RoutedEventArgs e)
            => _logActions?.LogFeatureSummary();

        // -- Support links (Share / Ko-fi / PayPal flyout) -----------

        private void MenuShare_Click(object sender, RoutedEventArgs e)
            => ShareHelper.ShowShareUI(WinRT.Interop.WindowNative.GetWindowHandle(this));

        private void MenuKofi_Click(object sender, RoutedEventArgs e)
            => ExternalLinks.OpenKofi();

        private void MenuPaypal_Click(object sender, RoutedEventArgs e)
            => ExternalLinks.OpenPaypal();

        // -- Closing ----------------------------------------------

        private async void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            if (_closeHandled || DonationHelper.HasDonated())
                return;

            args.Handled = true;
            await DonationHelper.ShowDonationDialogAsync(Content.XamlRoot);
            _closeHandled = true;
            Close();
        }
    }
}
