using Microsoft.Win32;
using System;
using Winslop;
using System.Threading.Tasks;

namespace Settings.Edge
{
    public class DefaultTopSites : FeatureBase
    {
        private const string keyName = @"HKEY_LOCAL_MACHINE\Software\Policies\Microsoft\Edge";
        private const string valueName = "NewTabPageHideDefaultTopSites";
        private const int recommendedValue = 1;

        public override string GetFeatureDetails()
        {
            return $"{keyName} | Value: {valueName} | Recommended Value: {recommendedValue}";
        }

        public override string ID() => "Don't Show Sponsored links in new tab page";

        public override string HelpAnchorId()
        {
            return ID();
        }

        public override Task<bool> CheckFeature()
        {
            return Task.FromResult(Utils.IntEquals(keyName, valueName, recommendedValue));
        }

        public override Task<bool> DoFeature()
        {
            try
            {
                Registry.SetValue(keyName, valueName, recommendedValue, Microsoft.Win32.RegistryValueKind.DWord);

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
                Registry.SetValue(keyName, valueName, 0, Microsoft.Win32.RegistryValueKind.DWord);

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