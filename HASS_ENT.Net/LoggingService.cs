using System;
using System.IO;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Logging and message handling service equivalent to FORTRAN LOG_MSG and UPDATESTATUS functions
    /// Provides centralized logging with file output and status messaging
    /// </summary>
    public class LoggingService
    {
        private static LoggingService _instance;
        private static readonly object _lock = new object();
        
        private StreamWriter _logWriter;
        private string _logFilePath;
        private bool _isInitialized;
        
        public static LoggingService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new LoggingService();
                    }
                }
                return _instance;
            }
        }
        
        /// <summary>
        /// Initialize the logging service - equivalent to LOG_MSG('OPEN')
        /// </summary>
        /// <param name="logFilePath">Path to log file, defaults to "ERROR.FIL"</param>
        public static void Initialize(string logFilePath = "ERROR.FIL")
        {
            Instance.InitializeInternal(logFilePath);
        }
        
        private void InitializeInternal(string logFilePath)
        {
            if (_isInitialized) return;
            
            try
            {
                _logFilePath = logFilePath;
                
                // Open or create the log file (append mode like FORTRAN)
                _logWriter = new StreamWriter(_logFilePath, append: true)
                {
                    AutoFlush = true
                };
                
                LogMessage("LOG_MSG:ERROR.FIL OPENED");
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening log file {logFilePath}: {ex.Message}");
                // Fall back to console logging only
                _isInitialized = true;
            }
        }
        
        /// <summary>
        /// Log a message - equivalent to FORTRAN LOG_MSG function
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void LogMessage(string message)
        {
            Instance.LogMessageInternal(message);
        }
        
        /// <summary>
        /// Log an error message
        /// </summary>
        /// <param name="message">Error message</param>
        public static void LogError(string message)
        {
            Instance.LogMessageInternal($"ERROR: {message}");
        }

        /// <summary>
        /// Log an information message
        /// </summary>
        /// <param name="message">Information message</param>
        public static void LogInfo(string message)
        {
            Instance.LogMessageInternal($"INFO: {message}");
        }
        
        /// <summary>
        /// Log a warning message
        /// </summary>
        /// <param name="message">Warning message</param>
        public static void LogWarning(string message)
        {
            Instance.LogMessageInternal($"WARNING: {message}");
        }
        
        /// <summary>
        /// Log debug information
        /// </summary>
        /// <param name="message">Debug message</param>
        public static void LogDebug(string message)
        {
            Instance.LogMessageInternal($"DEBUG: {message}");
        }
        
        private void LogMessageInternal(string message)
        {
            if (!_isInitialized) return;
            
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string logEntry = $"{timestamp} : {message}";
            
            try
            {
                // Write to file if available
                _logWriter?.WriteLine(logEntry);
                
                // Also write to console for debugging
                Console.WriteLine(logEntry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to log: {ex.Message}");
                Console.WriteLine($"Original message: {logEntry}");
            }
        }
        
        /// <summary>
        /// Update status with progress information - equivalent to UPDATESTATUS
        /// </summary>
        /// <param name="statusType">Type of status update</param>
        /// <param name="message">Status message</param>
        public static void UpdateStatus(int statusType, string message)
        {
            Instance.UpdateStatusInternal(statusType, message);
        }
        
        private void UpdateStatusInternal(int statusType, string message)
        {
            string statusPrefix = statusType switch
            {
                1 => "STATUS",
                5 => "PROGRESS",
                7 => "DEBUG", 
                10 => "MSG",
                99 => "COMPLETE",
                _ => "INFO"
            };
            
            LogMessageInternal($"{statusPrefix}: {message}");
        }
        
        /// <summary>
        /// Enable or disable write mode - equivalent to LOG_MSG('WRITE')
        /// </summary>
        /// <param name="enabled">True to enable writing</param>
        public static void SetWriteMode(bool enabled)
        {
            if (enabled)
            {
                Instance.LogMessageInternal("LOG_MSG: Write mode enabled");
            }
        }
        
        /// <summary>
        /// Close the logging service - equivalent to LOG_MSG('CLOSE')
        /// </summary>
        public static void Shutdown()
        {
            Instance.ShutdownInternal();
        }
        
        private void ShutdownInternal()
        {
            if (!_isInitialized) return;
            
            try
            {
                LogMessage("LOG_MSG:ERROR.FIL CLOSING");
                _logWriter?.Close();
                _logWriter?.Dispose();
                _logWriter = null;
                _isInitialized = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing log file: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Check if a specific unit/file is open - equivalent to FORTRAN INQUIRE functionality
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <returns>True if file exists and is accessible</returns>
        public static bool IsFileOpen(string filePath)
        {
            try
            {
                return File.Exists(filePath) && !IsFileLocked(filePath);
            }
            catch
            {
                return false;
            }
        }
        
        private static bool IsFileLocked(string filePath)
        {
            try
            {
                using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                return false;
            }
            catch (IOException)
            {
                return true;
            }
        }
    }
}