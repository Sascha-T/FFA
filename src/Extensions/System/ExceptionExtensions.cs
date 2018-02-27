using System;

namespace FFA.Extensions.System
{
    internal static class ExceptionExtensions
    {
        internal static Exception Last(this Exception err)
        {
            var next = err;

            while (next.InnerException != null)
                next = next.InnerException;

            return next;
        }

        internal static string LastMessage(this Exception err)
            => err.Last().Message;
    }
}
