using System;
using System.Runtime.InteropServices;
using System.Text;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Interoperability helpers for integration with external systems and legacy code
    /// Provides utilities for data conversion, marshalling, and system integration
    /// </summary>
    public static class InteropHelpers
    {
        /// <summary>
        /// Convert integer array to character string - equivalent to FORTRAN character array conversions
        /// </summary>
        /// <param name="intArray">Array of integers representing ASCII characters</param>
        /// <param name="length">Length to convert</param>
        /// <returns>Converted string</returns>
        public static string IntArrayToString(int[] intArray, int length = -1)
        {
            try
            {
                if (intArray == null) return "";
                
                int len = length < 0 ? intArray.Length : Math.Min(length, intArray.Length);
                var sb = new StringBuilder(len);
                
                for (int i = 0; i < len; i++)
                {
                    if (intArray[i] == 0) break; // Null terminator
                    sb.Append((char)intArray[i]);
                }
                
                return sb.ToString().TrimEnd();
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error converting int array to string: {ex.Message}");
                return "";
            }
        }
        
        /// <summary>
        /// Convert string to integer array - equivalent to FORTRAN character array packing
        /// </summary>
        /// <param name="text">String to convert</param>
        /// <param name="intArray">Output integer array</param>
        /// <param name="maxLength">Maximum length to convert</param>
        public static void StringToIntArray(string text, int[] intArray, int maxLength = -1)
        {
            try
            {
                if (intArray == null || string.IsNullOrEmpty(text)) return;
                
                int len = maxLength < 0 ? Math.Min(text.Length, intArray.Length) : Math.Min(maxLength, Math.Min(text.Length, intArray.Length));
                
                for (int i = 0; i < len; i++)
                {
                    intArray[i] = (int)text[i];
                }
                
                // Fill remaining with spaces (ASCII 32) like FORTRAN
                for (int i = len; i < intArray.Length && (maxLength < 0 || i < maxLength); i++)
                {
                    intArray[i] = 32; // Space character
                }
                
                LoggingService.LogDebug($"Converted string '{text}' to int array of length {len}");
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error converting string to int array: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Convert FORTRAN-style date array to .NET DateTime
        /// </summary>
        /// <param name="fortranDate">FORTRAN date array [year, month, day, hour, minute, second]</param>
        /// <returns>DateTime object</returns>
        public static DateTime FortranDateToDateTime(int[] fortranDate)
        {
            try
            {
                if (fortranDate == null || fortranDate.Length < 6)
                {
                    LoggingService.LogError("Invalid FORTRAN date array");
                    return DateTime.MinValue;
                }
                
                int year = fortranDate[0];
                int month = Math.Max(1, Math.Min(12, fortranDate[1]));
                int day = Math.Max(1, Math.Min(31, fortranDate[2]));
                int hour = Math.Max(0, Math.Min(23, fortranDate[3]));
                int minute = Math.Max(0, Math.Min(59, fortranDate[4]));
                int second = Math.Max(0, Math.Min(59, fortranDate[5]));
                
                return new DateTime(year, month, day, hour, minute, second);
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error converting FORTRAN date: {ex.Message}");
                return DateTime.MinValue;
            }
        }
        
        /// <summary>
        /// Convert .NET DateTime to FORTRAN-style date array
        /// </summary>
        /// <param name="dateTime">DateTime to convert</param>
        /// <param name="fortranDate">Output FORTRAN date array [year, month, day, hour, minute, second]</param>
        public static void DateTimeToFortranDate(DateTime dateTime, int[] fortranDate)
        {
            try
            {
                if (fortranDate == null || fortranDate.Length < 6)
                {
                    LoggingService.LogError("Invalid FORTRAN date array for output");
                    return;
                }
                
                fortranDate[0] = dateTime.Year;
                fortranDate[1] = dateTime.Month;
                fortranDate[2] = dateTime.Day;
                fortranDate[3] = dateTime.Hour;
                fortranDate[4] = dateTime.Minute;
                fortranDate[5] = dateTime.Second;
                
                LoggingService.LogDebug($"Converted DateTime {dateTime:yyyy-MM-dd HH:mm:ss} to FORTRAN date array");
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error converting DateTime to FORTRAN date: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Swap byte order for cross-platform binary compatibility
        /// </summary>
        /// <param name="value">32-bit integer to swap</param>
        /// <returns>Byte-swapped integer</returns>
        public static int SwapBytes(int value)
        {
            return (int)(((value & 0x000000FF) << 24) |
                         ((value & 0x0000FF00) << 8) |
                         ((value & 0x00FF0000) >> 8) |
                         ((value & 0xFF000000) >> 24));
        }
        
        /// <summary>
        /// Check system endianness for binary file compatibility
        /// </summary>
        /// <returns>True if system is little-endian</returns>
        public static bool IsLittleEndian()
        {
            return BitConverter.IsLittleEndian;
        }
        
        /// <summary>
        /// Get current system status for integration monitoring
        /// </summary>
        /// <returns>System status information</returns>
        public static SystemStatus GetSystemStatus()
        {
            try
            {
                return new SystemStatus
                {
                    AvailableMemory = GC.GetTotalMemory(false),
                    ProcessorCount = Environment.ProcessorCount,
                    IsLittleEndian = IsLittleEndian(),
                    OSVersion = Environment.OSVersion.ToString(),
                    CLRVersion = Environment.Version.ToString(),
                    WorkingDirectory = Environment.CurrentDirectory,
                    Timestamp = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error getting system status: {ex.Message}");
                return new SystemStatus();
            }
        }
        
        /// <summary>
        /// System status information structure
        /// </summary>
        public class SystemStatus
        {
            public long AvailableMemory { get; set; }
            public int ProcessorCount { get; set; }
            public bool IsLittleEndian { get; set; }
            public string OSVersion { get; set; } = "";
            public string CLRVersion { get; set; } = "";
            public string WorkingDirectory { get; set; } = "";
            public DateTime Timestamp { get; set; }
            
            public override string ToString()
            {
                return $"Memory: {AvailableMemory / 1024 / 1024} MB, CPUs: {ProcessorCount}, " +
                       $"OS: {OSVersion}, CLR: {CLRVersion}, Endian: {(IsLittleEndian ? "Little" : "Big")}";
            }
        }
    }
}