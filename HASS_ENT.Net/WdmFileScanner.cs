using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Standalone WDM File Dataset Scanner
    /// Provides comprehensive analysis of WDM files including datasets, station IDs, variables, and date ranges
    /// </summary>
    public static class WdmFileScanner
    {
        /// <summary>
        /// Scan a WDM file and return detailed dataset information
        /// </summary>
        /// <param name="wdmFilePath">Path to WDM file</param>
        /// <param name="maxDsn">Maximum dataset number to scan (default 1500)</param>
        /// <returns>List of dataset information</returns>
        public static List<WdmDatasetInfo> ScanWdmFile(string wdmFilePath, int maxDsn = 1500)
        {
            if (!File.Exists(wdmFilePath))
                throw new FileNotFoundException($"WDM file not found: {wdmFilePath}");

            var datasets = new List<WdmDatasetInfo>();
            
            try
            {
                HassEntLibrary.Initialize();
                
                int wdmUnit = HassEntFunctions.F90_WDBOPN(1, wdmFilePath);
                if (wdmUnit <= 0)
                    throw new InvalidOperationException($"Could not open WDM file: {wdmFilePath}");

                Console.WriteLine($"?? Scanning WDM file: {Path.GetFileName(wdmFilePath)}");
                
                for (int dsn = 1; dsn <= maxDsn; dsn++)
                {
                    int datasetType = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
                    if (datasetType > 0)
                    {
                        Console.Write($"\r?? Found {datasets.Count + 1} datasets...");
                        
                        var datasetInfo = ExtractDatasetInfo(wdmUnit, dsn, datasetType);
                        if (datasetInfo != null)
                        {
                            datasets.Add(datasetInfo);
                        }
                    }
                }

                Console.WriteLine($"\r? Scan complete: Found {datasets.Count} datasets in {Path.GetFileName(wdmFilePath)}");
                
                HassEntFunctions.F90_WDFLCL(wdmUnit);
                HassEntLibrary.Shutdown();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Scan failed: {ex.Message}");
                throw;
            }

            return datasets;
        }

        /// <summary>
        /// Quick scan to get basic dataset list without detailed analysis
        /// </summary>
        /// <param name="wdmFilePath">Path to WDM file</param>
        /// <param name="maxDsn">Maximum dataset number to scan</param>
        /// <returns>List of basic dataset information</returns>
        public static List<BasicDatasetInfo> QuickScan(string wdmFilePath, int maxDsn = 1500)
        {
            if (!File.Exists(wdmFilePath))
                throw new FileNotFoundException($"WDM file not found: {wdmFilePath}");

            var datasets = new List<BasicDatasetInfo>();
            
            try
            {
                HassEntLibrary.Initialize();
                
                int wdmUnit = HassEntFunctions.F90_WDBOPN(1, wdmFilePath);
                if (wdmUnit <= 0)
                    throw new InvalidOperationException($"Could not open WDM file: {wdmFilePath}");

                for (int dsn = 1; dsn <= maxDsn; dsn++)
                {
                    int datasetType = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
                    if (datasetType > 0)
                    {
                        datasets.Add(new BasicDatasetInfo 
                        { 
                            Dsn = dsn, 
                            DatasetType = datasetType 
                        });
                    }
                }

                HassEntFunctions.F90_WDFLCL(wdmUnit);
                HassEntLibrary.Shutdown();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Quick scan failed: {ex.Message}");
                throw;
            }

            return datasets;
        }

        /// <summary>
        /// Extract detailed information from a specific dataset
        /// </summary>
        /// <param name="wdmUnit">WDM unit number</param>
        /// <param name="dsn">Dataset number</param>
        /// <param name="datasetType">Dataset type</param>
        /// <returns>Detailed dataset information</returns>
        private static WdmDatasetInfo? ExtractDatasetInfo(int wdmUnit, int dsn, int datasetType)
        {
            try
            {
                var info = new WdmDatasetInfo
                {
                    Dsn = dsn,
                    DatasetType = datasetType
                };

                // Extract all WDM attributes
                string stationId = "";
                string variable = "";
                string scenario = "";
                string location = "";
                string description = "";
                
                ExtractStringAttribute(wdmUnit, dsn, 2, out stationId, "STATION_" + dsn.ToString("D3"));
                ExtractStringAttribute(wdmUnit, dsn, 289, out variable);
                ExtractStringAttribute(wdmUnit, dsn, 288, out scenario);
                ExtractStringAttribute(wdmUnit, dsn, 290, out location);
                ExtractStringAttribute(wdmUnit, dsn, 45, out description);
                
                info.StationId = stationId;
                info.Variable = variable;
                info.Scenario = scenario;
                info.Location = location;
                info.Description = description;
                
                // Extract numeric attributes
                int timeStep = 0;
                ExtractIntegerAttribute(wdmUnit, dsn, 17, out timeStep);
                ExtractIntegerAttribute(wdmUnit, dsn, 27, out int timeUnitCode);
                ExtractIntegerAttribute(wdmUnit, dsn, 33, out int unitsCode);
                
                info.TimeStep = timeStep;
                info.TimeUnit = ConvertTimeUnitCode(timeUnitCode);
                info.Units = ConvertUnitsCode(unitsCode);

                // Handle variable fallback
                if (string.IsNullOrEmpty(info.Variable))
                {
                    string tsType = "";
                    ExtractStringAttribute(wdmUnit, dsn, 1, out tsType);
                    info.Variable = ConvertTimeSeriesType(tsType);
                }

                // Extract time series statistics
                ExtractTimeSeriesStatistics(wdmUnit, dsn, info);

                return info;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n?? Error extracting dataset {dsn}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Extract string attribute from WDM dataset
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        /// <param name="attrIndex">Attribute index</param>
        /// <param name="value">Output value</param>
        /// <param name="defaultValue">Default value if not found</param>
        private static void ExtractStringAttribute(int wdmUnit, int dsn, int attrIndex, out string value, string defaultValue = "")
        {
            try
            {
                int[] buffer = new int[50];
                WdmOperations.F90_WDBSGC_XX(wdmUnit, dsn, attrIndex, 50, buffer);
                value = DataConversionUtilities.IntArrayToString(buffer).Trim();
                
                if (string.IsNullOrEmpty(value))
                    value = defaultValue;
            }
            catch
            {
                value = defaultValue;
            }
        }

        /// <summary>
        /// Extract integer attribute from WDM dataset
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        /// <param name="attrIndex">Attribute index</param>
        /// <param name="value">Output value</param>
        private static void ExtractIntegerAttribute(int wdmUnit, int dsn, int attrIndex, out int value)
        {
            try
            {
                int[] buffer = new int[1];
                WdmOperations.F90_WDBSGI(wdmUnit, dsn, attrIndex, 1, buffer, out int retCode);
                value = retCode == 0 ? buffer[0] : 0;
            }
            catch
            {
                value = 0;
            }
        }

        /// <summary>
        /// Extract time series statistics and date range
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        /// <param name="info">Dataset info to update</param>
        private static void ExtractTimeSeriesStatistics(int wdmUnit, int dsn, WdmDatasetInfo info)
        {
            try
            {
                // Try to get a sample of data to determine statistics
                int[] startDate = { 1900, 1, 1, 0, 0, 0 };
                float[] values = new float[1000];
                int timeStepToUse = info.TimeStep > 0 ? info.TimeStep : 1440;
                
                WdmOperations.F90_WDTGET(wdmUnit, dsn, timeStepToUse, startDate, values.Length, 0, 0, 4, values, out int getResult);

                if (getResult == 0)
                {
                    var validValues = values.Where(v => !float.IsNaN(v) && v != -999.0f && v != -9999.0f).ToList();
                    
                    if (validValues.Count > 0)
                    {
                        info.DataPointCount = validValues.Count;
                        info.MinValue = validValues.Min();
                        info.MaxValue = validValues.Max();
                        info.MeanValue = validValues.Average();
                        
                        // Estimate date range
                        EstimateDateRange(info);
                    }
                }
            }
            catch
            {
                info.DataPointCount = 0;
            }
        }

        /// <summary>
        /// Estimate date range based on available information
        /// </summary>
        /// <param name="info">Dataset info to update</param>
        private static void EstimateDateRange(WdmDatasetInfo info)
        {
            try
            {
                // This is a simple estimation - in a real implementation you'd want to
                // use WDM functions to get actual start/end dates
                info.EstimatedStartDate = new DateTime(1990, 1, 1);
                
                if (info.TimeStep > 0 && info.DataPointCount > 0)
                {
                    var timeSpan = info.TimeUnit.ToLower() switch
                    {
                        var unit when unit.Contains("minute") => TimeSpan.FromMinutes(info.TimeStep * info.DataPointCount),
                        var unit when unit.Contains("hour") => TimeSpan.FromHours(info.TimeStep * info.DataPointCount),
                        var unit when unit.Contains("day") => TimeSpan.FromDays(info.TimeStep * info.DataPointCount),
                        _ => TimeSpan.FromDays(info.DataPointCount)
                    };
                    
                    info.EstimatedEndDate = info.EstimatedStartDate?.Add(timeSpan);
                }
            }
            catch
            {
                // If estimation fails, leave dates null
            }
        }

        /// <summary>
        /// Convert time unit code to description
        /// </summary>
        /// <param name="code">Time unit code</param>
        /// <returns>Time unit description</returns>
        private static string ConvertTimeUnitCode(int code)
        {
            return code switch
            {
                1 => "Seconds",
                2 => "Minutes",
                3 => "Hours", 
                4 => "Days",
                5 => "Months",
                6 => "Years",
                _ => $"Code_{code}"
            };
        }

        /// <summary>
        /// Convert units code to description
        /// </summary>
        /// <param name="code">Units code</param>
        /// <returns>Units description</returns>
        private static string ConvertUnitsCode(int code)
        {
            return code switch
            {
                1 => "CFS",
                2 => "Inches", 
                3 => "Degrees F",
                4 => "MGD",
                5 => "MM",
                6 => "Degrees C",
                7 => "M3/S",
                8 => "GPM",
                _ => $"Units_{code}"
            };
        }

        /// <summary>
        /// Convert time series type to variable name
        /// </summary>
        /// <param name="tsType">Time series type</param>
        /// <returns>Variable name</returns>
        private static string ConvertTimeSeriesType(string tsType)
        {
            if (string.IsNullOrEmpty(tsType)) return "DATA";
            
            return tsType.ToUpper().Trim() switch
            {
                "1" or "PREC" => "PREC",
                "2" or "PEVT" => "PEVT",
                "3" or "FLOW" => "FLOW",
                "4" or "TEMP" => "TEMP",
                "5" or "SRAD" => "SRAD",
                _ => tsType.ToUpper()
            };
        }

        /// <summary>
        /// Export dataset information to CSV file
        /// </summary>
        /// <param name="datasets">Dataset list</param>
        /// <param name="fileName">Output file name</param>
        public static void ExportToCsv(List<WdmDatasetInfo> datasets, string fileName)
        {
            try
            {
                using var writer = new StreamWriter(fileName);
                
                // Write header
                writer.WriteLine("DSN,Dataset_Type,Station_ID,Variable,Scenario,Location,Time_Step,Time_Unit,Units," +
                                "Data_Points,Min_Value,Max_Value,Mean_Value,Estimated_Start_Date,Estimated_End_Date,Description");
                
                // Write data
                foreach (var dataset in datasets.OrderBy(d => d.Dsn))
                {
                    writer.WriteLine($"{dataset.Dsn}," +
                                   $"{dataset.DatasetType}," +
                                   $"\"{dataset.StationId}\"," +
                                   $"\"{dataset.Variable}\"," +
                                   $"\"{dataset.Scenario}\"," +
                                   $"\"{dataset.Location}\"," +
                                   $"{dataset.TimeStep}," +
                                   $"\"{dataset.TimeUnit}\"," +
                                   $"\"{dataset.Units}\"," +
                                   $"{dataset.DataPointCount}," +
                                   $"{dataset.MinValue:F6}," +
                                   $"{dataset.MaxValue:F6}," +
                                   $"{dataset.MeanValue:F6}," +
                                   $"{dataset.EstimatedStartDate:yyyy-MM-dd HH:mm:ss}," +
                                   $"{dataset.EstimatedEndDate:yyyy-MM-dd HH:mm:ss}," +
                                   $"\"{dataset.Description}\"");
                }
                
                Console.WriteLine($"? Dataset list exported to: {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Export failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Display summary statistics for the datasets
        /// </summary>
        /// <param name="datasets">Dataset list</param>
        public static void DisplaySummary(List<WdmDatasetInfo> datasets)
        {
            if (datasets.Count == 0)
            {
                Console.WriteLine("? No datasets found");
                return;
            }

            Console.WriteLine($"\n?? WDM File Summary");
            Console.WriteLine($"==================");
            Console.WriteLine($"?? Total Datasets: {datasets.Count}");
            
            // Group by variable
            var variableGroups = datasets.GroupBy(d => d.Variable).OrderBy(g => g.Key);
            Console.WriteLine($"???  Variables: {variableGroups.Count()}");
            foreach (var group in variableGroups)
            {
                Console.WriteLine($"   • {group.Key}: {group.Count()} datasets");
            }
            
            // Group by station
            var stationGroups = datasets.GroupBy(d => d.StationId).OrderBy(g => g.Key);
            Console.WriteLine($"?? Stations: {stationGroups.Count()}");
            
            // Data availability
            var datasetsWithData = datasets.Where(d => d.DataPointCount > 0);
            var totalDataPoints = datasetsWithData.Sum(d => d.DataPointCount);
            Console.WriteLine($"?? Data Points: {totalDataPoints:N0} total");
            Console.WriteLine($"? Datasets with data: {datasetsWithData.Count()}/{datasets.Count}");
            
            // Date range
            var datesWithData = datasets.Where(d => d.EstimatedStartDate.HasValue).ToList();
            if (datesWithData.Count > 0)
            {
                var earliestDate = datesWithData.Min(d => d.EstimatedStartDate);
                var latestDate = datesWithData.Max(d => d.EstimatedEndDate);
                Console.WriteLine($"?? Date Range: {earliestDate:yyyy-MM-dd} to {latestDate:yyyy-MM-dd}");
            }
        }
    }

    /// <summary>
    /// Detailed WDM dataset information
    /// </summary>
    public class WdmDatasetInfo
    {
        public int Dsn { get; set; }
        public int DatasetType { get; set; }
        public string StationId { get; set; } = "";
        public string Variable { get; set; } = "";
        public string Scenario { get; set; } = "";
        public string Location { get; set; } = "";
        public int TimeStep { get; set; }
        public string TimeUnit { get; set; } = "";
        public string Units { get; set; } = "";
        public int DataPointCount { get; set; }
        public float? MinValue { get; set; }
        public float? MaxValue { get; set; }
        public float? MeanValue { get; set; }
        public DateTime? EstimatedStartDate { get; set; }
        public DateTime? EstimatedEndDate { get; set; }
        public string Description { get; set; } = "";
    }

    /// <summary>
    /// Basic dataset information for quick scans
    /// </summary>
    public class BasicDatasetInfo
    {
        public int Dsn { get; set; }
        public int DatasetType { get; set; }
    }
}