using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using Winslop;

namespace Settings.Ads
{
    internal class TailoredExperiences : FeatureBase
    {
        private const string keyPrivacy = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Privacy";
        private const string valPrivacy = "TailoredExperiencesWithDiagnosticDataEnabled";
        private const int recommendedPrivacyValue = 0; // 0 = off

        private const string keyPolicyCu = @"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\CloudContent";
        private const string keyPolicyLm = @"HKEY_LOCAL_MACHINE\Software\Policies\Microsoft\Windows\CloudContent";
        private const string valPolicy = "DisableTailoredExperiencesWithDiagnosticData";
        private const int recommendedPolicyValue = 1; // 1 = disable via policy

        public override string ID()
        {
            return "Disable Tailored Experiences";
        }

        public override string GetFeatureDetails()
        {
            return $"{keyPrivacy} ({valPrivacy}={recommendedPrivacyValue}), {keyPolicyCu}/{keyPolicyLm} ({valPolicy}={recommendedPolicyValue})";
        }

        public override Task<bool> CheckFeature()
        {
            bool privacyOk = Utils.IntEquals(keyPrivacy, valPrivacy, recommendedPrivacyValue);

            bool policyCuOk = Utils.IntEquals(keyPolicyCu, valPolicy, recommendedPolicyValue);

            object lmObj = Registry.GetValue(keyPolicyLm, valPolicy, null);
            bool policyLmOk = lmObj == null || (lmObj is int i && i == recommendedPolicyValue);

            return Task.FromResult(privacyOk && policyCuOk && policyLmOk);
        }

        public override Task<bool> DoFeature()
        {
            try
            {
                // Disable tailored experiences at the user setting level
                Registry.SetValue(keyPrivacy, valPrivacy, recommendedPrivacyValue, RegistryValueKind.DWord);

                // Enforce disable via policy (per-user)
                Registry.SetValue(keyPolicyCu, valPolicy, recommendedPolicyValue, RegistryValueKind.DWord);

                // Enforce disable via policy (machine)
                try
                {
                    Registry.SetValue(keyPolicyLm, valPolicy, recommendedPolicyValue, RegistryValueKind.DWord);
                }
                catch (Exception ex)
                {
                    // Not fatal: HKLM require admin rights
                    Logger.Log("HKLM write skipped: " + ex.Message, LogLevel.Warning);
                }

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
                // Restore default user setting (enabled)
                Registry.SetValue(keyPrivacy, valPrivacy, 1, RegistryValueKind.DWord);

                // Remove policies to return to Windows default behavior.
                DeleteValueHKCU(@"Software\Policies\Microsoft\Windows\CloudContent", valPolicy);
                DeleteValueHKLM(@"Software\Policies\Microsoft\Windows\CloudContent", valPolicy);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log("Code red in " + ex.Message, LogLevel.Error);
                return false;
            }
        }

        private static void DeleteValueHKCU(string subKey, string valueName)
        {
            using (var k = Registry.CurrentUser.OpenSubKey(subKey, writable: true))
            {
                k?.DeleteValue(valueName, throwOnMissingValue: false);
            }
        }

        private static void DeleteValueHKLM(string subKey, string valueName)
        {
            try
            {
                using (var k = Registry.LocalMachine.OpenSubKey(subKey, writable: true))
                {
                    k?.DeleteValue(valueName, throwOnMissingValue: false);
                }
            }
            catch
            {
                // just ignore if not elevated
            }
        }
    }
}