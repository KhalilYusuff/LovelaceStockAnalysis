namespace Lovelace.StockAnalysis.Indicators;

/// <summary>
/// Represents an interface for calculating stock analysis indicators based on stock price data.
/// </summary>
/// <remarks>
/// This interface defines methods for performing calculations on a collection of stock price data
/// to generate indicator values. Implementations may provide various types of technical indicators,
/// such as moving averages, relative strength index (RSI), or others.
/// </remarks>
public interface IIndicator
{
    /// <summary>
    /// Calculates indicator results for the specified sequence of stock data points.
    /// </summary>
    /// <param name="data">A read-only list of stock data points to analyze. Cannot be null or empty.</param>
    /// <returns>A read-only list of indicator results corresponding to the input data. The list will be empty if no results are
    /// calculated.</returns>
    IReadOnlyList<IndicatorResult> Calculate(IReadOnlyList<StockDataPoint> data);
}