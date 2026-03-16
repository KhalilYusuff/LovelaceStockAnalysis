using System;

namespace Lovelace.src.StockAnalysis.Core.Exceptions
{
    /// <summary>
    /// Thrown when an invalid period is provided for stock analysis calculations.
    /// </summary>
    public class InvalidPeriodException : Exception
    {
        public InvalidPeriodException(string message) : base(message)
        {
            
        }
    }
}
