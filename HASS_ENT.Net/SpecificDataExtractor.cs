using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Targeted WDM data extractor for specific dataset criteria
    /// Extracts data based on Scenario, Location, Constituent, ID, and date range
    /// </summary>
    public static class SpecificDataExtractor
    {
        /// <summary>
        /// Extract data from WDM file based on specific criteria
        /// </summary>
        /// <param name="wdmFilePath">Path to WDM file</param>
        /// <param name="scenario">Scenario name (e.g., "Observed")</param>
        /// <param name="location">Location ID (e.g., "72030763")</param>
        /// <param name="constituent">Constituent name (e.g., "WIND")</param>
        /// <param name="datasetId">Specific dataset ID (e.g., 20)</param>
        /// <param name="startDate">Start date for extraction</param>
        /// <param name="endDate">End date for extraction</param>
        /// <returns>Extracted time series data</returns>
        public static List<TimeSeriesDataPoint> ExtractSpecificData(
            string wdmFilePath, 
            string scenario, 
            string location, 
            string constituent, 
            int? datasetId, 
            DateTime startDate, 
            DateTime endDate)
        {
            var results = new List<TimeSeriesDataPoint>();
            
            try
            {
                Console.WriteLine("?? Specific Data Extraction");
                Console.WriteLine("==========================");
                Console.WriteLine($"?? WDM File: {Path.GetFileName(wdmFilePath)}");
                Console.WriteLine($"?? Scenario: {scenario}");
                Console.WriteLine($"?? Location: {location}");
                Console.WriteLine($"?? Constituent: {constituent}");
                Console.WriteLine($"?? Dataset ID: {datasetId?.ToString() ?? "Any"}");
                Console.WriteLine($"?? Date Range: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
                Console.WriteLine();

                if (!File.Exists(wdmFilePath))
                    throw new FileNotFoundException($"WDM file not found: {wdmFilePath}");

                HassEntLibrary.Initialize();
                
                int wdmUnit = HassEntFunctions.F90_WDBOPN(1, wdmFilePath);
                if (wdmUnit <= 0)
                    throw new InvalidOperationException($"Could not open WDM file: {wdmFilePath}");

                Console.WriteLine("?? Searching for matching datasets...");
                
                var matchingDatasets = FindMatchingDatasets(wdmUnit, scenario, location, constituent, datasetId);
                
                if (matchingDatasets.Count == 0)
                {
                    Console.WriteLine("? No datasets found matching the specified criteria.");
                    ShowAvailableData(wdmUnit);
                }
                else
                {
                    Console.WriteLine($"? Found {matchingDatasets.Count} matching dataset(s):");
                    
                    foreach (var dataset in matchingDatasets)
                    {
                        Console.WriteLine($"\n?? Dataset {dataset.Dsn}:");
                        Console.WriteLine($"   ?? Station: {dataset.StationId}");
                        Console.WriteLine($"   ?? Scenario: {dataset.Scenario}");
                        Console.WriteLine($"   ?? Variable: {dataset.Variable}");
                        Console.WriteLine($"   ?? Location: {dataset.Location}");
                        
                        var dataPoints = ExtractTimeSeriesData(wdmUnit, dataset.Dsn, startDate, endDate);
                        
                        Console.WriteLine($"   ?? Extracted {dataPoints.Count} data points");
                        
                        results.AddRange(dataPoints);
                    }
                }

                HassEntFunctions.F90_WDFLCL(wdmUnit);
                HassEntLibrary.Shutdown();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Extraction failed: {ex.Message}");
                throw;
            }

            return results;
        }

        /// <summary>
        /// Find datasets matching the specified criteria
        /// </summary>
        private static List<DatasetMatch> FindMatchingDatasets(
            int wdmUnit, 
            string scenario, 
            string location, 
            string constituent, 
            int? datasetId)
        {
            var matches = new List<DatasetMatch>();
            
            // Search through possible dataset numbers
            int searchLimit = datasetId.HasValue ? datasetId.Value + 1 : 2000;
            int searchStart = datasetId.HasValue ? datasetId.Value : 1;
            
            for (int dsn = searchStart; dsn < searchLimit; dsn++)
            {
                int datasetType = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
                if (datasetType <= 0) continue;
                
                var dataset = ExtractDatasetMetadata(wdmUnit, dsn);
                if (dataset == null) continue;
                
                // Check if dataset matches criteria
                bool matches_scenario = string.IsNullOrEmpty(scenario) || 
                    dataset.Scenario.Equals(scenario, StringComparison.OrdinalIgnoreCase);
                
                bool matches_location = string.IsNullOrEmpty(location) || 
                    dataset.Location.Equals(location, StringComparison.OrdinalIgnoreCase) ||
                    dataset.StationId.Equals(location, StringComparison.OrdinalIgnoreCase);
                
                bool matches_constituent = string.IsNullOrEmpty(constituent) || 
                    dataset.Variable.Equals(constituent, StringComparison.OrdinalIgnoreCase);
                
                bool matches_id = !datasetId.HasValue || dsn == datasetId.Value;
                
                if (matches_scenario && matches_location && matches_constituent && matches_id)
                {
                    matches.Add(dataset);
                }
            }
            
            return matches;
        }

        /// <summary>
        /// Extract metadata from a specific dataset
        /// </summary>
        private static DatasetMatch? ExtractDatasetMetadata(int wdmUnit, int dsn)
        {
            try
            {
                var dataset = new DatasetMatch { Dsn = dsn };

                // Extract station ID (attribute 2)
                int[] stationIdArray = new int[20];
                WdmOperations.F90_WDBSGC_XX(wdmUnit, dsn, 2, 20, stationIdArray);
                dataset.StationId = DataConversionUtilities.IntArrayToString(stationIdArray).Trim();

                // Extract scenario (attribute 288)
                int[] scenarioArray = new int[20];
                WdmOperations.F90_WDBSGC_XX(wdmUnit, dsn, 288, 20, scenarioArray);
                dataset.Scenario = DataConversionUtilities.IntArrayToString(scenarioArray).Trim();

                // Extract location (attribute 290)
                int[] locationArray = new int[20];
                WdmOperations.F90_WDBSGC_XX(wdmUnit, dsn, 290, 20, locationArray);
                dataset.Location = DataConversionUtilities.IntArrayToString(locationArray).Trim();

                // Extract constituent/variable (attribute 289)
                int[] constituentArray = new int[20];
                WdmOperations.F90_WDBSGC_XX(wdmUnit, dsn, 289, 20, constituentArray);
                dataset.Variable = DataConversionUtilities.IntArrayToString(constituentArray).Trim();
                
                // Try time series type if no constituent
                if (string.IsNullOrEmpty(dataset.Variable))
                {
                    int[] tsTypeArray = new int[10];
                    WdmOperations.F90_WDBSGC_XX(wdmUnit, dsn, 1, 10, tsTypeArray);
                    dataset.Variable = DataConversionUtilities.IntArrayToString(tsTypeArray).Trim();
                }

                return dataset;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Extract time series data for a specific dataset and date range
        /// </summary>
        private static List<TimeSeriesDataPoint> ExtractTimeSeriesData(
            int wdmUnit, 
            int dsn, 
            DateTime startDate, 
            DateTime endDate)
        {
            var dataPoints = new List<TimeSeriesDataPoint>();
            
            try
            {
                // Convert dates to WDM format
                int[] wdmStartDate = DateTimeUtilities.DateTimeToArray(startDate);
                int[] wdmEndDate = DateTimeUtilities.DateTimeToArray(endDate);
                
                // Calculate number of days in range
                int totalDays = (endDate - startDate).Days + 1;
                
                // Allocate arrays for data
                float[] values = new float[totalDays];
                
                // Extract data
                WdmOperations.F90_WDTGET(wdmUnit, dsn, 1440, wdmStartDate, totalDays, 0, 0, 4, values, out int getResult);
                
                if (getResult == 0)
                {
                    // Convert to time series data points
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (!float.IsNaN(values[i]) && values[i] != -999.0f && values[i] != -9999.0f)
                        {
                            var currentDate = startDate.AddDays(i);
                            dataPoints.Add(new TimeSeriesDataPoint
                            {
                                DateTime = currentDate,
                                Value = values[i],
                                DatasetNumber = dsn
                            });
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"?? Could not retrieve data for dataset {dsn} (error code: {getResult})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"?? Error extracting data from dataset {dsn}: {ex.Message}");
            }
            
            return dataPoints;
        }

        /// <summary>
        /// Show available data in the WDM file to help user understand what's available
        /// </summary>
        private static void ShowAvailableData(int wdmUnit)
        {
            Console.WriteLine("\n?? Available data in WDM file:");
            Console.WriteLine("==============================");
            
            var scenarios = new HashSet<string>();
            var locations = new HashSet<string>();
            var constituents = new HashSet<string>();
            var datasetIds = new List<int>();
            
            for (int dsn = 1; dsn <= 500; dsn++)
            {
                int datasetType = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
                if (datasetType <= 0) continue;
                
                var dataset = ExtractDatasetMetadata(wdmUnit, dsn);
                if (dataset != null)
                {
                    datasetIds.Add(dsn);
                    if (!string.IsNullOrEmpty(dataset.Scenario)) scenarios.Add(dataset.Scenario);
                    if (!string.IsNullOrEmpty(dataset.Location)) locations.Add(dataset.Location);
                    if (!string.IsNullOrEmpty(dataset.StationId)) locations.Add(dataset.StationId);
                    if (!string.IsNullOrEmpty(dataset.Variable)) constituents.Add(dataset.Variable);
                }
            }
            
            Console.WriteLine($"?? Scenarios ({scenarios.Count}): {string.Join(", ", scenarios.OrderBy(s => s).Take(10))}");
            Console.WriteLine($"?? Locations ({locations.Count}): {string.Join(", ", locations.OrderBy(l => l).Take(10))}");
            Console.WriteLine($"?? Constituents ({constituents.Count}): {string.Join(", ", constituents.OrderBy(c => c))}");
            Console.WriteLine($"?? Dataset IDs: {datasetIds.Min()} - {datasetIds.Max()} ({datasetIds.Count} total)");
            
            if (scenarios.Count > 10 || locations.Count > 10)
                Console.WriteLine("   (showing first 10 of each)");
        }

        /// <summary>
        /// Export extracted data to CSV format
        /// </summary>
        /// <param name="dataPoints">Extracted time series data</param>
        /// <param name="outputFileName">Output file name</param>
        /// <param name="criteria">Search criteria for documentation</param>
        public static void ExportToCSV(
            List<TimeSeriesDataPoint> dataPoints, 
            string outputFileName, 
            string criteria)
        {
            try
            {
                using var writer = new StreamWriter(outputFileName);
                
                // Write header with metadata
                writer.WriteLine("# WDM Data Extraction");
                writer.WriteLine($"# Criteria: {criteria}");
                writer.WriteLine($"# Extraction Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine($"# Total Records: {dataPoints.Count}");
                writer.WriteLine("#");
                writer.WriteLine("Dataset_ID,DateTime,Value");
                
                // Write data
                foreach (var point in dataPoints.OrderBy(p => p.DateTime))
                {
                    writer.WriteLine($"{point.DatasetNumber},{point.DateTime:yyyy-MM-dd HH:mm:ss},{point.Value:F6}");
                }
                
                Console.WriteLine($"? Data exported to: {outputFileName}");
                Console.WriteLine($"?? Total records exported: {dataPoints.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Export failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Enhanced export with Station_ID and Variable columns (like your original export format)
        /// </summary>
        /// <param name="dataPoints">Extracted time series data</param>
        /// <param name="outputFileName">Output file name</param>
        /// <param name="stationId">Station ID</param>
        /// <param name="variable">Variable name</param>
        /// <param name="criteria">Search criteria for documentation</param>
        public static void ExportToEnhancedCSV(
            List<TimeSeriesDataPoint> dataPoints, 
            string outputFileName, 
            string stationId, 
            string variable,
            string criteria)
        {
            try
            {
                using var writer = new StreamWriter(outputFileName);
                
                // Write header - matches your original format
                writer.WriteLine("Station_ID\t Variable\t DateTime\t Value");
                
                // Write data
                foreach (var point in dataPoints.OrderBy(p => p.DateTime))
                {
                    writer.WriteLine($"{stationId}\t{variable}\t{point.DateTime:M/d/yyyy H:mm}\t{point.Value}");
                }
                
                Console.WriteLine($"? Enhanced data exported to: {outputFileName}");
                Console.WriteLine($"?? Total records exported: {dataPoints.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Enhanced export failed: {ex.Message}");
                throw;
            }
        }
    }

    /// <summary>
    /// Dataset match information
    /// </summary>
    public class DatasetMatch
    {
        public int Dsn { get; set; }
        public string StationId { get; set; } = "";
        public string Scenario { get; set; } = "";
        public string Location { get; set; } = "";
        public string Variable { get; set; } = "";
    }

    /// <summary>
    /// Time series data point
    /// </summary>
    public class TimeSeriesDataPoint
    {
        public DateTime DateTime { get; set; }
        public float Value { get; set; }
        public int DatasetNumber { get; set; }
    }
}