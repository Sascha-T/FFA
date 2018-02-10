using Discord.Commands;
using FFA.Common;
using FFA.Preconditions;
using FFA.Services;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Threading.Tasks;

namespace FFA.Modules
{
    [BotOwner]
    public sealed class BotOwners : ModuleBase<Context>
    {
        private readonly EvalService _evalService;

        public BotOwners(EvalService evalService)
        {
            _evalService = evalService;
        }

        [Command("Eval")]
        [Summary("Evaluate C# code in a command context.")]
        public async Task EvalAsync([Summary("Client.Token")] [Remainder] string code)
        {
            var script = CSharpScript.Create(code, Configuration.SCRIPT_OPTIONS, typeof(Globals));

            if (!_evalService.TryCompile(script, out string errorMessage))
            {
                await Context.SendFieldsErrorAsync("Eval", $"```cs\n{code}```", "Compilation Error", $"```{errorMessage}```");
            }
            else
            {
                var result = await _evalService.EvalAsync(Context.Client, Context.Guild, Context.Db, script);

                if (result.Success)
                {
                    await Context.SendFieldsAsync(null, "Eval", $"```cs\n{code}```", "Result", $"```{result.Result}```");
                }
                else
                {
                    await Context.SendFieldsErrorAsync("Eval", $"```cs\n{code}```", "Runtime Error", $"```{result.Exception}```");
                }
            }
        }
    }
}
