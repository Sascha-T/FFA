using System;
using System.Threading.Tasks;
using Discord.Commands;
using FFA.Database.Models;
using FFA.Extensions.Database;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace FFA.Preconditions.Parameter
{
    public sealed class UniqueCustomCommandAttribute : ParameterPreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value,
            IServiceProvider services)
        {
            var customCommandCollection = services.GetRequiredService<IMongoCollection<CustomCommand>>();
            var strValue = (value as string)?.ToLower();

            if (strValue != null && await customCommandCollection.AnyAsync(x => x.Name == strValue))
            {
                return PreconditionResult.FromError("This command already exists.");
            }

            return PreconditionResult.FromSuccess();
        }
    }
}
