using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using Winslop;

namespace Settings.Taskbar
{
    internal class TaskbarSmallIcons : FeatureBase
    {
        private const string keyName = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
        private const string valueName = "TaskbarSmallIcons";
        private const int recommendedValue = 1; // 1 = small taskbar icons

        public override string ID()
        {
            return "Make taskbar small";
        }

        public override string GetFeatureDetails()
        {
            return $"{keyName} | Value: {valueName} | Recommended Value: {recommendedValue}";
        }

        public override bool IsApplicable()
        {
            // This feature is intended for Windows 10 only.
            return WindowsVersion.IsWindows10();
        }

        public override string InapplicableReason()
        {
            return "Windows 10 only";
        }

        public override Task<bool> CheckFeature()
        {
            return Task.FromResult(
                Utils.IntEquals(keyName, valueName, recommendedValue)
            );
        }

        public override Task<bool> DoFeature()
        {
            try
            {
                Registry.SetValue(keyName, valueName, 1, RegistryValueKind.DWord);

                // Optional: restart Explorer to apply immediately.
                // Utils.RestartExplorer();

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
                // Restore default behavior (0 = normal size icons).
                Registry.SetValue(keyName, valueName, 0, RegistryValueKind.DWord);

                // Optional:
                // Utils.RestartExplorer();

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