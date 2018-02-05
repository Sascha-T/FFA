using Discord.Commands;
using FFA.Common;
using FFA.Database;
using FFA.Extensions;
using FFA.Preconditions;
using FFA.Services;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FFA.Modules
{
    [BotOwner]
    public class BotOwners : ModuleBase<Context>
    {
        private readonly FFAContext _ffaContext;
        private readonly SendingService _sender;
        
        public BotOwners(FFAContext ffaContext, SendingService sender)
        {
            _ffaContext = ffaContext;
            _sender = sender;
        }

        [Command("Eval")]
        [Summary("Evaluate C# code in a command context.")]
        public async Task EvalAsync([Summary("Client.Token")] [Remainder] string code)
        {
            var script = CSharpScript.Create(code, Configuration.SCRIPT_OPTIONS);
            var diagnostics = script.Compile();
            var compilerError = diagnostics.FirstOrDefault(x => x.Severity == DiagnosticSeverity.Error);

            if (compilerError != default(Diagnostic))
            {
                await Context.SendFieldsErrorAsync("Eval", $"```cs\n{code}```", "Compilation Error", $"```{compilerError.GetMessage()}```");
            }
            else
            {
                try
                {
                    var result = await script.RunAsync(new { Context, _ffaContext, _sender });
                    await Context.SendFieldsAsync(null, "Eval", $"```cs\n{code}```", "Result", $"```{result.ReturnValue?.ToString() ?? "No result."}```");
                }
                catch (Exception ex)
                {
                    await Context.SendFieldsErrorAsync("Eval", $"```cs\n{code}```", "Runtime Error", $"```{ex.LastMessage()}```");
                }
            }
        }
    }
}