using System;

namespace Assets.Source.Model.Cycles.BlackJack
{
    public class BJCycleException : Exception
    {
        public Exception Exception { get; private set; }

        public BJCycleException(Exception exception)
        {
            Exception = exception;
        }
    }
}
