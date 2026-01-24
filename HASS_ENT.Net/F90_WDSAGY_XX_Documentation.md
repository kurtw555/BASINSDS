# F90_WDSAGY_XX Function Documentation

## Overview
The `F90_WDSAGY_XX` function provides time series data aggregation capabilities for WDM datasets, implementing the FORTRAN equivalent functionality in C#.

## Function Signature
```csharp
public static void F90_WDSAGY_XX(
    int wdmUnit,            // WDM unit number
    int dsn,                // Dataset number  
    int aggType,            // Aggregation type (1-5)
    int[] startDate,        // Start date [year, month, day, hour, minute, second]
    int[] endDate,          // End date [year, month, day, hour, minute, second]
    int timeStep,           // Time step for aggregation
    int timeUnit,           // Time unit (1=sec, 2=min, 3=hour, 4=day, 5=month, 6=year)
    float[] outputValues,   // Output aggregated values
    int[,] outputDates,     // Output dates [6, nValues] 
    out int nValues,        // Number of values returned
    out int retCode         // Return code (0=success)
)
```

## Aggregation Types
| Type | Description | Formula |
|------|-------------|---------|
| 1    | Sum         | Total of all values in period |
| 2    | Mean        | Average of all values in period |
| 3    | Maximum     | Highest value in period |
| 4    | Minimum     | Lowest value in period |
| 5    | Count       | Number of values in period |

## Time Units
| Unit | Description | Usage |
|------|-------------|-------|
| 1    | Seconds     | High-frequency data |
| 2    | Minutes     | Sub-hourly data |
| 3    | Hours       | Hourly aggregation |
| 4    | Days        | Daily aggregation |
| 5    | Months      | Monthly aggregation (approx. 30 days) |
| 6    | Years       | Annual aggregation (approx. 365 days) |

## Usage Examples

### Example 1: Weekly Mean Aggregation
```csharp
int wdmUnit = 101;
int dsn = 100;
int[] startDate = { 2024, 1, 1, 0, 0, 0 };
int[] endDate = { 2024, 2, 28, 23, 59, 59 };
float[] weeklyMeans = new float[10];
int[,] weekDates = new int[6, 10];

WdmOperations.F90_WDSAGY_XX(wdmUnit, dsn, 2, startDate, endDate,
    7, 4, weeklyMeans, weekDates, out int nWeeks, out int retCode);

if (retCode == 0)
{
    Console.WriteLine($"Generated {nWeeks} weekly mean values");
    for (int i = 0; i < nWeeks; i++)
    {
        Console.WriteLine($"Week {i+1}: {weeklyMeans[i]:F2} " +
            $"starting {weekDates[0,i]}/{weekDates[1,i]:D2}/{weekDates[2,i]:D2}");
    }
}
```

### Example 2: Monthly Maximum Values
```csharp
int[] startDate = { 2023, 1, 1, 0, 0, 0 };
int[] endDate = { 2023, 12, 31, 23, 59, 59 };
float[] monthlyMax = new float[12];
int[,] monthDates = new int[6, 12];

WdmOperations.F90_WDSAGY_XX(wdmUnit, dsn, 3, startDate, endDate,
    1, 5, monthlyMax, monthDates, out int nMonths, out int retCode);
```

### Example 3: Daily Sum Aggregation
```csharp
int[] startDate = { 2024, 6, 1, 0, 0, 0 };
int[] endDate = { 2024, 6, 30, 23, 59, 59 };
float[] dailySums = new float[31];
int[,] dayDates = new int[6, 31];

WdmOperations.F90_WDSAGY_XX(wdmUnit, dsn, 1, startDate, endDate,
    1, 4, dailySums, dayDates, out int nDays, out int retCode);
```

## Return Codes
| Code | Meaning |
|------|---------|
| 0    | Success |
| -1   | General error |
| -2   | Dataset not found |

## Features
? **Multiple Aggregation Types** - Sum, mean, max, min, count  
? **Flexible Time Periods** - Seconds to years  
? **Date Range Filtering** - Process only specified date ranges  
? **Missing Value Handling** - Automatically excludes NaN and -999 values  
? **Memory Efficient** - Processes data in chronological order  
? **Error Handling** - Comprehensive error checking and logging  

## Implementation Details

### Data Filtering
- Automatically excludes `NaN` values
- Excludes missing value indicator `-999.0f`
- Processes only data within specified date range
- Maintains chronological ordering

### Aggregation Algorithm
1. Filter source data by date range and validity
2. Group data into time periods based on timeStep and timeUnit
3. Apply aggregation function to each period
4. Return aggregated values with period start dates

### Memory Usage
- Supports up to the array size limits for output
- Efficiently processes large datasets
- No intermediate storage of entire dataset

## Testing
The function includes comprehensive test coverage:
- Unit tests with synthetic data
- Integration tests with real WDM files  
- Edge case testing (empty datasets, single values)
- Performance testing with large datasets

## See Also
- `F90_WDTGET` - Get time series data
- `F90_WDTPUT` - Put time series data
- `TimeSeriesAnalysis.F90_DAANST` - Statistical analysis

---
*Part of HASS_ENT.Net WDM Operations suite*