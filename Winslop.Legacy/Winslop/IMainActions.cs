using System.Threading.Tasks;

/// <summary>
/// Defines a contract for a view that can be refreshed to update its displayed content.
/// </summary>
public interface IView
{
    /// <summary>
    /// Refreshes the view. Used when the user clicks refresh or returns to the page.
    /// </summary>
    void RefreshView();
}

/// <summary>
/// Defines actions a page can expose to the shell (Analyze/Fix).
/// </summary>
public interface IMainActions
{
    Task AnalyzeAsync();
    Task FixAsync();
}

/// <summary>
/// Exposes a simple search API for views that support filtering.
/// </summary>
public interface ISearchable
{
    /// <summary>
    /// Applies a search query (empty/null should reset the filter).
    /// </summary>
    void ApplySearch(string query);
}

