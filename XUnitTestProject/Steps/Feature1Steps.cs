using Xunit;
using OpenQA.Selenium;
using System;
using TechTalk.SpecFlow;
using XUnitTestProject.Pages;
using OpenQA.Selenium.Support.PageObjects;

namespace XUnitTestProject.Features
{
    [Binding]
    public class Feature1Steps


    {

        readonly Feature1Page feature1page = new Feature1Page();

        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;


        public Feature1Steps(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
        }

        [Given(@"user is on (.*)")]
        public void GivenUserIsOnUrl(string URL)
        {
            feature1page.NavigateTo(URL);
            Console.WriteLine("user is on "+URL);
            TestRunner.Test.Pass(_scenarioContext.ScenarioInfo.Title);
        }
        
        [When(@"navigate menu (.*)")]
        public void WhenNavigateToTestShall(string path)
        {
            feature1page.NavigateMultipleMenuItems(path);
            Console.WriteLine("Step 2");
        }
        
        [Then(@"(.*) page should be loaded")]
        public void ThenPageShouldBeLoaded(string title)
        {
            feature1page.SwitchToNewWindow();
            feature1page.VerifyPage(title);
            Console.WriteLine("Loaded");
            Console.WriteLine("Step 3");
        }
    }
}
