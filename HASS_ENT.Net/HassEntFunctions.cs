using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;

namespace HASS_ENT.Net
{
    /// <summary>
    /// C# implementation of HASS_ENT FORTRAN functions
    /// Provides core functionality for hydrologic modeling and data management
    /// </summary>
    public static class HassEntFunctions
    {
        private static readonly Dictionary<int, FileStream> _openFiles = new();
        private static readonly Dictionary<string, int> _fileUnitMap = new();
        private static int _nextUnit = 101;
        private static bool _logWriteFlag = false;
        private static string _errorFileName = "ERROR.FIL";

        #region Logging Functions

        /// <summary>
        /// F90_MSG - Log a message to the error file
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void F90_MSG(string message)
        {
            LogMsg(message);
        }

        /// <summary>
        /// F90_W99OPN - Open log file for writing
        /// </summary>
        public static void F90_W99OPN()
        {
            LogMsg("WRITE");
        }

        /// <summary>
        /// F90_W99CLO - Close log file
        /// </summary>
        public static void F90_W99CLO()
        {
            LogMsg("CLOSE");
        }

        /// <summary>
        /// LOG_MSG - Internal logging function - make public for library use
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void LogMsg(string message)
        {
            try
            {
                string timeStamp = DateTime.Now.ToString("HH:mm:ss.fff") + " : ";
                bool fileExists = File.Exists(_errorFileName);
                
                if (message.Trim() == "WRITE")
                {
                    _logWriteFlag = true;
                }

                if (message.Trim() == "OPEN" || message.Trim() == "WRITE")
                {
                    using (var writer = new StreamWriter(_errorFileName, true))
                    {
                        writer.WriteLine($"{timeStamp}LOG_MSG:ERROR.FIL OPENED");
                    }
                }
                else if (message.Trim() == "CLOSE")
                {
                    if (File.Exists(_errorFileName))
                    {
                        using (var writer = new StreamWriter(_errorFileName, true))
                        {
                            writer.WriteLine($"{timeStamp}LOG_MSG:ERROR.FIL CLOSING");
                        }
                    }
                    _logWriteFlag = false;
                }
                else if (_logWriteFlag && !string.IsNullOrEmpty(message))
                {
                    using (var writer = new StreamWriter(_errorFileName, true))
                    {
                        writer.WriteLine($"{timeStamp}{message.Trim()}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LogMsg: {ex.Message}");
            }
        }

        #endregion

        #region File Operations

        /// <summary>
        /// F90_WDMOPN - Open a WDM file
        /// </summary>
        /// <param name="unit">File unit number</param>
        /// <param name="fileName">File name</param>
        /// <returns>0 if successful, 1 if already open, -1 if error</returns>
        public static int F90_WDMOPN(int unit, string fileName)
        {
            try
            {
                if (_openFiles.ContainsKey(unit))
                {
                    return 1; // Already open
                }

                FileStream fileStream;
                
                // Try to open with read-write first, then fall back to read-only
                // Use FileShare.ReadWrite to allow other processes to access the file
                try
                {
                    fileStream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    LogMsg($"F90_WDMOPN: Opened {fileName} with ReadWrite access");
                }
                catch (UnauthorizedAccessException)
                {
                    // File might be read-only, try read-only access
                    LogMsg($"F90_WDMOPN: ReadWrite access failed for {fileName}, trying ReadOnly");
                    fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                }
                catch (IOException ex)
                {
                    // File might be in use, try read-only access with shared mode
                    LogMsg($"F90_WDMOPN: ReadWrite access failed for {fileName} (IOException: {ex.Message}), trying ReadOnly with shared access");
                    try
                    {
                        fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    }
                    catch (IOException)
                    {
                        // Last resort: try with delete sharing as well
                        LogMsg($"F90_WDMOPN: Trying ReadOnly with full sharing for {fileName}");
                        fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                    }
                }

                _openFiles[unit] = fileStream;
                _fileUnitMap[fileName] = unit;
                
                LogMsg($"F90_WDMOPN: Successfully opened {fileName} on unit {unit} (CanWrite: {fileStream.CanWrite})");
                return 0; // Success
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening {unit} {fileName}: {ex.Message}");
                LogMsg($"F90_WDMOPN: Error opening {unit} {fileName}: {ex.Message}");
                return -1; // Error
            }
        }

        /// <summary>
        /// F90_WDMCLO - Close a WDM file
        /// </summary>
        /// <param name="unit">File unit number</param>
        /// <returns>0 if successful, -1 if error</returns>
        public static int F90_WDMCLO(int unit)
        {
            try
            {
                if (_openFiles.TryGetValue(unit, out var fileStream))
                {
                    fileStream.Close();
                    fileStream.Dispose();
                    _openFiles.Remove(unit);
                    
                    // Remove from file unit map
                    var toRemove = new List<string>();
                    foreach (var kvp in _fileUnitMap)
                    {
                        if (kvp.Value == unit)
                            toRemove.Add(kvp.Key);
                    }
                    foreach (var key in toRemove)
                        _fileUnitMap.Remove(key);
                }
                return 0;
            }
            catch (Exception ex)
            {
                LogMsg($"Error code {ex.HResult} closing unit {unit}");
                return -1;
            }
        }

        /// <summary>
        /// F90_WDBOPN - Open WDM database file
        /// </summary>
        /// <param name="rwFlag">Read/write flag (1=readonly, other=read/write)</param>
        /// <param name="wdName">WDM file name</param>
        /// <returns>File unit number if successful, 0 if error</returns>
        public static int F90_WDBOPN(int rwFlag, string wdName)
        {
            try
            {
                int wdmUnit;
                
                if (rwFlag == 1)
                {
                    // Read only, assign special number
                    wdmUnit = InquireName(wdName, 100);
                }
                else
                {
                    wdmUnit = InquireName(wdName, 0);
                }

                wdmUnit = GetWdmFun(wdmUnit);

                // Open the file at the OS level
                var result = F90_WDMOPN(wdmUnit, wdName);
                if (result != 0)
                {
                    LogMsg($"HASS_ENT:F90_WDBOPN:RETCOD,WDMSFL: {result} {wdmUnit}");
                    return 0;
                }

                // Create or register the WDM file with WdmOperations
                // This creates the WdmFileInfo object that tracks datasets
                WdmOperations.RegisterWdmFile(wdmUnit, wdName, rwFlag == 1);

                LogMsg($"HASS_ENT:F90_WDBOPN:RETCOD,WDMSFL: 0 {wdmUnit}");
                return wdmUnit;
            }
            catch (Exception ex)
            {
                LogMsg($"Error in F90_WDBOPN: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// F90_INQNAM - Check if a file name is open
        /// </summary>
        /// <param name="fileName">File name to check</param>
        /// <returns>Unit number if open, 0 if not open</returns>
        public static int F90_INQNAM(string fileName)
        {
            if (_fileUnitMap.TryGetValue(fileName, out int unit))
            {
                return unit;
            }
            return 0;
        }

        /// <summary>
        /// F90_WDFLCL - Close WDM file
        /// </summary>
        /// <param name="wdmUnit">WDM unit number</param>
        /// <returns>Return code</returns>
        public static int F90_WDFLCL(int wdmUnit)
        {
            try
            {
                int result = 0;
                
                // Clean up WdmOperations level first
                var wdmInfo = WdmOperations.GetWdmFileInfo(wdmUnit);
                if (wdmInfo != null)
                {
                    // Clear all datasets for this unit
                    wdmInfo.DataSets.Clear();
                    LogMsg($"F90_WDFLCL: Cleared {wdmInfo.DataSets.Count} datasets for unit {wdmUnit}");
                }

                // Close at file system level
                if (_openFiles.ContainsKey(wdmUnit))
                {
                    result = F90_WDMCLO(wdmUnit);
                    LogMsg($"F90_WDFLCL: Closed file system handle for unit {wdmUnit}, result: {result}");
                }
                else
                {
                    result = -255; // Not open, can't close it
                    LogMsg($"F90_WDFLCL: Unit {wdmUnit} was not open at file system level");
                }

                return result;
            }
            catch (Exception ex)
            {
                LogMsg($"Error in F90_WDFLCL: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// INQUIRE_NAME - Get available unit number for file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="funDef">Suggested unit number</param>
        /// <returns>Available unit number</returns>
        private static int InquireName(string fileName, int funDef)
        {
            if (_fileUnitMap.TryGetValue(fileName, out int existingUnit))
            {
                return existingUnit;
            }

            int funTry = funDef <= 0 ? _nextUnit : funDef;
            
            while (_openFiles.ContainsKey(funTry))
            {
                funTry++;
            }

            if (funDef < 0)
            {
                _nextUnit++;
            }

            return funTry;
        }

        /// <summary>
        /// GET_WDM_FUN - Get next available WDM unit number
        /// </summary>
        /// <param name="wdmUnit">Suggested unit number</param>
        /// <returns>Available unit number</returns>
        private static int GetWdmFun(int wdmUnit)
        {
            int nextWdm = wdmUnit >= 100 ? wdmUnit : 101;
            
            while (_openFiles.ContainsKey(nextWdm))
            {
                nextWdm++;
            }
            
            return nextWdm;
        }

        #endregion

        #region Date/Time Utilities

        /// <summary>
        /// F90_DAYMON - Get number of days in a month
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="month">Month</param>
        /// <returns>Number of days in the month</returns>
        public static int F90_DAYMON(int year, int month)
        {
            try
            {
                return DateTime.DaysInMonth(year, month);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// F90_TIMCHK - Compare two dates
        /// </summary>
        /// <param name="date1">First date array [year, month, day, hour, minute, second]</param>
        /// <param name="date2">Second date array [year, month, day, hour, minute, second]</param>
        /// <returns>-1 if date1 < date2, 0 if equal, 1 if date1 > date2</returns>
        public static int F90_TIMCHK(int[] date1, int[] date2)
        {
            try
            {
                var dt1 = new DateTime(date1[0], date1[1], date1[2], date1[3], date1[4], date1[5]);
                var dt2 = new DateTime(date2[0], date2[1], date2[2], date2[3], date2[4], date2[5]);
                
                return dt1.CompareTo(dt2);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// F90_JDMODY - Convert Julian day to month/day
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="julianDay">Julian day</param>
        /// <param name="month">Output month</param>
        /// <param name="day">Output day</param>
        public static void F90_JDMODY(int year, int julianDay, out int month, out int day)
        {
            try
            {
                var date = new DateTime(year, 1, 1).AddDays(julianDay - 1);
                month = date.Month;
                day = date.Day;
            }
            catch
            {
                month = 1;
                day = 1;
            }
        }

        /// <summary>
        /// F90_TIMADD - Add time to a date
        /// </summary>
        /// <param name="date1">Input date [year, month, day, hour, minute, second]</param>
        /// <param name="tCode">Time code (1=sec, 2=min, 3=hour, 4=day, 5=month, 6=year)</param>
        /// <param name="tStep">Time step</param>
        /// <param name="nVals">Number of values to add</param>
        /// <param name="date2">Output date</param>
        public static void F90_TIMADD(int[] date1, int tCode, int tStep, int nVals, int[] date2)
        {
            try
            {
                var dt = new DateTime(date1[0], date1[1], date1[2], date1[3], date1[4], date1[5]);
                int totalIncrement = tStep * nVals;

                switch (tCode)
                {
                    case 1: // seconds
                        dt = dt.AddSeconds(totalIncrement);
                        break;
                    case 2: // minutes
                        dt = dt.AddMinutes(totalIncrement);
                        break;
                    case 3: // hours
                        dt = dt.AddHours(totalIncrement);
                        break;
                    case 4: // days
                        dt = dt.AddDays(totalIncrement);
                        break;
                    case 5: // months
                        dt = dt.AddMonths(totalIncrement);
                        break;
                    case 6: // years
                        dt = dt.AddYears(totalIncrement);
                        break;
                }

                date2[0] = dt.Year;
                date2[1] = dt.Month;
                date2[2] = dt.Day;
                date2[3] = dt.Hour;
                date2[4] = dt.Minute;
                date2[5] = dt.Second;
            }
            catch
            {
                Array.Copy(date1, date2, 6);
            }
        }

        /// <summary>
        /// F90_TIMDIF - Calculate time difference between two dates
        /// </summary>
        /// <param name="date1">First date</param>
        /// <param name="date2">Second date</param>
        /// <param name="tCode">Time code</param>
        /// <param name="tStep">Time step</param>
        /// <param name="nVals">Output number of time steps</param>
        public static void F90_TIMDIF(int[] date1, int[] date2, int tCode, int tStep, out int nVals)
        {
            try
            {
                var dt1 = new DateTime(date1[0], date1[1], date1[2], date1[3], date1[4], date1[5]);
                var dt2 = new DateTime(date2[0], date2[1], date2[2], date2[3], date2[4], date2[5]);
                
                var diff = dt2 - dt1;
                
                double totalUnits = tCode switch
                {
                    1 => diff.TotalSeconds,
                    2 => diff.TotalMinutes,
                    3 => diff.TotalHours,
                    4 => diff.TotalDays,
                    _ => diff.TotalDays
                };
                
                nVals = (int)(totalUnits / tStep);
            }
            catch
            {
                nVals = 0;
            }
        }

        #endregion

        #region Mathematical Utilities

        /// <summary>
        /// F90_SCALIT - Scale values for plotting
        /// </summary>
        /// <param name="iType">Scale type</param>
        /// <param name="dataMin">Data minimum</param>
        /// <param name="dataMax">Data maximum</param>
        /// <param name="plotMin">Output plot minimum</param>
        /// <param name="plotMax">Output plot maximum</param>
        public static void F90_SCALIT(int iType, float dataMin, float dataMax, out float plotMin, out float plotMax)
        {
            try
            {
                // Simple scaling implementation
                float range = dataMax - dataMin;
                float margin = range * 0.1f; // 10% margin
                
                plotMin = dataMin - margin;
                plotMax = dataMax + margin;
                
                if (iType == 2 && plotMin < 1.0f)
                {
                    plotMin = 1.0f;
                }
            }
            catch
            {
                plotMin = dataMin;
                plotMax = dataMax;
            }
        }

        /// <summary>
        /// F90_ASRTRP - Sort real array
        /// </summary>
        /// <param name="values">Array to sort</param>
        public static void F90_ASRTRP(float[] values)
        {
            try
            {
                Array.Sort(values);
            }
            catch (Exception ex)
            {
                LogMsg($"Error in F90_ASRTRP: {ex.Message}");
            }
        }

        /// <summary>
        /// F90_DECPRC - Round value to specified precision
        /// </summary>
        /// <param name="sigDig">Significant digits</param>
        /// <param name="decPla">Decimal places</param>
        /// <param name="value">Value to round (modified in place)</param>
        public static void F90_DECPRC(int sigDig, int decPla, ref float value)
        {
            try
            {
                value = (float)Math.Round(value, decPla);
            }
            catch
            {
                // Value unchanged if error
            }
        }

        #endregion

        #region Utility Functions

        /// <summary>
        /// F90_PUTOLV - Set output level
        /// </summary>
        /// <param name="outLevel">Output level to set</param>
        public static void F90_PUTOLV(int outLevel)
        {
            LoggingService.LogInfo($"Output level set to: {outLevel}");
        }

        /// <summary>
        /// F90_WDBFIN - Finalize WDM operations
        /// </summary>
        public static void F90_WDBFIN()
        {
            try
            {
                // Get list of open units before clearing
                var openUnits = new List<int>(_openFiles.Keys);
                
                // Close all open files at both levels
                foreach (var unit in openUnits)
                {
                    // Close WdmOperations level
                    var wdmInfo = WdmOperations.GetWdmFileInfo(unit);
                    if (wdmInfo != null)
                    {
                        wdmInfo.DataSets.Clear();
                        LogMsg($"F90_WDBFIN: Cleared datasets for unit {unit}");
                    }
                    
                    // Close file system level
                    F90_WDMCLO(unit);
                }
                
                // Clear collections
                _openFiles.Clear();
                _fileUnitMap.Clear();
                _nextUnit = 101;
                
                LoggingService.LogInfo($"WDM operations finalized - closed {openUnits.Count} files");
                LogMsg($"F90_WDBFIN: Finalized WDM operations, closed {openUnits.Count} files");
            }
            catch (Exception ex)
            {
                LogMsg($"Error in F90_WDBFIN: {ex.Message}");
            }
        }

        /// <summary>
        /// Get status of all open files
        /// </summary>
        /// <returns>Dictionary of unit numbers and their file names</returns>
        public static Dictionary<int, string> GetOpenFiles()
        {
            var result = new Dictionary<int, string>();
            foreach (var kvp in _fileUnitMap)
            {
                result[kvp.Value] = kvp.Key;
            }
            return result;
        }

        #endregion

        #region Missing FORTRAN Functions

        /// <summary>
        /// F90_WMSGTW_XX - Write message table (placeholder implementation)
        /// </summary>
        /// <param name="messUnit">Message unit</param>
        /// <param name="tableId">Table ID</param>
        public static void F90_WMSGTW_XX(int messUnit, int tableId)
        {
            try
            {
                LogMsg($"F90_WMSGTW_XX: Writing message table {tableId} to unit {messUnit}");
                // Placeholder implementation - in real FORTRAN this would write table data
            }
            catch (Exception ex)
            {
                LogMsg($"Error in F90_WMSGTW_XX: {ex.Message}");
            }
        }

        /// <summary>
        /// F90_WMSGTT_XX - Get message table text
        /// </summary>
        /// <param name="wdmUnit">WDM unit number</param>
        /// <param name="dsn">Dataset number</param>
        /// <param name="groupNum">Group number</param>
        /// <param name="initFlag">Initialization flag</param>
        /// <param name="outputLength">Output length</param>
        /// <param name="continuation">Continuation flag</param>
        /// <param name="outputBuffer">Output buffer as integer array</param>
        public static void F90_WMSGTT_XX(int wdmUnit, int dsn, int groupNum, int initFlag, 
            ref int outputLength, out int continuation, int[] outputBuffer)
        {
            try
            {
                continuation = 0;
                
                if (outputLength > 256) 
                    outputLength = 256;

                // Simulate getting message text from WDM
                string messageText = $"Message from DSN {dsn}, Group {groupNum}";
                
                // Convert to integer array
                int maxLength = Math.Min(outputLength, Math.Min(outputBuffer.Length, messageText.Length));
                for (int i = 0; i < maxLength; i++)
                {
                    outputBuffer[i] = (int)messageText[i];
                }
                
                // Fill remaining with spaces
                for (int i = messageText.Length; i < Math.Min(outputLength, outputBuffer.Length); i++)
                {
                    outputBuffer[i] = 32; // Space
                }

                LogMsg($"F90_WMSGTT_XX: Retrieved message for DSN {dsn}");
            }
            catch (Exception ex)
            {
                continuation = -1;
                LogMsg($"Error in F90_WMSGTT_XX: {ex.Message}");
            }
        }

        /// <summary>
        /// F90_WMSGTH - Get message table header
        /// </summary>
        /// <param name="messUnit">Message unit</param>
        /// <param name="tableId">Table ID</param>
        public static void F90_WMSGTH(int messUnit, int tableId)
        {
            try
            {
                LogMsg($"F90_WMSGTH: Getting message table header for table {tableId} from unit {messUnit}");
                // Placeholder implementation - in real FORTRAN this would read table header
            }
            catch (Exception ex)
            {
                LogMsg($"Error in F90_WMSGTH: {ex.Message}");
            }
        }

        /// <summary>
        /// F90_GTNXKW_XX - Get next keyword
        /// </summary>
        /// <param name="inputUnit">Input unit</param>
        /// <param name="outputUnit">Output unit</param>
        /// <param name="keyword">Keyword as integer array</param>
        /// <param name="keywordLength">Keyword length</param>
        /// <param name="lineNumber">Line number</param>
        /// <param name="errorCode">Error code</param>
        public static void F90_GTNXKW_XX(int inputUnit, int outputUnit, int[] keyword, 
            out int keywordLength, out int lineNumber, out int errorCode)
        {
            try
            {
                keywordLength = 0;
                lineNumber = 0;
                errorCode = 0;

                // Simulate reading next keyword from input
                string nextKeyword = "KEYWORD"; // Placeholder
                
                keywordLength = Math.Min(nextKeyword.Length, keyword.Length);
                
                for (int i = 0; i < keywordLength; i++)
                {
                    keyword[i] = (int)nextKeyword[i];
                }
                
                // Fill remaining with spaces
                for (int i = keywordLength; i < keyword.Length; i++)
                {
                    keyword[i] = 32; // Space
                }

                lineNumber = 1; // Placeholder line number
                
                LogMsg($"F90_GTNXKW_XX: Retrieved keyword '{nextKeyword}'");
            }
            catch (Exception ex)
            {
                keywordLength = 0;
                lineNumber = 0;
                errorCode = -1;
                LogMsg($"Error in F90_GTNXKW_XX: {ex.Message}");
            }
        }

        /// <summary>
        /// F90_XTINFO_XX - Get extended information
        /// </summary>
        /// <param name="infoType">Information type</param>
        /// <param name="param1">Parameter 1</param>
        /// <param name="param2">Parameter 2</param>
        /// <param name="param3">Parameter 3</param>
        /// <param name="param4">Parameter 4</param>
        /// <param name="param5">Parameter 5</param>
        /// <param name="param6">Parameter 6</param>
        /// <param name="param7">Parameter 7</param>
        /// <param name="param8">Parameter 8</param>
        /// <param name="param9">Parameter 9</param>
        /// <param name="param10">Parameter 10</param>
        /// <param name="param11">Parameter 11</param>
        /// <param name="realParam1">Real parameter 1</param>
        /// <param name="realParam2">Real parameter 2</param>
        /// <param name="realParam3">Real parameter 3</param>
        /// <param name="textParam1">Text parameter 1 as integer array</param>
        /// <param name="textParam2">Text parameter 2 as integer array</param>
        /// <param name="textParam3">Text parameter 3 as integer array</param>
        /// <param name="textParam4">Text parameter 4 as integer array</param>
        /// <param name="textParam5">Text parameter 5 as integer array</param>
        /// <param name="textParam6">Text parameter 6 as integer array</param>
        public static void F90_XTINFO_XX(int infoType, int param1, int param2, int param3, int param4, 
            int param5, int param6, int param7, int param8, int param9, int param10, int param11,
            float realParam1, float realParam2, float realParam3,
            int[] textParam1, int[] textParam2, int[] textParam3, int[] textParam4, int[] textParam5, int[] textParam6)
        {
            try
            {
                LogMsg($"F90_XTINFO_XX: Getting extended info type {infoType}");
                
                // Simulate filling extended information based on type
                switch (infoType)
                {
                    case 1:
                        // System information
                        DataConversionUtilities.StringToIntArray("SYSTEM", textParam1);
                        DataConversionUtilities.StringToIntArray("INFO", textParam2);
                        break;
                    case 2:
                        // Version information
                        DataConversionUtilities.StringToIntArray("VERSION", textParam1);
                        DataConversionUtilities.StringToIntArray("1.0", textParam2);
                        break;
                    default:
                        // Default information
                        DataConversionUtilities.StringToIntArray("UNKNOWN", textParam1);
                        break;
                }
                
                LogMsg($"F90_XTINFO_XX: Extended info type {infoType} processed");
            }
            catch (Exception ex)
            {
                LogMsg($"Error in F90_XTINFO_XX: {ex.Message}");
            }
        }

        /// <summary>
        /// F90_MSGUNIT - Set message unit
        /// </summary>
        /// <param name="messageUnit">Message unit number</param>
        public static void F90_MSGUNIT(int messageUnit)
        {
            try
            {
                LogMsg($"F90_MSGUNIT: Setting message unit to {messageUnit}");
                // In FORTRAN this would set the global message unit for output
                // For C# implementation, we'll just log the action
            }
            catch (Exception ex)
            {
                LogMsg($"Error in F90_MSGUNIT: {ex.Message}");
            }
        }

        #endregion
    }
}