using System.Diagnostics;

using FFMpegCore;
using FFMpegCore.Extensions.Downloader;
using FFMpegCore.Extensions.Downloader.Enums;

namespace XNBVerter.Core
{
    /// <summary>
    /// Provides functionality for ensuring ffprobe is available,
    /// downloading it automatically if not found on the system.
    /// </summary>
    internal static class FfprobeBinaryProvider
    {
        private static bool _hasAttemptedDownload;

        /// <summary>
        /// Ensures ffprobe is available by checking for local installation
        /// and downloading it if necessary.
        /// </summary>
        /// <returns>
        /// The path to the ffprobe executable if found or successfully downloaded;
        /// otherwise, <c>null</c>.
        /// </returns>
        /// <remarks>
        /// This method first checks for a local ffprobe executable in the application's directory.
        /// If not found locally, it checks the system PATH.
        /// If ffprobe is not found anywhere and a download hasn't been attempted yet,
        /// it will use FFMpegCore to download the latest version.
        /// </remarks>
        public static string? EnsureFfprobeAvailable()
        {
            // Check for local ffprobe first
            string? localFfprobe = FindLocalFfprobe();
            if (localFfprobe is not null)
            {
                return localFfprobe;
            }

            // Check if ffprobe is available in PATH
            if (IsFfprobeInPath())
            {
                return "ffprobe";
            }

            // If not found and we haven't attempted download yet, try downloading
            if (!_hasAttemptedDownload)
            {
                _hasAttemptedDownload = true;

                // Check if auto-download is disabled via environment variable
                string? noAutoDownload = Environment.GetEnvironmentVariable("NO_AUTO_DOWNLOAD_FFPROBE");
                if (noAutoDownload == "1" || string.Equals(noAutoDownload, "true", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("ffprobe is not found on your system.");
                    Console.WriteLine("Auto-download is disabled (NO_AUTO_DOWNLOAD_FFPROBE is set).");
                    Console.WriteLine("Please install ffprobe manually or add it to your system PATH.");
                    return null;
                }

                // Check if console is interactive
                bool isInteractive = !Console.IsInputRedirected && !Console.IsOutputRedirected;
                bool shouldDownload;
                if (isInteractive)
                {
                    // Prompt user for confirmation in interactive mode
                    Console.WriteLine("ffprobe is not found on your system. Would you like to download it now?");
                    Console.Write("(Y) yes  (N) no: ");
                    string? response = Console.ReadLine()?.Trim().ToUpperInvariant();

                    if (response is "Y" or "YES")
                    {
                        shouldDownload = true;
                    }
                    else
                    {
                        Console.WriteLine("Download cancelled. Please install ffprobe manually or add it to your system PATH.");
                        return null;
                    }
                }
                else
                {
                    // Auto-download in non-interactive mode
                    Console.WriteLine("ffprobe is not found on your system.");
                    Console.WriteLine("Running in non-interactive mode. Automatically downloading ffprobe...");
                    Console.WriteLine("(Set NO_AUTO_DOWNLOAD_FFPROBE=1 to disable auto-download)");
                    shouldDownload = true;
                }

                if (!shouldDownload)
                {
                    return null;
                }

                try
                {
                    Console.WriteLine("Downloading ffprobe...");

                    // Determine the application directory for downloading ffprobe
                    string? processPath = Environment.ProcessPath;
                    string? appDir = processPath is not null
                        ? Path.GetDirectoryName(processPath)
                        : Directory.GetCurrentDirectory();

                    if (string.IsNullOrEmpty(appDir))
                    {
                        appDir = Directory.GetCurrentDirectory();
                    }

                    // Configure FFMpegCore to download to the application directory
                    FFOptions ffOptions = new() { BinaryFolder = appDir };

                    // Download ffprobe using FFMpegCore's FFMpegDownloader
                    // We only need FFProbe, not the full suite
                    List<string> downloadedPaths = FFMpegDownloader.DownloadBinaries(
                        FFMpegVersions.LatestAvailable,
                        FFMpegBinaries.FFProbe,
                        ffOptions).GetAwaiter().GetResult();

                    Console.WriteLine($"ffprobe downloaded successfully to: {string.Join(", ", downloadedPaths)}");

                    // After download, check again for local installation
                    localFfprobe = FindLocalFfprobe();
                    if (localFfprobe is not null)
                    {
                        return localFfprobe;
                    }

                    // FFMpegCore might have downloaded to its own location
                    // Try to get the binary path from GlobalFFOptions
                    if (!string.IsNullOrEmpty(GlobalFFOptions.Current.BinaryFolder))
                    {
                        string ffprobeInBinFolder = Path.Combine(
                            GlobalFFOptions.Current.BinaryFolder,
                            OperatingSystem.IsWindows() ? "ffprobe.exe" : "ffprobe");

                        if (File.Exists(ffprobeInBinFolder))
                        {
                            return ffprobeInBinFolder;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to download ffprobe: {ex.Message}");
                    Console.WriteLine("Please install ffprobe manually or add it to your system PATH.");
                }
            }

            return null;
        }

        /// <summary>
        /// Searches for a local ffprobe executable in the application's directory.
        /// </summary>
        /// <returns>
        /// The full path to the local ffprobe executable if found; otherwise, <c>null</c>.
        /// </returns>
        private static string? FindLocalFfprobe()
        {
            string? processPath = Environment.ProcessPath;
            string? currentDir = processPath is not null
                ? Path.GetDirectoryName(processPath)
                : null;

            if (string.IsNullOrEmpty(currentDir))
            {
                return null;
            }

            string exePath = Path.Combine(currentDir, "ffprobe.exe");
            string unixPath = Path.Combine(currentDir, "ffprobe");

            if (File.Exists(exePath))
            {
                return exePath;
            }
            else if (File.Exists(unixPath))
            {
                return unixPath;
            }

            return null;
        }

        /// <summary>
        /// Checks if ffprobe is available in the system PATH.
        /// </summary>
        /// <returns>
        /// <c>true</c> if ffprobe can be executed from PATH; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsFfprobeInPath()
        {
            try
            {
                using Process process = new();
                process.StartInfo.FileName = "ffprobe";
                process.StartInfo.Arguments = "-version";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                _ = process.Start();
                process.WaitForExit();

                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
