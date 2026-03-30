using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using Winslop;

namespace Settings.Taskbar
{
    internal class RemoveMeetNowButton : FeatureBase
    {
        private const string keyName = @"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\Explorer";
        private const string valueName = "HideSCAMeetNow";
        private const int recommendedValue = 1; // 1 = hide Meet Now

        public override bool IsApplicable()
        {
            // This feature is intended for Windows 10 only
            return WindowsVersion.IsWindows10();
        }

        public override string InapplicableReason()
        {
            return "Windows 10 only";
        }


        public override string GetFeatureDetails()
        {
            return $"{keyName} | Value: {valueName} | Recommended Value: {recommendedValue}";
        }

        public override string ID()
        {
            return "Remove 'Meet Now' button from system tray";
        }

        public override string HelpAnchorId()
        {
            return ID();
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
                Registry.SetValue(keyName, valueName, recommendedValue, RegistryValueKind.DWord);

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
       
                Registry.SetValue(keyName, valueName, 0, RegistryValueKind.DWord);

                // Alternative Remove policy value:
                // using (var k = Registry.CurrentUser.OpenSubKey(@"Software\Policies\Microsoft\Windows\Explorer", true))
                //     k?.DeleteValue(valueName, false);

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