using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Playwright;
using TechTalk.SpecFlow;

namespace Hooks
{
    /// <summary>
    /// Provides setup and teardown hooks for browser automation using Playwright.
    /// </summary>
    [Binding]
    public sealed class BrowserHook
    {
        /// <summary>
        /// Shared page instance for test scenarios.
        /// </summary>
        public static IPage Page;

        /// <summary>
        /// Browser instance.
        /// </summary>
        private static IBrowser _browser;

        /// <summary>
        /// Playwright instance.
        /// </summary>
        private static IPlaywright _playwright;

        /// <summary>
        /// Browser context for isolated sessions.
        /// </summary>
        public static IBrowserContext Context;

        /// <summary>
        /// Initializes the Playwright instance, browser, and page before any tests run.
        /// </summary>
        [BeforeTestRun]
        public static async Task Initialize()
        {
            try
            {
                _playwright = await Playwright.CreateAsync();
                _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = false, // Launch with GUI
                    Channel = "chrome", // Use Chrome browser
                });
                Context = await _browser.NewContextAsync();
                Page = await Context.NewPageAsync();

                Console.WriteLine("Browser has been launched.");
            }
            catch (PlaywrightException pwEx)
            {
                Console.WriteLine("Playwright error during initialization: " + pwEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error during initialization: " + ex.Message);
            }
        }

        /// <summary>
        /// Cleans up browser and Playwright resources after all tests run.
        /// </summary>
        [AfterTestRun]
        public static async Task Cleanup()
        {
            try
            {
                if (Page != null)
                    await Page.CloseAsync();

                if (_browser != null)
                    await _browser.CloseAsync();

                _playwright?.Dispose();

                Console.WriteLine("Browser and Playwright resources cleaned up.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during cleanup: " + ex.Message);
            }
        }

        /// <summary>
        /// Retrieves the titles of all open pages in the current browser context.
        /// </summary>
        /// <returns>List of page titles.</returns>
        public static async Task<List<string>> GetPages()
        {
            var titles = new List<string>();

            try
            {
                var pages = Context.Pages;

                foreach (var p in pages)
                {
                    titles.Add(await p.TitleAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving page titles: " + ex.Message);
            }

            return titles;
        }
    }
}
