# Sample WDM File Testing Guide

## Overview
This guide explains how to use the Sample.wdm test cases that have been created for the HASS_ENT.Net project.

## Available WDM Files
- `data/Sample.wdm` - Main sample WDM file
- `data/Sample_Annual.wdm` - Annual time series sample file

## Test Options

### 1. Quick Integration Test (Built into main program)
The main `TestProgram.cs` includes a quick test of the Sample.wdm file:

```bash
dotnet run --project HASS_ENT.Net.csproj
```

This runs all tests including a simple WDM file demonstration.

### 2. Comprehensive WDM Tests
To run all comprehensive WDM tests programmatically:

```csharp
SampleWdmTests.RunAllSampleWdmTests();
```

This includes:
- ? **Basic File Operations** - Opening, closing, unit management
- ? **Dataset Inspection** - Finding and examining datasets
- ? **Time Series Access** - Reading data values and statistics
- ? **Attribute Operations** - Reading WDM attributes like station IDs, time steps
- ? **Multiple File Handling** - Working with multiple WDM files
- ? **File Management** - Dataset enumeration and availability
- ? **Data Export** - Exporting data to CSV format
- ? **Advanced Operations** - Message tables and extended info

### 3. Interactive Test Program
For interactive testing with menu options:

```bash
# Run the dedicated Sample WDM test program
dotnet run --project HASS_ENT.Net.csproj SampleWdmTestProgram.cs
```

**Menu Options:**
1. **Simple WDM Demo** - Quick file open and dataset scan
2. **Run All Comprehensive Tests** - Full test suite
3. **Test Specific Dataset** - Examine a particular dataset number
4. **Test Data Export** - Export dataset to CSV file
5. **Show WDM File Info** - Display file information and dataset list
6. **Test Time Series Analysis** - Statistical analysis of dataset

### 4. Full Automated Tests
To run all tests without interaction:

```bash
dotnet run --project HASS_ENT.Net.csproj SampleWdmTestProgram.cs full
```

## Test Cases Explained

### Test Case 1: Basic WDM File Operations
```csharp
// Opens Sample.wdm in read-only mode
int wdmUnit = HassEntFunctions.F90_WDBOPN(1, SampleWdmPath);
```

**Tests:**
- File opening and unit assignment
- File inquiry operations
- WDM file info retrieval
- Proper file closing

### Test Case 2: WDM Dataset Inspection
```csharp
// Scans common dataset number ranges
int datasetType = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
```

**Tests:**
- Dataset existence checking
- Dataset type identification
- Basic attribute reading (Station ID, Scenario)

### Test Case 3: Time Series Data Access
```csharp
// Retrieves time series values
WdmOperations.F90_WDTGET(wdmUnit, dsn, 1440, startDate, 100, 0, 0, 4, values, out retCode);
```

**Tests:**
- Time series data retrieval
- Data validation and statistics
- Missing value handling
- Basic data quality checks

### Test Case 4: WDM Attribute Operations
```csharp
// Reads various WDM attributes
WdmOperations.F90_WDBSGC_XX(wdmUnit, dsn, attrIndex, 20, stringValue);
```

**Tests:**
- String attribute reading (STAID, SCENARIO, etc.)
- Integer attribute reading (TSTEP, TCODE, etc.)
- Attribute validation

### Test Case 5: Multiple WDM Files
```csharp
// Opens both sample files
int unit1 = HassEntFunctions.F90_WDBOPN(1, SampleWdmPath);
int unit2 = HassEntFunctions.F90_WDBOPN(1, SampleAnnualWdmPath);
```

**Tests:**
- Multiple file management
- Unit tracking
- Proper resource cleanup

### Test Case 6: WDM File Management
```csharp
// Scans for available datasets
for (int dsn = 1; dsn <= 1000; dsn += 10)
{
    int datasetType = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
}
```

**Tests:**
- Dataset enumeration
- Free dataset number finding
- File structure analysis

### Test Case 7: Data Export Operations
```csharp
// Exports dataset to CSV
FileManagement.F90_TSFLAT(wdmUnit, dsn, exportPath, 1, "F10.2", 0, 1440, 4, 0, 1, -999.0f, startDate, endDate, out result);
```

**Tests:**
- Time series export to flat file
- CSV format generation
- Export validation

### Test Case 8: Advanced WDM Operations
```csharp
// Tests message and extended operations
HassEntFunctions.F90_WMSGTT_XX(wdmUnit, 100, 1, 0, ref outputLength, out continuation, msgBuffer);
```

**Tests:**
- Message table operations
- Extended information retrieval
- Advanced WDM features

## Expected Results

### Successful Test Output:
```
? Successfully opened Sample.wdm on unit 101 (read-only)
? Found dataset 100 with type: 1
? Retrieved 50 valid values
? Export successful: export_dsn_100.csv (2,134 bytes)
```

### Common Issues:
- **File Not Found**: Ensure Sample.wdm is in the `data` folder
- **No Datasets Found**: WDM file may be empty or corrupted
- **Access Denied**: Check file permissions

## Customization

### Testing Specific Dataset Numbers
Modify the test DSN ranges in `TestWdmDatasetInspection()`:

```csharp
int[] testDsns = { 100, 101, 102, 200, 201, 300, 301, 400, 401 };
```

### Testing Different Date Ranges
Adjust date ranges in time series tests:

```csharp
int[] startDate = { 1990, 1, 1, 0, 0, 0 };  // Start date
int[] endDate = { 2020, 12, 31, 23, 59, 59 }; // End date
```

### Adding New Test Cases
Create additional test methods in `SampleWdmTests.cs`:

```csharp
private static void TestCustomOperation()
{
    // Your custom WDM test logic here
}
```

## Integration with Existing Code
The Sample WDM tests integrate seamlessly with the HASS_ENT.Net library:

```csharp
// Initialize library
HassEntLibrary.Initialize();

// Run your WDM operations
int wdmUnit = HassEntFunctions.F90_WDBOPN(1, "data/Sample.wdm");
// ... your WDM operations ...

// Clean up
HassEntLibrary.Shutdown();
```

## Troubleshooting

1. **File Path Issues**: Ensure the data folder exists relative to the executable
2. **WDM Format**: Verify the WDM file is in proper binary format
3. **Permissions**: Check read/write permissions on the data folder
4. **Memory**: Large WDM files may require more memory for processing

---

*Use these test cases to validate your WDM file operations and ensure the HASS_ENT.Net library works correctly with real hydrologic data files.*