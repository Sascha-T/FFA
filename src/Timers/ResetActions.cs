using System;
using System.Threading.Tasks;
using FFA.Common;
using FFA.Entities.FFATimer;
using FFA.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FFA.Timers
{
    public sealed class ResetActions : FFATimer
    {
        private readonly ActionService _actionService;

        public ResetActions(IServiceProvider provider) : base(provider, Config.RESET_ACTIONS_TIMER)
        {
            _actionService = provider.GetRequiredService<ActionService>();
        }

        protected override Task Execute()
        {
            _actionService.Reset();
            return Task.CompletedTask;
        }
    }
}
