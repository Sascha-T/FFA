using System.Collections.Generic;

namespace FFA.Common
{
    internal class Credentials
    {
        public string Token { get; set; }
        public IReadOnlyList<ulong> OwnerIds { get; set; }
    }
}
