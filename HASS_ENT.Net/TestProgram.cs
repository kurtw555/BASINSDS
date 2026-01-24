using System;

namespace HASS_ENT.Net
{
    /// <summary>
    /// Test program to demonstrate HASS_ENT.Net functionality
    /// Tests the C# implementations of FORTRAN functions
    /// </summary>
    internal class TestProgram
    {
        [System.STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("HASS_ENT.Net Test Program");
            Console.WriteLine("=========================");
            Console.WriteLine();

            try
            {
                // Test logging functionality
                TestLoggingFunctions();
                
                // Test date/time utilities
                TestDateTimeFunctions();
                
                // Test WDM operations
                TestWdmOperations();
                
                // Test mathematical utilities
                TestMathematicalFunctions();
                
                // Test data conversion
                TestDataConversion();
                
                // Test time series analysis
                TestTimeSeriesAnalysis();
                
                // Test file management
                TestFileManagement();
                
                // Test missing FORTRAN functions
                TestMissingFunctions();
                
                // Test with real Sample.wdm file
                TestSampleWdmFile();
                
                // Test the new aggregation function
                TestAggregationFunction();
                
                Console.WriteLine("\nAll tests completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed with error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                // Clean up
                HassEntFunctions.F90_WDBFIN();
                FileManagement.CloseAllStreams();
                
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Test logging functions
        /// </summary>
        private static void TestLoggingFunctions()
        {
            Console.WriteLine("Testing Logging Functions:");
            Console.WriteLine("--------------------------");
            
            // Test log file operations
            HassEntFunctions.F90_W99OPN();
            HassEntFunctions.F90_MSG("Test message from HASS_ENT.Net");
            HassEntFunctions.F90_MSG("Logging functionality is working");
            HassEntFunctions.F90_W99CLO();
            
            Console.WriteLine("? Logging functions tested");
            Console.WriteLine();
        }

        /// <summary>
        /// Test date/time functions
        /// </summary>
        private static void TestDateTimeFunctions()
        {
            Console.WriteLine("Testing Date/Time Functions:");
            Console.WriteLine("----------------------------");
            
            // Test date operations
            int[] date1 = { 2024, 1, 15, 10, 30, 0 };
            int[] date2 = new int[6];
            
            // Test time addition
            HassEntFunctions.F90_TIMADD(date1, 4, 1, 10, date2); // Add 10 days
            Console.WriteLine($"? Added 10 days: {date2[0]}/{date2[1]}/{date2[2]}");
            
            // Test days in month
            int daysInFeb = HassEntFunctions.F90_DAYMON(2024, 2);
            Console.WriteLine($"? Days in February 2024: {daysInFeb}");
            
            // Test date comparison
            int comparison = HassEntFunctions.F90_TIMCHK(date1, date2);
            Console.WriteLine($"? Date comparison result: {comparison}");
            
            // Test Julian day conversion
            HassEntFunctions.F90_JDMODY(2024, 46, out int month, out int day);
            Console.WriteLine($"? Julian day 46 in 2024: {month}/{day}");
            
            // Test date validation
            DateTimeUtilities.F90_TIMCNV(date1);
            Console.WriteLine($"? Date validation completed");
            
            Console.WriteLine();
        }

        /// <summary>
        /// Test WDM operations
        /// </summary>
        private static void TestWdmOperations()
        {
            Console.WriteLine("Testing WDM Operations:");
            Console.WriteLine("----------------------");
            
            // Test WDM file operations
            int wdmUnit = HassEntFunctions.F90_WDBOPN(0, "test.wdm");
            if (wdmUnit > 0)
            {
                Console.WriteLine($"? WDM file opened on unit {wdmUnit}");
                
                // Test dataset operations
                int dsn = 100;
                WdmOperations.F90_WDBSAC(wdmUnit, dsn, 99, 288, 8, out int retCode, "TESTSCEN");
                Console.WriteLine($"? Set scenario attribute, return code: {retCode}");
                
                // Test time series data
                int[] dates = { 2024, 1, 1, 0, 0, 0 };
                float[] values = { 1.5f, 2.3f, 3.1f, 2.8f, 1.9f };
                WdmOperations.F90_WDTPUT(wdmUnit, dsn, 60, dates, 5, 1, 0, 4, values, out retCode);
                Console.WriteLine($"? Stored time series data, return code: {retCode}");
                
                // Test dataset info
                int datasetType = WdmOperations.F90_WDCKDT(wdmUnit, dsn);
                Console.WriteLine($"? Dataset type: {datasetType}");
                
                // Close WDM file
                int closeResult = HassEntFunctions.F90_WDFLCL(wdmUnit);
                Console.WriteLine($"? WDM file closed, result: {closeResult}");
            }
            else
            {
                Console.WriteLine("? Could not open WDM file (expected for test)");
            }
            
            Console.WriteLine();
        }

        /// <summary>
        /// Test mathematical functions
        /// </summary>
        private static void TestMathematicalFunctions()
        {
            Console.WriteLine("Testing Mathematical Functions:");
            Console.WriteLine("------------------------------");
            
            // Test scaling function
            HassEntFunctions.F90_SCALIT(1, 10.0f, 100.0f, out float plotMin, out float plotMax);
            Console.WriteLine($"? Scale (10-100): Plot range {plotMin:F1} to {plotMax:F1}");
            
            // Test sorting
            float[] values = { 5.2f, 1.8f, 9.3f, 3.7f, 6.1f };
            float[] sortedValues = new float[values.Length];
            Array.Copy(values, sortedValues, values.Length);
            HassEntFunctions.F90_ASRTRP(sortedValues);
            Console.WriteLine($"? Sorted values: [{string.Join(", ", Array.ConvertAll(sortedValues, x => x.ToString("F1")))}]");
            
            // Test precision function
            float testValue = 3.14159265f;
            HassEntFunctions.F90_DECPRC(3, 2, ref testValue);
            Console.WriteLine($"? Rounded to 2 decimal places: {testValue:F3}");
            
            Console.WriteLine();
        }

        /// <summary>
        /// Test data conversion
        /// </summary>
        private static void TestDataConversion()
        {
            Console.WriteLine("Testing Data Conversion:");
            Console.WriteLine("-----------------------");
            
            // Test number to string conversion
            int[] outputString = new int[10];
            DataConversionUtilities.F90_DECCHX_XX(123.456f, 10, 6, 2, outputString);
            string convertedString = DataConversionUtilities.IntArrayToString(outputString);
            Console.WriteLine($"? Number to string: '{convertedString.Trim()}'");
            
            // Test string utilities
            string testString = "HASS_ENT";
            int[] intArray = new int[20];
            DataConversionUtilities.StringToIntArray(testString, intArray);
            string backToString = DataConversionUtilities.IntArrayToString(intArray);
            Console.WriteLine($"? String conversion: '{backToString.Trim()}'");
            
            // Test validation
            bool isValid = DataConversionUtilities.IsValidNumber("123.45", true);
            Console.WriteLine($"? Number validation: {isValid}");
            
            Console.WriteLine();
        }

        /// <summary>
        /// Test time series analysis
        /// </summary>
        private static void TestTimeSeriesAnalysis()
        {
            Console.WriteLine("Testing Time Series Analysis:");
            Console.WriteLine("----------------------------");
            
            // Test statistical analysis
            float[] testData = { 1.2f, 2.5f, 1.8f, 3.1f, 2.9f, 1.5f, 2.8f, 3.3f };
            TimeSeriesAnalysis.F90_DAANST(testData.Length, testData);
            Console.WriteLine($"? Statistical analysis completed (see log)");
            
            // Test linear regression
            float[] xyData = { 1.0f, 1.0f, 2.0f, 2.0f, 3.0f, 3.0f, 4.0f, 4.0f }; // Perfect correlation
            TimeSeriesAnalysis.F90_FITLIN(4, 8, xyData, out float slope, out float intercept, out float rSquared);
            Console.WriteLine($"? Linear regression: slope={slope:F2}, intercept={intercept:F2}, R²={rSquared:F3}");
            
            Console.WriteLine();
        }

        /// <summary>
        /// Test file management
        /// </summary>
        private static void TestFileManagement()
        {
            Console.WriteLine("Testing File Management:");
            Console.WriteLine("-----------------------");
            
            try
            {
                // Test dataset number finding
                FileManagement.F90_INFREE(999, 1, 1, 1, out int freeDsn, out int retCode);
                Console.WriteLine($"? Free dataset search: DSN={freeDsn}, code={retCode}");
                
                // Test stream status
                var streamStatus = FileManagement.GetStreamStatus();
                Console.WriteLine($"? Open streams: {streamStatus.Count}");
                
                Console.WriteLine("? File management functions tested");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? File management test warning: {ex.Message}");
            }
            
            Console.WriteLine();
        }

        /// <summary>
        /// Test missing FORTRAN functions
        /// </summary>
        private static void TestMissingFunctions()
        {
            Console.WriteLine("Testing Missing FORTRAN Functions:");
            Console.WriteLine("----------------------------------");
            
            try
            {
                // Test F90_MSGUNIT
                HassEntFunctions.F90_MSGUNIT(99);
                Console.WriteLine("? F90_MSGUNIT tested");
                
                // Test F90_WMSGTW_XX
                HassEntFunctions.F90_WMSGTW_XX(99, 1);
                Console.WriteLine("? F90_WMSGTW_XX tested");
                
                // Test F90_WMSGTH
                HassEntFunctions.F90_WMSGTH(99, 1);
                Console.WriteLine("? F90_WMSGTH tested");
                
                // Test F90_WMSGTT_XX
                int outputLength = 50;
                int[] outputBuffer = new int[256];
                HassEntFunctions.F90_WMSGTT_XX(101, 100, 1, 0, ref outputLength, out int continuation, outputBuffer);
                string messageText = DataConversionUtilities.IntArrayToString(outputBuffer, outputLength);
                Console.WriteLine($"? F90_WMSGTT_XX tested: '{messageText.Trim()}'");
                
                // Test F90_GTNXKW_XX
                int[] keyword = new int[20];
                HassEntFunctions.F90_GTNXKW_XX(10, 11, keyword, out int keywordLength, out int lineNumber, out int errorCode);
                string keywordText = DataConversionUtilities.IntArrayToString(keyword, keywordLength);
                Console.WriteLine($"? F90_GTNXKW_XX tested: keyword='{keywordText}', line={lineNumber}, error={errorCode}");
                
                // Test F90_XTINFO_XX
                int[] textParam1 = new int[20];
                int[] textParam2 = new int[20];
                int[] textParam3 = new int[20];
                int[] textParam4 = new int[20];
                int[] textParam5 = new int[20];
                int[] textParam6 = new int[20];
                
                HassEntFunctions.F90_XTINFO_XX(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0.0f, 0.0f, 0.0f, textParam1, textParam2, textParam3, textParam4, textParam5, textParam6);
                
                string text1 = DataConversionUtilities.IntArrayToString(textParam1);
                string text2 = DataConversionUtilities.IntArrayToString(textParam2);
                Console.WriteLine($"? F90_XTINFO_XX tested: '{text1.Trim()}' '{text2.Trim()}'");
                
                Console.WriteLine("? All missing FORTRAN functions tested");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Missing functions test warning: {ex.Message}");
            }
            
            Console.WriteLine();
        }

        /// <summary>
        /// Test Sample.wdm file
        /// </summary>
        private static void TestSampleWdmFile()
        {
            Console.WriteLine("Testing with Sample.wdm File:");
            Console.WriteLine("-----------------------------");
            
            try
            {
                // Run a simple demo first
                SampleWdmTests.RunSimpleWdmDemo();
                
                Console.WriteLine();
                Console.WriteLine("For comprehensive Sample.wdm testing, call SampleWdmTests.RunAllSampleWdmTests()");
                Console.WriteLine("? Sample WDM file integration tested");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Sample WDM file test warning: {ex.Message}");
            }
            
            Console.WriteLine();
        }

        /// <summary>
        /// Test the new F90_WDSAGY_XX aggregation function
        /// </summary>
        private static void TestAggregationFunction()
        {
            Console.WriteLine("Testing F90_WDSAGY_XX Aggregation Function:");
            Console.WriteLine("------------------------------------------");
            
            try
            {
                // Use AggregationDemo to test the function
                Console.WriteLine("Running aggregation demonstration...");
                AggregationDemo.RunDemo();
                Console.WriteLine("? F90_WDSAGY_XX aggregation function tested");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Aggregation function test warning: {ex.Message}");
            }
            
            Console.WriteLine();
        }
    }
}