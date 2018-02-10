using System;

namespace FFA.Extensions
{
    public static class ExceptionExtensions
    {
        public static Exception Last(this Exception err)
        {
            var next = err;

            while (next.InnerException != null)
            {
                next = next.InnerException;
            }

            return next;
        }

        public static string LastMessage(this Exception err)
            => err.Last().Message;
    }
}
