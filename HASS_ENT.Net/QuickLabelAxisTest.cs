using System;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Quick test of the F90_WDLBAX label axis function
    /// </summary>
    public static class QuickLabelAxisTest
    {
        public static void RunTest()
        {
            Console.WriteLine("F90_WDLBAX Label Axis Quick Test");
            Console.WriteLine("================================");

            try
            {
                HassEntLibrary.Initialize();
                
                // Create a mock WDM file
                int wdmUnit = 102;
                var wdmInfo = new WdmOperations.WdmFileInfo
                {
                    Unit = wdmUnit,
                    FileName = "test_labels.wdm",
                    ReadOnly = false
                };

                // Manually add it to the internal collection (for testing purposes)
                var wdmFilesField = typeof(WdmOperations).GetField("_wdmFiles", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                if (wdmFilesField?.GetValue(null) is System.Collections.Generic.Dictionary<int, WdmOperations.WdmFileInfo> wdmFiles)
                {
                    wdmFiles[wdmUnit] = wdmInfo;
                }

                // Create test dataset with comprehensive attributes
                int dsn = 200;
                var testDataSet = new WdmOperations.WdmDataSet
                {
                    Dsn = dsn,
                    DataSetType = 1,
                    TimeSeriesType = "FLOW",
                    TimeUnit = 4,
                    TimeStep = 1
                };

                // Add comprehensive WDM attributes
                testDataSet.Attributes[2] = "USGS_01010000";     // Station ID
                testDataSet.Attributes[289] = "STREAMFLOW";      // Constituent
                testDataSet.Attributes[288] = "HISTORICAL";      // Scenario
                testDataSet.Attributes[290] = "MAIN_RIVER";      // Location
                testDataSet.Attributes[17] = 1440;               // Time step (1440 min = daily)
                testDataSet.Attributes[27] = 4;                  // Time code (daily)
                testDataSet.Attributes[33] = 1;                  // Units code (CFS)
                testDataSet.Attributes[45] = "Main River Gauge"; // Station name
                testDataSet.Attributes[83] = "Discharge";        // Parameter name
                testDataSet.Attributes[84] = "cubic feet per second"; // Units text
                testDataSet.Attributes[85] = "Daily mean discharge"; // Description

                wdmInfo.DataSets[dsn] = testDataSet;

                Console.WriteLine($"? Created test dataset {dsn} with comprehensive attributes");
                Console.WriteLine("Dataset attributes:");
                Console.WriteLine("  Station ID: USGS_01010000");
                Console.WriteLine("  Parameter: STREAMFLOW"); 
                Console.WriteLine("  Scenario: HISTORICAL");
                Console.WriteLine("  Time Step: Daily");
                Console.WriteLine("  Units: CFS");

                Console.WriteLine("\nTesting label retrieval:");

                // Test all label types
                TestLabelRetrieval(wdmUnit, dsn, 1, "Station");
                TestLabelRetrieval(wdmUnit, dsn, 2, "Parameter");
                TestLabelRetrieval(wdmUnit, dsn, 3, "Time");
                TestLabelRetrieval(wdmUnit, dsn, 4, "Units");
                TestLabelRetrieval(wdmUnit, dsn, 5, "Scenario");
                TestLabelRetrieval(wdmUnit, dsn, 6, "Description");

                // Test with non-existent label type
                TestLabelRetrieval(wdmUnit, dsn, 99, "Unknown");

                // Test with non-existent dataset
                TestLabelRetrieval(wdmUnit, 9999, 1, "NonExistent");

                HassEntLibrary.Shutdown();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Test failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private static void TestLabelRetrieval(int wdmUnit, int dsn, int labelType, string labelName)
        {
            try
            {
                int[] labelBuffer = new int[100];
                
                WdmOperations.F90_WDLBAX(wdmUnit, dsn, labelType, 0, 100, labelBuffer,
                    out int actualLength, out int retCode);
                
                if (retCode == 0 && actualLength > 0)
                {
                    string labelText = DataConversionUtilities.IntArrayToString(labelBuffer, actualLength);
                    Console.WriteLine($"? {labelName} Label: '{labelText.Trim()}' (length: {actualLength})");
                }
                else if (retCode == 1)
                {
                    Console.WriteLine($"?? {labelName} Label: No label available");
                }
                else if (retCode == -2)
                {
                    Console.WriteLine($"? {labelName} Label: Dataset not found");
                }
                else
                {
                    Console.WriteLine($"? {labelName} Label: Error (return code: {retCode})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? {labelName} Label: Exception - {ex.Message}");
            }
        }
    }
}