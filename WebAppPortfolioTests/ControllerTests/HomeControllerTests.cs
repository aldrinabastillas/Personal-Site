using System.Web.Mvc;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using WebAppPortfolio.Controllers;
using WebAppPortfolioTests.Classes;

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
            var controller = new HomeController(); //Arrange
            var result = controller.Index() as ViewResult; //Act
            Assert.AreEqual("Index", result.ViewName); //Verify
        }

        [Test]
        //[TestMethod]
        public void TestResume()
        {
            var controller = new HomeController(); //Arrange
            var file = controller.Resume(new TestPathProvider()) as FileResult; //Act
            Assert.AreEqual("Aldrin F Abastillas - Resume 2016.pdf", file.FileDownloadName); //Verify
        }
    }
}
