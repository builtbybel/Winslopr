using Microsoft.Win32;
using System;
using Winslop;
using System.Threading.Tasks;

namespace Settings.AI
{
    internal class CopilotTaskbar : FeatureBase
    {
        private const string keyName = @"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\WindowsCopilot";
        private const string valueName = "TurnOffWindowsCopilot";
        private const int recommendedValue = 1;

        public override string GetFeatureDetails()
        {
            return $"{keyName} | Value: {valueName} | Recommended Value: {recommendedValue}";
        }

        public override string ID()
        {
            return "Don't Show Copilot in Taskbar";
        }

        public override string HelpAnchorId()
        {
            return ID();
        }


        public override Task<bool> CheckFeature()
        {
            return Task.FromResult(Utils.IntEquals(keyName, valueName, 1));
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
                Logger.Log("Code red in " + ex.Message, LogLevel.Error);
            }

            return Task.FromResult(false);
        }

        public override bool UndoFeature()
        {
            try
            {
                Registry.SetValue(keyName, valueName, 0, RegistryValueKind.DWord);
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