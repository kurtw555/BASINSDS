using System;
using System.Globalization;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Time series processing and date/time utilities
    /// Equivalent to FORTRAN functions for time manipulation and analysis
    /// </summary>
    public static class TimeSeriesProcessor
    {
        /// <summary>
        /// Time code constants
        /// </summary>
        public enum TimeCode
        {
            Annual = 1,
            Monthly = 2, 
            Daily = 3,
            Hourly = 4,
            Minutes = 5,
            Seconds = 6
        }
        
        /// <summary>
        /// Add time to a date - equivalent to F90_TIMADD
        /// </summary>
        /// <param name="startDate">Starting date [year, month, day, hour, minute, second]</param>
        /// <param name="timeCode">Time code (units)</param>
        /// <param name="timeStep">Time step increment</param>
        /// <param name="numValues">Number of increments to add</param>
        /// <param name="endDate">Resulting date (output)</param>
        public static void AddTime(int[] startDate, TimeCode timeCode, int timeStep, int numValues, int[] endDate)
        {
            try
            {
                DateTime dateTime = DateArrayToDateTime(startDate);
                TimeSpan increment = GetTimeSpan(timeCode, timeStep * numValues);
                DateTime resultDate = dateTime.Add(increment);
                
                DateTimeToArray(resultDate, endDate);
                
                LoggingService.LogDebug($"Added time: {dateTime:yyyy-MM-dd HH:mm:ss} + {numValues}*{timeStep} {timeCode} = {resultDate:yyyy-MM-dd HH:mm:ss}");
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error adding time: {ex.Message}");
                Array.Copy(startDate, endDate, Math.Min(startDate.Length, endDate.Length));
            }
        }
        
        /// <summary>
        /// Calculate time difference between dates - equivalent to F90_TIMDIF
        /// </summary>
        /// <param name="startDate">Start date [year, month, day, hour, minute, second]</param>
        /// <param name="endDate">End date [year, month, day, hour, minute, second]</param>
        /// <param name="timeCode">Time code (units)</param>
        /// <param name="timeStep">Time step</param>
        /// <returns>Number of time steps between dates</returns>
        public static int TimeDifference(int[] startDate, int[] endDate, TimeCode timeCode, int timeStep)
        {
            try
            {
                DateTime start = DateArrayToDateTime(startDate);
                DateTime end = DateArrayToDateTime(endDate);
                TimeSpan difference = end - start;
                
                double totalUnits = timeCode switch
                {
                    TimeCode.Annual => difference.TotalDays / 365.25,
                    TimeCode.Monthly => difference.TotalDays / 30.44, // Average month length
                    TimeCode.Daily => difference.TotalDays,
                    TimeCode.Hourly => difference.TotalHours,
                    TimeCode.Minutes => difference.TotalMinutes,
                    TimeCode.Seconds => difference.TotalSeconds,
                    _ => difference.TotalDays
                };
                
                int numSteps = (int)Math.Round(totalUnits / timeStep);
                
                LoggingService.LogDebug($"Time difference: {start:yyyy-MM-dd HH:mm:ss} to {end:yyyy-MM-dd HH:mm:ss} = {numSteps} steps of {timeStep} {timeCode}");
                return numSteps;
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error calculating time difference: {ex.Message}");
                return 0;
            }
        }
        
        /// <summary>
        /// Get next date - equivalent to F90_DATNXT
        /// </summary>
        /// <param name="interval">Time interval</param>
        /// <param name="upBack">Direction: 1 = forward, -1 = backward</param>
        /// <param name="date">Date to modify (input/output)</param>
        public static void NextDate(int interval, int upBack, int[] date)
        {
            try
            {
                DateTime dateTime = DateArrayToDateTime(date);
                
                TimeSpan increment = interval switch
                {
                    1 => TimeSpan.FromDays(365), // Annual
                    2 => TimeSpan.FromDays(30),  // Monthly (approximate)
                    3 => TimeSpan.FromDays(1),   // Daily
                    4 => TimeSpan.FromHours(1),  // Hourly
                    _ => TimeSpan.FromDays(1)
                };
                
                if (upBack < 0)
                {
                    increment = increment.Negate();
                }
                
                DateTime resultDate = dateTime.Add(increment);
                DateTimeToArray(resultDate, date);
                
                LoggingService.LogDebug($"Next date: {dateTime:yyyy-MM-dd HH:mm:ss} -> {resultDate:yyyy-MM-dd HH:mm:ss}");
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error getting next date: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Check time validity - equivalent to F90_TIMCHK
        /// </summary>
        /// <param name="date1">First date</param>
        /// <param name="date2">Second date</param>
        /// <returns>-1 if date1 < date2, 0 if equal, 1 if date1 > date2</returns>
        public static int TimeCheck(int[] date1, int[] date2)
        {
            try
            {
                DateTime dt1 = DateArrayToDateTime(date1);
                DateTime dt2 = DateArrayToDateTime(date2);
                
                return DateTime.Compare(dt1, dt2);
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error checking time: {ex.Message}");
                return 0;
            }
        }
        
        /// <summary>
        /// Convert Julian day to month/day - equivalent to F90_JDMODY
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="julianDay">Julian day of year</param>
        /// <param name="month">Month (output)</param>
        /// <param name="day">Day (output)</param>
        public static void JulianDayToMonthDay(int year, int julianDay, out int month, out int day)
        {
            try
            {
                DateTime jan1 = new DateTime(year, 1, 1);
                DateTime targetDate = jan1.AddDays(julianDay - 1);
                
                month = targetDate.Month;
                day = targetDate.Day;
                
                LoggingService.LogDebug($"Julian day {julianDay} of {year} = {month}/{day}");
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error converting Julian day: {ex.Message}");
                month = 1;
                day = 1;
            }
        }
        
        /// <summary>
        /// Get days in month - equivalent to F90_DAYMON
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="month">Month</param>
        /// <returns>Number of days in the month</returns>
        public static int DaysInMonth(int year, int month)
        {
            try
            {
                return DateTime.DaysInMonth(year, month);
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error getting days in month {year}/{month}: {ex.Message}");
                return 30; // Default fallback
            }
        }
        
        /// <summary>
        /// Convert time units - equivalent to F90_TIMCNV
        /// </summary>
        /// <param name="date">Date array to convert (modified in place)</param>
        public static void ConvertTime(int[] date)
        {
            try
            {
                // Normalize the date components (handle overflow/underflow)
                DateTime dt = DateArrayToDateTime(date);
                DateTimeToArray(dt, date);
                
                LoggingService.LogDebug($"Converted time to: {dt:yyyy-MM-dd HH:mm:ss}");
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error converting time: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Back up time - equivalent to F90_TIMBAK
        /// </summary>
        /// <param name="timeCode">Time code</param>
        /// <param name="date">Date to back up (modified in place)</param>
        public static void BackupTime(TimeCode timeCode, int[] date)
        {
            try
            {
                DateTime dateTime = DateArrayToDateTime(date);
                
                TimeSpan backup = timeCode switch
                {
                    TimeCode.Annual => TimeSpan.FromDays(365),
                    TimeCode.Monthly => TimeSpan.FromDays(30),
                    TimeCode.Daily => TimeSpan.FromDays(1),
                    TimeCode.Hourly => TimeSpan.FromHours(1),
                    TimeCode.Minutes => TimeSpan.FromMinutes(1),
                    TimeCode.Seconds => TimeSpan.FromSeconds(1),
                    _ => TimeSpan.FromDays(1)
                };
                
                DateTime resultDate = dateTime.Subtract(backup);
                DateTimeToArray(resultDate, date);
                
                LoggingService.LogDebug($"Backed up time: {dateTime:yyyy-MM-dd HH:mm:ss} -> {resultDate:yyyy-MM-dd HH:mm:ss}");
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error backing up time: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Format date as string - equivalent to F90_DATLST_XX
        /// </summary>
        /// <param name="date">Date array [year, month, day, hour, minute, second]</param>
        /// <returns>Formatted date string</returns>
        public static string FormatDateString(int[] date)
        {
            try
            {
                DateTime dateTime = DateArrayToDateTime(date);
                return dateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error formatting date: {ex.Message}");
                return "Invalid Date";
            }
        }
        
        /// <summary>
        /// Compare time steps - equivalent to F90_CMPTIM
        /// </summary>
        /// <param name="timeCode1">First time code</param>
        /// <param name="timeStep1">First time step</param>
        /// <param name="timeCode2">Second time code</param>
        /// <param name="timeStep2">Second time step</param>
        /// <param name="stepFlag">Step compatibility flag (output)</param>
        /// <param name="codeFlag">Code compatibility flag (output)</param>
        public static void CompareTimeSteps(TimeCode timeCode1, int timeStep1, TimeCode timeCode2, int timeStep2,
            out int stepFlag, out int codeFlag)
        {
            stepFlag = 0;
            codeFlag = 0;
            
            try
            {
                // Check if time codes are compatible
                codeFlag = timeCode1 == timeCode2 ? 0 : -1;
                
                // Check if time steps are compatible
                if (codeFlag == 0)
                {
                    stepFlag = timeStep1 == timeStep2 ? 0 : 1;
                }
                else
                {
                    // Different time codes - more complex comparison needed
                    TimeSpan span1 = GetTimeSpan(timeCode1, timeStep1);
                    TimeSpan span2 = GetTimeSpan(timeCode2, timeStep2);
                    
                    stepFlag = span1 == span2 ? 0 : 1;
                }
                
                LoggingService.LogDebug($"Time step comparison: {timeCode1}({timeStep1}) vs {timeCode2}({timeStep2}) -> stepFlag={stepFlag}, codeFlag={codeFlag}");
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error comparing time steps: {ex.Message}");
                stepFlag = 1;
                codeFlag = -1;
            }
        }
        
        /// <summary>
        /// Convert date array to DateTime
        /// </summary>
        /// <param name="dateArray">Date array [year, month, day, hour, minute, second]</param>
        /// <returns>DateTime object</returns>
        private static DateTime DateArrayToDateTime(int[] dateArray)
        {
            if (dateArray == null || dateArray.Length < 6)
                throw new ArgumentException("Date array must have at least 6 elements");
            
            int year = dateArray[0];
            int month = Math.Max(1, Math.Min(12, dateArray[1]));
            int day = Math.Max(1, Math.Min(31, dateArray[2]));
            int hour = Math.Max(0, Math.Min(23, dateArray[3]));
            int minute = Math.Max(0, Math.Min(59, dateArray[4]));
            int second = Math.Max(0, Math.Min(59, dateArray[5]));
            
            return new DateTime(year, month, day, hour, minute, second);
        }
        
        /// <summary>
        /// Convert DateTime to date array
        /// </summary>
        /// <param name="dateTime">DateTime object</param>
        /// <param name="dateArray">Date array to fill [year, month, day, hour, minute, second]</param>
        private static void DateTimeToArray(DateTime dateTime, int[] dateArray)
        {
            if (dateArray == null || dateArray.Length < 6)
                throw new ArgumentException("Date array must have at least 6 elements");
            
            dateArray[0] = dateTime.Year;
            dateArray[1] = dateTime.Month;
            dateArray[2] = dateTime.Day;
            dateArray[3] = dateTime.Hour;
            dateArray[4] = dateTime.Minute;
            dateArray[5] = dateTime.Second;
        }
        
        /// <summary>
        /// Get TimeSpan for time code and step
        /// </summary>
        /// <param name="timeCode">Time code</param>
        /// <param name="timeStep">Time step value</param>
        /// <returns>Corresponding TimeSpan</returns>
        private static TimeSpan GetTimeSpan(TimeCode timeCode, int timeStep)
        {
            return timeCode switch
            {
                TimeCode.Annual => TimeSpan.FromDays(365.25 * timeStep),
                TimeCode.Monthly => TimeSpan.FromDays(30.44 * timeStep),
                TimeCode.Daily => TimeSpan.FromDays(timeStep),
                TimeCode.Hourly => TimeSpan.FromHours(timeStep),
                TimeCode.Minutes => TimeSpan.FromMinutes(timeStep),
                TimeCode.Seconds => TimeSpan.FromSeconds(timeStep),
                _ => TimeSpan.FromDays(timeStep)
            };
        }
    }
}