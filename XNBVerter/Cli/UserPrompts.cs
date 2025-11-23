using XNBVerter.Core;

namespace XNBVerter.Cli
{
    internal static class UserPrompts
    {
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
