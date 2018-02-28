using System;
using System.Linq;
using System.Reflection;

namespace FFA.Utility
{
    public static class EventLoader
    {
        public static void Load(IServiceProvider provider)
        {
            var serviceTypes = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsClass && !x.IsNested && x.Namespace == "FFA.Events");

            foreach (var type in serviceTypes)
            {
                var ctor = type.GetConstructor(new[] { typeof(IServiceProvider) });
                ctor.Invoke(new[] { provider });
            }
        }
    }
}
