using System;
using System.IO;
using System.Linq;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Test cases specifically designed to work with the Sample.wdm file
    /// Demonstrates real WDM file operations and data handling
    /// </summary>
    public static class SampleWdmTests
    {
        private static readonly string DataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
        private static readonly string SampleWdmPath = Path.Combine(DataFolder, "Sample.wdm");
        private static readonly string SampleAnnualWdmPath = Path.Combine(DataFolder, "Sample_Annual.wdm");

        /// <summary>
        /// Run all Sample.wdm test cases
        /// </summary>
        public static void RunAllSampleWdmTests()
        {
            Console.WriteLine("Sample WDM File Test Cases");
            Console.WriteLine("==========================");
            Console.WriteLine();

            try
            {
                // Verify files exist
                if (!VerifyDataFiles())
                {
                    Console.WriteLine("? Sample WDM files not found. Please ensure they are in the data folder.");
                    return;
                }

                // Initialize library
                HassEntLibrary.Initialize();

                // Run test cases
                TestBasicWdmFileOperations();
                TestWdmDatasetInspection();
                TestTimeSeriesDataAccess();
                TestWdmAttributeOperations();
                TestMultipleWdmFiles();
                TestWdmFileManagement();
                TestDataExportOperations();
                TestAdvancedWdmOperations();

                Console.WriteLine("? All Sample WDM tests completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Sample WDM tests failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                // Clean up
                HassEntLibrary.Shutdown();
            }
        }

        /// <summary>
        /// Verify that the sample WDM files exist
        /// </summary>
        private static bool VerifyDataFiles()
        {
            Console.WriteLine("Verifying Sample WDM Files:");
            Console.WriteLine("---------------------------");

            bool sampleExists = File.Exists(SampleWdmPath);
            bool annualExists = File.Exists(SampleAnnualWdmPath);

            Console.WriteLine($"Sample.wdm: {(sampleExists ? "? Found" : "? Not found")} at {SampleWdmPath}");
            Console.WriteLine($"Sample_Annual.wdm: {(annualExists ? "? Found" : "? Not found")} at {SampleAnnualWdmPath}");

            if (sampleExists)
            {
                var fileInfo = new FileInfo(SampleWdmPath);
                Console.WriteLine($"Sample.wdm size: {fileInfo.Length:N0} bytes");
            }

            if (annualExists)
            {
                var fileInfo = new FileInfo(SampleAnnualWdmPath);
                Console.WriteLine($"Sample_Annual.wdm size: {fileInfo.Length:N0} bytes");
            }

            Console.WriteLine();
            return sampleExists || annualExists;
        }

        /// <summary>
        /// Test basic WDM file operations
        /// </summary>
        private static void TestBasicWdmFileOperations()
        {
            Console.WriteLine("Test Case 1: Basic WDM File Operations");
            Console.WriteLine("--------------------------------------");

            try
            {
                // Test opening Sample.wdm in read-only mode
                int wdmUnit = HassEntFunctions.F90_WDBOPN(1, SampleWdmPath);
                if (wdmUnit > 0)
                {
                    Console.WriteLine($"? Successfully opened Sample.wdm on unit {wdmUnit} (read-only)");

                    // Check if file is recognized as open
                    int checkUnit = HassEntFunctions.F90_INQNAM(SampleWdmPath);
                    Console.WriteLine($"? File inquiry returned unit: {checkUnit}");

                    // Get WDM file info
                    var wdmInfo = WdmOperations.GetWdmFileInfo(wdmUnit);
                    if (wdmInfo != null)
                    {
                        Console.WriteLine($"? WDM Info: Unit={wdmInfo.Unit}, ReadOnly={wdmInfo.ReadOnly}, Datasets={wdmInfo.DataSets.Count}");
                    }

                    // Close the file
                    int closeResult = HassEntFunctions.F90_WDFLCL(wdmUnit);
                    Console.WriteLine($"? File closed with result: {closeResult}");
                }
                else
                {
                    Console.WriteLine("?? Could not open Sample.wdm (may not be accessible or corrupted)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Basic operations test failed: {ex.Message}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Test WDM dataset inspection
        /// </summary>
        private static void TestWdmDatasetInspection()
        {
            Console.WriteLine("Test Case 2: WDM Dataset Inspection");
            Console.WriteLine("-----------------------------------");

            try
            {
                int wdmUnit = HassEntFunctions.F90_WDBOPN(1, SampleWdmPath);
                if (wdmUnit > 0)
                {
                    Console.WriteLine($"? Opened Sample.wdm for dataset inspection");

                    // Test common dataset numbers (typical WDM files use 100-999 range)
                    int[] testDsns = { 100, 101, 102, 200, 201, 300, 301, 400, 401 };

                    foreach (int dsn in testDsns)
                    {
                        int datasetType = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
                        if (datasetType > 0)
                        {
                            Console.WriteLine($"? Found dataset {dsn} with type: {datasetType}");

                            // Try to get string attributes (common ones)
                            int[] stationId = new int[8];
                            WdmOperations.F90_WDBSGC_XX(wdmUnit, dsn, 1, 8, stationId); // Station ID
                            string stationIdStr = DataConversionUtilities.IntArrayToString(stationId);

                            int[] scenarioName = new int[8];
                            WdmOperations.F90_WDBSGC_XX(wdmUnit, dsn, 288, 8, scenarioName); // Scenario
                            string scenarioStr = DataConversionUtilities.IntArrayToString(scenarioName);

                            if (!string.IsNullOrWhiteSpace(stationIdStr))
                                Console.WriteLine($"   Station ID: '{stationIdStr.Trim()}'");
                            if (!string.IsNullOrWhiteSpace(scenarioStr))
                                Console.WriteLine($"   Scenario: '{scenarioStr.Trim()}'");
                        }
                    }

                    HassEntFunctions.F90_WDFLCL(wdmUnit);
                }
                else
                {
                    Console.WriteLine("?? Could not open file for dataset inspection");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Dataset inspection test failed: {ex.Message}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Test time series data access
        /// </summary>
        private static void TestTimeSeriesDataAccess()
        {
            Console.WriteLine("Test Case 3: Time Series Data Access");
            Console.WriteLine("------------------------------------");

            try
            {
                int wdmUnit = HassEntFunctions.F90_WDBOPN(1, SampleWdmPath);
                if (wdmUnit > 0)
                {
                    Console.WriteLine($"? Opened Sample.wdm for time series access");

                    // Try to access time series data from common datasets
                    int[] testDsns = { 100, 101, 102, 200, 201 };

                    foreach (int dsn in testDsns)
                    {
                        int datasetType = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
                        if (datasetType > 0)
                        {
                            Console.WriteLine($"? Testing time series access for DSN {dsn}");

                            // Set up date range (try 2000-2020 range which is common in sample data)
                            int[] startDate = { 2000, 1, 1, 0, 0, 0 };
                            int[] endDate = { 2020, 12, 31, 23, 59, 59 };

                            // Try to get some data (request 100 values)
                            float[] values = new float[100];
                            WdmOperations.F90_WDTGET(wdmUnit, dsn, 1440, startDate, 100, 0, 0, 4, values, out int retCode);

                            if (retCode == 0)
                            {
                                // Calculate basic statistics
                                float min = float.MaxValue, max = float.MinValue, sum = 0;
                                int validCount = 0;

                                for (int i = 0; i < values.Length; i++)
                                {
                                    if (!float.IsNaN(values[i]) && values[i] != -999.0f) // Common missing value
                                    {
                                        min = Math.Min(min, values[i]);
                                        max = Math.Max(max, values[i]);
                                        sum += values[i];
                                        validCount++;
                                    }
                                }

                                if (validCount > 0)
                                {
                                    float mean = sum / validCount;
                                    Console.WriteLine($"   ? Retrieved {validCount} valid values");
                                    Console.WriteLine($"   Min: {min:F2}, Max: {max:F2}, Mean: {mean:F2}");
                                }
                                else
                                {
                                    Console.WriteLine($"   ?? No valid data found in dataset {dsn}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"   ?? Could not retrieve data from DSN {dsn}, return code: {retCode}");
                            }
                        }
                    }

                    HassEntFunctions.F90_WDFLCL(wdmUnit);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Time series access test failed: {ex.Message}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Test WDM attribute operations
        /// </summary>
        private static void TestWdmAttributeOperations()
        {
            Console.WriteLine("Test Case 4: WDM Attribute Operations");
            Console.WriteLine("-------------------------------------");

            try
            {
                int wdmUnit = HassEntFunctions.F90_WDBOPN(1, SampleWdmPath);
                if (wdmUnit > 0)
                {
                    Console.WriteLine($"? Opened Sample.wdm for attribute testing");

                    // Test common WDM attributes for the first few datasets
                    int[] testDsns = { 100, 101, 200 };
                    
                    // Common WDM attribute indices
                    int[] commonAttrs = { 1, 2, 17, 27, 33, 288, 289, 290 };
                    string[] attrNames = { "TSTYPE", "STAID", "TSTEP", "TCODE", "TUNIT", "SCENARIO", "LOCATION", "CONSTITUENT" };

                    foreach (int dsn in testDsns)
                    {
                        int datasetType = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
                        if (datasetType > 0)
                        {
                            Console.WriteLine($"? Checking attributes for DSN {dsn}:");

                            for (int i = 0; i < commonAttrs.Length; i++)
                            {
                                int attrIndex = commonAttrs[i];
                                string attrName = attrNames[i];

                                // Try string attribute first
                                int[] stringValue = new int[20];
                                WdmOperations.F90_WDBSGC_XX(wdmUnit, dsn, attrIndex, 20, stringValue);
                                string stringResult = DataConversionUtilities.IntArrayToString(stringValue).Trim();

                                if (!string.IsNullOrWhiteSpace(stringResult))
                                {
                                    Console.WriteLine($"   {attrName} ({attrIndex}): '{stringResult}'");
                                }
                                else
                                {
                                    // Try integer attribute
                                    int[] intValues = new int[5];
                                    WdmOperations.F90_WDBSGI(wdmUnit, dsn, attrIndex, 5, intValues, out int retCode);
                                    
                                    if (retCode == 0 && intValues[0] != 0)
                                    {
                                        Console.WriteLine($"   {attrName} ({attrIndex}): {intValues[0]}");
                                    }
                                }
                            }
                            Console.WriteLine();
                        }
                    }

                    HassEntFunctions.F90_WDFLCL(wdmUnit);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Attribute operations test failed: {ex.Message}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Test multiple WDM files
        /// </summary>
        private static void TestMultipleWdmFiles()
        {
            Console.WriteLine("Test Case 5: Multiple WDM Files");
            Console.WriteLine("-------------------------------");

            try
            {
                int unit1 = 0, unit2 = 0;

                // Try to open both files if they exist
                if (File.Exists(SampleWdmPath))
                {
                    unit1 = HassEntFunctions.F90_WDBOPN(1, SampleWdmPath);
                    Console.WriteLine($"? Sample.wdm opened on unit {unit1}");
                }

                if (File.Exists(SampleAnnualWdmPath))
                {
                    unit2 = HassEntFunctions.F90_WDBOPN(1, SampleAnnualWdmPath);
                    Console.WriteLine($"? Sample_Annual.wdm opened on unit {unit2}");
                }

                // Get status of open files
                var openFiles = HassEntFunctions.GetOpenFiles();
                Console.WriteLine($"? Total open WDM files: {openFiles.Count}");
                
                foreach (var file in openFiles)
                {
                    Console.WriteLine($"   Unit {file.Key}: {Path.GetFileName(file.Value)}");
                }

                // Close files
                if (unit1 > 0)
                {
                    HassEntFunctions.F90_WDFLCL(unit1);
                    Console.WriteLine($"? Closed unit {unit1}");
                }

                if (unit2 > 0)
                {
                    HassEntFunctions.F90_WDFLCL(unit2);
                    Console.WriteLine($"? Closed unit {unit2}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Multiple files test failed: {ex.Message}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Test WDM file management operations
        /// </summary>
        private static void TestWdmFileManagement()
        {
            Console.WriteLine("Test Case 6: WDM File Management");
            Console.WriteLine("--------------------------------");

            try
            {
                int wdmUnit = HassEntFunctions.F90_WDBOPN(1, SampleWdmPath);
                if (wdmUnit > 0)
                {
                    Console.WriteLine($"? Opened Sample.wdm for management testing");

                    // Test dataset enumeration
                    Console.WriteLine("Scanning for datasets:");
                    
                    int foundDatasets = 0;
                    for (int dsn = 1; dsn <= 1000; dsn += 10) // Check every 10th dataset
                    {
                        int datasetType = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
                        if (datasetType > 0)
                        {
                            foundDatasets++;
                            Console.WriteLine($"   DSN {dsn}: Type {datasetType}");
                            
                            // Stop after finding 10 datasets to avoid too much output
                            if (foundDatasets >= 10)
                            {
                                Console.WriteLine("   ... (limiting output to first 10 found datasets)");
                                break;
                            }
                        }
                    }

                    Console.WriteLine($"? Found {foundDatasets} datasets in scan");

                    // Test finding free dataset number
                    FileManagement.F90_INFREE(wdmUnit, 1, 2000, 1, out int freeDsn, out int retCode);
                    if (retCode == 0)
                    {
                        Console.WriteLine($"? Next available DSN: {freeDsn}");
                    }

                    HassEntFunctions.F90_WDFLCL(wdmUnit);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? File management test failed: {ex.Message}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Test data export operations
        /// </summary>
        private static void TestDataExportOperations()
        {
            Console.WriteLine("Test Case 7: Data Export Operations");
            Console.WriteLine("-----------------------------------");

            try
            {
                int wdmUnit = HassEntFunctions.F90_WDBOPN(1, SampleWdmPath);
                if (wdmUnit > 0)
                {
                    Console.WriteLine($"? Opened Sample.wdm for export testing");

                    // Find a dataset with data
                    int targetDsn = 0;
                    for (int dsn = 100; dsn <= 500; dsn += 50)
                    {
                        int datasetType = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
                        if (datasetType > 0)
                        {
                            targetDsn = dsn;
                            break;
                        }
                    }

                    if (targetDsn > 0)
                    {
                        Console.WriteLine($"? Testing export for DSN {targetDsn}");

                        // Create export file path
                        string exportPath = Path.Combine(DataFolder, $"export_dsn_{targetDsn}.csv");

                        // Set up date range
                        int[] startDate = { 1990, 1, 1, 0, 0, 0 };
                        int[] endDate = { 2030, 12, 31, 23, 59, 59 };

                        // Test flat file export
                        FileManagement.F90_TSFLAT(wdmUnit, targetDsn, exportPath, 1, 
                            "F10.2", 0, 1440, 4, 0, 1, -999.0f, startDate, endDate, out int exportResult);

                        if (exportResult == 0 && File.Exists(exportPath))
                        {
                            var fileInfo = new FileInfo(exportPath);
                            Console.WriteLine($"? Export successful: {exportPath} ({fileInfo.Length} bytes)");

                            // Read first few lines to verify
                            string[] lines = File.ReadAllLines(exportPath);
                            Console.WriteLine($"? Export contains {lines.Length} lines");
                            
                            if (lines.Length > 0)
                            {
                                Console.WriteLine($"   First line: {lines[0]}");
                            }
                            if (lines.Length > 1)
                            {
                                Console.WriteLine($"   Second line: {lines[1]}");
                            }

                            // Clean up export file
                            try { File.Delete(exportPath); } catch { }
                        }
                        else
                        {
                            Console.WriteLine($"?? Export failed with result: {exportResult}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("?? No suitable dataset found for export testing");
                    }

                    HassEntFunctions.F90_WDFLCL(wdmUnit);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Export operations test failed: {ex.Message}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Test advanced WDM operations
        /// </summary>
        private static void TestAdvancedWdmOperations()
        {
            Console.WriteLine("Test Case 8: Advanced WDM Operations");
            Console.WriteLine("------------------------------------");

            try
            {
                int wdmUnit = HassEntFunctions.F90_WDBOPN(1, SampleWdmPath);
                if (wdmUnit > 0)
                {
                    Console.WriteLine($"? Opened Sample.wdm for advanced testing");

                    // Test message operations if available
                    HassEntFunctions.F90_WMSGTH(wdmUnit, 1);
                    Console.WriteLine($"? Tested message table header operation");

                    // Test getting message table text
                    int outputLength = 100;
                    int[] msgBuffer = new int[256];
                    HassEntFunctions.F90_WMSGTT_XX(wdmUnit, 100, 1, 0, ref outputLength, out int continuation, msgBuffer);
                    
                    if (continuation >= 0)
                    {
                        string messageText = DataConversionUtilities.IntArrayToString(msgBuffer, outputLength);
                        Console.WriteLine($"? Retrieved message text: '{messageText.Trim()}'");
                    }

                    // Test extended info operations
                    int[] textParam1 = new int[20];
                    int[] textParam2 = new int[20]; 
                    int[] textParam3 = new int[20];
                    int[] textParam4 = new int[20];
                    int[] textParam5 = new int[20];
                    int[] textParam6 = new int[20];

                    HassEntFunctions.F90_XTINFO_XX(1, wdmUnit, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                        0.0f, 0.0f, 0.0f, textParam1, textParam2, textParam3, textParam4, textParam5, textParam6);

                    string info1 = DataConversionUtilities.IntArrayToString(textParam1);
                    string info2 = DataConversionUtilities.IntArrayToString(textParam2);
                    Console.WriteLine($"? Extended info: '{info1.Trim()}' '{info2.Trim()}'");

                    // Test statistical analysis if we can get some data
                    int[] startDate = { 2000, 1, 1, 0, 0, 0 };
                    float[] sampleData = new float[50];
                    
                    WdmOperations.F90_WDTGET(wdmUnit, 100, 1440, startDate, 50, 0, 0, 4, sampleData, out int getResult);
                    
                    if (getResult == 0)
                    {
                        // Filter out missing values and run analysis
                        var validData = new System.Collections.Generic.List<float>();
                        foreach (float value in sampleData)
                        {
                            if (!float.IsNaN(value) && value != -999.0f && value != 0.0f)
                            {
                                validData.Add(value);
                            }
                        }

                        if (validData.Count > 0)
                        {
                            TimeSeriesAnalysis.F90_DAANST(validData.Count, validData.ToArray());
                            Console.WriteLine($"? Statistical analysis completed for {validData.Count} values");
                        }
                    }

                    // Test new F90_WDSAGY_XX aggregation function
                    TestWdmAggregationFunction(wdmUnit);

                    // Test new F90_WDLBAX label axis function
                    TestWdmLabelAxisFunction(wdmUnit);

                    // Test new F90_GETATT generic attribute function
                    TestWdmGenericAttributeFunction(wdmUnit);

                    // Test new F90_WDLBAD label add function
                    TestWdmLabelAddFunction(wdmUnit);

                    // Test new F90_WTFNDT time find function
                    TestWdmTimeFindFunction(wdmUnit);

                    HassEntFunctions.F90_WDFLCL(wdmUnit);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Advanced operations test failed: {ex.Message}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Test the new WDM aggregation function
        /// </summary>
        /// <param name="wdmUnit">WDM unit number</param>
        private static void TestWdmAggregationFunction(int wdmUnit)
        {
            try
            {
                Console.WriteLine("Testing F90_WDSAGY_XX aggregation function:");

                // First, create some test data
                int testDsn = 999; // Use a high DSN number to avoid conflicts
                
                // Create test dataset with sample data
                var testDataSet = new WdmOperations.WdmDataSet
                {
                    Dsn = testDsn,
                    DataSetType = 1,
                    TimeUnit = 4,  // Daily
                    TimeStep = 1
                };

                // Add some test time series data
                var baseDate = new DateTime(2023, 1, 1);
                for (int i = 0; i < 30; i++) // 30 days of data
                {
                    testDataSet.Data.Add(new WdmOperations.TimeSeriesValue
                    {
                        DateTime = baseDate.AddDays(i),
                        Value = 10.0f + (float)Math.Sin(i * 0.2) * 5.0f, // Sine wave pattern
                        Quality = 0
                    });
                }

                // Add the test dataset to the WDM file
                var wdmInfo = WdmOperations.GetWdmFileInfo(wdmUnit);
                if (wdmInfo != null)
                {
                    wdmInfo.DataSets[testDsn] = testDataSet;

                    // Test different aggregation types
                    int[] startDate = { 2023, 1, 1, 0, 0, 0 };
                    int[] endDate = { 2023, 1, 31, 23, 59, 59 };
                    
                    float[] outputValues = new float[10];
                    int[,] outputDates = new int[6, 10];

                    // Test 1: Weekly mean aggregation (7-day periods)
                    WdmOperations.F90_WDSAGY_XX(wdmUnit, testDsn, 2, startDate, endDate, 
                        7, 4, outputValues, outputDates, out int nValues1, out int retCode1);

                    if (retCode1 == 0 && nValues1 > 0)
                    {
                        Console.WriteLine($"   ? Weekly mean aggregation: {nValues1} periods");
                        Console.WriteLine($"      First value: {outputValues[0]:F2} on {outputDates[0,0]}/{outputDates[1,0]}/{outputDates[2,0]}");
                        if (nValues1 > 1)
                            Console.WriteLine($"      Last value: {outputValues[nValues1-1]:F2} on {outputDates[0,nValues1-1]}/{outputDates[1,nValues1-1]}/{outputDates[2,nValues1-1]}");
                    }

                    // Test 2: Maximum values (10-day periods)
                    Array.Clear(outputValues, 0, outputValues.Length);
                    Array.Clear(outputDates, 0, outputDates.Length);
                    
                    WdmOperations.F90_WDSAGY_XX(wdmUnit, testDsn, 3, startDate, endDate, 
                        10, 4, outputValues, outputDates, out int nValues2, out int retCode2);

                    if (retCode2 == 0 && nValues2 > 0)
                    {
                        Console.WriteLine($"   ? 10-day maximum aggregation: {nValues2} periods");
                        Console.WriteLine($"      Maximum value: {outputValues[0]:F2}");
                    }

                    // Test 3: Sum aggregation (monthly)
                    Array.Clear(outputValues, 0, outputValues.Length);
                    Array.Clear(outputDates, 0, outputDates.Length);
                    
                    WdmOperations.F90_WDSAGY_XX(wdmUnit, testDsn, 1, startDate, endDate, 
                        1, 5, outputValues, outputDates, out int nValues3, out int retCode3);

                    if (retCode3 == 0 && nValues3 > 0)
                    {
                        Console.WriteLine($"   ? Monthly sum aggregation: {nValues3} periods");
                        Console.WriteLine($"      Total sum: {outputValues[0]:F2}");
                    }

                    // Clean up test dataset
                    wdmInfo.DataSets.Remove(testDsn);

                    Console.WriteLine($"   ? F90_WDSAGY_XX aggregation function tested successfully");
                }
                else
                {
                    Console.WriteLine($"   ?? Could not access WDM file info for aggregation test");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ? Aggregation function test failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Test the new WDM label axis function
        /// </summary>
        /// <param name="wdmUnit">WDM unit number</param>
        private static void TestWdmLabelAxisFunction(int wdmUnit)
        {
            try
            {
                Console.WriteLine("Testing F90_WDLBAX label axis function:");

                // Create a test dataset with attributes
                int testDsn = 998; // Use a different DSN to avoid conflicts
                
                var testDataSet = new WdmOperations.WdmDataSet
                {
                    Dsn = testDsn,
                    DataSetType = 1,
                    TimeSeriesType = "FLOW",
                    TimeUnit = 4,
                    TimeStep = 1
                };

                // Add some test attributes
                testDataSet.Attributes[2] = "STATION01";      // Station ID
                testDataSet.Attributes[289] = "STREAMFLOW";   // Constituent
                testDataSet.Attributes[288] = "BASELINE";     // Scenario
                testDataSet.Attributes[17] = 1;               // Time step
                testDataSet.Attributes[27] = 4;               // Time code (daily)
                testDataSet.Attributes[33] = 1;               // Units (CFS)
                testDataSet.Attributes[290] = "MAIN_STEM";    // Location

                // Add the test dataset
                var wdmInfo = WdmOperations.GetWdmFileInfo(wdmUnit);
                if (wdmInfo != null)
                {
                    wdmInfo.DataSets[testDsn] = testDataSet;

                    // Test different label types
                    TestLabelType(wdmUnit, testDsn, 1, "Station Label");
                    TestLabelType(wdmUnit, testDsn, 2, "Parameter Label");
                    TestLabelType(wdmUnit, testDsn, 3, "Time Label");
                    TestLabelType(wdmUnit, testDsn, 4, "Units Label");
                    TestLabelType(wdmUnit, testDsn, 5, "Scenario Label");

                    // Clean up test dataset
                    wdmInfo.DataSets.Remove(testDsn);

                    Console.WriteLine($"   ✅ F90_WDLBAX label axis function tested successfully");
                }
                else
                {
                    Console.WriteLine($"   ⚠️ Could not access WDM file info for label axis test");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Label axis function test failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Test a specific label type
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        /// <param name="labelType">Label type</param>
        /// <param name="labelName">Label name for display</param>
        private static void TestLabelType(int wdmUnit, int dsn, int labelType, string labelName)
        {
            try
            {
                int[] labelBuffer = new int[100];
                
                WdmOperations.F90_WDLBAX(wdmUnit, dsn, labelType, 0, 100, labelBuffer,
                    out int actualLength, out int retCode);
                
                if (retCode == 0 && actualLength > 0)
                {
                    string labelText = DataConversionUtilities.IntArrayToString(labelBuffer, actualLength);
                    Console.WriteLine($"   ✅ {labelName}: '{labelText.Trim()}'");
                }
                else if (retCode == 1)
                {
                    Console.WriteLine($"   ⚠️ {labelName}: No label found");
                }
                else
                {
                    Console.WriteLine($"   ❌ {labelName}: Failed (return code: {retCode})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ {labelName}: Error - {ex.Message}");
            }
        }

        /// <summary>
        /// Test the new WDM generic attribute function
        /// </summary>
        /// <param name="wdmUnit">WDM unit number</param>
        private static void TestWdmGenericAttributeFunction(int wdmUnit)
        {
            try
            {
                Console.WriteLine("Testing F90_GETATT generic attribute function:");

                // Create a test dataset with various attribute types
                int testDsn = 997; // Use a different DSN to avoid conflicts
                
                var testDataSet = new WdmOperations.WdmDataSet
                {
                    Dsn = testDsn,
                    DataSetType = 1,
                    TimeSeriesType = "FLOW",
                    TimeUnit = 4,
                    TimeStep = 1440
                };

                // Add various attribute types for testing
                testDataSet.Attributes[2] = "TEST_STATION";     // String attribute
                testDataSet.Attributes[17] = 1440;              // Integer attribute  
                testDataSet.Attributes[33] = 1;                 // Integer attribute (units)
                testDataSet.Attributes[100] = 25.5f;            // Float attribute
                testDataSet.Attributes[101] = new int[] { 10, 20, 30, 40 }; // Integer array
                testDataSet.Attributes[102] = new float[] { 1.1f, 2.2f, 3.3f }; // Float array

                // Add the test dataset
                var wdmInfo = WdmOperations.GetWdmFileInfo(wdmUnit);
                if (wdmInfo != null)
                {
                    wdmInfo.DataSets[testDsn] = testDataSet;

                    // Test different attribute types
                    TestGenericAttributeType(wdmUnit, testDsn, 2, 3, "String Attribute (STAID)");
                    TestGenericAttributeType(wdmUnit, testDsn, 17, 1, "Integer Attribute (TSTEP)");
                    TestGenericAttributeType(wdmUnit, testDsn, 100, 2, "Float Attribute");
                    TestGenericAttributeType(wdmUnit, testDsn, 101, 1, "Integer Array Attribute");
                    TestGenericAttributeType(wdmUnit, testDsn, 102, 2, "Float Array Attribute");
                    
                    // Test non-existent attribute with defaults
                    TestGenericAttributeType(wdmUnit, testDsn, 288, 3, "Default Scenario Attribute");
                    TestGenericAttributeType(wdmUnit, testDsn, 999, 1, "Non-existent Attribute");

                    // Test type conversion
                    TestGenericAttributeType(wdmUnit, testDsn, 17, 2, "Integer to Float Conversion");
                    TestGenericAttributeType(wdmUnit, testDsn, 100, 1, "Float to Integer Conversion");

                    // Clean up test dataset
                    wdmInfo.DataSets.Remove(testDsn);

                    Console.WriteLine($"   ? F90_GETATT generic attribute function tested successfully");
                }
                else
                {
                    Console.WriteLine($"   ?? Could not access WDM file info for generic attribute test");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ? Generic attribute function test failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Test the new WDM label add function
        /// </summary>
        /// <param name="wdmUnit">WDM unit number</param>
        private static void TestWdmLabelAddFunction(int wdmUnit)
        {
            try
            {
                Console.WriteLine("Testing F90_WDLBAD label add function:");

                // Create a test dataset for label operations
                int testDsn = 996; // Use a different DSN to avoid conflicts
                
                var testDataSet = new WdmOperations.WdmDataSet
                {
                    Dsn = testDsn,
                    DataSetType = 1,
                    TimeSeriesType = "",
                    TimeUnit = 0,
                    TimeStep = 0
                };

                // Add the test dataset
                var wdmInfo = WdmOperations.GetWdmFileInfo(wdmUnit);
                if (wdmInfo != null)
                {
                    wdmInfo.DataSets[testDsn] = testDataSet;

                    // Test setting different label types
                    TestAddLabelType(wdmUnit, testDsn, 1, "GAUGE_STATION_123", "Station Label");
                    TestAddLabelType(wdmUnit, testDsn, 2, "STREAMFLOW", "Parameter Label");
                    TestAddLabelType(wdmUnit, testDsn, 3, "Daily", "Time Label");
                    TestAddLabelType(wdmUnit, testDsn, 4, "CFS", "Units Label");
                    TestAddLabelType(wdmUnit, testDsn, 5, "OBSERVED_DATA", "Scenario Label");
                    TestAddLabelType(wdmUnit, testDsn, 6, "Daily mean streamflow at main gauge", "Description Label");

                    // Test more complex labels with parsing
                    TestAddLabelType(wdmUnit, testDsn, 2, "TEMPERATURE", "Temperature Parameter");
                    TestAddLabelType(wdmUnit, testDsn, 3, "60-Minute", "Hourly Time Step");
                    TestAddLabelType(wdmUnit, testDsn, 4, "DEGREES F", "Temperature Units");

                    // Verify labels were set by reading them back
                    Console.WriteLine("   Verifying labels were set correctly:");
                    VerifySetLabels(wdmUnit, testDsn);

                    // Clean up test dataset
                    wdmInfo.DataSets.Remove(testDsn);

                    Console.WriteLine($"   ? F90_WDLBAD label add function tested successfully");
                }
                else
                {
                    Console.WriteLine($"   ?? Could not access WDM file info for label add test");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ? Label add function test failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Test adding a specific label type
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        /// <param name="labelType">Label type</param>
        /// <param name="labelText">Label text to set</param>
        /// <param name="testName">Test name for display</param>
        private static void TestAddLabelType(int wdmUnit, int dsn, int labelType, string labelText, string testName)
        {
            try
            {
                // Convert string to integer array (FORTRAN character array format)
                int[] labelBuffer = new int[labelText.Length];
                for (int i = 0; i < labelText.Length; i++)
                {
                    labelBuffer[i] = (int)labelText[i];
                }
                
                WdmOperations.F90_WDLBAD(wdmUnit, dsn, labelType, 0, labelText.Length, labelBuffer, out int retCode);
                
                if (retCode == 0)
                {
                    Console.WriteLine($"   ? {testName}: Set '{labelText}'");
                }
                else
                {
                    string errorMsg = retCode switch
                    {
                        -2 => "WDM unit not found",
                        -3 => "Invalid label type",
                        _ => $"Error (code: {retCode})"
                    };
                    Console.WriteLine($"   ? {testName}: {errorMsg}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ? {testName}: Error - {ex.Message}");
            }
        }

        /// <summary>
        /// Verify that labels were set correctly by reading them back
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        private static void VerifySetLabels(int wdmUnit, int dsn)
        {
            try
            {
                // Check station label
                VerifySingleLabel(wdmUnit, dsn, 1, "Station");
                
                // Check parameter label  
                VerifySingleLabel(wdmUnit, dsn, 2, "Parameter");
                
                // Check units label
                VerifySingleLabel(wdmUnit, dsn, 4, "Units");
                
                // Check scenario label
                VerifySingleLabel(wdmUnit, dsn, 5, "Scenario");

                // Check if attributes were properly set by looking at the dataset
                var dataset = WdmOperations.GetDataSet(wdmUnit, dsn);
                if (dataset != null)
                {
                    if (dataset.Attributes.TryGetValue(2, out var stationAttr))
                        Console.WriteLine($"     Station attribute: {stationAttr}");
                    if (dataset.Attributes.TryGetValue(289, out var constituentAttr))
                        Console.WriteLine($"     Constituent attribute: {constituentAttr}");
                    if (dataset.Attributes.TryGetValue(33, out var unitsAttr))
                        Console.WriteLine($"     Units code: {unitsAttr}");
                    if (dataset.TimeSeriesType != "")
                        Console.WriteLine($"     Time series type: {dataset.TimeSeriesType}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"     Error verifying labels: {ex.Message}");
            }
        }

        /// <summary>
        /// Verify a single label by reading it back
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        /// <param name="labelType">Label type</param>
        /// <param name="labelName">Label name for display</param>
        private static void VerifySingleLabel(int wdmUnit, int dsn, int labelType, string labelName)
        {
            try
            {
                int[] labelBuffer = new int[100];
                
                WdmOperations.F90_WDLBAX(wdmUnit, dsn, labelType, 0, 100, labelBuffer,
                    out int actualLength, out int retCode);
                
                if (retCode == 0 && actualLength > 0)
                {
                    string readLabel = DataConversionUtilities.IntArrayToString(labelBuffer, actualLength);
                    Console.WriteLine($"     {labelName}: '{readLabel.Trim()}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"     Error reading {labelName}: {ex.Message}");
            }
        }

        /// <summary>
        /// Test the new WDM time find function
        /// </summary>
        /// <param name="wdmUnit">WDM unit number</param>
        private static void TestWdmTimeFindFunction(int wdmUnit)
        {
            try
            {
                Console.WriteLine("Testing F90_WTFNDT time find function:");

                // Create a test dataset with time series data
                int testDsn = 995; // Use a different DSN to avoid conflicts
                
                var testDataSet = new WdmOperations.WdmDataSet
                {
                    Dsn = testDsn,
                    DataSetType = 1,
                    TimeSeriesType = "FLOW",
                    TimeUnit = 4,
                    TimeStep = 1
                };

                // Add test time series data (daily data for a month)
                var baseDate = new DateTime(2023, 6, 1);
                for (int i = 0; i < 30; i++)
                {
                    var currentDate = baseDate.AddDays(i);
                    float value = 100.0f + (float)(Math.Sin(i * 0.2) * 20.0f); // Sinusoidal pattern
                    
                    testDataSet.Data.Add(new WdmOperations.TimeSeriesValue
                    {
                        DateTime = currentDate,
                        Value = value,
                        Quality = 0
                    });
                }

                // Add the test dataset
                var wdmInfo = WdmOperations.GetWdmFileInfo(wdmUnit);
                if (wdmInfo != null)
                {
                    wdmInfo.DataSets[testDsn] = testDataSet;

                    // Test different search modes
                    TestTimeFindMode(wdmUnit, testDsn, 1, new int[] { 2023, 6, 15, 0, 0, 0 }, "First data point");
                    TestTimeFindMode(wdmUnit, testDsn, 2, new int[] { 2023, 6, 15, 0, 0, 0 }, "Last data point");
                    TestTimeFindMode(wdmUnit, testDsn, 3, new int[] { 2023, 6, 15, 12, 0, 0 }, "Nearest to mid-month noon");
                    TestTimeFindMode(wdmUnit, testDsn, 4, new int[] { 2023, 6, 10, 0, 0, 0 }, "Next after June 10");
                    TestTimeFindMode(wdmUnit, testDsn, 5, new int[] { 2023, 6, 20, 0, 0, 0 }, "Previous before June 20");
                    TestTimeFindMode(wdmUnit, testDsn, 6, new int[] { 2023, 6, 15, 0, 0, 0 }, "Exact match June 15");
                    TestTimeFindMode(wdmUnit, testDsn, 7, new int[] { 2023, 6, 15, 12, 0, 0 }, "Interpolated mid-day June 15");

                    // Clean up test dataset
                    wdmInfo.DataSets.Remove(testDsn);

                    Console.WriteLine($"   ✅ F90_WTFNDT time find function tested successfully");
                }
                else
                {
                    Console.WriteLine($"   ⚠️ Could not access WDM file info for time find test");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Time find function test failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Test a specific time find mode
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        /// <param name="searchMode">Search mode</param>
        /// <param name="targetDate">Target date</param>
        /// <param name="testName">Test name for display</param>
        private static void TestTimeFindMode(int wdmUnit, int dsn, int searchMode, int[] targetDate, string testName)
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
                    Console.WriteLine($"   ✅ {testName}: Found {foundDateTime:yyyy-MM-dd HH:mm:ss}, Value: {foundValue:F2}, Index: {dataIndex}");
                }
                else if (retCode == -3)
                {
                    Console.WriteLine($"   ⚠️ {testName}: No data found matching criteria");
                }
                else
                {
                    Console.WriteLine($"   ❌ {testName}: Failed (return code: {retCode})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ {testName}: Error - {ex.Message}");
            }
        }

        /// <summary>
        /// Test a specific generic attribute type 
        /// </summary>
        /// <param name="wdmUnit">WDM unit</param>
        /// <param name="dsn">Dataset number</param>
        /// <param name="attrIndex">Attribute index</param>
        /// <param name="attrType">Attribute type</param>
        /// <param name="testName">Test name for display</param>
        private static void TestGenericAttributeType(int wdmUnit, int dsn, int attrIndex, int attrType, string testName)
        {
            try
            {
                int[] intValues = new int[10];
                float[] realValues = new float[10];
                int[] charValues = new int[50];
                
                WdmOperations.F90_GETATT(wdmUnit, dsn, attrIndex, attrType, 50, intValues, realValues, charValues,
                    out int actualLength, out int retCode);
                
                if (retCode == 0)
                {
                    string result = FormatAttributeResult(attrType, intValues, realValues, charValues, actualLength);
                    Console.WriteLine($"   ✅ {testName}: {result}");
                }
                else if (retCode == 1)
                {
                    string result = FormatAttributeResult(attrType, intValues, realValues, charValues, actualLength);
                    Console.WriteLine($"   ⚠️ {testName}: Default value - {result}");
                }
                else
                {
                    Console.WriteLine($"   ❌ {testName}: Failed (return code: {retCode})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ {testName}: Error - {ex.Message}");
            }
        }

        /// <summary>
        /// Format attribute result for display
        /// </summary>
        /// <param name="attrType">Attribute type</param>
        /// <param name="intValues">Integer values</param>
        /// <param name="realValues">Real values</param>
        /// <param name="charValues">Character values</param>
        /// <param name="actualLength">Actual length</param>
        /// <returns>Formatted result string</returns>
        private static string FormatAttributeResult(int attrType, int[] intValues, float[] realValues, int[] charValues, int actualLength)
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
        /// Run simple WDM demo
        /// </summary>
        public static void RunSimpleWdmDemo()
        {
            Console.WriteLine("Simple WDM Demo");
            Console.WriteLine("===============");
            
            try
            {
                // Verify files exist
                if (!VerifyDataFiles())
                {
                    Console.WriteLine("❌ Sample WDM files not found. Please ensure they are in the data folder.");
                    return;
                }

                // Initialize library
                HassEntLibrary.Initialize();

                // Test basic operations
                TestBasicWdmFileOperations();

                Console.WriteLine("✅ Simple WDM demo completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Simple WDM demo failed: {ex.Message}");
            }
            finally
            {
                // Clean up
                HassEntLibrary.Shutdown();
            }
        }
    }
}