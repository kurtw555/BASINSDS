using System;
using System.Globalization;
using System.Text;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Date and time utility functions corresponding to FORTRAN utdate module
    /// Provides comprehensive date/time manipulation and conversion utilities
    /// </summary>
    public static class DateTimeUtilities
    {
        #region Date/Time Conversion Functions

        /// <summary>
        /// F90_DATLST_XX - Convert date to string format
        /// </summary>
        /// <param name="date">Date array [year, month, day, hour, minute, second]</param>
        /// <param name="idStr">Output string as integer array</param>
        /// <param name="length">Output length</param>
        /// <param name="errorCode">Error code</param>
        public static void F90_DATLST_XX(int[] date, int[] idStr, out int length, out int errorCode)
        {
            try
            {
                var dateTime = new DateTime(date[0], date[1], date[2], date[3], date[4], date[5]);
                string dateString = dateTime.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
                
                length = Math.Min(dateString.Length, idStr.Length);
                
                for (int i = 0; i < length; i++)
                {
                    idStr[i] = (int)dateString[i];
                }
                
                // Handle single digit day padding like FORTRAN version
                if (date[2] < 10 && length < idStr.Length - 1)
                {
                    // Insert space for single digit day
                    for (int i = length; i > 9; i--)
                    {
                        if (i < idStr.Length)
                            idStr[i] = idStr[i - 1];
                    }
                    idStr[9] = 32; // Space
                    length++;
                }
                
                errorCode = 0;
            }
            catch
            {
                length = 0;
                errorCode = -1;
            }
        }

        /// <summary>
        /// F90_TIMCNV - Convert time format
        /// </summary>
        /// <param name="dateArray">Date array [year, month, day, hour, minute, second] (modified)</param>
        public static void F90_TIMCNV(int[] dateArray)
        {
            try
            {
                // Validate and normalize date components
                if (dateArray.Length >= 6)
                {
                    // Ensure valid year
                    if (dateArray[0] < 1900) dateArray[0] = 1900;
                    if (dateArray[0] > 2100) dateArray[0] = 2100;
                    
                    // Ensure valid month
                    if (dateArray[1] < 1) dateArray[1] = 1;
                    if (dateArray[1] > 12) dateArray[1] = 12;
                    
                    // Ensure valid day for the month
                    int daysInMonth = DateTime.DaysInMonth(dateArray[0], dateArray[1]);
                    if (dateArray[2] < 1) dateArray[2] = 1;
                    if (dateArray[2] > daysInMonth) dateArray[2] = daysInMonth;
                    
                    // Ensure valid hour
                    if (dateArray[3] < 0) dateArray[3] = 0;
                    if (dateArray[3] > 23) dateArray[3] = 23;
                    
                    // Ensure valid minute
                    if (dateArray[4] < 0) dateArray[4] = 0;
                    if (dateArray[4] > 59) dateArray[4] = 59;
                    
                    // Ensure valid second
                    if (dateArray[5] < 0) dateArray[5] = 0;
                    if (dateArray[5] > 59) dateArray[5] = 59;
                }
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error in F90_TIMCNV: {ex.Message}");
            }
        }

        /// <summary>
        /// F90_TIMCVT - Convert time between formats
        /// </summary>
        /// <param name="dateArray">Date array (modified)</param>
        public static void F90_TIMCVT(int[] dateArray)
        {
            // Similar to TIMCNV but with different conversion logic
            F90_TIMCNV(dateArray);
        }

        /// <summary>
        /// F90_TIMBAK - Move time backward by one time step
        /// </summary>
        /// <param name="tCode">Time code</param>
        /// <param name="dateArray">Date array (modified)</param>
        public static void F90_TIMBAK(int tCode, int[] dateArray)
        {
            try
            {
                var dateTime = new DateTime(dateArray[0], dateArray[1], dateArray[2], dateArray[3], dateArray[4], dateArray[5]);
                
                switch (tCode)
                {
                    case 1: // seconds
                        dateTime = dateTime.AddSeconds(-1);
                        break;
                    case 2: // minutes
                        dateTime = dateTime.AddMinutes(-1);
                        break;
                    case 3: // hours
                        dateTime = dateTime.AddHours(-1);
                        break;
                    case 4: // days
                        dateTime = dateTime.AddDays(-1);
                        break;
                    case 5: // months
                        dateTime = dateTime.AddMonths(-1);
                        break;
                    case 6: // years
                        dateTime = dateTime.AddYears(-1);
                        break;
                }
                
                dateArray[0] = dateTime.Year;
                dateArray[1] = dateTime.Month;
                dateArray[2] = dateTime.Day;
                dateArray[3] = dateTime.Hour;
                dateArray[4] = dateTime.Minute;
                dateArray[5] = dateTime.Second;
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error in F90_TIMBAK: {ex.Message}");
            }
        }

        /// <summary>
        /// F90_CMPTIM - Compare time specifications
        /// </summary>
        /// <param name="tCode1">First time code</param>
        /// <param name="tStep1">First time step</param>
        /// <param name="tCode2">Second time code</param>
        /// <param name="tStep2">Second time step</param>
        /// <param name="tStepF">Output final time step</param>
        /// <param name="tcdCmp">Output time code comparison</param>
        public static void F90_CMPTIM(int tCode1, int tStep1, int tCode2, int tStep2, out int tStepF, out int tcdCmp)
        {
            try
            {
                // Convert both time specifications to common unit (minutes)
                int minutes1 = ConvertToMinutes(tCode1, tStep1);
                int minutes2 = ConvertToMinutes(tCode2, tStep2);
                
                if (minutes1 == minutes2)
                {
                    tcdCmp = 0; // Equal
                    tStepF = tStep1;
                }
                else if (minutes1 < minutes2)
                {
                    tcdCmp = -1; // First is smaller
                    tStepF = FindGCD(minutes1, minutes2);
                }
                else
                {
                    tcdCmp = 1; // First is larger
                    tStepF = FindGCD(minutes1, minutes2);
                }
            }
            catch
            {
                tcdCmp = 0;
                tStepF = 1;
            }
        }

        /// <summary>
        /// F90_DATNXT - Get next date given interval
        /// </summary>
        /// <param name="interval">Time interval code</param>
        /// <param name="upBack">Direction (1=forward, -1=backward)</param>
        /// <param name="dateArray">Date array (modified)</param>
        public static void F90_DATNXT(int interval, int upBack, int[] dateArray)
        {
            try
            {
                var dateTime = new DateTime(dateArray[0], dateArray[1], dateArray[2], dateArray[3], dateArray[4], dateArray[5]);
                
                int direction = upBack >= 0 ? 1 : -1;
                
                switch (interval)
                {
                    case 1: // Second
                        dateTime = dateTime.AddSeconds(direction);
                        break;
                    case 2: // Minute
                        dateTime = dateTime.AddMinutes(direction);
                        break;
                    case 3: // Hour
                        dateTime = dateTime.AddHours(direction);
                        break;
                    case 4: // Day
                        dateTime = dateTime.AddDays(direction);
                        break;
                    case 5: // Month
                        dateTime = dateTime.AddMonths(direction);
                        break;
                    case 6: // Year
                        dateTime = dateTime.AddYears(direction);
                        break;
                    default: // Default to day
                        dateTime = dateTime.AddDays(direction);
                        break;
                }
                
                dateArray[0] = dateTime.Year;
                dateArray[1] = dateTime.Month;
                dateArray[2] = dateTime.Day;
                dateArray[3] = dateTime.Hour;
                dateArray[4] = dateTime.Minute;
                dateArray[5] = dateTime.Second;
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error in F90_DATNXT: {ex.Message}");
            }
        }

        /// <summary>
        /// F90_DTMCMN - Find common time period for multiple datasets
        /// </summary>
        /// <param name="nDat">Number of datasets</param>
        /// <param name="startDates">Start dates for each dataset</param>
        /// <param name="stopDates">Stop dates for each dataset</param>
        /// <param name="tStep">Time step</param>
        /// <param name="tCode">Time code</param>
        /// <param name="commonStart">Output common start date</param>
        /// <param name="commonEnd">Output common end date</param>
        /// <param name="finalTStep">Output final time step</param>
        /// <param name="finalTCode">Output final time code</param>
        /// <param name="retCode">Return code</param>
        public static void F90_DTMCMN(int nDat, int[,] startDates, int[,] stopDates, int tStep, int tCode,
            int[] commonStart, int[] commonEnd, out int finalTStep, out int finalTCode, out int retCode)
        {
            try
            {
                if (nDat <= 0 || startDates.GetLength(1) < nDat || stopDates.GetLength(1) < nDat)
                {
                    retCode = -1;
                    finalTStep = tStep;
                    finalTCode = tCode;
                    return;
                }

                // Find the latest start date (common start)
                DateTime latestStart = DateTime.MinValue;
                DateTime earliestEnd = DateTime.MaxValue;
                
                for (int i = 0; i < nDat; i++)
                {
                    var startDate = new DateTime(startDates[0, i], startDates[1, i], startDates[2, i],
                                               startDates[3, i], startDates[4, i], startDates[5, i]);
                    var endDate = new DateTime(stopDates[0, i], stopDates[1, i], stopDates[2, i],
                                             stopDates[3, i], stopDates[4, i], stopDates[5, i]);
                    
                    if (startDate > latestStart) latestStart = startDate;
                    if (endDate < earliestEnd) earliestEnd = endDate;
                }
                
                if (latestStart > earliestEnd)
                {
                    retCode = -2; // No common period
                    finalTStep = tStep;
                    finalTCode = tCode;
                    return;
                }
                
                // Set common period
                commonStart[0] = latestStart.Year;
                commonStart[1] = latestStart.Month;
                commonStart[2] = latestStart.Day;
                commonStart[3] = latestStart.Hour;
                commonStart[4] = latestStart.Minute;
                commonStart[5] = latestStart.Second;
                
                commonEnd[0] = earliestEnd.Year;
                commonEnd[1] = earliestEnd.Month;
                commonEnd[2] = earliestEnd.Day;
                commonEnd[3] = earliestEnd.Hour;
                commonEnd[4] = earliestEnd.Minute;
                commonEnd[5] = earliestEnd.Second;
                
                finalTStep = tStep;
                finalTCode = tCode;
                retCode = 0;
            }
            catch
            {
                retCode = -1;
                finalTStep = tStep;
                finalTCode = tCode;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Convert time specification to minutes
        /// </summary>
        /// <param name="tCode">Time code</param>
        /// <param name="tStep">Time step</param>
        /// <returns>Equivalent minutes</returns>
        private static int ConvertToMinutes(int tCode, int tStep)
        {
            return tCode switch
            {
                1 => tStep / 60,    // seconds to minutes
                2 => tStep,         // minutes
                3 => tStep * 60,    // hours to minutes
                4 => tStep * 1440,  // days to minutes
                5 => tStep * 43200, // months to minutes (approximate)
                6 => tStep * 525600, // years to minutes (approximate)
                _ => tStep
            };
        }

        /// <summary>
        /// Find greatest common divisor
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>Greatest common divisor</returns>
        private static int FindGCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return Math.Abs(a);
        }

        /// <summary>
        /// Validate date array
        /// </summary>
        /// <param name="dateArray">Date array to validate</param>
        /// <returns>True if valid</returns>
        public static bool ValidateDateArray(int[] dateArray)
        {
            if (dateArray.Length < 6) return false;
            
            try
            {
                var _ = new DateTime(dateArray[0], dateArray[1], dateArray[2], dateArray[3], dateArray[4], dateArray[5]);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Convert DateTime to FORTRAN date array
        /// </summary>
        /// <param name="dateTime">DateTime to convert</param>
        /// <returns>FORTRAN date array</returns>
        public static int[] DateTimeToArray(DateTime dateTime)
        {
            return new int[]
            {
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second
            };
        }

        /// <summary>
        /// Convert FORTRAN date array to DateTime
        /// </summary>
        /// <param name="dateArray">FORTRAN date array</param>
        /// <returns>DateTime object</returns>
        public static DateTime ArrayToDateTime(int[] dateArray)
        {
            if (dateArray.Length < 6)
                throw new ArgumentException("Date array must have at least 6 elements");
            
            return new DateTime(dateArray[0], dateArray[1], dateArray[2], dateArray[3], dateArray[4], dateArray[5]);
        }

        #endregion
    }
}