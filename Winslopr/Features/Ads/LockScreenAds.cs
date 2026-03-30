using Microsoft.Win32;
using System;
using Winslop;
using System.Threading.Tasks;

namespace Settings.Ads
{
    internal class LockScreenAds : FeatureBase
    {
        private const string keyName = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager";
        private const string valueName = "RotatingLockScreenOverlayEnabled";
        private const string valueName2 = "SubscribedContent-338387Enabled";
        private const int recommendedValue = 0;

        public override string ID() => "Disable Lock Screen Tips and Ads";

        public override string HelpAnchorId()
        {
            return ID();
        }

        public override string GetFeatureDetails()
        {
            return $"{keyName} | Value: {valueName} | Recommended Value: {recommendedValue}";
        }

        public override Task<bool> CheckFeature()
        {
            return Task.FromResult(Utils.IntEquals(keyName, valueName, recommendedValue) &&
                   Utils.IntEquals(keyName, valueName2, recommendedValue)
            );
        }

        public override Task<bool> DoFeature()
        {
            try
            {
                Registry.SetValue(keyName, valueName, 0, Microsoft.Win32.RegistryValueKind.DWord);
                Registry.SetValue(keyName, valueName2, 0, Microsoft.Win32.RegistryValueKind.DWord);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Logger.Log("Code red in " + ex.Message, LogLevel.Error);
            }
                
            return Task.FromResult(false);
        }

        public override bool UndoFeature()
        {
            try
            {
                Registry.SetValue(keyName, valueName, 1, Microsoft.Win32.RegistryValueKind.DWord);
                Registry.SetValue(keyName, valueName2, 1, Microsoft.Win32.RegistryValueKind.DWord);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log("Code red in " + ex.Message, LogLevel.Error);
            }

            return false;
        }
    }
}