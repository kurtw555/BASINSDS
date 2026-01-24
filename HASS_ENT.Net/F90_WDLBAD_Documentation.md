# F90_WDLBAD Function Documentation

## Overview
The `F90_WDLBAD` function provides the ability to add or set WDM dataset label information with intelligent parsing and automatic attribute mapping. This function complements `F90_WDLBAX` by allowing users to set descriptive labels that are automatically mapped to appropriate WDM attributes.

## Function Signature
```csharp
public static void F90_WDLBAD(
    int wdmUnit,            // WDM unit number
    int dsn,                // Dataset number
    int labelType,          // Label type (1-6+)
    int axisIndex,          // Axis index for multi-dimensional data
    int labelLength,        // Length of label text
    int[] labelBuffer,      // Input label buffer as integer array (FORTRAN character array)
    out int retCode         // Return code
)
```

## Label Types
| Type | Description | Target Attributes | Intelligent Parsing |
|------|-------------|-------------------|-------------------|
| 1    | Station     | STAID(2), LOCATION(290) | Station identifier formatting |
| 2    | Parameter   | CONSTITUENT(289), TSTYPE(1) | Parameter type recognition |
| 3    | Time        | TSTEP(17), TCODE(27) | Time step extraction |
| 4    | Units       | TUNIT(33), Units Text(84) | Units code mapping |
| 5    | Scenario    | SCENARIO(288) | Direct text storage |
| 6    | Description | Description(85) | Direct text storage |

## Return Codes
| Code | Meaning |
|------|---------|
| 0    | Success - label set successfully |
| -1   | General error |
| -2   | WDM unit not found |
| -3   | Invalid label type |

## Usage Examples

### Example 1: Set Station Information
```csharp
int wdmUnit = 101;
int dsn = 100;
string stationId = "USGS_01234567";

// Convert to FORTRAN character array format
int[] labelBuffer = new int[stationId.Length];
for (int i = 0; i < stationId.Length; i++)
{
    labelBuffer[i] = (int)stationId[i];
}

WdmOperations.F90_WDLBAD(wdmUnit, dsn, 1, 0, stationId.Length, labelBuffer, out int retCode);

if (retCode == 0)
{
    Console.WriteLine($"Station ID set successfully: {stationId}");
    // This automatically sets:
    // - Attribute 2 (STAID): "USGS_01234567"
    // - Attribute 290 (LOCATION): "USGS_01234567" (if > 8 chars)
}
```

### Example 2: Set Parameter with Automatic Type Recognition
```csharp
string parameter = "STREAMFLOW";
int[] paramBuffer = new int[parameter.Length];
for (int i = 0; i < parameter.Length; i++)
{
    paramBuffer[i] = (int)parameter[i];
}

WdmOperations.F90_WDLBAD(wdmUnit, dsn, 2, 0, parameter.Length, paramBuffer, out int retCode);

if (retCode == 0)
{
    Console.WriteLine($"Parameter set: {parameter}");
    // This automatically sets:
    // - Attribute 289 (CONSTITUENT): "STREAMFLOW"
    // - Attribute 1 (TSTYPE): "FLOW"
    // - TimeSeriesType: "STREAMFLOW"
}
```

### Example 3: Set Time Step with Parsing
```csharp
string timeStep = "60-Minute";
int[] timeBuffer = new int[timeStep.Length];
for (int i = 0; i < timeStep.Length; i++)
{
    timeBuffer[i] = (int)timeStep[i];
}

WdmOperations.F90_WDLBAD(wdmUnit, dsn, 3, 0, timeStep.Length, timeBuffer, out int retCode);

if (retCode == 0)
{
    Console.WriteLine($"Time step set: {timeStep}");
    // This automatically sets:
    // - Attribute 17 (TSTEP): 60
    // - Attribute 27 (TCODE): 2 (minutes)
    // - Attribute 103 (custom): "60-Minute"
    // - TimeStep: 60, TimeUnit: 2
}
```

### Example 4: Set Units with Code Mapping
```csharp
string units = "CFS";
int[] unitsBuffer = new int[units.Length];
for (int i = 0; i < units.Length; i++)
{
    unitsBuffer[i] = (int)units[i];
}

WdmOperations.F90_WDLBAD(wdmUnit, dsn, 4, 0, units.Length, unitsBuffer, out int retCode);

if (retCode == 0)
{
    Console.WriteLine($"Units set: {units}");
    // This automatically sets:
    // - Attribute 33 (TUNIT): 1 (CFS code)
    // - Attribute 84 (Units Text): "CFS"
}
```

### Example 5: Helper Function for Easy Use
```csharp
void SetLabel(int wdmUnit, int dsn, int labelType, string labelText)
{
    int[] labelBuffer = new int[labelText.Length];
    for (int i = 0; i < labelText.Length; i++)
    {
        labelBuffer[i] = (int)labelText[i];
    }
    
    WdmOperations.F90_WDLBAD(wdmUnit, dsn, labelType, 0, labelText.Length, labelBuffer, out int retCode);
    
    if (retCode == 0)
    {
        Console.WriteLine($"Successfully set label: {labelText}");
    }
    else
    {
        Console.WriteLine($"Failed to set label: error code {retCode}");
    }
}

// Usage
SetLabel(wdmUnit, dsn, 1, "STATION_001");
SetLabel(wdmUnit, dsn, 2, "TEMPERATURE");
SetLabel(wdmUnit, dsn, 3, "Daily");
SetLabel(wdmUnit, dsn, 4, "DEGREES F");
SetLabel(wdmUnit, dsn, 5, "OBSERVED");
SetLabel(wdmUnit, dsn, 6, "Daily maximum temperature");
```

## Intelligent Parsing Features

### Parameter Type Recognition
The function automatically recognizes parameter types and sets appropriate TSTYPE attributes:

| Input Text | Normalized TSTYPE | Description |
|------------|-------------------|-------------|
| "STREAMFLOW", "DISCHARGE", "FLOW" | "FLOW" | Stream flow data |
| "PRECIPITATION", "PRECIP", "RAIN" | "PREC" | Precipitation data |
| "EVAPORATION", "EVAP" | "EVAP" | Evaporation data |
| "TEMPERATURE", "TEMP" | "TEMP" | Temperature data |
| "CONCENTRATION", "CONC" | "CONC" | Concentration data |
| "LOAD" | "LOAD" | Load data |
| "STAGE", "LEVEL" | "STAGE" | Stage/level data |
| "QUALITY", "QUAL" | "QUAL" | Water quality data |

### Time Step Parsing
The function can extract time step information from descriptive text:

| Input Text | TSTEP | TCODE | Description |
|------------|-------|--------|-------------|
| "Daily", "1-Day" | 1 | 4 | Daily data |
| "Hourly", "1-Hour" | 1 | 3 | Hourly data |
| "60-Minute" | 60 | 2 | 60-minute data |
| "15-Minute" | 15 | 2 | 15-minute data |
| "Monthly" | 1 | 5 | Monthly data |
| "Annual", "Yearly" | 1 | 6 | Annual data |

### Units Code Mapping
Automatic mapping of units text to WDM standard codes:

| Input Text | TUNIT Code | Description |
|------------|------------|-------------|
| "CFS" | 1 | Cubic feet per second |
| "INCHES", "IN" | 2 | Inches |
| "DEGREES F", "°F", "FAHRENHEIT" | 3 | Degrees Fahrenheit |
| "MG/L" | 4 | Milligrams per liter |
| "PERCENT", "%" | 5 | Percent |
| "FEET", "FT" | 6 | Feet |
| "TONS/DAY" | 7 | Tons per day |
| "ACRE-FEET" | 8 | Acre-feet |
| "DEGREES C", "°C", "CELSIUS" | 9 | Degrees Celsius |
| "MM" | 10 | Millimeters |
| "CMS" | 11 | Cubic meters per second |
| "KG/DAY" | 12 | Kilograms per day |

## Automatic Attribute Mapping

### Station Labels (Type 1)
- **Primary**: Attribute 2 (STAID)
- **Secondary**: Attribute 290 (LOCATION) if label length > 8

### Parameter Labels (Type 2)
- **Primary**: Attribute 289 (CONSTITUENT)
- **Secondary**: Attribute 1 (TSTYPE) - normalized type
- **Property**: TimeSeriesType property

### Time Labels (Type 3)
- **Primary**: Attribute 17 (TSTEP) - parsed time step
- **Secondary**: Attribute 27 (TCODE) - parsed time code
- **Custom**: Attribute 103 - original text
- **Properties**: TimeStep, TimeUnit properties

### Units Labels (Type 4)
- **Primary**: Attribute 33 (TUNIT) - mapped code
- **Secondary**: Attribute 84 - original text

### Scenario Labels (Type 5)
- **Primary**: Attribute 288 (SCENARIO)

### Description Labels (Type 6)
- **Primary**: Attribute 85 (Description)

## Advanced Features

### Dataset Auto-Creation
If the specified dataset doesn't exist, it will be automatically created with:
- DSN set to the requested number
- DataSetType set to 1 (default)
- Empty data collection initialized

### Custom Label Types
For label types > 6, labels are stored in custom attribute range:
```
Attribute Index = labelType + 100 + axisIndex
```

### Multi-Axis Support
The axisIndex parameter allows for multi-dimensional data labeling:
```csharp
// Set X-axis label
F90_WDLBAD(wdmUnit, dsn, 7, 0, length, buffer, out retCode);
// Set Y-axis label  
F90_WDLBAD(wdmUnit, dsn, 7, 1, length, buffer, out retCode);
```

### Integration with F90_WDLBAX
Labels set with F90_WDLBAD can be retrieved with F90_WDLBAX:
```csharp
// Set a label
F90_WDLBAD(wdmUnit, dsn, 1, 0, length, setBuffer, out int setCode);

// Read it back
F90_WDLBAX(wdmUnit, dsn, 1, 0, maxLength, getBuffer, out int getLength, out int getCode);
```

## Error Handling
The function provides comprehensive error handling:
- **Input validation** - Check parameters for validity
- **WDM unit verification** - Ensure unit exists and is accessible
- **Character conversion** - Handle FORTRAN character array format
- **Attribute setting** - Validate attribute operations
- **Exception management** - Graceful handling of unexpected errors

## Performance Considerations
- **Efficient parsing** - Smart pattern matching for type recognition
- **Minimal allocation** - Direct array operations for character handling
- **Batch operations** - Single function call sets multiple attributes
- **Memory safe** - Proper bounds checking for all array operations

## See Also
- `F90_WDLBAX` - Get WDM dataset label information
- `F90_WDBSAC` - Set character attributes (direct)
- `F90_WDBSAI` - Set integer attributes (direct)
- `DataConversionUtilities.StringToIntArray` - Convert strings to integer arrays

---
*Part of HASS_ENT.Net WDM Operations suite*