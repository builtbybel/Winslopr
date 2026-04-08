using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using Winslop;

namespace Settings.UI
{
    internal class DisableWidgets : FeatureBase
    {
        private const string keyCu = @"HKEY_CURRENT_USER\Software\Policies\Microsoft\Dsh";
        private const string keyLm = @"HKEY_LOCAL_MACHINE\Software\Policies\Microsoft\Dsh";
        private const string valueName = "AllowNewsAndInterests";

        private const int recommendedValue = 0; // 0 = disable Widgets
        private const int undoValue = 1;        // 1 = allow Widgets (or delete policy)

        public override string ID() => "Disable Widgets";

        public override string GetFeatureDetails()
            => $"{keyCu} / {keyLm} | Value: {valueName} | Recommended Value: {recommendedValue}";

        public override bool IsApplicable()
        {
            // This feature is intended for Windows 11 only.
            return WindowsVersion.IsWindows11OrLater();
        }

        public override string InapplicableReason() => "Windows 11 only";

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
                // Option A: set to 1 (allow)
                Registry.SetValue(keyCu, valueName, undoValue, RegistryValueKind.DWord);

                try
                {
                    Registry.SetValue(keyLm, valueName, undoValue, RegistryValueKind.DWord);
                }
                catch (Exception ex)
                {
                    Logger.Log("HKLM write skipped: " + ex.Message, LogLevel.Warning);
                }

                // Alternative would be to delete the policy values instead of setting to 1
                // DeletePolicyValue(keyCu, valueName);
                // DeletePolicyValue(keyLm, valueName);

                // Optional:
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