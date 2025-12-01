using XNBVerter.Core;

namespace XNBVerter.Cli
{
    /// <summary>
    /// Provides utilities for parsing command-line arguments supplied to XNBVerter.
    /// </summary>
    internal sealed class ArgumentParser
    {
        /// <summary>
        /// Checks if a value exists after the current argument.
        /// </summary>
        /// <param name="args">The arguments array.</param>
        /// <param name="currentIndex">The current argument index.</param>
        /// <param name="optionName">The name of the option that requires a value.</param>
        /// <param name="result">The error result if no value is found.</param>
        /// <returns>True if a value exists, false otherwise.</returns>
        private static bool TryGetNextValue(string[] args, int currentIndex, string optionName, out ParseResult? result)
        {
            if (currentIndex + 1 >= args.Length)
            {
                result = new ParseResult($"Missing value after {optionName}");
                return false;
            }

            result = null;
            return true;
        }

        public static ParseResult Parse(string[] args)
        {
            List<string> files = [];
            TaskType? task = null;

            if (args is null || args.Length == 0)
            {
                return new ParseResult(task, files);
            }

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                switch (arg.ToLowerInvariant())
                {
                    case "-ot":
                    case "--output-type":
                        if (!TryGetNextValue(args, i, arg, out ParseResult? error))
                        {
                            return error!;
                        }

                        string value = args[++i];

                        switch (value.ToLowerInvariant())
                        {
                            case "song":
                                task = TaskType.Song;
                                break;

                            default:
                                return new ParseResult($"Unknown output type: {value}");
                        }
                        break;

                    default:
                        if (arg.StartsWith('-'))
                        {
                            return new ParseResult($"Unknown option: {arg}");
                        }

                        // Anything else is treated as a file path
                        // Check if it's a directory that exists
                        if (Directory.Exists(arg))
                        {
                            return new ParseResult($"Expected a file, but got a directory: {arg}");
                        }

                        if (!File.Exists(arg))
                        {
                            return new ParseResult($"File not found: {arg}");
                        }

                        // Accept any file that exists - task-specific validation happens later
                        files.Add(Path.GetFullPath(arg));
                        break;
                }
            }

            return new ParseResult(task, files);
        }
    }
}
