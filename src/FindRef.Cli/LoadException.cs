using System;

namespace FindRef.Cli
{
    public class LoadException : AggregateException
    {
        public LoadException(Exception innerException) : base(innerException)
        {
            
        }
    }
}