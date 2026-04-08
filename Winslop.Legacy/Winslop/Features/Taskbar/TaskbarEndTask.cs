using Microsoft.Win32;
using System;
using Winslop;
using System.Threading.Tasks;

namespace Settings.System
{
    internal class TaskbarEndTask : FeatureBase
    {
        private const string keyName = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\TaskbarDeveloperSettings";
        private const string valueName = "TaskbarEndTask";
        private const int recommendedValue = 1;

        public override bool IsApplicable()
        {
            // This feature is intended for Windows 11 only.
            return WindowsVersion.IsWindows11OrLater();
        }

        public override string InapplicableReason()
        {
            return "Windows 11 only";
        }

        public override string GetFeatureDetails()
        {
            return $"{keyName} | Value: {valueName} | Recommended Value: {recommendedValue}";
        }

        public override string ID()
        {
            return "Enable End Task";
        }

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
                Registry.SetValue(keyName, valueName, recommendedValue, RegistryValueKind.DWord);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to enable End Task: " + ex.Message, LogLevel.Error);
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
                Logger.Log("Failed to disable End Task: " + ex.Message, LogLevel.Error);
            }

            return false;
        }
    }
}
