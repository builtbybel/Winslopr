using Microsoft.Win32;
using Settings.UI;
using System;
using System.Threading.Tasks;
using Winslop;

namespace Settings.Personalization
{
    internal class AppDarkMode : FeatureBase
    {
        private const string keyName = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string valueName = "AppsUseLightTheme";
        private const int recommendedValue = 0;

        public override string GetFeatureDetails()
        {
            return $"{keyName} | Value: {valueName} | Recommended: {recommendedValue} (Dark mode – preferred for a modern look, but up to you)";
        }
        public override string HelpAnchorId()
        {
            return ID();
        }

        public override string ID()
        {
            return "Enable Dark Mode for Apps";
        }

        public override Task<bool> CheckFeature()
        {
            return Task.FromResult(
                   Utils.IntEquals(keyName, valueName, recommendedValue)
             );
        }

        public override Task<bool> DoFeature()
        {
            try
            {
                Registry.SetValue(keyName, valueName, 0, RegistryValueKind.DWord);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Logger.Log("Error in AppDarkMode: " + ex.Message, LogLevel.Error);
            }

            return Task.FromResult(false);
        }

        public override bool UndoFeature()
        {
            try
            {
                Registry.SetValue(keyName, valueName, 1, RegistryValueKind.DWord);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log("Error in AppDarkMode (Undo): " + ex.Message, LogLevel.Error);
            }

            return false;
        }
    }

    internal class SystemDarkMode : FeatureBase
    {
        private const string keyName = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string valueName = "SystemUsesLightTheme";
        private const int recommendedValue = 0;

        public override string GetFeatureDetails()
        {
            return $"{keyName} | Value: {valueName} | Suggestion: {recommendedValue} (Dark mode – easy on the eyes, but totally your call)";
        }

        public override string ID()
        {
            return "Enable Dark Mode for System";
        }

        public override string HelpAnchorId()
        {
            return ID();
        }

        public override Task<bool> CheckFeature()
        {
            return Task.FromResult(
                   Utils.IntEquals(keyName, valueName, recommendedValue)
             );
        }

        public override Task<bool> DoFeature()
        {
            try
            {
                Registry.SetValue(keyName, valueName, 0, RegistryValueKind.DWord);
                Utils.RestartExplorer(); // Restart Explorer to apply changes
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Logger.Log("Error in SystemDarkMode: " + ex.Message, LogLevel.Error);
            }

            return Task.FromResult(false);
        }

        public override bool UndoFeature()
        {
            try
            {
                Registry.SetValue(keyName, valueName, 1, RegistryValueKind.DWord);
                Utils.RestartExplorer(); // Restart Explorer to apply changes
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log("Error in SystemDarkMode (Undo): " + ex.Message, LogLevel.Error);
            }

            return false;
        }
    }
}
