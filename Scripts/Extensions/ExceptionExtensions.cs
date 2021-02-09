using System;
using System.Collections.Generic;

namespace ElasticSea.Framework.Extensions
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