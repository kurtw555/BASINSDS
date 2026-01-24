using System;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Quick test of the F90_WDSAGY_XX function
    /// </summary>
    public static class QuickAggregationTest
    {
        public static void RunTest()
        {
            Console.WriteLine("F90_WDSAGY_XX Quick Test");
            Console.WriteLine("========================");

            try
            {
                HassEntLibrary.Initialize();
                
                // Create a mock WDM file
                int wdmUnit = 101;
                var wdmInfo = new WdmOperations.WdmFileInfo
                {
                    Unit = wdmUnit,
                    FileName = "test.wdm",
                    ReadOnly = false
                };

                // Manually add it to the internal collection (for testing purposes)
                var wdmFilesField = typeof(WdmOperations).GetField("_wdmFiles", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                if (wdmFilesField?.GetValue(null) is System.Collections.Generic.Dictionary<int, WdmOperations.WdmFileInfo> wdmFiles)
                {
                    wdmFiles[wdmUnit] = wdmInfo;
                }

                // Create test dataset
                int dsn = 100;
                var testDataSet = new WdmOperations.WdmDataSet
                {
                    Dsn = dsn,
                    DataSetType = 1,
                    TimeUnit = 4,
                    TimeStep = 1
                };

                // Add sample time series data
                var baseDate = new DateTime(2024, 1, 1);
                for (int i = 0; i < 14; i++) // 2 weeks of daily data
                {
                    testDataSet.Data.Add(new WdmOperations.TimeSeriesValue
                    {
                        DateTime = baseDate.AddDays(i),
                        Value = 5.0f + i * 0.5f, // Linear increase
                        Quality = 0
                    });
                }

                wdmInfo.DataSets[dsn] = testDataSet;

                Console.WriteLine($"? Created test dataset with {testDataSet.Data.Count} values");
                Console.WriteLine("Sample values:");
                for (int i = 0; i < Math.Min(5, testDataSet.Data.Count); i++)
                {
                    var val = testDataSet.Data[i];
                    Console.WriteLine($"  {val.DateTime:yyyy-MM-dd}: {val.Value:F2}");
                }

                // Test aggregation: Weekly mean
                int[] startDate = { 2024, 1, 1, 0, 0, 0 };
                int[] endDate = { 2024, 1, 31, 23, 59, 59 };
                float[] outputValues = new float[5];
                int[,] outputDates = new int[6, 5];

                WdmOperations.F90_WDSAGY_XX(wdmUnit, dsn, 2, startDate, endDate,
                    7, 4, outputValues, outputDates, out int nValues, out int retCode);

                if (retCode == 0 && nValues > 0)
                {
                    Console.WriteLine($"\n? Weekly mean aggregation successful!");
                    Console.WriteLine($"Number of aggregated periods: {nValues}");
                    
                    for (int i = 0; i < nValues; i++)
                    {
                        Console.WriteLine($"Week {i+1}: {outputValues[i]:F2} " +
                            $"(starting {outputDates[0,i]}/{outputDates[1,i]:D2}/{outputDates[2,i]:D2})");
                    }
                }
                else
                {
                    Console.WriteLine($"? Aggregation failed with return code: {retCode}");
                }

                HassEntLibrary.Shutdown();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Test failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}