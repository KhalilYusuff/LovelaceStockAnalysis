namespace Lovelace.StockAnalysis.Indicators;

/// <summary>
/// Defines a contract for calculating technical analysis indicators that operate on multiple series of stock data points.
/// </summary>
/// <remarks>
/// Implementations of this interface are designed to process one or more series of stock data points and produce
/// indicator results that can be used for financial analysis. Each series represents a collection of stock data
/// (e.g., open, high, low, close, volume) over a period of time.
/// </remarks>
public interface IMultiSeriesIndicator
{
    /// <summary>
    /// Calculates indicator results for each series of stock data points.
    /// </summary>
    /// <param name="series">A collection of stock data series, where each inner list represents a sequence of data points for a single stock
    /// or instrument. Cannot be null or contain null elements.</param>
    /// <returns>A read-only list of indicator results, with one result for each input series. The list will be empty if no
    /// series are provided.</returns>
    IReadOnlyList<IndicatorResult> Calculate(IReadOnlyList<IReadOnlyList<StockDataPoint>> series);
    
}