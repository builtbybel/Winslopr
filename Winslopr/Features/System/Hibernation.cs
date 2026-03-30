using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Winslop;

namespace Settings.System
{
    internal class DisableHibernation : FeatureBase
    {
        private const string powerKey = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Power";
        private const string hibernateValueName = "HibernateEnabled";
        private const int disabledValue = 0;
        private const int enabledValue = 1;

        // Key to hide/show the Hibernate option in the power menu Alt+F4 dialog
        private const string flyoutMenuKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FlyoutMenuSettings";
        private const string showHibernateOptionName = "ShowHibernateOption";

        public override string GetFeatureDetails()
        {
            return "Disables hibernation by setting registry values and running powercfg, and hides/shows the Hibernate option in the power menu.";
        }

        public override string ID()
        {
            return "Disable Hibernation";
        }

        public override string HelpAnchorId()
        {
            return ID();
        }

        public override Task<bool> CheckFeature()
        {
            bool hibernateDisabled = Utils.IntEquals(powerKey, hibernateValueName, disabledValue);
            bool optionHidden = Utils.IntEquals(flyoutMenuKey, showHibernateOptionName, 0);
            return Task.FromResult(hibernateDisabled && optionHidden);
        }

        public override Task<bool> DoFeature()
        {
            try
            {
                Registry.SetValue(powerKey, hibernateValueName, disabledValue, RegistryValueKind.DWord);
                Registry.SetValue(flyoutMenuKey, showHibernateOptionName, 0, RegistryValueKind.DWord);
                RunCommand("powercfg", "/hibernate off");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Logger.Log("Error disabling hibernation: " + ex.Message, LogLevel.Error);
                return Task.FromResult(false);
            }
        }

        public override bool UndoFeature()
        {
            try
            {
                Registry.SetValue(powerKey, hibernateValueName, enabledValue, RegistryValueKind.DWord);
                Registry.SetValue(flyoutMenuKey, showHibernateOptionName, 1, RegistryValueKind.DWord);
                RunCommand("powercfg", "/hibernate on");
                bool hibernateEnabled = Utils.IntEquals(powerKey, hibernateValueName, enabledValue);
                bool optionShown = Utils.IntEquals(flyoutMenuKey, showHibernateOptionName, 1);
                return hibernateEnabled && optionShown;
            }
            catch (Exception ex)
            {
                Logger.Log("Error enabling hibernation: " + ex.Message, LogLevel.Error);
                return false;
            }
        }

        private void RunCommand(string fileName, string arguments)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit();
            }
        }
    }
}
