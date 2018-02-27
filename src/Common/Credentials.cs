using System.Collections.Generic;

namespace FFA.Common
{
    public sealed class Credentials
    {
        public string Token { get; set; }
        public string DbConnectionString { get; set; }
        public string DbName { get; set; }
        public IReadOnlyList<ulong> OwnerIds { get; set; }
    }
}
