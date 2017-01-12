using System;
using System.Web.Mvc;
using NUnit.Framework;
using WebAppPortfolio.Controllers;

namespace WebAppPortfolioTests.ControllerTests
{
    [TestFixture]
    public class SpotifyControllerTests
    {
        [Test]
        public void TestIndex()
        {
            var controller = new SpotifyController(); //Arrange
            var result = controller.Index() as ViewResult; //Act
            Assert.AreEqual("Index", result.ViewName); //Verify
        }

        public void TestGetPrediction()
        {
            throw new NotImplementedException();
        }

        public void TestGetYearList()
        {
            throw new NotImplementedException();
        }

        public void TestCallMlService()
        {
            throw new NotImplementedException();
        }

    }
}
