using System;

namespace FFA.Extensions
{
    public static class ExceptionExtension
    {
        public static string LastMessage(this Exception err)
        {
            return err.Last().Message;
        }

        public static Exception Last(this Exception err)
        {
            var next = err;

            while (next.InnerException != null)
            {
                next = next.InnerException;
            }

            return next;
        }
    }
}
