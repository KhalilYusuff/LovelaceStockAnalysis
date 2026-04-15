namespace Lovelace.StockAnalysis.Indicators;

/// <summary>
/// Defines a contract for calculating technical analysis indicators that produce multiple named output series
/// from a single collection of stock data points.
/// </summary>
/// <remarks>
/// Implementations of this interface are designed to process a single series of stock data points and produce
/// multiple named indicator result series. Each output series is identified by a string key defined as a
/// constant on the implementing class. This interface is intended for indicators such as MACD or Bollinger Bands
/// that produce more than one output series per calculation.
/// </remarks>
public interface IMultiOutputIndicator
{
    /// <summary>
    /// Calculates multiple named indicator series from the specified stock data.
    /// </summary>
    /// <returns>A read-only dictionary where each key identifies a named output series and the corresponding value
    /// is a read-only list of <see cref="IndicatorResult"/> objects for that series.</returns>

    IReadOnlyDictionary<string, IReadOnlyList<IndicatorResult>> Calculate(IReadOnlyList<StockDataPoint> data);
}
