using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Error handling and validation utilities for HASS_ENT operations
    /// Provides comprehensive validation, error codes, and exception handling
    /// </summary>
    public static class ErrorHandling
    {
        #region Error Codes and Constants

        /// <summary>
        /// Standard HASS_ENT error codes
        /// </summary>
        public enum HassErrorCode
        {
            Success = 0,
            GeneralError = -1,
            FileNotFound = -2,
            InvalidParameter = -3,
            DatasetNotFound = -4,
            InsufficientData = -5,
            InvalidDateFormat = -6,
            UnitNotOpen = -7,
            DatasetAlreadyExists = -8,
            IncompatibleTimeStep = -9,
            MemoryError = -10,
            ReadError = -11,
            WriteError = -12,
            InvalidDataType = -13,
            OutOfRange = -14,
            ConversionError = -15,
            AttributeNotFound = -16,
            InvalidFormat = -17,
            NetworkSimplificationError = -18,
            TableGenerationError = -19,
            ReportError = -20
        }

        /// <summary>
        /// Error message descriptions
        /// </summary>
        private static readonly Dictionary<HassErrorCode, string> _errorMessages = new()
        {
            { HassErrorCode.Success, "Operation completed successfully" },
            { HassErrorCode.GeneralError, "General error occurred" },
            { HassErrorCode.FileNotFound, "File not found or cannot be accessed" },
            { HassErrorCode.InvalidParameter, "Invalid parameter provided" },
            { HassErrorCode.DatasetNotFound, "Dataset not found in WDM file" },
            { HassErrorCode.InsufficientData, "Insufficient data for operation" },
            { HassErrorCode.InvalidDateFormat, "Invalid date format or date range" },
            { HassErrorCode.UnitNotOpen, "File unit not open or invalid" },
            { HassErrorCode.DatasetAlreadyExists, "Dataset already exists" },
            { HassErrorCode.IncompatibleTimeStep, "Incompatible time step specification" },
            { HassErrorCode.MemoryError, "Memory allocation error" },
            { HassErrorCode.ReadError, "Error reading data" },
            { HassErrorCode.WriteError, "Error writing data" },
            { HassErrorCode.InvalidDataType, "Invalid data type for operation" },
            { HassErrorCode.OutOfRange, "Value out of valid range" },
            { HassErrorCode.ConversionError, "Data conversion error" },
            { HassErrorCode.AttributeNotFound, "Attribute not found" },
            { HassErrorCode.InvalidFormat, "Invalid data format" },
            { HassErrorCode.NetworkSimplificationError, "Network simplification error" },
            { HassErrorCode.TableGenerationError, "Table generation error" },
            { HassErrorCode.ReportError, "Report generation error" }
        };

        #endregion

        #region Error Handling Functions

        /// <summary>
        /// Get error message for error code
        /// </summary>
        /// <param name="errorCode">Error code</param>
        /// <returns>Error message</returns>
        public static string GetErrorMessage(HassErrorCode errorCode)
        {
            return _errorMessages.TryGetValue(errorCode, out string? message) ? message : "Unknown error";
        }

        /// <summary>
        /// Get error message for integer error code
        /// </summary>
        /// <param name="errorCode">Integer error code</param>
        /// <returns>Error message</returns>
        public static string GetErrorMessage(int errorCode)
        {
            if (Enum.IsDefined(typeof(HassErrorCode), errorCode))
            {
                return GetErrorMessage((HassErrorCode)errorCode);
            }
            return $"Unknown error code: {errorCode}";
        }

        /// <summary>
        /// Log error with context information
        /// </summary>
        /// <param name="errorCode">Error code</param>
        /// <param name="functionName">Function name where error occurred</param>
        /// <param name="additionalInfo">Additional error information</param>
        public static void LogError(HassErrorCode errorCode, string functionName, string? additionalInfo = null)
        {
            string errorMsg = $"{functionName}: {GetErrorMessage(errorCode)}";
            if (!string.IsNullOrEmpty(additionalInfo))
            {
                errorMsg += $" - {additionalInfo}";
            }
            
            HassEntFunctions.LogMsg($"ERROR: {errorMsg}");
            LoggingService.LogError(errorMsg);
        }

        /// <summary>
        /// Handle exception with logging
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="functionName">Function name</param>
        /// <param name="returnErrorCode">Whether to return error code</param>
        /// <returns>Error code if requested</returns>
        public static int HandleException(Exception ex, string functionName, bool returnErrorCode = true)
        {
            string errorMsg = $"{functionName}: Exception - {ex.Message}";
            HassEntFunctions.LogMsg($"EXCEPTION: {errorMsg}");
            LoggingService.LogError(errorMsg);
            
            return returnErrorCode ? (int)HassErrorCode.GeneralError : 0;
        }

        #endregion

        #region Validation Functions

        /// <summary>
        /// Validate WDM unit number
        /// </summary>
        /// <param name="wdmUnit">WDM unit number</param>
        /// <returns>True if valid</returns>
        public static bool ValidateWdmUnit(int wdmUnit)
        {
            if (wdmUnit <= 0 || wdmUnit > 9999)
            {
                LogError(HassErrorCode.InvalidParameter, nameof(ValidateWdmUnit), $"Invalid WDM unit: {wdmUnit}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validate dataset number
        /// </summary>
        /// <param name="dsn">Dataset number</param>
        /// <returns>True if valid</returns>
        public static bool ValidateDatasetNumber(int dsn)
        {
            if (dsn <= 0 || dsn > 32000)
            {
                LogError(HassErrorCode.InvalidParameter, nameof(ValidateDatasetNumber), $"Invalid DSN: {dsn}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validate date array
        /// </summary>
        /// <param name="dateArray">Date array [year, month, day, hour, minute, second]</param>
        /// <param name="parameterName">Parameter name for logging</param>
        /// <returns>True if valid</returns>
        public static bool ValidateDateArray(int[] dateArray, string parameterName = "dateArray")
        {
            if (dateArray == null || dateArray.Length < 6)
            {
                LogError(HassErrorCode.InvalidDateFormat, nameof(ValidateDateArray), $"{parameterName} is null or too short");
                return false;
            }

            try
            {
                // Validate year
                if (dateArray[0] < 1900 || dateArray[0] > 2100)
                {
                    LogError(HassErrorCode.InvalidDateFormat, nameof(ValidateDateArray), $"{parameterName}: Invalid year {dateArray[0]}");
                    return false;
                }

                // Validate month
                if (dateArray[1] < 1 || dateArray[1] > 12)
                {
                    LogError(HassErrorCode.InvalidDateFormat, nameof(ValidateDateArray), $"{parameterName}: Invalid month {dateArray[1]}");
                    return false;
                }

                // Validate day
                int daysInMonth = DateTime.DaysInMonth(dateArray[0], dateArray[1]);
                if (dateArray[2] < 1 || dateArray[2] > daysInMonth)
                {
                    LogError(HassErrorCode.InvalidDateFormat, nameof(ValidateDateArray), $"{parameterName}: Invalid day {dateArray[2]}");
                    return false;
                }

                // Validate hour
                if (dateArray[3] < 0 || dateArray[3] > 23)
                {
                    LogError(HassErrorCode.InvalidDateFormat, nameof(ValidateDateArray), $"{parameterName}: Invalid hour {dateArray[3]}");
                    return false;
                }

                // Validate minute
                if (dateArray[4] < 0 || dateArray[4] > 59)
                {
                    LogError(HassErrorCode.InvalidDateFormat, nameof(ValidateDateArray), $"{parameterName}: Invalid minute {dateArray[4]}");
                    return false;
                }

                // Validate second
                if (dateArray[5] < 0 || dateArray[5] > 59)
                {
                    LogError(HassErrorCode.InvalidDateFormat, nameof(ValidateDateArray), $"{parameterName}: Invalid second {dateArray[5]}");
                    return false;
                }

                // Try to create DateTime to final validation
                var _ = new DateTime(dateArray[0], dateArray[1], dateArray[2], dateArray[3], dateArray[4], dateArray[5]);
                return true;
            }
            catch (Exception ex)
            {
                LogError(HassErrorCode.InvalidDateFormat, nameof(ValidateDateArray), $"{parameterName}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Validate time step and time unit compatibility
        /// </summary>
        /// <param name="timeUnit">Time unit code</param>
        /// <param name="timeStep">Time step</param>
        /// <returns>True if valid</returns>
        public static bool ValidateTimeStep(int timeUnit, int timeStep)
        {
            if (timeStep <= 0)
            {
                LogError(HassErrorCode.InvalidParameter, nameof(ValidateTimeStep), $"Invalid time step: {timeStep}");
                return false;
            }

            if (timeUnit < 1 || timeUnit > 6)
            {
                LogError(HassErrorCode.InvalidParameter, nameof(ValidateTimeStep), $"Invalid time unit: {timeUnit}");
                return false;
            }

            // Additional validation based on time unit
            switch (timeUnit)
            {
                case 1: // Seconds
                    if (timeStep > 86400) // More than 1 day in seconds
                    {
                        LogError(HassErrorCode.OutOfRange, nameof(ValidateTimeStep), $"Time step too large for seconds: {timeStep}");
                        return false;
                    }
                    break;
                case 2: // Minutes
                    if (timeStep > 1440) // More than 1 day in minutes
                    {
                        LogError(HassErrorCode.OutOfRange, nameof(ValidateTimeStep), $"Time step too large for minutes: {timeStep}");
                        return false;
                    }
                    break;
                case 3: // Hours
                    if (timeStep > 24) // More than 1 day in hours
                    {
                        LogError(HassErrorCode.OutOfRange, nameof(ValidateTimeStep), $"Time step too large for hours: {timeStep}");
                        return false;
                    }
                    break;
            }

            return true;
        }

        /// <summary>
        /// Validate array parameter
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">Array to validate</param>
        /// <param name="minLength">Minimum required length</param>
        /// <param name="maxLength">Maximum allowed length</param>
        /// <param name="parameterName">Parameter name for logging</param>
        /// <returns>True if valid</returns>
        public static bool ValidateArray<T>(T[]? array, int minLength = 1, int maxLength = int.MaxValue, string parameterName = "array")
        {
            if (array == null)
            {
                LogError(HassErrorCode.InvalidParameter, nameof(ValidateArray), $"{parameterName} is null");
                return false;
            }

            if (array.Length < minLength)
            {
                LogError(HassErrorCode.InvalidParameter, nameof(ValidateArray), $"{parameterName} length {array.Length} is less than minimum {minLength}");
                return false;
            }

            if (array.Length > maxLength)
            {
                LogError(HassErrorCode.InvalidParameter, nameof(ValidateArray), $"{parameterName} length {array.Length} exceeds maximum {maxLength}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate numeric range
        /// </summary>
        /// <param name="value">Value to validate</param>
        /// <param name="minValue">Minimum value</param>
        /// <param name="maxValue">Maximum value</param>
        /// <param name="parameterName">Parameter name for logging</param>
        /// <returns>True if valid</returns>
        public static bool ValidateRange(float value, float minValue, float maxValue, string parameterName = "value")
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                LogError(HassErrorCode.InvalidParameter, nameof(ValidateRange), $"{parameterName} is NaN or Infinity");
                return false;
            }

            if (value < minValue || value > maxValue)
            {
                LogError(HassErrorCode.OutOfRange, nameof(ValidateRange), $"{parameterName} {value} is outside range [{minValue}, {maxValue}]");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate string parameter
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="allowNull">Allow null values</param>
        /// <param name="allowEmpty">Allow empty strings</param>
        /// <param name="minLength">Minimum length</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="parameterName">Parameter name for logging</param>
        /// <returns>True if valid</returns>
        public static bool ValidateString(string? value, bool allowNull = false, bool allowEmpty = true, 
            int minLength = 0, int maxLength = int.MaxValue, string parameterName = "string")
        {
            if (value == null)
            {
                if (!allowNull)
                {
                    LogError(HassErrorCode.InvalidParameter, nameof(ValidateString), $"{parameterName} is null");
                    return false;
                }
                return true;
            }

            if (string.IsNullOrEmpty(value) && !allowEmpty)
            {
                LogError(HassErrorCode.InvalidParameter, nameof(ValidateString), $"{parameterName} is empty");
                return false;
            }

            if (value.Length < minLength)
            {
                LogError(HassErrorCode.InvalidParameter, nameof(ValidateString), $"{parameterName} length {value.Length} is less than minimum {minLength}");
                return false;
            }

            if (value.Length > maxLength)
            {
                LogError(HassErrorCode.InvalidParameter, nameof(ValidateString), $"{parameterName} length {value.Length} exceeds maximum {maxLength}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate file path
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="mustExist">File must exist</param>
        /// <param name="parameterName">Parameter name for logging</param>
        /// <returns>True if valid</returns>
        public static bool ValidateFilePath(string? filePath, bool mustExist = false, string parameterName = "filePath")
        {
            if (!ValidateString(filePath, false, false, 1, 260, parameterName))
                return false;

            try
            {
                if (mustExist && !System.IO.File.Exists(filePath))
                {
                    LogError(HassErrorCode.FileNotFound, nameof(ValidateFilePath), $"File not found: {filePath}");
                    return false;
                }

                // Check for invalid characters
                char[] invalidChars = System.IO.Path.GetInvalidPathChars();
                if (filePath!.IndexOfAny(invalidChars) >= 0)
                {
                    LogError(HassErrorCode.InvalidParameter, nameof(ValidateFilePath), $"Invalid characters in file path: {filePath}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogError(HassErrorCode.InvalidParameter, nameof(ValidateFilePath), $"Invalid file path {filePath}: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Defensive Programming Helpers

        /// <summary>
        /// Safe array access with bounds checking
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">Array</param>
        /// <param name="index">Index to access</param>
        /// <param name="defaultValue">Default value if out of bounds</param>
        /// <returns>Array value or default</returns>
        public static T SafeArrayAccess<T>(T[]? array, int index, T defaultValue = default!)
        {
            if (array == null || index < 0 || index >= array.Length)
                return defaultValue;
            return array[index];
        }

        /// <summary>
        /// Safe division with zero check
        /// </summary>
        /// <param name="numerator">Numerator</param>
        /// <param name="denominator">Denominator</param>
        /// <param name="defaultValue">Default value if division by zero</param>
        /// <returns>Division result or default</returns>
        public static float SafeDivide(float numerator, float denominator, float defaultValue = 0.0f)
        {
            if (Math.Abs(denominator) < 1e-10)
                return defaultValue;
            return numerator / denominator;
        }

        /// <summary>
        /// Clamp value to range
        /// </summary>
        /// <param name="value">Value to clamp</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Clamped value</returns>
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0) return min;
            if (value.CompareTo(max) > 0) return max;
            return value;
        }

        /// <summary>
        /// Try parse with error logging
        /// </summary>
        /// <param name="text">Text to parse</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="functionName">Calling function name</param>
        /// <returns>Parsed value or default</returns>
        public static float TryParseFloat(string? text, float defaultValue = 0.0f, string functionName = "TryParseFloat")
        {
            if (string.IsNullOrWhiteSpace(text))
                return defaultValue;

            if (float.TryParse(text.Trim(), out float result))
                return result;

            LogError(HassErrorCode.ConversionError, functionName, $"Cannot parse '{text}' as float");
            return defaultValue;
        }

        /// <summary>
        /// Try parse with error logging
        /// </summary>
        /// <param name="text">Text to parse</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="functionName">Calling function name</param>
        /// <returns>Parsed value or default</returns>
        public static int TryParseInt(string? text, int defaultValue = 0, string functionName = "TryParseInt")
        {
            if (string.IsNullOrWhiteSpace(text))
                return defaultValue;

            if (int.TryParse(text.Trim(), out int result))
                return result;

            LogError(HassErrorCode.ConversionError, functionName, $"Cannot parse '{text}' as integer");
            return defaultValue;
        }

        #endregion
    }
}