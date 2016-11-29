using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Reflection;
using System.Web.Mvc;

namespace WebAppPortfolio.Classes
{
    public class EventLogger
    {
        #region Properties
        private int eventId {get; set;}
        #endregion

        #region Fields
        private string appName = Assembly.GetExecutingAssembly().FullName;
        private EventLog log; 
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the event source and event log
        /// </summary>
        public EventLogger()
        {
            if (!EventLog.SourceExists(appName))
            {
                EventLog.CreateEventSource(appName, "EventLogs");
            }
            log = new EventLog();
            eventId = 0;
            log.Source = appName;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Logs an error event to the event log with the specified message
        /// </summary>
        /// <param name="message"></param>
        public void LogException(string message)
        {
            log.WriteEntry(message, EventLogEntryType.Error, eventId);
            eventId++;
        }

        /// <summary>
        /// Logs a warning event to the event log with the specified message
        /// </summary>
        /// <param name="message"></param>
        public void LogWarning(string message)
        {
            log.WriteEntry(message, EventLogEntryType.Warning, eventId);
            eventId++;
        }
        #endregion

    }
}