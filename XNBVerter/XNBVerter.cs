using System.Diagnostics;

Console.WriteLine("XNBVerter 1.0.0\n");

int[] tasks = [];
string[] filePaths = [];

HashSet<string> allFileTypes = new(StringComparer.OrdinalIgnoreCase)
            {
                ".wav", ".mp3", ".ogg", ".wma", ".xnb"
            };
HashSet<string> songFileTypes = new(StringComparer.OrdinalIgnoreCase)
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
                    tasks = [.. tasks, 1];
                }
                else
                {
                    break;
                }
            }
        }
        else if (File.Exists(arg))
        {
            if (allFileTypes.Contains(Path.GetExtension(arg)))
            {
                filePaths = [.. filePaths, arg];
            }
        }
    }
}

if (tasks.Length > 0)
{
    goto DoTasks;
}
else if (filePaths.Length > 0)
{
    if (!Console.IsOutputRedirected)
    {
        int optionNumber = 0;
        string inputNumber;
        while (optionNumber != 1)
        {
            Console.Clear();
            Console.WriteLine("Enter your option and press Enter/Return:");
            Console.WriteLine("1. Create Song .XNB");
            inputNumber = Console.ReadLine();
            _ = int.TryParse(inputNumber, out optionNumber);
        }
        if (optionNumber == 1)
        {
            tasks = [.. tasks, 1];
        }
    }

    goto DoTasks;
}
else
{
    Console.WriteLine("You can drag and drop files or use the command line. If no options are set UI mode will be used.");
    Console.WriteLine("XNBVerter INPUT_FILE [OPTIONS]\nYou may have input files and options wherever you want, but this formatting is recommended.");
    Console.WriteLine("    -output_type        Sets the output type your input files should use.");
    Console.WriteLine("        song            Creates a Song XNB for .wav, .mp3, .ogg or .wma files.");
    Console.WriteLine("                        If ffprobe is not found you will have to enter the length of the audio files manually.");
    if (!Console.IsOutputRedirected)
    {
        _ = Console.ReadKey(true);
    }
    return;
}

DoTasks:
if (!Console.IsOutputRedirected)
{
    Console.Clear();
}
foreach (int task in tasks)
{
    if (task == 1)
    {
        foreach (string filePath in filePaths)
        {
            if (songFileTypes.Contains(Path.GetExtension(filePath)))
            {
                Console.WriteLine($"Creating a Song XNB for {Path.GetFileName(filePath)}.");
                Console.WriteLine(CreateSongXnb(filePath));
            }
        }
    }
}
if (!Console.IsOutputRedirected)
{
    _ = Console.ReadKey(true);
}
return;

static string CreateSongXnb(string inputFile)
{
    using (FileStream file = File.Create(Path.ChangeExtension(inputFile, "xnb")))
    {
        using BinaryWriter file_writer = new(file);
        file_writer.Write("XNB".ToCharArray()); // Format indetifier
        file_writer.Write("w".ToCharArray()); // Target platform
        file_writer.Write((byte)5); // XNB format version
        file_writer.Write((byte)0); // Flag bits
        file_writer.Write(Path.GetFileName(inputFile).Length + 114); // File size
        file_writer.Write7BitEncodedInt(2); // Type reader count
        file_writer.Write("Microsoft.Xna.Framework.Content.SongReader"); // Type reader name
        file_writer.Write(0); // Reader version number
        file_writer.Write("Microsoft.Xna.Framework.Content.Int32Reader"); // Primary asset data?
        file_writer.Write(0); // I don't know what these are
        file_writer.Write((byte)0); // But it works so
        file_writer.Write((byte)1); // I'm fine with it
        file_writer.Write(Path.GetFileName(inputFile)); // Streaming filename
        file_writer.Write((byte)2); // Int32 Object ID?

        int duration = 0;
        string currentDir = Path.GetDirectoryName(Environment.ProcessPath);
        if (File.Exists(Path.Join(currentDir, "ffprobe.exe")) || File.Exists(Path.Join(currentDir, "ffprobe")))
        {
            using Process ffprobe = new();
            ffprobe.StartInfo.FileName = Path.Join(currentDir, "ffprobe");
            ffprobe.StartInfo.Arguments = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{inputFile}\"";
            ffprobe.StartInfo.RedirectStandardOutput = true;
            ffprobe.StartInfo.UseShellExecute = false;
            ffprobe.StartInfo.CreateNoWindow = true;
            _ = ffprobe.Start();

            string output = ffprobe.StandardOutput.ReadToEnd();
            ffprobe.WaitForExit();
            if (output != null)
            {
                duration = (int)Math.Round(decimal.Parse(output) * 1000);
            }
        }
        else
        {
            if (!Console.IsOutputRedirected) // Add exception throw using else
            {
                int realNumber = 0;
                string inputNumber = "";
                while (!int.TryParse(inputNumber, out realNumber))
                {
                    Console.Clear();
                    Console.WriteLine($"Enter the duration of {inputFile} in milliseconds.");
                    inputNumber = Console.ReadLine();
                }
            }
        }
        file_writer.Write(duration); // Duration in milliseconds
    }
    return File.Exists(Path.ChangeExtension(inputFile, "xnb")) ? "XNB succesfully created!" : "Something went wrong...";
}
