using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using Winslop;

namespace Settings.Privacy
{
    internal class OnlineSpeechRecognition : FeatureBase
    {
        private const string keySpeech = @"HKEY_CURRENT_USER\Software\Microsoft\Speech_OneCore\Settings\OnlineSpeechPrivacy";
        private const string valueHasAccepted = "HasAccepted";

        private const string keyPolicyCu = @"HKEY_CURRENT_USER\SOFTWARE\Policies\Microsoft\InputPersonalization";
        private const string keyPolicyLm = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\InputPersonalization";
        private const string valueAllowInputPersonalization = "AllowInputPersonalization";

        private const int recommendedValue = 0; // 0 = disable
        private const int defaultEnabledValue = 1;

        public override string ID()
        {
            return "Disable Online Speech Recognition";
        }

        public override string GetFeatureDetails()
        {
            return $"{keySpeech} ({valueHasAccepted}), {keyPolicyCu}/{keyPolicyLm} ({valueAllowInputPersonalization}) | Recommended Value: {recommendedValue}";
        }

        public override Task<bool> CheckFeature()
        {
            bool speechOk = Utils.IntEquals(keySpeech, valueHasAccepted, recommendedValue);

            bool policyCuOk = Utils.IntEquals(keyPolicyCu, valueAllowInputPersonalization, recommendedValue);

            object lmObj = Registry.GetValue(keyPolicyLm, valueAllowInputPersonalization, null);
            bool policyLmOk = lmObj == null || (lmObj is int i && i == recommendedValue);

            return Task.FromResult(speechOk && policyCuOk && policyLmOk);
        }

        public override Task<bool> DoFeature()
        {
            try
            {
                // Disable online speech recognition
                Registry.SetValue(keySpeech, valueHasAccepted, recommendedValue, RegistryValueKind.DWord);

                // Disable input personalization via policy (per-user)
                Registry.SetValue(keyPolicyCu, valueAllowInputPersonalization, recommendedValue, RegistryValueKind.DWord);

                // Disable input personalization via policy (machine)
                try
                {
                    Registry.SetValue(keyPolicyLm, valueAllowInputPersonalization, recommendedValue, RegistryValueKind.DWord);
                }
                catch (Exception ex)
                {
                    // Again not fatal: HKLM may require admin rights
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
                // Restore default behavior for online speech recognition
                Registry.SetValue(keySpeech, valueHasAccepted, defaultEnabledValue, RegistryValueKind.DWord);

                // Remove policy values to return to Windows default behavior
                DeletePolicyValueHKCU(@"SOFTWARE\Policies\Microsoft\InputPersonalization", valueAllowInputPersonalization);
                DeletePolicyValueHKLM(@"SOFTWARE\Policies\Microsoft\InputPersonalization", valueAllowInputPersonalization);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log("Code red in " + ex.Message, LogLevel.Error);
                return false;
            }
        }

        private static void DeletePolicyValueHKCU(string subKey, string valueName)
        {
            using (var k = Registry.CurrentUser.OpenSubKey(subKey, writable: true))
            {
                k?.DeleteValue(valueName, throwOnMissingValue: false);
            }
        }

        private static void DeletePolicyValueHKLM(string subKey, string valueName)
        {
            using (var k = Registry.LocalMachine.OpenSubKey(subKey, writable: true))
            {
                k?.DeleteValue(valueName, throwOnMissingValue: false);
            }
        }
    }
}