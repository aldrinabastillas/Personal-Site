using System.Diagnostics;
using System.Web.Mvc;
using WebAppPortfolio.Classes; //to get the EventLogger class

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
        public virtual ActionResult Index()
        {
            //logger = new EventLogger(); //unnecessary in Azure
            return View();
        }

        /// <summary>
        /// Returns the Visualizer page: Views/Home/Visualizer.cshtml
        /// Page is cached in browser for 5 mins
        /// </summary>
        #if (!DEBUG)
            [OutputCache(Duration = 300)]
        #endif
        public ActionResult Visualizer()
        {
            return View("Visualizer");
        }

        /// <summary>
        /// Download link for resume PDF
        /// See https://msdn.microsoft.com/en-us/library/dd492593(v=vs.98).aspx
        /// </summary>
        public FileResult Resume()
        {
            string fileName = "Aldrin F Abastillas - Resume 2016.pdf";
            string mapPath = Server.MapPath("~/Content/documents/" + fileName);
            return File(mapPath, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);
        }

        /// <summary>
        /// Test writing to the error log
        /// </summary>
        public EmptyResult TestError()
        {
            //var exception = new Exception("Testing Error Logger");
            //OnException(new ExceptionContext(this.ControllerContext, exception));
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
            //if (logger == null)
            //{
            //    logger = new EventLogger();
            //}

            string message = (exceptionContext.Exception.Message != null) ? exceptionContext.Exception.Message : "No ex message" + this.ToString();
            //logger.LogException("Error in HomeController: " + message);
            Trace.TraceError("Error in HomeController: " + message);
        }
        #endregion
    }
}