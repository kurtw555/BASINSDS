using System;
using System.Collections.Generic;
using System.IO;

namespace HASS_ENT.Net
{
    /// <summary>
    /// MODFLOW data reader for groundwater modeling integration
    /// Reads and processes MODFLOW input and output files for hydrologic modeling
    /// </summary>
    public class ModflowDataReader : HassBaseOperation
    {
        public override string OperationName => "MODFLOW Data Reader";

        private string _inputFilePath = "";
        private Dictionary<string, float[,]> _gridData = new();
        
        /// <summary>
        /// Set the input file path
        /// </summary>
        /// <param name="filePath">Path to MODFLOW file</param>
        public void SetInputFile(string filePath)
        {
            _inputFilePath = filePath;
            LogProgress($"Input file set: {filePath}");
        }

        /// <summary>
        /// Read MODFLOW grid data
        /// </summary>
        /// <param name="arrayName">Name of data array</param>
        /// <returns>2D data array or null if not found</returns>
        public float[,]? GetGridData(string arrayName)
        {
            return _gridData.TryGetValue(arrayName, out var data) ? data : null;
        }

        protected override bool ExecuteInternal()
        {
            if (string.IsNullOrEmpty(_inputFilePath))
            {
                LogError("No input file specified");
                return false;
            }

            if (!File.Exists(_inputFilePath))
            {
                LogError($"Input file not found: {_inputFilePath}");
                return false;
            }

            try
            {
                LogProgress("Reading MODFLOW data file");
                
                // Simplified MODFLOW file reading
                using var reader = new StreamReader(_inputFilePath);
                string? line;
                string currentArray = "";
                var arrayData = new List<float[]>();
                
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Trim().StartsWith("INTERNAL"))
                    {
                        // New array starting
                        if (!string.IsNullOrEmpty(currentArray) && arrayData.Count > 0)
                        {
                            // Store previous array
                            StoreArrayData(currentArray, arrayData);
                        }
                        
                        currentArray = $"Array_{_gridData.Count + 1}";
                        arrayData.Clear();
                    }
                    else if (IsDataLine(line))
                    {
                        // Parse data line
                        var values = ParseDataLine(line);
                        if (values != null)
                            arrayData.Add(values);
                    }
                }
                
                // Store last array
                if (!string.IsNullOrEmpty(currentArray) && arrayData.Count > 0)
                {
                    StoreArrayData(currentArray, arrayData);
                }
                
                LogProgress($"Successfully read {_gridData.Count} data arrays");
                return true;
            }
            catch (Exception ex)
            {
                LogError($"Error reading MODFLOW file: {ex.Message}");
                return false;
            }
        }

        private void StoreArrayData(string arrayName, List<float[]> arrayData)
        {
            if (arrayData.Count == 0) return;
            
            int rows = arrayData.Count;
            int cols = arrayData[0].Length;
            var grid = new float[rows, cols];
            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < Math.Min(cols, arrayData[i].Length); j++)
                {
                    grid[i, j] = arrayData[i][j];
                }
            }
            
            _gridData[arrayName] = grid;
        }

        private bool IsDataLine(string? line)
        {
            if (string.IsNullOrWhiteSpace(line)) return false;
            
            // Simple check for numeric data
            string[] parts = line.Trim().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 && float.TryParse(parts[0], out _);
        }

        private float[]? ParseDataLine(string line)
        {
            try
            {
                string[] parts = line.Trim().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                var values = new float[parts.Length];
                
                for (int i = 0; i < parts.Length; i++)
                {
                    if (!float.TryParse(parts[i], out values[i]))
                        return null;
                }
                
                return values;
            }
            catch
            {
                return null;
            }
        }
    }
}