using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Winslop
{
    public static class DonationHelper
    {
        // Donation status file stored next to the exe (portable-friendly)
        private static readonly string FilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Winslop.txt");

        // Marker that indicates the user donated
        private const string DonationFlag = "#donated=true";

        /// <summary>
        /// Returns true if the donation flag is present in the status file.
        /// </summary>
        public static bool HasDonated()
        {
            if (!File.Exists(FilePath))
                return false;

            return File.ReadAllLines(FilePath)
                .Any(line => string.Equals(line.Trim(), DonationFlag, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Writes or removes the donation flag in the status file.
        /// </summary>
        public static void SetDonationStatus(bool donated)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

            var lines = File.Exists(FilePath)
                ? File.ReadAllLines(FilePath).ToList()
                : new System.Collections.Generic.List<string>();

            // Remove any existing donated flags
            lines.RemoveAll(l => l.Trim().StartsWith("#donated", StringComparison.OrdinalIgnoreCase));

            if (donated)
                lines.Add(DonationFlag);

            File.WriteAllLines(FilePath, lines);
        }

        /// <summary>
        /// Shows a short donation prompt (optional/voluntary).
        /// </summary>
        public static void ShowDonationPrompt()
        {
            string message =
              "Winslop is intentionally small, plain and functional — anti-slop by design.\n\n" +
              "If Winslop helped you remove bloat, speed things up, or regain control of Windows,\n" +
              "please consider a small voluntary donation.\n\n" +
              "It keeps development going and helps keep everything free for everyone.\n\n" +
              "Open the PayPal donation link now?";


            var result = MessageBox.Show(
                message,
                "Support Winslop",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (result != DialogResult.Yes)
                return;

            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.paypal.com/donate/?hosted_button_id=BNVXAGPQ8CTR6",
                UseShellExecute = true
            });
        }
    }
}
