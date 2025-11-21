using Hooks;
using Pages;
using TechTalk.SpecFlow;
using Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Playwright;
using AventStack.ExtentReports.Gherkin.Model;

namespace StepDefinitions
{
    [Binding]
    public class InsuranceFormSteps
    {
        private readonly Dictionary<string, string> _data;
        private readonly VehicleDataPage _vehicleDataPage;
        private readonly InsurantDataPage _insurantDataPage;
        private readonly ProductDataPage _productDataPage;
        private readonly PriceOptionPage _priceOptionPage;
        private readonly SendQuotePage _sendQuotePage;

        private readonly IPage page;
        public InsuranceFormSteps(ScenarioContext scenarioContext)
        {
            _data = TestDataHelper.LoadTestData();
            page = Hooks.BrowserHook.Page;
            _vehicleDataPage = new VehicleDataPage(page, _data);
            _insurantDataPage = new InsurantDataPage(page, _data);
            _productDataPage = new ProductDataPage(page, _data);
            _priceOptionPage = new PriceOptionPage(page, _data);
            _sendQuotePage = new SendQuotePage(page, _data);
        }
        //  <----Scenario 1---->
        [Given(@"the browser is launched")]
        public async Task GivenTheBrowserIsLaunched()
        {
            await LaunchTheSite(_data["VehicleType"]);
            Console.WriteLine($"User is on the {_data["VehicleType"]} vehicle data page.");
        }

        [When(@"the user fills the vehicle data form")]
        public async Task WhenTheUserFillsTheVehicleDataForm()
        {
            await _vehicleDataPage.FillAsync();
            await ButtonClick("#nextenterinsurantdata");
        }

        [When(@"the user fills the insurant data form")]
        public async Task WhenTheUserFillsTheInsurantDataForm()
        {
            await _insurantDataPage.FillAsync();
            await ButtonClick("#nextenterproductdata");
        }

        [When(@"the user fills the product data form")]
        public async Task WhenTheUserFillsTheProductDataForm()
        {
            await _productDataPage.FillAsync();
            await ButtonClick("#nextselectpriceoption");
        }

        [When(@"the user selects a price option")]
        public async Task WhenTheUserSelectsAPriceOption()
        {
            await _priceOptionPage.SelectAsync();
            await ButtonClick("#nextsendquote");
        }

        [When(@"the user fills and submits the quote")]
        public async Task WhenTheUserFillsAndSubmitsTheQuote()
        {
            await _sendQuotePage.FillAndSubmitAsync();
        }

        [Then(@"the form should be submitted successfully")]
        public async Task ThenTheFormShouldBeSubmittedSuccessfully()
        {
            var confirmationText = await Hooks.BrowserHook.Page.Locator("div.sweet-alert").TextContentAsync();
            Assert.IsTrue(confirmationText.Contains("Sending e-mail success!"));
            await Hooks.BrowserHook.Page.Locator("//button[normalize-space()='OK']").ClickAsync();

        }
        // <----Scenario 2----->
        [Given(@"the user is on the ""(.*)"" vehicle data page")]
        public async Task Giventheuserisonthevehicledatapage(string vehicleType)
        {
            await LaunchTheSite(vehicleType);
        }

        [When(@"the user enters data like ""(.*)"" , ""(.*)"" , ""(.*)"" ,""(.*)"" , ""(.*)""")]
        public async Task Whentheuserentersdatalike(string enginePerformance, string dateOfManufacture, string listPrice, string licensePlateNo, string annualMileage)
        {
            Dictionary<string, string> myList = new Dictionary<string, string>();
            myList.Add("EnginePerformance", enginePerformance);
            myList.Add("DateOfManufacture", dateOfManufacture);
            myList.Add("ListPrice", listPrice);
            myList.Add("LicensePlateNumber", licensePlateNo);
            myList.Add("AnnualMileage", annualMileage);
            VehicleDataPage vehicle = new VehicleDataPage(page, myList);
            await vehicle.FillAsync();
        }

        [Then(@"the form should show errors in the respective fields")]
        public async Task Thentheformshouldshowerrorsintherespectivefields()
        {
            await ValidateError("#engineperformance", "//*[@id='insurance-form']/div/section[1]/div[2]/span", "Must be a number between 1 and 2000");
            await ValidateError("#dateofmanufacture", "//*[@id='insurance-form']/div/section[1]/div[3]/span", "Must be today or somewhere in the past");
            await ValidateError("#listprice", "//*[@id='insurance-form']/div/section[1]/div[6]/span", "Must be a number between 500 and 100000");
            await ValidateError("#licenseplatenumber", "//*[@id='insurance-form']/div/section[1]/div[7]/span", "Must be under 10 characters");
            await ValidateError("#annualmileage", "//*[@id='insurance-form']/div/section[1]/div[8]/span", "Must be a number between 100 and 100000");

        }
        // <----Scenario 3----->
        [Given(@"the user is on the ""(.*)"" insurant data page")]
        public async Task Giventheuserisontheinsurantdatapage(string vehicleType)
        {
            await LaunchTheSite(vehicleType);
            await ButtonClick("#enterinsurantdata");
        }

        [When(@"the user enter the personal data like ""(.*)"" , ""(.*)"" , ""(.*)"" , ""(.*)"" , ""(.*)""")]
        public async Task Whentheuserenterthepersonaldatalike(string firstName, string lastName, string dob, string zip, string website)
        {
            Dictionary<string, string> myList = new Dictionary<string, string>();
            myList.Add("FirstName", firstName);
            myList.Add("LastName", lastName);
            myList.Add("BirthDate", dob);
            myList.Add("Zip", zip);
            myList.Add("Website", website);
            InsurantDataPage insurant = new InsurantDataPage(page, myList);
            await insurant.FillAsync();
        }


        [Then(@"the form should show errors in the fields")]
        public async Task Thentheformshouldshowerrorsinthefields()
        {
            await ValidateError("#firstname", "//*[@id='insurance-form']/div/section[2]/div[1]/span", "Must be at least 2 characters long and must only contain letters");
            await ValidateError("#lastname", "//*[@id='insurance-form']/div/section[2]/div[2]/span", "Must be at least 2 characters long and must only contain letters");
            await ValidateError("#birthdate", "//*[@id='insurance-form']/div/section[2]/div[3]/span", "You must be between 18 and 70 years of age");
            await ValidateError("#zipcode", "//*[@id='insurance-form']/div/section[2]/div[7]/span", "Must be a number between 4 and 8 digits,Must be only digits");
            await ValidateError("#website", "//*[@id='insurance-form']/div/section[2]/div[11]/span", "Must be a valid URL");
        }
        // <---Scenario 4---->
        [When(@"the user fills data forms and selects premimum")]
        public async Task Whentheuserfillsdataformsandselectspremimum()
        {
            VehicleDataPage vehicleData = new VehicleDataPage(page, _data);
            await vehicleData.FillAsync();
            await ButtonClick("#nextenterinsurantdata");
            InsurantDataPage insurant = new InsurantDataPage(page, _data);
            await insurant.FillAsync();
            await ButtonClick("#nextenterproductdata");
            ProductDataPage product = new ProductDataPage(page, _data);
            await product.FillAsync();
            await ButtonClick("#nextselectpriceoption");
            // PriceOptionPage price = new PriceOptionPage(page, _data);
            // await price.SelectAsync();

        }

        [When(@"the user clicks the view quote link")]
        public async Task WhenTheUserClicksTheViewQuoteLink()
        {
            await page.Locator("//a[@id='viewquote']//span[@class='hb hb-sm inv spin-icon hb-file-pdf-o-inv']").ClickAsync();
            // await page.Locator("//*[@id='LoadingPDF']").ClickAsync();
            await ButtonClick("#LoadingPDF");
            await page.WaitForTimeoutAsync(20000);
        }

        [Then(@"the quote should open in a new browser tab")]
        public async Task Thenthequoteshouldopeninanewbrowsertab()
        {
            List<String> app = await BrowserHook.GetPages();
            Console.WriteLine("PDF Generated");
        }

        // <---Scenario 5---->
        [When(@"the user fills all forms up to price option")]
        public async Task Whentheuserfillsallformsuptopriceoption()
        {
            await Whentheuserfillsdataformsandselectspremimum();
        }

        [When(@"tries to proceed without selecting a price option")]
        public async Task Whentriestoproceedwithoutselectingapriceoption()
        {
            await ButtonClick("#nextsendquote");
        }

        [Then(@"the form should display a message to select a price option")]
        public async Task Thentheformshoulddisplayamessagetoselectapriceoption()
        {
            Assert.AreEqual(await page.Locator("div#xLoaderQuote p").InnerTextAsync(), "Please, select a price option to send the quote.");
            Console.WriteLine("Message to select the price option is displayed");
        }
        //<---Scenario 6--->
        [When(@"the user directly navigates to the select price option step")]
        public async Task Whentheuserdirectlynavigatestotheselectpriceoptionstep()
        {
           await ButtonClick("#selectpriceoption");
        }

        [Then(@"the application should show a message indicating incomplete steps")]
        public async Task Thentheapplicationshouldshowamessageindicatingincompletesteps()
        {
            Assert.AreEqual(await page.Locator("div#xLoaderPrice p").InnerTextAsync(), "Please, complete the first three steps to see the price table.");
            Console.WriteLine("Message to fill the form is displayed");
        }

        // Extra Written Methods
        public async Task LaunchTheSite(string vehicleType)
        {

            await Hooks.BrowserHook.Page.GotoAsync("https://sampleapp.tricentis.com/101/app.php");
            string selector = vehicleType.ToLower() switch
            {
                "automobile" => "#nav_automobile",
                "truck" => "#nav_truck",
                "motorcycle" => "#nav_motorcycle",
                "camper" => "#nav_camper",
                _ => throw new ArgumentException("Invalid vehicle type")
            };
            await ButtonClick(selector);
        }
        public static bool IsPresent(Dictionary<string, string> data, string key)
        {
            if (data.ContainsKey(key) && !string.IsNullOrEmpty(data[key]) && data[key].ToLower() != "null")
            {
                return true;
            }
            return false;
        }
        public async Task ValidateError(string id, string errorId, string message)
        {
            await page.Locator(id).FocusAsync();
            var errorElement = page.Locator(errorId);
            if (await errorElement.IsVisibleAsync())
            {
                var errorText = await errorElement.InnerTextAsync();
                if (id == "#zipcode")
                {
                    string[] data = message.Split(",");
                    Console.WriteLine(data[0]);
                    Console.WriteLine(data[1]);
                    if (errorText.ToString().Equals(data[0]))
                    {
                        message = data[0];
                    }
                    else
                    {
                        message = data[1];
                    }
                }
                Console.WriteLine("Error Message: " + errorText);
                Assert.AreEqual(message, errorText);
            }
            else
            {
                Console.WriteLine("No validation error message is visible.");
            }

        }
        public async Task ButtonClick(String id)
        {
            await page.ClickAsync(id);
        }
    }
}
