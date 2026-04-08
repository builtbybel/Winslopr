using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;

namespace Winslopr.Services
{
    /// <summary>
    /// Renders <see cref="LogEntry"/> items into a <see cref="RichTextBlock"/>
    /// and keeps the <see cref="ScrollViewer"/> pinned to the bottom.
    /// </summary>
    public sealed class LoggerDisplay
    {
        private readonly RichTextBlock _target;
        private readonly ScrollViewer _scroll;
        private readonly DispatcherQueue _dispatcher;

        public LoggerDisplay(RichTextBlock target, ScrollViewer scroll, DispatcherQueue dispatcher)
        {
            _target = target;
            _scroll = scroll;
            _dispatcher = dispatcher;

            Logger.LogAdded += OnLogAdded;
            Logger.LogCleared += OnLogCleared;
        }

        /// <summary>
        /// Unsubscribe from global logger events (call on window close if needed).
        /// </summary>
        public void Detach()
        {
            Logger.LogAdded -= OnLogAdded;
            Logger.LogCleared -= OnLogCleared;
        }

        private void OnLogAdded(LogEntry entry)
        {
            _dispatcher.TryEnqueue(() =>
            {
                var paragraph = new Paragraph();
                var run = new Run { Text = entry.Message };
                var fg = BrushForLevel(entry.Level);
                if (fg != null) run.Foreground = fg; // null = inherit from RichTextBlock (theme-sensitive)
                paragraph.Inlines.Add(run);
                _target.Blocks.Add(paragraph);
                _target.UpdateLayout(); // ensure new content is measured before scrolling
                _scroll.ChangeView(null, _scroll.ScrollableHeight, null, true);
            });
        }

        private void OnLogCleared()
            => _dispatcher.TryEnqueue(() => _target.Blocks.Clear());

        // Returns null for default levels: Run inherits foreground from RichTextBlock (theme-sensitive).
        // Only Error/Warning get an explicit color.
        private static Brush? BrushForLevel(LogLevel level) => level switch
        {
            LogLevel.Error   => new SolidColorBrush(Microsoft.UI.Colors.IndianRed),
            LogLevel.Warning => new SolidColorBrush(Microsoft.UI.Colors.Chocolate),
            _                => null
        };
    }
}