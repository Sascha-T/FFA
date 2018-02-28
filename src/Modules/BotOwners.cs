using Discord;
using Discord.Commands;
using FFA.Common;
using FFA.Preconditions.Command;
using FFA.Services;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FFA.Modules
{
    [BotOwner]
    public sealed class BotOwners : ModuleBase<Context>
    {
        private readonly EvalService _evalService;
        private readonly LoggingService _logger;

        public BotOwners(EvalService evalService, LoggingService logger)
        {
            _evalService = evalService;
            _logger = logger;
        }

        [Command("Reboot")]
        [Alias("restart")]
        [Summary("Reboots the bot.")]
        public async Task RebootAsync()
        {
            await Context.ReplyAsync("Rebooting...");
            Environment.Exit(0);
        }

        [Command("ErrorLogs")]
        [Alias("errorlog")]
        [Summary("Sends the error logs as an attached file.")]
        public async Task ErrorLogsAsync()
        {
            var fileName = _logger.LogFileName(LogSeverity.Error);
            
            if (!File.Exists(fileName))
                await Context.ReplyErrorAsync("No error log file has been created.");
            else
                await Context.Channel.SendFileAsync(fileName);
        }

        [Command("LastErrorLogs")]
        [Alias("lasterror")]
        [Summary("Sends the most recent error logs.")]
        public async Task ErrorLogsAsync(int lineCount = 20)
        {
            var fileName = _logger.LogFileName(LogSeverity.Error);

            if (!File.Exists(fileName))
            {
                await Context.ReplyErrorAsync("No error log file has been created.");
            }
            else
            {
                var lines = await File.ReadAllLinesAsync(fileName);
                var message = "```";

                for (int i = lineCount >= lines.Length ? 0 : lines.Length - lineCount; i < lines.Length; i++)
                    message += $"{lines[i]}\n";

                await ReplyAsync($"{message}```");
            }
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
                var result = await _evalService.EvalAsync(Context.Guild, script);

                if (result.Success)
                    await Context.SendFieldsAsync(null, "Eval", $"```cs\n{code}```", "Result", $"```{result.Result}```");
                else
                    await Context.SendFieldsErrorAsync("Eval", $"```cs\n{code}```", "Runtime Error", $"```{result.Exception}```");
            }
        }
    }
}
