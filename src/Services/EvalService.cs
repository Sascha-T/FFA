using Discord;
using Discord.WebSocket;
using FFA.Entities.Eval;
using FFA.Entities.Service;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class EvalService : Service
    {
        private readonly SendingService _sender;
        private readonly RulesService _rulesService;
        private readonly ReputationService _repService;
        private readonly IDiscordClient _client;
        private readonly IMongoDatabase _db;

        public EvalService(SendingService sender, RulesService rulesService, ReputationService repService, DiscordSocketClient client,
            IMongoDatabase db)
        {
            _sender = sender;
            _client = client;
            _db = db;
            _rulesService = rulesService;
            _repService = repService;
        }

        public bool TryCompile(Script script, out string errorMessage)
        {
            var diagnostics = script.Compile();
            var compilerErrors = diagnostics.Where(x => x.Severity == DiagnosticSeverity.Error);

            errorMessage = string.Empty;

            foreach (var error in compilerErrors)
                errorMessage += $"{error.GetMessage()}\n";

            return string.IsNullOrWhiteSpace(errorMessage);
        }

        public async Task<EvalResult> EvalAsync(IGuild guild, Script script)
        {
            try
            {
                var scriptResult = await script.RunAsync(new Globals(_client, guild, _db, _sender, _rulesService, _repService));
                return EvalResult.FromSuccess(scriptResult.ReturnValue?.ToString() ?? "Success.");
            }
            catch (Exception ex)
            {
                return EvalResult.FromError(ex);
            }
        }
    }
}
