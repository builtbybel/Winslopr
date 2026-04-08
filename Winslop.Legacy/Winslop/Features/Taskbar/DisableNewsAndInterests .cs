using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using Winslop;

namespace Settings.UI
{
    internal class DisableNewsAndInterests : FeatureBase
    {
        private const string keyCu = @"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\Windows Feeds";
        private const string keyLm = @"HKEY_LOCAL_MACHINE\Software\Policies\Microsoft\Windows\Windows Feeds";
        private const string valueName = "EnableFeeds";

        private const int recommendedValue = 0; // 0 = disable Feeds (News and Interests)
        private const int undoValue = 1;        // 1 = allow Feeds (or delete policy)

        public override string ID()
        {
            return "Disable News and Interests";
        }

        public override string GetFeatureDetails()
        {
            return $"{keyCu} / {keyLm} | Value: {valueName} | Recommended Value: {recommendedValue}";
        }

        public override bool IsApplicable()
        {
            // This feature is intended for Windows 10 only
            return WindowsVersion.IsWindows10();
        }

        public override string InapplicableReason()
        {
            return "Windows 10 only";
        }

        public override Task<bool> CheckFeature()
        {
            bool cuOk = Utils.IntEquals(keyCu, valueName, recommendedValue);

            object lmObj = Registry.GetValue(keyLm, valueName, null);
            bool lmOk = lmObj == null || (lmObj is int i && i == recommendedValue);

            return Task.FromResult(cuOk && lmOk);
        }

        public override Task<bool> DoFeature()
        {
            try
            {
                Registry.SetValue(keyCu, valueName, recommendedValue, RegistryValueKind.DWord);

                try
                {
                    Registry.SetValue(keyLm, valueName, recommendedValue, RegistryValueKind.DWord);
                }
                catch (Exception ex)
                {
                    // Not fatal: HKLM may require admin rights.
                    Logger.Log("HKLM write skipped: " + ex.Message, LogLevel.Warning);
                }

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
                // Option 1: set to 1 (allow).
                Registry.SetValue(keyCu, valueName, undoValue, RegistryValueKind.DWord);

                try
                {
                    Registry.SetValue(keyLm, valueName, undoValue, RegistryValueKind.DWord);
                }
                catch (Exception ex)
                {
                    Logger.Log("HKLM write skipped: " + ex.Message, LogLevel.Warning);
                }

                // Alternative: delete the policy values instead of setting to 1.
                // DeletePolicyValue(keyCu, valueName);
                // DeletePolicyValue(keyLm, valueName);

                 Utils.RestartExplorer();

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