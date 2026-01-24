using System;
using System.Collections.Generic;
using System.Linq;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Time series analysis and processing functions
    /// Implements statistical analysis, duration curves, and comparison functions
    /// </summary>
    public static class TimeSeriesAnalysis
    {
        #region Statistical Analysis

        /// <summary>
        /// F90_DAANST - Analyze time series statistics
        /// </summary>
        /// <param name="nVals">Number of values</param>
        /// <param name="values">Value array</param>
        public static void F90_DAANST(int nVals, float[] values)
        {
            try
            {
                if (values == null || nVals <= 0 || nVals > values.Length)
                    return;

                // Calculate basic statistics
                var stats = CalculateBasicStatistics(values.Take(nVals).ToArray());
                
                // Log the statistics
                HassEntFunctions.LogMsg($"Time Series Statistics for {nVals} values:");
                HassEntFunctions.LogMsg($"Mean: {stats.Mean:F3}");
                HassEntFunctions.LogMsg($"StdDev: {stats.StandardDeviation:F3}");
                HassEntFunctions.LogMsg($"Min: {stats.Minimum:F3}");
                HassEntFunctions.LogMsg($"Max: {stats.Maximum:F3}");
                HassEntFunctions.LogMsg($"Sum: {stats.Sum:F3}");
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error in F90_DAANST: {ex.Message}");
            }
        }

        /// <summary>
        /// F90_FITLIN - Fit linear regression line
        /// </summary>
        /// <param name="nPts">Number of points</param>
        /// <param name="bufMax">Buffer maximum size</param>
        /// <param name="yx">Y-X paired data array</param>
        /// <param name="aCoef">Output A coefficient (slope)</param>
        /// <param name="bCoef">Output B coefficient (intercept)</param>
        /// <param name="rSquare">Output R-squared</param>
        public static void F90_FITLIN(int nPts, int bufMax, float[] yx, out float aCoef, out float bCoef, out float rSquare)
        {
            try
            {
                if (yx == null || nPts <= 1 || nPts * 2 > yx.Length)
                {
                    aCoef = 0;
                    bCoef = 0;
                    rSquare = 0;
                    return;
                }

                // Extract X and Y values (assuming alternating Y,X pairs)
                var xValues = new float[nPts];
                var yValues = new float[nPts];
                
                for (int i = 0; i < nPts; i++)
                {
                    yValues[i] = yx[i * 2];     // Y values
                    xValues[i] = yx[i * 2 + 1]; // X values
                }

                // Calculate linear regression
                var regression = CalculateLinearRegression(xValues, yValues);
                
                aCoef = regression.Slope;
                bCoef = regression.Intercept;
                rSquare = regression.RSquared;
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error in F90_FITLIN: {ex.Message}");
                aCoef = 0;
                bCoef = 0;
                rSquare = 0;
            }
        }

        /// <summary>
        /// F90_CMSTRM - Calculate storm statistics
        /// </summary>
        /// <param name="start">Start date</param>
        /// <param name="endDt">End date</param>
        /// <param name="nStrm">Number of storms</param>
        /// <param name="lStrm">Storm length</param>
        /// <param name="fStrm">Storm factor</param>
        /// <param name="bufMax">Buffer maximum</param>
        /// <param name="yx">Data array</param>
        /// <param name="ts">Time step</param>
        /// <param name="tu">Time unit</param>
        /// <param name="nVals">Number of values</param>
        /// <param name="indx">Index array</param>
        /// <param name="cStrm">Storm count</param>
        /// <param name="stStrm">Storm start dates</param>
        /// <param name="enStrm">Storm end dates</param>
        /// <param name="trStrm">Storm totals</param>
        /// <param name="strmPk">Storm peaks</param>
        /// <param name="avsVl">Average values</param>
        public static void F90_CMSTRM(int[] start, int[] endDt, int nStrm, int lStrm, float fStrm,
            int bufMax, float[] yx, int ts, int tu, int nVals, int[] indx, int cStrm,
            int[,] stStrm, int[,] enStrm, float[] trStrm, float[] strmPk, float[] avsVl)
        {
            try
            {
                // Simplified storm analysis implementation
                if (yx == null || nVals <= 0) return;

                // Find storms based on threshold (fStrm)
                var stormPeriods = FindStormPeriods(yx, nVals, fStrm);
                
                int stormCount = Math.Min(stormPeriods.Count, nStrm);
                
                for (int i = 0; i < stormCount; i++)
                {
                    var storm = stormPeriods[i];
                    
                    // Calculate storm statistics
                    trStrm[i] = storm.Total;
                    strmPk[i] = storm.Peak;
                    avsVl[i] = storm.Average;
                    
                    // Set storm dates (simplified)
                    var stormStart = DateTimeUtilities.ArrayToDateTime(start).AddMinutes(storm.StartIndex * ts);
                    var stormEnd = DateTimeUtilities.ArrayToDateTime(start).AddMinutes(storm.EndIndex * ts);
                    
                    var startArray = DateTimeUtilities.DateTimeToArray(stormStart);
                    var endArray = DateTimeUtilities.DateTimeToArray(stormEnd);
                    
                    for (int j = 0; j < 6; j++)
                    {
                        stStrm[j, i] = startArray[j];
                        enStrm[j, i] = endArray[j];
                    }
                }
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error in F90_CMSTRM: {ex.Message}");
            }
        }

        #endregion

        #region Comparison and Validation

        /// <summary>
        /// F90_TSCBAT - Time series comparison batch analysis
        /// </summary>
        /// <param name="nPts">Number of points</param>
        /// <param name="yx">Paired Y-X data</param>
        /// <param name="nci">Number of class intervals</param>
        /// <param name="classes">Class boundaries</param>
        /// <param name="badVal">Bad values to ignore</param>
        /// <param name="sDate">Start date</param>
        /// <param name="eDate">End date</param>
        /// <param name="tu">Time unit</param>
        /// <param name="ts">Time step</param>
        /// <param name="dTran">Data transformation</param>
        /// <param name="zanb">Zero A, non-zero B count</param>
        /// <param name="zb">Zero B count</param>
        /// <param name="za">Zero A count</param>
        /// <param name="zbna">Zero B, non-zero A count</param>
        /// <param name="zab">Both zero count</param>
        /// <param name="tNum">Total number</param>
        /// <param name="tsDif">Time series difference</param>
        /// <param name="tpDif">Percent difference</param>
        /// <param name="tsDif2">Squared difference</param>
        /// <param name="tpDif2">Squared percent difference</param>
        /// <param name="tBias">Bias</param>
        /// <param name="tpBias">Percent bias</param>
        /// <param name="sTest">Statistical test</param>
        /// <param name="eTot">Error totals</param>
        /// <param name="cpctA">Percent A by class</param>
        /// <param name="cpctB">Percent B by class</param>
        /// <param name="label1">Label 1</param>
        /// <param name="label2">Label 2</param>
        /// <param name="fileName">Output file name</param>
        public static void F90_TSCBAT(int nPts, float[] yx, int nci, float[] classes, float[] badVal,
            int[] sDate, int[] eDate, int tu, int ts, int dTran,
            out int zanb, out int zb, out int za, out int zbna, out int zab, out int tNum,
            out float tsDif, out float tpDif, out float tsDif2, out float tpDif2,
            out float tBias, out float tpBias, out float sTest, int[] eTot,
            float[] cpctA, float[] cpctB, string label1, string label2, string fileName)
        {
            try
            {
                // Initialize output variables
                zanb = zb = za = zbna = zab = tNum = 0;
                tsDif = tpDif = tsDif2 = tpDif2 = tBias = tpBias = sTest = 0.0f;

                if (yx == null || nPts <= 0 || nPts * 2 > yx.Length)
                    return;

                // Extract paired data
                var aValues = new List<float>();
                var bValues = new List<float>();
                
                for (int i = 0; i < nPts; i++)
                {
                    float valA = yx[i * 2];
                    float valB = yx[i * 2 + 1];
                    
                    // Check for bad values
                    bool aIsBad = IsBadValue(valA, badVal);
                    bool bIsBad = IsBadValue(valB, badVal);
                    
                    if (!aIsBad && !bIsBad)
                    {
                        aValues.Add(valA);
                        bValues.Add(valB);
                    }
                }

                tNum = aValues.Count;
                if (tNum == 0) return;

                // Calculate comparison statistics
                var comparison = CalculateComparisonStatistics(aValues.ToArray(), bValues.ToArray());
                
                zanb = comparison.ZeroANonZeroB;
                zb = comparison.ZeroB;
                za = comparison.ZeroA;
                zbna = comparison.ZeroBNonZeroA;
                zab = comparison.BothZero;
                
                tsDif = comparison.TotalDifference;
                tpDif = comparison.PercentDifference;
                tsDif2 = comparison.SquaredDifference;
                tpDif2 = comparison.SquaredPercentDifference;
                tBias = comparison.Bias;
                tpBias = comparison.PercentBias;
                sTest = comparison.StatisticalTest;

                // Calculate class percentages
                CalculateClassPercentages(aValues.ToArray(), bValues.ToArray(), classes, nci, cpctA, cpctB);

                // Write output file if specified
                if (!string.IsNullOrEmpty(fileName))
                {
                    WriteComparisonResults(fileName, comparison, label1, label2);
                }
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error in F90_TSCBAT: {ex.Message}");
                zanb = zb = za = zbna = zab = tNum = 0;
                tsDif = tpDif = tsDif2 = tpDif2 = tBias = tpBias = sTest = 0.0f;
            }
        }

        #endregion

        #region Helper Classes and Methods

        /// <summary>
        /// Basic statistics structure
        /// </summary>
        public class BasicStatistics
        {
            public float Mean { get; set; }
            public float StandardDeviation { get; set; }
            public float Minimum { get; set; }
            public float Maximum { get; set; }
            public float Sum { get; set; }
            public float Variance { get; set; }
            public int Count { get; set; }
        }

        /// <summary>
        /// Linear regression result
        /// </summary>
        public class LinearRegressionResult
        {
            public float Slope { get; set; }
            public float Intercept { get; set; }
            public float RSquared { get; set; }
            public float CorrelationCoefficient { get; set; }
        }

        /// <summary>
        /// Storm period information
        /// </summary>
        public class StormPeriod
        {
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
            public float Total { get; set; }
            public float Peak { get; set; }
            public float Average { get; set; }
            public int Duration { get; set; }
        }

        /// <summary>
        /// Time series comparison result
        /// </summary>
        public class ComparisonStatistics
        {
            public int ZeroANonZeroB { get; set; }
            public int ZeroB { get; set; }
            public int ZeroA { get; set; }
            public int ZeroBNonZeroA { get; set; }
            public int BothZero { get; set; }
            public float TotalDifference { get; set; }
            public float PercentDifference { get; set; }
            public float SquaredDifference { get; set; }
            public float SquaredPercentDifference { get; set; }
            public float Bias { get; set; }
            public float PercentBias { get; set; }
            public float StatisticalTest { get; set; }
        }

        /// <summary>
        /// Calculate basic statistics
        /// </summary>
        /// <param name="values">Data values</param>
        /// <returns>Basic statistics</returns>
        private static BasicStatistics CalculateBasicStatistics(float[] values)
        {
            if (values == null || values.Length == 0)
                return new BasicStatistics();

            var result = new BasicStatistics
            {
                Count = values.Length,
                Minimum = values.Min(),
                Maximum = values.Max(),
                Sum = values.Sum(),
                Mean = values.Average()
            };

            // Calculate variance and standard deviation
            float variance = values.Select(x => (x - result.Mean) * (x - result.Mean)).Average();
            result.Variance = variance;
            result.StandardDeviation = (float)Math.Sqrt(variance);

            return result;
        }

        /// <summary>
        /// Calculate linear regression
        /// </summary>
        /// <param name="xValues">X values</param>
        /// <param name="yValues">Y values</param>
        /// <returns>Linear regression result</returns>
        private static LinearRegressionResult CalculateLinearRegression(float[] xValues, float[] yValues)
        {
            if (xValues == null || yValues == null || xValues.Length != yValues.Length || xValues.Length < 2)
                return new LinearRegressionResult();

            float n = xValues.Length;
            float sumX = xValues.Sum();
            float sumY = yValues.Sum();
            float sumXY = xValues.Zip(yValues, (x, y) => x * y).Sum();
            float sumXX = xValues.Select(x => x * x).Sum();
            float sumYY = yValues.Select(y => y * y).Sum();

            float meanX = sumX / n;
            float meanY = sumY / n;

            // Calculate slope and intercept
            float denominator = sumXX - (sumX * sumX) / n;
            float slope = denominator != 0 ? (sumXY - (sumX * sumY) / n) / denominator : 0;
            float intercept = meanY - slope * meanX;

            // Calculate R-squared
            float ssTot = sumYY - n * meanY * meanY;
            float ssRes = yValues.Zip(xValues, (y, x) => {
                float predicted = slope * x + intercept;
                return (y - predicted) * (y - predicted);
            }).Sum();

            float rSquared = ssTot != 0 ? 1 - (ssRes / ssTot) : 0;

            return new LinearRegressionResult
            {
                Slope = slope,
                Intercept = intercept,
                RSquared = rSquared,
                CorrelationCoefficient = (float)Math.Sqrt(Math.Max(0, rSquared))
            };
        }

        /// <summary>
        /// Find storm periods in time series data
        /// </summary>
        /// <param name="values">Data values</param>
        /// <param name="nVals">Number of values</param>
        /// <param name="threshold">Storm threshold</param>
        /// <returns>List of storm periods</returns>
        private static List<StormPeriod> FindStormPeriods(float[] values, int nVals, float threshold)
        {
            var storms = new List<StormPeriod>();
            
            bool inStorm = false;
            int stormStart = 0;
            float stormTotal = 0;
            float stormPeak = 0;

            for (int i = 0; i < Math.Min(nVals, values.Length); i++)
            {
                if (values[i] >= threshold)
                {
                    if (!inStorm)
                    {
                        // Start new storm
                        inStorm = true;
                        stormStart = i;
                        stormTotal = values[i];
                        stormPeak = values[i];
                    }
                    else
                    {
                        // Continue storm
                        stormTotal += values[i];
                        if (values[i] > stormPeak)
                            stormPeak = values[i];
                    }
                }
                else
                {
                    if (inStorm)
                    {
                        // End storm
                        int duration = i - stormStart;
                        storms.Add(new StormPeriod
                        {
                            StartIndex = stormStart,
                            EndIndex = i - 1,
                            Total = stormTotal,
                            Peak = stormPeak,
                            Average = duration > 0 ? stormTotal / duration : 0,
                            Duration = duration
                        });
                        
                        inStorm = false;
                        stormTotal = 0;
                        stormPeak = 0;
                    }
                }
            }

            // Handle storm at end of data
            if (inStorm)
            {
                int duration = nVals - stormStart;
                storms.Add(new StormPeriod
                {
                    StartIndex = stormStart,
                    EndIndex = nVals - 1,
                    Total = stormTotal,
                    Peak = stormPeak,
                    Average = duration > 0 ? stormTotal / duration : 0,
                    Duration = duration
                });
            }

            return storms;
        }

        /// <summary>
        /// Calculate comparison statistics between two time series
        /// </summary>
        /// <param name="aValues">First time series</param>
        /// <param name="bValues">Second time series</param>
        /// <returns>Comparison statistics</returns>
        private static ComparisonStatistics CalculateComparisonStatistics(float[] aValues, float[] bValues)
        {
            var result = new ComparisonStatistics();
            
            if (aValues == null || bValues == null || aValues.Length != bValues.Length)
                return result;

            float totalDiff = 0;
            float totalSquaredDiff = 0;
            float sumA = 0;
            float sumB = 0;

            for (int i = 0; i < aValues.Length; i++)
            {
                float a = aValues[i];
                float b = bValues[i];
                
                // Count zero combinations
                if (Math.Abs(a) < 1e-6 && Math.Abs(b) >= 1e-6) result.ZeroANonZeroB++;
                else if (Math.Abs(b) < 1e-6 && Math.Abs(a) >= 1e-6) result.ZeroBNonZeroA++;
                else if (Math.Abs(a) < 1e-6) result.ZeroA++;
                else if (Math.Abs(b) < 1e-6) result.ZeroB++;
                else if (Math.Abs(a) < 1e-6 && Math.Abs(b) < 1e-6) result.BothZero++;

                float diff = a - b;
                totalDiff += diff;
                totalSquaredDiff += diff * diff;
                sumA += a;
                sumB += b;
            }

            result.TotalDifference = totalDiff;
            result.SquaredDifference = totalSquaredDiff;
            result.Bias = totalDiff / aValues.Length;
            
            if (sumB != 0)
            {
                result.PercentDifference = (totalDiff / sumB) * 100;
                result.PercentBias = (result.Bias / (sumB / aValues.Length)) * 100;
            }

            // Nash-Sutcliffe efficiency as statistical test
            float meanA = sumA / aValues.Length;
            float sumSquaredDeviationA = aValues.Select(a => (a - meanA) * (a - meanA)).Sum();
            
            if (sumSquaredDeviationA != 0)
            {
                result.StatisticalTest = 1 - (totalSquaredDiff / sumSquaredDeviationA);
            }

            return result;
        }

        /// <summary>
        /// Check if value is bad/missing
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="badValues">Array of bad values</param>
        /// <returns>True if value is bad</returns>
        private static bool IsBadValue(float value, float[] badValues)
        {
            if (badValues == null) return false;
            
            foreach (float badVal in badValues)
            {
                if (Math.Abs(value - badVal) < 1e-6)
                    return true;
            }
            
            return float.IsNaN(value) || float.IsInfinity(value);
        }

        /// <summary>
        /// Calculate class percentages
        /// </summary>
        /// <param name="aValues">First series</param>
        /// <param name="bValues">Second series</param>
        /// <param name="classes">Class boundaries</param>
        /// <param name="nci">Number of class intervals</param>
        /// <param name="cpctA">Percent A by class</param>
        /// <param name="cpctB">Percent B by class</param>
        private static void CalculateClassPercentages(float[] aValues, float[] bValues, float[] classes, int nci, float[] cpctA, float[] cpctB)
        {
            if (aValues == null || bValues == null || classes == null || cpctA == null || cpctB == null)
                return;

            var aClassCounts = new int[nci];
            var bClassCounts = new int[nci];

            // Count values in each class
            for (int i = 0; i < aValues.Length; i++)
            {
                int aClass = FindClassIndex(aValues[i], classes, nci);
                int bClass = FindClassIndex(bValues[i], classes, nci);
                
                if (aClass >= 0 && aClass < nci) aClassCounts[aClass]++;
                if (bClass >= 0 && bClass < nci) bClassCounts[bClass]++;
            }

            // Calculate percentages
            for (int i = 0; i < Math.Min(nci, Math.Min(cpctA.Length, cpctB.Length)); i++)
            {
                cpctA[i] = aValues.Length > 0 ? (aClassCounts[i] * 100.0f) / aValues.Length : 0;
                cpctB[i] = bValues.Length > 0 ? (bClassCounts[i] * 100.0f) / bValues.Length : 0;
            }
        }

        /// <summary>
        /// Find class index for a value
        /// </summary>
        /// <param name="value">Value to classify</param>
        /// <param name="classes">Class boundaries</param>
        /// <param name="nci">Number of class intervals</param>
        /// <returns>Class index</returns>
        private static int FindClassIndex(float value, float[] classes, int nci)
        {
            for (int i = 0; i < Math.Min(nci - 1, classes.Length - 1); i++)
            {
                if (value >= classes[i] && value < classes[i + 1])
                    return i;
            }
            
            // Last class includes upper boundary
            if (nci > 0 && nci <= classes.Length && value >= classes[nci - 1])
                return nci - 1;
            
            return -1; // Not in any class
        }

        /// <summary>
        /// Write comparison results to file
        /// </summary>
        /// <param name="fileName">Output file name</param>
        /// <param name="comparison">Comparison statistics</param>
        /// <param name="label1">Label for first series</param>
        /// <param name="label2">Label for second series</param>
        private static void WriteComparisonResults(string fileName, ComparisonStatistics comparison, string label1, string label2)
        {
            try
            {
                using (var writer = new System.IO.StreamWriter(fileName))
                {
                    writer.WriteLine($"Time Series Comparison Results");
                    writer.WriteLine($"Series A: {label1}");
                    writer.WriteLine($"Series B: {label2}");
                    writer.WriteLine();
                    writer.WriteLine($"Zero A, Non-zero B: {comparison.ZeroANonZeroB}");
                    writer.WriteLine($"Zero B: {comparison.ZeroB}");
                    writer.WriteLine($"Zero A: {comparison.ZeroA}");
                    writer.WriteLine($"Zero B, Non-zero A: {comparison.ZeroBNonZeroA}");
                    writer.WriteLine($"Both Zero: {comparison.BothZero}");
                    writer.WriteLine();
                    writer.WriteLine($"Total Difference: {comparison.TotalDifference:F3}");
                    writer.WriteLine($"Percent Difference: {comparison.PercentDifference:F3}%");
                    writer.WriteLine($"Bias: {comparison.Bias:F3}");
                    writer.WriteLine($"Percent Bias: {comparison.PercentBias:F3}%");
                    writer.WriteLine($"Nash-Sutcliffe Efficiency: {comparison.StatisticalTest:F3}");
                }
            }
            catch (Exception ex)
            {
                HassEntFunctions.LogMsg($"Error writing comparison results: {ex.Message}");
            }
        }

        #endregion
    }
}