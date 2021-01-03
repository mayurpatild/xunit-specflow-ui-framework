using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XUnitTestProject.Utils
{
    class ReportManager
    {
        private static AventStack.ExtentReports.ExtentReports _extent;
        private static ExtentHtmlReporter _htmlReporter;
        public static AventStack.ExtentReports.ExtentReports Setup()
        {
            string css = "img.r-img {width: 50% ;}";
            var config = Directory.GetCurrentDirectory();
            Directory.CreateDirectory(config).CreateSubdirectory("Reports");
            _htmlReporter = new ExtentHtmlReporter(Path.Combine(config, "Reports", @"Report.html"));
            _htmlReporter.Config.CSS = css;
            _htmlReporter.Config.DocumentTitle = "Test Automation Reports";
            _htmlReporter.Config.ReportName = "Test Smoke Reports";
            _htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Dark;
            _extent = new AventStack.ExtentReports.ExtentReports();
            _extent.AnalysisStrategy = AnalysisStrategy.Test;
            _extent.AddSystemInfo("os", "windows");
            _extent.AttachReporter(_htmlReporter);
            return _extent;
        }
    }
}
