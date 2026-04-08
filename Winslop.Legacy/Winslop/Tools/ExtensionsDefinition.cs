using System.Collections.Generic;

namespace Winslop.Extensions
{
    public class ExtensionsDefinition
    {
        public string Title { get; }
        public string Description { get; }
        public string Icon { get; }
        public string ScriptPath { get; }

        // Store category to allow fast filtering without re-reading script files
        public ExtensionsCategory Category { get; set; } = ExtensionsCategory.All;

        public List<string> Options { get; } = new List<string>();

        // Script host behavior
        public bool UseConsole { get; set; } = false;
        public bool UseLog { get; set; } = false;

        // Optional input support
        public bool SupportsInput { get; set; } = false;
        public string InputPlaceholder { get; set; } = string.Empty;

        // Optional attribution link
        public string PoweredByText { get; set; } = string.Empty;
        public string PoweredByUrl { get; set; } = string.Empty;

        public ExtensionsDefinition(string title, string description, string icon, string scriptPath)
        {
            Title = title;
            Description = description;
            Icon = icon;
            ScriptPath = scriptPath;
        }

        // Helps the ListBox display, even if DisplayMember is not set
        public override string ToString() => Title;
    }
}
