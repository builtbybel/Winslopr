using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Winslop;

namespace Settings.Issues
{
    internal class BasicCleanup : FeatureBase
    {
        private readonly string tempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp");
        private const int cleanupSetNumber = 1;

        public override string ID() => "Basic Disk Cleanup";

        public override string HelpAnchorId()
        {
            return ID();
        }

        public override Task<bool> CheckFeature()
        {
            try
            {
                var totalSize = GetDirectorySize(tempPath);

                bool isOk = totalSize <= 50;

                return Task.FromResult(isOk);
            }
            catch (Exception ex)
            {
                Logger.Log($"Error checking Temp folder: {ex.Message}", LogLevel.Error);
                return Task.FromResult(false);
            }
        }

        public override string GetFeatureDetails()
        {
            try
            {
                var totalSize = GetDirectorySize(tempPath);
                return $"Temp folder size: {totalSize} MB (including cleanmgr /sagerun:1 in the next run)";
            }
            catch (Exception ex)
            {
                Logger.Log($"Error accessing Temp folder: {ex.Message}", LogLevel.Error);
                return $"Temp folder not accessible: {tempPath}";
            }
        }

        public override async Task<bool> DoFeature()
        {
            try
            {
                await CleanTempFolderAsync();
                await RunDiskCleanup();
                Logger.Log("Basic Cleanup completed successfully.", LogLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"Error cleaning Temp folder: {ex.Message}", LogLevel.Warning);
                return false;
            }
        }

        private async Task CleanTempFolderAsync()
        {
            var (deletedFiles, failedFiles, deletedDirs, failedDirs) = await Task.Run(() =>
            {
                int df = 0, ff = 0, dd = 0, fd = 0;

                foreach (var file in Directory.EnumerateFiles(tempPath, "*", SearchOption.AllDirectories))
                {
                    try { File.Delete(file); df++; }
                    catch { ff++; }
                }

                foreach (var dir in Directory.EnumerateDirectories(tempPath))
                {
                    try { Directory.Delete(dir, true); dd++; }
                    catch { fd++; }
                }

                return (df, ff, dd, fd);
            });

            Logger.Log($"Deleted {deletedFiles} files, {deletedDirs} directories from Temp folder.", LogLevel.Info);
            if (failedFiles > 0 || failedDirs > 0)
                Logger.Log($"Skipped {failedFiles} files, {failedDirs} directories (in use or protected).", LogLevel.Warning);
        }

        // calculate the size of the directory in MB
        private long GetDirectorySize(string directory)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(directory);
                long size = 0;

                // Calculate size of all files
                size += directoryInfo.GetFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);

                return size / (1024 * 1024);                 // return size in MB
            }
            catch (Exception ex)
            {
                Logger.Log($"Error calculating directory size: {ex.Message}", LogLevel.Warning);
                return 0;
            }
        }

        private async Task RunDiskCleanup()
        {
            try
            {
                Logger.Log("Running Disk Cleanup utility (cleanmgr)...", LogLevel.Info);

                var sageSetCmd = $"cleanmgr.exe /sageset:{cleanupSetNumber}";
                var startInfo1 = new ProcessStartInfo("cmd", $"/c {sageSetCmd}")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                var sageRunCmd = $"cleanmgr.exe /sagerun:{cleanupSetNumber}";
                var veryLowDiskCmd = $"cleanmgr.exe /verylowdisk";
                var startInfo2 = new ProcessStartInfo("cmd", $"/c {sageRunCmd} {veryLowDiskCmd}")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                using (var process1 = Process.Start(startInfo1))
                {
                    if (process1 != null)
                        await Task.Run(() => process1.WaitForExit());
                }

                using (var process2 = Process.Start(startInfo2))
                {
                    if (process2 != null)
                        await Task.Run(() => process2.WaitForExit());
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error running Disk Cleanup: {ex.Message}", LogLevel.Warning);
            }
        }

        // Undo method: Cleanup cannot be undone, so return false
        public override bool UndoFeature() => false;
    }
}