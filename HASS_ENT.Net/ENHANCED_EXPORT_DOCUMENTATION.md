# Enhanced Export Functionality Documentation

## Overview

The HASS_ENT.Net library has been enhanced with improved CSV export functionality that can output time series data in formats similar to professional water modeling tools. The enhanced export includes station ID and variable name columns for better data organization and analysis.

## New Export Functions

### F90_TSFLAT_EX - Enhanced Export

This new function extends the original `F90_TSFLAT` with metadata columns:

```csharp
FileManagement.F90_TSFLAT_EX(
    wdmUnit, dsn, fileName, includeHeader, dataFormat, 
    blankIfZero, timeStep, timeUnit, qualityCheck, overwrite, 
    fillValue, startDate, endDate, includeMetadata, out returnCode);
```

**Key Parameters:**
- `includeMetadata` (bool): When true, includes Station_ID and Variable columns
- All other parameters match the original `F90_TSFLAT` function

## Export Format Options

### Standard Format (Compatible with Original)
```
# Time Series Data Export
# Dataset: 100
# Station: 72030763
# Variable: PEVT
# Time Step: 60
# Time Unit: 3
# Date/Time	 Value
1/1/2015 0:00	 0.00000
1/1/2015 1:00	 0.00000
1/1/2015 2:00	 0.00000
```

### Enhanced Format (Station_ID, Variable, DateTime, Value)
```
Station_ID	 Variable	 DateTime	 Value
72030763	PEVT	1/1/2015 0:00	0
72030763	PEVT	1/1/2015 1:00	0
72030763	PEVT	1/1/2015 2:00	0
72030763	PEVT	1/1/2015 3:00	0
72030763	PEVT	1/1/2015 8:00	0.001
72030763	PEVT	1/1/2015 9:00	0.00217
72030763	PEVT	1/1/2015 10:00	0.00284
```

## Station ID and Variable Detection

The system automatically extracts Station ID and Variable names from WDM dataset attributes:

### Station ID Detection (in priority order):
1. **Attribute 2** (STAID) - Standard station identifier
2. **Attribute 290** (LOCATION) - Location name
3. **Attribute 45** (Station name)
4. **Default**: Zero-padded dataset number (e.g., "00000100")

### Variable Detection (in priority order):
1. **Attribute 289** (CONSTITUENT) - Parameter/constituent name
2. **Attribute 1** (TSTYPE) - Time series type (with conversion)
3. **TimeSeriesType** property
4. **Attribute 83** (Parameter name)
5. **Default**: "DATA"

### Time Series Type Conversion
Common WDM time series types are converted to standard variable names:
- `1` or `FLOW` ? `FLOW`
- `2` or `PREC` ? `PEVT` (Potential ET)
- `3` or `EVAP` ? `EVAP`
- `4` or `TEMP` ? `TEMP`
- `STREAMFLOW` ? `FLOW`
- `POTENTIAL_ET` ? `PEVT`
- etc.

## FORTRAN Format Support

Both export functions support FORTRAN-style format specifications:

### Supported Format Types:
- **F format**: `F10.2` (Fixed decimal with 2 places) ? `.NET F2`
- **E format**: `E12.4` (Scientific notation) ? `.NET E4`
- **I format**: `I5` (Integer) ? `.NET F0`
- **G format**: `G10.3` (General) ? `.NET G3`

### Format Examples:
```csharp
"F10.2"  // 2 decimal places: 123.45
"F10.3"  // 3 decimal places: 123.456
"F10.5"  // 5 decimal places: 123.45600
"E12.4"  // Scientific: 1.2345E+002
```

## Usage Examples

### Basic Enhanced Export
```csharp
// Enhanced export with metadata columns
FileManagement.F90_TSFLAT_EX(wdmUnit, dsn, "output.csv", 1,
    "F10.3", 0, 1440, 4, 0, 1, -999.0f, 
    startDate, endDate, true, out int result);
```

### Standard Export (Backward Compatible)
```csharp
// Original format for compatibility
FileManagement.F90_TSFLAT(wdmUnit, dsn, "output.csv", 1,
    "F10.2", 0, 1440, 4, 0, 1, -999.0f, 
    startDate, endDate, out int result);
```

### Interactive Testing
Run the test program with export options:
```bash
dotnet run export  # Quick test with sample data
dotnet run         # Interactive menu with full options
```

## Benefits

### Enhanced Format Advantages:
1. **Better Data Organization**: Station ID and variable in every row
2. **Database-Friendly**: Easy import into databases and analysis tools
3. **Self-Documenting**: No need to track metadata separately
4. **Standardized**: Consistent with professional water modeling tools

### Backward Compatibility:
- Original `F90_TSFLAT` function unchanged
- All existing code continues to work
- Optional enhancement via new `F90_TSFLAT_EX` function

## Testing

The enhanced export functionality includes comprehensive tests:

- **QuickExportTest**: Demonstrates both formats with sample data
- **Interactive Tests**: Menu option #4 in the test program
- **Format Validation**: Tests FORTRAN format conversion
- **Attribute Detection**: Verifies station ID and variable extraction

Run tests:
```bash
cd HASS_ENT.Net
dotnet run export          # Quick demonstration
dotnet run                 # Full interactive menu
```

This enhancement provides professional-grade CSV export capabilities while maintaining full backward compatibility with existing HASS_ENT.Net applications.