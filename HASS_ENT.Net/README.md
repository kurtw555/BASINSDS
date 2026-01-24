# HASS_ENT.Net Function Implementation Summary

## Overview
Complete C# implementation of HASS_ENT FORTRAN functions for .NET 8, providing equivalent functionality for hydrologic modeling and water data management.

## Implemented Functions

### Logging Functions
- **F90_MSG(string message)** - Log a message to the error file
- **F90_W99OPN()** - Open log file for writing
- **F90_W99CLO()** - Close log file
- **F90_MSGUNIT(int messageUnit)** - Set message unit

### File Operations
- **F90_WDMOPN(int unit, string fileName)** - Open a WDM file
- **F90_WDMCLO(int unit)** - Close a WDM file
- **F90_WDBOPN(int rwFlag, string wdName)** - Open WDM database file
- **F90_INQNAM(string fileName)** - Check if a file name is open
- **F90_WDFLCL(int wdmUnit)** - Close WDM file
- **F90_WDBFIN()** - Finalize WDM operations

### Date/Time Utilities
- **F90_DAYMON(int year, int month)** - Get number of days in a month
- **F90_TIMCHK(int[] date1, int[] date2)** - Compare two dates
- **F90_JDMODY(int year, int julianDay, out int month, out int day)** - Convert Julian day to month/day
- **F90_TIMADD(int[] date1, int tCode, int tStep, int nVals, int[] date2)** - Add time to a date
- **F90_TIMDIF(int[] date1, int[] date2, int tCode, int tStep, out int nVals)** - Calculate time difference

### Mathematical Utilities
- **F90_SCALIT(int iType, float dataMin, float dataMax, out float plotMin, out float plotMax)** - Scale values for plotting
- **F90_ASRTRP(float[] values)** - Sort real array
- **F90_DECPRC(int sigDig, int decPla, ref float value)** - Round value to specified precision
- **F90_PUTOLV(int outLevel)** - Set output level

### Message and Table Functions
- **F90_WMSGTW_XX(int messUnit, int tableId)** - Write message table
- **F90_WMSGTT_XX(int wdmUnit, int dsn, int groupNum, int initFlag, ref int outputLength, out int continuation, int[] outputBuffer)** - Get message table text
- **F90_WMSGTH(int messUnit, int tableId)** - Get message table header

### Keyword and Information Functions
- **F90_GTNXKW_XX(int inputUnit, int outputUnit, int[] keyword, out int keywordLength, out int lineNumber, out int errorCode)** - Get next keyword
- **F90_XTINFO_XX(int infoType, int param1-11, float realParam1-3, int[] textParam1-6)** - Get extended information

### WDM Dataset Operations (WdmOperations.cs)
- **F90_WDBOPNR(int rwFlag, string wdName, out int wdmUnit, out int retCode)** - Open WDM database with return code
- **F90_WDCKDT(int wdmUnit, int dsn)** - Check dataset type
- **F90_WDDSNX(int wdmUnit, ref int dsn)** - Get next dataset number
- **F90_WDDSRN(int wdmUnit, int oldDsn, int newDsn, out int retCode)** - Rename dataset
- **F90_WDDSDL(int wdmUnit, int dsn, out int retCode)** - Delete dataset

### Attribute Operations
- **F90_WDBSGC_XX(int wdmUnit, int dsn, int attrIndex, int attrLength, int[] values)** - Get string attribute
- **F90_WDBSGI(int wdmUnit, int dsn, int attrIndex, int attrLength, int[] values, out int retCode)** - Get integer attribute
- **F90_WDBSGR(int wdmUnit, int dsn, int attrIndex, int attrLength, float[] values, out int retCode)** - Get real attribute
- **F90_WDBSAC(int wdmUnit, int dsn, int messUnit, int attrIndex, int attrLength, out int retCode, string value)** - Set character attribute
- **F90_WDBSAI(int wdmUnit, int dsn, int messUnit, int attrIndex, int attrLength, int[] values, out int retCode)** - Set integer attribute
- **F90_WDBSAR(int wdmUnit, int dsn, int messUnit, int attrIndex, int attrLength, float[] values, out int retCode)** - Set real attribute
- **F90_GETATT(int wdmUnit, int dsn, int attrIndex, int attrType, int maxLength, int[] intValues, float[] realValues, int[] charValues, out int actualLength, out int retCode)** - Get WDM dataset attribute generically with automatic type conversion and default value support

### Time Series Data Operations
- **F90_WDTGET(int wdmUnit, int dsn, int delta, int[] dates, int nVal, int dataTran, int qualFlag, int tUnits, float[] values, out int retCode)** - Get time series data
- **F90_WDTPUT(int wdmUnit, int dsn, int delta, int[] dates, int nVal, int dataOverwrite, int qualFlag, int tUnits, float[] values, out int retCode)** - Put time series data
- **F90_WDSAGY_XX(int wdmUnit, int dsn, int aggType, int[] startDate, int[] endDate, int timeStep, int timeUnit, float[] outputValues, int[,] outputDates, out int nValues, out int retCode)** - Aggregate time series data with multiple aggregation types (sum, mean, max, min, count)
- **F90_WTFNDT(int wdmUnit, int dsn, int searchMode, int[] targetDate, int[] startDate, int[] endDate, int[] foundDate, out float foundValue, out int foundQuality, out int dataIndex, out int retCode)** - Find time series data with comprehensive search modes (first, last, nearest, next, previous, exact, interpolated)

### Label and Metadata Operations  
- **F90_WDLBAX(int wdmUnit, int dsn, int labelType, int axisIndex, int maxLength, int[] labelBuffer, out int actualLength, out int retCode)** - Get WDM dataset label axis information for stations, parameters, time, units, and scenarios
- **F90_WDLBAD(int wdmUnit, int dsn, int labelType, int axisIndex, int labelLength, int[] labelBuffer, out int retCode)** - Add/Set WDM dataset label information with intelligent parsing and attribute mapping

### Date/Time Conversion Functions (DateTimeUtilities.cs)
- **F90_DATLST_XX(int[] date, int[] idStr, out int length, out int errorCode)** - Convert date to string format
- **F90_TIMCNV(int[] dateArray)** - Convert time format
- **F90_TIMCVT(int[] dateArray)** - Convert time between formats
- **F90_TIMBAK(int tCode, int[] dateArray)** - Move time backward by one time step
- **F90_CMPTIM(int tCode1, int tStep1, int tCode2, int tStep2, out int tStepF, out int tcdCmp)** - Compare time specifications
- **F90_DATNXT(int interval, int upBack, int[] dateArray)** - Get next date given interval
- **F90_DTMCMN(int nDat, int[,] startDates, int[,] stopDates, int tStep, int tCode, int[] commonStart, int[] commonEnd, out int finalTStep, out int finalTCode, out int retCode)** - Find common time period

### Data Conversion Functions (DataConversionUtilities.cs)
- **F90_DECCHX_XX(float realValue, int length, int sigDig, int decPla, int[] outputString)** - Convert real number to character string
- **IntArrayToString(int[] intArray, int length = -1)** - Convert integer array to string
- **StringToIntArray(string text, int[] intArray, int maxLength = -1)** - Convert string to integer array

### Time Series Analysis Functions (TimeSeriesAnalysis.cs)
- **F90_DAANST(int nVals, float[] values)** - Analyze time series statistics
- **F90_FITLIN(int nPts, int bufMax, float[] yx, out float aCoef, out float bCoef, out float rSquare)** - Fit linear regression line
- **F90_CMSTRM(...)** - Calculate storm statistics
- **F90_TSCBAT(...)** - Time series comparison batch analysis

### File Management Functions (FileManagement.cs)
- **F90_WATINI(string fileName, int unit)** - Initialize water data input
- **F90_WATHED_XX(...)** - Read water data header
- **F90_WATINP(...)** - Input water data
- **F90_WATCLO(int inputUnit)** - Close water data input
- **F90_TSFLAT(...)** - Export time series to flat file
- **F90_INFREE(int wdmUnit, int datasetType, int startDsn, int increment, out int freeDsn, out int returnCode)** - Find free dataset number

## Supporting Infrastructure
- **ErrorHandling.cs** - Comprehensive error management and validation
- **HassEntLibrary.cs** - Main library interface with initialization/shutdown
- **HassBaseOperation.cs** - Base class for operations
- **LoggingService.cs** - Enhanced logging capabilities

## Usage Example
```csharp
// Initialize the library
HassEntLibrary.Initialize();

// Open WDM file
int wdmUnit = HassEntFunctions.F90_WDBOPN(0, "data.wdm");

// Work with dates
int[] date = { 2024, 1, 15, 10, 30, 0 };
int[] newDate = new int[6];
HassEntFunctions.F90_TIMADD(date, 4, 1, 10, newDate);

// Set message unit
HassEntFunctions.F90_MSGUNIT(99);

// Log messages
HassEntFunctions.F90_MSG("Processing complete");

// Clean up
HassEntLibrary.Shutdown();
```

## Key Features
? **Complete FORTRAN function compatibility**  
? **Full WDM database functionality**  
? **Comprehensive error handling**  
? **Modern C# patterns and practices**  
? **Memory and resource management**  
? **.NET 8 compatibility**  
? **Extensive validation and logging**  
? **Production-ready implementation**  

## Testing
All functions are tested in the `TestProgram.cs` which demonstrates proper usage and validates functionality.

---
*HASS_ENT.Net v1.0.0 - Complete C# Implementation of HASS_ENT FORTRAN Functions*