using LovelaceGroup6.StockAnalysis.Models;

namespace LovelaceGroup6.StockAnalysis.Interfaces;

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
    IReadOnlyList<decimal> Calculate(IReadOnlyList<StockDataPoint> data);
}