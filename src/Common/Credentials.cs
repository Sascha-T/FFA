using System.Collections.Generic;

namespace FFA.Common
{
    public class Credentials
    {
        public string Token { get; set; }
        public string DbConnectionString { get; set; }
        public IReadOnlyList<ulong> OwnerIds { get; set; }
    }
}
