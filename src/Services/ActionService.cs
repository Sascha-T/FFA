using Discord.Commands;
using FFA.Common;
using FFA.Entities.Service;
using System.Collections.Concurrent;

namespace FFA.Services
{
    public sealed class ActionService : Service
    {
        private readonly ConcurrentDictionary<ulong, uint> actions = new ConcurrentDictionary<ulong, uint>();

        public void Increment(Context ctx, CommandInfo cmd)
        {
            if (cmd.Module.Name != "Moderation")
                return;

            actions.AddOrUpdate(ctx.User.Id, 1, (key, val) => val + 1);
        }

        public bool Authenticate(Context ctx)
            => !actions.TryGetValue(ctx.User.Id, out uint val) || val <= ctx.DbGuild.MaxActions;

        public void Reset()
            => actions.Clear();
    }
}
