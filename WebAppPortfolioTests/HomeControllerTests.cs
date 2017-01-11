using System.Web.Mvc;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using WebAppPortfolio.Controllers;

namespace WebAppPortfolioTests
{
    [TestFixture]
    //[TestClass]
    public class HomeControllerTests
    {
        [Test]
        //[TestMethod]
        public void TestIndex()
        {
            var controller = new HomeController();
            var result = controller.Index() as ViewResult;
            Assert.AreEqual("Index", result.ViewName);
        }

        [Test]
        //[TestMethod]
        public void TestResume()
        {
            var controller = new HomeController();
                var file = controller.Resume() as FileResult;
                Assert.AreEqual("Aldrin F Abastillas - Resume 2016.pdf", file.FileDownloadName);
        }
    }
}
