using System;
using System.IO;
using System.Collections.Generic;

namespace HASS_ENT.Net
{
    /// <summary>
    /// File I/O utilities providing C# equivalents of FORTRAN file operations
    /// Includes functions for opening, closing, and managing file units
    /// </summary>
    public static class FileUtilities
    {
        private static readonly Dictionary<int, FileStream> _openFiles = new Dictionary<int, FileStream>();
        private static int _nextFileUnit = 101; // Start from FORTRAN base unit number
        
        /// <summary>
        /// Get the next available file unit number - equivalent to INQUIRE_NAME
        /// </summary>
        /// <param name="preferredUnit">Preferred unit number, 0 for auto-assign</param>
        /// <returns>Available unit number</returns>
        public static int GetAvailableUnit(int preferredUnit = 0)
        {
            if (preferredUnit > 0 && !_openFiles.ContainsKey(preferredUnit))
            {
                return preferredUnit;
            }
            
            // Find next available unit starting from base
            int unit = _nextFileUnit;
            while (_openFiles.ContainsKey(unit))
            {
                unit++;
            }
            _nextFileUnit = unit + 1;
            
            LoggingService.LogDebug($"FileUtilities: Assigned unit {unit}");
            return unit;
        }
        
        /// <summary>
        /// Open a file for reading/writing - equivalent to FORTRAN OPEN statements
        /// </summary>
        /// <param name="fileName">Name of file to open</param>
        /// <param name="mode">File access mode</param>
        /// <param name="unit">File unit number (0 for auto-assign)</param>
        /// <returns>File unit number if successful, negative error code if failed</returns>
        public static int OpenFile(string fileName, FileAccess mode, int unit = 0)
        {
            try
            {
                if (unit <= 0)
                {
                    unit = GetAvailableUnit();
                }
                
                if (_openFiles.ContainsKey(unit))
                {
                    LoggingService.LogWarning($"Unit {unit} already open, closing first");
                    CloseFile(unit);
                }
                
                FileMode fileMode = mode == FileAccess.Read ? FileMode.Open : FileMode.OpenOrCreate;
                var stream = new FileStream(fileName, fileMode, mode, FileShare.Read);
                _openFiles[unit] = stream;
                
                LoggingService.LogMessage($"Opened file: {fileName} on unit {unit}");
                return unit;
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error opening file {fileName}: {ex.Message}");
                return -1; // Error code like FORTRAN
            }
        }
        
        /// <summary>
        /// Close a file unit - equivalent to FORTRAN CLOSE
        /// </summary>
        /// <param name="unit">File unit number to close</param>
        /// <returns>0 if successful, error code otherwise</returns>
        public static int CloseFile(int unit)
        {
            try
            {
                if (_openFiles.TryGetValue(unit, out FileStream stream))
                {
                    stream.Close();
                    stream.Dispose();
                    _openFiles.Remove(unit);
                    LoggingService.LogMessage($"Closed file unit {unit}");
                    return 0;
                }
                else
                {
                    LoggingService.LogWarning($"Unit {unit} was not open");
                    return -1;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error closing unit {unit}: {ex.Message}");
                return -1;
            }
        }
        
        /// <summary>
        /// Check if a file unit is open - equivalent to FORTRAN INQUIRE
        /// </summary>
        /// <param name="unit">File unit number</param>
        /// <returns>True if unit is open</returns>
        public static bool IsUnitOpen(int unit)
        {
            return _openFiles.ContainsKey(unit);
        }
        
        /// <summary>
        /// Get the stream for a file unit
        /// </summary>
        /// <param name="unit">File unit number</param>
        /// <returns>FileStream or null if not open</returns>
        public static FileStream GetStream(int unit)
        {
            return _openFiles.TryGetValue(unit, out FileStream stream) ? stream : null;
        }
        
        /// <summary>
        /// Read an array of integers from a file - equivalent to FORTRAN array reads
        /// </summary>
        /// <param name="unit">File unit</param>
        /// <param name="data">Array to read into</param>
        /// <returns>Number of values read, negative on error</returns>
        public static int ReadIntegerArray(int unit, int[] data)
        {
            try
            {
                var stream = GetStream(unit);
                if (stream == null) return -1;
                
                using var reader = new BinaryReader(stream, System.Text.Encoding.Default, leaveOpen: true);
                
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = reader.ReadInt32();
                }
                
                LoggingService.LogDebug($"Read {data.Length} integers from unit {unit}");
                return data.Length;
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error reading integer array from unit {unit}: {ex.Message}");
                return -1;
            }
        }
        
        /// <summary>
        /// Read an array of real numbers from a file - equivalent to FORTRAN real array reads
        /// </summary>
        /// <param name="unit">File unit</param>
        /// <param name="data">Array to read into</param>
        /// <returns>Number of values read, negative on error</returns>
        public static int ReadRealArray(int unit, float[] data)
        {
            try
            {
                var stream = GetStream(unit);
                if (stream == null) return -1;
                
                using var reader = new BinaryReader(stream, System.Text.Encoding.Default, leaveOpen: true);
                
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = reader.ReadSingle();
                }
                
                LoggingService.LogDebug($"Read {data.Length} real values from unit {unit}");
                return data.Length;
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error reading real array from unit {unit}: {ex.Message}");
                return -1;
            }
        }
        
        /// <summary>
        /// Write an array of real numbers to a file - equivalent to FORTRAN real array writes
        /// </summary>
        /// <param name="unit">File unit</param>
        /// <param name="data">Array to write</param>
        /// <returns>Number of values written, negative on error</returns>
        public static int WriteRealArray(int unit, float[] data)
        {
            try
            {
                var stream = GetStream(unit);
                if (stream == null) return -1;
                
                using var writer = new BinaryWriter(stream, System.Text.Encoding.Default, leaveOpen: true);
                
                foreach (float value in data)
                {
                    writer.Write(value);
                }
                
                LoggingService.LogDebug($"Wrote {data.Length} real values to unit {unit}");
                return data.Length;
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error writing real array to unit {unit}: {ex.Message}");
                return -1;
            }
        }
        
        /// <summary>
        /// Get information about a file by name - equivalent to FORTRAN INQUIRE by file name
        /// </summary>
        /// <param name="fileName">Name of file to inquire about</param>
        /// <returns>File unit number if open, 0 if file exists but not open, -1 if not exists</returns>
        public static int InquireByName(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName)) return -1;
                
                // Check if file is currently open
                foreach (var kvp in _openFiles)
                {
                    var stream = kvp.Value;
                    if (stream is FileStream fs && fs.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                    {
                        return kvp.Key;
                    }
                }
                
                // File not open, check if it exists
                return File.Exists(fileName) ? 0 : -1;
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error inquiring about file {fileName}: {ex.Message}");
                return -1;
            }
        }
        
        /// <summary>
        /// Close all open files - cleanup function
        /// </summary>
        public static void CloseAllFiles()
        {
            LoggingService.LogMessage("Closing all open files");
            
            var units = new List<int>(_openFiles.Keys);
            foreach (int unit in units)
            {
                CloseFile(unit);
            }
        }
    }
}