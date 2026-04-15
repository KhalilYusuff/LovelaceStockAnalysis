using System;
namespace Lovelace.StockAnalysis.Core
{
    /// <summary>
    /// Thrown when stock data is not in the correct chronological order.
    /// </summary>
    public class DataOrderException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataOrderException"/> with a detail message.
        /// </summary>
        public DataOrderException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of <see cref="DataOrderException"/> with a detail message
        /// and the index where the ordering violation was found.
        /// </summary>
        public DataOrderException(int index, DateTimeOffset previous, DateTimeOffset current)
            : base($"Data is not in chronological order at index {index}. " +
                   $"Expected {current} to be after {previous}.") { }

        /// <summary>
        /// Initializes a new instance of <see cref="DataOrderException"/> with a message
        /// and an inner exception.
        /// </summary>
        public DataOrderException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}