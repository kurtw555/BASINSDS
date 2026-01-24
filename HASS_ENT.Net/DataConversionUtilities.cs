using System;
using System.Globalization;
using System.Text;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Data conversion and formatting utilities corresponding to FORTRAN utchar and utnumb modules
    /// Provides numeric formatting, string conversion, and data manipulation functions
    /// </summary>
    public static class DataConversionUtilities
    {
        #region String/Character Conversion Functions

        /// <summary>
        /// F90_DECCHX_XX - Convert real number to character string
        /// </summary>
        /// <param name="realValue">Real value to convert</param>
        /// <param name="length">Total length of output string</param>
        /// <param name="sigDig">Number of significant digits</param>
        /// <param name="decPla">Number of decimal places</param>
        /// <param name="outputString">Output string as integer array</param>
        public static void F90_DECCHX_XX(float realValue, int length, int sigDig, int decPla, int[] outputString)
        {
            try
            {
                // Format the real value according to specifications
                string formatString;
                
                if (decPla >= 0)
                {
                    // Fixed decimal places
                    formatString = $"F{decPla}";
                }
                else
                {
                    // Scientific notation or general format
                    formatString = $"G{sigDig}";
                }
                
                string formatted = realValue.ToString(formatString, CultureInfo.InvariantCulture);
                
                // Ensure the string fits in the specified length
                if (formatted.Length > length)
                {
                    // Try exponential format if too long
                    formatted = realValue.ToString($"E{Math.Max(0, length - 7)}", CultureInfo.InvariantCulture);
                    if (formatted.Length > length)
                    {
                        // Truncate if still too long
                        formatted = formatted.Substring(0, length);
                    }
                }
                else if (formatted.Length < length)
                {
                    // Right-justify the number (pad with spaces on left)
                    formatted = formatted.PadLeft(length);
                }
                
                // Convert to integer array
                for (int i = 0; i < Math.Min(length, outputString.Length); i++)
                {
                    if (i < formatted.Length)
                        outputString[i] = (int)formatted[i];
                    else
                        outputString[i] = 32; // Space
                }
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error in F90_DECCHX_XX: {ex.Message}");
                
                // Fill with spaces on error
                for (int i = 0; i < Math.Min(length, outputString.Length); i++)
                    outputString[i] = 32;
            }
        }

        /// <summary>
        /// Convert integer array to string (utility function)
        /// </summary>
        /// <param name="intArray">Integer array representing characters</param>
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
                HassEntFunctions.LogMsg($"Error in IntArrayToString: {ex.Message}");
                return "";
            }
        }

        /// <summary>
        /// Convert string to integer array (utility function)
        /// </summary>
        /// <param name="text">String to convert</param>
        /// <param name="intArray">Output integer array</param>
        /// <param name="maxLength">Maximum length</param>
        public static void StringToIntArray(string text, int[] intArray, int maxLength = -1)
        {
            try
            {
                if (intArray == null || text == null) return;
                
                int len = maxLength < 0 ? Math.Min(text.Length, intArray.Length) : Math.Min(maxLength, Math.Min(text.Length, intArray.Length));
                
                for (int i = 0; i < len; i++)
                {
                    intArray[i] = (int)text[i];
                }
                
                // Fill remaining with spaces
                for (int i = len; i < intArray.Length && (maxLength < 0 || i < maxLength); i++)
                {
                    intArray[i] = 32; // Space
                }
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error in StringToIntArray: {ex.Message}");
            }
        }

        #endregion

        #region Numeric Formatting Functions

        /// <summary>
        /// Format floating point number with specified precision
        /// </summary>
        /// <param name="value">Value to format</param>
        /// <param name="totalWidth">Total width of formatted string</param>
        /// <param name="decimalPlaces">Number of decimal places</param>
        /// <param name="useExponential">Use exponential notation if needed</param>
        /// <returns>Formatted string</returns>
        public static string FormatFloat(float value, int totalWidth, int decimalPlaces, bool useExponential = false)
        {
            try
            {
                string format = useExponential ? $"E{decimalPlaces}" : $"F{decimalPlaces}";
                string result = value.ToString(format, CultureInfo.InvariantCulture);
                
                if (result.Length > totalWidth)
                {
                    // Try exponential if not already using it
                    if (!useExponential && totalWidth >= 7)
                    {
                        result = value.ToString($"E{Math.Max(0, totalWidth - 7)}", CultureInfo.InvariantCulture);
                    }
                    
                    // Truncate if still too long
                    if (result.Length > totalWidth)
                    {
                        result = result.Substring(0, totalWidth);
                    }
                }
                
                return result.PadLeft(totalWidth);
            }
            catch
            {
                return new string('*', totalWidth); // Fill with asterisks on error
            }
        }

        /// <summary>
        /// Format integer with specified width
        /// </summary>
        /// <param name="value">Integer value</param>
        /// <param name="width">Total width</param>
        /// <param name="rightJustify">Right justify (default true)</param>
        /// <returns>Formatted string</returns>
        public static string FormatInteger(int value, int width, bool rightJustify = true)
        {
            try
            {
                string result = value.ToString();
                
                if (result.Length > width)
                {
                    return new string('*', width); // Overflow indicator
                }
                
                return rightJustify ? result.PadLeft(width) : result.PadRight(width);
            }
            catch
            {
                return new string('*', width);
            }
        }

        /// <summary>
        /// Parse string to float with error handling
        /// </summary>
        /// <param name="text">Text to parse</param>
        /// <param name="defaultValue">Default value if parsing fails</param>
        /// <returns>Parsed float value</returns>
        public static float ParseFloat(string text, float defaultValue = 0.0f)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                    return defaultValue;
                
                return float.Parse(text.Trim(), CultureInfo.InvariantCulture);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Parse string to integer with error handling
        /// </summary>
        /// <param name="text">Text to parse</param>
        /// <param name="defaultValue">Default value if parsing fails</param>
        /// <returns>Parsed integer value</returns>
        public static int ParseInteger(string text, int defaultValue = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                    return defaultValue;
                
                return int.Parse(text.Trim());
            }
            catch
            {
                return defaultValue;
            }
        }

        #endregion

        #region Data Array Utilities

        /// <summary>
        /// Copy integer array with bounds checking
        /// </summary>
        /// <param name="source">Source array</param>
        /// <param name="destination">Destination array</param>
        /// <param name="length">Number of elements to copy</param>
        public static void CopyIntArray(int[] source, int[] destination, int length = -1)
        {
            try
            {
                if (source == null || destination == null) return;
                
                int copyLength = length < 0 ? Math.Min(source.Length, destination.Length) : Math.Min(length, Math.Min(source.Length, destination.Length));
                
                Array.Copy(source, destination, copyLength);
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error in CopyIntArray: {ex.Message}");
            }
        }

        /// <summary>
        /// Copy float array with bounds checking
        /// </summary>
        /// <param name="source">Source array</param>
        /// <param name="destination">Destination array</param>
        /// <param name="length">Number of elements to copy</param>
        public static void CopyFloatArray(float[] source, float[] destination, int length = -1)
        {
            try
            {
                if (source == null || destination == null) return;
                
                int copyLength = length < 0 ? Math.Min(source.Length, destination.Length) : Math.Min(length, Math.Min(source.Length, destination.Length));
                
                Array.Copy(source, destination, copyLength);
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error in CopyFloatArray: {ex.Message}");
            }
        }

        /// <summary>
        /// Initialize integer array with specific value
        /// </summary>
        /// <param name="array">Array to initialize</param>
        /// <param name="value">Value to set</param>
        /// <param name="length">Number of elements to set</param>
        public static void InitializeIntArray(int[] array, int value, int length = -1)
        {
            try
            {
                if (array == null) return;
                
                int initLength = length < 0 ? array.Length : Math.Min(length, array.Length);
                
                for (int i = 0; i < initLength; i++)
                {
                    array[i] = value;
                }
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error in InitializeIntArray: {ex.Message}");
            }
        }

        /// <summary>
        /// Initialize float array with specific value
        /// </summary>
        /// <param name="array">Array to initialize</param>
        /// <param name="value">Value to set</param>
        /// <param name="length">Number of elements to set</param>
        public static void InitializeFloatArray(float[] array, float value, int length = -1)
        {
            try
            {
                if (array == null) return;
                
                int initLength = length < 0 ? array.Length : Math.Min(length, array.Length);
                
                for (int i = 0; i < initLength; i++)
                {
                    array[i] = value;
                }
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error in InitializeFloatArray: {ex.Message}");
            }
        }

        #endregion

        #region Validation and Utility Functions

        /// <summary>
        /// Validate numeric string
        /// </summary>
        /// <param name="text">Text to validate</param>
        /// <param name="allowFloat">Allow floating point numbers</param>
        /// <returns>True if valid number</returns>
        public static bool IsValidNumber(string text, bool allowFloat = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                    return false;
                
                if (allowFloat)
                    return float.TryParse(text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out _);
                else
                    return int.TryParse(text.Trim(), out _);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Trim spaces from integer array representing string
        /// </summary>
        /// <param name="intArray">Integer array</param>
        /// <returns>Trimmed string</returns>
        public static string TrimIntArrayString(int[] intArray)
        {
            if (intArray == null) return "";
            
            var sb = new StringBuilder();
            foreach (int value in intArray)
            {
                if (value == 0) break; // Null terminator
                sb.Append((char)value);
            }
            
            return sb.ToString().Trim();
        }

        /// <summary>
        /// Get length of string in integer array (excluding trailing spaces)
        /// </summary>
        /// <param name="intArray">Integer array</param>
        /// <returns>Effective string length</returns>
        public static int GetStringLength(int[] intArray)
        {
            if (intArray == null) return 0;
            
            int length = 0;
            for (int i = intArray.Length - 1; i >= 0; i--)
            {
                if (intArray[i] != 32 && intArray[i] != 0) // Not space or null
                {
                    length = i + 1;
                    break;
                }
            }
            
            return length;
        }

        /// <summary>
        /// Pad string to specified length
        /// </summary>
        /// <param name="text">Text to pad</param>
        /// <param name="totalLength">Total desired length</param>
        /// <param name="padChar">Character to pad with</param>
        /// <param name="rightJustify">Right justify (pad left)</param>
        /// <returns>Padded string</returns>
        public static string PadString(string text, int totalLength, char padChar = ' ', bool rightJustify = true)
        {
            try
            {
                if (text == null) text = "";
                
                if (text.Length >= totalLength)
                    return text.Substring(0, totalLength);
                
                return rightJustify ? text.PadLeft(totalLength, padChar) : text.PadRight(totalLength, padChar);
            }
            catch
            {
                return new string(padChar, totalLength);
            }
        }

        #endregion
    }
}