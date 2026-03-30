using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Winslop.Helpers;

namespace Winslop
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides application-specific behavior to supplement the default Application class.
        /// </summary>

        // -- Fields -----------------------------------------------

        /// <summary>
        /// The main application window reference is used by NativeMethods for HWND (file picker etc.)
        /// </summary>
        public static Window? MainWindow { get; set; }

        // Tracks the user-selected app theme (Light/Dark/Default).
        // I learned that ContentDialogs don't inherit RequestedTheme from the page tree,
        // so they read this value explicitly to stay in sync with the app theme.
        public static ElementTheme CurrentTheme { get; private set; } = ElementTheme.Default;

        // Feature loading starts immediately in OnLaunched, parallel to window init.
        // FeaturesPage.InitializeAppState awaits this instead of starting its own Task.Run.
        internal static Task<(List<FeatureTreeItem> Features, FeatureTreeItem Plugins)> PreloadTask;


        // -- Constructor ------------------------------------------

        public App()
        {
            // Language must be set before InitializeComponent so WinUI reads it once at startup.
            try
            {
                string savedLang = SettingsHelper.GetLanguage();
                if (!string.IsNullOrWhiteSpace(savedLang))
                    Microsoft.Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = savedLang;
            }
            catch (Exception ex)
            {
                Logger.Log($"Language init failed: {ex.Message}", LogLevel.Error);
            }

            InitializeComponent();

            // Write unhandled managed exceptions to crash.log as native WinUI crashes
            // bypass this handler and won't appear here.
            this.UnhandledException += (s, e) =>
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash.log");
                File.AppendAllText(path, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {e.Message}\n{e.Exception}\n\n");
                e.Handled = false;
            };
        }

        // -- Launch -----------------------------------------------
        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // Start loading features immediately and runs while the window is being created.
            PreloadTask = Task.Run(() => (FeatureNodeManager.LoadFeatures(), PluginManager.LoadPlugins()));

            try
            {
                MainWindow = new MainWindow();
                ApplySavedAppearance(MainWindow);
                CenterWindow(MainWindow, width: 700, height: 800);
                MainWindow.Activate();
            }
            catch (Exception ex)
            {
                Logger.Log($"Window creation failed: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        // -- Helpers ----------------------------------------------

        // Centers the window on the primary display at the given size.
        private static void CenterWindow(Window window, int width, int height)
        {
            var area = Microsoft.UI.Windowing.DisplayArea
                .GetFromWindowId(window.AppWindow.Id, Microsoft.UI.Windowing.DisplayAreaFallback.Primary)
                .WorkArea;

            window.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(
                (area.Width - width) / 2,
                (area.Height - height) / 2,
                width, height));
        }

        // Applies saved appearance (backdrop, theme, title bar colors) after window creation.
        private static void ApplySavedAppearance(Window window)
        {
            if (SettingsHelper.Get("backdrop") == "Base")
                window.SystemBackdrop = new MicaBackdrop { Kind = MicaKind.Base };

            ApplyTheme(window, SettingsHelper.Get("theme"));
        }

        // Applies theme to the window content and syncs title bar button colors.
        // Called at startup (ApplySavedAppearance) and on live theme change in the SettingsPage.
        public static void ApplyTheme(Window window, string? theme)
        {
            if (window.Content is FrameworkElement root)
            {
                root.RequestedTheme = theme switch
                {
                    "Dark"  => ElementTheme.Dark,
                    "Light" => ElementTheme.Light,
                    _       => ElementTheme.Default
                };
                CurrentTheme = root.RequestedTheme;
            }

            if (theme is "Dark" or "Light")
            {
                var fg = theme == "Dark"
                    ? Windows.UI.Color.FromArgb(255, 255, 255, 255)
                    : Windows.UI.Color.FromArgb(255, 0, 0, 0);

                var titleBar = window.AppWindow.TitleBar;
                titleBar.ButtonForegroundColor = fg;
                titleBar.ButtonHoverForegroundColor = fg;
                titleBar.ButtonInactiveForegroundColor = fg;
            }
        }
    }
}