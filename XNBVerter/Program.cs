using System.Reflection;

using XNBVerter.Cli;
using XNBVerter.Core;

namespace XNBVerter
{
    /// <summary>
    /// Entry point class for the XNBVerter application.
    /// </summary>
    internal static class Program
    {
        private static Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        /// Main entry point for the application.
        /// </summary>
        /// <param name="args">CLI arguments.</param>
        /// <returns>Exit code: 0 for success, 1 for failure.</returns>
        private static int Main(string[] args)
        {
            Console.WriteLine($"XNBVerter {appVersion}\n");

            ParseResult result = ArgumentParser.Parse(args);

            // No input files at all -> show help and exit
            if (result.FilePaths.Count == 0)
            {
                HelpPrinter.ShowUsage();
                WaitIfInteractive();
                return 1;
            }

            TaskType task = result.Task ?? TaskType.None;

            // If no task from CLI but we have files and an interactive console -> ask user
            if (task == TaskType.None && !Console.IsOutputRedirected)
            {
                task = UserPrompts.AskTask();
            }

            if (task == TaskType.None)
            {
                Console.WriteLine("No tasks selected. Exiting.");
                WaitIfInteractive();
                return 1;
            }

            if (!Console.IsOutputRedirected)
            {
                Console.Clear();
                Console.WriteLine($"XNBVerter {appVersion}\n");
            }

            FfprobeService ffprobe = new();
            SongXnbCreator songCreator = new();

            try
            {
                RunTasks(task, result.FilePaths, ffprobe, songCreator);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                WaitIfInteractive();
                return 1;
            }

            WaitIfInteractive();
            return 0;
        }

        /// <summary>
        /// Routes execution to the appropriate task handler based on the specified task type.
        /// </summary>
        /// <param name="task">The type of task to execute.</param>
        /// <param name="filePaths">List of file paths to process.</param>
        /// <param name="ffprobe">FFprobe service for retrieving audio metadata.</param>
        /// <param name="songCreator">Service for creating Song XNB files.</param>
        private static void RunTasks(
            TaskType task,
            IReadOnlyList<string> filePaths,
            FfprobeService ffprobe,
            SongXnbCreator songCreator)
        {
            switch (task)
            {
                case TaskType.Song:
                    RunSongTask(filePaths, ffprobe, songCreator);
                    break;
                case TaskType.None:
                    break;
                default:
                    Console.WriteLine("Unsupported task type.");
                    break;
            }
        }

        /// <summary>
        /// Processes audio files and creates Song XNB files for each valid input.
        /// </summary>
        /// <param name="filePaths">List of file paths to process.</param>
        /// <param name="ffprobe">FFprobe service for retrieving audio duration metadata.</param>
        /// <param name="creator">Service for creating Song XNB files.</param>
        private static void RunSongTask(
            IReadOnlyList<string> filePaths,
            FfprobeService ffprobe,
            SongXnbCreator creator)
        {
            foreach (string filePath in filePaths)
            {
                if (!FileTypes.SongFileTypes.Contains(Path.GetExtension(filePath)))
                {
                    continue;
                }

                Console.WriteLine($"Creating Song XNB for {Path.GetFileName(filePath)}...");

                int durationMs;

                if (FfprobeService.TryGetDurationMs(filePath, out int ffprobeDuration))
                {
                    durationMs = ffprobeDuration;
                }
                else if (!Console.IsOutputRedirected)
                {
                    durationMs = UserPrompts.AskDurationMs(filePath);
                }
                else
                {
                    // Non-interactive and no ffprobe -> fallback to 0 ms
                    durationMs = 0;
                }

                SongXnbCreator.CreateSongXnb(filePath, durationMs);

                Console.WriteLine($"✅ XNB created: {Path.ChangeExtension(filePath, ".xnb")}");
            }
        }

        /// <summary>
        /// Prompts the user to press a key before exiting if running in an interactive console.
        /// </summary>
        private static void WaitIfInteractive()
        {
            if (!Console.IsOutputRedirected)
            {
                Console.WriteLine();
                Console.Write("Press any key to exit...");
                _ = Console.ReadKey(true);
            }
        }
    }
}
