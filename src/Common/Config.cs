using Discord;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace FFA.Common
{
    public static class Config
    {
        // Command handler
        public const string PREFIX = ";";

        // Current user
        public const string GAME = PREFIX + "help";

        // Moderation settings
        public const int MIN_MUTE_LENGTH = 1, CLEAR_DELETE_DELAY = 3000;

        // Spam settings
        public const int SPAM_LIMIT = 5, SPAM_REP_PENALTY = 2;
        public static readonly TimeSpan SPAM_MUTE_LENGTH = TimeSpan.FromHours(2), SPAM_DURATION = TimeSpan.FromSeconds(4);

        // Rate limit settings
        public static readonly TimeSpan IGNORE_DURATION = TimeSpan.FromHours(2);

        // Reputation commands
        public const int REP_INCREASE = 1, UNREP_DECREASE = 1;

        // Reputation requirements
        public const int TOP_MOD_COMMAND = 20, TOP_MOD = 30, TOP_COLOR = 40;

        // Maximums
        public const int MAX_CLEAR = 100,  MAX_HEX_LENGTH = 6, MAX_REASON_LENGTH = 1000, MAX_CMD_LENGTH = 200, MAX_CMD_NEW_LINES = 10;

        // Minimums
        public const int MIN_CLEAR = 3;

        // Regexes
        public static readonly Regex NEW_LINE_REGEX = new Regex(@"\r\n?|\n"), NUMBER_REGEX = new Regex(@"^\d+(\.\d+)?");

        // Defaults
        public const int CLEAR_DEFAULT = 20, LB_COUNT = 10;

        // Cooldowns in hours
        public const int REP_CD = 6, UNREP_CD = 6, COLOR_CD = 1, UNMUTE_CD = 12, MOD_CMD_CD = 1;

        // Timers in milliseconds
        public const int AUTO_UNMUTE_TIMER = 60000;

        // Logs
        public const string LOGS_DIRECTORY = "logs/";

        // Discord code responses
        public static readonly IReadOnlyDictionary<int, string> DISCORD_CODES = new Dictionary<int, string>()
        {
            { 20001, "Only a user account may perform this action." },
            { 50007, "I cannot DM you. Please allow direct messages from guild users." },
            { 50013, "I do not have permission to do that." },
            { 50034, "Discord does not allow bulk deletion of messages that are more than two weeks old." }
        }.ToImmutableDictionary();

        // HTTP code responses
        public static readonly IReadOnlyDictionary<HttpStatusCode, string> HTTP_CODES = new Dictionary<HttpStatusCode, string>()
        {
            { HttpStatusCode.Forbidden, "I do not have permission to do that." },
            { HttpStatusCode.InternalServerError, "An unexpected error has occurred, please try again later." },
            { HttpStatusCode.RequestTimeout, "The request has timed out, please try again later." }
        }.ToImmutableDictionary();

        // Custom colors
        public static readonly Color ERROR_COLOR = new Color(0xFF0000), MUTE_COLOR = new Color(0xFF3E29), UNMUTE_COLOR = new Color(0x72FF65),
            CLEAR_COLOR = new Color(0x4D3DFF);

        // Default colors
        public static readonly IReadOnlyList<Color> DEFAULT_COLORS = new Color[]
        {
            new Color(0xFF269A), new Color(0x66FFCC),
            new Color(0x00FF00), new Color(0xB10DC9),
            new Color(0x00E828), new Color(0xFFFF00),
            new Color(0x08F8FF), new Color(0x03FFAB),
            new Color(0xF226FF), new Color(0xFF00BB),
            new Color(0xFF1C8E), new Color(0x00FFFF),
            new Color(0x68FF22), new Color(0x14DEA0),
            new Color(0xFFBE11), new Color(0x0FFFFF),
            new Color(0x2954FF), new Color(0x40E0D0),
            new Color(0x9624ED), new Color(0x01ADB0),
            new Color(0xA8ED00), new Color(0xBF255F)
        }.ToImmutableArray();

        // Eval imports
        public static readonly IReadOnlyList<string> EVAL_IMPORTS = new string[]
        {
            "System",
            "System.Net",
            "System.Linq",
            "System.Threading.Tasks",
            "Discord",
            "Discord.Commands",
            "Discord.WebSocket",
            "FFA.Database.Models",
            "FFA.Extensions.Database",
            "FFA.Extensions.Discord",
            "FFA.Extensions.System",
            "MongoDB.Driver"
        }.ToImmutableArray();

        // Eval script options
        public static readonly ScriptOptions SCRIPT_OPTIONS = ScriptOptions.Default
                .WithImports(EVAL_IMPORTS)
                .WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic &&
                                !string.IsNullOrWhiteSpace(x.Location)));

        // JSON serialization settings
        public static readonly JsonSerializerSettings JSON_SETTINGS = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error
        };
    }
}
