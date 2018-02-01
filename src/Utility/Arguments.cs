using System;
using System.IO;
using System.Threading.Tasks;

namespace FFA.Utility
{
    public static class Arguments
    {
        public static async Task<string[]> ParseAsync(string[] args)
        {
            var configFile = "Configuration.json";
            var credentialsFile = "Credentials.json";

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-c" || args[i] == "--conf")
                {
                    configFile = args[i + 1];
                    i++;
                }
                else if (args[i] == "-C" || args[i] == "--creds")
                {
                    credentialsFile = args[i + 1];
                    i++;
                }
                else
                {
                    await TerminateAsync("\nUnknown argument: " + args[i]);
                }
            }

            if (!File.Exists(configFile))
            {
                await TerminateAsync("\nThe " + configFile + " file does not exist.");
            }
            else if (!File.Exists(credentialsFile))
            {
                await TerminateAsync("\nThe " + credentialsFile + " file does not exist.");
            }

            return new string[] { await File.ReadAllTextAsync(configFile), await File.ReadAllTextAsync(credentialsFile) };
        }

        private static async Task TerminateAsync(string message)
        {
            await ColoredConsole.WriteLineAsync(message, ConsoleColor.Red);
            await Console.Out.WriteLineAsync("\nUsage: dotnet FFA.dll [options]\n\n" +
                              "Options:\n" +
                              "  -c, --conf     The configuration file.\n" +
                              "  -C, --creds    The credentials file.\n\n" +
                              "Defaults:\n" +
                              "  -c, --conf     Configuration.json\n" +
                              "  -C, --creds    Credentials.json\n");

#if DEBUG
            Console.Read();
#endif

            Environment.Exit(-1);
        }
    }
}
