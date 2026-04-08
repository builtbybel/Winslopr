using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using Winslop;

namespace Settings.UI
{
    internal class CleanTaskbar : FeatureBase
    {
        private const string keyName = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Taskband";
        private const string valueName = "Favorites";

        public override string ID()
        {
            return "Clean Taskbar";
        }

        public override string GetFeatureDetails()
        {
            // So this is an action feature, not a simple recommended-value check
            return $"{keyName} | Value: {valueName} | Action: Clear pinned taskbar items (Favorites)";
        }

        public override Task<bool> CheckFeature()
        {
            // Action features are not idempotent checks; return true to avoid showing as an "issue".
            return Task.FromResult(true);
        }

        public override Task<bool> DoFeature()
        {
            try
            {
                // Clear pinned taskbar items by emptying the Favorites binary value.
                Registry.SetValue(keyName, valueName, new byte[0], RegistryValueKind.Binary);

                Utils.RestartExplorer();

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Logger.Log("Code red in " + ex.Message, LogLevel.Error);
                return Task.FromResult(false);
            }
        }

        public override bool UndoFeature()
        {
            try
            {
                // There is no reliable "undo" without exporting the previous Favorites blob first.
                // Returning false makes it clear this action cannot be reverted automatically.
                return false;
            }
            catch (Exception ex)
            {
                Logger.Log("Code red in " + ex.Message, LogLevel.Error);
                return false;
            }
        }
    }
}