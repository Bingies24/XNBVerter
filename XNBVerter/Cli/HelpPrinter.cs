namespace XNBVerter.Cli
{
    /// <summary>
    /// Provides helper methods for displaying usage information
    /// and command-line instructions for the XNBVerter CLI.
    /// </summary>
    internal static class HelpPrinter
    {
        public static void ShowUsage()
        {
            Console.WriteLine("You can drag and drop files or use the command line.");
            Console.WriteLine("If no options are set, UI mode will be used.");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  XNBVerter INPUT_FILE [OPTIONS]");
            Console.WriteLine();
            Console.WriteLine("You may have input files and options wherever you want,");
            Console.WriteLine("but this formatting is recommended.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  -ot, -output_type   Sets the output type your input files should use.");
            Console.WriteLine("     song             Creates a Song XNB for .wav, .mp3, .ogg or .wma files.");
            Console.WriteLine("                      If ffprobe is not found, you will have to");
            Console.WriteLine("                      enter the length of the audio files manually.");
            Console.WriteLine();
        }
    }
}
