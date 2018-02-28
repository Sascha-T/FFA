using Discord;
using Discord.WebSocket;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Services
{
    public sealed class EvalService
    {
        private readonly SendingService _sender;
        private readonly RulesService _rulesService;
        private readonly ReputationService _repService;
        private readonly IDiscordClient _client;
        private readonly IMongoDatabase _database;

        public EvalService(SendingService sender, RulesService rulesService, ReputationService repService, DiscordSocketClient client,
            IMongoDatabase database)
        {
            _sender = sender;
            _client = client;
            _database = database;
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
                var scriptResult = await script.RunAsync(new Globals(_client, guild, _database, _sender, _rulesService, _repService));
                return EvalResult.FromSuccess(scriptResult.ReturnValue?.ToString() ?? "Success.");
            }
            catch (Exception ex)
            {
                return EvalResult.FromError(ex);
            }
        }
    }

    public struct EvalResult
    {
        private EvalResult(bool success, string result = null, Exception exception = null)
        {
            Success = success;
            Result = result;
            Exception = exception;
        }

        public static EvalResult FromSuccess(string result)
            => new EvalResult(true, result);

        public static EvalResult FromError(Exception exception)
            => new EvalResult(true, exception: exception);

        public bool Success { get; }
        public string Result { get; }
        public Exception Exception { get; }
    }

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
