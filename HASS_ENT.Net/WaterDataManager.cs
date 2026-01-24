using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Water data management and processing operations
    /// Handles time series data import, export, and manipulation for hydrologic modeling
    /// </summary>
    public class WaterDataManager : HassBaseOperation
    {
        public override string OperationName => "Water Data Management";

        private readonly Dictionary<string, TimeSeriesData> _timeSeries = new();
        private string _activeDataSource = "";

        /// <summary>
        /// Import time series data from file
        /// </summary>
        /// <param name="filePath">Path to data file</param>
        /// <param name="dataName">Name for the data series</param>
        /// <returns>True if successful</returns>
        public bool ImportTimeSeriesData(string filePath, string dataName)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    LogError($"Data file not found: {filePath}");
                    return false;
                }

                var timeSeries = new TimeSeriesData
                {
                    Name = dataName,
                    SourceFile = filePath,
                    ImportDate = DateTime.Now
                };

                // Simple CSV parsing
                using var reader = new StreamReader(filePath);
                string? line;
                bool skipHeader = true;

                while ((line = reader.ReadLine()) != null)
                {
                    if (skipHeader)
                    {
                        skipHeader = false;
                        continue;
                    }

                    var parts = line.Split(',');
                    if (parts.Length >= 2 &&
                        DateTime.TryParse(parts[0], out DateTime date) &&
                        float.TryParse(parts[1], out float value))
                    {
                        timeSeries.Values.Add(new DataPoint
                        {
                            DateTime = date,
                            Value = value
                        });
                    }
                }

                _timeSeries[dataName] = timeSeries;
                LogProgress($"Imported {timeSeries.Values.Count} data points for {dataName}");
                
                return true;
            }
            catch (Exception ex)
            {
                LogError($"Error importing time series data: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Export time series data to file
        /// </summary>
        /// <param name="dataName">Name of data series</param>
        /// <param name="filePath">Output file path</param>
        /// <returns>True if successful</returns>
        public bool ExportTimeSeriesData(string dataName, string filePath)
        {
            try
            {
                if (!_timeSeries.TryGetValue(dataName, out var timeSeries))
                {
                    LogError($"Time series not found: {dataName}");
                    return false;
                }

                using var writer = new StreamWriter(filePath);
                writer.WriteLine("DateTime,Value");

                foreach (var point in timeSeries.Values)
                {
                    writer.WriteLine($"{point.DateTime:yyyy-MM-dd HH:mm:ss},{point.Value}");
                }

                LogProgress($"Exported {timeSeries.Values.Count} data points to {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                LogError($"Error exporting time series data: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get available time series names
        /// </summary>
        /// <returns>List of time series names</returns>
        public List<string> GetTimeSeriesNames()
        {
            return new List<string>(_timeSeries.Keys);
        }

        /// <summary>
        /// Get time series data
        /// </summary>
        /// <param name="dataName">Name of data series</param>
        /// <returns>Time series data or null</returns>
        public TimeSeriesData? GetTimeSeries(string dataName)
        {
            return _timeSeries.TryGetValue(dataName, out var data) ? data : null;
        }

        protected override bool ExecuteInternal()
        {
            LogProgress("Validating all time series data");
            
            bool allValid = true;
            foreach (var kvp in _timeSeries)
            {
                var timeSeries = kvp.Value;
                if (timeSeries.Values.Count == 0)
                {
                    LogError($"Time series {kvp.Key} has no data points");
                    allValid = false;
                }
                else
                {
                    LogProgress($"Time series {kvp.Key}: {timeSeries.Values.Count} points");
                }
            }
            
            return allValid;
        }

        /// <summary>
        /// Calculate statistics for a time series
        /// </summary>
        /// <param name="dataName">Name of data series</param>
        /// <returns>Statistics or null if not found</returns>
        public TimeSeriesStatistics? CalculateStatistics(string dataName)
        {
            if (!_timeSeries.TryGetValue(dataName, out var timeSeries))
                return null;

            if (timeSeries.Values.Count == 0)
                return null;

            var values = timeSeries.Values.Select(p => p.Value).ToArray();
            
            return new TimeSeriesStatistics
            {
                Count = values.Length,
                Mean = values.Average(),
                Minimum = values.Min(),
                Maximum = values.Max(),
                StandardDeviation = CalculateStandardDeviation(values)
            };
        }

        private float CalculateStandardDeviation(float[] values)
        {
            if (values.Length <= 1) return 0;
            
            float mean = values.Average();
            float sumSquaredDiffs = values.Sum(v => (v - mean) * (v - mean));
            
            return (float)Math.Sqrt(sumSquaredDiffs / (values.Length - 1));
        }
    }

    /// <summary>
    /// Time series data structure
    /// </summary>
    public class TimeSeriesData
    {
        public string Name { get; set; } = "";
        public string SourceFile { get; set; } = "";
        public DateTime ImportDate { get; set; }
        public List<DataPoint> Values { get; set; } = new();
        public Dictionary<string, string> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Data point structure
    /// </summary>
    public class DataPoint
    {
        public DateTime DateTime { get; set; }
        public float Value { get; set; }
        public int Quality { get; set; } = 0;
    }

    /// <summary>
    /// Time series statistics
    /// </summary>
    public class TimeSeriesStatistics
    {
        public int Count { get; set; }
        public float Mean { get; set; }
        public float Minimum { get; set; }
        public float Maximum { get; set; }
        public float StandardDeviation { get; set; }
    }
}