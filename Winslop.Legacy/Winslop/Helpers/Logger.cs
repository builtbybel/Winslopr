using System;
using System.Drawing;
using System.Windows.Forms;

namespace Winslop
{
    /// <summary>
    /// Log level types used to define message severity.
    /// </summary>
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Custom
    }

    /// <summary>
    /// A simple logger class to log messages to a RichTextBox.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// The RichTextBox control to which log messages are written.
        /// </summary>
        public static RichTextBox OutputBox;

        private static readonly Font DefaultFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular);
        private static readonly Font SectionFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);

        /// <summary>
        /// Writes a message with an optional log level and custom font.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="level">The log level (e.g., Info, Warning, Error).</param>
        /// <param name="customFont">An optional font for the message.</param>
        public static void Log(string message, LogLevel level = LogLevel.Info, Font customFont = null)
        {
            if (OutputBox == null)
                return;

            if (OutputBox.InvokeRequired)
            {
                OutputBox.Invoke(new Action(() =>
                    LogInternal(message, level, customFont)));
            }
            else
            {
                LogInternal(message, level, customFont);
            }
        }

        /// <summary>
        /// Starts a new log section, visually separated in the log window.
        /// Example: Logger.BeginSection("Extension / Tool Name");
        /// </summary>
        /// <param name="sectionName">The section name.</param>
        public static void BeginSection(string sectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
                sectionName = "Unnamed Section";

            string header =
                "===== " + sectionName.ToUpper() +
                " (" + DateTime.Now.ToString("HH:mm:ss") + ") =====";

            WriteLine(string.Empty, Color.Black, DefaultFont);
            WriteLine(header, Color.DarkSlateBlue, SectionFont);
            WriteLine(string.Empty, Color.Black, DefaultFont);
        }

        /// <summary>
        /// Internal method to append text to the RichTextBox with formatting.
        /// </summary>
        private static void LogInternal(string message, LogLevel level, Font customFont)
        {
            string fullMessage = message + Environment.NewLine;

            // Set color based on log level
            Color color;
            switch (level)
            {
                case LogLevel.Warning:
                    color = Color.OrangeRed;
                    break;

                case LogLevel.Error:
                    color = Color.Red;
                    break;

                case LogLevel.Custom:
                    color = Color.Magenta;
                    break;

                default:
                    color = Color.Black;
                    break;
            }

            OutputBox.SelectionStart = OutputBox.TextLength;
            OutputBox.SelectionLength = 0;
            OutputBox.SelectionColor = color;
            OutputBox.SelectionFont = customFont ?? DefaultFont;
            OutputBox.AppendText(fullMessage);

            // Reset selection to default
            OutputBox.SelectionColor = OutputBox.ForeColor;
            OutputBox.SelectionFont = DefaultFont;
            OutputBox.ScrollToCaret();
        }

        /// <summary>
        /// Writes a single formatted line with a specific color and font.
        /// </summary>
        private static void WriteLine(string message, Color color, Font font)
        {
            if (OutputBox == null)
                return;

            Action writeAction = () =>
            {
                OutputBox.SelectionStart = OutputBox.TextLength;
                OutputBox.SelectionLength = 0;
                OutputBox.SelectionColor = color;
                OutputBox.SelectionFont = font;
                OutputBox.AppendText(message + Environment.NewLine);

                OutputBox.SelectionColor = OutputBox.ForeColor;
                OutputBox.SelectionFont = DefaultFont;
                OutputBox.ScrollToCaret();
            };

            if (OutputBox.InvokeRequired)
                OutputBox.Invoke(writeAction);
            else
                writeAction();
        }

        /// <summary>
        /// Clears the log output.
        /// </summary>
        public static void Clear()
        {
            if (OutputBox == null || OutputBox.IsDisposed)
                return;

            if (OutputBox.InvokeRequired)
            {
                OutputBox.Invoke(new Action(Clear));
                return;
            }

            OutputBox.Clear();
            OutputBox.SelectionColor = Color.Black;
        }
    }
}