using System;
using System.IO;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Test program for specific data extraction based on user criteria
    /// Scenario = Observed, Location = 72030763, Constituent = WIND, ID = 20
    /// Date range: 2015-01-01 to 2019-12-31
    /// </summary>
    internal class SpecificDataExtractionTest
    {
        static void Main(string[] args)
        {
            Console.WriteLine("?? Specific WDM Data Extraction Test");
            Console.WriteLine("===================================");
            Console.WriteLine();

            try
            {
                // Your specific criteria
                string scenario = "Observed";
                string location = "72030763";
                string constituent = "WIND";
                int? datasetId = 20;
                DateTime startDate = new DateTime(2015, 1, 1);
                DateTime endDate = new DateTime(2019, 12, 31);

                // Path to WDM file
                string dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                string wdmFilePath = Path.Combine(dataFolder, "Sample.wdm");

                // Check if file exists
                if (!File.Exists(wdmFilePath))
                {
                    Console.WriteLine($"? WDM file not found: {wdmFilePath}");
                    Console.WriteLine("?? Note: This test uses Sample.wdm which may not contain your specific data.");
                    Console.WriteLine("?? Please replace with the path to your actual WDM file.");
                    return;
                }

                // Extract the specific data
                var extractedData = SpecificDataExtractor.ExtractSpecificData(
                    wdmFilePath, 
                    scenario, 
                    location, 
                    constituent, 
                    datasetId, 
                    startDate, 
                    endDate);

                if (extractedData.Count > 0)
                {
                    // Create output file names
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string basicCsvFile = $"extracted_data_{timestamp}.csv";
                    string enhancedCsvFile = $"extracted_data_enhanced_{timestamp}.csv";
                    
                    // Create criteria string for documentation
                    string criteria = $"Scenario={scenario}, Location={location}, Constituent={constituent}, ID={datasetId}, " +
                                    $"DateRange={startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}";

                    // Export in basic format
                    SpecificDataExtractor.ExportToCSV(extractedData, basicCsvFile, criteria);
                    
                    // Export in enhanced format (matching your original request)
                    SpecificDataExtractor.ExportToEnhancedCSV(
                        extractedData, 
                        enhancedCsvFile, 
                        location, 
                        constituent, 
                        criteria);

                    // Show summary
                    Console.WriteLine("\n?? Extraction Summary");
                    Console.WriteLine("====================");
                    Console.WriteLine($"?? Date Range: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
                    Console.WriteLine($"?? Records Found: {extractedData.Count}");
                    
                    if (extractedData.Count > 0)
                    {
                        var firstPoint = extractedData[0];
                        var lastPoint = extractedData[^1];
                        Console.WriteLine($"??? First Record: {firstPoint.DateTime:yyyy-MM-dd} = {firstPoint.Value}");
                        Console.WriteLine($"??? Last Record: {lastPoint.DateTime:yyyy-MM-dd} = {lastPoint.Value}");
                        
                        var minValue = extractedData.Min(d => d.Value);
                        var maxValue = extractedData.Max(d => d.Value);
                        var avgValue = extractedData.Average(d => d.Value);
                        
                        Console.WriteLine($"?? Value Range: {minValue:F3} to {maxValue:F3} (Average: {avgValue:F3})");
                    }

                    // Show file preview
                    Console.WriteLine("\n?? Enhanced CSV Preview (first 10 lines):");
                    Console.WriteLine("==========================================");
                    try
                    {
                        var lines = File.ReadAllLines(enhancedCsvFile);
                        for (int i = 0; i < Math.Min(10, lines.Length); i++)
                        {
                            Console.WriteLine($"  {lines[i]}");
                        }
                        if (lines.Length > 10)
                        {
                            Console.WriteLine($"  ... ({lines.Length - 10} more lines)");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  ? Could not preview file: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("\n? No data found matching your criteria.");
                    Console.WriteLine("\n?? Suggestions:");
                    Console.WriteLine("   1. Check if your WDM file contains the specified criteria");
                    Console.WriteLine("   2. Try broader search criteria (e.g., remove dataset ID constraint)");
                    Console.WriteLine("   3. Check the available data summary shown above");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Test failed: {ex.Message}");
                Console.WriteLine($"?? Details: {ex}");
            }

            Console.WriteLine("\nPress any key to exit...");
            try
            {
                Console.ReadKey();
            }
            catch
            {
                Console.WriteLine("Program finished.");
            }
        }
    }
}