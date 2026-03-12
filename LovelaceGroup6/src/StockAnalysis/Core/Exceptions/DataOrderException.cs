using System;

namespace LovelaceGroup6.src.StockAnalysis.Core.Exceptions
{
    /// <summary>
    /// Thrown when stock data is not in the correct chronological order.
    /// </summary>
    public class DataOrderException : Exception
    {
        public DataOrderException(string message) : base(message)
        {
        }
    }
}