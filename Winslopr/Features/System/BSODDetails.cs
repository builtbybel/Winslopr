using Microsoft.Win32;
using System;
using Winslop;
using System.Threading.Tasks;

namespace Settings.System
{
    internal class BSODDetails : FeatureBase
    {
        private const string keyName = @"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\CrashControl";
        private const string valueName = "DisplayParameters";
        private const string valueName2 = "DisableEmoticon";
        private const int recommendedValue = 1;

        public override string GetFeatureDetails()
        {
            return $"{keyName} | Values: {valueName}, {valueName2} | Recommended Value: {recommendedValue}";
        }

        public override string ID()
        {
            return "Show BSOD details instead of sad smiley";
        }

        public override string HelpAnchorId()
        {
            return ID();
        }

        public override Task<bool> CheckFeature()
        {
            return Task.FromResult(
                Utils.IntEquals(keyName, valueName, recommendedValue) &&
                Utils.IntEquals(keyName, valueName2, recommendedValue)
            );
        }

        public override Task<bool> DoFeature()
        {
            try
            {
                Registry.SetValue(keyName, valueName, recommendedValue, RegistryValueKind.DWord);
                Registry.SetValue(keyName, valueName2, recommendedValue, RegistryValueKind.DWord);
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
                Registry.SetValue(keyName, valueName, 0, RegistryValueKind.DWord);
                Registry.SetValue(keyName, valueName2, 0, RegistryValueKind.DWord);
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
