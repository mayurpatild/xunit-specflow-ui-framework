﻿using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.IdentityModel.Tokens;
using Ninject;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;

namespace XUnitTestProject.Pages
{
    class BasePage : IDisposable
    {
        public IWebDriver driver;

      
        private IWebDriver SetupWebDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-notifications");
            return new ChromeDriver(options);
        }

       
        public IWebDriver StartDriver(BrowserType browserType)
        {
            switch (browserType)
            {
                case BrowserType.Chrome:
                    driver = new ChromeDriver();
                    break;
                case BrowserType.Firefox:
                    driver = new FirefoxDriver();
                    break;
                default:
                    //throw new ArgumentOutOfRangeException("browserType");
                    driver = new ChromeDriver();
                    break;
            }
            driver.Manage().Window.Maximize();

            return driver;
        }

        public void quitDriver()
        {
            driver.Close();
            driver.Quit();
        }

        
        
        public BasePage()
        {
            if (driver != null)
            {
                driver.Dispose();
                driver = null;
            }
            else
            {
                StartDriver(BrowserType.Chrome);               
            }
        }


        public bool IsAt(string pageTitle)
        {
            return Title.Contains(pageTitle);
        }

        public WebDriverWait Wait()
        {
            return new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        }


        public string Title
        {
            get { return driver.Title; }
        }


        /// <summary>
        /// Checks if the current page contains the specified keyword.
        /// </summary>
        /// <param name="keyWord">The keyword to be checked for.</param>
        /// <returns>Returns true, if word is found and false if not.</returns>
        public bool CheckPage(string keyWord)
        {
            return driver.PageSource.IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) >= 0;
        }


        /// <summary>
        /// Enters text into an element
        /// </summary>
        /// <param name="inputText">The text to input</param>
        /// <param name="locator">The element to enter text into</param>
        public void Type(string inputText, By locator)
        {
            Find(locator).SendKeys(inputText);
        }

        /// <summary>
        /// Waits  for an element to be visible, then returns it
        /// </summary>
        /// <param name="locator">The element to find</param>
        /// <returns>WebDriver IWebElement</returns>
        public IWebElement Find(By locator)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            wait.Until(ExpectedConditions.ElementIsVisible(locator));
            return driver.FindElement(locator);
        }

        public IWebElement WaitForElement(By locator)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            wait.Until(VisibilityOfElementLocated(locator));
            return driver.FindElement(locator);
        }

        private Func<IWebDriver, bool> VisibilityOfElementLocated(By by)
        {
            return driver =>
            {
                try
                {
                    Thread.Sleep(500);
                }
                catch (ThreadInterruptedException e)
                {
                    Console.WriteLine(e.Message);
                }
                IWebElement element = Find(by);
                return element.Displayed;
            };
        }

        public void SwitchToNewWindow()
        {
            List<string> allWindows = driver.WindowHandles.ToList();
            int windowCount = allWindows.Count;
            if (windowCount > 1)
            {
                driver.SwitchTo().Window(driver.WindowHandles[windowCount - 1]);
            }
            Console.WriteLine("Switched to Window : " + driver.CurrentWindowHandle + " - Title is - " + driver.Title);
        }
        public void VerifyPage(string pageTitle)
        {
            if (!pageTitle.Equals(driver.Title))
            {
                throw new InvalidOperationException("This page is not " + pageTitle + ". The title is: " + driver.Title);
            }
        }



        /// <summary>
        /// Finds multiple web elements matching the given locator.
        /// </summary>
        /// <param name="locator">param used to identify the elements.</param>
        /// <returns>Returns a collection of elements that match the given locator.</returns>
        public System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> FindElements(By locator)
        {

            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
                wait.Until(ExpectedConditions.ElementIsVisible(locator));
                return driver.FindElements(locator);
            }
            catch (NoSuchElementException e1)
            {
                Console.WriteLine(GetType().Name + " could not find the element. " + locator + e1.Message);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(GetType().Name + " encountered an error. " + e.Message);
                throw;
            }
        }


        /// <summary>
        /// Opens the BaseUrl appended with the page url and verifies the 
        /// page title is as expected.
        /// </summary>
        /// <param name="url"></param>
        public void NavigateTo(string url, string pageTitle)
        {
            driver.Navigate().GoToUrl(url);
            VerifyPage(pageTitle);
        }


        /// <summary>
        /// Opens the BaseUrl appended with the page url and verifies the 
        /// page title is as expected.
        /// </summary>
        /// <param name="url"></param>
        public void NavigateTo(string url)
        {
            driver.Navigate().GoToUrl(url);
        }

        /// <summary>
        /// Provide Menu items in click sequence delimited by ">"
        /// </summary>
        public void NavigateMultipleMenuItems(string pathDelimited)
        {
            string[] steps = pathDelimited.Split('>');
            foreach (string step in steps)
            {
                Find(By.XPath("//a[text()='" + step.Trim() + "']")).Click();
            }
        }

        /// <summary>
        /// Left clicks an element
        /// </summary>
        /// <param name="locator">The element to click</param>
        public void Click(By locator)
        {
            Find(locator).Click();
        }


        /// <summary>
        /// Left clicks an element
        /// </summary>
        /// <param name="locator">The element to click</param>
        public void Clear(By locator)
        {
            Find(locator).Clear();
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        public static String GetTimestamp()
        {
            return new DateTime().ToString("yyyyMMddHHmmssffff");
        }
       

        public string TakeScreenshot()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", "");
            string path = dir + "Screenshot\\" + GetTimestamp() + ".png";
            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            screenshot.SaveAsFile(path, ScreenshotImageFormat.Png);
            return path;
        }


        /// <summary>
        /// gets the inner HTML test of an element
        /// </summary>
        /// <param name="locator">The element which contains the text</param>
        /// <returns></returns>
        public string GetText(By locator)
        {
            return Find(locator).Text;
        }
        /// <summary>
        /// Validate if Element present in provided list of Elements
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="expectedText"></param>
        /// <returns></returns>
        public bool IsElementPresent(By locator, string expectedText)
        {

            IList<IWebElement> subelements = driver.FindElements(locator);
            for (int i = 0; i < subelements.Count; i++)
            {

                //Console.Write(subelements[i].Text);

                if (subelements[i].Text == expectedText)
                {
                    Console.Write("Element Found " + subelements[i].Text);
                    return true;

                }

            }
            return false;
        }


        /// <summary>
        /// Checks whether an element is displayed and enabled
        /// </summary>
        /// <param name="locator">The element to be checked</param>
        /// <returns>True if the element is visible and enabled</returns>
        public bool IsDisplayed(By locator)
        {
            try
            {
                IWebElement element = Find(locator);
                return element.Displayed && element.Enabled;
            }
            catch (NoSuchElementException ex)
            {
                return false;
            }
        }


        /// <summary>
        /// Submits a form
        /// </summary>
        /// <param name="locator"></param>
        public void Submit(By locator)
        {
            Find(locator).Submit();
        }

        /// <summary>
        /// Returns the destination URL of hyperlinks
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>the contents of the href attribute (for hyperlinks and images)</returns>
        public string GetLinkDestination(By locator)
        {
            return Find(locator).GetAttribute("href");
        }

        /// <summary>
        /// Checks a checkbox or radio button. Performs no action if the element is already checked or selected
        /// </summary>
        /// <param name="locator">The element to be checked</param>
        /// <returns></returns>
        public void Check(By locator)
        {
            IWebElement element = Find(locator);
            if (!element.Selected)
            {
                element.Click();
            }
        }

        /// <summary>
        /// Unchecks a checkbox or radio button. Performs no action if the element is already unchecked or selected
        /// </summary>
        /// <param name="locator">The element to be unchecked</param>
        /// <returns></returns>
        public void Uncheck(By locator)
        {
            IWebElement element = Find(locator);
            if (element.Selected)
            {
                element.Click();
            }
        }

        /// <summary>
        /// Verifies if a checkbox is checked
        /// </summary>
        /// <param name="locator">The element to verify</param>
        /// <returns>True if checkbox is selected otherwise returns False</returns>
        public bool IsSelected(By locator)
        {
            return Find(locator).Selected;
        }


        /// <summary>
        /// Find the element by locator and element text
        /// </summary>
        /// <param name="locator">The element to find</param>
        /// <param name="elementText">elemtnt text</param>
        /// <returns>return the element</returns>
        public IWebElement FindElementByText(By locator, string elementText)
        {
            var elements = FindElements(locator);
            return elements.FirstOrDefault(y => elements.Any(x => x.Text.ToUpperInvariant().Contains(elementText.ToUpperInvariant())));

        }

        public void ClickOnSubMenuByText(By mainMenuLocator, By subMenuLocator, string subMenuText)
        {
            var builder = new Actions(driver);
            var mainMenu = Find(mainMenuLocator);
            builder.MoveToElement(mainMenu).Build().Perform();
            //Thread.Sleep(500); //add a wait
            var sunMenu = FindElementByText(subMenuLocator, subMenuText);
            builder.MoveToElement(sunMenu).Build().Perform();
            sunMenu.Click();
        }

        public void ClickOnMenu(By menuLocator)
        {
            //Somehow driver varible is not have right value check later
            var builder = new Actions(driver);
            var mainMenu = Find(menuLocator);
            builder.MoveToElement(mainMenu).Build().Perform();
            mainMenu.Click();
        }


        public void ClickAction(By locator)
        {
            Actions actions = new Actions(driver);

            actions.Click(Find(locator)).Perform();


        }

        public void Dispose()
        {
            driver.Dispose();
        }

        ~BasePage()
        {
            driver.Quit();
            driver.Dispose();
        }
    }
}
