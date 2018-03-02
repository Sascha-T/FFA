using FFA.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FFA.Entities.CustomCmd
{
    public sealed class CmdResponse
    {
        public string Value { get; }

        public CmdResponse(IServiceProvider provider, string response)
        {
            var customCmdService = provider.GetRequiredService<CustomCmdService>();

            Value = customCmdService.SterilizeResponse(response);
        }

        public override string ToString()
            => Value;
    }
}
