using XNBVerter.Core;

namespace XNBVerter.Cli
{
    /// <summary>
    /// Provides interactive console prompts used when running XNBVerter
    /// in non-automated mode.
    /// </summary>
    internal static class UserPrompts
    {
        /// <summary>
        /// Displays a menu asking the user which task they want to perform
        /// and waits until a valid option is provided.
        /// </summary>
        public static TaskType AskTask()
        {
            int optionNumber = 0;

            while (optionNumber != 1)
            {
                Console.Clear();
                Console.WriteLine("Enter your option and press Enter/Return:");
                Console.WriteLine("1. Create Song .XNB");
                Console.Write("> ");

                string? inputNumber = Console.ReadLine();
                _ = int.TryParse(inputNumber, out optionNumber);
            }

            return TaskType.Song;
        }

        /// <summary>
        /// Prompts the user to manually enter the duration of an audio file
        /// in milliseconds, validating that the input is a non-negative integer.
        /// </summary>
        public static int AskDurationMs(string inputFile)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Enter the duration of:");
                Console.WriteLine(inputFile);
                Console.WriteLine("in milliseconds (integer):");
                Console.Write("> ");

                string? inputNumber = Console.ReadLine();
                if (int.TryParse(inputNumber, out int realNumber) && realNumber >= 0)
                {
                    return realNumber;
                }

                Console.WriteLine("Invalid duration. Press any key and try again...");
                _ = Console.ReadKey(true);
            }
        }
    }
}
