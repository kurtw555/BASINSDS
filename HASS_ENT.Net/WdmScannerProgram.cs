using System;
using System.IO;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Simple command-line WDM scanner program
    /// Usage: dotnet run scan [wdm_file_path]
    /// </summary>
    internal class WdmScannerProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("WDM File Scanner");
            Console.WriteLine("================");
            Console.WriteLine();

            string wdmFilePath = "";

            // Check command line arguments
            if (args.Length > 1 && args[0].ToLower() == "scan")
            {
                wdmFilePath = args[1];
            }
            else
            {
                // Interactive mode - ask for file path
                Console.Write("Enter path to WDM file: ");
                wdmFilePath = Console.ReadLine()?.Trim() ?? "";
                
                // Try default location if no path provided
                if (string.IsNullOrEmpty(wdmFilePath))
                {
                    wdmFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "Sample.wdm");
                    Console.WriteLine($"Using default: {wdmFilePath}");
                }
            }

            if (!File.Exists(wdmFilePath))
            {
                Console.WriteLine($"? File not found: {wdmFilePath}");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }

            try
            {
                // Perform detailed scan
                Console.WriteLine($"?? Scanning: {Path.GetFileName(wdmFilePath)}");
                Console.WriteLine("This may take a moment for large files...\n");

                var datasets = WdmFileScanner.ScanWdmFile(wdmFilePath);

                // Display results
                if (datasets.Count > 0)
                {
                    WdmFileScanner.DisplaySummary(datasets);
                    
                    Console.WriteLine("\n?? Dataset Details:");
                    Console.WriteLine("{0,-5} {1,-15} {2,-10} {3,-12} {4,-8} {5,-12} {6,-15}",
                        "DSN", "Station_ID", "Variable", "Scenario", "Points", "Value_Range", "Period");
                    Console.WriteLine(new string('=', 85));

                    foreach (var dataset in datasets)
                    {
                        string valueRange = "N/A";
                        if (dataset.MinValue.HasValue && dataset.MaxValue.HasValue)
                        {
                            valueRange = $"{dataset.MinValue:F1}-{dataset.MaxValue:F1}";
                        }

                        string period = "No data";
                        if (dataset.EstimatedStartDate.HasValue && dataset.EstimatedEndDate.HasValue)
                        {
                            period = $"{dataset.EstimatedStartDate:yyyy}-{dataset.EstimatedEndDate:yyyy}";
                        }

                        Console.WriteLine("{0,-5} {1,-15} {2,-10} {3,-12} {4,-8} {5,-12} {6,-15}",
                            dataset.Dsn,
                            TruncateString(dataset.StationId, 15),
                            TruncateString(dataset.Variable, 10),
                            TruncateString(dataset.Scenario, 12),
                            dataset.DataPointCount.ToString("N0"),
                            TruncateString(valueRange, 12),
                            TruncateString(period, 15));
                    }

                    // Offer export options
                    Console.WriteLine("\nOptions:");
                    Console.WriteLine("E - Export to CSV");
                    Console.WriteLine("Q - Quit");
                    Console.Write("Enter choice: ");
                    
                    var choice = Console.ReadKey().KeyChar;
                    Console.WriteLine();

                    if (char.ToLower(choice) == 'e')
                    {
                        string csvFileName = $"{Path.GetFileNameWithoutExtension(wdmFilePath)}_scan_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                        WdmFileScanner.ExportToCsv(datasets, csvFileName);
                    }
                }
                else
                {
                    Console.WriteLine("? No datasets found in the WDM file.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Scanning failed: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Truncate string to fit in column
        /// </summary>
        /// <param name="text">Text to truncate</param>
        /// <param name="maxLength">Maximum length</param>
        /// <returns>Truncated text</returns>
        private static string TruncateString(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Length <= maxLength ? text : text.Substring(0, maxLength - 2) + "..";
        }
    }
}