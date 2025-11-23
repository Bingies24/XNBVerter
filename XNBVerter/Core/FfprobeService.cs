using System.Diagnostics;
using System.Globalization;

namespace XNBVerter.Core
{
    internal sealed class FfprobeService
    {
        public static bool TryGetDurationMs(string inputFile, out int durationMs)
        {
            durationMs = 0;

            try
            {
                string? processPath = Environment.ProcessPath;
                string? currentDir = processPath is not null
                    ? Path.GetDirectoryName(processPath)
                    : null;

                string? localFfprobe = null;

                if (!string.IsNullOrEmpty(currentDir))
                {
                    string exePath = Path.Combine(currentDir, "ffprobe.exe");
                    string unixPath = Path.Combine(currentDir, "ffprobe");

                    if (File.Exists(exePath))
                    {
                        localFfprobe = exePath;
                    }
                    else if (File.Exists(unixPath))
                    {
                        localFfprobe = unixPath;
                    }
                }

                string fileName = localFfprobe ?? "ffprobe";

                using Process ffprobe = new();
                ffprobe.StartInfo.FileName = fileName;
                ffprobe.StartInfo.Arguments =
                    $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{inputFile}\"";
                ffprobe.StartInfo.RedirectStandardOutput = true;
                ffprobe.StartInfo.RedirectStandardError = true;
                ffprobe.StartInfo.UseShellExecute = false;
                ffprobe.StartInfo.CreateNoWindow = true;

                if (!ffprobe.Start())
                {
                    return false;
                }

                string output = ffprobe.StandardOutput.ReadToEnd();
                ffprobe.WaitForExit();

                if (ffprobe.ExitCode != 0)
                {
                    return false;
                }

                output = output.Trim();

                if (double.TryParse(
                        output,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out double seconds))
                {
                    durationMs = (int)Math.Round(seconds * 1000.0);
                    return true;
                }

                return false;
            }
            catch
            {
                // Any error means "no duration"
                return false;
            }
        }
    }
}
