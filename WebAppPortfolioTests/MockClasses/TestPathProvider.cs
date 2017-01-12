using System.IO;
using WebAppPortfolio.Interfaces;

namespace WebAppPortfolioTests.MockClasses
{
    public class TestPathProvider : IPathProvider
    {
        /// <summary>
        /// Mocks the Server.MapPath() static method
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string MapPath(string path)
        {
            return Path.Combine(@"C:\Users\Aldrin\Documents\Visual Studio 2015\Projects\WebAppPortfolio\WebAppPortfolio", path);
        }
    }
}
