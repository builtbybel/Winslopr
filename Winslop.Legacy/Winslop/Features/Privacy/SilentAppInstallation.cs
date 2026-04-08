using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using Winslop;

namespace Settings.Privacy
{
    internal class SilentAppInstallation : FeatureBase
    {
        private const string keyName = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager";
        private const string valueName = "SilentInstalledAppsEnabled";

        private const int recommendedValue = 0; // 0 = prevent silent installs
        private const int undoValue = 1;        // 1 = allow silent installs (default)

        public override string ID()
        {
            return "Prevent Silent App Installation";
        }

        public override string GetFeatureDetails()
        {
            return $"{keyName} | Value: {valueName} | Recommended Value: {recommendedValue}";
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
                // Disable silent app installation in the background.
                Registry.SetValue(keyName, valueName, recommendedValue, RegistryValueKind.DWord);
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
                // Restore default behavior (allow silent installs).
                Registry.SetValue(keyName, valueName, undoValue, RegistryValueKind.DWord);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log("Code red in " + ex.Message, LogLevel.Error);
                return false;
            }
        }
    }
}