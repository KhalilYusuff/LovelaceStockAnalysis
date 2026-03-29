using Lovelace.StockAnalysis.Core.Validation;
using Lovelace.StockAnalysis.Models;
using Lovelace.StockAnalysis.Core.Exceptions;  // I added this line  

namespace Lovelace.StockAnalysis.Indicators;

/// <summary>
/// Represents a calculation for the return of stock data points over time.
/// </summary>
/// <remarks>
/// The return is calculated as the relative change in value between consecutive stock data points.
/// The calculation assumes that the provided stock data points are in chronological order.
/// </remarks>
public sealed class ReturnCalculation : IIndicator
{
    private readonly Func<StockDataPoint, decimal> _selector;

    public ReturnCalculation(Func<StockDataPoint, decimal> selector)
    {
        InputValidation.ValidateSelector(selector);
        _selector = selector;
    }

    /// <summary>
    /// Calculates the return values for a given set of stock data points.
    /// </summary>
    /// <param name="data">A collection of stock data points used for return calculation.</param>
    /// <returns>A read-only list of <see cref="IndicatorResult"/> containing the calculated return values for each stock data point, except the first one.</returns>
    /// <exception cref="DivideByZeroException">Thrown when a stock data point's previous value is zero, resulting in a division by zero.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="data"/> is empty.</exception>
    /// <exception cref="DataOrderException">Thrown if <paramref name="data"/> is not sorted
    /// in chronological order by timestamp.</exception>
    public IReadOnlyList<IndicatorResult> Calculate(IReadOnlyList<StockDataPoint> data)
    {
        InputValidation.ValidateData(data);

        var results = new List<IndicatorResult>(data.Count - 1);

        for (int i = 1; i < data.Count; i++)
        {
            decimal previousValue = _selector(data[i - 1]);
            decimal currentValue = _selector(data[i]);

            if (previousValue == 0)
            {
                throw new DivideByZeroException();
            }

            decimal returnValue =
                (currentValue - previousValue) / previousValue;

            results.Add(
                new IndicatorResult(data[i].Timestamp, returnValue));
        }

        return results.AsReadOnly();
    }
}