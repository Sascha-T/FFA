using System;
using System.Threading.Tasks;
using Discord;
using FFA.Entities.Event;
using FFA.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FFA.Events
{
    public sealed class MessageDeleted : Event
    {
        private readonly DeletedMessagesService _deletedMsgsService;

        public MessageDeleted(IServiceProvider provider) : base(provider)
        {
            _deletedMsgsService = provider.GetRequiredService<DeletedMessagesService>();

            _client.MessageDeleted += OnMessageDeletedAsync;
        }

        public async Task OnMessageDeletedAsync(Cacheable<IMessage, ulong> cacheableMsg, IMessageChannel channel)
        {
            var msg = await cacheableMsg.GetOrDownloadAsync();

            if (msg is IUserMessage userMsg && !userMsg.Author.IsBot)
                _deletedMsgsService.Add(channel.Id, userMsg);
        }
    }
}
