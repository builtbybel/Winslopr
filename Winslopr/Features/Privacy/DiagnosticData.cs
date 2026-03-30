using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using Winslop;

namespace Settings.Privacy
{
    internal class DiagnosticData : FeatureBase
    {
        private const int recommendedValue = 1; // 1 = basic/required diagnostic data
        private const int enabledValue = 3;     // 3 = full/optional diagnostic data (depending on Windows edition)

        // Registry paths from your SettingDefinition
        private const string keyDiagTrackToast = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Diagnostics\DiagTrack";
        private const string valShowedToastAtLevel = "ShowedToastAtLevel";

        private const string keyPolicyDataCollectionHKCU = @"HKEY_CURRENT_USER\SOFTWARE\Policies\Microsoft\Windows\DataCollection";
        private const string keyPolicyDataCollectionHKLM = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\DataCollection";

        private const string keyDataCollectionPolicyHKCU = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection";
        private const string keyDataCollectionPolicyHKLM = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection";

        private const string valAllowTelemetry = "AllowTelemetry";
        private const string valMaxTelemetryAllowed = "MaxTelemetryAllowed";

        public override string ID()
        {
            return "Reduce Diagnostic Data (Basic)";
        }

        public override string GetFeatureDetails()
        {
            return "Sets diagnostic data level to Basic/Required (1) via registry and policies (where applicable).";
        }

        public override Task<bool> CheckFeature()
        {
            // All configured values should match the recommended value.
            // For the two Policy keys where your definition had DisabledValue = null,
            // we still treat "missing" as NOT matching recommended (strict mode),
            // because recommended wants an explicit 1.
            bool ok =
                Utils.IntEquals(keyDiagTrackToast, valShowedToastAtLevel, recommendedValue) &&
                Utils.IntEquals(keyDataCollectionPolicyHKCU, valAllowTelemetry, recommendedValue) &&
                Utils.IntEquals(keyDataCollectionPolicyHKLM, valAllowTelemetry, recommendedValue) &&
                Utils.IntEquals(keyDataCollectionPolicyHKCU, valMaxTelemetryAllowed, recommendedValue) &&
                Utils.IntEquals(keyDataCollectionPolicyHKLM, valMaxTelemetryAllowed, recommendedValue) &&
                Utils.IntEquals(keyPolicyDataCollectionHKCU, valAllowTelemetry, recommendedValue) &&
                Utils.IntEquals(keyPolicyDataCollectionHKLM, valAllowTelemetry, recommendedValue);

            return Task.FromResult(ok);
        }

        public override Task<bool> DoFeature()
        {
            try
            {
                // Set recommended "basic/required" level.
                SetDword(keyDiagTrackToast, valShowedToastAtLevel, recommendedValue);

                SetDword(keyPolicyDataCollectionHKCU, valAllowTelemetry, recommendedValue);
                TrySetDwordHKLM(keyPolicyDataCollectionHKLM, valAllowTelemetry, recommendedValue);

                SetDword(keyDataCollectionPolicyHKCU, valAllowTelemetry, recommendedValue);
                TrySetDwordHKLM(keyDataCollectionPolicyHKLM, valAllowTelemetry, recommendedValue);

                SetDword(keyDataCollectionPolicyHKCU, valMaxTelemetryAllowed, recommendedValue);
                TrySetDwordHKLM(keyDataCollectionPolicyHKLM, valMaxTelemetryAllowed, recommendedValue);

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
                // Undo: return toward "enabled/default" behavior.
                // Where possible, set to 3 (more permissive), and remove policies that might have been enforced.
                SetDword(keyDiagTrackToast, valShowedToastAtLevel, enabledValue);

                // Prefer deleting policy values on undo to avoid forcing telemetry level.
                DeleteValueHKCU(@"SOFTWARE\Policies\Microsoft\Windows\DataCollection", valAllowTelemetry);
                DeleteValueHKLM(@"SOFTWARE\Policies\Microsoft\Windows\DataCollection", valAllowTelemetry);

                // Restore non-policy keys to default-ish "enabled" state.
                SetDword(keyDataCollectionPolicyHKCU, valAllowTelemetry, enabledValue);
                TrySetDwordHKLM(keyDataCollectionPolicyHKLM, valAllowTelemetry, enabledValue);

                SetDword(keyDataCollectionPolicyHKCU, valMaxTelemetryAllowed, enabledValue);
                TrySetDwordHKLM(keyDataCollectionPolicyHKLM, valMaxTelemetryAllowed, enabledValue);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log("Code red in " + ex.Message, LogLevel.Error);
                return false;
            }
        }

        private static void SetDword(string fullKeyPath, string valueName, int value)
        {
            Registry.SetValue(fullKeyPath, valueName, value, RegistryValueKind.DWord);
        }

        private static void TrySetDwordHKLM(string fullKeyPath, string valueName, int value)
        {
            try
            {
                Registry.SetValue(fullKeyPath, valueName, value, RegistryValueKind.DWord);
            }
            catch (Exception ex)
            {
                // Not fatal: HKLM may require admin rights.
                Logger.Log("HKLM write skipped: " + ex.Message, LogLevel.Warning);
            }
        }

        private static void DeleteValueHKCU(string subKey, string valueName)
        {
            using (var k = Registry.CurrentUser.OpenSubKey(subKey, writable: true))
            {
                k?.DeleteValue(valueName, throwOnMissingValue: false);
            }
        }

        private static void DeleteValueHKLM(string subKey, string valueName)
        {
            try
            {
                using (var k = Registry.LocalMachine.OpenSubKey(subKey, writable: true))
                {
                    k?.DeleteValue(valueName, throwOnMissingValue: false);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("HKLM delete skipped: " + ex.Message, LogLevel.Warning);
            }
        }
    }
}