using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Winslop.Extensions
{
    public partial class ExtensionsItemControl : UserControl
    {
        // The tool currently shown in this details panel
        private ExtensionsDefinition _tool;

        private const string DefaultPlaceholder = "Enter input (e.g., IDs or raw arguments)";

        /// <summary>
        /// Fired after a script was deleted via Uninstall button.
        /// The master view should reload the list after this.
        /// </summary>
        public event EventHandler ToolUninstalled;

        /// <summary>
        /// Parameterless constructor is important for the WinForms Designer
        /// and also allows us to create this control in code.
        /// </summary>
        public ExtensionsItemControl()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            ClearUI();
        }

        /// <summary>
        /// Optional convenience constructor.
        /// </summary>
        public ExtensionsItemControl(ExtensionsDefinition tool) : this()
        {
            SetTool(tool);
        }

        /// <summary>
        /// Sets the tool to show in this control (details mode).
        /// Called when selection changes in the master list
        /// </summary>
        public void SetTool(ExtensionsDefinition tool)
        {
            _tool = tool;

            if (_tool == null)
            {
                ClearUI();
                return;
            }

            // Basic UI
            labelTitle.Text = _tool.Title ?? "";
            labelDescription.Text = _tool.Description ?? "";
            labelIcon.Text = _tool.Icon ?? "";

            progressBar.Visible = false;
            labelStatus.Text = "";

            // Options dropdown
            comboOptions.Items.Clear();
            if (_tool.Options != null && _tool.Options.Count > 0)
            {
                comboOptions.Visible = true;
                comboOptions.Items.AddRange(_tool.Options.ToArray());
                comboOptions.SelectedIndex = 0;
            }
            else
            {
                comboOptions.Visible = false;
            }

            // Input textbox
            if (textInput != null)
            {
                textInput.Visible = _tool.SupportsInput;
                if (_tool.SupportsInput)
                {
                    var placeholder = !string.IsNullOrWhiteSpace(_tool.InputPlaceholder)
                        ? _tool.InputPlaceholder
                        : DefaultPlaceholder;

                    SetupPlaceholder(textInput, placeholder);
                }
            }

            // Powered by link
            SetupPoweredBy();

            // Enable actions
            btnRun.Visible = true;
            btnUninstall.Visible = true;
        }

        /// <summary>
        /// Clears UI when no tool is selected.
        /// </summary>
        private void ClearUI()
        {
            _tool = null;

            labelTitle.Text = "";
            labelDescription.Text = "";
            labelIcon.Text = "";

            comboOptions.Visible = false;
            comboOptions.Items.Clear();

            if (textInput != null)
            {
                textInput.Visible = false;
                textInput.Text = "";
            }

            linkPoweredBy.Visible = false;

            progressBar.Visible = false;
            labelStatus.Text = "Select an extension on the left.";

            btnRun.Visible = false;
            btnUninstall.Visible = false;
        }

        /// <summary>
        /// Ensures placeholder behavior works without stacking event handlers.
        /// </summary>
        private void SetupPlaceholder(TextBox tb, string placeholder)
        {
            // Avoid stacking handlers if SetTool is called multiple times
            tb.GotFocus -= TextInput_GotFocus;
            tb.LostFocus -= TextInput_LostFocus;

            tb.Text = placeholder;
            tb.ForeColor = Color.Gray;

            tb.GotFocus += TextInput_GotFocus;
            tb.LostFocus += TextInput_LostFocus;
        }

        private void TextInput_GotFocus(object sender, EventArgs e)
        {
            if (_tool == null || textInput == null) return;

            var placeholder = !string.IsNullOrWhiteSpace(_tool.InputPlaceholder)
                ? _tool.InputPlaceholder
                : DefaultPlaceholder;

            if (textInput.Text == placeholder)
            {
                textInput.Text = "";
                textInput.ForeColor = SystemColors.WindowText;
            }
        }

        private void TextInput_LostFocus(object sender, EventArgs e)
        {
            if (_tool == null || textInput == null) return;

            var placeholder = !string.IsNullOrWhiteSpace(_tool.InputPlaceholder)
                ? _tool.InputPlaceholder
                : DefaultPlaceholder;

            if (string.IsNullOrWhiteSpace(textInput.Text))
            {
                textInput.Text = placeholder;
                textInput.ForeColor = Color.Gray;
            }
        }

        private void SetupPoweredBy()
        {
            // Unhook handler to avoid duplicates
            linkPoweredBy.LinkClicked -= linkPoweredBy_LinkClicked;

            if (_tool == null ||
                string.IsNullOrWhiteSpace(_tool.PoweredByText) ||
                string.IsNullOrWhiteSpace(_tool.PoweredByUrl))
            {
                linkPoweredBy.Visible = false;
                return;
            }

            linkPoweredBy.Text = _tool.PoweredByText.Trim();
            linkPoweredBy.Tag = _tool.PoweredByUrl.Trim();
            linkPoweredBy.Visible = true;

            linkPoweredBy.LinkClicked += linkPoweredBy_LinkClicked;

            // Accessibility (optional)
            linkPoweredBy.AccessibleName = "Powered by link";
            linkPoweredBy.AccessibleDescription = "Opens the developer's website";
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            if (_tool == null)
                return;

            if (!File.Exists(_tool.ScriptPath))
            {
                MessageBox.Show("Script not found: " + _tool.ScriptPath, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnRun.Enabled = false;
            btnUninstall.Enabled = false;

            progressBar.Visible = true;
            progressBar.Style = ProgressBarStyle.Marquee;
            labelStatus.Text = "Running...";

            try
            {
                // Defaults from # Host
                bool useConsole = _tool.UseConsole;
                bool useLog = _tool.UseLog;

                // Selected option text (may carry host-suffix overrides)
                string optionArg = null;
                if (comboOptions != null && comboOptions.Visible && comboOptions.SelectedItem != null)
                {
                    optionArg = comboOptions.SelectedItem.ToString();

                    if (optionArg.EndsWith(" (console)", StringComparison.Ordinal))
                    {
                        useConsole = true; useLog = false;
                        optionArg = optionArg.Substring(0, optionArg.Length - " (console)".Length).Trim();
                    }
                    else if (optionArg.EndsWith(" (silent)", StringComparison.Ordinal))
                    {
                        useConsole = false; useLog = false;
                        optionArg = optionArg.Substring(0, optionArg.Length - " (silent)".Length).Trim();
                    }
                    else if (optionArg.EndsWith(" (log)", StringComparison.Ordinal))
                    {
                        useLog = true; useConsole = false;
                        optionArg = optionArg.Substring(0, optionArg.Length - " (log)".Length).Trim();
                    }
                }

                // Optional free text argument (only if visible and real input)
                string inputArg = null;
                if (_tool.SupportsInput && textInput != null && textInput.Visible)
                {
                    var t = (textInput.Text ?? string.Empty).Trim();
                    var placeholder = !string.IsNullOrWhiteSpace(_tool.InputPlaceholder)
                        ? _tool.InputPlaceholder
                        : DefaultPlaceholder;

                    if (!string.IsNullOrEmpty(t) && !string.Equals(t, placeholder, StringComparison.Ordinal))
                        inputArg = t;
                }

                // Build positional argument string:
                //   "<optionArg>" "<inputArg>"
                var extraArgs = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(optionArg))
                    extraArgs.Append(" ").Append(QuoteForPs(optionArg));
                if (!string.IsNullOrWhiteSpace(inputArg))
                    extraArgs.Append(" ").Append(QuoteForPs(inputArg));

                if (useLog)
                {
                    // Start a new visual log section for this tool run
                    Logger.BeginSection($"Running {_tool.Title ?? _tool.ScriptPath}");
                }

                // Run script (console vs. silent) and stream logs
                var output = await RunScriptAsync(_tool.ScriptPath, extraArgs.ToString(), useConsole);

                labelStatus.Text = useConsole ? "Opened in console."
                                  : useLog ? "Completed with log."
                                           : "Done.";

                if (!string.IsNullOrWhiteSpace(output))
                    Logger.Log(output, LogLevel.Info);
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Error: " + ex.Message;
                Logger.Log("ERROR: " + ex.Message, LogLevel.Error);
            }
            finally
            {
                progressBar.Visible = false;
                btnRun.Enabled = true;
                btnUninstall.Enabled = true;
            }
        }

        private Task<string> RunScriptAsync(string scriptPath, string positionalArgs, bool useConsole)
        {
            return Task.Run(() =>
            {
                var argsForPs = $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\"{positionalArgs}";

                if (useConsole)
                {
                    var psi = new ProcessStartInfo("powershell.exe", "-NoExit " + argsForPs)
                    {
                        UseShellExecute = true,
                        CreateNoWindow = false
                    };
                    Process.Start(psi);
                    return "Launched in external console.";
                }
                else
                {
                    var psi = new ProcessStartInfo("powershell.exe", argsForPs)
                    {
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    var sb = new StringBuilder();
                    using (var p = new Process { StartInfo = psi })
                    {
                        p.OutputDataReceived += (s, e) =>
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                                Logger.Log(e.Data, LogLevel.Info);
                        };

                        p.ErrorDataReceived += (s, e) =>
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                            {
                                sb.AppendLine("ERROR: " + e.Data);
                                Logger.Log("ERROR: " + e.Data, LogLevel.Error);
                            }
                        };

                        p.Start();
                        p.BeginOutputReadLine();
                        p.BeginErrorReadLine();
                        p.WaitForExit();
                    }

                    return sb.ToString();
                }
            });
        }

        /// <summary>
        /// Quotes a string for PowerShell positional args.
        /// </summary>
        private static string QuoteForPs(string value)
        {
            if (value == null) return "\"\"";
            var escaped = value.Replace("\"", "\\\"");
            return "\"" + escaped + "\"";
        }

        private void linkPoweredBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var url = linkPoweredBy.Tag?.ToString();
                if (string.IsNullOrWhiteSpace(url))
                    return;

                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open link:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUninstall_Click(object sender, EventArgs e)
        {
            try
            {
                if (_tool == null)
                    return;

                if (!File.Exists(_tool.ScriptPath))
                {
                    MessageBox.Show("File already missing:\n" + _tool.ScriptPath,
                        "Uninstall", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Notify master to reload list
                    ToolUninstalled?.Invoke(this, EventArgs.Empty);

                    // Clear details panel
                    ClearUI();
                    return;
                }

                var confirm = MessageBox.Show(
                    "Do you want to remove this extension?\n\n" + (_tool.Title ?? _tool.ScriptPath),
                    "Confirm uninstall",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation);

                if (confirm != DialogResult.Yes)
                    return;

                File.Delete(_tool.ScriptPath);

                // Notify master view so it can refresh the left list
                ToolUninstalled?.Invoke(this, EventArgs.Empty);

                // Clear the details UI after uninstall
                ClearUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not delete script:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
