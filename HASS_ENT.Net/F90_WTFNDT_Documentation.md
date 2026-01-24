# F90_WTFNDT Function Documentation

## Overview
The `F90_WTFNDT` function provides powerful time series data finding capabilities for WDM datasets, implementing multiple search modes to locate data points based on temporal criteria. This function is essential for time series analysis and data extraction operations.

## Function Signature
```csharp
public static void F90_WTFNDT(
    int wdmUnit,            // WDM unit number
    int dsn,                // Dataset number
    int searchMode,         // Search mode (1-7)
    int[] targetDate,       // Target date [year, month, day, hour, minute, second]
    int[] startDate,        // Start date for range search (optional)
    int[] endDate,          // End date for range search (optional)
    int[] foundDate,        // Found date [year, month, day, hour, minute, second]
    out float foundValue,   // Found data value
    out int foundQuality,   // Found data quality flag
    out int dataIndex,      // Index of found data point in dataset
    out int retCode         // Return code
)
```

## Search Modes
| Mode | Description | Usage | Example |
|------|-------------|-------|---------|
| 1    | First       | Find first data point in range | Data period start |
| 2    | Last        | Find last data point in range | Data period end |
| 3    | Nearest     | Find nearest data point to target | Closest available data |
| 4    | Next        | Find next data point after target | Forward lookup |
| 5    | Previous    | Find previous data point before target | Backward lookup |
| 6    | Exact       | Find exact match at target time | Precise time match |
| 7    | Interpolated| Calculate interpolated value at target | Estimated value |

## Return Codes
| Code | Meaning |
|------|---------|
| 0    | Success - data found |
| -1   | General error |
| -2   | Dataset not found |
| -3   | No data found matching criteria |

## Date Format
All date arrays use the format: `[year, month, day, hour, minute, second]`

Example: June 15, 2023 at 12:30:45 ? `[2023, 6, 15, 12, 30, 45]`

## Usage Examples

### Example 1: Find First Data Point
```csharp
int wdmUnit = 101;
int dsn = 100;
int[] targetDate = { 2023, 1, 1, 0, 0, 0 };
int[] foundDate = new int[6];

WdmOperations.F90_WTFNDT(wdmUnit, dsn, 1, targetDate, null, null,
    foundDate, out float foundValue, out int foundQuality, out int dataIndex, out int retCode);

if (retCode == 0)
{
    var firstDateTime = new DateTime(foundDate[0], foundDate[1], foundDate[2], 
                                   foundDate[3], foundDate[4], foundDate[5]);
    Console.WriteLine($"First data point: {firstDateTime:yyyy-MM-dd HH:mm:ss}, Value: {foundValue:F2}");
}
```

### Example 2: Find Nearest Data Point
```csharp
// Find data point closest to a specific time
int[] targetDate = { 2023, 6, 15, 12, 30, 0 }; // June 15, 2023 at 12:30
int[] foundDate = new int[6];

WdmOperations.F90_WTFNDT(wdmUnit, dsn, 3, targetDate, null, null,
    foundDate, out float foundValue, out int foundQuality, out int dataIndex, out int retCode);

if (retCode == 0)
{
    var nearestDateTime = new DateTime(foundDate[0], foundDate[1], foundDate[2], 
                                     foundDate[3], foundDate[4], foundDate[5]);
    Console.WriteLine($"Nearest data: {nearestDateTime:yyyy-MM-dd HH:mm:ss}, Value: {foundValue:F2}");
    Console.WriteLine($"Data index: {dataIndex}, Quality: {foundQuality}");
}
```

### Example 3: Get Interpolated Value
```csharp
// Get interpolated value at a specific time between data points
int[] targetDate = { 2023, 6, 15, 12, 30, 0 };
int[] foundDate = new int[6];

WdmOperations.F90_WTFNDT(wdmUnit, dsn, 7, targetDate, null, null,
    foundDate, out float foundValue, out int foundQuality, out int dataIndex, out int retCode);

if (retCode == 0)
{
    Console.WriteLine($"Interpolated value at {targetDate[0]}-{targetDate[1]:D2}-{targetDate[2]:D2} " +
                     $"{targetDate[3]:D2}:{targetDate[4]:D2}:{targetDate[5]:D2}: {foundValue:F2}");
    Console.WriteLine($"Quality based on surrounding points: {foundQuality}");
}
```

### Example 4: Find Data in Date Range
```csharp
// Find data within a specific date range
int[] targetDate = { 2023, 6, 15, 0, 0, 0 };
int[] startDate = { 2023, 6, 1, 0, 0, 0 };   // Start of June
int[] endDate = { 2023, 6, 30, 23, 59, 59 }; // End of June
int[] foundDate = new int[6];

// Find nearest data within the June range
WdmOperations.F90_WTFNDT(wdmUnit, dsn, 3, targetDate, startDate, endDate,
    foundDate, out float foundValue, out int foundQuality, out int dataIndex, out int retCode);

if (retCode == 0)
{
    Console.WriteLine($"Found data in June range: Value = {foundValue:F2}");
}
else if (retCode == -3)
{
    Console.WriteLine("No data found in the specified June range");
}
```

### Example 5: Time Series Navigation
```csharp
// Navigate through time series data
void NavigateTimeSeries(int wdmUnit, int dsn, DateTime currentTime)
{
    int[] currentDate = { currentTime.Year, currentTime.Month, currentTime.Day,
                         currentTime.Hour, currentTime.Minute, currentTime.Second };
    int[] foundDate = new int[6];
    
    // Find next data point
    WdmOperations.F90_WTFNDT(wdmUnit, dsn, 4, currentDate, null, null,
        foundDate, out float nextValue, out int nextQuality, out int nextIndex, out int retCode);
    
    if (retCode == 0)
    {
        var nextDateTime = new DateTime(foundDate[0], foundDate[1], foundDate[2], 
                                      foundDate[3], foundDate[4], foundDate[5]);
        Console.WriteLine($"Next data point: {nextDateTime:yyyy-MM-dd HH:mm:ss}, Value: {nextValue:F2}");
    }
    
    // Find previous data point
    WdmOperations.F90_WTFNDT(wdmUnit, dsn, 5, currentDate, null, null,
        foundDate, out float prevValue, out int prevQuality, out int prevIndex, out int retCode2);
    
    if (retCode2 == 0)
    {
        var prevDateTime = new DateTime(foundDate[0], foundDate[1], foundDate[2], 
                                      foundDate[3], foundDate[4], foundDate[5]);
        Console.WriteLine($"Previous data point: {prevDateTime:yyyy-MM-dd HH:mm:ss}, Value: {prevValue:F2}");
    }
}
```

### Example 6: Data Quality Analysis
```csharp
// Analyze data quality around a specific time
void AnalyzeDataQuality(int wdmUnit, int dsn, DateTime analysisTime)
{
    int[] targetDate = { analysisTime.Year, analysisTime.Month, analysisTime.Day,
                        analysisTime.Hour, analysisTime.Minute, analysisTime.Second };
    int[] foundDate = new int[6];
    
    // Check for exact data at target time
    WdmOperations.F90_WTFNDT(wdmUnit, dsn, 6, targetDate, null, null,
        foundDate, out float exactValue, out int exactQuality, out int exactIndex, out int retCode);
    
    if (retCode == 0)
    {
        Console.WriteLine($"Exact data found: Value = {exactValue:F2}, Quality = {exactQuality}");
    }
    else
    {
        // Get interpolated value and compare with nearest actual values
        WdmOperations.F90_WTFNDT(wdmUnit, dsn, 7, targetDate, null, null,
            foundDate, out float interpValue, out int interpQuality, out int interpIndex, out int retCode2);
        
        WdmOperations.F90_WTFNDT(wdmUnit, dsn, 3, targetDate, null, null,
            foundDate, out float nearestValue, out int nearestQuality, out int nearestIndex, out int retCode3);
        
        if (retCode2 == 0 && retCode3 == 0)
        {
            Console.WriteLine($"Interpolated value: {interpValue:F2}");
            Console.WriteLine($"Nearest actual value: {nearestValue:F2}");
            Console.WriteLine($"Difference: {Math.Abs(interpValue - nearestValue):F2}");
        }
    }
}
```

## Advanced Features

### Interpolation Algorithm
The interpolation mode (7) uses linear interpolation between surrounding data points:
```
interpolatedValue = beforeValue + ((afterValue - beforeValue) * timeRatio)
```
Where `timeRatio` is the proportional time distance between the before and after points.

### Quality Flag Handling
- **Exact matches**: Use original quality flag
- **Interpolated values**: Use the worst (minimum) quality of surrounding points
- **Missing data**: Quality flag set to -999

### Data Index Information
The `dataIndex` parameter provides the position of the found data point in the original dataset:
- **Valid for modes 1-6**: Actual index in dataset
- **Mode 7 (interpolated)**: Returns -1 (no actual data point)
- **No data found**: Returns -1

### Date Range Filtering
When `startDate` and/or `endDate` are provided:
- Only data within the specified range is considered
- Useful for limiting search scope
- Improves performance for large datasets

### Error Handling
```csharp
// Comprehensive error handling example
void SafeDataFind(int wdmUnit, int dsn, int searchMode, int[] targetDate)
{
    try
    {
        int[] foundDate = new int[6];
        WdmOperations.F90_WTFNDT(wdmUnit, dsn, searchMode, targetDate, null, null,
            foundDate, out float foundValue, out int foundQuality, out int dataIndex, out int retCode);
        
        switch (retCode)
        {
            case 0:
                var foundDateTime = new DateTime(foundDate[0], foundDate[1], foundDate[2], 
                                               foundDate[3], foundDate[4], foundDate[5]);
                Console.WriteLine($"Success: Found data at {foundDateTime:yyyy-MM-dd HH:mm:ss}");
                break;
                
            case -2:
                Console.WriteLine("Error: Dataset not found");
                break;
                
            case -3:
                Console.WriteLine("Warning: No data found matching search criteria");
                break;
                
            default:
                Console.WriteLine($"Error: Unexpected return code {retCode}");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception during data search: {ex.Message}");
    }
}
```

## Performance Considerations
- **Large datasets**: Use date range filtering to improve performance
- **Frequent searches**: Consider caching results for repeated queries
- **Interpolation**: More computationally intensive than direct lookups
- **Memory usage**: Function operates on in-memory data structures

## Integration with Other Functions
F90_WTFNDT works seamlessly with other WDM functions:

```csharp
// Combined workflow example
void CompleteDataAnalysis(int wdmUnit, int dsn)
{
    // Find data period
    int[] startSearch = { 1900, 1, 1, 0, 0, 0 };
    int[] foundStart = new int[6];
    int[] foundEnd = new int[6];
    
    WdmOperations.F90_WTFNDT(wdmUnit, dsn, 1, startSearch, null, null,
        foundStart, out float firstValue, out int firstQuality, out int firstIndex, out int retCode1);
    
    WdmOperations.F90_WTFNDT(wdmUnit, dsn, 2, startSearch, null, null,
        foundEnd, out float lastValue, out int lastQuality, out int lastIndex, out int retCode2);
    
    if (retCode1 == 0 && retCode2 == 0)
    {
        Console.WriteLine($"Data period: {foundStart[0]}-{foundStart[1]:D2}-{foundStart[2]:D2} to " +
                         $"{foundEnd[0]}-{foundEnd[1]:D2}-{foundEnd[2]:D2}");
        
        // Get aggregated data for the period
        float[] aggValues = new float[12];
        int[,] aggDates = new int[6, 12];
        
        WdmOperations.F90_WDSAGY_XX(wdmUnit, dsn, 2, foundStart, foundEnd,
            1, 5, aggValues, aggDates, out int nValues, out int aggRetCode);
        
        if (aggRetCode == 0)
        {
            Console.WriteLine($"Generated {nValues} monthly averages");
        }
    }
}
```

## Common Use Cases

### 1. Data Gap Analysis
Find missing data periods by searching for exact matches at expected times.

### 2. Quality Control
Locate data points with specific quality flags for validation.

### 3. Time Series Alignment
Align multiple datasets by finding corresponding time points.

### 4. Interpolation Studies
Compare interpolated values with actual measurements.

### 5. Report Generation
Extract data at specific report times (end of month, year, etc.).

### 6. Real-time Applications
Find the most recent data point for current conditions.

## See Also
- `F90_WDTGET` - Get time series data in bulk
- `F90_WDSAGY_XX` - Aggregate time series data
- `F90_WDLBAX` - Get dataset label information
- `F90_GETATT` - Get dataset attributes

---
*Part of HASS_ENT.Net WDM Operations suite*