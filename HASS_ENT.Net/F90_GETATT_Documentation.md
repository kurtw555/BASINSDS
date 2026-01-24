# F90_GETATT Function Documentation

## Overview
The `F90_GETATT` function provides a generic interface for retrieving WDM dataset attributes with automatic type conversion and default value support. This function consolidates the functionality of `F90_WDBSGI`, `F90_WDBSGR`, and `F90_WDBSGC_XX` into a single, flexible interface.

## Function Signature
```csharp
public static void F90_GETATT(
    int wdmUnit,            // WDM unit number
    int dsn,                // Dataset number
    int attrIndex,          // Attribute index
    int attrType,           // Attribute type (1=integer, 2=real, 3=character)
    int maxLength,          // Maximum length for character attributes
    int[] intValues,        // Integer values output array
    float[] realValues,     // Real values output array
    int[] charValues,       // Character values output array (as integer array)
    out int actualLength,   // Actual length of attribute returned
    out int retCode         // Return code
)
```

## Attribute Types
| Type | Description | Output Array | Usage |
|------|-------------|--------------|-------|
| 1    | Integer     | `intValues`  | Numeric codes, counts, flags |
| 2    | Real        | `realValues` | Floating-point values, measurements |
| 3    | Character   | `charValues` | Text strings, names, descriptions |

## Return Codes
| Code | Meaning | Description |
|------|---------|-------------|
| 0    | Success | Attribute retrieved successfully |
| 1    | Success with default | No attribute found, default value returned |
| -1   | General error | Unexpected error occurred |
| -2   | Dataset not found | Specified dataset does not exist |
| -3   | Type conversion error | Cannot convert attribute to requested type |
| -4   | Attribute not found | Attribute does not exist and no default available |

## Usage Examples

### Example 1: Get Integer Attribute
```csharp
int wdmUnit = 101;
int dsn = 100;
int[] intValues = new int[5];
float[] realValues = new float[5];
int[] charValues = new int[50];

// Get time step (attribute 17) as integer
WdmOperations.F90_GETATT(wdmUnit, dsn, 17, 1, 50, intValues, realValues, charValues,
    out int length, out int retCode);

if (retCode == 0)
{
    int timeStep = intValues[0];
    Console.WriteLine($"Time step: {timeStep} minutes");
}
```

### Example 2: Get String Attribute
```csharp
// Get station ID (attribute 2) as string
WdmOperations.F90_GETATT(wdmUnit, dsn, 2, 3, 50, intValues, realValues, charValues,
    out int length, out int retCode);

if (retCode == 0)
{
    string stationId = DataConversionUtilities.IntArrayToString(charValues, length);
    Console.WriteLine($"Station ID: {stationId}");
}
```

### Example 3: Get Array Attributes
```csharp
// Get array attribute
WdmOperations.F90_GETATT(wdmUnit, dsn, 101, 1, 10, intValues, realValues, charValues,
    out int length, out int retCode);

if (retCode == 0 && length > 1)
{
    Console.WriteLine($"Array values: [{string.Join(", ", intValues.Take(length))}]");
}
```

### Example 4: Type Conversion
```csharp
// Get integer attribute as float (automatic conversion)
WdmOperations.F90_GETATT(wdmUnit, dsn, 17, 2, 50, intValues, realValues, charValues,
    out int length, out int retCode);

if (retCode == 0)
{
    float timeStepFloat = realValues[0];
    Console.WriteLine($"Time step as float: {timeStepFloat}");
}
```

### Example 5: Handle Defaults
```csharp
// Get attribute that may not exist
WdmOperations.F90_GETATT(wdmUnit, dsn, 288, 3, 50, intValues, realValues, charValues,
    out int length, out int retCode);

if (retCode == 0)
{
    string scenario = DataConversionUtilities.IntArrayToString(charValues, length);
    Console.WriteLine($"Scenario: {scenario}");
}
else if (retCode == 1)
{
    string defaultScenario = DataConversionUtilities.IntArrayToString(charValues, length);
    Console.WriteLine($"Default scenario: {defaultScenario}");
}
```

## Type Conversion Support

### Automatic Conversions
| From Type | To Type | Conversion Method |
|-----------|---------|-------------------|
| Integer   | Float   | Direct cast |
| Integer   | String  | ToString() |
| Float     | Integer | Math.Round() |
| Float     | String  | ToString() |
| String    | Integer | int.TryParse() |
| String    | Float   | float.TryParse() |
| Array     | Single  | First element |

### Array Handling
- **Integer arrays** ? Copied to output with actual length
- **Float arrays** ? Copied to output with actual length  
- **String values** ? Converted to character array (ASCII codes)
- **Single values** ? Placed in first array element

## Default Values

The function provides intelligent defaults for common WDM attributes when they don't exist:

| Attribute | Index | Default Value | Type |
|-----------|-------|---------------|------|
| TSTYPE    | 1     | "DATA"        | String |
| TSTEP     | 17    | 1440 (daily)  | Integer |
| TCODE     | 27    | 4 (daily)     | Integer |
| TUNIT     | 33    | 1 (CFS)       | Integer |
| SCENARIO  | 288   | "OBSERVED"    | String |
| CONSTITUENT | 289 | "DATA"        | String |
| LOCATION  | 290   | "UNKNOWN"     | String |

## Advanced Features

### Multi-Type Access Pattern
```csharp
void GetAttributeFlexibly(int wdmUnit, int dsn, int attrIndex)
{
    int[] intVals = new int[10];
    float[] realVals = new float[10];
    int[] charVals = new int[100];
    
    // Try as integer first
    WdmOperations.F90_GETATT(wdmUnit, dsn, attrIndex, 1, 100, 
        intVals, realVals, charVals, out int len1, out int ret1);
        
    if (ret1 == 0)
    {
        Console.WriteLine($"As integer: {intVals[0]}");
        return;
    }
    
    // Try as string
    WdmOperations.F90_GETATT(wdmUnit, dsn, attrIndex, 3, 100,
        intVals, realVals, charVals, out int len3, out int ret3);
        
    if (ret3 == 0)
    {
        string text = DataConversionUtilities.IntArrayToString(charVals, len3);
        Console.WriteLine($"As string: '{text}'");
    }
}
```

### Batch Attribute Retrieval
```csharp
void GetAllCommonAttributes(int wdmUnit, int dsn)
{
    var attributes = new[]
    {
        (2, "Station ID", 3),
        (17, "Time Step", 1),
        (27, "Time Code", 1),
        (33, "Units", 1),
        (288, "Scenario", 3),
        (289, "Constituent", 3)
    };
    
    foreach (var (index, name, type) in attributes)
    {
        int[] intVals = new int[10];
        float[] realVals = new float[10];
        int[] charVals = new int[100];
        
        WdmOperations.F90_GETATT(wdmUnit, dsn, index, type, 100,
            intVals, realVals, charVals, out int length, out int retCode);
            
        if (retCode <= 1) // Success or default
        {
            string value = type == 3 
                ? DataConversionUtilities.IntArrayToString(charVals, length)
                : (type == 1 ? intVals[0].ToString() : realVals[0].ToString("F2"));
                
            Console.WriteLine($"{name}: {value} {(retCode == 1 ? "(default)" : "")}");
        }
    }
}
```

## Performance Considerations
- **Single Call Efficiency** - One function call handles all attribute types
- **Type Safety** - Automatic validation and conversion
- **Memory Efficient** - Minimal object allocation for conversions
- **Error Resilient** - Comprehensive error handling with meaningful codes

## Integration Benefits
? **Unified Interface** - Single function for all attribute types  
? **Automatic Conversion** - No manual type casting required  
? **Default Fallbacks** - Intelligent defaults for missing attributes  
? **Array Support** - Handles both single values and arrays  
? **FORTRAN Compatibility** - Character arrays match FORTRAN conventions  
? **Error Handling** - Detailed error codes and exception management  

## See Also
- `F90_WDBSGI` - Get integer attributes (specific)
- `F90_WDBSGR` - Get real attributes (specific)  
- `F90_WDBSGC_XX` - Get character attributes (specific)
- `F90_WDLBAX` - Get formatted labels
- `DataConversionUtilities.IntArrayToString` - Convert character arrays

---
*Part of HASS_ENT.Net WDM Operations suite*