using Discord;
using FFA.Database;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
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

        public EvalService(SendingService sender, RulesService rulesService, ReputationService repService)
        {
            _sender = sender;
            _rulesService = rulesService;
            _repService = repService;
        }
        
        public bool TryCompile(Script script, out string errorMessage)
        {
            var diagnostics = script.Compile();
            var compilerErrors = diagnostics.Where(x => x.Severity == DiagnosticSeverity.Error);

            errorMessage = string.Empty;

            foreach (var error in compilerErrors)
            {
                errorMessage += $"{error.GetMessage()}\n";
            }

            return string.IsNullOrWhiteSpace(errorMessage);
        }

        public async Task<EvalResult> EvalAsync(IDiscordClient client, IGuild guild, FFAContext ffaContext, Script script)
        {
            try
            {
                var scriptResult = await script.RunAsync(new Globals(client, guild, ffaContext, _sender, _rulesService, _repService));
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
        public Globals(IDiscordClient client, IGuild guild, FFAContext ffaContext, SendingService sender, RulesService rulesService,
                       ReputationService reputationService)
        {
            Client = client;
            Guild = guild;
            FFAContext = ffaContext;
            Sender = sender;
            RulesService = rulesService;
            ReputationService = reputationService;
        }

        public IDiscordClient Client { get; }
        public IGuild Guild { get; }
        public FFAContext FFAContext { get; }
        public SendingService Sender { get; }
        public RulesService RulesService { get; }
        public ReputationService ReputationService { get; }
    }
}
