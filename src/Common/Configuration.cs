using Discord;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;

namespace FFA.Common
{
    internal static class Configuration
    {
        // Command handler
        internal const string PREFIX = ";";

        // Current user
        internal const string GAME = PREFIX + "help";

        // Moderation settings
        internal const int MIN_MUTE_LENGTH = 1, CLEAR_DELETE_DELAY = 3000;

        // Spam settings
        internal const int SPAM_LIMIT = 4, SPAM_REP_PENALTY = 2;
        internal const double SPAM_SIMILARITY = 0.8;
        internal static readonly TimeSpan SPAM_DURATION = TimeSpan.FromSeconds(5), SPAM_MUTE_LENGTH = TimeSpan.FromHours(2);

        // Reputation commands
        internal const int REP_INCREASE = 1, UNREP_DECREASE = 1;

        // Reputation requirements
        internal const int TOP_MOD = 30, TOP_COLOR = 40;

        // Maximums
        internal const int MAX_CLEAR = 100, MAX_ROLES = 500, MAX_HEX_LENGTH = 6;

        // Minimums
        internal const int MIN_CLEAR = 3;

        // Defaults
        internal const int CLEAR_DEFAULT = 20, LB_COUNT_DEFAULT = 10;

        // Cooldowns in hours
        internal const int REP_COOLDOWN = 6, UNREP_COOLDOWN = 6, COLOR_COOLDOWN = 1, UNMUTE_COOLDOWN = 12;

        // Timers in milliseconds
        internal const int AUTO_UNMUTE_TIMER = 60000;

        // Logs
        internal const string LOGS_DIRECTORY = "logs/";

        // Discord code responses
        internal static readonly IReadOnlyDictionary<int, string> DISCORD_CODE_RESPONSES = new Dictionary<int, string>()
        {
            { 20001, "Only a user account may perform this action." },
            { 50007, "I cannot DM you. Please allow direct messages from guild users." },
            { 50013, "I do not have permission to do that." },
            { 50034, "Discord does not allow bulk deletion of messages that are more than two weeks old." }
        }.ToImmutableDictionary();

        // HTTP code responses
        internal static readonly IReadOnlyDictionary<HttpStatusCode, string> HTTP_CODE_RESPONSES = new Dictionary<HttpStatusCode, string>()
        {
            { HttpStatusCode.Forbidden, "I do not have permission to do that." },
            { HttpStatusCode.InternalServerError, "An unexpected error has occurred, please try again later." },
            { HttpStatusCode.RequestTimeout, "The request has timed out, please try again later." }
        }.ToImmutableDictionary();

        // Custom colors
        internal static readonly Color ERROR_COLOR = new Color(0xFF0000), MUTE_COLOR = new Color(0xFF3E29), UNMUTE_COLOR = new Color(0x72FF65),
                                       CLEAR_COLOR = new Color(0x4D3DFF);

        // Default colors
        // TODO: more colors!!!
        internal static readonly IReadOnlyList<Color> DEFAULT_COLORS = new Color[]
        {
            new Color(0xFF269A),
            new Color(0x00FF00),
            new Color(0x00E828),
            new Color(0x08F8FF),
            new Color(0xF226FF),
            new Color(0xFF1C8E),
            new Color(0x68FF22),
            new Color(0xFFBE11),
            new Color(0x2954FF),
            new Color(0x9624ED),
            new Color(0xA8ED00)
        }.ToImmutableArray();

        // Eval imports
        internal static readonly IReadOnlyList<string> EVAL_IMPORTS = new string[]
        {
            "System",
            "System.Linq",
            "System.Threading.Tasks",
            "Discord",
            "Discord.Commands",
            "Discord.WebSocket"
        }.ToImmutableArray();

        // Eval script options
        internal static readonly ScriptOptions SCRIPT_OPTIONS = ScriptOptions.Default
                .WithImports(EVAL_IMPORTS)
                .WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic &&
                                !string.IsNullOrWhiteSpace(x.Location)));

        // JSON serialization settings
        internal static readonly JsonSerializerSettings JSON_SETTINGS = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error
        };
    }
}
