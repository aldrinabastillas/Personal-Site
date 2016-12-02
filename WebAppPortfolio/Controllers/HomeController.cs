using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
        /// <returns></returns>
        [OutputCache(Duration = 300)]
        public virtual ActionResult Index()
        {
            //logger = new EventLogger();
            return View();
        }
        #endregion

        #region Exception Handler
        ///// <summary>
        ///// Logs an exception to the error log
        ///// </summary>
        ///// <param name="exceptionContext"></param>
        //protected override void OnException(ExceptionContext exceptionContext)
        //{
        //    if (logger == null)
        //    {
        //        logger = new EventLogger();
        //    }

        //    string message = (exceptionContext.Exception.Message != null) ? exceptionContext.Exception.Message : "No ex message" + this.ToString();
        //    logger.LogException("Error in HomeController: " + message);
        //}

        ///// <summary>
        ///// Test writing to the error log
        ///// </summary>
        ///// <returns></returns>
        //public EmptyResult TestError()
        //{
        //    var exception = new Exception("Testing Error Logger");
        //    OnException(new ExceptionContext(this.ControllerContext, exception));
        //    return null;
        //}
        #endregion
    }
}