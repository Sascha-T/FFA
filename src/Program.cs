using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FFA.Common;
using FFA.Utility;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

// TODO: clear all duplicates from DB!
// TODO: README, contributing, all other github things.
// TODO: move all command checks to preconditions!
namespace FFA
{
    public sealed class Program
    {
        private static void Main(string[] args)
            => new Program().StartAsync(args).GetAwaiter().GetResult();

        private async Task StartAsync(string[] args)
        {
            var parsedArgs = await Arguments.ParseAsync(args);
            var credsFileName = parsedArgs["credentials"];

            if (!File.Exists(credsFileName))
                await Arguments.TerminateAsync($"The {credsFileName} file does not exist.");

            var creds = JsonConvert.DeserializeObject<Credentials>(await File.ReadAllTextAsync(credsFileName), Config.JSON_SETTINGS);

            var client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                AlwaysDownloadUsers = true,
                HandlerTimeout = null
            });

            var commands = new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Sync,
                LogLevel = LogSeverity.Info,
                IgnoreExtraArgs = true
            });

            var rand = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
            var mongo = new MongoClient(creds.DbConnectionString);
            var db = mongo.GetDatabase(creds.DbName);

            var services = new ServiceCollection() 
                .AddSingleton(creds)
                .AddSingleton(mongo)
                .AddSingleton(db)
                .AddSingleton(client)
                .AddSingleton(commands)
                .AddSingleton(rand);

            Loader.LoadServices(services);
            Loader.LoadCollections(services, db);

            var provider = services.BuildServiceProvider();

            Loader.LoadEvents(provider);
            Loader.LoadReaders(commands);

            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
            await client.LoginAsync(TokenType.Bot, creds.Token);
            await client.StartAsync();

            await Task.Delay(-1);
        }
    }
}
