using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using Winslop;

namespace Settings.Privacy
{
    internal class WindowsSpotlightLockScreen : FeatureBase
    {
        private const string keyName = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager";
        private const string valueName = "RotatingLockScreenEnabled";

        private const int recommendedValue = 0; // 0 = disable rotating Spotlight images
        private const int undoValue = 1;        // 1 = enable rotating Spotlight images (default)

        public override string ID()
        {
            return "Disable Spotlight on Lock Screen";
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
                // Disable Windows Spotlight rotating images on the lock screen.
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
                // Restore default behavior (enable rotating Spotlight images).
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