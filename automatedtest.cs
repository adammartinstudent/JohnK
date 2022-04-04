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

    private static HashSet<string> visitedLinks = new HashSet<string>();
    private static Dictionary<string, string> visitedLinkTitles = new Dictionary<string, string>();
    private static Queue<string> unvisitedLinks = new Queue<string>();
    private static Regex guidRegex
        = new Regex(@"[0-9A-F]{8}-([0-9A-F]{4}-){3}[0-9A-F]{12}",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static string rootPage = "https://christophermartin77.wixsite.com/indian-lake-animal-s";

    private static Page currentPage = new Page(rootPage);

    private static IWebDriver newDriver()
    {
        FirefoxOptions options = new FirefoxOptions();
        options.AddArgument("--headless");
        string driverPath = @"G:\My Drive\CSharp\WebScraper\WebScraper\";
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
            if (unvisitedLinks.Peek().Contains("Account/SignOut")
                    && unvisitedLinks.Count > 1)
            {
                unvisitedLinks.Enqueue(unvisitedLinks.Dequeue());
                continue;
            }

            currentPage = new Page(unvisitedLinks.Dequeue());

            try
            {
                driver.Navigate().GoToUrl(currentPage.link);
                if (!driver.Url.Equals(currentPage.link))
                {
                    if (!driver.Url.StartsWith(rootPage))
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

            visitedLinks.Add(currentPage.link);
            Console.WriteLine($"Visited page: {currentPage.link}");
            
            htmlDoc.LoadHtml(driver.PageSource); // This takes a while, faster on better connections.

            if (driver.Title.Equals(""))
                visitedLinkTitles.Add(currentPage.link, currentPage.link);
            else
                visitedLinkTitles.Add(currentPage.link, driver.Title);
            
            if (currentPage.link.EndsWith("Site/SignIn"))
            {
                LogIn();
            }
        }

        driver.Quit();

        Console.WriteLine($"\n\nFound {visitedLinks.Count} total pages\n");
    }

    public static void LogIn()
    {
        driver.FindElement(By.Id("emailAddress")).SendKeys("arnavf3@vaticanakq.com");
        driver.FindElement(By.Id("password")).SendKeys("shelteradmin");
        driver.FindElement(By.Id("signInButton")).Click();

        Thread.Sleep(750);
        driver.FindElement(By.Id("siteList")).Click();
        Thread.Sleep(500);
        driver.FindElement(By.XPath("//*[@id='siteDialog']/div[1]/ul/div/li[5]")).Click();
        Thread.Sleep(500);
        driver.FindElement(By.Id("selectSite")).Click();
        Thread.Sleep(500);

        unvisitedLinks.Enqueue(driver.Url);
    }
}
