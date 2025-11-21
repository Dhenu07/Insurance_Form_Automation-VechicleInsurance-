using Hooks;
using Microsoft.Playwright;
using TechTalk.SpecFlow;
using System;
using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Hook to capture a screenshot after every step execution in a scenario.
/// Saves screenshots in a folder named 'Screenshots' in the project root.
/// </summary>
[Binding]
public class ScreenShotHook
{
    private IPage _page = BrowserHook.Page;

    /// <summary>
    /// Captures a full-page screenshot after each step in a scenario.
    /// </summary>
    [AfterStep]
    public async Task TakeScreenshotAfterScenario()
    {
        try
        {
            // Get scenario title and generate timestamped filename
            var scenarioTitle = ScenarioContext.Current.ScenarioInfo.Title;
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var screenshotFileName = $"{scenarioTitle}_{timestamp}.png".Replace(" ", "_");

            // Determine path to Screenshots directory
            string screenshotsDir = Path.Combine(
                Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName ?? string.Empty,
                "Screenshots");

            // Ensure the Screenshots directory exists
            if (!Directory.Exists(screenshotsDir))
            {
                Directory.CreateDirectory(screenshotsDir);
            }

            // Full path to the screenshot file
            var screenshotPath = Path.Combine(screenshotsDir, screenshotFileName);

            if (_page != null)
            {
                // Capture full-page screenshot
                await _page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = screenshotPath,
                    FullPage = true
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error capturing screenshot: {ex.Message}");
        }
    }
}
