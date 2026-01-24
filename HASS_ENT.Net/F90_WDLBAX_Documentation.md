# F90_WDLBAX Function Documentation

## Overview
The `F90_WDLBAX` function retrieves WDM dataset label axis information, providing descriptive labels for stations, parameters, time axes, units, and other metadata from WDM datasets.

## Function Signature
```csharp
public static void F90_WDLBAX(
    int wdmUnit,            // WDM unit number
    int dsn,                // Dataset number
    int labelType,          // Label type (1-6+)
    int axisIndex,          // Axis index for multi-dimensional data
    int maxLength,          // Maximum length of output buffer
    int[] labelBuffer,      // Output label buffer as integer array
    out int actualLength,   // Actual length of label returned
    out int retCode         // Return code (0=success, 1=no label, -1=error, -2=dataset not found)
)
```

## Label Types
| Type | Description | Source Attributes | Example Output |
|------|-------------|-------------------|----------------|
| 1    | Station     | STAID(2), LOCATION(290), Station Name(45) | "USGS_01010000", "MAIN_STEM" |
| 2    | Parameter   | CONSTITUENT(289), TSTYPE(1), Parameter Name(83) | "STREAMFLOW", "Temperature" |
| 3    | Time        | TSTEP(17), TCODE(27) | "Daily", "1440-Day", "Hourly" |
| 4    | Units       | TUNIT(33), Units Text(84) | "CFS", "DEGREES F", "MG/L" |
| 5    | Scenario    | SCENARIO(288) | "BASELINE", "HISTORICAL" |
| 6    | Description | Description(85), Comments(86) | "Daily mean discharge" |

## Return Codes
| Code | Meaning |
|------|---------|
| 0    | Success - label retrieved |
| 1    | Success - no label available |
| -1   | General error |
| -2   | Dataset not found |

## Usage Examples

### Example 1: Get Station Label
```csharp
int wdmUnit = 101;
int dsn = 100;
int[] stationLabel = new int[50];

WdmOperations.F90_WDLBAX(wdmUnit, dsn, 1, 0, 50, stationLabel, 
    out int length, out int retCode);

if (retCode == 0 && length > 0)
{
    string station = DataConversionUtilities.IntArrayToString(stationLabel, length);
    Console.WriteLine($"Station: {station}");
}
```

### Example 2: Get Parameter and Units Labels
```csharp
int[] paramLabel = new int[30];
int[] unitsLabel = new int[20];

// Get parameter label
WdmOperations.F90_WDLBAX(wdmUnit, dsn, 2, 0, 30, paramLabel, 
    out int paramLength, out int paramCode);

// Get units label  
WdmOperations.F90_WDLBAX(wdmUnit, dsn, 4, 0, 20, unitsLabel,
    out int unitsLength, out int unitsCode);

if (paramCode == 0 && unitsCode == 0)
{
    string parameter = DataConversionUtilities.IntArrayToString(paramLabel, paramLength);
    string units = DataConversionUtilities.IntArrayToString(unitsLabel, unitsLength);
    Console.WriteLine($"Parameter: {parameter} ({units})");
}
```

### Example 3: Get All Labels for a Dataset
```csharp
void PrintAllLabels(int wdmUnit, int dsn)
{
    string[] labelTypes = { "Station", "Parameter", "Time", "Units", "Scenario", "Description" };
    
    for (int labelType = 1; labelType <= 6; labelType++)
    {
        int[] buffer = new int[100];
        WdmOperations.F90_WDLBAX(wdmUnit, dsn, labelType, 0, 100, buffer,
            out int length, out int retCode);
            
        if (retCode == 0 && length > 0)
        {
            string label = DataConversionUtilities.IntArrayToString(buffer, length);
            Console.WriteLine($"{labelTypes[labelType-1]}: {label}");
        }
    }
}
```

## Supported WDM Attributes

### Station Information
- **Attribute 2 (STAID)** - Station identifier
- **Attribute 290 (LOCATION)** - Location description  
- **Attribute 45** - Station name

### Parameter Information
- **Attribute 289 (CONSTITUENT)** - Constituent/parameter name
- **Attribute 1 (TSTYPE)** - Time series type
- **Attribute 83** - Parameter name

### Time Information
- **Attribute 17 (TSTEP)** - Time step value
- **Attribute 27 (TCODE)** - Time code (1=sec, 2=min, 3=hour, 4=day, 5=month, 6=year)

### Units Information  
- **Attribute 33 (TUNIT)** - Units code
- **Attribute 84** - Units text description

### Scenario Information
- **Attribute 288 (SCENARIO)** - Scenario name

### Description Information
- **Attribute 85** - Description text
- **Attribute 86** - Comments

## Parameter Type Mapping
| TSTYPE Code | Description |
|-------------|-------------|
| FLOW        | Stream Flow |
| PREC        | Precipitation |
| EVAP        | Evaporation |
| TEMP        | Temperature |
| CONC        | Concentration |
| LOAD        | Load |
| STAGE       | Stage/Level |
| QUAL        | Water Quality |

## Units Code Mapping
| Code | Description |
|------|-------------|
| 1    | CFS (Cubic feet per second) |
| 2    | INCHES |
| 3    | DEGREES F |
| 4    | MG/L (Milligrams per liter) |
| 5    | PERCENT |
| 6    | FEET |
| 7    | TONS/DAY |
| 8    | ACRE-FEET |
| 9    | DEGREES C |
| 10   | MM (Millimeters) |
| 11   | CMS (Cubic meters per second) |
| 12   | KG/DAY (Kilograms per day) |

## Features
? **Comprehensive Label Types** - Station, parameter, time, units, scenario, description  
? **WDM Attribute Mapping** - Automatically maps WDM attribute indices to meaningful labels  
? **Flexible Output** - Variable-length label retrieval with actual length reporting  
? **Error Handling** - Comprehensive error codes and exception handling  
? **Default Fallbacks** - Provides generic labels when attributes are missing  
? **FORTRAN Compatibility** - Character array output format matching original FORTRAN  

## Implementation Details

### Label Resolution Priority
1. Primary WDM attribute (e.g., STAID for stations)
2. Secondary WDM attribute (e.g., LOCATION for stations)
3. Tertiary attributes or calculated values
4. Generic default labels

### Character Encoding
- Labels are returned as integer arrays where each integer represents an ASCII character
- Use `DataConversionUtilities.IntArrayToString()` to convert to C# strings
- Unused buffer positions are filled with space characters (ASCII 32)

### Memory Usage
- Efficient string handling with minimal memory allocation
- Buffer size determines maximum label length
- Actual length parameter indicates valid data length

## Testing
The function includes comprehensive test coverage:
- Unit tests with mock datasets and attributes
- Integration tests with real WDM files
- Edge case testing (missing attributes, non-existent datasets)
- Interactive testing through menu option 8

## See Also
- `F90_WDBSGC_XX` - Get string attributes
- `F90_WDBSGI` - Get integer attributes  
- `F90_WDCKDT` - Check dataset type
- `DataConversionUtilities.IntArrayToString` - Convert integer array to string

---
*Part of HASS_ENT.Net WDM Operations suite*