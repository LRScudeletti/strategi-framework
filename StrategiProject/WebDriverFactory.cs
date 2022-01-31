using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Safari;

namespace Selenium.Utils
{
    public static class WebDriverFactory
    {
        public static IWebDriver? WebDriver { get; set; }

        public static IWebDriver CreateWebDriver(Browsers browsers)
        {
            WebDriver = null;

            var browsersPath = Utils.GetAppSettingsParam("BrowsersPath");

            switch (browsers)
            {
                case Browsers.Chrome:
                    ChromeOptions chromeOptions = new();
                    chromeOptions.SetLoggingPreference(LogType.Browser, LogLevel.All);
                    WebDriver = new ChromeDriver(browsersPath, chromeOptions);
                    break;
                case Browsers.Firefox:
                    FirefoxOptions firefoxOptions = new();
                    firefoxOptions.SetLoggingPreference(LogType.Browser, LogLevel.All);
                    WebDriver = new FirefoxDriver(browsersPath, firefoxOptions);
                    break;
                case Browsers.Edge:
                    EdgeOptions edgeOptions = new();
                    edgeOptions.SetLoggingPreference(LogType.Browser, LogLevel.All);
                    WebDriver = new EdgeDriver(browsersPath, edgeOptions);
                    break;
                case Browsers.Safari:
                    SafariOptions safariOptions = new();
                    safariOptions.SetLoggingPreference(LogType.Browser, LogLevel.All);
                    WebDriver = new SafariDriver(browsersPath, safariOptions);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(browsers), browsers, null);
            }
            return WebDriver;
        }

        public static IWebDriver? GetWebDriver()
        {
            return WebDriver;
        }
    }
}