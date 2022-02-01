using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Utils
{
    public static class WebDriverExtensions
    {
        public static void LoadPage(this IWebDriver webDriver, string url, int waitingTime = 0)
        {
            webDriver.Manage().Timeouts().PageLoad = TimeSpan.FromMilliseconds(waitingTime);
            webDriver.Navigate().GoToUrl(url);
            webDriver.Manage().Window.Maximize();
        }

        public static IWebElement GetElement(this IWebDriver webDriver, By by, int waitingTime = 0)
        {
            var wait = new WebDriverWait(webDriver, TimeSpan.FromMilliseconds(waitingTime));
            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));
        }

        public static IWebElement GetShadowRootElement(this IWebDriver webDriver, By by, int waitingTime = 0)
        {
            if (waitingTime > 0)
                Thread.Sleep(waitingTime);

            return (IWebElement)webDriver.FindElement(@by).GetShadowRoot();
        }

        public static bool CheckVisible(this IWebDriver webDriver, By by, int waitingTime = 0)
        {
            var wait = new WebDriverWait(webDriver, TimeSpan.FromMilliseconds(waitingTime));
            var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));

            return element.Displayed;
        }

        public static bool CheckEnabled(this IWebDriver webDriver, By by, int waitingTime = 0)
        {
            var wait = new WebDriverWait(webDriver, TimeSpan.FromMilliseconds(waitingTime));
            var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));

            return element.Enabled;
        }

        public static void SetText(this IWebDriver webDriver, By by, string text)
        {
            var webElement = webDriver.FindElement(by);
            webElement.SendKeys(text);
        }

        public static string? GetText(this IWebDriver webDriver, By by)
        {
            var webElement = webDriver.FindElement(by);
            return webElement.Text;
        }

        public static void Click(this IWebDriver? webDriver, By by, int waitingTime = 0)
        {
            if (waitingTime > 0)
                Thread.Sleep(waitingTime);

            var webElement = webDriver!.FindElement(by);
            webElement.Click();
        }

        public static void Print(this IWebDriver? webDriver, string testName, bool printLog, string exception)
        {
            var basePath = Utils.GetAppSettingsParam("ErrorScreenshotPath");
            var date = Utils.DateConverter(DateTime.Now);

            var screenshotsErrorPath = string.Concat(basePath, "\\", testName, "-", date, ".png");

            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            ((ITakesScreenshot)webDriver!).GetScreenshot()
                .SaveAsFile(screenshotsErrorPath, ScreenshotImageFormat.Png);

            if (printLog)
                Utils.CreateLog(testName, exception);
        }

        public static void CloseDriver(this IWebDriver? webDriver)
        {
            webDriver?.Close();
            webDriver?.Quit();
        }

        public static bool CheckText(this IWebDriver? webDriver, string text, int waitingTime = 0)
        {
            if (waitingTime > 0)
                Thread.Sleep(waitingTime);

            return webDriver!.PageSource.Contains(text);
        }
    }
}