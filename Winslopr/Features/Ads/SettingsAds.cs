using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using Winslop;

namespace Settings.Privacy
{
    internal class SettingsAds : FeatureBase
    {
        private const string keyName = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager";

        private static readonly string[] valueNames = new[]
        {
            "SubscribedContent-338393Enabled",
            "SubscribedContent-353694Enabled",
            "SubscribedContent-353696Enabled",
        };

        private const int recommendedValue = 0; // 0 = disable suggested/promotional content in Settings

        public override string ID()
        {
            return "Disable Settings Ads";
        }

        public override string GetFeatureDetails()
        {
            return $"{keyName} | Values: {string.Join(", ", valueNames)} | Recommended Value: {recommendedValue}";
        }

        public override Task<bool> CheckFeature()
        {
            // All values must match the recommended value.
            foreach (string vn in valueNames)
            {
                if (!Utils.IntEquals(keyName, vn, recommendedValue))
                    return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public override Task<bool> DoFeature()
        {
            try
            {
                // Disable suggested content by setting all related values to 0.
                foreach (string vn in valueNames)
                    Registry.SetValue(keyName, vn, recommendedValue, RegistryValueKind.DWord);

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
                // Remove values to return to Windows default behavior.
                using (var k = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", writable: true))
                {
                    if (k != null)
                    {
                        foreach (string vn in valueNames)
                            k.DeleteValue(vn, throwOnMissingValue: false);
                    }
                }

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