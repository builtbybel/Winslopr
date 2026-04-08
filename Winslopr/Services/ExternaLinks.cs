using System.Diagnostics;

namespace Winsloprr.Services
{
    /// <summary>
    /// Central registry of every external URL the app can open.
    /// </summary>
    public static class ExternalLinks
    {
        // -- Donation / support -----------------------------------
        public static void OpenKofi()
            => Open("https://ko-fi.com/builtbybel");

        public static void OpenPaypal()
            => Open("https://www.paypal.com/donate/?hosted_button_id=BNVXAGPQ8CTR6");

        // -- Project ----------------------------------------------
        public static void OpenGitHub()
            => Open("https://github.com/builtbybel/Winslopr");

        public static void OpenHelp()
            => Open("https://github.com/builtbybel/Winslopr/blob/main/docs/Help.md");

        public static void OpenFeedback()
            => Open("https://github.com/builtbybel/Winslopr/issues");

        public static void OpenUpdateCheck(string rawVersion)
            => Open($"https://builtbybel.github.io/Winslopr/update.html?version={rawVersion}");

        // -- Log inspector ----------------------------------------
        public const string LogInspectorUrl = "https://builtbybel.github.io/Winslopr/log-inspector/index.html";

        public static void OpenLogInspector()
            => Open(LogInspectorUrl);

        // -- Helper -----------------------------------------------
        private static void Open(string url)
            => Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
    }
}
