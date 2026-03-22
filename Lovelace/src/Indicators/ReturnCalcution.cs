using StockAnalysis.Core.Validation;
using StockAnalysis.Indicators;
using StockAnalysis.Models;
using System;
using System.Collections.Generic;

namespace StockAnalysis.Indicators;

    public sealed class ReturnCalculation : IIndicator
    {
        // this Selector will decides which value to use (Close, Open, High, etc.)
        private readonly Func<StockDataPoint, decimal> _selector;

        // Constructor
        public ReturnCalculation(Func<StockDataPoint, decimal> selector)
        {
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));
        }

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
