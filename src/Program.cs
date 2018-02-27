using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FFA.Common;
using FFA.Database.Models;
using FFA.Events;
using FFA.Readers;
using FFA.Services;
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
// TODO: Rules revamp
// TODO: move all command checks to preconditions!
namespace FFA
{
    internal sealed class Program
    {
        private static void Main(string[] args)
            => new Program().StartAsync(args).GetAwaiter().GetResult();

        private async Task StartAsync(string[] args)
        {
            var parsedArgs = await Arguments.ParseAsync(args);
            var credentials = JsonConvert.DeserializeObject<Credentials>(parsedArgs[0], Configuration.JSON_SETTINGS);

            var client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 10,
                AlwaysDownloadUsers = true
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
                // TODO: array of collections and loop to get?
                .AddSingleton(database.GetCollection<User>("users"))
                .AddSingleton(database.GetCollection<Guild>("guilds"))
                .AddSingleton(database.GetCollection<Mute>("mutes"))
                .AddSingleton(database.GetCollection<Rule>("rules"))
                .AddSingleton(database.GetCollection<Poll>("polls"))
                .AddSingleton(database.GetCollection<Vote>("votes"))
                .AddSingleton<LoggingService>()
                .AddSingleton(client)
                .AddSingleton(commandService)
                .AddSingleton(new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode())))
                .AddSingleton<SendingService>()
                .AddSingleton<RulesService>()
                .AddSingleton<RateLimitService>()
                .AddSingleton<ResultService>()
                .AddSingleton<MessageReceived>()
                .AddSingleton<ModerationService>()
                .AddSingleton<SpamService>()
                .AddSingleton<ReputationService>()
                .AddSingleton<EvalService>();

            var provider = services.BuildServiceProvider();

            new ClientLog(provider);
            new MessageReceived(provider);
            new Ready(provider);
            new UserJoined(provider);

            commandService.AddTypeReader<Rule>(new RuleTypeReader());
            commandService.AddTypeReader<Color>(new ColorTypeReader());

            await commandService.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
            await client.LoginAsync(TokenType.Bot, credentials.Token);
            await client.StartAsync();

            await Task.Delay(-1);
        }
    }
}
