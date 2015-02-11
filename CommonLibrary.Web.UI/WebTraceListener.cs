using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Diagnostics;

namespace CommonLibrary.Web.UI {
	[ConfigurationElementType(typeof(CustomTraceListenerData))]
	public class WebTraceListener : CustomTraceListener {

		#region Constructors
		public WebTraceListener() : base() { }
		#endregion

		#region Methods
		public void Write(LogEntry entry) {
			if (System.Web.HttpContext.Current.Handler is Page) {
				Page page = (Page)System.Web.HttpContext.Current.Handler;
				if (entry.ExtendedProperties.ContainsKey("sourceStackFrame")) {
					StackFrame frame = (StackFrame)entry.ExtendedProperties["sourceStackFrame"];
					page.WriteTrace(entry.Message, entry.Categories, frame.GetFileName(), frame.GetFileLineNumber());
				} else {
					page.WriteTrace(entry.Message, entry.Categories);
				}
			}
		}

		public override void Write(string message) {
			if (System.Web.HttpContext.Current.Handler is Page) {
				Page page = (Page)System.Web.HttpContext.Current.Handler;
				page.WriteTrace(message, new string[] { });
			}
		}

		public override void WriteLine(string message) {
			this.Write(message);
		}

		public override void TraceData(System.Diagnostics.TraceEventCache eventCache, string source, System.Diagnostics.TraceEventType eventType, int id, object data) {
			if (data is LogEntry) {
				this.Write((LogEntry)data);
			}
		}
		#endregion
		
	}
}