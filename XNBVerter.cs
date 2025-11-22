class XNBVerter
{
    static void Main(string[] args)
    {
        Console.WriteLine("XNBVerter 1.0.0\n");

        int[] tasks = { };
        string[] filePaths = { };

        var allFileTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".wav", ".mp3", ".ogg", ".wma", ".xnb"
            };
        var songFileTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".wav", ".mp3", ".ogg", ".wma"
            };

        if (args.Length > 0)
        {
            int prevArgIndex = -1;
            foreach (string arg in args)
            {
                if (arg == "-output_type")
                {
                    prevArgIndex = Array.IndexOf(args, arg, prevArgIndex + 1);
                    if (prevArgIndex + 1 < args.Length)
                    {
                        if (args[prevArgIndex + 1] == "song" && !tasks.Contains(1))
                        {
                            tasks = tasks.Append(1).ToArray();
                        }
                        else break;
                    }
                }
                else if (File.Exists(arg))
                {
                    if (allFileTypes.Contains(Path.GetExtension(arg)))
                    {
                        filePaths = filePaths.Append(arg).ToArray();
                    }
                }
            }
        }

        if (tasks.Length > 0) goto DoTasks;
        else if (filePaths.Length > 0)
        {
            Console.WriteLine("Placeholder for UI code.");
            int optionNumber = 0;
            string inputNumber = "";
            while (optionNumber != 1)
            {
                Console.Clear();
                Console.WriteLine("Enter your option and press Enter/Return:");
                Console.WriteLine("1. Create Song .XNB");
                inputNumber = Console.ReadLine();
                int.TryParse(inputNumber, out optionNumber);
            }
            if (optionNumber == 1) tasks = tasks.Append(1).ToArray();
            goto DoTasks;
        }
        else
        {
            Console.WriteLine("You can drag and drop files or use the command line. If no options are set UI mode will be used.");
            Console.WriteLine("XNBVerter INPUT_FILE [OPTIONS]\nYou may have input files and options wherever you want, but this formatting is recommended.");
            Console.WriteLine("    -output_type        Sets the output type your input files should use.");
            Console.WriteLine("        song                Creates a Song XNB for .wav, .mp3, .ogg or .wma files.\n                            If ffprobe is not found you will have to enter the length of the audio files manually.");
            Console.ReadKey(true);
            return;
        }

        DoTasks:
            Console.Clear();
            foreach (int task in tasks)
            {
                if (task == 1)
                {
                    foreach (string filePath in filePaths)
                    {
                        if (songFileTypes.Contains(Path.GetExtension(filePath)))
                        {
                            Console.WriteLine("Placeholder for running the Song XNB creation class.");
                        }
                    }
                }
            }
            Console.ReadKey(true);
            return;
    }
}