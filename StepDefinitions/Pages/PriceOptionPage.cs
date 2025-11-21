using System.Configuration;
using Microsoft.Playwright;

namespace Pages
{
    /// <summary>
    /// Handles the Price Option page interactions.
    /// </summary>
    public class PriceOptionPage
    {
        private readonly IPage _page;
        private readonly Dictionary<string, string> _data;

        /// <summary>
        /// Constructor for the PriceOptionPage.
        /// </summary>
        /// <param name="page">Playwright IPage instance.</param>
        /// <param name="data">Dictionary containing test data, including PriceOption.</param>
        public PriceOptionPage(IPage page, Dictionary<string, string> data)
        {
            _page = page;
            _data = data;
        }

        /// <summary>
        /// Selects the price option radio button based on the test data.
        /// </summary>
        public async Task SelectAsync()
        {
            // Map of price options to their corresponding radio button index
            var options = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "silver", 2 },
                { "gold", 3 },
                { "platinum", 4 },
                { "ultimate", 5 }
            };

            var selectedOption = _data["PriceOption"].Trim().ToLowerInvariant();

            if (options.ContainsKey(selectedOption))
            {
                int index = options[selectedOption];

                // Select the corresponding radio button using XPath index
                await _page.Locator($"(//span[@class='ideal-radio'])[{index}]").CheckAsync();

                Console.WriteLine($"Selected Price Option: {selectedOption} (Index: {index})");
            }
            else
            {
                Console.WriteLine($"Error: Invalid PriceOption '{_data["PriceOption"]}' specified.");
                // Optionally throw exception or handle gracefully
            }
        }
    }
}
