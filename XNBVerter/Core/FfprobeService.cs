using System.Diagnostics;
using System.Globalization;

namespace XNBVerter.Core
{
    /// <summary>
    /// Provides functionality for invoking <c>ffprobe</c> to determine
    /// the duration of an audio file.
    /// </summary>
    /// <remarks>
    /// This service first checks for a local <c>ffprobe</c> executable located
    /// in the application's directory.  
    /// If not found, it falls back to invoking <c>ffprobe</c> from the system PATH.
    /// </remarks>
    internal sealed class FfprobeService
    {
        /// <summary>
        /// Attempts to read the duration of an audio file in milliseconds
        /// by executing <c>ffprobe</c>.
        /// </summary>
        /// <param name="inputFile">
        /// The audio file whose duration should be measured.
        /// </param>
        /// <param name="durationMs">
        /// When this method returns <c>true</c>, contains the duration of the file
        /// in milliseconds; otherwise, <c>0</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <c>ffprobe</c> executed successfully and returned
        /// a parsable numeric duration; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The method suppresses most exceptions to simplify upstream usage.
        /// Common failures (missing executable, process error, invalid output)
        /// results in a <c>false</c> return value.
        /// </para>
        ///
        /// <para>
        /// Expected <c>ffprobe</c> output is a plain floating-point number
        /// representing seconds, which is then converted to milliseconds.
        /// </para>
        /// </remarks>
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
                ffprobe.StartInfo.RedirectStandardOutput = true;  // Capture ffprobe's duration output
                ffprobe.StartInfo.RedirectStandardError = true;   // Suppress error messages from showing in console
                ffprobe.StartInfo.UseShellExecute = false;        // Required for stream redirection
                ffprobe.StartInfo.CreateNoWindow = true;          // Prevent console window from flashing

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
