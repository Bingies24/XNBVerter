using System.Diagnostics;
using System.Globalization;

namespace XNBVerter.Core
{
    /// <summary>
    /// Provides functionality for invoking <c>ffprobe</c> to determine
    /// the duration of an audio file.
    /// </summary>
    /// <remarks>
    /// This service uses <see cref="FfprobeBinaryProvider"/> to ensure <c>ffprobe</c> is available.
    /// If <c>ffprobe</c> is not found locally or in the system PATH, it will be automatically
    /// downloaded using FFMpegCore.
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
                // Use FfprobeBinaryProvider to ensure ffprobe is available
                string? fileName = FfprobeBinaryProvider.EnsureFfprobeAvailable();

                if (fileName is null)
                {
                    // ffprobe not found and could not be downloaded
                    return false;
                }

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
