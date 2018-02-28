using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace FFA.Utility
{
    public static class ServiceLoader
    {
        public static void Load(IServiceCollection services)
        {
            // TODO: move all non service classes out of services!!!
            var serviceTypes = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsClass && !x.IsNested && x.Namespace == "FFA.Services" &&
                x.Name.EndsWith("Service"));

            foreach (var type in serviceTypes)
                services.AddSingleton(type);
        }
    }
}
