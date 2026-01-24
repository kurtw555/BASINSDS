using System;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Quick test of the F90_WDLBAD label add function
    /// </summary>
    public static class QuickLabelAddTest
    {
        public static void RunTest()
        {
            Console.WriteLine("F90_WDLBAD Label Add Quick Test");
            Console.WriteLine("===============================");

            try
            {
                HassEntLibrary.Initialize();
                
                // Create a mock WDM file
                int wdmUnit = 104;
                var wdmInfo = new WdmOperations.WdmFileInfo
                {
                    Unit = wdmUnit,
                    FileName = "test_label_add.wdm",
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
                int dsn = 400;

                Console.WriteLine($"? Created mock WDM file on unit {wdmUnit}");
                Console.WriteLine($"Testing label operations on dataset {dsn}");

                Console.WriteLine("\nSetting comprehensive dataset labels:");

                // Test all label types with realistic data
                TestSetLabel(wdmUnit, dsn, 1, "USGS_12345678", "Station ID");
                TestSetLabel(wdmUnit, dsn, 2, "STREAMFLOW", "Parameter");
                TestSetLabel(wdmUnit, dsn, 3, "Daily", "Time Step");
                TestSetLabel(wdmUnit, dsn, 4, "CFS", "Units");
                TestSetLabel(wdmUnit, dsn, 5, "OBSERVED", "Scenario");
                TestSetLabel(wdmUnit, dsn, 6, "Daily mean discharge at river mile 23.5", "Description");

                Console.WriteLine("\nTesting more complex labels:");

                // Test labels that trigger intelligent parsing
                TestSetLabel(wdmUnit, dsn, 2, "TEMPERATURE", "Temperature Parameter");
                TestSetLabel(wdmUnit, dsn, 3, "60-Minute", "Hourly Time Step");
                TestSetLabel(wdmUnit, dsn, 4, "DEGREES F", "Temperature Units");
                TestSetLabel(wdmUnit, dsn, 5, "CALIBRATION_RUN_01", "Simulation Scenario");

                Console.WriteLine("\nVerifying labels by reading them back:");
                
                // Verify labels using F90_WDLBAX
                VerifyLabel(wdmUnit, dsn, 1, "Station ID");
                VerifyLabel(wdmUnit, dsn, 2, "Parameter");
                VerifyLabel(wdmUnit, dsn, 3, "Time Step");
                VerifyLabel(wdmUnit, dsn, 4, "Units");
                VerifyLabel(wdmUnit, dsn, 5, "Scenario");
                VerifyLabel(wdmUnit, dsn, 6, "Description");

                Console.WriteLine("\nChecking dataset attributes that were automatically set:");
                
                // Check the dataset to see what attributes were set
                var dataset = WdmOperations.GetDataSet(wdmUnit, dsn);
                if (dataset != null)
                {
                    ShowSetAttributes(dataset);
                }

                HassEntLibrary.Shutdown();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Test failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private static void TestSetLabel(int wdmUnit, int dsn, int labelType, string labelText, string labelName)
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
                
                if (retCode == 0)
                {
                    Console.WriteLine($"? {labelName}: Set '{labelText}'");
                }
                else
                {
                    string errorMsg = retCode switch
                    {
                        -1 => "General error",
                        -2 => "WDM unit not found",
                        -3 => "Invalid label type",
                        _ => $"Unknown error (code: {retCode})"
                    };
                    Console.WriteLine($"? {labelName}: {errorMsg}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? {labelName}: Exception - {ex.Message}");
            }
        }

        private static void VerifyLabel(int wdmUnit, int dsn, int labelType, string labelName)
        {
            try
            {
                int[] labelBuffer = new int[200];
                
                WdmOperations.F90_WDLBAX(wdmUnit, dsn, labelType, 0, 200, labelBuffer,
                    out int actualLength, out int retCode);
                
                if (retCode == 0 && actualLength > 0)
                {
                    string readLabel = DataConversionUtilities.IntArrayToString(labelBuffer, actualLength);
                    Console.WriteLine($"? {labelName}: '{readLabel.Trim()}'");
                }
                else if (retCode == 1)
                {
                    Console.WriteLine($"?? {labelName}: No label found");
                }
                else
                {
                    Console.WriteLine($"? {labelName}: Error reading (code: {retCode})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? {labelName}: Read error - {ex.Message}");
            }
        }

        private static void ShowSetAttributes(WdmOperations.WdmDataSet dataset)
        {
            try
            {
                Console.WriteLine("Attributes automatically set:");
                
                if (dataset.Attributes.TryGetValue(2, out var stationId))
                    Console.WriteLine($"  STAID (2): {stationId}");
                    
                if (dataset.Attributes.TryGetValue(289, out var constituent))
                    Console.WriteLine($"  CONSTITUENT (289): {constituent}");
                    
                if (dataset.Attributes.TryGetValue(1, out var tstype))
                    Console.WriteLine($"  TSTYPE (1): {tstype}");
                    
                if (dataset.Attributes.TryGetValue(17, out var tstep))
                    Console.WriteLine($"  TSTEP (17): {tstep}");
                    
                if (dataset.Attributes.TryGetValue(27, out var tcode))
                    Console.WriteLine($"  TCODE (27): {tcode}");
                    
                if (dataset.Attributes.TryGetValue(33, out var tunit))
                    Console.WriteLine($"  TUNIT (33): {tunit}");
                    
                if (dataset.Attributes.TryGetValue(84, out var unitsText))
                    Console.WriteLine($"  Units text (84): {unitsText}");
                    
                if (dataset.Attributes.TryGetValue(288, out var scenario))
                    Console.WriteLine($"  SCENARIO (288): {scenario}");
                    
                if (dataset.Attributes.TryGetValue(85, out var description))
                    Console.WriteLine($"  Description (85): {description}");

                Console.WriteLine($"Dataset TimeSeriesType: {dataset.TimeSeriesType}");
                Console.WriteLine($"Dataset TimeStep: {dataset.TimeStep}");
                Console.WriteLine($"Dataset TimeUnit: {dataset.TimeUnit}");
                
                Console.WriteLine($"Total attributes set: {dataset.Attributes.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error showing attributes: {ex.Message}");
            }
        }
    }
}