using Microsoft.Playwright;
using StepDefinitions;
namespace Pages
{
    /// <summary>
    /// Represents the Insurant Data page and encapsulates methods to fill out the form.
    /// </summary>
    public class InsurantDataPage
    {
        private readonly IPage _page;
        private readonly Dictionary<string, string> _data;

        /// <summary>
        /// Constructor for the InsurantDataPage class.
        /// </summary>
        /// <param name="page">The Playwright page instance.</param>
        /// <param name="data">Dictionary containing the form data.</param>
        public InsurantDataPage(IPage page, Dictionary<string, string> data)
        {
            _page = page;
            _data = data;
        }

        /// <summary>
        /// Fills in the Insurant Data form based on provided input values.
        /// </summary>
        public async Task FillAsync()
        {
            // Helper to fill input fields if key present
            async Task FillIfPresent(string key, string selector)
            {
                if (InsuranceFormSteps.IsPresent(_data, key))
                {
                    await _page.FillAsync(selector, _data[key]);
                }
            }

            // Helper to select dropdown option if key present
            async Task SelectIfPresent(string key, string selector)
            {
                if (InsuranceFormSteps.IsPresent(_data, key))
                {
                    await _page.SelectOptionAsync(selector, _data[key]);
                }
            }

            // Fill text fields
            await FillIfPresent("FirstName", "#firstname");
            await FillIfPresent("LastName", "#lastname");
            await FillIfPresent("BirthDate", "#birthdate");
            await FillIfPresent("Street", "#streetaddress");
            await FillIfPresent("Zip", "#zipcode");
            await FillIfPresent("City", "#city");
            await FillIfPresent("Website", "#website");

            // Select Gender radio button
            if (InsuranceFormSteps.IsPresent(_data, "Gender"))
            {
                string value = _data["Gender"].Trim().ToLower() == "male" ? "Male" : "Female";
                await _page.Locator($"//label[normalize-space()='{value}']").CheckAsync();
            }

            // Select dropdowns
            await SelectIfPresent("Country", "#country");
            await SelectIfPresent("Occupation", "#occupation");

            // Check multiple hobbies checkboxes if present
            if (InsuranceFormSteps.IsPresent(_data, "Hobbies"))
            {
                string[] hobbies = _data["Hobbies"].Split(',');
                foreach (var hobby in hobbies)
                {
                    string trimmedHobby = hobby.Trim();
                    if (!string.IsNullOrEmpty(trimmedHobby))
                    {
                        await _page.Locator($"//label[normalize-space()='{trimmedHobby}']").CheckAsync();
                    }
                }
            }
        }
    }
}
