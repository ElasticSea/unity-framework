using System;
using System.Collections.Generic;

namespace _Framework.Core.Scripts.Extensions
{
    public static class ExceptionExtensions
    {
        public static IEnumerable<string> GetAllMessages(this Exception exception)
        {
            while (exception != null)
            {
                yield return exception.Message;
                exception = exception.InnerException;
            }
        }
    }
}