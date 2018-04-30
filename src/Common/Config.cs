using Discord;
using FFA.Database.Models;
using Microsoft.CodeAnalysis.Scripting;
using MongoDB.Driver;
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

        // Guild, TODO: move to some json thing
        public const string INVITE_LINK = "https://discord.gg/F7reg7e", HELP_MESSAGE =
            "FFA's goal is to provide a fully decentralized discord server by allowing the community to control " +
            "every aspect of the guild. The entire system is based around reputation. The most reputable users are the " +
            "moderators. If believe a certain user is suitable to moderate, you may enter the following: `" + PREFIX +
            "rep username#1234`. The opposite can be done with `" + PREFIX + "unrep`.\n\nIt is essential that reputation " +
            "remains in the right hands, as everything revolves around it. It is your duty as a member of this community " +
            "to ensure that if a user was unjustly punished, the culprit must pay the consequences while vindicating the " +
            "victim. You may view anyone's reputation by using `" + PREFIX + "getrep`.\n\nIf you wish to view the various " +
            "command categories, you may use `" + PREFIX + "modules`. To view all the commands of a module, you may use `" +
            PREFIX + "module general`. You may also view all commands by using `" + PREFIX + "commands`. If you wish " +
            "to view the progress of this bot, or simply support the creators, you may join the official FFA server here: "
            + INVITE_LINK + ".";

        // Account age required in days
        public const int MEMBER_AGE = 2;

        // Chat settings
        public static readonly TimeSpan CHAT_SERVICE_DELAY = TimeSpan.FromSeconds(30);
        public const double CHAT_REWARD = 0.025;

        // Reputation decay settings
        public const double DECAY_MUL = 0.99;
        public static readonly UpdateDefinition<User> DECAY_UPDATE =
            new UpdateDefinitionBuilder<User>().Mul(x => x.Reputation, DECAY_MUL);

        // Moderation settings
        public const int CLEAR_DELETE_DELAY = 3000;

        // Spam settings
        public const double SPAM_REP_PENALTY = 2;
        public const int SPAM_LIMIT = 5; 
        public static readonly TimeSpan SPAM_MUTE_LENGTH = TimeSpan.FromHours(6),
            SPAM_DURATION = TimeSpan.FromSeconds(4);

        // Deleted messages settings
        public const int DELETED_MESSAGES_CHARS = 350;

        // Rate limit settings
        public static readonly TimeSpan IGNORE_DURATION = TimeSpan.FromMinutes(30);

        // Reputation commands
        public const double REP_INCREASE = 1, UNREP_DECREASE = 1;

        // Reputation requirements
        public const int TOP_REMOVE_EMOTE = 20, TOP_MOD_CMD = 20, TOP_REMOVE_CMD = 20, TOP_MOD = 30, TOP_COLOR = 40,
            TOP_ADD_EMOTE = 40;

        // Maximums
        public const int MAX_LB = 30, MAX_CLEAR = 100, MAX_HEX_LENGTH = 6, MAX_REASON_LENGTH = 600,
            MAX_CMD_LENGTH = 500, MAX_CMD_NEW_LINES = 10, MAX_DELETED_MSGS = 10;

        // Minimums
        public const int MIN_LB = 5, MIN_CLEAR = 3, MIN_DELETED_MSGS = 1, MIN_MUTE_LENGTH = 1;

        // Regexes
        public static readonly Regex NEW_LINE_REGEX = new Regex(@"\r\n?|\n"), NUMBER_REGEX = new Regex(@"^\d+(\.\d+)?"),
            EMOTE_REGEX = new Regex(@"<:.+:\d+>"), MENTION_REGEX = new Regex(@"@here|@everyone|<@!?\d+>"),
            EMOTE_ID_REGEX = new Regex(@"<:.+:|>"), CAMEL_CASE = new Regex("(\\B[A-Z])"),
            MARKDOWN_REGEX = new Regex(@"\*|`|_|~");

        // Defaults
        public const int CLEAR_DEFAULT = 20, LB_COUNT = 10, DELETED_MSGS = 5;

        // Cooldowns in hours
        public const double REP_CD = 6, UNREP_CD = 6, COLOR_CD = 1, UNMUTE_CD = 12, MOD_CMD_CD = 1, REMOVE_CMD_CD = 1,
            ADD_EMOTE_CD = 0.5, REMOVE_EMOTE_CD = 0.5;

        // Timers
        public static readonly TimeSpan AUTO_UNMUTE_TIMER = TimeSpan.FromMinutes(1),
            REP_DECAY_TIMER = TimeSpan.FromHours(1), DISBOARD_BUMP_TIMER = TimeSpan.FromHours(1),
            SERVER_HOUND_BUMP_TIMER = TimeSpan.FromHours(4), RESET_ACTIONS_TIMER = TimeSpan.FromHours(1);

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
        public static readonly IReadOnlyDictionary<HttpStatusCode, string> HTTP_CODES =
            new Dictionary<HttpStatusCode, string>()
            {
                { HttpStatusCode.Forbidden, "I do not have permission to do that." },
                { HttpStatusCode.InternalServerError, "An unexpected error has occurred, please try again later." },
                { HttpStatusCode.RequestTimeout, "The request has timed out, please try again later." }
            }.ToImmutableDictionary();

        // Custom colors
        public static readonly Color ERROR_COLOR = new Color(0xFF0000), MUTE_COLOR = new Color(0xFF3E29),
            UNMUTE_COLOR = new Color(0x72FF65), CLEAR_COLOR = new Color(0x4D3DFF);

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
