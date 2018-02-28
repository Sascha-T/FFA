using System;
using System.IO;
using System.Threading.Tasks;

namespace FFA.Utility
{
    public static class Arguments
    {
        // TODO: proper parsing instead of reading files inside parser LOL
        public static async Task<string[]> ParseAsync(string[] args)
        {
            var credentialsFile = "credentials.json";

            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == "-C" || args[i] == "--creds")
                    credentialsFile = args[1 + i++];
                else
                    await TerminateAsync($"Unknown argument: {args[i]}.");
            }

            if (!File.Exists(credentialsFile))
                await TerminateAsync($"The {credentialsFile} file does not exist.");

            return new string[] { await File.ReadAllTextAsync(credentialsFile) };
        }

        public static async Task TerminateAsync(string message)
        {
            await ColoredConsole.WriteLineAsync($"\n{message}", ConsoleColor.Red);
            await Console.Out.WriteLineAsync("\nUsage: dotnet FFA.dll [options]\n\n" +
                                             "Options:\n" +
                                             "  -C, --creds    The credentials file.\n\n" +
                                             "Defaults:\n" +
                                             "  -C, --creds    credentials.json\n");

#if DEBUG
            Console.Read();
#endif

            Environment.Exit(-1);
        }
    }
}
