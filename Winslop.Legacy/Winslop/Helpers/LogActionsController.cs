using System;
using System.Windows.Forms;

/// <summary>
/// Manages log-related actions using a ComboBox in MainForm.
/// </summary>
public class LogActionsController
{
    private readonly ComboBox _combo;
    private readonly LogActions _logActions;

    public LogActionsController(ComboBox comboLogActions, LogActions logActions)
    {
        if (comboLogActions == null) throw new ArgumentNullException(nameof(comboLogActions));
        if (logActions == null) throw new ArgumentNullException(nameof(logActions));

        _combo = comboLogActions;
        _logActions = logActions;

        Initialize();
    }

    private void Initialize()
    {
        _combo.Items.Clear();
        _combo.Items.Add("Select an action...");
        _combo.Items.Add("Inspect log online (recommended)");
        _combo.Items.Add("Copy log to clipboard");
        _combo.Items.Add("Clear log");

        // Feature-related log tools (optional)
        _combo.Items.Add("Log: checked features");
        _combo.Items.Add("Log: unchecked leaf features");
        _combo.Items.Add("Log: summary (total/checked)");

        _combo.SelectedIndex = 0;
        _combo.SelectedIndexChanged += Combo_SelectedIndexChanged;
    }


    private void Combo_SelectedIndexChanged(object sender, EventArgs e)
    {
        string selected = _combo.SelectedItem as string;
        if (string.IsNullOrEmpty(selected))
        {
            Reset();
            return;
        }

        switch (selected)
        {
            case "Inspect log online (recommended)":
                _logActions.AnalyzeOnline("https://builtbybel.github.io/Winslop/log-inspector/index.html");
                break;

            case "Copy log to clipboard":
                _logActions.CopyToClipboard();
                break;

            case "Clear log":
                _logActions.Clear();
                break;

            case "Log: checked features":
                _logActions.LogCheckedFeatures();
                break;

            case "Log: unchecked leaf features":
                _logActions.LogUncheckedLeafFeatures();
                break;

            case "Log: summary (total/checked)":
                _logActions.LogFeatureSummary();
                break;
        }

        Reset();
    }

    private void Reset()
    {
        // Reset the combo box to the default selection
        _combo.SelectedIndex = 0;
    }
}