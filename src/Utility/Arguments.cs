using System;
using System.IO;
using System.Threading.Tasks;

namespace FFA.Utility
{
    internal static class Arguments
    {
        // TODO: proper parsing instead of reading files inside parser LOL
        internal static async Task<string[]> ParseAsync(string[] args)
        {
            var credentialsFile = "Credentials.json";

            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == "-C" || args[i] == "--creds")
                {
                    credentialsFile = args[i + 1];
                    i++;
                }
                else
                {
                    await TerminateAsync($"Unknown argument: {args[i]}");
                }
            }

            if (!File.Exists(credentialsFile))
            {
                await TerminateAsync($"The {credentialsFile} file does not exist.");
            }

            return new string[] { await File.ReadAllTextAsync(credentialsFile) };
        }

        internal static async Task TerminateAsync(string message)
        {
            await ColoredConsole.WriteLineAsync($"\n{message}", ConsoleColor.Red);
            await Console.Out.WriteLineAsync("\nUsage: dotnet FFA.dll [options]\n\n" +
                                             "Options:\n" +
                                             "  -C, --creds    The credentials file.\n\n" +
                                             "Defaults:\n" +
                                             "  -C, --creds    Credentials.json\n");

#if DEBUG
            Console.Read();
#endif

            Environment.Exit(-1);
        }
    }
}
