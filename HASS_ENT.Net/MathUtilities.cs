using System;
using System.Linq;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Mathematical and statistical utilities equivalent to FORTRAN math functions
    /// Includes statistical analysis, array operations, and numerical utilities
    /// </summary>
    public static class MathUtilities
    {
        /// <summary>
        /// Sort real array in ascending order - equivalent to F90_ASRTRP
        /// </summary>
        /// <param name="data">Array to sort (modified in place)</param>
        public static void SortRealArray(float[] data)
        {
            try
            {
                Array.Sort(data);
                LoggingService.LogDebug($"Sorted array of {data.Length} real values");
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error sorting real array: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Calculate basic statistics for an array
        /// </summary>
        /// <param name="data">Data array</param>
        /// <param name="validCount">Number of valid values</param>
        /// <param name="mean">Mean value (output)</param>
        /// <param name="minimum">Minimum value (output)</param>
        /// <param name="maximum">Maximum value (output)</param>
        /// <param name="standardDeviation">Standard deviation (output)</param>
        /// <param name="missingValue">Value to consider as missing</param>
        public static void CalculateStatistics(float[] data, out int validCount, out float mean, 
            out float minimum, out float maximum, out float standardDeviation, float missingValue = -999.0f)
        {
            validCount = 0;
            mean = 0.0f;
            minimum = float.MaxValue;
            maximum = float.MinValue;
            standardDeviation = 0.0f;
            
            try
            {
                // Filter out missing values and count valid ones
                var validData = data.Where(x => Math.Abs(x - missingValue) > 1e-6).ToArray();
                validCount = validData.Length;
                
                if (validCount == 0)
                {
                    LoggingService.LogWarning("No valid data for statistics calculation");
                    return;
                }
                
                // Calculate basic statistics
                minimum = validData.Min();
                maximum = validData.Max();
                mean = validData.Average();
                
                // Calculate standard deviation
                if (validCount > 1)
                {
                    double meanDouble = mean; // Use local variable to avoid lambda capture issue
                    double sumSquaredDiffs = validData.Sum(x => Math.Pow(x - meanDouble, 2));
                    standardDeviation = (float)Math.Sqrt(sumSquaredDiffs / (validCount - 1));
                }
                
                LoggingService.LogDebug($"Statistics: n={validCount}, mean={mean:F3}, min={minimum:F3}, max={maximum:F3}, std={standardDeviation:F3}");
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error calculating statistics: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Fit linear regression - equivalent to F90_FITLIN
        /// </summary>
        /// <param name="data">Array of X,Y pairs [x1,y1,x2,y2,...]</param>
        /// <param name="numPoints">Number of data points</param>
        /// <param name="aCoefficient">Intercept coefficient (output)</param>
        /// <param name="bCoefficient">Slope coefficient (output)</param>
        /// <param name="rSquared">R-squared correlation (output)</param>
        public static void FitLinearRegression(float[] data, int numPoints, out float aCoefficient, 
            out float bCoefficient, out float rSquared)
        {
            aCoefficient = 0.0f;
            bCoefficient = 0.0f;
            rSquared = 0.0f;
            
            try
            {
                if (numPoints < 2 || data.Length < numPoints * 2)
                {
                    LoggingService.LogError("Insufficient data for linear regression");
                    return;
                }
                
                // Extract X and Y values
                double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0, sumY2 = 0;
                
                for (int i = 0; i < numPoints; i++)
                {
                    double x = data[i * 2];
                    double y = data[i * 2 + 1];
                    
                    sumX += x;
                    sumY += y;
                    sumXY += x * y;
                    sumX2 += x * x;
                    sumY2 += y * y;
                }
                
                double n = numPoints;
                
                // Calculate slope (b) and intercept (a)
                double denominator = n * sumX2 - sumX * sumX;
                if (Math.Abs(denominator) < 1e-10)
                {
                    LoggingService.LogError("Linear regression failed: singular matrix");
                    return;
                }
                
                bCoefficient = (float)((n * sumXY - sumX * sumY) / denominator);
                aCoefficient = (float)((sumY - bCoefficient * sumX) / n);
                
                // Calculate R-squared
                double ssTotal = sumY2 - (sumY * sumY) / n;
                double ssResidual = 0;
                
                for (int i = 0; i < numPoints; i++)
                {
                    double x = data[i * 2];
                    double y = data[i * 2 + 1];
                    double predicted = aCoefficient + bCoefficient * x;
                    double residual = y - predicted;
                    ssResidual += residual * residual;
                }
                
                if (Math.Abs(ssTotal) > 1e-10)
                {
                    rSquared = (float)(1.0 - ssResidual / ssTotal);
                }
                
                LoggingService.LogDebug($"Linear regression: y = {aCoefficient:F4} + {bCoefficient:F4}*x, R² = {rSquared:F4}");
            }
            catch (Exception ex)
            {
                LoggingService.LogError($"Error in linear regression: {ex.Message}");
            }
        }
    }
}