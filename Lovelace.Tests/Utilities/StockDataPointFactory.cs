using StockAnalysis.Models;

namespace StockAnalysis.Tests.Utilities;

/// <summary>
/// Provides default methods for creating <see cref="StockDataPoint"/> instances in tests.
/// </summary>
public static class StockDataPointFactory
{
    /// <summary>
    /// Creates a <see cref="StockDataPoint"/> with default values for fields not specified.
    /// </summary>
    
    public static StockDataPoint Create(
        decimal open = 100m,
        decimal high = 110m,
        decimal low = 90m,
        decimal close = 105m,
        decimal volume = 1000m,
        DateTime? timestamp = null)
    {
        return new StockDataPoint(
            timestamp: timestamp ?? DateTime.Today,
            open: open,
            high: high,
            low: low,
            close: close,
            volume: volume);
    }
}
