using Discord.Commands;
using FFA.Common;
using FFA.Database;
using FFA.Entities.Event;
using FFA.Entities.FFATimer;
using FFA.Entities.Service;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Linq;

namespace FFA.Utility
{
    public static class Loader
    {
        public static void LoadServices(IServiceCollection services)
        {
            var serviceTypes = Constants.ASSEMBLY_CLASSES.Where(x => x.BaseType == typeof(Service));

            foreach (var type in serviceTypes)
                services.AddSingleton(type);
        }

        public static void LoadCollections(IServiceCollection services, IMongoDatabase db)
        {
            var dbModelTypes = Constants.ASSEMBLY_CLASSES.Where(x => x.BaseType == typeof(Entity));
            var method = db.GetType().GetMethod("GetCollection");

            foreach (var model in dbModelTypes)
            {
                var generic = method.MakeGenericMethod(new Type[] { model });
                var collection = generic.Invoke(db, new object[] { model.Name.ToLower() + 's', null });
                var type = typeof(IMongoCollection<>).MakeGenericType(model);

                services.AddSingleton(type, collection);
            }
        }

        public static void LoadEvents(IServiceProvider provider)
            => ProviderLoad(provider, typeof(Event));

        public static void LoadTimers(IServiceProvider provider)
            => ProviderLoad(provider, typeof(FFATimer));

        public static void LoadReaders(CommandService commands)
        {
            var readerTypes = Constants.ASSEMBLY_CLASSES.Where(x => x.BaseType == typeof(TypeReader));
            var method = commands.GetType().GetMethod("AddTypeReader", new Type[] { typeof(Type), typeof(TypeReader) });

            foreach (var readerType in readerTypes)
            {
                var ctor = readerType.GetConstructor(new Type[] { });
                var reader = ctor.Invoke(null);
                var type = reader.GetType().GetProperty("Type").GetValue(reader);

                method.Invoke(commands, new object[] { type, ctor.Invoke(null) });
            }
        }

        public static void ProviderLoad(IServiceProvider provider, Type baseType)
        {
            var serviceTypes = Constants.ASSEMBLY_CLASSES.Where(x => x.BaseType == baseType);

            foreach (var service in serviceTypes)
            {
                var ctor = service.GetConstructor(new[] { typeof(IServiceProvider) });

                ctor.Invoke(new[] { provider });
            }
        }
    }
}
