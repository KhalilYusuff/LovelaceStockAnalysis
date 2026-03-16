namespace StockAnalysis.Models;

/// <summary>
/// Represents a data point for a stock, encapsulating the price and volume
/// information of a stock at a specific timestamp.
/// </summary>
/// <remarks>
/// The StockDataPoint class provides key data for stock analysis, including
/// the opening, high, low, and closing prices, as well as the trading volume
/// for a given timestamp. Instances of this class are immutable and allow
/// precise representation of the state of a stock at a specific point in time.
/// </remarks>
/// <exception cref="ArgumentException">
/// Thrown when the low price exceeds the high price, which is an invalid state.
/// </exception>
public sealed class StockDataPoint
{
    /// <summary>
    /// Gets the date and time associated with this stock data point.
    /// This property represents the timestamp when the stock data was recorded,
    /// providing temporal context for the associated price and volume details.
    /// </summary>
    public DateTime Timestamp { get;}

    /// <summary>
    /// Gets the opening price for the stock at the associated timestamp.
    /// This property indicates the price level at which the stock began trading
    /// during the specified time period, serving as a reference point for the session's price movement.
    /// </summary>
    public decimal Open { get; }

    /// <summary>
    /// Gets the highest price recorded for the stock during the specified time period.
    /// This property reflects the peak value reached, providing insight into the maximum trade price.
    /// </summary>
    public decimal High { get; }

    /// <summary>
    /// Gets the lowest price of the stock during the corresponding time period.
    /// This property indicates the minimum trading value and is used to assess
    /// the daily or interval price range of the stock.
    /// </summary>
    public decimal Low { get; }

    /// <summary>
    /// Gets the closing price of the stock for this data point.
    /// This property represents the final traded price of the stock
    /// at the end of the recorded time interval.
    /// </summary>
    public decimal Close { get; }

    /// <summary>
    /// Gets the total trading volume for the associated stock during a specific time period.
    /// This property indicates the quantity of shares traded, reflecting market activity and liquidity.
    /// </summary>
    public decimal Volume { get; }

    public StockDataPoint( DateTime timestamp, decimal open, decimal high, decimal low, decimal close, decimal volume)
    {
        if (high < low)
        {
            throw new ArgumentException("Low cannot be higher than high");
        }
        Timestamp = timestamp;
        Open = open;
        High = high;
        Low = low;
        Close = close;
        Volume = volume; 
    }
    
}