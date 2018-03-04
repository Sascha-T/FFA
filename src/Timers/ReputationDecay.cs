using System;
using System.Threading.Tasks;
using FFA.Common;
using FFA.Database.Models;
using FFA.Entities.FFATimer;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace FFA.Timers
{
    public sealed class ReputationDecay : FFATimer
    {
        private readonly IMongoCollection<User> _dbUsers;

        public ReputationDecay(IServiceProvider provider) : base(provider, Config.REP_DECAY_TIMER)
        {
            _dbUsers = provider.GetRequiredService<IMongoCollection<User>>();
        }

        protected override Task Execute()
            => _dbUsers.UpdateManyAsync(FilterDefinition<User>.Empty, Config.DECAY_UPDATE);
    }
}
