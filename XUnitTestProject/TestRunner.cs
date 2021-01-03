using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using TechTalk.SpecFlow;
using AventStack.ExtentReports;
using XUnitTestProject.Utils;
/* For using Remote Selenium WebDriver */

// [assembly: CollectionBehavior(MaxParallelThreads = 4)]
//[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]

namespace XUnitTestProject
{
    [Binding]
    public sealed class TestRunner
    {
        
        private readonly ScenarioContext context;
        private readonly FeatureContext featureContext;
        private static AventStack.ExtentReports.ExtentReports _extent;
        [ThreadStatic]
        public static ExtentTest Test;

        public TestRunner(ScenarioContext injectedContext, FeatureContext fContext)
        {
            context = injectedContext;
            featureContext = fContext;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            //Setup Extent Reports
            _extent = ReportManager.Setup();
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            Test = _extent.CreateTest(context.ScenarioInfo.Title);
            Test.AssignCategory(featureContext.FeatureInfo.Title);
        }


        [AfterTestRun]
        public static void AfterTestRun()
        {
            //Flush the reports
            _extent.Flush();
        }
    }
}
