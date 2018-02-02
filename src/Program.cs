using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FFA.Common;
using FFA.Database;
using FFA.Database.Models;
using FFA.Events;
using FFA.Readers;
using FFA.Services;
using FFA.Timers;
using FFA.Utility;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FFA
{
    public class Program
    {
        static void Main(string[] args)
            => new Program().StartAsync(args).GetAwaiter().GetResult();

        public async Task StartAsync(string[] args)
        {
            var parsedArgs = await Arguments.ParseAsync(args);
            var config = JsonConvert.DeserializeObject<Configuration>(parsedArgs[0]);
            var credentials = JsonConvert.DeserializeObject<Credentials>(parsedArgs[1]);

            var client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 10
            });

            var commandService = new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Info,
                IgnoreExtraArgs = true
            });
            
            var services = new ServiceCollection()
                .AddDbContext<FFAContext>(ServiceLifetime.Transient)
                .AddSingleton<Logger>()
                .AddSingleton(client)
                .AddSingleton(commandService)
                .AddSingleton(new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode())))
                .AddSingleton(config)
                .AddSingleton(credentials)
                .AddSingleton<Sender>()
                .AddSingleton<MessageReceived>()
                .AddSingleton<ClientLog>()
                .AddSingleton<CommandLog>()
                .AddSingleton<Ready>()
                .AddSingleton<AutoUnmute>();

            var provider = services.BuildServiceProvider();

            commandService.AddTypeReader<Rule>(new RuleTypeReader());
            commandService.AddTypeReader<TimeSpan>(new TimeSpanTypeReader());
            await commandService.AddModulesAsync(Assembly.GetEntryAssembly());
            await client.LoginAsync(TokenType.Bot, credentials.Token);
            await client.StartAsync();

            // TODO: reflexion?
            provider.GetRequiredService<MessageReceived>();
            provider.GetRequiredService<ClientLog>();
            provider.GetRequiredService<CommandLog>();
            provider.GetRequiredService<Ready>();

            await Task.Delay(-1);
        }
    }
}
