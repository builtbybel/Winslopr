using Microsoft.Win32;
using System;
using Winslop;
using System.Threading.Tasks;

namespace Settings.System
{
    internal class NetworkThrottling : FeatureBase
    {
        private const string keyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile";
        private const string valueName = "NetworkThrottlingIndex";
        private const int recommendedValue = -1; // 0xFFFFFFFF

        public override string GetFeatureDetails()
        {
            return $"{keyName} | Value: {valueName} | Recommended Value: 0xFFFFFFFF (decimal: {uint.MaxValue})";
        }

        public override string ID()
        {
            return "Disable Network Throttling";
        }

        public override string HelpAnchorId()
        {
            return ID();
        }

        public override Task<bool> CheckFeature()
        {
            return Task.FromResult(Utils.IntEquals(keyName, valueName, unchecked((int)0xFFFFFFFF)));
        }

        public override Task<bool> DoFeature()
        {
            try
            {
                Registry.SetValue(keyName, valueName, unchecked((int)0xFFFFFFFF), RegistryValueKind.DWord);
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
                Registry.SetValue(keyName, valueName, 10, RegistryValueKind.DWord); // Default is 10
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
