using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using Winslop;

namespace Settings.UI
{
    internal class DisableBingSearch : FeatureBase
    {
        private const string keyName = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Search";
        private const string valueName = "BingSearchEnabled";
        private const int recommendedValue = 0;

        public override string HelpAnchorId()
        {
            return DisableBingSearch.valueName; 
        }

        public override string GetFeatureDetails()
        {
            return $"{keyName} | Value: {valueName} | Recommended Value: {recommendedValue}";
        }

        public override string ID()
        {
            return "Disable Bing Search";
        }


        public override Task<bool> CheckFeature()
        {
            return Task.FromResult(Utils.IntEquals(keyName, valueName, recommendedValue));
        }

        public override Task<bool> DoFeature()
        {
            try
            {
                Registry.SetValue(keyName, valueName, recommendedValue, RegistryValueKind.DWord);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Logger.Log("Error in DisableBingSearch: " + ex.Message, LogLevel.Error);
                return Task.FromResult(false);
            }
        }

        public override bool UndoFeature()
        {
            try
            {
                Registry.SetValue(keyName, valueName, 1, RegistryValueKind.DWord);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log("Error undoing DisableBingSearch: " + ex.Message, LogLevel.Error);
                return false;
            }
        }
    }
}
