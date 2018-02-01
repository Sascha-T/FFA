using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FFA.Common;
using FFA.Events;
using FFA.Services;
using FFA.Utility;
using FFA.Database;

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
                LogLevel = LogSeverity.Debug,
                MessageCacheSize = 10
            });

            var commandService = new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug,
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
                .AddSingleton<ChannelCreated>()
                .AddSingleton<ClientLog>()
                .AddSingleton<CommandLog>();

            var provider = services.BuildServiceProvider();
            
            provider.GetRequiredService<MessageReceived>();
            provider.GetRequiredService<ChannelCreated>();
            provider.GetRequiredService<ClientLog>();
            provider.GetRequiredService<CommandLog>();

            await commandService.AddModulesAsync(Assembly.GetEntryAssembly());
            await client.LoginAsync(TokenType.Bot, credentials.Token);
            await client.StartAsync();
            
            await Task.Delay(-1);
        }
    }
}
