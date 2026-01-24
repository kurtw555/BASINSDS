using System;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Quick test of the F90_GETATT generic attribute function
    /// </summary>
    public static class QuickGenericAttributeTest
    {
        public static void RunTest()
        {
            Console.WriteLine("F90_GETATT Generic Attribute Quick Test");
            Console.WriteLine("=======================================");

            try
            {
                HassEntLibrary.Initialize();
                
                // Create a mock WDM file
                int wdmUnit = 103;
                var wdmInfo = new WdmOperations.WdmFileInfo
                {
                    Unit = wdmUnit,
                    FileName = "test_attributes.wdm",
                    ReadOnly = false
                };

                // Manually add it to the internal collection (for testing purposes)
                var wdmFilesField = typeof(WdmOperations).GetField("_wdmFiles", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                if (wdmFilesField?.GetValue(null) is System.Collections.Generic.Dictionary<int, WdmOperations.WdmFileInfo> wdmFiles)
                {
                    wdmFiles[wdmUnit] = wdmInfo;
                }

                // Create test dataset with various attribute types
                int dsn = 300;
                var testDataSet = new WdmOperations.WdmDataSet
                {
                    Dsn = dsn,
                    DataSetType = 1,
                    TimeSeriesType = "TEMP",
                    TimeUnit = 3,
                    TimeStep = 1
                };

                // Add comprehensive attributes for testing
                testDataSet.Attributes[2] = "TEMP_STATION_01";        // String attribute (Station ID)
                testDataSet.Attributes[17] = 60;                      // Integer attribute (60-minute time step)
                testDataSet.Attributes[27] = 3;                       // Integer attribute (hourly)
                testDataSet.Attributes[33] = 3;                       // Integer attribute (degrees F)
                testDataSet.Attributes[100] = 72.5f;                  // Float attribute (temperature)
                testDataSet.Attributes[101] = new int[] { 1, 2, 3, 4, 5 }; // Integer array
                testDataSet.Attributes[102] = new float[] { 32.0f, 68.0f, 100.0f }; // Float array (temps)
                testDataSet.Attributes[288] = "CALIBRATION";          // String attribute (scenario)
                testDataSet.Attributes[289] = "TEMPERATURE";          // String attribute (constituent)

                wdmInfo.DataSets[dsn] = testDataSet;

                Console.WriteLine($"? Created test dataset {dsn} with comprehensive attributes:");
                Console.WriteLine("  Station ID: TEMP_STATION_01 (string)");
                Console.WriteLine("  Time Step: 60 minutes (integer)");
                Console.WriteLine("  Units: Degrees F (integer code 3)");
                Console.WriteLine("  Temperature: 72.5°F (float)");
                Console.WriteLine("  Integer Array: [1,2,3,4,5]");
                Console.WriteLine("  Float Array: [32.0,68.0,100.0] (temps)");

                Console.WriteLine("\nTesting F90_GETATT with different attribute types:");

                // Test 1: String attribute as string
                TestAttribute(wdmUnit, dsn, 2, 3, "Station ID as String");

                // Test 2: Integer attribute as integer
                TestAttribute(wdmUnit, dsn, 17, 1, "Time Step as Integer");

                // Test 3: Float attribute as float
                TestAttribute(wdmUnit, dsn, 100, 2, "Temperature as Float");

                // Test 4: Integer array as integer array
                TestAttribute(wdmUnit, dsn, 101, 1, "Integer Array");

                // Test 5: Float array as float array
                TestAttribute(wdmUnit, dsn, 102, 2, "Float Array");

                // Test 6: Type conversions - Integer to Float
                TestAttribute(wdmUnit, dsn, 17, 2, "Time Step as Float (conversion)");

                // Test 7: Type conversions - Float to Integer
                TestAttribute(wdmUnit, dsn, 100, 1, "Temperature as Integer (conversion)");

                // Test 8: Type conversions - Integer to String
                TestAttribute(wdmUnit, dsn, 17, 3, "Time Step as String (conversion)");

                // Test 9: Default values for missing attributes
                TestAttribute(wdmUnit, dsn, 288, 3, "Scenario (existing)");
                TestAttribute(wdmUnit, dsn, 290, 3, "Location (default)");

                // Test 10: Non-existent attribute
                TestAttribute(wdmUnit, dsn, 999, 1, "Non-existent Attribute");

                HassEntLibrary.Shutdown();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Test failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private static void TestAttribute(int wdmUnit, int dsn, int attrIndex, int attrType, string testName)
        {
            try
            {
                int[] intValues = new int[10];
                float[] realValues = new float[10];
                int[] charValues = new int[100];
                
                WdmOperations.F90_GETATT(wdmUnit, dsn, attrIndex, attrType, 100, intValues, realValues, charValues,
                    out int actualLength, out int retCode);
                
                string resultText = "";
                string statusIcon = "";
                
                if (retCode == 0)
                {
                    statusIcon = "?";
                    resultText = FormatResult(attrType, intValues, realValues, charValues, actualLength);
                }
                else if (retCode == 1)
                {
                    statusIcon = "??";
                    resultText = "Default: " + FormatResult(attrType, intValues, realValues, charValues, actualLength);
                }
                else
                {
                    statusIcon = "?";
                    resultText = GetErrorMessage(retCode);
                }
                
                Console.WriteLine($"{statusIcon} {testName}: {resultText} (length: {actualLength}, code: {retCode})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? {testName}: Exception - {ex.Message}");
            }
        }

        private static string FormatResult(int attrType, int[] intValues, float[] realValues, int[] charValues, int actualLength)
        {
            return attrType switch
            {
                1 => actualLength == 1 ? intValues[0].ToString() : 
                     $"[{string.Join(", ", System.Linq.Enumerable.Take(intValues, actualLength))}]",
                2 => actualLength == 1 ? realValues[0].ToString("F2") : 
                     $"[{string.Join(", ", System.Linq.Enumerable.Take(realValues, actualLength).Select(f => f.ToString("F2")))}]",
                3 => $"'{DataConversionUtilities.IntArrayToString(charValues, actualLength).Trim()}'",
                _ => "Unknown type"
            };
        }

        private static string GetErrorMessage(int retCode)
        {
            return retCode switch
            {
                -1 => "General error",
                -2 => "Dataset not found",
                -3 => "Type conversion error",
                -4 => "Attribute not found",
                _ => $"Unknown error (code: {retCode})"
            };
        }
    }
}