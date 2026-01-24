using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HASS_ENT.Net
{
    /// <summary>
    /// File management and I/O operations for HASS_ENT system
    /// Handles water data input/output, flat file operations, and data import/export
    /// </summary>
    public static class FileManagement
    {
        private static readonly Dictionary<int, StreamReader> _inputStreams = new();
        private static readonly Dictionary<int, StreamWriter> _outputStreams = new();

        #region Water Data Input/Output Functions

        /// <summary>
        /// F90_WATINI - Initialize water data input
        /// </summary>
        /// <param name="fileName">Water data file name</param>
        /// <param name="unit">File unit number</param>
        public static void F90_WATINI(string fileName, int unit)
        {
            try
            {
                if (_inputStreams.ContainsKey(unit))
                {
                    _inputStreams[unit].Close();
                    _inputStreams.Remove(unit);
                }

                var reader = new StreamReader(fileName);
                _inputStreams[unit] = reader;
                
                HassEntFunctions.LogMsg($"Water data file initialized: {fileName} on unit {unit}");
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error initializing water data file {fileName}: {ex.Message}");
            }
        }

        /// <summary>
        /// F90_WATHED_XX - Read water data header
        /// </summary>
        /// <param name="messUnit">Message unit</param>
        /// <param name="inputUnit">Input unit</param>
        /// <param name="iVal">Integer values</param>
        /// <param name="rVal">Real values</param>
        /// <param name="siteCode">Site code as integer array</param>
        /// <param name="typeCode">Type code as integer array</param>
        /// <param name="stationId">Station ID as integer array</param>
        /// <param name="stationName">Station name as integer array</param>
        public static void F90_WATHED_XX(int messUnit, int inputUnit, int[] iVal, float[] rVal, 
            int[] siteCode, int[] typeCode, int[] stationId, int[] stationName)
        {
            try
            {
                if (!_inputStreams.TryGetValue(inputUnit, out var reader))
                {
                    HassEntFunctions.LogMsg($"Input unit {inputUnit} not open for water data header read");
                    return;
                }

                // Read header line
                string? headerLine = reader.ReadLine();
                if (string.IsNullOrEmpty(headerLine))
                {
                    HassEntFunctions.LogMsg("No header line found in water data file");
                    return;
                }

                // Parse header line - simplified implementation
                string[] fields = headerLine.Split('\t', ',', ' ');
                
                // Fill integer values (dates, etc.)
                for (int i = 0; i < Math.Min(6, iVal.Length); i++)
                {
                    if (i < fields.Length && int.TryParse(fields[i], out int intVal))
                        iVal[i] = intVal;
                    else
                        iVal[i] = 0;
                }

                // Fill real values (coordinates, etc.)
                for (int i = 0; i < Math.Min(7, rVal.Length); i++)
                {
                    int fieldIndex = i + 6;
                    if (fieldIndex < fields.Length && float.TryParse(fields[fieldIndex], out float floatVal))
                        rVal[i] = floatVal;
                    else
                        rVal[i] = 0.0f;
                }

                // Fill site code, type code, station ID, and name
                FillStringField(fields, 13, siteCode, 2);
                FillStringField(fields, 14, typeCode, 4);
                FillStringField(fields, 15, stationId, 8);
                FillStringField(fields, 16, stationName, 48);
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error reading water data header: {ex.Message}");
            }
        }

        /// <summary>
        /// F90_WATINP - Input water data
        /// </summary>
        /// <param name="messUnit">Message unit</param>
        /// <param name="inputUnit">Input unit</param>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="retCode">Return code</param>
        /// <param name="iVal">Integer values</param>
        /// <param name="rVal">Real values</param>
        /// <param name="scenario">Scenario name</param>
        /// <param name="location">Location name</param>
        /// <param name="constituent">Constituent name</param>
        /// <param name="stationId">Station ID</param>
        /// <param name="stationName">Station name</param>
        /// <param name="site">Site code</param>
        /// <param name="tsType">Time series type</param>
        public static void F90_WATINP(int messUnit, int inputUnit, int wdmUnit, out int retCode, 
            int[] iVal, float[] rVal, string scenario, string location, string constituent,
            string stationId, string stationName, string site, string tsType)
        {
            retCode = 0;
            try
            {
                if (!_inputStreams.TryGetValue(inputUnit, out var reader))
                {
                    retCode = -1;
                    return;
                }

                // Read data lines and process
                string? dataLine;
                var dataValues = new List<float>();
                var dates = new List<DateTime>();

                while ((dataLine = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(dataLine)) continue;
                    
                    string[] fields = dataLine.Split('\t', ',', ' ');
                    if (fields.Length < 2) continue;

                    // Parse date and value
                    if (DateTime.TryParse(fields[0], out DateTime date) &&
                        float.TryParse(fields[1], out float value))
                    {
                        dates.Add(date);
                        dataValues.Add(value);
                    }
                }

                // Store data in WDM if we have data
                if (dataValues.Count > 0)
                {
                    // Find or create dataset
                    int dsn = FindAvailableDatasetNumber(wdmUnit);
                    
                    // Set attributes
                    WdmOperations.F90_WDBSAC(wdmUnit, dsn, messUnit, 288, 8, out _, scenario);
                    WdmOperations.F90_WDBSAC(wdmUnit, dsn, messUnit, 289, 8, out _, constituent);
                    WdmOperations.F90_WDBSAC(wdmUnit, dsn, messUnit, 290, 8, out _, location);
                    WdmOperations.F90_WDBSAC(wdmUnit, dsn, messUnit, 1, 4, out _, tsType);

                    // Store time series data
                    var startDate = DateTimeUtilities.DateTimeToArray(dates[0]);
                    WdmOperations.F90_WDTPUT(wdmUnit, dsn, 1, startDate, dataValues.Count,
                        1, 0, 4, dataValues.ToArray(), out int putRetCode);
                    
                    retCode = putRetCode;
                }
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error in water data input: {ex.Message}");
                retCode = -1;
            }
        }

        /// <summary>
        /// F90_WATCLO - Close water data input
        /// </summary>
        /// <param name="inputUnit">Input unit to close</param>
        public static void F90_WATCLO(int inputUnit)
        {
            try
            {
                if (_inputStreams.TryGetValue(inputUnit, out var reader))
                {
                    reader.Close();
                    reader.Dispose();
                    _inputStreams.Remove(inputUnit);
                }
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error closing water data input unit {inputUnit}: {ex.Message}");
            }
        }

        #endregion

        #region Flat File Operations

        /// <summary>
        /// F90_TSFLAT - Export time series to flat file
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        /// <param name="fileName">Output file name</param>
        /// <param name="includeHeader">Include header flag</param>
        /// <param name="dataFormat">Data format string (FORTRAN style like F10.2)</param>
        /// <param name="blankIfZero">Blank if zero flag</param>
        /// <param name="timeStep">Time step</param>
        /// <param name="timeUnit">Time unit</param>
        /// <param name="qualityCheck">Quality check</param>
        /// <param name="overwrite">Overwrite flag</param>
        /// <param name="fillValue">Fill value for missing data</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <param name="returnCode">Return code</param>
        public static void F90_TSFLAT(int wdmUnit, int dsn, string fileName, int includeHeader,
            string dataFormat, int blankIfZero, int timeStep, int timeUnit, int qualityCheck,
            int overwrite, float fillValue, int[] startDate, int[] endDate, out int returnCode)
        {
            returnCode = 0;
            try
            {
                var dataSet = WdmOperations.GetDataSet(wdmUnit, dsn);
                if (dataSet == null)
                {
                    returnCode = -1;
                    return;
                }

                using (var writer = new StreamWriter(fileName))
                {
                    // Write header if requested
                    if (includeHeader != 0)
                    {
                        writer.WriteLine("# Time Series Data Export");
                        writer.WriteLine($"# Dataset: {dsn}");
                        writer.WriteLine($"# Time Step: {timeStep}");
                        writer.WriteLine($"# Time Unit: {timeUnit}");
                        writer.WriteLine("# Date/Time\t Value");
                    }

                    // Write data
                    var start = DateTimeUtilities.ArrayToDateTime(startDate);
                    var end = DateTimeUtilities.ArrayToDateTime(endDate);

                    foreach (var dataPoint in dataSet.Data)
                    {
                        if (dataPoint.DateTime >= start && dataPoint.DateTime <= end)
                        {
                            string dateStr = dataPoint.DateTime.ToString("M/d/yyyy H:mm");
                            string valueStr;

                            if (blankIfZero != 0 && Math.Abs(dataPoint.Value) < 1e-6)
                            {
                                valueStr = "";
                            }
                            else
                            {
                                // Convert FORTRAN format to .NET format
                                string netFormat = ConvertFortranFormatToNet(dataFormat);
                                valueStr = dataPoint.Value.ToString(netFormat);
                            }

                            writer.WriteLine($"{dateStr}\t {valueStr}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error exporting flat file {fileName}: {ex.Message}");
                returnCode = -2;
            }
        }

        /// <summary>
        /// F90_TSFLAT_EX - Enhanced export time series to flat file with station and variable metadata
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        /// <param name="fileName">Output file name</param>
        /// <param name="includeHeader">Include header flag</param>
        /// <param name="dataFormat">Data format string (FORTRAN style like F10.2)</param>
        /// <param name="blankIfZero">Blank if zero flag</param>
        /// <param name="timeStep">Time step</param>
        /// <param name="timeUnit">Time unit</param>
        /// <param name="qualityCheck">Quality check</param>
        /// <param name="overwrite">Overwrite flag</param>
        /// <param name="fillValue">Fill value for missing data</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <param name="includeMetadata">Include Station_ID and Variable columns</param>
        /// <param name="returnCode">Return code</param>
        public static void F90_TSFLAT_EX(int wdmUnit, int dsn, string fileName, int includeHeader,
            string dataFormat, int blankIfZero, int timeStep, int timeUnit, int qualityCheck,
            int overwrite, float fillValue, int[] startDate, int[] endDate, bool includeMetadata, out int returnCode)
        {
            returnCode = 0;
            try
            {
                var dataSet = WdmOperations.GetDataSet(wdmUnit, dsn);
                if (dataSet == null)
                {
                    returnCode = -1;
                    return;
                }

                // Get station ID and variable from dataset attributes
                string stationId = GetStationId(dataSet);
                string variable = GetVariable(dataSet);

                using (var writer = new StreamWriter(fileName))
                {
                    // Write header if requested
                    if (includeHeader != 0)
                    {
                        if (includeMetadata)
                        {
                            writer.WriteLine("Station_ID\t Variable\t DateTime\t Value");
                        }
                        else
                        {
                            writer.WriteLine("# Time Series Data Export");
                            writer.WriteLine($"# Dataset: {dsn}");
                            writer.WriteLine($"# Station: {stationId}");
                            writer.WriteLine($"# Variable: {variable}");
                            writer.WriteLine($"# Time Step: {timeStep}");
                            writer.WriteLine($"# Time Unit: {timeUnit}");
                            writer.WriteLine("# Date/Time\t Value");
                        }
                    }

                    // Write data
                    var start = DateTimeUtilities.ArrayToDateTime(startDate);
                    var end = DateTimeUtilities.ArrayToDateTime(endDate);

                    foreach (var dataPoint in dataSet.Data)
                    {
                        if (dataPoint.DateTime >= start && dataPoint.DateTime <= end)
                        {
                            string dateStr = dataPoint.DateTime.ToString("M/d/yyyy H:mm");
                            string valueStr;

                            if (blankIfZero != 0 && Math.Abs(dataPoint.Value) < 1e-6)
                            {
                                valueStr = "0";
                            }
                            else
                            {
                                // Convert FORTRAN format to .NET format
                                string netFormat = ConvertFortranFormatToNet(dataFormat);
                                valueStr = dataPoint.Value.ToString(netFormat);
                            }

                            if (includeMetadata)
                            {
                                writer.WriteLine($"{stationId}\t{variable}\t{dateStr}\t{valueStr}");
                            }
                            else
                            {
                                writer.WriteLine($"{dateStr}\t {valueStr}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error exporting enhanced flat file {fileName}: {ex.Message}");
                returnCode = -2;
            }
        }

        /// <summary>
        /// Get station ID from dataset attributes
        /// </summary>
        /// <param name="dataSet">WDM dataset</param>
        /// <returns>Station ID string</returns>
        private static string GetStationId(WdmOperations.WdmDataSet dataSet)
        {
            try
            {
                // Try different common station ID attributes
                if (dataSet.Attributes.TryGetValue(2, out var stationId2))  // STAID attribute
                {
                    string station = stationId2.ToString()?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(station))
                        return station;
                }

                if (dataSet.Attributes.TryGetValue(290, out var location))  // LOCATION attribute
                {
                    string loc = location.ToString()?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(loc))
                        return loc;
                }

                if (dataSet.Attributes.TryGetValue(45, out var stationName))  // Station name
                {
                    string name = stationName.ToString()?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(name))
                        return name;
                }

                // Default format based on dataset number
                return $"{dataSet.Dsn:D8}";
            }
            catch
            {
                return $"{dataSet.Dsn:D8}";
            }
        }

        /// <summary>
        /// Get variable/parameter name from dataset attributes
        /// </summary>
        /// <param name="dataSet">WDM dataset</param>
        /// <returns>Variable name string</returns>
        private static string GetVariable(WdmOperations.WdmDataSet dataSet)
        {
            try
            {
                // Try different common parameter/variable attributes
                if (dataSet.Attributes.TryGetValue(289, out var constituent))  // CONSTITUENT attribute
                {
                    string constValue = constituent.ToString()?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(constValue))
                        return constValue;
                }

                if (dataSet.Attributes.TryGetValue(1, out var tsType))  // TSTYPE attribute
                {
                    string type = tsType.ToString()?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(type))
                        return ConvertTsTypeToVariable(type);
                }

                if (!string.IsNullOrEmpty(dataSet.TimeSeriesType))
                {
                    return dataSet.TimeSeriesType;
                }

                if (dataSet.Attributes.TryGetValue(83, out var paramName))  // Parameter name
                {
                    string param = paramName.ToString()?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(param))
                        return param;
                }

                // Default to DATA if no specific parameter found
                return "DATA";
            }
            catch
            {
                return "DATA";
            }
        }

        /// <summary>
        /// Convert time series type code to variable name
        /// </summary>
        /// <param name="tsType">Time series type</param>
        /// <returns>Variable name</returns>
        private static string ConvertTsTypeToVariable(string tsType)
        {
            return tsType.ToUpper() switch
            {
                "1" or "FLOW" => "FLOW",
                "2" or "PREC" => "PEVT",  // Potential evapotranspiration often called PEVT
                "3" or "EVAP" => "EVAP",
                "4" or "TEMP" => "TEMP",
                "5" or "CONC" => "CONC",
                "6" or "LOAD" => "LOAD",
                "7" or "STAGE" => "STAGE",
                "8" or "QUAL" => "QUAL",
                "STREAMFLOW" => "FLOW",
                "PRECIPITATION" => "PREC",
                "TEMPERATURE" => "TEMP",
                "EVAPORATION" => "EVAP",
                "POTENTIAL_ET" => "PEVT",
                "PET" => "PEVT",
                _ => tsType.ToUpper()
            };
        }

        #endregion

        #region Dataset Management

        /// <summary>
        /// F90_INFREE - Find free dataset number
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="datasetType">Dataset type</param>
        /// <param name="startDsn">Starting dataset number</param>
        /// <param name="increment">Increment</param>
        /// <param name="freeDsn">Output free dataset number</param>
        /// <param name="returnCode">Return code</param>
        public static void F90_INFREE(int wdmUnit, int datasetType, int startDsn, int increment,
            out int freeDsn, out int returnCode)
        {
            freeDsn = 0;
            returnCode = 0;
            
            try
            {
                var wdmFile = WdmOperations.GetWdmFileInfo(wdmUnit);
                if (wdmFile == null)
                {
                    returnCode = -1;
                    return;
                }

                int currentDsn = startDsn;
                while (currentDsn <= 32000) // Reasonable upper limit
                {
                    if (!wdmFile.DataSets.ContainsKey(currentDsn))
                    {
                        freeDsn = currentDsn;
                        return;
                    }
                    currentDsn += increment;
                }

                returnCode = -1; // No free dataset found
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error finding free dataset: {ex.Message}");
                returnCode = -1;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Fill string field from parsed data
        /// </summary>
        /// <param name="fields">Parsed fields</param>
        /// <param name="fieldIndex">Field index</param>
        /// <param name="output">Output integer array</param>
        /// <param name="maxLength">Maximum length</param>
        private static void FillStringField(string[] fields, int fieldIndex, int[] output, int maxLength)
        {
            try
            {
                string value = "";
                if (fieldIndex < fields.Length)
                    value = fields[fieldIndex];

                DataConversionUtilities.StringToIntArray(value, output, maxLength);
            }
            catch
            {
                DataConversionUtilities.InitializeIntArray(output, 32); // Fill with spaces
            }
        }

        /// <summary>
        /// Find available dataset number
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <returns>Available dataset number</returns>
        private static int FindAvailableDatasetNumber(int wdmUnit)
        {
            var wdmFile = WdmOperations.GetWdmFileInfo(wdmUnit);
            if (wdmFile == null) return 1;

            for (int dsn = 1; dsn <= 32000; dsn++)
            {
                if (!wdmFile.DataSets.ContainsKey(dsn))
                    return dsn;
            }

            return 1; // Fallback
        }

        /// <summary>
        /// Get status of all open streams
        /// </summary>
        /// <returns>Dictionary of unit numbers and stream status</returns>
        public static Dictionary<int, string> GetStreamStatus()
        {
            var status = new Dictionary<int, string>();
            
            foreach (var kvp in _inputStreams)
            {
                status[kvp.Key] = $"Input Stream (Open: {!kvp.Value.EndOfStream})";
            }
            
            foreach (var kvp in _outputStreams)
            {
                status[kvp.Key] = "Output Stream (Open)";
            }
            
            return status;
        }

        /// <summary>
        /// Close all open streams
        /// </summary>
        public static void CloseAllStreams()
        {
            try
            {
                var inputUnits = new List<int>(_inputStreams.Keys);
                foreach (var unit in inputUnits)
                {
                    F90_WATCLO(unit);
                }

                var outputUnits = new List<int>(_outputStreams.Keys);
                foreach (var unit in outputUnits)
                {
                    if (_outputStreams.TryGetValue(unit, out var writer))
                    {
                        writer.Close();
                        writer.Dispose();
                        _outputStreams.Remove(unit);
                    }
                }
                
                HassEntFunctions.LogMsg("All file streams closed");
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error closing streams: {ex.Message}");
            }
        }

        /// <summary>
        /// Convert FORTRAN format specification to .NET format string
        /// </summary>
        /// <param name="fortranFormat">FORTRAN format like F10.2, E12.4, I5</param>
        /// <returns>.NET format string</returns>
        private static string ConvertFortranFormatToNet(string fortranFormat)
        {
            if (string.IsNullOrEmpty(fortranFormat))
                return "F3";

            try
            {
                string format = fortranFormat.Trim().ToUpper();
                
                // Handle F format (fixed point decimal)
                if (format.StartsWith("F"))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(format, @"F(\d+)\.(\d+)");
                    if (match.Success)
                    {
                        int decimalPlaces = int.Parse(match.Groups[2].Value);
                        return $"F{decimalPlaces}";
                    }
                    else if (format.StartsWith("F"))
                    {
                        // Simple F format without specification
                        return "F2";
                    }
                }
                
                // Handle E format (exponential/scientific)
                else if (format.StartsWith("E"))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(format, @"E(\d+)\.(\d+)");
                    if (match.Success)
                    {
                        int decimalPlaces = int.Parse(match.Groups[2].Value);
                        return $"E{decimalPlaces}";
                    }
                    else
                    {
                        return "E2";
                    }
                }
                
                // Handle I format (integer)
                else if (format.StartsWith("I"))
                {
                    return "F0"; // Integer display as fixed point with 0 decimals
                }
                
                // Handle G format (general)
                else if (format.StartsWith("G"))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(format, @"G(\d+)\.(\d+)");
                    if (match.Success)
                    {
                        int decimalPlaces = int.Parse(match.Groups[2].Value);
                        return $"G{decimalPlaces}";
                    }
                    else
                    {
                        return "G";
                    }
                }
                
                // Default fallback
                return "F3";
            }
            catch
            {
                return "F3"; // Safe fallback
            }
        }

        #endregion
    }
}