using System.Web.Mvc;
using NUnit.Framework;
using WebAppPortfolio.Controllers;
using WebAppPortfolioTests.MockClasses;

namespace WebAppPortfolioTests.ControllerTests
{
    [TestFixture]
    public class HomeControllerTests
    {
        [Test]
        public void TestIndex()
        {
            var controller = new HomeController(); //Arrange
            var result = controller.Index() as ViewResult; //Act
            Assert.AreEqual("Index", result.ViewName); //Verify
        }

        [Test]
        public void TestResume()
        {
            var controller = new HomeController(); //Arrange
            var file = controller.Resume(new TestPathProvider()) as FileResult; //Act
            Assert.AreEqual("Aldrin F Abastillas - Resume 2016.pdf", file.FileDownloadName); //Verify
        }
    }
}
