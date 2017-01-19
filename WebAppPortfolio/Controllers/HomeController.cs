using System.Diagnostics;
using System.Web.Mvc;
using WebAppPortfolio.Classes; 
using WebAppPortfolio.Interfaces;

namespace WebAppPortfolio.Controllers
{
    public class HomeController : Controller
    {
        #region Properties
        private EventLogger logger { get; set; }
        #endregion

        #region Public Actions
        /// <summary>
        /// Returns the main landing page: Views/Home/Index.cshtml
        /// Page is cached in browser for 5 mins
        /// </summary>
        #if (!DEBUG)
            [OutputCache(Duration = 300)]
        #endif
        public virtual ViewResult Index()
        {
            //logger = new EventLogger(); //unnecessary in Azure

            return View("Index"); //explicitly naming Index page only need for Unit Tests
        }

        /// <summary>
        /// Download link for resume PDF
        /// See https://msdn.microsoft.com/en-us/library/dd492593(v=vs.98).aspx
        /// Uses ServerPathProvider implementation of IPathProvider in RouteConfig.cs
        /// </summary>
        public FileResult Resume(IPathProvider pathProvider)
        {
            string fileName = "Aldrin F Abastillas - Resume 2016.pdf";
            string mapPath = pathProvider.MapPath("~/Content/documents/" + fileName);
            return File(mapPath, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);
        }

        /// <summary>
        /// Test writing to the error log by navigating to /Home/TestError
        /// </summary>
        public EmptyResult TestError()
        {
            Trace.TraceError("Testing Error Logger");
            return null;
        }
        #endregion

        #region Exception Handler
        /// <summary>
        /// Logs an exception to the error log
        /// </summary>
        /// <param name="exceptionContext"></param>
        protected override void OnException(ExceptionContext exceptionContext)
        {
            string message = (exceptionContext.Exception.Message != null) ? 
                exceptionContext.Exception.Message : "No exception message: " + this.ToString();
            Trace.TraceError("Error in HomeController: " + message);
        }
        #endregion
    }
}