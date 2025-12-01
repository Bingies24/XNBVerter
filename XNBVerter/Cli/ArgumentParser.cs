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

                switch (arg.ToLowerInvariant())
                {
                    case "-ot":
                    case "--output-type":
                        if (i + 1 >= args.Length)
                        {
                            return new ParseResult("Missing value after --output-type");
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

                        // Anything else might be a file path
                        if (File.Exists(arg))
                        {
                            string ext = Path.GetExtension(arg);
                            if (FileTypes.AllFileTypes.Contains(ext))
                            {
                                files.Add(Path.GetFullPath(arg));
                            }
                        }
                        break;
                }
            }

            return new ParseResult(task, files);
        }
    }
}
