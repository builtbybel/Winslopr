using Microsoft.Win32;
using System;
using Winslop;
using System.Threading.Tasks;

namespace Settings.Personalization
{
    internal class DisableSnapAssistFlyout : FeatureBase
    {
        private const string keyName = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
        private const string valueName = "EnableSnapAssistFlyout";
        private const int recommendedValue = 0; 

        public override string GetFeatureDetails()
        {
            return $"{keyName} | Value: {valueName} | Recommended Value: {recommendedValue}";
        }

        public override string ID()
        {
            return "Disable Snap Assist Flyout";
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
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Logger.Log("Error in DisableSnapAssistFlyout: " + ex.Message, LogLevel.Error);
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
                Logger.Log("Error in DisableSnapAssistFlyout (Undo): " + ex.Message, LogLevel.Error);
            }

            return false;
        }
    }
}
