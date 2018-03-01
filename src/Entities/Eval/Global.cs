using Discord;
using FFA.Services;
using MongoDB.Driver;

namespace FFA.Entities.Eval
{
    public class Globals
    {
        public Globals(IDiscordClient client, IGuild guild, IMongoDatabase database, SendingService sender, RulesService rulesService,
                       ReputationService reputationService)
        {
            Client = client;
            Guild = guild;
            Database = database;
            Sender = sender;
            RulesService = rulesService;
            ReputationService = reputationService;
        }

        public IDiscordClient Client { get; }
        public IGuild Guild { get; }
        public IMongoDatabase Database { get; }
        public SendingService Sender { get; }
        public RulesService RulesService { get; }
        public ReputationService ReputationService { get; }
    }
}
