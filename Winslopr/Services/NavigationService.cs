using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;

namespace Winslopr.Services
{
    /// <summary>
    /// Manages page navigation and nav-button highlight state.
    /// </summary>
    public sealed class NavigationService
    {
        private readonly Frame _frame;
        private readonly Button[] _navButtons;
        private readonly Dictionary<string, Type> _pages;
        private readonly FrameworkElement _themeRoot;

        public string CurrentTag { get; private set; } = "Home";

        public NavigationService(
            Frame frame,
            Button[] navButtons,
            Dictionary<string, Type> pages,
            FrameworkElement themeRoot)
        {
            _frame = frame;
            _navButtons = navButtons;
            _pages = pages;
            _themeRoot = themeRoot;
            // Re-apply nav highlight on theme change to fix color glitches
            _themeRoot.ActualThemeChanged += (s, e) => UpdateHighlight(CurrentTag);

        }

        /// <summary>
        /// Navigate to the page associated with <paramref name="tag"/>.
        /// Returns false if the tag is already active or unknown.
        /// </summary>
        public bool NavigateTo(string tag)
        {
            if (tag == CurrentTag) return false;
            if (!_pages.TryGetValue(tag, out var pageType)) return false;

            //  _frame.Navigate(pageType); SuppressNavigationTransitionInfo disables the default "drill in" animation
            _frame.Navigate(pageType, null, new SuppressNavigationTransitionInfo());
            UpdateHighlight(tag);
            return true;
        }

        /// <summary>
        /// Navigate to the default page (used during startup).
        /// </summary>
        public void NavigateToDefault(string tag = "Home")
        {
            if (_pages.TryGetValue(tag, out var pageType))
            {

                _frame.Navigate(pageType, null, new SuppressNavigationTransitionInfo());
                UpdateHighlight(tag);
            }
        }

        /// <summary>
        /// Update nav buttons: active = icon only (accent bg),
        /// inactive = icon + text (grey, transparent bg).
        /// </summary>
        public void UpdateHighlight(string activeTag)
        {
            CurrentTag = activeTag;

            bool isLight = _themeRoot.ActualTheme == ElementTheme.Light;

            // SystemAccentColor is theme-independent — same raw value in light and dark
            var accentColor = (Windows.UI.Color)Application.Current.Resources["SystemAccentColor"];
            var accentBrush = new SolidColorBrush(accentColor);
            var activeBg    = new SolidColorBrush(Windows.UI.Color.FromArgb(30, accentColor.R, accentColor.G, accentColor.B)); // ca. 12% accent tint, visible in both themes

            foreach (var btn in _navButtons)
            {
                bool isActive = btn.Tag is string t && t == activeTag;

                // Pill background on active, transparent on inactive
                btn.Background = isActive
                    ? activeBg
                    : new SolidColorBrush(Microsoft.UI.Colors.Transparent);

                if (btn.Content is StackPanel sp && sp.Children.Count >= 2 &&
                    sp.Children[0] is FontIcon icon && sp.Children[1] is TextBlock lbl)
                {
                    if (isActive)
                    {
                        icon.Foreground = accentBrush;
                        lbl.Foreground  = accentBrush;
                    }
                    else
                    {
                        // ClearValue lets the XAML ThemeResource binding take over > live theme switch baby
                        icon.ClearValue(IconElement.ForegroundProperty);
                        lbl.ClearValue(TextBlock.ForegroundProperty);
                    }
                    lbl.Visibility = Visibility.Visible;
                }
            }
        }
    }
}