using Microsoft.Win32;
using System;
using Winslop;
using System.Threading.Tasks;

namespace Settings.AI
{
    internal class Recall : FeatureBase
    {
        private const string keyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\WindowsAI";
        private const string valueName = "AllowRecallEnablement";
        private const int recommendedValue = 0;

        public override string GetFeatureDetails()
        {
            return $"{keyName} | Value: {valueName} | Recommended: {recommendedValue} (Recall off – protect your privacy, prevent AI from accessing personal data)";
        }

        public override bool IsApplicable()
        {
            // This feature is intended for Windows 11 only.
            return WindowsVersion.IsWindows11OrLater();
        }

        public override string InapplicableReason()
        {
            return "Windows 11 only";
        }

        public override string ID()
        {
            return "Turn off Recall in Windows 11";
        }

        public override string HelpAnchorId()
        {
            return ID();
        }

        public override Task<bool> CheckFeature()
        {
            // Check if reg key exists
            object value = Registry.GetValue(keyName, valueName, null);
            if (value == null)
            {
                // Key does not exist, turn off feature
                return Task.FromResult(false);
            }

            // Key exists, check if value is desired value
            return Task.FromResult((int)value == recommendedValue);
        }

        public override Task<bool> DoFeature()
        {
            try
            {
                Registry.SetValue(keyName, valueName, recommendedValue, Microsoft.Win32.RegistryValueKind.DWord);
                Logger.Log("You've even disabled system-wide Snapshots for all users now.", LogLevel.Info);
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