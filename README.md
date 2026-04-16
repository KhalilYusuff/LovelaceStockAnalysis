# Lovelace.StockAnalysis

A .NET library for stock analysis and financial time-series data processing. Provides a structured, consistent way to represent, store, and analyze price and volume data.

---

## Table of Contents

- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Usage Examples](#usage-examples)
- [API Reference](#api-reference)
- [Project Structure](#project-structure)

---

## Features

- Immutable OHLCV data model for representing market data points
- Simple Moving Average (SMA)
- Exponential Moving Average (EMA)
- Rolling Volatility (standard deviation)
- Return Calculation (relative price change)
- Multi-series indicator support for cross-asset analysis
- Built-in input validation

---

## Requirements

- .NET 10 or later

---

## Installation

Install via NuGet:

```bash
dotnet add package Lovelace.StockAnalysis
```

Then import the relevant namespaces:

```csharp
using Lovelace.StockAnalysis;
using Lovelace.StockAnalysis.Indicators;
```

---

## Quick Start

Create a data point and read its values:

```csharp
var dataPoint = new StockDataPoint(
    timestamp: DateTime.UtcNow,
    open:      100.25m,
    high:      104.10m,
    low:       99.80m,
    close:     103.40m,
    volume:    152000m
);

Console.WriteLine($"Timestamp: {dataPoint.Timestamp}");
Console.WriteLine($"Open:      {dataPoint.Open}");
Console.WriteLine($"High:      {dataPoint.High}");
Console.WriteLine($"Low:       {dataPoint.Low}");
Console.WriteLine($"Close:     {dataPoint.Close}");
Console.WriteLine($"Volume:    {dataPoint.Volume}");
```

---

## Usage Examples

### Simple Moving Average (Close Price)

```csharp
var now = DateTime.UtcNow;

var points = new List<StockDataPoint>
{
    new(now.AddDays(-4), 100m,   102m, 99m,  101m,   12000m),
    new(now.AddDays(-3), 101m,   103m, 100m, 102.5m, 15000m),
    new(now.AddDays(-2), 102.5m, 105m, 101m, 104m,   18000m),
    new(now.AddDays(-1), 104m,   106m, 103m, 105m,   20000m),
    new(now,             105m,   108m, 104m, 107m,   25000m)
};

var sma = new SimpleMovingAverage(period: 3, selector: p => p.Close);
IReadOnlyList<IndicatorResult> results = sma.Calculate(points);

foreach (var result in results)
    Console.WriteLine($"{result.Timestamp:yyyy-MM-dd}  SMA: {result.Value:F2}");
```

### Simple Moving Average (Volume)

The `selector` parameter makes indicators reusable across any numeric field on `StockDataPoint`:

```csharp
var volumeSma = new SimpleMovingAverage(period: 2, selector: p => p.Volume);
IReadOnlyList<IndicatorResult> volumeResults = volumeSma.Calculate(points);

foreach (var result in volumeResults)
    Console.WriteLine($"{result.Timestamp:yyyy-MM-dd}  Avg Volume: {result.Value:F0}");
```

---

## API Reference

### `StockDataPoint`

Represents a single OHLCV data point for a financial instrument. Immutable.

| Property    | Type       | Description                        |
|-------------|------------|------------------------------------|
| `Timestamp` | `DateTime` | The point in time this data covers |
| `Open`      | `decimal`  | Opening price for the period       |
| `High`      | `decimal`  | Highest price in the period        |
| `Low`       | `decimal`  | Lowest price in the period         |
| `Close`     | `decimal`  | Closing price for the period       |
| `Volume`    | `decimal`  | Total trading volume for the period|

---

### `IndicatorResult`

Holds the output of an indicator calculation for a single point in time.

| Property    | Type       | Description                            |
|-------------|------------|----------------------------------------|
| `Timestamp` | `DateTime` | The point in time the value applies to |
| `Value`     | `decimal`  | The calculated indicator value         |

---

### `IIndicator`

Base contract for all single-series indicators.

```csharp
public interface IIndicator
{
    IReadOnlyList<IndicatorResult> Calculate(IReadOnlyList<StockDataPoint> data);
}
```

---

### `IMultiSeriesIndicator`

Contract for indicators that operate across multiple data series (e.g. correlation, spread).

```csharp
public interface IMultiSeriesIndicator
{
    IReadOnlyList<IndicatorResult> Calculate(IReadOnlyList<IReadOnlyList<StockDataPoint>> series);
}
```

---

### `SimpleMovingAverage`

Calculates the arithmetic mean over a rolling window. Implements `IIndicator`.

```csharp
public SimpleMovingAverage(int period, Func<StockDataPoint, decimal> selector)
```

| Parameter  | Description                                                          |
|------------|----------------------------------------------------------------------|
| `period`   | Number of data points in the rolling window. Must be greater than 0. |
| `selector` | Function selecting which field of `StockDataPoint` to average.       |

---

### `ExponentialMovingAverage`

Calculates EMA over a rolling window, weighting recent data points more heavily than SMA. Implements `IIndicator`.

```csharp
public ExponentialMovingAverage(int period, Func<StockDataPoint, decimal> selector)
```

| Parameter  | Description                                                          |
|------------|----------------------------------------------------------------------|
| `period`   | Number of data points for the calculation. Must be greater than 0.  |
| `selector` | Function selecting which field of `StockDataPoint` to use.          |

The first value is seeded using SMA; subsequent values apply the EMA multiplier.

---

### `Volatility`

Calculates rolling standard deviation of a selected price value. Reflects price variability within a period — higher values indicate greater market volatility.

```csharp
public Volatility(int period, Func<StockDataPoint, decimal> selector)
```

| Parameter  | Description                                                  |
|------------|--------------------------------------------------------------|
| `period`   | Size of the rolling window. Must be at least 2.              |
| `selector` | Function selecting which field of `StockDataPoint` to use.   |

---

### `ReturnCalculation`

Calculates the relative price change between consecutive data points.

```csharp
public ReturnCalculation(Func<StockDataPoint, decimal> selector)
```

| Parameter  | Description                                                  |
|------------|--------------------------------------------------------------|
| `selector` | Function selecting which field of `StockDataPoint` to use.   |

---

### `InputValidation`

Static helper class that validates inputs before indicator calculations are performed. Called internally by all indicators — no manual usage required in most cases.

| Method                         | Description                                                              |
|--------------------------------|--------------------------------------------------------------------------|
| `ValidateData(data)`           | Throws if data is null, empty, or not in chronological order.            |
| `ValidatePeriod(period, count)`| Throws if period is ≤ 0 or exceeds the number of data points.            |
| `ValidateVolatilityPeriod(p)`  | Throws if period is less than 2.                                         |
| `ValidateSelector(selector)`   | Throws if the selector function is null.                                 |

---

## Project Structure

```
Lovelace library
├── Lovelace.StockAnalysis
│   ├── Lovelace.StockAnalysis.Core          # Domain models (StockDataPoint, IndicatorResult)
│   └── Lovelace.StockAnalysis.Indicators    # Indicator implementations
└── LovelaceStockAnalysis.Tests
    ├── Lovelace.StockAnalysis.Tests.Core
    └── Lovelace.StockAnalysis.Tests.Indicators
```