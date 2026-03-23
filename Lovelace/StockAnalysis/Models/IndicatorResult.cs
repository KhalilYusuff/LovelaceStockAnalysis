namespace Lovelace.StockAnalysis.Models;

/// <summary>
/// Represents the result of a technical analysis indicator calculation for a specific point in time.
/// </summary>
/// <remarks>
/// This class is commonly used to encapsulate the value of a computed indicator
/// along with the associated timestamp, which allows for precise tracking of historical data.
/// It is typically used in conjunction with implementations of the <c>IIndicator</c> interface.
/// </remarks>
public sealed class IndicatorResult
{
    /// <summary>
    /// Gets the timestamp indicating the date and time associated with the indicator result.
    /// </summary>
    /// <remarks>
    /// This property represents the specific moment when the indicator value was recorded.
    /// </remarks>
    public DateTime Timestamp { get;}

    /// <summary>
    /// Gets the numerical value calculated by the indicator.
    /// </summary>
    /// <remarks>
    /// This property represents the result produced by a specific stock analysis indicator,
    /// providing a quantifiable metric based on the input data.
    /// </remarks>
    public decimal Value { get; }

    public IndicatorResult(DateTime timestamp, decimal value)
    {
        Timestamp = timestamp;
        Value = value; 
        
    }
}