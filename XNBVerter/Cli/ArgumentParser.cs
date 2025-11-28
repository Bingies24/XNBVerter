using XNBVerter.Core;

namespace XNBVerter.Cli
{
    /// <summary>
    /// Provides utilities for parsing command-line arguments supplied to XNBVerter.
    /// </summary>
    internal sealed class ArgumentParser
    {
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

                if (string.Equals(arg, "-ot", StringComparison.OrdinalIgnoreCase) || string.Equals(arg, "--output_type", StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 < args.Length)
                    {
                        string value = args[i + 1];

                        if (string.Equals(value, "song", StringComparison.OrdinalIgnoreCase))
                        {
                            task = TaskType.Song;
                        }

                        i++; // skip the value
                    }

                    continue;
                }

                // Anything else might be a file path
                if (File.Exists(arg))
                {
                    string ext = Path.GetExtension(arg);
                    if (FileTypes.AllFileTypes.Contains(ext))
                    {
                        files.Add(Path.GetFullPath(arg));
                    }
                }
            }

            return new ParseResult(task, files);
        }
    }
}
