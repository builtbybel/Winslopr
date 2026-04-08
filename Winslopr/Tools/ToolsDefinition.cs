using System.Collections.Generic;

namespace Winslop.Tools
{
    // Parsed from script header comments (# Description:, # Category:, etc.)
    public record ScriptMeta
    {
        public string Description { get; init; } = "No description available.";
        public List<string> Options { get; init; } = new();
        public ToolsCategory Category { get; init; } = ToolsCategory.All;
        public bool UseConsole { get; init; }
        public bool UseLog { get; init; }
        public bool SupportsInput { get; init; }
        public string InputPlaceholder { get; init; } = "";
        public string PoweredByText { get; init; } = "";
        public string PoweredByUrl { get; init; } = "";
    }

    public class ToolsDefinition
    {
        public string Title { get; }
        public string Description { get; }
        public string Icon { get; }
        public string ScriptPath { get; }

        // Store category to allow fast filtering without re-reading script files
        public ToolsCategory Category { get; set; } = ToolsCategory.All;

        public List<string> Options { get; } = new();

        // Script host behavior
        public bool UseConsole { get; set; }
        public bool UseLog { get; set; }

        // Optional input support
        public bool SupportsInput { get; set; }
        public string InputPlaceholder { get; set; } = "";

        // Optional attribution link
        public string PoweredByText { get; set; } = "";
        public string PoweredByUrl { get; set; } = "";

        public ToolsDefinition(string title, string icon, string scriptPath, ScriptMeta meta)
        {
            Title = title;
            Description = meta.Description;
            Icon = icon;
            ScriptPath = scriptPath;
            Category = meta.Category;
            UseConsole = meta.UseConsole;
            UseLog = meta.UseLog;
            SupportsInput = meta.SupportsInput;
            InputPlaceholder = meta.InputPlaceholder;
            PoweredByText = meta.PoweredByText;
            PoweredByUrl = meta.PoweredByUrl;
            Options.AddRange(meta.Options);
        }

        public override string ToString() => Title;
    }
}
