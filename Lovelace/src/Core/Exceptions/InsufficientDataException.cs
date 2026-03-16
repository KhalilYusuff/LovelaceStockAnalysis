using System;

namespace Lovelace.src.StockAnalysis.Core.Exceptions
{
    /// <summary>
    /// Thrown when there is not enough stock data to perform a calculation.
    /// </summary>
    public class InsufficientDataException : Exception
    {
        public InsufficientDataException(string message) : base(message)
        {
        }
    }
}
