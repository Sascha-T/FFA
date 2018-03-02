using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FFA.Utility
{
    public static class Arguments
    {
        public static async Task<IReadOnlyDictionary<string, string>> ParseAsync(string[] args)
        {
            var parsedArgs = new Dictionary<string, string>
            {
                { "credentials",  "credentials.json" }
            };

            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == "-C" || args[i] == "--creds")
                {
                    parsedArgs["credentials"] = args[i + 1];
                    i++;
                }
                else
                {
                    await TerminateAsync($"Unknown argument: {args[i]}.");
                }
            }

            return parsedArgs;
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
