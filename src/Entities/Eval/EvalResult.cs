using System;

namespace FFA.Entities.Eval
{
    public struct EvalResult
    {
        private EvalResult(bool success, string result = null, Exception exception = null)
        {
            Success = success;
            Result = result;
            Exception = exception;
        }

        public static EvalResult FromSuccess(string result)
            => new EvalResult(true, result);

        public static EvalResult FromError(Exception exception)
            => new EvalResult(true, exception: exception);

        public bool Success { get; }
        public string Result { get; }
        public Exception Exception { get; }
    }
}
