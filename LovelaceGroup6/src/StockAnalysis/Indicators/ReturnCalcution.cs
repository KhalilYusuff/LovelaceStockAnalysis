using LovelaceGroup6.src.StockAnalysis.Core.Validation;
using LovelaceGroup6.StockAnalysis.Interfaces;
using LovelaceGroup6.StockAnalysis.Models;
using System;
using System.Collections.Generic;

namespace LovelaceGroup6.src.StockAnalysis.Indicators
{
    public sealed class ReturnCalculation : IIndicator
    {
        public IReadOnlyList<IndicatorResult> Calculate(IReadOnlyList<StockDataPoint> data)
        {
            InputValidation.ValidateData(data);

            var results = new List<IndicatorResult>(data.Count - 1);

            for (int i = 1; i < data.Count; i++)
            {
                decimal previousClose = data[i - 1].Close;
                decimal currentClose = data[i].Close;
                
                if (previousClose == 0)
                {
                    throw new DivideByZeroException();
                }

                decimal returnValue =
                    (currentClose - previousClose) / previousClose;

                results.Add(
                    new IndicatorResult(data[i].Timestamp, returnValue));
            }

            return results.AsReadOnly();
        }
    }
}