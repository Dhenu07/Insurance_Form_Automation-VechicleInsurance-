using Microsoft.Playwright;
using StepDefinitions;

namespace Pages
{
    /// <summary>
    /// Page object model to fill vehicle data form fields.
    /// </summary>
    public class VehicleDataPage
    {
        private readonly IPage _page;
        private readonly Dictionary<string, string> _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleDataPage"/> class.
        /// </summary>
        /// <param name="page">Playwright page instance.</param>
        /// <param name="data">Dictionary containing vehicle data to fill.</param>
        public VehicleDataPage(IPage page, Dictionary<string, string> data)
        {
            _page = page;
            _data = data;
        }

        /// <summary>
        /// Fills the vehicle data form with provided data.
        /// </summary>
        public async Task FillAsync()
        {
            // Local helper method to check presence and fill/select fields
            async Task FillIfPresent(string key, string selector, bool isSelect = false, string? optionLabel = null)
            {
                if (InsuranceFormSteps.IsPresent(_data, key))
                {
                    if (isSelect)
                    {
                        if (optionLabel != null)
                        {
                            await _page.Locator(selector).SelectOptionAsync(new SelectOptionValue { Label = optionLabel });
                        }
                        else
                        {
                            await _page.SelectOptionAsync(selector, _data[key]);
                        }
                    }
                    else
                    {
                        await _page.FillAsync(selector, _data[key]);
                    }
                }
            }

            await FillIfPresent("Make", "#make", true);
            await FillIfPresent("Model", "#model", true);
            await FillIfPresent("CylinderCapacity", "#cylindercapacity");
            await FillIfPresent("EnginePerformance", "#engineperformance");
            await FillIfPresent("DateOfManufacture", "#dateofmanufacture");
            
            // Seats has two possible selectors combined, so handle explicitly
              if (InsuranceFormSteps.IsPresent(_data,"Seats"))
            {
                  await _page.Locator("#numberofseats, #numberofseatsmotorcycle")
                .SelectOptionAsync(new SelectOptionValue { Label = _data["Seats"] });
            }

            // RightHand is a radio button with Yes/No options
            if (InsuranceFormSteps.IsPresent(_data, "RightHand"))
            {
                string value = _data["RightHand"].Trim().ToLower() == "yes" ? "Yes" : "No";
                await _page.Locator($"//label[normalize-space()='{value}']").CheckAsync();
            }
            await FillIfPresent("Fuel", "#fuel", true);
            await FillIfPresent("Payload", "//input[@id='payload']");
            await FillIfPresent("Weight", "//input[@id='totalweight']");
            await FillIfPresent("ListPrice", "//input[@id='listprice']");
            await FillIfPresent("LicensePlateNumber", "//input[@id='licenseplatenumber']");
            await FillIfPresent("AnnualMileage", "//input[@id='annualmileage']");
        }
    }
}
