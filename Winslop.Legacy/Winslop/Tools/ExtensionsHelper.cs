using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;


// DEPRECATED: This class is no longer used but kept for reference.
namespace Winslop.Extensions
{
    /// <summary>
    /// Provides helper methods for installing and managing extension scripts (.ps1).
    /// </summary>
    public static class ExtensionsHelper
    {
        /// <summary>
        /// Open the extensions scripts folder in Explorer.
        /// </summary>
        public static void OpenScriptsFolder(IWin32Window owner)
        {
            string scriptDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scripts");
            Directory.CreateDirectory(scriptDirectory);

            try
            {
                Process.Start("explorer.exe", scriptDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, "Could not open folder: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Open the online guide for writing extensions.
        /// </summary>
        public static void OpenExtensionGuide()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/builtbybel/Winslop/blob/main/docs/extensions.md",
                UseShellExecute = true
            });
        }
    }
}