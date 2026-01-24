using System;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Quick test of the F90_WTFNDT time find function
    /// </summary>
    public static class QuickTimeFindTest
    {
        public static void RunTest()
        {
            Console.WriteLine("F90_WTFNDT Time Find Quick Test");
            Console.WriteLine("===============================");

            try
            {
                HassEntLibrary.Initialize();
                
                // Create a mock WDM file
                int wdmUnit = 105;
                var wdmInfo = new WdmOperations.WdmFileInfo
                {
                    Unit = wdmUnit,
                    FileName = "test_time_find.wdm",
                    ReadOnly = false
                };

                // Manually add it to the internal collection (for testing purposes)
                var wdmFilesField = typeof(WdmOperations).GetField("_wdmFiles", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                if (wdmFilesField?.GetValue(null) is System.Collections.Generic.Dictionary<int, WdmOperations.WdmFileInfo> wdmFiles)
                {
                    wdmFiles[wdmUnit] = wdmInfo;
                }

                // Test dataset number
                int dsn = 500;

                Console.WriteLine($"? Created mock WDM file on unit {wdmUnit}");
                Console.WriteLine($"Testing time find operations on dataset {dsn}");

                // Create test dataset with time series data
                var testDataSet = new WdmOperations.WdmDataSet
                {
                    Dsn = dsn,
                    DataSetType = 1,
                    TimeSeriesType = "FLOW",
                    TimeUnit = 4,
                    TimeStep = 1
                };

                // Add test time series data (30 days of daily data)
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

                wdmInfo.DataSets[dsn] = testDataSet;

                Console.WriteLine($"? Created {testDataSet.Data.Count} daily time series values for testing");

                // Test target date
                int[] targetDate = { 2023, 6, 15, 12, 0, 0 }; // Mid-month, noon

                Console.WriteLine("\nTesting different search modes:");

                // Test all search modes
                TestTimeFindMode(wdmUnit, dsn, 1, targetDate, "First data point");
                TestTimeFindMode(wdmUnit, dsn, 2, targetDate, "Last data point");
                TestTimeFindMode(wdmUnit, dsn, 3, targetDate, "Nearest to target");
                TestTimeFindMode(wdmUnit, dsn, 4, targetDate, "Next after target");
                TestTimeFindMode(wdmUnit, dsn, 5, targetDate, "Previous before target");
                TestTimeFindMode(wdmUnit, dsn, 6, targetDate, "Exact match");
                TestTimeFindMode(wdmUnit, dsn, 7, targetDate, "Interpolated value");

                // Test with different target dates
                Console.WriteLine("\nTesting with different target dates:");
                TestTimeFindMode(wdmUnit, dsn, 3, new int[] { 2023, 5, 30, 0, 0, 0 }, "Before data range");
                TestTimeFindMode(wdmUnit, dsn, 3, new int[] { 2023, 7, 5, 0, 0, 0 }, "After data range");
                TestTimeFindMode(wdmUnit, dsn, 6, new int[] { 2023, 6, 1, 0, 0, 0 }, "Exact first date");
                TestTimeFindMode(wdmUnit, dsn, 6, new int[] { 2023, 6, 30, 0, 0, 0 }, "Exact last date");

                HassEntLibrary.Shutdown();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Test failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

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
                    var targetDateTime = new DateTime(targetDate[0], targetDate[1], targetDate[2], 
                                                    targetDate[3], targetDate[4], targetDate[5]);
                    
                    Console.WriteLine($"? {testName}:");
                    Console.WriteLine($"   Target: {targetDateTime:yyyy-MM-dd HH:mm:ss}");
                    Console.WriteLine($"   Found: {foundDateTime:yyyy-MM-dd HH:mm:ss}");
                    Console.WriteLine($"   Value: {foundValue:F2}");
                    Console.WriteLine($"   Quality: {foundQuality}");
                    Console.WriteLine($"   Index: {dataIndex}");
                }
                else if (retCode == -3)
                {
                    Console.WriteLine($"?? {testName}: No data found matching criteria");
                }
                else
                {
                    Console.WriteLine($"? {testName}: Failed (return code: {retCode})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? {testName}: Error - {ex.Message}");
            }
        }
    }
}