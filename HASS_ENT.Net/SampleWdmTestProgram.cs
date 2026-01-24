using System;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Dedicated test program for Sample.wdm file operations
    /// Run this to perform comprehensive testing with the real WDM file
    /// </summary>
    internal class SampleWdmTestProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("HASS_ENT.Net - Sample.wdm Test Program");
            Console.WriteLine("=======================================");
            Console.WriteLine();

            if (args.Length > 0 && args[0].ToLower() == "export")
            {
                // Run export format test
                Console.WriteLine("Running Export Format Test...");
                QuickExportTest.RunTest();
            }
            else if (args.Length > 0 && args[0].ToLower() == "scan")
            {
                // Run WDM scanner
                Console.WriteLine("Running WDM Scanner...");
                if (args.Length > 1)
                {
                    // Use provided file path
                    RunWdmScanner(args[1]);
                }
                else
                {
                    // Use default Sample.wdm
                    string defaultPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "Sample.wdm");
                    RunWdmScanner(defaultPath);
                }
            }
            else if (args.Length > 0 && args[0].ToLower() == "full")
            {
                // Run full comprehensive tests
                RunFullTests();
            }
            else if (args.Length > 0 && args[0].ToLower() == "agg")
            {
                // Run aggregation test
                Console.WriteLine("Running Aggregation Test...");
                QuickAggregationTest.RunTest();
            }
            else if (args.Length > 0 && args[0].ToLower() == "label")
            {
                // Run label axis test
                Console.WriteLine("Running Label Axis Test...");
                QuickLabelAxisTest.RunTest();
            }
            else if (args.Length > 0 && args[0].ToLower() == "attr")
            {
                // Run generic attribute test
                Console.WriteLine("Running Generic Attribute Test...");
                QuickGenericAttributeTest.RunTest();
            }
            else if (args.Length > 0 && args[0].ToLower() == "addlabel")
            {
                // Run label add test
                Console.WriteLine("Running Label Add Test...");
                QuickLabelAddTest.RunTest();
            }
            else if (args.Length > 0 && args[0].ToLower() == "timefind")
            {
                // Run time find test
                Console.WriteLine("Running Time Find Test...");
                QuickTimeFindTest.RunTest();
            }
            else if (args.Length > 0 && args[0].ToLower() == "extract")
            {
                // Run specific data extraction
                Console.WriteLine("Running Specific Data Extraction...");
                RunSpecificDataExtraction(args);
            }
            else
            {
                // Run basic tests and provide menu
                RunInteractiveTests();
            }

            Console.WriteLine("\nPress any key to exit...");
            try
            {
                Console.ReadKey();
            }
            catch
            {
                // Ignore if not running in interactive mode
                Console.WriteLine("Program finished.");
            }
        }

        /// <summary>
        /// Run full comprehensive tests
        /// </summary>
        private static void RunFullTests()
        {
            try
            {
                SampleWdmTests.RunAllSampleWdmTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Full tests failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Run interactive test menu
        /// </summary>
        private static void RunInteractiveTests()
        {
            bool continueRunning = true;
            
            while (continueRunning)
            {
                ShowMenu();
                
                ConsoleKeyInfo key = Console.ReadKey(true);
                Console.WriteLine();
                
                try
                {
                    switch (key.KeyChar)
                    {
                        case '1':
                            Console.WriteLine("Running Simple WDM Demo...");
                            SampleWdmTests.RunSimpleWdmDemo();
                            break;
                            
                        case '2':
                            Console.WriteLine("Running All Sample WDM Tests...");
                            SampleWdmTests.RunAllSampleWdmTests();
                            break;
                            
                        case '3':
                            TestSpecificDataset();
                            break;
                            
                        case '4':
                            TestDataExport();
                            break;
                            
                        case '5':
                            ShowWdmFileInfo();
                            break;
                            
                        case '6':
                            TestTimeSeriesAnalysis();
                            break;
                            
                        case '7':
                            Console.WriteLine("Running Data Aggregation Test...");
                            QuickAggregationTest.RunTest();
                            break;
                            
                        case '8':
                            Console.WriteLine("Running Label Axis Test...");
                            TestLabelAxisFunction();
                            break;
                            
                        case '9':
                            TestGenericAttributesFunction();
                            break;
                            
                        case 'A':
                        case 'a':
                            Console.WriteLine("Running Add Labels Test...");
                            TestAddLabelsFunction();
                            break;
                            
                        case 'B':
                        case 'b':
                            Console.WriteLine("Running Find Time Data Test...");
                            TestFindTimeDataFunction();
                            break;
                            
                        case 'C':
                        case 'c':
                            Console.WriteLine("Scanning WDM Datasets...");
                            ScanWdmDatasets();
                            break;
                            
                        case 'D':
                        case 'd':
                            Console.WriteLine("Running Specific Data Extraction...");
                            RunInteractiveExtraction();
                            break;
                            
                        case 'q':
                        case 'Q':
                            continueRunning = false;
                            break;
                            
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Test failed: {ex.Message}");
                }
                
                if (continueRunning)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey(true);
                    Console.Clear();
                }
            }
        }

        /// <summary>
        /// Show interactive menu
        /// </summary>
        private static void ShowMenu()
        {
            Console.WriteLine("Sample.wdm Test Options:");
            Console.WriteLine("========================");
            Console.WriteLine("1. Simple WDM Demo");
            Console.WriteLine("2. Run All Comprehensive Tests");
            Console.WriteLine("3. Test Specific Dataset");
            Console.WriteLine("4. Test Data Export");
            Console.WriteLine("5. Show WDM File Info");
            Console.WriteLine("6. Test Time Series Analysis");
            Console.WriteLine("7. Test Data Aggregation (F90_WDSAGY_XX)");
            Console.WriteLine("8. Test Label Axis (F90_WDLBAX)");
            Console.WriteLine("9. Test Generic Attributes (F90_GETATT)");
            Console.WriteLine("A. Test Add Labels (F90_WDLBAD)");
            Console.WriteLine("B. Test Find Time Data (F90_WTFNDT)");
            Console.WriteLine("C. Scan WDM Datasets (Detailed Analysis)");
            Console.WriteLine("D. Extract Specific Data (Scenario/Location/Constituent)");
            Console.WriteLine("Q. Quit");
            Console.WriteLine();
            Console.Write("Select an option: ");
        }

        /// <summary>
        /// Test a specific dataset interactively
        /// </summary>
        private static void TestSpecificDataset()
        {
            Console.WriteLine("Test Specific Dataset");
            Console.WriteLine("====================");
            
            Console.Write("Enter dataset number (e.g., 100): ");
            string input = Console.ReadLine() ?? "";
            
            if (int.TryParse(input, out int dsn))
            {
                TestDatasetDetails(dsn);
            }
            else
            {
                Console.WriteLine("Invalid dataset number");
            }
        }

        /// <summary>
        /// Test detailed information about a specific dataset
        /// </summary>
        /// <param name="dsn">Dataset number</param>
        private static void TestDatasetDetails(int dsn)
        {
            try
            {
                HassEntLibrary.Initialize();
                
                string wdmPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "Sample.wdm");
                int wdmUnit = HassEntFunctions.F90_WDBOPN(1, wdmPath);
                
                if (wdmUnit > 0)
                {
                    Console.WriteLine($"\nTesting Dataset {dsn}:");
                    Console.WriteLine("=====================");
                    
                    // Check if dataset exists
                    int datasetType = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
                    
                    if (datasetType > 0)
                    {
                        Console.WriteLine($"? Dataset {dsn} exists with type: {datasetType}");
                        
                        // Get attributes
                        Console.WriteLine("\nAttributes:");
                        
                        // Station ID
                        int[] stationId = new int[16];
                        WdmOperations.F90_WDBSGC_XX(wdmUnit, dsn, 2, 16, stationId);
                        string station = DataConversionUtilities.IntArrayToString(stationId).Trim();
                        if (!string.IsNullOrEmpty(station))
                            Console.WriteLine($"  Station ID: {station}");
                        
                        // Time step
                        int[] timeStep = new int[1];
                        WdmOperations.F90_WDBSGI(wdmUnit, dsn, 17, 1, timeStep, out int retCode);
                        if (retCode == 0 && timeStep[0] > 0)
                            Console.WriteLine($"  Time Step: {timeStep[0]}");
                        
                        // Date range - simplified without unavailable functions
                        int[] startDate = { 1990, 1, 1, 0, 0, 0 };
                        Console.WriteLine($"    Estimated Start Date: {startDate[0]:D4}-{startDate[1]:D2}-{startDate[2]:D2}");
                        
                        // Try to get some data
                        Console.WriteLine("\nData Sample:");
                        int[] dataSample = new int[5];
                        int[] actualDate = new int[6];
                        float[] values = new float[10];
                        
                        // Try reading a small sample of data
                        WdmOperations.F90_WDTGET(wdmUnit, dsn, 1, startDate, 5, 0, 0, 4, values, out int getResult);
                        
                        if (getResult == 0)
                        {
                            Console.WriteLine("  First 5 values:");
                            for (int i = 0; i < values.Length; i++)
                            {
                                if (!float.IsNaN(values[i]))
                                    Console.WriteLine($"    {i + 1}: {values[i]:F2}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"  Could not retrieve data sample (code: {getResult})");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"? Dataset {dsn} does not exist");
                    }
                    
                    HassEntFunctions.F90_WDFLCL(wdmUnit);
                }
                else
                {
                    Console.WriteLine("? Could not open Sample.wdm file");
                }
                
                HassEntLibrary.Shutdown();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error testing dataset {dsn}: {ex.Message}");
            }
        }

        /// <summary>
        /// Run WDM scanner on specified file
        /// </summary>
        /// <param name="wdmFilePath">Path to WDM file</param>
        private static void RunWdmScanner(string wdmFilePath)
        {
            try
            {
                if (!System.IO.File.Exists(wdmFilePath))
                {
                    Console.WriteLine($"? WDM file not found: {wdmFilePath}");
                    return;
                }

                Console.WriteLine($"?? Scanning: {System.IO.Path.GetFileName(wdmFilePath)}");
                Console.WriteLine("Please wait...\n");

                var datasets = WdmFileScanner.ScanWdmFile(wdmFilePath);

                if (datasets.Count > 0)
                {
                    WdmFileScanner.DisplaySummary(datasets);

                    // Export to CSV automatically
                    string csvFileName = $"{System.IO.Path.GetFileNameWithoutExtension(wdmFilePath)}_scan_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                    WdmFileScanner.ExportToCsv(datasets, csvFileName);
                }
                else
                {
                    Console.WriteLine("? No datasets found in the WDM file.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? WDM scanning failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Test data export functionality
        /// </summary>
        private static void TestDataExport()
        {
            Console.WriteLine("Test Data Export");
            Console.WriteLine("================");
            
            Console.Write("Enter dataset number to export: ");
            string input = Console.ReadLine() ?? "";
            
            if (int.TryParse(input, out int dsn))
            {
                Console.WriteLine("Select export format:");
                Console.WriteLine("1. Standard format (Date/Time, Value)");
                Console.WriteLine("2. Enhanced format (Station_ID, Variable, DateTime, Value)");
                Console.Write("Enter choice (1 or 2): ");
                string formatChoice = Console.ReadLine() ?? "1";
                
                bool useEnhanced = formatChoice == "2";
                
                Console.WriteLine("Select data format:");
                Console.WriteLine("1. F10.2 (2 decimal places)");
                Console.WriteLine("2. F10.3 (3 decimal places)");  
                Console.WriteLine("3. F10.5 (5 decimal places)");
                Console.WriteLine("4. E12.4 (Scientific notation)");
                Console.WriteLine("5. Custom");
                Console.Write("Enter choice (1-5): ");
                string dataFormatChoice = Console.ReadLine() ?? "1";
                
                string dataFormat = dataFormatChoice switch
                {
                    "2" => "F10.3",
                    "3" => "F10.5", 
                    "4" => "E12.4",
                    "5" => GetCustomFormat(),
                    _ => "F10.2"
                };
                
                try
                {
                    HassEntLibrary.Initialize();
                    
                    string wdmPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "Sample.wdm");
                    int wdmUnit = HassEntFunctions.F90_WDBOPN(1, wdmPath);
                    
                    if (wdmUnit > 0)
                    {
                        int datasetType = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
                        
                        if (datasetType > 0)
                        {
                            string suffix = useEnhanced ? "_enhanced" : "_standard";
                            string formatSuffix = dataFormat.Replace(".", "_");
                            string exportFile = $"export_dsn_{dsn}{suffix}_{formatSuffix}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                            
                            int[] startDate = { 1990, 1, 1, 0, 0, 0 };
                            int[] endDate = { 2025, 12, 31, 23, 59, 59 };
                            
                            if (useEnhanced)
                            {
                                FileManagement.F90_TSFLAT_EX(wdmUnit, dsn, exportFile, 1,
                                    dataFormat, 0, 1440, 4, 0, 1, -999.0f, startDate, endDate, true, out int result);
                                
                                if (result == 0 && System.IO.File.Exists(exportFile))
                                {
                                    var fileInfo = new System.IO.FileInfo(exportFile);
                                    Console.WriteLine($"? Enhanced export successful: {exportFile} ({fileInfo.Length:N0} bytes)");
                                    Console.WriteLine($"?? Format: {dataFormat}");
                                    
                                    ShowExportSample(exportFile, "Enhanced format (Station_ID, Variable, DateTime, Value)");
                                }
                                else
                                {
                                    Console.WriteLine($"? Enhanced export failed (result: {result})");
                                }
                            }
                            else
                            {
                                FileManagement.F90_TSFLAT(wdmUnit, dsn, exportFile, 1,
                                    dataFormat, 0, 1440, 4, 0, 1, -999.0f, startDate, endDate, out int result);
                                
                                if (result == 0 && System.IO.File.Exists(exportFile))
                                {
                                    var fileInfo = new System.IO.FileInfo(exportFile);
                                    Console.WriteLine($"? Standard export successful: {exportFile} ({fileInfo.Length:N0} bytes)");
                                    Console.WriteLine($"?? Format: {dataFormat}");
                                    
                                    ShowExportSample(exportFile, "Standard format (Date/Time, Value)");
                                }
                                else
                                {
                                    Console.WriteLine($"? Standard export failed (result: {result})");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"? Dataset {dsn} does not exist");
                        }
                        
                        HassEntFunctions.F90_WDFLCL(wdmUnit);
                    }
                    
                    HassEntLibrary.Shutdown();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Export failed: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid dataset number");
            }
        }

        /// <summary>
        /// Get custom format from user
        /// </summary>
        /// <returns>Custom format string</returns>
        private static string GetCustomFormat()
        {
            Console.Write("Enter custom format (e.g., F15.6, E10.3): ");
            string custom = Console.ReadLine()?.Trim() ?? "";
            return string.IsNullOrEmpty(custom) ? "F10.2" : custom;
        }

        /// <summary>
        /// Show sample of exported file
        /// </summary>
        /// <param name="fileName">Export file name</param>
        /// <param name="description">Format description</param>
        private static void ShowExportSample(string fileName, string description)
        {
            try
            {
                string[] lines = System.IO.File.ReadAllLines(fileName);
                Console.WriteLine($"\n{description} - File contains {lines.Length} lines");
                Console.WriteLine("First few lines:");
                
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
                Console.WriteLine($"Error reading export file: {ex.Message}");
            }
        }

        /// <summary>
        /// Show WDM file information
        /// </summary>
        private static void ShowWdmFileInfo()
        {
            Console.WriteLine("WDM File Information");
            Console.WriteLine("===================");
            
            try
            {
                string dataFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                string sampleWdm = System.IO.Path.Combine(dataFolder, "Sample.wdm");
                string annualWdm = System.IO.Path.Combine(dataFolder, "Sample_Annual.wdm");
                
                Console.WriteLine("Available WDM files:");
                
                if (System.IO.File.Exists(sampleWdm))
                {
                    var info = new System.IO.FileInfo(sampleWdm);
                    Console.WriteLine($"  Sample.wdm: {info.Length:N0} bytes, Modified: {info.LastWriteTime:yyyy-MM-dd HH:mm}");
                }
                else
                {
                    Console.WriteLine("  Sample.wdm: Not found");
                }
                
                if (System.IO.File.Exists(annualWdm))
                {
                    var info = new System.IO.FileInfo(annualWdm);
                    Console.WriteLine($"  Sample_Annual.wdm: {info.Length:N0} bytes, Modified: {info.LastWriteTime:yyyy-MM-dd HH:mm}");
                }
                else
                {
                    Console.WriteLine("  Sample_Annual.wdm: Not found");
                }
                
                // Quick dataset scan
                if (System.IO.File.Exists(sampleWdm))
                {
                    HassEntLibrary.Initialize();
                    
                    int wdmUnit = HassEntFunctions.F90_WDBOPN(1, sampleWdm);
                    if (wdmUnit > 0)
                    {
                        Console.WriteLine("\nDataset scan:");
                        int count = 0;
                        
                        for (int dsn = 1; dsn <= 1000; dsn++)
                        {
                            int type = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
                            if (type > 0)
                            {
                                count++;
                                if (count <= 20) // Show first 20
                                {
                                    Console.WriteLine($"  DSN {dsn}: Type {type}");
                                }
                            }
                        }
                        
                        Console.WriteLine($"\nTotal datasets found: {count}");
                        HassEntFunctions.F90_WDFLCL(wdmUnit);
                    }
                    
                    HassEntLibrary.Shutdown();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting file info: {ex.Message}");
            }
        }

        /// <summary>
        /// Test time series analysis
        /// </summary>
        private static void TestTimeSeriesAnalysis()
        {
            Console.WriteLine("Test Time Series Analysis");
            Console.WriteLine("=========================");
            
            Console.Write("Enter dataset number for analysis: ");
            string input = Console.ReadLine() ?? "";
            
            if (int.TryParse(input, out int dsn))
            {
                try
                {
                    HassEntLibrary.Initialize();
                    
                    string wdmPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "Sample.wdm");
                    int wdmUnit = HassEntFunctions.F90_WDBOPN(1, wdmPath);
                    
                    if (wdmUnit > 0)
                    {
                        int datasetType = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
                        
                        if (datasetType > 0)
                        {
                            // Get a larger sample for analysis
                            int[] startDate = { 1990, 1, 1, 0, 0, 0 };
                            float[] values = new float[365]; // One year of daily data
                            
                            WdmOperations.F90_WDTGET(wdmUnit, dsn, 1440, startDate, 365, 0, 0, 4, values, out int getResult);
                            
                            if (getResult == 0)
                            {
                                // Filter valid data
                                var validData = new System.Collections.Generic.List<float>();
                                foreach (float value in values)
                                {
                                    if (!float.IsNaN(value) && value != -999.0f)
                                    {
                                        validData.Add(value);
                                    }
                                }
                                
                                if (validData.Count > 0)
                                {
                                    Console.WriteLine($"\nAnalyzing {validData.Count} valid values from dataset {dsn}:");
                                    
                                    // Run statistical analysis
                                    TimeSeriesAnalysis.F90_DAANST(validData.Count, validData.ToArray());
                                    
                                    // Calculate additional statistics
                                    float min = validData.Min();
                                    float max = validData.Max();
                                    float mean = validData.Average();
                                    float sum = validData.Sum();
                                    
                                    Console.WriteLine($"Summary Statistics:");
                                    Console.WriteLine($"  Count: {validData.Count}");
                                    Console.WriteLine($"  Minimum: {min:F2}");
                                    Console.WriteLine($"  Maximum: {max:F2}");
                                    Console.WriteLine($"  Mean: {mean:F2}");
                                    Console.WriteLine($"  Sum: {sum:F2}");
                                }
                                else
                                {
                                    Console.WriteLine($"No valid data found in dataset {dsn}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Could not retrieve data from dataset {dsn} (code: {getResult})");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Dataset {dsn} does not exist");
                        }
                        
                        HassEntFunctions.F90_WDFLCL(wdmUnit);
                    }
                    
                    HassEntLibrary.Shutdown();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Analysis failed: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid dataset number");
            }
        }

        /// <summary>
        /// Test data aggregation
        /// </summary>
        private static void TestDataAggregation()
        {
            Console.WriteLine("Test Data Aggregation (F90_WDSAGY_XX)");
            Console.WriteLine("=====================================");
            
            Console.Write("Enter dataset number for aggregation: ");
            string input = Console.ReadLine() ?? "";
            
            if (int.TryParse(input, out int dsn))
            {
                try
                {
                    HassEntLibrary.Initialize();
                    
                    string wdmPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "Sample.wdm");
                    int wdmUnit = HassEntFunctions.F90_WDBOPN(1, wdmPath);
                    
                    if (wdmUnit > 0)
                    {
                        int datasetType = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
                        
                        if (datasetType > 0)
                        {
                            Console.WriteLine($"\nTesting aggregation for dataset {dsn}:");
                            
                            // Create some test data first
                            CreateTestDataForAggregation(wdmUnit, dsn);
                            
                            // Define aggregation parameters
                            int[] startDate = { 2023, 1, 1, 0, 0, 0 };
                            int[] endDate = { 2023, 2, 28, 23, 59, 59 };
                            
                            // Test different aggregation types
                            TestAggregationType(wdmUnit, dsn, 1, "Sum", startDate, endDate, 7, 4); // Weekly sum
                            TestAggregationType(wdmUnit, dsn, 2, "Mean", startDate, endDate, 7, 4); // Weekly mean
                            TestAggregationType(wdmUnit, dsn, 3, "Maximum", startDate, endDate, 7, 4); // Weekly max
                            TestAggregationType(wdmUnit, dsn, 4, "Minimum", startDate, endDate, 7, 4); // Weekly min
                            TestAggregationType(wdmUnit, dsn, 5, "Count", startDate, endDate, 7, 4); // Weekly count
                        }
                        else
                        {
                            Console.WriteLine($"? Dataset {dsn} does not exist");
                        }
                        
                        HassEntFunctions.F90_WDFLCL(wdmUnit);
                    }
                    
                    HassEntLibrary.Shutdown();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Aggregation test failed: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid dataset number");
            }
        }

        /// <summary>
        /// Create test data for aggregation testing
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        private static void CreateTestDataForAggregation(int wdmUnit, int dsn)
        {
            try
            {
                // Get WDM file info
                var wdmInfo = WdmOperations.GetWdmFileInfo(wdmUnit);
                if (wdmInfo != null)
                {
                    // Create test dataset with sample data
                    var testDataSet = new WdmOperations.WdmDataSet
                    {
                        Dsn = dsn,
                        DataSetType = 1,
                        TimeUnit = 4,  // Daily
                        TimeStep = 1
                    };

                    // Add some test time series data (2 months of daily data)
                    var baseDate = new DateTime(2023, 1, 1);
                    for (int i = 0; i < 60; i++)
                    {
                        testDataSet.Data.Add(new WdmOperations.TimeSeriesValue
                        {
                            DateTime = baseDate.AddDays(i),
                            Value = 10.0f + (float)Math.Sin(i * 0.1) * 5.0f + (float)(i % 7), // Varied pattern
                            Quality = 0
                        });
                    }

                    // Update or add the test dataset
                    wdmInfo.DataSets[dsn] = testDataSet;
                    Console.WriteLine($"? Created test data: {testDataSet.Data.Count} daily values");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating test data: {ex.Message}");
            }
        }

        /// <summary>
        /// Test a specific aggregation type
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        /// <param name="aggType">Aggregation type</param>
        /// <param name="aggName">Aggregation name for display</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <param name="timeStep">Time step</param>
        /// <param name="timeUnit">Time unit</param>
        private static void TestAggregationType(int wdmUnit, int dsn, int aggType, string aggName, 
            int[] startDate, int[] endDate, int timeStep, int timeUnit)
        {
            try
            {
                float[] outputValues = new float[20];
                int[,] outputDates = new int[6, 20];
                
                WdmOperations.F90_WDSAGY_XX(wdmUnit, dsn, aggType, startDate, endDate, 
                    timeStep, timeUnit, outputValues, outputDates, out int nValues, out int retCode);
                
                if (retCode == 0 && nValues > 0)
                {
                    Console.WriteLine($"? {aggName} aggregation: {nValues} periods");
                    Console.WriteLine($"   First value: {outputValues[0]:F2} on {outputDates[0,0]}/{outputDates[1,0]}/{outputDates[2,0]}");
                    if (nValues > 1)
                        Console.WriteLine($"   Last value: {outputValues[nValues-1]:F2} on {outputDates[0,nValues-1]}/{outputDates[1,nValues-1]}/{outputDates[2,nValues-1]}");
                    
                    // Show statistics of aggregated values
                    float total = 0;
                    for (int i = 0; i < nValues; i++)
                        total += outputValues[i];
                    Console.WriteLine($"   Average of aggregated values: {total/nValues:F2}");
                }
                else
                {
                    Console.WriteLine($"? {aggName} aggregation failed (code: {retCode})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? {aggName} aggregation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Test label axis functionality
        /// </summary>
        private static void TestLabelAxisFunction()
        {
            Console.WriteLine("Test Label Axis (F90_WDLBAX)");
            Console.WriteLine("=============================");
            
            Console.Write("Enter dataset number for label testing: ");
            string input = Console.ReadLine() ?? "";
            
            if (int.TryParse(input, out int dsn))
            {
                try
                {
                    HassEntLibrary.Initialize();
                    
                    string wdmPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "Sample.wdm");
                    int wdmUnit = HassEntFunctions.F90_WDBOPN(1, wdmPath);
                    
                    if (wdmUnit > 0)
                    {
                        int datasetType = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
                        
                        if (datasetType > 0)
                        {
                            Console.WriteLine($"\nTesting labels for dataset {dsn}:");
                            
                            // Create some test attributes first if dataset exists but has no labels
                            CreateTestLabelsForDataset(wdmUnit, dsn);
                            
                            // Test different label types
                            TestSingleLabelType(wdmUnit, dsn, 1, "Station");
                            TestSingleLabelType(wdmUnit, dsn, 2, "Parameter");
                            TestSingleLabelType(wdmUnit, dsn, 3, "Time");
                            TestSingleLabelType(wdmUnit, dsn, 4, "Units");
                            TestSingleLabelType(wdmUnit, dsn, 5, "Scenario");
                            TestSingleLabelType(wdmUnit, dsn, 6, "Description");
                        }
                        else
                        {
                            Console.WriteLine($"? Dataset {dsn} does not exist");
                        }
                        
                        HassEntFunctions.F90_WDFLCL(wdmUnit);
                    }
                    
                    HassEntLibrary.Shutdown();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Label axis test failed: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid dataset number");
            }
        }

        /// <summary>
        /// Create test labels for a dataset
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        private static void CreateTestLabelsForDataset(int wdmUnit, int dsn)
        {
            try
            {
                var wdmInfo = WdmOperations.GetWdmFileInfo(wdmUnit);
                if (wdmInfo != null && wdmInfo.DataSets.TryGetValue(dsn, out var dataSet))
                {
                    // Add some sample attributes if they don't exist
                    if (!dataSet.Attributes.ContainsKey(2))
                        dataSet.Attributes[2] = $"STA_{dsn:D3}"; // Station ID
                    
                    if (!dataSet.Attributes.ContainsKey(289))
                        dataSet.Attributes[289] = "FLOW"; // Constituent
                        
                    if (!dataSet.Attributes.ContainsKey(17))
                        dataSet.Attributes[17] = 1440; // Time step (daily)
                        
                    if (!dataSet.Attributes.ContainsKey(27))
                        dataSet.Attributes[27] = 4; // Time code (days)
                        
                    if (!dataSet.Attributes.ContainsKey(33))
                        dataSet.Attributes[33] = 1; // Units (CFS)
                        
                    if (!dataSet.Attributes.ContainsKey(288))
                        dataSet.Attributes[288] = "OBSERVED"; // Scenario

                    Console.WriteLine("? Created test labels for dataset");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating test labels: {ex.Message}");
            }
        }

        /// <summary>
        /// Test a single label type
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        /// <param name="labelType">Label type</param>
        /// <param name="labelName">Label name for display</param>
        private static void TestSingleLabelType(int wdmUnit, int dsn, int labelType, string labelName)
        {
            try
            {
                int[] labelBuffer = new int[100];
                
                WdmOperations.F90_WDLBAX(wdmUnit, dsn, labelType, 0, 100, labelBuffer,
                    out int actualLength, out int retCode);
                
                if (retCode == 0 && actualLength > 0)
                {
                    string labelText = DataConversionUtilities.IntArrayToString(labelBuffer, actualLength);
                    Console.WriteLine($"  ? {labelName} Label: '{labelText.Trim()}'");
                }
                else if (retCode == 1)
                {
                    Console.WriteLine($"  ?? {labelName} Label: No label found");
                }
                else
                {
                    Console.WriteLine($"  ? {labelName} Label: Failed (return code: {retCode})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ? {labelName} Label: Error - {ex.Message}");
            }
        }

        /// <summary>
        /// Test generic attributes functionality
        /// </summary>
        private static void TestGenericAttributesFunction()
        {
            Console.WriteLine("Test Generic Attributes (F90_GETATT)");
            Console.WriteLine("====================================");
            
            Console.Write("Enter dataset number for attribute testing: ");
            string input = Console.ReadLine() ?? "";
            
            if (int.TryParse(input, out int dsn))
            {
                try
                {
                    HassEntLibrary.Initialize();
                    
                    string wdmPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "Sample.wdm");
                    int wdmUnit = HassEntFunctions.F90_WDBOPN(1, wdmPath);
                    
                    if (wdmUnit > 0)
                    {
                        int datasetType = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
                        
                        if (datasetType > 0)
                        {
                            Console.WriteLine($"\nTesting generic attributes for dataset {dsn}:");
                            
                            // Create some test attributes first
                            CreateTestAttributesForDataset(wdmUnit, dsn);
                            
                            // Test common WDM attributes with different types
                            TestSingleGenericAttribute(wdmUnit, dsn, 2, 3, "Station ID (String)");
                            TestSingleGenericAttribute(wdmUnit, dsn, 17, 1, "Time Step (Integer)");
                            TestSingleGenericAttribute(wdmUnit, dsn, 27, 1, "Time Code (Integer)");
                            TestSingleGenericAttribute(wdmUnit, dsn, 33, 1, "Units (Integer)");
                            TestSingleGenericAttribute(wdmUnit, dsn, 288, 3, "Scenario (String)");
                            TestSingleGenericAttribute(wdmUnit, dsn, 289, 3, "Constituent (String)");
                            

                            // Test type conversions
                            Console.WriteLine("\nTesting type conversions:");
                            TestSingleGenericAttribute(wdmUnit, dsn, 17, 2, "Time Step as Real");
                            TestSingleGenericAttribute(wdmUnit, dsn, 17, 3, "Time Step as String");
                            
                            // Test non-existent attributes
                            Console.WriteLine("\nTesting non-existent attributes:");
                            TestSingleGenericAttribute(wdmUnit, dsn, 999, 1, "Non-existent Integer");
                            TestSingleGenericAttribute(wdmUnit, dsn, 998, 3, "Non-existent String");
                        }
                        else
                        {
                            Console.WriteLine($"? Dataset {dsn} does not exist");
                        }
                        
                        HassEntFunctions.F90_WDFLCL(wdmUnit);
                    }
                    
                    HassEntLibrary.Shutdown();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Generic attributes test failed: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid dataset number");
            }
        }

        /// <summary>
        /// Create test attributes for a dataset
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        private static void CreateTestAttributesForDataset(int wdmUnit, int dsn)
        {
            try
            {
                var wdmInfo = WdmOperations.GetWdmFileInfo(wdmUnit);
                if (wdmInfo != null && wdmInfo.DataSets.TryGetValue(dsn, out var dataSet))
                {
                    // Add comprehensive test attributes
                    dataSet.Attributes[2] = $"STATION_{dsn:D3}";    // String attribute
                    dataSet.Attributes[17] = 1440;                  // Integer attribute (daily)
                    dataSet.Attributes[27] = 4;                     // Integer attribute (day code)
                    dataSet.Attributes[33] = 1;                     // Integer attribute (CFS)
                    dataSet.Attributes[100] = 123.45f;              // Float attribute
                    dataSet.Attributes[101] = new int[] { 1, 2, 3, 4, 5 }; // Integer array
                    dataSet.Attributes[102] = new float[] { 1.1f, 2.2f, 3.3f }; // Float array
                    dataSet.Attributes[288] = "TEST_SCENARIO";      // String attribute
                    dataSet.Attributes[289] = "FLOW";               // String attribute

                    Console.WriteLine("? Created comprehensive test attributes for dataset");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating test attributes: {ex.Message}");
            }
        }

        /// <summary>
        /// Test a single generic attribute
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        /// <param name="attrIndex">Attribute index</param>
        /// <param name="attrType">Attribute type</param>
        /// <param name="attrName">Attribute name for display</param>
        private static void TestSingleGenericAttribute(int wdmUnit, int dsn, int attrIndex, int attrType, string attrName)
        {
            try
            {
                int[] intValues = new int[10];
                float[] realValues = new float[10];
                int[] charValues = new int[100];
                
                WdmOperations.F90_GETATT(wdmUnit, dsn, attrIndex, attrType, 100, intValues, realValues, charValues,
                    out int actualLength, out int retCode);
                
                if (retCode == 0)
                {
                    string result = FormatGenericAttributeResult(attrType, intValues, realValues, charValues, actualLength);
                    Console.WriteLine($"  ? {attrName}: {result} (length: {actualLength})");
                }
                else if (retCode == 1)
                {
                    string result = FormatGenericAttributeResult(attrType, intValues, realValues, charValues, actualLength);
                    Console.WriteLine($"  ?? {attrName}: Default - {result}");
                }
                else
                {
                    string errorMsg = retCode switch
                    {
                        -2 => "Dataset not found",
                        -3 => "Type conversion error",
                        -4 => "Attribute not found",
                        _ => $"Error (code: {retCode})"
                    };
                    Console.WriteLine($"  ? {attrName}: {errorMsg}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ? {attrName}: Error - {ex.Message}");
            }
        }

        /// <summary>
        /// Format generic attribute result for display
        /// </summary>
        /// <param name="attrType">Attribute type</param>
        /// <param name="intValues">Integer values</param>
        /// <param name="realValues">Real values</param>
        /// <param name="charValues">Character values</param>
        /// <param name="actualLength">Actual length</param>
        /// <returns>Formatted result string</returns>
        private static string FormatGenericAttributeResult(int attrType, int[] intValues, float[] realValues, int[] charValues, int actualLength)
        {
            return attrType switch
            {
                1 => actualLength == 1 ? intValues[0].ToString() : 
                     $"[{string.Join(", ", intValues.Take(actualLength))}]",
                2 => actualLength == 1 ? realValues[0].ToString("F2") : 
                     $"[{string.Join(", ", realValues.Take(actualLength).Select(f => f.ToString("F2")))}]",
                3 => $"'{DataConversionUtilities.IntArrayToString(charValues, actualLength).Trim()}'",
                _ => "Unknown type"
            };
        }

        /// <summary>
        /// Test add labels functionality
        /// </summary>
        private static void TestAddLabelsFunction()
        {
            Console.WriteLine("Test Add Labels (F90_WDLBAD)");
            Console.WriteLine("============================");
            
            Console.Write("Enter dataset number for label setting: ");
            string input = Console.ReadLine() ?? "";
            
            if (int.TryParse(input, out int dsn))
            {
                try
                {
                    HassEntLibrary.Initialize();
                    
                    string wdmPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "Sample.wdm");
                    int wdmUnit = HassEntFunctions.F90_WDBOPN(0, wdmPath); // Open for read-write
                    
                    if (wdmUnit > 0)
                    {
                        Console.WriteLine($"\nSetting labels for dataset {dsn}:");
                        Console.WriteLine("Note: This will create the dataset if it doesn't exist.");
                        
                        // Interactive label setting
                        SetInteractiveLabels(wdmUnit, dsn);
                        
                        // Verify labels were set by reading them back
                        Console.WriteLine("\nVerifying set labels:");
                        VerifyInteractiveLabels(wdmUnit, dsn);
                        
                        HassEntFunctions.F90_WDFLCL(wdmUnit);
                    }
                    else
                    {
                        Console.WriteLine("? Could not open WDM file for writing");
                    }
                    
                    HassEntLibrary.Shutdown();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Add labels test failed: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid dataset number");
            }
        }

        /// <summary>
        /// Set labels interactively
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        private static void SetInteractiveLabels(int wdmUnit, int dsn)
        {
            try
            {
                // Set a series of common labels
                var labelsToSet = new[]
                {
                    (1, "Station ID", "Enter station identifier (e.g., USGS_01234567):", "STA_" + dsn.ToString("D3")),
                    (2, "Parameter", "Enter parameter name (e.g., STREAMFLOW, TEMPERATURE):", "FLOW"),
                    (3, "Time Step", "Enter time step (e.g., Daily, Hourly, 15-Minute):", "Daily"),
                    (4, "Units", "Enter units (e.g., CFS, DEGREES F, MG/L):", "CFS"),
                    (5, "Scenario", "Enter scenario name (e.g., OBSERVED, SIMULATED):", "OBSERVED"),
                    (6, "Description", "Enter description:", $"Dataset {dsn} time series data")
                };

                foreach (var (labelType, labelName, prompt, defaultValue) in labelsToSet)
                {
                    Console.Write($"{prompt} [{defaultValue}]: ");
                    string userInput = Console.ReadLine()?.Trim() ?? "";
                    
                    string labelText = string.IsNullOrEmpty(userInput) ? defaultValue : userInput;
                    
                    if (SetSingleLabel(wdmUnit, dsn, labelType, labelText))
                    {
                        Console.WriteLine($"  ? Set {labelName}: '{labelText}'");
                    }
                    else
                    {
                        Console.WriteLine($"  ? Failed to set {labelName}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting interactive labels: {ex.Message}");
            }
        }

        /// <summary>
        /// Set a single label using F90_WDLBAD
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        /// <param name="labelType">Label type</param>
        /// <param name="labelText">Label text</param>
        /// <returns>True if successful</returns>
        private static bool SetSingleLabel(int wdmUnit, int dsn, int labelType, string labelText)
        {
            try
            {
                // Convert string to integer array (FORTRAN character format)
                int[] labelBuffer = new int[labelText.Length];
                for (int i = 0; i < labelText.Length; i++)
                {
                    labelBuffer[i] = (int)labelText[i];
                }
                
                WdmOperations.F90_WDLBAD(wdmUnit, dsn, labelType, 0, labelText.Length, labelBuffer, out int retCode);
                return retCode == 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verify interactive labels by reading them back
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        private static void VerifyInteractiveLabels(int wdmUnit, int dsn)
        {
            try
            {
                string[] labelNames = { "", "Station", "Parameter", "Time", "Units", "Scenario", "Description" };
                
                for (int labelType = 1; labelType <= 6; labelType++)
                {
                    int[] labelBuffer = new int[200];
                    
                    WdmOperations.F90_WDLBAX(wdmUnit, dsn, labelType, 0, 200, labelBuffer,
                        out int actualLength, out int retCode);
                    
                    if (retCode == 0 && actualLength > 0)
                    {
                        string readLabel = DataConversionUtilities.IntArrayToString(labelBuffer, actualLength);
                        Console.WriteLine($"  ? {labelNames[labelType]}: '{readLabel.Trim()}'");
                    }
                    else
                    {
                        Console.WriteLine($"  ?? {labelNames[labelType]}: Could not read back");
                    }
                }

                // Also show some key attributes that should have been set
                var dataset = WdmOperations.GetDataSet(wdmUnit, dsn);
                if (dataset != null)
                {
                    Console.WriteLine("\nDataset attributes set:");
                    if (dataset.Attributes.ContainsKey(2))
                        Console.WriteLine($"  Station ID (attr 2): {dataset.Attributes[2]}");
                    if (dataset.Attributes.ContainsKey(289))
                        Console.WriteLine($"  Constituent (attr 289): {dataset.Attributes[289]}");
                    if (dataset.Attributes.ContainsKey(33))
                        Console.WriteLine($"  Units code (attr 33): {dataset.Attributes[33]}");
                    if (dataset.Attributes.ContainsKey(17))
                        Console.WriteLine($"  Time step (attr 17): {dataset.Attributes[17]}");
                    if (!string.IsNullOrEmpty(dataset.TimeSeriesType))
                        Console.WriteLine($"  Time series type: {dataset.TimeSeriesType}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying labels: {ex.Message}");
            }
        }

        /// <summary>
        /// Test find time data functionality
        /// </summary>
        private static void TestFindTimeDataFunction()
        {
            Console.WriteLine("Test Find Time Data (F90_WTFNDT)");
            Console.WriteLine("===============================");
            
            Console.Write("Enter dataset number for time find test: ");
            string input = Console.ReadLine() ?? "";
            
            if (int.TryParse(input, out int dsn))
            {
                try
                {
                    HassEntLibrary.Initialize();
                    
                    string wdmPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "Sample.wdm");
                    int wdmUnit = HassEntFunctions.F90_WDBOPN(1, wdmPath);
                    
                    if (wdmUnit > 0)
                    {
                        int datasetType = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
                        
                        if (datasetType > 0)
                        {
                            Console.WriteLine($"\nTesting time find operations for dataset {dsn}:");
                            
                            // Create some test data first
                            CreateTestTimeDataForDataset(wdmUnit, dsn);
                            
                            // Test target date
                            int[] targetDate = { 2023, 6, 15, 12, 0, 0 }; // Mid-month, noon
                            
                            // Test different search modes
                            TestTimeFindOperation(wdmUnit, dsn, 1, targetDate, "First data point");
                            TestTimeFindOperation(wdmUnit, dsn, 2, targetDate, "Last data point");
                            TestTimeFindOperation(wdmUnit, dsn, 3, targetDate, "Nearest to target");
                            TestTimeFindOperation(wdmUnit, dsn, 4, targetDate, "Next after target");
                            TestTimeFindOperation(wdmUnit, dsn, 5, targetDate, "Previous before target");
                            TestTimeFindOperation(wdmUnit, dsn, 6, targetDate, "Exact match");
                            TestTimeFindOperation(wdmUnit, dsn, 7, targetDate, "Interpolated value");
                        }
                        else
                        {
                            Console.WriteLine($"? Dataset {dsn} does not exist");
                        }
                        
                        HassEntFunctions.F90_WDFLCL(wdmUnit);
                    }
                    
                    HassEntLibrary.Shutdown();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Time find test failed: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid dataset number");
            }
        }

        /// <summary>
        /// Create test time data for a dataset
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        private static void CreateTestTimeDataForDataset(int wdmUnit, int dsn)
        {
            try
            {
                var wdmInfo = WdmOperations.GetWdmFileInfo(wdmUnit);
                if (wdmInfo != null)
                {
                    // Create or update test dataset with time series data
                    if (!wdmInfo.DataSets.ContainsKey(dsn))
                    {
                        wdmInfo.DataSets[dsn] = new WdmOperations.WdmDataSet
                        {
                            Dsn = dsn,
                            DataSetType = 1,
                            TimeSeriesType = "FLOW",
                            TimeUnit = 4,
                            TimeStep = 1
                        };
                    }

                    var testDataSet = wdmInfo.DataSets[dsn];
                    testDataSet.Data.Clear(); // Clear existing data

                    // Add test time series data (30 days of daily data)
                    var baseDate = new DateTime(2023, 6, 1);
                    for (int i = 0; i < 30; i++)
                    {
                        var currentDate = baseDate.AddDays(i);
                        float value = 100.0f + (float)(Math.Sin(i * 0.3) * 25.0f); // Sinusoidal pattern
                        
                        testDataSet.Data.Add(new WdmOperations.TimeSeriesValue
                        {
                            DateTime = currentDate,
                            Value = value,
                            Quality = 0
                        });
                    }

                    Console.WriteLine($"? Created {testDataSet.Data.Count} daily time series values for testing");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating test time data: {ex.Message}");
            }
        }

        /// <summary>
        /// Test a specific time find operation
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        /// <param name="searchMode">Search mode</param>
        /// <param name="targetDate">Target date</param>
        /// <param name="operationName">Operation name for display</param>
        private static void TestTimeFindOperation(int wdmUnit, int dsn, int searchMode, int[] targetDate, string operationName)
        {
            try
            {
                int[] foundDate = new int[6];
                
                WdmOperations.F90_WTFNDT(wdmUnit, dsn, searchMode, targetDate, null, null,
                    foundDate, out float foundValue, out int foundQuality, out int dataIndex, out int retCode);
                
                if (retCode == 0)
                {
                    var foundDateTime = new DateTime(foundDate[0], foundDate[1], foundDate[2], 
                                                   foundDate[3], foundDate[4], foundDate[5]);
                    Console.WriteLine($"  ? {operationName}:");
                    Console.WriteLine($"     Date: {foundDateTime:yyyy-MM-dd HH:mm:ss}");
                    Console.WriteLine($"     Value: {foundValue:F2}");
                    Console.WriteLine($"     Quality: {foundQuality}");
                    Console.WriteLine($"     Index: {dataIndex}");
                }
                else if (retCode == -3)
                {
                    Console.WriteLine($"  ?? {operationName}: No data found matching criteria");
                }
                else
                {
                    Console.WriteLine($"  ? {operationName}: Failed (return code: {retCode})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ? {operationName}: Error - {ex.Message}");
            }
        }

        /// <summary>
        /// Run detailed dataset scanning and analysis
        /// </summary>
        private static void RunDetailedDatasetScan()
        {
            Console.WriteLine("Detailed WDM Dataset Scan");
            Console.WriteLine("=========================");
            
            try
            {
                string dataFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                string sampleWdm = System.IO.Path.Combine(dataFolder, "Sample.wdm");
                
                if (System.IO.File.Exists(sampleWdm))
                {
                    HassEntLibrary.Initialize();
                    
                    int wdmUnit = HassEntFunctions.F90_WDBOPN(1, sampleWdm);
                    if (wdmUnit > 0)
                    {
                        Console.WriteLine("Scanning datasets and attributes...");
                        int totalDatasets = 0;
                        
                        for (int dsn = 1; dsn <= 1000; dsn++)
                        {
                            int datasetType = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
                            if (datasetType > 0)
                            {
                                totalDatasets++;
                                
                                // Display dataset header
                                Console.WriteLine($"\nDataset {dsn} (Type {datasetType}):");
                                
                                // Display attributes
                                Console.WriteLine("  Attributes:");
                                
                                // Station ID
                                int[] stationId = new int[16];
                                WdmOperations.F90_WDBSGC_XX(wdmUnit, dsn, 2, 16, stationId);
                                string station = DataConversionUtilities.IntArrayToString(stationId).Trim();
                                Console.WriteLine($"    Station ID: {station}");
                        
                                // Date range - simplified without unavailable functions
                                int[] startDate = { 1990, 1, 1, 0, 0, 0 };
                                Console.WriteLine($"    Estimated Start Date: {startDate[0]:D4}-{startDate[1]:D2}-{startDate[2]:D2}");
                                
                                // Try to get some data
                                Console.WriteLine("  Data Sample (first 5 values):");
                                float[] values = new float[5];
                                WdmOperations.F90_WDTGET(wdmUnit, dsn, 1440, startDate, 5, 0, 0, 4, values, out int getResult);
                                for (int i = 0; i < values.Length; i++)
                                {
                                    if (!float.IsNaN(values[i]))
                                        Console.WriteLine($"    Value {i + 1}: {values[i]:F2}");
                                }
                            }
                        }
                        
                        Console.WriteLine($"\nTotal datasets scanned: {totalDatasets}");
                        
                        HassEntFunctions.F90_WDFLCL(wdmUnit);
                    }
                    
                    HassEntLibrary.Shutdown();
                }
                else
                {
                    Console.WriteLine("Sample.wdm file not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during detailed scan: {ex.Message}");
            }
        }

        /// <summary>
        /// Scan WDM file and provide detailed dataset information
        /// </summary>
        private static void ScanWdmDatasets()
        {
            Console.WriteLine("WDM Dataset Scanner");
            Console.WriteLine("==================");
            
            try
            {
                string dataFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                string sampleWdm = System.IO.Path.Combine(dataFolder, "Sample.wdm");
                
                if (!System.IO.File.Exists(sampleWdm))
                {
                    Console.WriteLine("? Sample.wdm not found in data folder.");
                    return;
                }

                HassEntLibrary.Initialize();
                
                int wdmUnit = HassEntFunctions.F90_WDBOPN(1, sampleWdm);
                if (wdmUnit <= 0)
                {
                    Console.WriteLine("? Could not open Sample.wdm file");
                    return;
                }

                Console.WriteLine($"?? Scanning: {sampleWdm}");
                Console.WriteLine("?? Searching for datasets...\n");

                var datasets = new List<DatasetInfo>();
                int totalDatasets = 0;

                // Scan through possible dataset numbers (WDM files typically use 1-999 or higher)
                for (int dsn = 1; dsn <= 1500; dsn++)
                {
                    int datasetType = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
                    if (datasetType > 0)
                    {
                        totalDatasets++;
                        Console.Write($"\r?? Found {totalDatasets} datasets, scanning DSN {dsn}...");
                        
                        var datasetInfo = ExtractDatasetInfo(wdmUnit, dsn, datasetType);
                        if (datasetInfo != null)
                        {
                            datasets.Add(datasetInfo);
                        }
                    }
                }

                Console.WriteLine($"\r? Scan complete! Found {totalDatasets} datasets.                    \n");

                // Display results
                if (datasets.Count > 0)
                {
                    DisplayDatasetSummary(datasets);
                    
                    Console.WriteLine("\nOptions:");
                    Console.WriteLine("1. Show detailed dataset information");
                    Console.WriteLine("2. Export dataset list to CSV");
                    Console.WriteLine("3. Filter datasets by criteria");
                    Console.Write("Enter choice (1-3, or Enter to continue): ");
                    
                    string choice = Console.ReadLine()?.Trim() ?? "";
                    
                    switch (choice)
                    {
                        case "1":
                            ShowDetailedDatasetInfo(datasets);
                            break;
                        case "2":
                            ExportDatasetListToCsv(datasets);
                            break;
                        case "3":
                            FilterDatasets(datasets);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("? No datasets found in the WDM file.");
                }

                HassEntFunctions.F90_WDFLCL(wdmUnit);
                HassEntLibrary.Shutdown();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Dataset scanning failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Dataset information structure
        /// </summary>
        private class DatasetInfo
        {
            public int Dsn { get; set; }
            public int DatasetType { get; set; }
            public string StationId { get; set; } = "";
            public string Variable { get; set; } = "";
            public string Scenario { get; set; } = "";
            public string Location { get; set; } = "";
            public int TimeStep { get; set; }
            public string TimeUnit { get; set; } = "";
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public int DataPoints { get; set; }
            public float? MinValue { get; set; }
            public float? MaxValue { get; set; }
            public float? MeanValue { get; set; }
            public string Units { get; set; } = "";
            public string Description { get; set; } = "";
        }

        /// <summary>
        /// Extract detailed information from a dataset
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        /// <param name="datasetType">Dataset type</param>
        /// <returns>Dataset information</returns>
        private static DatasetInfo? ExtractDatasetInfo(int wdmUnit, int dsn, int datasetType)
        {
            try
            {
                var info = new DatasetInfo
                {
                    Dsn = dsn,
                    DatasetType = datasetType
                };

                // Extract station ID (attribute 2)
                int[] stationIdArray = new int[20];
                WdmOperations.F90_WDBSGC_XX(wdmUnit, dsn, 2, 20, stationIdArray);
                info.StationId = DataConversionUtilities.IntArrayToString(stationIdArray).Trim();
                if (string.IsNullOrEmpty(info.StationId))
                    info.StationId = $"DSN_{dsn:D3}";

                // Extract variable/constituent (attribute 289)
                int[] constituentArray = new int[20];
                WdmOperations.F90_WDBSGC_XX(wdmUnit, dsn, 289, 20, constituentArray);
                info.Variable = DataConversionUtilities.IntArrayToString(constituentArray).Trim();
                
                // Try time series type if no constituent
                if (string.IsNullOrEmpty(info.Variable))
                {
                    int[] tsTypeArray = new int[10];
                    WdmOperations.F90_WDBSGC_XX(wdmUnit, dsn, 1, 10, tsTypeArray);
                    string tsType = DataConversionUtilities.IntArrayToString(tsTypeArray).Trim();
                    info.Variable = ConvertTimeSeriesTypeToVariable(tsType);
                }
                if (string.IsNullOrEmpty(info.Variable))
                    info.Variable = "DATA";

                // Extract scenario (attribute 288)
                int[] scenarioArray = new int[20];
                WdmOperations.F90_WDBSGC_XX(wdmUnit, dsn, 288, 20, scenarioArray);
                info.Scenario = DataConversionUtilities.IntArrayToString(scenarioArray).Trim();

                // Extract location (attribute 290)
                int[] locationArray = new int[20];
                WdmOperations.F90_WDBSGC_XX(wdmUnit, dsn, 290, 20, locationArray);
                info.Location = DataConversionUtilities.IntArrayToString(locationArray).Trim();

                // Extract time step (attribute 17)
                int[] timeStepArray = new int[1];
                WdmOperations.F90_WDBSGI(wdmUnit, dsn, 17, 1, timeStepArray, out int retCode);
                if (retCode == 0)
                    info.TimeStep = timeStepArray[0];

                // Extract time unit code (attribute 27) and convert to description
                int[] timeUnitArray = new int[1];
                WdmOperations.F90_WDBSGI(wdmUnit, dsn, 27, 1, timeUnitArray, out retCode);
                if (retCode == 0)
                    info.TimeUnit = ConvertTimeUnitCodeToDescription(timeUnitArray[0]);

                // Extract units (attribute 33)
                int[] unitsArray = new int[1];
                WdmOperations.F90_WDBSGI(wdmUnit, dsn, 33, 1, unitsArray, out retCode);
                if (retCode == 0)
                    info.Units = ConvertUnitsCodeToDescription(unitsArray[0]);

                // Extract description (attribute 45)
                int[] descArray = new int[50];
                WdmOperations.F90_WDBSGC_XX(wdmUnit, dsn, 45, 50, descArray);
                info.Description = DataConversionUtilities.IntArrayToString(descArray).Trim();

                // Get time series data range and statistics
                ExtractTimeSeriesInfo(wdmUnit, dsn, info);

                return info;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n?? Error extracting info for DSN {dsn}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Extract time series information and statistics
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        /// <param name="info">Dataset info to update</param>
        private static void ExtractTimeSeriesInfo(int wdmUnit, int dsn, DatasetInfo info)
        {
            try
            {
                // Try to get data from a wide date range to find actual data bounds
                int[] startDate = { 1900, 1, 1, 0, 0, 0 };
                
                // Start with a small sample to check if data exists
                float[] sampleValues = new float[100];
                WdmOperations.F90_WDTGET(wdmUnit, dsn, info.TimeStep > 0 ? info.TimeStep : 1440, 
                    startDate, 100, 0, 0, 4, sampleValues, out int getResult);

                if (getResult == 0)
                {
                    // Get larger sample for better statistics
                    float[] values = new float[1000];
                    WdmOperations.F90_WDTGET(wdmUnit, dsn, info.TimeStep > 0 ? info.TimeStep : 1440, 
                        startDate, 1000, 0, 0, 4, values, out getResult);

                    if (getResult == 0)
                    {
                        var validValues = new List<float>();
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (!float.IsNaN(values[i]) && values[i] != -999.0f && values[i] != -9999.0f)
                            {
                                validValues.Add(values[i]);
                            }
                        }

                        if (validValues.Count > 0)
                        {
                            info.DataPoints = validValues.Count;
                            info.MinValue = validValues.Min();
                            info.MaxValue = validValues.Max();
                            info.MeanValue = validValues.Average();

                            // Simple date estimation
                            info.StartDate = DateTimeUtilities.ArrayToDateTime(startDate);
                            
                            // Calculate estimated end date based on time step and data count
                            if (info.TimeStep > 0)
                            {
                                var timeSpan = info.TimeUnit.ToLower() switch
                                {
                                    var unit when unit.Contains("minute") => TimeSpan.FromMinutes(info.TimeStep * validValues.Count),
                                    var unit when unit.Contains("hour") => TimeSpan.FromHours(info.TimeStep * validValues.Count),
                                    var unit when unit.Contains("day") => TimeSpan.FromDays(info.TimeStep * validValues.Count),
                                    var unit when unit.Contains("month") => TimeSpan.FromDays(info.TimeStep * validValues.Count * 30),
                                    var unit when unit.Contains("year") => TimeSpan.FromDays(info.TimeStep * validValues.Count * 365),
                                    _ => TimeSpan.FromDays(validValues.Count)
                                };

                                info.EndDate = info.StartDate?.Add(timeSpan);
                            }
                        }
                    }
                }
                
                // If no data found, mark as empty dataset
                if (info.DataPoints == 0)
                {
                    info.DataPoints = 0;
                    info.StartDate = null;
                    info.EndDate = null;
                }
            }
            catch (Exception ex)
            {
                // Don't fail the entire scan for one dataset
                Console.WriteLine($"\n?? Could not extract time series info for DSN {dsn}: {ex.Message}");
                info.DataPoints = 0;
            }
        }

        /// <summary>
        /// Convert time series type to variable name
        /// </summary>
        /// <param name="tsType">Time series type</param>
        /// <returns>Variable name</returns>
        private static string ConvertTimeSeriesTypeToVariable(string tsType)
        {
            if (string.IsNullOrEmpty(tsType)) return "DATA";
            
            return tsType.ToUpper().Trim() switch
            {
                "1" or "PREC" => "PREC",
                "2" or "PEVT" => "PEVT",
                "3" or "FLOW" => "FLOW", 
                "4" or "TEMP" => "TEMP",
                "5" or "SRAD" => "SRAD",
                "6" or "DEWP" => "DEWP",
                "7" or "WIND" => "WIND",
                "8" or "CLOUD" => "CLOUD",
                _ => tsType.ToUpper()
            };
        }

        /// <summary>
        /// Convert time unit code to description
        /// </summary>
        /// <param name="timeUnitCode">Time unit code</param>
        /// <returns>Time unit description</returns>
        private static string ConvertTimeUnitCodeToDescription(int timeUnitCode)
        {
            return timeUnitCode switch
            {
                1 => "Seconds",
                2 => "Minutes", 
                3 => "Hours",
                4 => "Days",
                5 => "Months",
                6 => "Years",
                _ => $"Code_{timeUnitCode}"
            };
        }

        /// <summary>
        /// Convert units code to description
        /// </summary>
        /// <param name="unitsCode">Units code</param>
        /// <returns>Units description</returns>
        private static string ConvertUnitsCodeToDescription(int unitsCode)
        {
            return unitsCode switch
            {
                1 => "CFS",
                2 => "Inches",
                3 => "Degrees F",
                4 => "MGD",
                5 => "MM",
                6 => "Degrees C",
                7 => "M3/S",
                8 => "GPM",
                9 => "AFD",
                10 => "PPM",
                11 => "MG/L",
                _ => $"Units_{unitsCode}"
            };
        }

        /// <summary>
        /// Display summary of all datasets
        /// </summary>
        /// <param name="datasets">List of dataset information</param>
        private static void DisplayDatasetSummary(List<DatasetInfo> datasets)
        {
            Console.WriteLine("?? Dataset Summary");
            Console.WriteLine("==================");
            Console.WriteLine();

            // Group by variable for summary
            var variableGroups = datasets.GroupBy(d => d.Variable).OrderBy(g => g.Key);
            
            Console.WriteLine("Variables found:");
            foreach (var group in variableGroups)
            {
                var datasetsInGroup = group.ToList();
                var stationCount = datasetsInGroup.Select(d => d.StationId).Distinct().Count();
                var totalDataPoints = datasetsInGroup.Sum(d => d.DataPoints);
                
                Console.WriteLine($"  ?? {group.Key}: {group.Count()} datasets, {stationCount} stations, {totalDataPoints:N0} total data points");
            }

            Console.WriteLine();
            Console.WriteLine("?? Dataset List:");
            Console.WriteLine("{0,-5} {1,-12} {2,-8} {3,-10} {4,-12} {5,-10} {6,-12} {7,-10}",
                "DSN", "Station_ID", "Variable", "Scenario", "Time_Step", "Data_Pts", "Date_Range", "Min-Max");
            Console.WriteLine(new string('=', 95));

            foreach (var dataset in datasets.OrderBy(d => d.Dsn))
            {
                string dateRange = "No data";
                if (dataset.StartDate.HasValue && dataset.EndDate.HasValue)
                {
                    dateRange = $"{dataset.StartDate:yyyy-MM-dd} to {dataset.EndDate:yyyy-MM-dd}";
                }

                string valueRange = "N/A";
                if (dataset.MinValue.HasValue && dataset.MaxValue.HasValue)
                {
                    valueRange = $"{dataset.MinValue:F3} to {dataset.MaxValue:F3}";
                }

                string timeStepStr = dataset.TimeStep > 0 ? 
                    $"{dataset.TimeStep}{(dataset.TimeUnit.Length > 0 ? dataset.TimeUnit[0] : 'D')}" : "N/A";

                Console.WriteLine("{0,-5} {1,-12} {2,-8} {3,-10} {4,-12} {5,-10} {6,-12} {7,-10}",
                    dataset.Dsn,
                    TruncateString(dataset.StationId, 12),
                    TruncateString(dataset.Variable, 8),
                    TruncateString(dataset.Scenario, 10),
                    timeStepStr,
                    dataset.DataPoints.ToString("N0"),
                    TruncateString(dateRange, 12),
                    TruncateString(valueRange, 10));
            }
        }

        /// <summary>
        /// Truncate string to specified length
        /// </summary>
        /// <param name="str">String to truncate</param>
        /// <param name="maxLength">Maximum length</param>
        /// <returns>Truncated string</returns>
        private static string TruncateString(string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str)) return "";
            return str.Length <= maxLength ? str : str.Substring(0, maxLength - 2) + "..";
        }

        /// <summary>
        /// Show detailed information for all datasets
        /// </summary>
        /// <param name="datasets">List of dataset information</param>
        private static void ShowDetailedDatasetInfo(List<DatasetInfo> datasets)
        {
            Console.WriteLine("\n?? Detailed Dataset Information");
            Console.WriteLine("===============================");

            foreach (var dataset in datasets.OrderBy(d => d.Dsn))
            {
                Console.WriteLine($"\n?? Dataset {dataset.Dsn} (Type: {dataset.DatasetType})");
                Console.WriteLine($"   ?? Station ID: {dataset.StationId}");
                Console.WriteLine($"   ?? Variable: {dataset.Variable}");
                if (!string.IsNullOrEmpty(dataset.Scenario))
                    Console.WriteLine($"   ?? Scenario: {dataset.Scenario}");
                if (!string.IsNullOrEmpty(dataset.Location))
                    Console.WriteLine($"   ?? Location: {dataset.Location}");
                if (!string.IsNullOrEmpty(dataset.Description))
                    Console.WriteLine($"   ?? Description: {dataset.Description}");
                
                Console.WriteLine($"   ? Time Step: {dataset.TimeStep} {dataset.TimeUnit}");
                Console.WriteLine($"   ?? Units: {dataset.Units}");
                Console.WriteLine($"   ?? Data Points: {dataset.DataPoints:N0}");
                
                if (dataset.StartDate.HasValue && dataset.EndDate.HasValue)
                {
                    Console.WriteLine($"   ?? Period: {dataset.StartDate:yyyy-MM-dd} to {dataset.EndDate:yyyy-MM-dd}");
                    var duration = dataset.EndDate - dataset.StartDate;
                    Console.WriteLine($"   ? Duration: {duration?.Days:N0} days");
                }
                
                if (dataset.MinValue.HasValue && dataset.MaxValue.HasValue && dataset.MeanValue.HasValue)
                {
                    Console.WriteLine($"   ?? Value Range: {dataset.MinValue:F3} to {dataset.MaxValue:F3} (Mean: {dataset.MeanValue:F3})");
                }
            }
        }

        /// <summary>
        /// Export dataset list to CSV file
        /// </summary>
        /// <param name="datasets">List of dataset information</param>
        private static void ExportDatasetListToCsv(List<DatasetInfo> datasets)
        {
            try
            {
                string fileName = $"wdm_dataset_list_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                
                using (var writer = new System.IO.StreamWriter(fileName))
                {
                    // Write header
                    writer.WriteLine("DSN,Station_ID,Variable,Scenario,Location,Time_Step,Time_Unit,Units,Data_Points,Start_Date,End_Date,Min_Value,Max_Value,Mean_Value,Description");
                    
                    // Write data
                    foreach (var dataset in datasets.OrderBy(d => d.Dsn))
                    {
                        writer.WriteLine($"{dataset.Dsn}," +
                                       $"\"{dataset.StationId}\"," +
                                       $"\"{dataset.Variable}\"," +
                                       $"\"{dataset.Scenario}\"," +
                                       $"\"{dataset.Location}\"," +
                                       $"{dataset.TimeStep}," +
                                       $"\"{dataset.TimeUnit}\"," +
                                       $"\"{dataset.Units}\"," +
                                       $"{dataset.DataPoints}," +
                                       $"{dataset.StartDate:yyyy-MM-dd HH:mm:ss}," +
                                       $"{dataset.EndDate:yyyy-MM-dd HH:mm:ss}," +
                                       $"{dataset.MinValue:F6}," +
                                       $"{dataset.MaxValue:F6}," +
                                       $"{dataset.MeanValue:F6}," +
                                       $"\"{dataset.Description}\"");
                    }
                }
                
                Console.WriteLine($"? Dataset list exported to: {fileName}");
                Console.WriteLine($"?? File contains {datasets.Count} datasets");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Export failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Filter datasets by various criteria
        /// </summary>
        /// <param name="datasets">List of dataset information</param>
        private static void FilterDatasets(List<DatasetInfo> datasets)
        {
            Console.WriteLine("\n?? Filter Datasets");
            Console.WriteLine("==================");
            Console.WriteLine("1. Filter by Variable");
            Console.WriteLine("2. Filter by Station ID");
            Console.WriteLine("3. Filter by Date Range");
            Console.WriteLine("4. Filter by Data Availability");
            Console.Write("Enter choice (1-4): ");
            
            string choice = Console.ReadLine()?.Trim() ?? "";
            List<DatasetInfo> filteredDatasets = datasets;
            
            switch (choice)
            {
                case "1":
                    filteredDatasets = FilterByVariable(datasets);
                    break;
                case "2":
                    filteredDatasets = FilterByStationId(datasets);
                    break;
                case "3":
                    filteredDatasets = FilterByDateRange(datasets);
                    break;
                case "4":
                    filteredDatasets = FilterByDataAvailability(datasets);
                    break;
                default:
                    Console.WriteLine("Invalid choice");
                    return;
            }
            
            if (filteredDatasets.Count > 0)
            {
                Console.WriteLine($"\n?? Filtered Results: {filteredDatasets.Count} datasets");
                DisplayDatasetSummary(filteredDatasets);
            }
            else
            {
                Console.WriteLine("? No datasets match the filter criteria");
            }
        }

        /// <summary>
        /// Filter datasets by variable
        /// </summary>
        /// <param name="datasets">All datasets</param>
        /// <returns>Filtered datasets</returns>
        private static List<DatasetInfo> FilterByVariable(List<DatasetInfo> datasets)
        {
            var variables = datasets.Select(d => d.Variable).Distinct().OrderBy(v => v).ToList();
            
            Console.WriteLine("\nAvailable variables:");
            for (int i = 0; i < variables.Count; i++)
            {
                var count = datasets.Count(d => d.Variable == variables[i]);
                Console.WriteLine($"{i + 1}. {variables[i]} ({count} datasets)");
            }
            
            Console.Write("Enter variable number: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= variables.Count)
            {
                string selectedVariable = variables[choice - 1];
                return datasets.Where(d => d.Variable == selectedVariable).ToList();
            }
            
            return new List<DatasetInfo>();
        }

        /// <summary>
        /// Filter datasets by station ID
        /// </summary>
        /// <param name="datasets">All datasets</param>
        /// <returns>Filtered datasets</returns>
        private static List<DatasetInfo> FilterByStationId(List<DatasetInfo> datasets)
        {
            Console.Write("Enter station ID (partial match): ");
            string stationFilter = Console.ReadLine()?.Trim().ToUpper() ?? "";
            
            if (!string.IsNullOrEmpty(stationFilter))
            {
                return datasets.Where(d => d.StationId.ToUpper().Contains(stationFilter)).ToList();
            }
            
            return new List<DatasetInfo>();
        }

        /// <summary>
        /// Filter datasets by date range
        /// </summary>
        /// <param name="datasets">All datasets</param>
        /// <returns>Filtered datasets</returns>
        private static List<DatasetInfo> FilterByDateRange(List<DatasetInfo> datasets)
        {
            Console.Write("Enter start year (YYYY): ");
            if (int.TryParse(Console.ReadLine(), out int startYear))
            {
                Console.Write("Enter end year (YYYY): ");
                if (int.TryParse(Console.ReadLine(), out int endYear))
                {
                    var filterStart = new DateTime(startYear, 1, 1);
                    var filterEnd = new DateTime(endYear, 12, 31);
                    
                    return datasets.Where(d => 
                        d.StartDate.HasValue && d.EndDate.HasValue &&
                        d.StartDate <= filterEnd && d.EndDate >= filterStart).ToList();
                }
            }
            
            return new List<DatasetInfo>();
        }

        /// <summary>
        /// Filter datasets by data availability
        /// </summary>
        /// <param name="datasets">All datasets</param>
        /// <returns>Filtered datasets</returns>
        private static List<DatasetInfo> FilterByDataAvailability(List<DatasetInfo> datasets)
        {
            Console.WriteLine("1. Datasets with data (>0 data points)");
            Console.WriteLine("2. Datasets without data (0 data points)");
            Console.WriteLine("3. Datasets with substantial data (>100 points)");
            Console.Write("Enter choice (1-3): ");
            
            string choice = Console.ReadLine()?.Trim() ?? "";
            
            return choice switch
            {
                "1" => datasets.Where(d => d.DataPoints > 0).ToList(),
                "2" => datasets.Where(d => d.DataPoints == 0).ToList(),
                "3" => datasets.Where(d => d.DataPoints > 100).ToList(),
                _ => new List<DatasetInfo>()
            };
        }

        /// <summary>
        /// Run specific data extraction based on command line arguments or user criteria
        /// </summary>
        /// <param name="args">Command line arguments</param>
        private static void RunSpecificDataExtraction(string[] args)
        {
            try
            {
                Console.WriteLine("?? Specific Data Extraction");
                Console.WriteLine("===========================");
                
                // Your specific criteria (can be modified or made interactive)
                string scenario = "Observed";
                string location = "72030763";
                string constituent = "WIND";
                int? datasetId = 20;
                DateTime startDate = new DateTime(2015, 1, 1);
                DateTime endDate = new DateTime(2019, 12, 31);

                // Allow override from command line
                if (args.Length > 1)
                {
                    // Format: dotnet run extract [wdmfile] [scenario] [location] [constituent] [id] [startdate] [enddate]
                    string wdmPath = args[1];
                    
                    if (args.Length > 2) scenario = args[2];
                    if (args.Length > 3) location = args[3];
                    if (args.Length > 4) constituent = args[4];
                    if (args.Length > 5 && int.TryParse(args[5], out int id)) datasetId = id;
                    if (args.Length > 6 && DateTime.TryParse(args[6], out DateTime start)) startDate = start;
                    if (args.Length > 7 && DateTime.TryParse(args[7], out DateTime end)) endDate = end;

                    // Extract the data
                    var extractedData = SpecificDataExtractor.ExtractSpecificData(
                        wdmPath, scenario, location, constituent, datasetId, startDate, endDate);

                    if (extractedData.Count > 0)
                    {
                        ExportExtractedData(extractedData, location, constituent, scenario);
                    }
                    else
                    {
                        Console.WriteLine("? No data found matching the specified criteria.");
                    }
                }
                else
                {
                    // Interactive mode
                    RunInteractiveExtraction();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Extraction failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Run interactive data extraction with user input
        /// </summary>
        private static void RunInteractiveExtraction()
        {
            Console.WriteLine("Interactive Data Extraction");
            Console.WriteLine("==========================");
            
            try
            {
                // Get criteria from user
                Console.Write("Enter scenario (default: Observed): ");
                string scenario = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(scenario)) scenario = "Observed";

                Console.Write("Enter location/station ID (default: 72030763): ");
                string location = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(location)) location = "72030763";

                Console.Write("Enter constituent/variable (default: WIND): ");
                string constituent = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(constituent)) constituent = "WIND";

                Console.Write("Enter specific dataset ID (or press Enter for any): ");
                string idInput = Console.ReadLine()?.Trim();
                int? datasetId = null;
                if (!string.IsNullOrEmpty(idInput) && int.TryParse(idInput, out int id))
                    datasetId = id;

                Console.Write("Enter start date (YYYY-MM-DD, default: 2015-01-01): ");
                string startInput = Console.ReadLine()?.Trim();
                DateTime startDate = DateTime.TryParse(startInput, out DateTime start) ? start : new DateTime(2015, 1, 1);

                Console.Write("Enter end date (YYYY-MM-DD, default: 2019-12-31): ");
                string endInput = Console.ReadLine()?.Trim();
                DateTime endDate = DateTime.TryParse(endInput, out DateTime end) ? end : new DateTime(2019, 12, 31);

                // Use default WDM file
                string wdmPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "Sample.wdm");
                Console.Write($"Enter WDM file path (default: {wdmPath}): ");
                string pathInput = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(pathInput))
                    wdmPath = pathInput;

                // Extract the data
                var extractedData = SpecificDataExtractor.ExtractSpecificData(
                    wdmPath, scenario, location, constituent, datasetId, startDate, endDate);

                if (extractedData.Count > 0)
                {
                    ExportExtractedData(extractedData, location, constituent, scenario);
                }
                else
                {
                    Console.WriteLine("? No data found matching the specified criteria.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Interactive extraction failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Export extracted data to both basic and enhanced CSV formats
        /// </summary>
        /// <param name="extractedData">Extracted time series data</param>
        /// <param name="location">Location identifier</param>
        /// <param name="constituent">Constituent/variable name</param>
        /// <param name="scenario">Scenario name</param>
        private static void ExportExtractedData(
            List<TimeSeriesDataPoint> extractedData, 
            string location, 
            string constituent, 
            string scenario)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string basicCsvFile = $"extracted_{constituent}_{location}_{timestamp}.csv";
                string enhancedCsvFile = $"extracted_{constituent}_{location}_enhanced_{timestamp}.csv";
                
                string criteria = $"Scenario={scenario}, Location={location}, Constituent={constituent}";

                // Export in both formats
                SpecificDataExtractor.ExportToCSV(extractedData, basicCsvFile, criteria);
                SpecificDataExtractor.ExportToEnhancedCSV(extractedData, enhancedCsvFile, location, constituent, criteria);

                // Show summary
                Console.WriteLine($"\n?? Extraction Summary:");
                Console.WriteLine($"   ?? Records: {extractedData.Count}");
                
                if (extractedData.Count > 0)
                {
                    var minValue = extractedData.Min(d => d.Value);
                    var maxValue = extractedData.Max(d => d.Value);
                    var avgValue = extractedData.Average(d => d.Value);
                    var firstDate = extractedData.Min(d => d.DateTime);
                    var lastDate = extractedData.Max(d => d.DateTime);
                    
                    Console.WriteLine($"   ?? Period: {firstDate:yyyy-MM-dd} to {lastDate:yyyy-MM-dd}");
                    Console.WriteLine($"   ?? Values: {minValue:F3} to {maxValue:F3} (avg: {avgValue:F3})");
                }

                // Show preview of enhanced format
                Console.WriteLine($"\n?? Preview of {enhancedCsvFile}:");
                var lines = System.IO.File.ReadAllLines(enhancedCsvFile);
                for (int i = 0; i < Math.Min(5, lines.Length); i++)
                {
                    Console.WriteLine($"   {lines[i]}");
                }
                if (lines.Length > 5)
                {
                    Console.WriteLine($"   ... ({lines.Length - 5} more lines)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Export failed: {ex.Message}");
            }
        }
    }
}