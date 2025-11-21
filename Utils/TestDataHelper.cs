using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Utils
{
    public static class TestDataHelper
    {
        private static int _currentIndex = 0; // Static index to track progress across calls
        private static readonly object _lock = new(); // Thread-safety lock

        /// <summary>
        /// Loads a different set of test data on each call from the 'appData.json' file.
        /// Returns a Dictionary representing a single test data object.
        /// </summary>
        public static Dictionary<string, string> LoadTestData()
        {
            const string fileName = "appData.json";

            try
            {
                // Read JSON content
                var json = File.ReadAllText(fileName);
                using var doc = JsonDocument.Parse(json);

                // Ensure TestData is an array
                if (!doc.RootElement.TryGetProperty("TestData", out JsonElement testDataArray) || testDataArray.ValueKind != JsonValueKind.Array)
                {
                    throw new JsonException($"The JSON file '{fileName}' must contain a 'TestData' array.");
                }

                var dataArray = testDataArray.EnumerateArray().ToList();

                if (dataArray.Count == 0)
                    throw new JsonException("The 'TestData' array is empty.");

                JsonElement selectedData;

                lock (_lock)
                {
                    // Circular rotation logic
                    selectedData = dataArray[_currentIndex % dataArray.Count];
                    _currentIndex++; // Move to next for future calls
                }

                // Convert selected JSON object to dictionary
                if (selectedData.ValueKind != JsonValueKind.Object)
                    throw new JsonException("Each entry in 'TestData' must be a JSON object.");

                return selectedData.EnumerateObject()
                    .ToDictionary(p => p.Name, p => p.Value.ValueKind == JsonValueKind.String ? p.Value.GetString() ?? "" : p.Value.ToString() ?? "");
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException($"Could not find the test data file '{fileName}'.", ex);
            }
            catch (JsonException ex)
            {
                throw new JsonException($"Error parsing the test data JSON file '{fileName}': {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred while loading test data from '{fileName}': {ex.Message}", ex);
            }
        }
    }
}
