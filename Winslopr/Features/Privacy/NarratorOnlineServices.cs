using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using Winslop;

namespace Settings.Privacy
{
    internal class NarratorOnlineServices : FeatureBase
    {
        private const string keyName = @"HKEY_CURRENT_USER\Software\Microsoft\Narrator\NoRoam";
        private const string valueName = "OnlineServicesEnabled";

        private const int recommendedValue = 0; // 0 = disable Narrator online services

        public override string ID()
        {
            return "Disable Narrator Online Services";
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
                // Disable cloud-based Narrator services (image descriptions, enhanced voices, etc.).
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
                // Remove the value to return to Windows default behavior.
                using (var k = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Narrator\NoRoam", writable: true))
                {
                    k?.DeleteValue(valueName, throwOnMissingValue: false);
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