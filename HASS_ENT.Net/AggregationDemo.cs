using System;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Simple demonstration of the F90_WDSAGY_XX aggregation function
    /// </summary>
    public static class AggregationDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("F90_WDSAGY_XX Aggregation Function Demo");
            Console.WriteLine("=======================================");
            
            try
            {
                HassEntLibrary.Initialize();
                
                // Create a temporary WDM file in memory
                string testWdmPath = "temp_test.wdm";
                int wdmUnit = HassEntFunctions.F90_WDBOPN(0, testWdmPath);
                
                if (wdmUnit > 0)
                {
                    Console.WriteLine($"? Created temporary WDM unit: {wdmUnit}");
                    
                    // Create test dataset
                    int testDsn = 100;
                    CreateSampleTimeSeries(wdmUnit, testDsn);
                    
                    // Test different aggregation types
                    TestAggregationTypes(wdmUnit, testDsn);
                    
                    HassEntFunctions.F90_WDFLCL(wdmUnit);
                }
                
                HassEntLibrary.Shutdown();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Demo failed: {ex.Message}");
            }
        }
        
        private static void CreateSampleTimeSeries(int wdmUnit, int dsn)
        {
            try
            {
                // Get WDM file info and create dataset
                var wdmInfo = WdmOperations.GetWdmFileInfo(wdmUnit);
                if (wdmInfo != null)
                {
                    var dataSet = new WdmOperations.WdmDataSet
                    {
                        Dsn = dsn,
                        DataSetType = 1,
                        TimeUnit = 4,  // Daily
                        TimeStep = 1
                    };
                    
                    // Create 30 days of sample data (January 2024)
                    var startDate = new DateTime(2024, 1, 1);
                    Console.WriteLine("Creating sample time series data:");
                    
                    for (int i = 0; i < 30; i++)
                    {
                        float value = 10.0f + (float)Math.Sin(i * 0.3) * 5.0f + (i % 7) * 2.0f;
                        
                        dataSet.Data.Add(new WdmOperations.TimeSeriesValue
                        {
                            DateTime = startDate.AddDays(i),
                            Value = value,
                            Quality = 0
                        });
                        
                        if (i < 5) // Show first 5 values
                        {
                            Console.WriteLine($"  Day {i+1}: {value:F2}");
                        }
                    }
                    
                    wdmInfo.DataSets[dsn] = dataSet;
                    Console.WriteLine($"? Created {dataSet.Data.Count} daily values for DSN {dsn}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating sample data: {ex.Message}");
            }
        }
        
        private static void TestAggregationTypes(int wdmUnit, int dsn)
        {
            try
            {
                Console.WriteLine("\nTesting aggregation types:");
                Console.WriteLine("=========================");
                
                int[] startDate = { 2024, 1, 1, 0, 0, 0 };
                int[] endDate = { 2024, 1, 31, 23, 59, 59 };
                
                // Test 1: Weekly Sum
                TestSingleAggregation(wdmUnit, dsn, 1, "Weekly Sum", startDate, endDate, 7, 4);
                
                // Test 2: Weekly Mean
                TestSingleAggregation(wdmUnit, dsn, 2, "Weekly Mean", startDate, endDate, 7, 4);
                
                // Test 3: Weekly Maximum
                TestSingleAggregation(wdmUnit, dsn, 3, "Weekly Maximum", startDate, endDate, 7, 4);
                
                // Test 4: Weekly Minimum
                TestSingleAggregation(wdmUnit, dsn, 4, "Weekly Minimum", startDate, endDate, 7, 4);
                
                // Test 5: Weekly Count
                TestSingleAggregation(wdmUnit, dsn, 5, "Weekly Count", startDate, endDate, 7, 4);
                
                // Test 6: Monthly aggregation
                TestSingleAggregation(wdmUnit, dsn, 2, "Monthly Mean", startDate, endDate, 1, 5);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in aggregation testing: {ex.Message}");
            }
        }
        
        private static void TestSingleAggregation(int wdmUnit, int dsn, int aggType, string name, 
            int[] startDate, int[] endDate, int timeStep, int timeUnit)
        {
            try
            {
                float[] outputValues = new float[10];
                int[,] outputDates = new int[6, 10];
                
                WdmOperations.F90_WDSAGY_XX(wdmUnit, dsn, aggType, startDate, endDate,
                    timeStep, timeUnit, outputValues, outputDates, out int nValues, out int retCode);
                
                if (retCode == 0 && nValues > 0)
                {
                    Console.WriteLine($"\n? {name}:");
                    Console.WriteLine($"   Periods: {nValues}");
                    
                    for (int i = 0; i < Math.Min(nValues, 5); i++) // Show first 5 results
                    {
                        Console.WriteLine($"   Period {i+1}: {outputValues[i]:F2} " +
                            $"(starting {outputDates[0,i]}/{outputDates[1,i]:D2}/{outputDates[2,i]:D2})");
                    }
                    
                    if (nValues > 5)
                        Console.WriteLine($"   ... and {nValues - 5} more periods");
                }
                else
                {
                    Console.WriteLine($"\n? {name}: Failed (return code: {retCode})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n? {name}: Error - {ex.Message}");
            }
        }
        
        public static void Main(string[] args)
        {
            RunDemo();
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}