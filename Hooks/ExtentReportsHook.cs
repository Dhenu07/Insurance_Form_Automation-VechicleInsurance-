using System;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Threading.Tasks;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
using TechTalk.SpecFlow;

/// <summary>
/// Hooks class to initialize, update, and flush ExtentReports.
/// Includes step logging and screenshot zipping functionality.
/// </summary>
[Binding]
public class ExtentReportsHooks
{
    private static ExtentReports extent;
    private static ExtentTest feature;
    private static ExtentTest scenario;

    // Report output folder path
    private static readonly string reportPath = Path.Combine(
        Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName ?? string.Empty,
        "ExtentReports");

    // Full path to the generated report file
    private static readonly string reportFile = Path.Combine(reportPath, "ExtentReports.html");

    /// <summary>
    /// Initializes the HTML Extent Report before any tests run.
    /// </summary>
    [BeforeTestRun]
    public static void InitializeReports()
    {
        try
        {
            if (!Directory.Exists(reportPath))
            {
                Directory.CreateDirectory(reportPath);
            }

            var htmlReporter = new ExtentSparkReporter(reportFile);
            extent = new ExtentReports();
            extent.AttachReporter(htmlReporter);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error initializing ExtentReports: " + ex.Message);
        }
    }

    /// <summary>
    /// Flushes the Extent Report and zips the Screenshots folder after all tests run.
    /// </summary>
    [AfterTestRun]
    public static void FlushReport()
    {
        try
        {
            extent.Flush();

            // Open the report in default browser
            var reportFullPath = Path.GetFullPath(reportFile);
            Process.Start(new ProcessStartInfo
            {
                FileName = reportFullPath,
                UseShellExecute = true
            });

            // Zip the Screenshots folder if it exists
            string screenshotsFolder = Path.Combine(
                Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName ?? string.Empty,
                "Screenshots");

            string zipPath = Path.Combine(
                Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName ?? string.Empty,
                "Screenshots.zip");

            if (Directory.Exists(screenshotsFolder))
            {
                if (File.Exists(zipPath)) File.Delete(zipPath);
                ZipFile.CreateFromDirectory(screenshotsFolder, zipPath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error during report flush or screenshot zipping: " + ex.Message);
        }
    }

    /// <summary>
    /// Logs the feature in the report before executing it.
    /// </summary>
    /// <param name="featureContext">Current feature context</param>
    [BeforeFeature]
    public static void BeforeFeature(FeatureContext featureContext)
    {
        try
        {
            feature = extent.CreateTest<Feature>(featureContext.FeatureInfo.Title);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error creating feature node: " + ex.Message);
        }
    }

    /// <summary>
    /// Logs the scenario in the report before executing it.
    /// </summary>
    /// <param name="scenarioContext">Current scenario context</param>
    [BeforeScenario]
    public static void BeforeScenario(ScenarioContext scenarioContext)
    {
        try
        {
            scenario = feature.CreateNode<Scenario>(scenarioContext.ScenarioInfo.Title);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error creating scenario node: " + ex.Message);
        }
    }

    /// <summary>
    /// Logs each step's result (pass/fail) after it is executed.
    /// </summary>
    /// <param name="scenarioContext">Scenario execution context</param>
    [AfterStep]
    public static void AfterStep(ScenarioContext scenarioContext)
    {
        try
        {
            var stepType = ScenarioStepContext.Current.StepInfo.StepDefinitionType.ToString();
            var stepInfo = ScenarioStepContext.Current.StepInfo.Text;

            if (scenario != null)
            {
                if (scenarioContext.TestError == null)
                {
                    switch (stepType)
                    {
                        case "Given": scenario.CreateNode<Given>(stepInfo); break;
                        case "When": scenario.CreateNode<When>(stepInfo); break;
                        case "Then": scenario.CreateNode<Then>(stepInfo); break;
                        case "And": scenario.CreateNode<And>(stepInfo); break;
                    }
                }
                else
                {
                    var errorMessage = scenarioContext.TestError.Message;
                    switch (stepType)
                    {
                        case "Given": scenario.CreateNode<Given>(stepInfo).Fail(errorMessage); break;
                        case "When": scenario.CreateNode<When>(stepInfo).Fail(errorMessage); break;
                        case "Then": scenario.CreateNode<Then>(stepInfo).Fail(errorMessage); break;
                        case "And": scenario.CreateNode<And>(stepInfo).Fail(errorMessage); break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error logging step: " + ex.Message);
        }
    }
}
