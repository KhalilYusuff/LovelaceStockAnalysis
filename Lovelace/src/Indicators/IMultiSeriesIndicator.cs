using StockAnalysis.Models;

namespace StockAnalysis.Indicators;

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
    IReadOnlyList<IndicatorResult> Calculate(IReadOnlyList<IReadOnlyList<StockDataPoint>> series);
    
}