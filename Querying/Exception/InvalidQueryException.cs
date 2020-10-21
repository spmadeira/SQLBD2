using System;

namespace Querying.Exception
{
    public class InvalidQueryException : System.Exception
    {
        public Type QueryType { get; }

        public InvalidQueryException(Type queryType, string message) : base(message)
        {
            QueryType = queryType;
        }
    }
}