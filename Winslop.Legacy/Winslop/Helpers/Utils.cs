using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Winslop
{
    internal static class Utils
    {
        private const string GitHubUrl = "https://github.com/builtbybel/Winslop";

        /// <summary>
        /// Checks if a registry value equals a specified integer.
        /// </summary>
        public static bool IntEquals(string keyName, string valueName, int expectedValue)
        {
            try
            {
                object value = Registry.GetValue(keyName, valueName, null);
                return value is int intValue && intValue == expectedValue;
            }
            catch (Exception ex)
            {
                Logger.Log($"Registry check failed for {keyName}\\{valueName}: {ex.Message}", LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// Checks if a registry value equals a specified string.
        /// </summary>
        public static bool StringEquals(string keyName, string valueName, string expectedValue)
        {
            try
            {
                object value = Registry.GetValue(keyName, valueName, null);
                return value is string strValue && strValue == expectedValue;
            }
            catch (Exception ex)
            {
                Logger.Log($"Registry check failed for {keyName}\\{valueName}: {ex.Message}", LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// Opens the GitHub project page in the default browser.
        /// </summary>
        public static void OpenGitHubPage(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = GitHubUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to open GitHub page: {ex.Message}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Restarts Windows Explorer to apply UI changes.
        /// </summary>
        public static void RestartExplorer()
        {
            try
            {
                Logger.Log("Restarting Windows Explorer to apply UI changes...", LogLevel.Info);

                foreach (var process in Process.GetProcessesByName("explorer"))
                {
                    process.Kill();
                    process.WaitForExit();
                }

                Process.Start("explorer.exe");
                Logger.Log("Explorer restarted successfully.", LogLevel.Info);
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to restart Explorer: {ex.Message}", LogLevel.Error);
            }
        }
    }
}
