using System;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Main entry point class for HASS_ENT.Net library
    /// Provides a unified interface to all HASS_ENT functionality
    /// </summary>
    public static class HassEntLibrary
    {
        private static bool _isInitialized = false;

        /// <summary>
        /// Initialize the HASS_ENT library
        /// </summary>
        /// <param name="enableLogging">Enable logging to file</param>
        /// <returns>True if initialization successful</returns>
        public static bool Initialize(bool enableLogging = true)
        {
            try
            {
                if (_isInitialized) return true;

                // Clear any existing file handles to ensure clean state
                HassEntFunctions.F90_WDBFIN();
                
                // Initialize logging
                LoggingService.LogInfo("Initializing HASS_ENT.Net library");
                
                if (enableLogging)
                {
                    HassEntFunctions.F90_W99OPN();
                    HassEntFunctions.F90_MSG("HASS_ENT.Net library initialized");
                }

                _isInitialized = true;
                LoggingService.LogInfo("HASS_ENT.Net library initialization completed");
                
                return true;
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Failed to initialize HASS_ENT library: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Shutdown the HASS_ENT library and clean up resources
        /// </summary>
        public static void Shutdown()
        {
            try
            {
                if (!_isInitialized) return;

                LoggingService.LogInfo("Shutting down HASS_ENT.Net library");
                
                // Close all WDM files
                HassEntFunctions.F90_WDBFIN();
                
                // Close all streams
                FileManagement.CloseAllStreams();
                
                // Close logging
                HassEntFunctions.F90_MSG("HASS_ENT.Net library shutdown");
                HassEntFunctions.F90_W99CLO();

                _isInitialized = false;
                
                LoggingService.LogInfo("HASS_ENT.Net library shutdown completed");
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error during HASS_ENT library shutdown: {ex.Message}");
            }
        }

        /// <summary>
        /// Get library version information
        /// </summary>
        /// <returns>Version string</returns>
        public static string GetVersion()
        {
            return "HASS_ENT.Net v1.0.0 - C# Implementation of HASS_ENT FORTRAN Functions";
        }

        /// <summary>
        /// Get library status
        /// </summary>
        /// <returns>Status information</returns>
        public static LibraryStatus GetStatus()
        {
            return new LibraryStatus
            {
                IsInitialized = _isInitialized,
                Version = GetVersion(),
                OpenWdmFiles = HassEntFunctions.GetOpenFiles(),
                OpenStreams = FileManagement.GetStreamStatus(),
                SystemInfo = InteropHelpers.GetSystemStatus(),
                Timestamp = DateTime.Now
            };
        }

        /// <summary>
        /// Library status information
        /// </summary>
        public class LibraryStatus
        {
            public bool IsInitialized { get; set; }
            public string Version { get; set; } = "";
            public System.Collections.Generic.Dictionary<int, string> OpenWdmFiles { get; set; } = new();
            public System.Collections.Generic.Dictionary<int, string> OpenStreams { get; set; } = new();
            public InteropHelpers.SystemStatus SystemInfo { get; set; } = new();
            public DateTime Timestamp { get; set; }

            public override string ToString()
            {
                return $"HASS_ENT.Net Status - Initialized: {IsInitialized}, " +
                       $"WDM Files: {OpenWdmFiles.Count}, Streams: {OpenStreams.Count}, " +
                       $"Time: {Timestamp:yyyy-MM-dd HH:mm:ss}";
            }
        }
    }
}
