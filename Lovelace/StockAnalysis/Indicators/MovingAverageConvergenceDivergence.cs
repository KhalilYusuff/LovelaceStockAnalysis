using Lovelace.StockAnalysis.Core;

namespace Lovelace.StockAnalysis.Indicators
{
    /// <summary>
    /// Calculates the MACD indicator (Moving Average Convergence Divergence).
    /// MACD line = fast EMA - slow EMA, Signal line = EMA of MACD, Histogram = MACD - Signal.
    /// Default settings are fast 12, slow 26, signal 9.
    /// </summary>
    public sealed class MovingAverageConvergenceDivergence : IMultiOutputIndicator
    {
        public const string MacdLineName = "MacdLine";
        public const string SignalLineName = "SignalLine";
        public const string HistogramName = "Histogram";

        private readonly int _fastPeriod;
        private readonly int _slowPeriod;
        private readonly int _signalPeriod;
        private readonly Func<StockDataPoint, decimal> _selector;
        private readonly decimal _multiplier;

        /// <summary>
        /// Creates a new MACD indicator with the given periods and price selector.
        /// </summary>
        public MovingAverageConvergenceDivergence(
            int fastPeriod,
            int slowPeriod,
            int signalPeriod,
            Func<StockDataPoint, decimal> selector)
        {
            if (fastPeriod <= 0 || slowPeriod <= 0 || signalPeriod <= 0)
            {
                throw new ArgumentException("All periods must be greater than zero.");
            }

            if (fastPeriod >= slowPeriod)
            {
                throw new ArgumentException("Fast period must be less than slow period.");
            }

            _fastPeriod = fastPeriod;
            _slowPeriod = slowPeriod;
            _signalPeriod = signalPeriod;
            InputValidation.ValidateSelector(selector);
            _selector = selector;
            _multiplier = 2m / (signalPeriod + 1);
        }

        /// <summary>
        /// Calculates MACD line, signal line and histogram from the given stock data.
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyList<IndicatorResult>> Calculate(IReadOnlyList<StockDataPoint> data)
        {
            InputValidation.ValidateData(data);

            int minimumRequired = _slowPeriod + _signalPeriod - 1;
            if (data.Count < minimumRequired)
            {
                throw new ArgumentException(
                    $"Not enough data points. Need at least {minimumRequired}, but got {data.Count}.");
            }

            var fastEma = new ExponentialMovingAverage(_fastPeriod, _selector);
            var slowEma = new ExponentialMovingAverage(_slowPeriod, _selector);
            var fastResults = fastEma.Calculate(data);
            var slowResults = slowEma.Calculate(data);

            int periodDifference = _slowPeriod - _fastPeriod;
            var macdValues = new List<decimal>();
            var macdTimestamps = new List<DateTime>();

            for (int i = 0; i < slowResults.Count; i++)
            {
                decimal macdValue = fastResults[i + periodDifference].Value - slowResults[i].Value;
                macdValues.Add(macdValue);
                macdTimestamps.Add(slowResults[i].Timestamp);
            }

            decimal currentSignal = 0;
            for (int i = 0; i < _signalPeriod; i++)
            {
                currentSignal += macdValues[i];
            }
            currentSignal = currentSignal / _signalPeriod;

            var macdLine = new List<IndicatorResult>();
            var signalLine = new List<IndicatorResult>();
            var histogram = new List<IndicatorResult>();

            int startIndex = _signalPeriod - 1;
            macdLine.Add(new IndicatorResult(macdTimestamps[startIndex], macdValues[startIndex]));
            signalLine.Add(new IndicatorResult(macdTimestamps[startIndex], currentSignal));
            histogram.Add(new IndicatorResult(macdTimestamps[startIndex], macdValues[startIndex] - currentSignal));

            for (int i = _signalPeriod; i < macdValues.Count; i++)
            {
                currentSignal = macdValues[i] * _multiplier + currentSignal * (1 - _multiplier);

                macdLine.Add(new IndicatorResult(macdTimestamps[i], macdValues[i]));
                signalLine.Add(new IndicatorResult(macdTimestamps[i], currentSignal));
                histogram.Add(new IndicatorResult(macdTimestamps[i], macdValues[i] - currentSignal));
            }

            var result = new Dictionary<string, IReadOnlyList<IndicatorResult>>
{
                { MacdLineName, macdLine },
                { SignalLineName, signalLine },
                { HistogramName, histogram }
};
            return result.AsReadOnly();
        }
    }
}

