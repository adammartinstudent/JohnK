using System.Text.RegularExpressions;
using System.Collections;

using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

public class Pathfinder
{
    public class Page
    {
        public string title { get; set; }
        public string link { get; set; }

        public Page(string link)
        {
            this.link = link;
        }

        public Page()
        {
            this.link = "";
        }
    }

    private static HashSet<string> visitedLinks = new HashSet<string>(); // Visited websites
    // visitedLinkTitles stores page title and link, otherwise saves page title as the link
    private static Dictionary<string, string> visitedLinkTitles = new Dictionary<string, string>();
    private static Queue<string> unvisitedLinks = new Queue<string>(); // Websitse to be visisted
    private static Regex guidRegex
        = new Regex(@"[0-9A-F]{8}-([0-9A-F]{4}-){3}[0-9A-F]{12}",
                RegexOptions.Compiled | RegexOptions.IgnoreCase); // Filters GUID's from queried pages

    // Starting page
    private static string rootPage = "https://christophermartin77.wixsite.com/indian-lake-animal-s";

    private static Page currentPage = new Page(rootPage);

    private static IWebDriver newDriver() // Can optimize for other driver support
    {
        FirefoxOptions options = new FirefoxOptions();
        options.AddArgument("--headless");
        string driverPath = @"G:\My Drive\CSharp\WebScraper\WebScraper\"; // Need to update filepaths
        return new FirefoxDriver(driverPath, options);
    }
    private static IWebDriver driver = newDriver();

    private static HtmlDocument htmlDoc = new HtmlDocument();

    public static void Main(string[] args)
    {
        unvisitedLinks.Enqueue(rootPage);
        driver.Manage().Window.Maximize();

        while (unvisitedLinks.Count > 0)
        {
            currentPage = new Page(unvisitedLinks.Dequeue()); // Enters page at the top of the stack

            try
            {
                driver.Navigate().GoToUrl(currentPage.link);
                if (!driver.Url.Equals(currentPage.link))
                {
                    if (!driver.Url.StartsWith(rootPage)) // Ensures link isn't a redirect
                    {
                        continue;
                    }
                }
            }
            catch (OpenQA.Selenium.UnhandledAlertException e)
            {
                Console.WriteLine($"{e.Message} on page {currentPage.link}");
                continue;
            }
            catch (OpenQA.Selenium.WebDriverException e)
            {
                Console.WriteLine($"Exception {e.InnerException} on page {currentPage.link}.\nWaiting and pushing to back of queue.");
                continue;
            }

            visitedLinks.Add(currentPage.link); // Adds page to visitedLinks
            Console.WriteLine($"Visited page: {currentPage.link}");
            
            htmlDoc.LoadHtml(driver.PageSource); // This takes a while, faster on better connections.

            if (driver.Title.Equals("")) // Adds page to visitedLinkTitles
                visitedLinkTitles.Add(currentPage.link, currentPage.link);
            else
                visitedLinkTitles.Add(currentPage.link, driver.Title);
            
            if (currentPage.link.EndsWith("Site/SignIn")) // Redirect if login detected
            {
                LogIn();
            }
        }

        driver.Quit();

        Console.WriteLine($"\n\nFound {visitedLinks.Count} total pages\n");
    }

    // Needs work, unable to detect login element
    public static void LogIn()
    {
        driver.FindElement(By.Id("emailAddress")).SendKeys("arnavf3@vaticanakq.com");
        driver.FindElement(By.Id("password")).SendKeys("shelteradmin");
        driver.FindElement(By.Id("signInButton")).Click();

        unvisitedLinks.Enqueue(driver.Url);
    }
}
