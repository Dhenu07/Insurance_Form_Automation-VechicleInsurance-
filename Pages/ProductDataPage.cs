using Microsoft.Playwright;
using StepDefinitions;

namespace Pages
{
    /// <summary>
    /// Page object model for filling the Product Data section of the insurance form.
    /// </summary>
    public class ProductDataPage
    {
        private readonly IPage _page;
        private readonly Dictionary<string, string> _data;

        /// <summary>
        /// Constructor to initialize ProductDataPage.
        /// </summary>
        /// <param name="page">The Playwright page instance.</param>
        /// <param name="data">Dictionary containing form field values.</param>
        public ProductDataPage(IPage page, Dictionary<string, string> data)
        {
            _page = page;
            _data = data;
        }

        /// <summary>
        /// Fills the Product Data form based on the values in _data dictionary.
        /// </summary>
        public async Task FillAsync()
        {
            // Local helper method for select option fields
            async Task SelectIfPresent(string key, string selector)
            {
                if (InsuranceFormSteps.IsPresent(_data, key))
                {
                    await _page.SelectOptionAsync(selector, _data[key]);
                }
            }

            // Local helper method for fill input fields
            async Task FillIfPresent(string key, string selector)
            {
                if (InsuranceFormSteps.IsPresent(_data, key))
                {
                    await _page.FillAsync(selector, _data[key]);
                }
            }

            await FillIfPresent("StartDate", "#startdate");
            await SelectIfPresent("InsuranceSum", "#insurancesum");
            await SelectIfPresent("MeritRating", "#meritrating");
            await SelectIfPresent("DamageInsurance", "#damageinsurance");

            if (InsuranceFormSteps.IsPresent(_data, "OptionalProduct"))
            {
                string[] optionalProducts = _data["OptionalProduct"].Split(',');
                foreach (string product in optionalProducts)
                {
                    string trimmedProduct = product.Trim();
                    if (!string.IsNullOrEmpty(trimmedProduct))
                    {
                        await _page.Locator($"//label[normalize-space()='{trimmedProduct}']").CheckAsync();
                    }
                }
            }

            await SelectIfPresent("CourtesyCar", "#courtesycar");
        }
    }
}
