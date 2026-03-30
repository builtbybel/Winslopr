using Microsoft.UI.Xaml.Controls;
using Winslopr.Views;

namespace Winslopr.Services
{
    /// <summary>
    /// Routes the "Actions dropdown" menu commands to whichever page is
    /// currently loaded in the <see cref="Frame"/>.
    /// </summary>
    public sealed class MenuActionRouter
    {
        private readonly Frame _frame;

        /// <param name="frame">The main content frame whose current page receives the routed commands.</param>
        public MenuActionRouter(Frame frame)
        {
            _frame = frame;
        }

        // The current page, or null. All methods below safely
        // no-op when the page doesn't implement the expected interface.
        private object? Page => _frame.Content;

        // -- Toggle All -------------------------------------------
        public void ToggleAll()
        {
            switch (Page)
            {
                case FeaturesPage fp: fp.ToggleSelection(); break;
                case AppsPage ap: ap.ToggleSelection(); break;
                case InstallPage ip: ip.ToggleSelection(); break;
            }
        }

        // -- Undo (FeaturesPage only) -----------------------------
        public void Undo()
        {
            if (Page is FeaturesPage fp)
                fp.RestoreSelection();
        }

        // -- Refresh (IView) --------------------------------------
        // IView.RefreshView() — reloads the page's displayed content.
        public void Refresh()
            => (Page as IView)?.RefreshView();

        // -- Analyze / Fix (IMainActions) -------------------------
        // IMainActions exposes AnalyzeAsync() and FixAsync() to the shell.
        public IMainActions? CurrentActions() => Page as IMainActions;

        // -- Search (ISearchable) ---------------------------------
        // ISearchable.ApplySearch() — filters the page; empty string resets.
        public ISearchable? CurrentSearchable() => Page as ISearchable;
    }
}