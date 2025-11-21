using Microsoft.Playwright;
using StepDefinitions;

namespace Pages
{
    /// <summary>
    /// Page object model to handle filling and submitting the Send Quote form.
    /// </summary>
    public class SendQuotePage
    {
        private readonly IPage _page;
        private readonly Dictionary<string, string> _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendQuotePage"/> class.
        /// </summary>
        /// <param name="page">Playwright page instance.</param>
        /// <param name="data">Dictionary containing form data.</param>
        public SendQuotePage(IPage page, Dictionary<string, string> data)
        {
            _page = page;
            _data = data;
        }

        /// <summary>
        /// Fills the send quote form with data and submits it.
        /// </summary>
        public async Task FillAndSubmitAsync()
        {
            // Helper local method to check presence and fill fields
            async Task FillIfPresent(string key, string selector)
            {
                if (InsuranceFormSteps.IsPresent(_data, key))
                {
                    await _page.FillAsync(selector, _data[key]);
                }
            }
            await FillIfPresent("Email", "#email");
            await FillIfPresent("Phone", "#phone");
            await FillIfPresent("Username", "#username");
            await FillIfPresent("Password", "#password");
            await FillIfPresent("ConfirmPassword", "#confirmpassword");
            await FillIfPresent("Comments", "#Comments");

            // Submit the form
            await _page.ClickAsync("#sendemail");
        }
    }
}
