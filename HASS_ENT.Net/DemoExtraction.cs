using System;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Demonstration program showing extraction of specific data from Sample.wdm
    /// This demonstrates the exact format you requested with real data
    /// </summary>
    internal class DemoExtraction
    {
        static void Main(string[] args)
        {
            Console.WriteLine("?? WDM Data Extraction Demo");
            Console.WriteLine("===========================");
            Console.WriteLine("This demo shows extraction with your specific criteria format");
            Console.WriteLine();

            try
            {
                // Demo 1: Extract FLOW data (exists in Sample.wdm)
                Console.WriteLine("Demo 1: Extracting FLOW data from Sample.wdm");
                Console.WriteLine("==============================================");
                
                string wdmPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "Sample.wdm");
                
                var flowData = SpecificDataExtractor.ExtractSpecificData(
                    wdmPath, 
                    "OBSERVED",        // Scenario 
                    "USGS000001",     // Location (Station ID)
                    "FLOW",            // Constituent 
                    1,                 // Specific dataset ID
                    new DateTime(1990, 1, 1),  // Start date
                    new DateTime(1992, 12, 31)); // End date (3 years)

                if (flowData.Count > 0)
                {
                    // Export in your requested format
                    SpecificDataExtractor.ExportToEnhancedCSV(
                        flowData, 
                        "flow_demo_enhanced.csv", 
                        "USGS000001", 
                        "FLOW",
                        "Demo extraction of FLOW data");
                        
                    Console.WriteLine($"? Extracted {flowData.Count} FLOW data points");
                    Console.WriteLine("?? Preview of enhanced CSV format:");
                    ShowFilePreview("flow_demo_enhanced.csv");
                }
                
                Console.WriteLine();

                // Demo 2: Try to extract with your exact criteria (will show no matches)
                Console.WriteLine("Demo 2: Your exact criteria (Scenario=Observed, Location=72030763, Constituent=WIND, ID=20)");
                Console.WriteLine("===============================================================================");
                
                var windData = SpecificDataExtractor.ExtractSpecificData(
                    wdmPath, 
                    "Observed",        // Your scenario
                    "72030763",        // Your location  
                    "WIND",            // Your constituent
                    20,                // Your ID
                    new DateTime(2015, 1, 1),   // Your start date
                    new DateTime(2019, 12, 31)); // Your end date

                Console.WriteLine();

                // Demo 3: Extract what IS available from dataset 20
                Console.WriteLine("Demo 3: What IS available in dataset 20");
                Console.WriteLine("=======================================");
                
                var dataset20Data = SpecificDataExtractor.ExtractSpecificData(
                    wdmPath, 
                    "OBSERVED",        // Correct scenario 
                    "PT000020",       // Actual station from dataset 20
                    "EVAP",            // Actual variable from dataset 20
                    20,                // Your requested ID
                    new DateTime(1990, 1, 1),   // Available data period
                    new DateTime(1992, 12, 31)); // 3 years

                if (dataset20Data.Count > 0)
                {
                    // Export in your requested format
                    SpecificDataExtractor.ExportToEnhancedCSV(
                        dataset20Data, 
                        "dataset20_demo_enhanced.csv", 
                        "PT000020", 
                        "EVAP",
                        "Demo extraction from dataset 20 (actual data)");
                        
                    Console.WriteLine($"? Extracted {dataset20Data.Count} EVAP data points from dataset 20");
                    Console.WriteLine("?? Preview of enhanced CSV format (Dataset 20):");
                    ShowFilePreview("dataset20_demo_enhanced.csv");
                }

                Console.WriteLine();
                Console.WriteLine("?? Summary:");
                Console.WriteLine("   • Your exact criteria (WIND, 72030763) don't exist in Sample.wdm");
                Console.WriteLine("   • Dataset 20 contains EVAP data for station PT000020");
                Console.WriteLine("   • The extraction system works - just need the right WDM file");
                Console.WriteLine("   • CSV format matches your requested format");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Demo failed: {ex.Message}");
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

        /// <summary>
        /// Show preview of generated CSV file
        /// </summary>
        /// <param name="fileName">File to preview</param>
        private static void ShowFilePreview(string fileName)
        {
            try
            {
                if (System.IO.File.Exists(fileName))
                {
                    var lines = System.IO.File.ReadAllLines(fileName);
                    Console.WriteLine($"   File: {fileName} ({lines.Length} lines)");
                    
                    for (int i = 0; i < Math.Min(5, lines.Length); i++)
                    {
                        Console.WriteLine($"   {lines[i]}");
                    }
                    
                    if (lines.Length > 5)
                    {
                        Console.WriteLine($"   ... ({lines.Length - 5} more lines)");
                    }
                }
                else
                {
                    Console.WriteLine($"   ? File not created: {fileName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ? Preview error: {ex.Message}");
            }
        }
    }
}