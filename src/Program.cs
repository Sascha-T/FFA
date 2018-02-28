using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FFA.Common;
using FFA.Database.Models;
using FFA.Readers;
using FFA.Utility;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

// TODO: custom commands added by users!
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
            var credentials = JsonConvert.DeserializeObject<Credentials>(parsedArgs[0], Config.JSON_SETTINGS);

            var client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                AlwaysDownloadUsers = true,
                HandlerTimeout = null
            });

            var commandService = new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Sync,
                LogLevel = LogSeverity.Info,
                IgnoreExtraArgs = true
            });

            var mongoClient = new MongoClient(credentials.DbConnectionString);
            var database = mongoClient.GetDatabase(credentials.DbName);

            // TODO: reorganize ordering of additions to service collection
            // TODO: reflexion to add all services/events/timers
            var services = new ServiceCollection() 
                .AddSingleton(credentials)
                .AddSingleton(mongoClient)
                .AddSingleton(database)
                .AddSingleton(client)
                .AddSingleton(commandService)
                .AddSingleton(new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode())));

            ServiceLoader.Load(services);

            var provider = services.BuildServiceProvider();

            EventLoader.Load(provider);

            // TODO: reflexion to add all readers!
            commandService.AddTypeReader<Rule>(new RuleReader());
            commandService.AddTypeReader<Color>(new ColorReader());
            commandService.AddTypeReader<CustomCmd>(new CustomCmdReader());
            commandService.AddTypeReader<TimeSpan>(new TimeSpanReader());

            await commandService.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
            await client.LoginAsync(TokenType.Bot, credentials.Token);
            await client.StartAsync();

            await Task.Delay(-1);
        }
    }
}
