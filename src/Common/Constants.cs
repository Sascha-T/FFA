using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace FFA.Common
{
    public static class Constants
    {
        // Discord response codes
        public const ushort TOO_MANY_REQUESTS = 429;

        // Assembly classes
        public static readonly IReadOnlyList<Type> ASSEMBLY_CLASSES =
            Assembly.GetEntryAssembly().GetTypes().Where(x => x.IsClass && !x.IsNested).ToImmutableArray();
    }
}
