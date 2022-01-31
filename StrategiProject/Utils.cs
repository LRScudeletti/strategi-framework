using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using System.Net;
using System.Net.Mail;

namespace Selenium.Utils
{
    public class Utils
    {
        private static IConfiguration? _configuration;

        public static string GetAppSettingsParam(string paramName)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile($"appsettings.json");
            _configuration = builder.Build();

            return _configuration.GetSection(string.Concat("Selenium:", paramName)).Value;
        }

        public static List<object[]> GetBrowsers()
        {
            var listBrowsers = new List<object[]>();

            if (bool.Parse(GetAppSettingsParam("ChromeTest")))
                listBrowsers.Add(new object[] { Browsers.Chrome });

            if (bool.Parse(GetAppSettingsParam("EdgeTest")))
                listBrowsers.Add(new object[] { Browsers.Edge });

            if (bool.Parse(GetAppSettingsParam("FirefoxTest")))
                listBrowsers.Add(new object[] { Browsers.Firefox });

            if (bool.Parse(GetAppSettingsParam("SafariTest")))
                listBrowsers.Add(new object[] { Browsers.Safari });

            return listBrowsers;
        }

        public static void Print(string testName, bool printLog, string exception)
        {
            WebDriverFactory.GetWebDriver().Print(testName, printLog, exception);
        }

        public static void SendEMail(string subject, bool html, string message)
        {
            var smtpEMail = GetAppSettingsParam("SmtpEmail");

            MailMessage mailMessage = new()
            {
                From = new MailAddress(smtpEMail),
                Subject = string.Concat(subject, DateConverter(DateTime.Now)),
                IsBodyHtml = html,
                Body = message
            };

            using SmtpClient smtpClient = new(GetAppSettingsParam("SmtpHost"));

            var credentialPass = GetAppSettingsParam("SmtpPass");

            var listEmail = GetAppSettingsParam("ErrorNotificationEmail");

            foreach (var email in listEmail
                         .Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                mailMessage.To.Add(email);

            smtpClient.Credentials = new NetworkCredential(smtpEMail, credentialPass);
            smtpClient.Port = Convert.ToInt32(GetAppSettingsParam("SmtpPort"));
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void CreateLog(string testName, string exception)
        {
            List<LogEntry>? consoleBrowserLogs = WebDriverFactory.GetWebDriver()?.Manage()
                .Logs.GetLog(LogType.Browser).ToList();

            List<LogEntry>? driverBrowserLogs = WebDriverFactory.GetWebDriver()?.Manage()
                .Logs.GetLog(LogType.Driver).ToList();

            string? logs = null;

            if (consoleBrowserLogs != null)
                logs = string.Concat("Log Browser", Environment.NewLine, consoleBrowserLogs
                    .Aggregate("", (current, log) => current + (log + Environment.NewLine)));

            if (driverBrowserLogs != null)
                logs = string.Concat(logs, "Log Driver", Environment.NewLine, logs, driverBrowserLogs
                    .Aggregate("", (current, log) => current + (log + Environment.NewLine)));

            if (!string.IsNullOrEmpty(exception))
                logs = string.Concat(logs, "Log C#", Environment.NewLine, logs, exception);

            var basePath = GetAppSettingsParam("ErrorLogPath");

            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            var date = DateConverter(DateTime.Now);

            var pathLogs = string.Concat(basePath, "\\", testName, "-", date, ".txt");
            var logFileInfo = new FileInfo(pathLogs);

            var fileStream = logFileInfo.Create();
            var streamWriter = new StreamWriter(fileStream);
            streamWriter.WriteLine(logs);
            streamWriter.Close();
        }

        public static void CloseBrowser()
        {
            WebDriverFactory.GetWebDriver().CloseDriver();
        }

        public static string DateConverter(DateTime value)
        {
            return value.ToString("dd-MM-yyyy HH-mm-ss");
        }
    }
}
