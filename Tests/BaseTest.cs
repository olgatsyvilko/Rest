using Allure.NUnit;
using Allure.NUnit.Attributes;
using NLog;
using NUnit.Framework;

namespace Rest.Tests
{
    [AllureNUnit]
    public class BaseTest
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [SetUp]
        [AllureBefore]
        public void BeforeMethod()
        {
            Logger.Debug($"*** Test '{TestContext.CurrentContext.Test.Name}' is started ***");
        }

        [TearDown]
        [AllureAfter]
        public void AfterMethod()
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            Logger.Debug($"*** Test '{TestContext.CurrentContext.Test.Name}' is finished with '{status}' status.***");
        }
    }
}
