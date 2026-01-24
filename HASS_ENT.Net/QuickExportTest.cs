using System;
using System.IO;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Quick test for enhanced export format with Station_ID and Variable columns
    /// </summary>
    public static class QuickExportTest
    {
        public static void RunTest()
        {
            Console.WriteLine("Quick Enhanced Export Format Test");
            Console.WriteLine("=================================");

            try
            {
                HassEntLibrary.Initialize();
                
                // Create a test WDM file
                int wdmUnit = 105;
                WdmOperations.RegisterWdmFile(wdmUnit, "test_enhanced_export.wdm", false);

                // Create test dataset with realistic attributes and data
                var wdmInfo = WdmOperations.GetWdmFileInfo(wdmUnit);
                if (wdmInfo != null)
                {
                    var testDataSet = new WdmOperations.WdmDataSet
                    {
                        Dsn = 100,
                        DataSetType = 1,
                        TimeSeriesType = "PEVT",  // Potential ET
                        TimeUnit = 4,
                        TimeStep = 1
                    };

                    // Set realistic WDM attributes like your example
                    testDataSet.Attributes[2] = "72030763";      // Station ID (STAID)
                    testDataSet.Attributes[289] = "PEVT";        // Constituent/Variable
                    testDataSet.Attributes[17] = 60;             // Time step in minutes (hourly)
                    testDataSet.Attributes[27] = 3;              // Time code (hours)
                    testDataSet.Attributes[33] = 1;              // Units

                    // Add hourly test data like your example
                    var startDate = new DateTime(2015, 1, 1, 0, 0, 0);
                    float[] sampleValues = { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0.001f, 0.00217f, 0.00284f, 0.00284f, 0.00284f, 0.00284f, 0.00284f, 0.00217f, 0.001f };
                    
                    for (int i = 0; i < sampleValues.Length; i++)
                    {
                        testDataSet.Data.Add(new WdmOperations.TimeSeriesValue
                        {
                            DateTime = startDate.AddHours(i),
                            Value = sampleValues[i],
                            Quality = 0
                        });
                    }

                    wdmInfo.DataSets[100] = testDataSet;

                    Console.WriteLine("Testing both export formats:");
                    Console.WriteLine();

                    // Test standard format
                    TestStandardFormat(wdmUnit, testDataSet);
                    Console.WriteLine();

                    // Test enhanced format (like your example)
                    TestEnhancedFormat(wdmUnit, testDataSet);
                }
                else
                {
                    Console.WriteLine("? Could not create test WDM file");
                }

                HassEntLibrary.Shutdown();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Test failed: {ex.Message}");
            }
        }

        private static void TestStandardFormat(int wdmUnit, WdmOperations.WdmDataSet dataSet)
        {
            Console.WriteLine("1. Standard Export Format:");
            Console.WriteLine("   ------------------------");

            string exportFile = "test_standard_export.csv";
            int[] exportStartDate = { 2015, 1, 1, 0, 0, 0 };
            int[] exportEndDate = { 2015, 1, 2, 23, 59, 59 };

            FileManagement.F90_TSFLAT(wdmUnit, 100, exportFile, 1,
                "F10.5", 0, 60, 3, 0, 1, -999.0f, exportStartDate, exportEndDate, out int result);

            if (result == 0 && File.Exists(exportFile))
            {
                Console.WriteLine("   ? Standard export successful!");
                ShowFileContents(exportFile, 8);
                try { File.Delete(exportFile); } catch { }
            }
            else
            {
                Console.WriteLine($"   ? Standard export failed with result: {result}");
            }
        }

        private static void TestEnhancedFormat(int wdmUnit, WdmOperations.WdmDataSet dataSet)
        {
            Console.WriteLine("2. Enhanced Export Format (Station_ID, Variable, DateTime, Value):");
            Console.WriteLine("   ---------------------------------------------------------------");

            string exportFile = "test_enhanced_export.csv";
            int[] exportStartDate = { 2015, 1, 1, 0, 0, 0 };
            int[] exportEndDate = { 2015, 1, 2, 23, 59, 59 };

            FileManagement.F90_TSFLAT_EX(wdmUnit, 100, exportFile, 1,
                "F10.5", 0, 60, 3, 0, 1, -999.0f, exportStartDate, exportEndDate, true, out int result);

            if (result == 0 && File.Exists(exportFile))
            {
                Console.WriteLine("   ? Enhanced export successful!");
                Console.WriteLine("   ?? Output matches your required format:");
                ShowFileContents(exportFile, 10);
                try { File.Delete(exportFile); } catch { }
            }
            else
            {
                Console.WriteLine($"   ? Enhanced export failed with result: {result}");
            }
        }

        private static void ShowFileContents(string fileName, int maxLines)
        {
            try
            {
                string[] lines = File.ReadAllLines(fileName);
                Console.WriteLine($"   ?? File contains {lines.Length} lines:");
                
                int linesToShow = Math.Min(maxLines, lines.Length);
                for (int i = 0; i < linesToShow; i++)
                {
                    Console.WriteLine($"      {lines[i]}");
                }
                
                if (lines.Length > maxLines)
                {
                    Console.WriteLine($"      ... ({lines.Length - maxLines} more lines)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ? Error reading file: {ex.Message}");
            }
        }
    }
}