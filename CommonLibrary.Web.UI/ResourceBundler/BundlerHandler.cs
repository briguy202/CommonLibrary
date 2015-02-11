using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using CommonLibrary.Framework.Caching;
using System.Collections.ObjectModel;
using CommonLibrary.Framework.Tracing;

namespace CommonLibrary.Web.UI.ResourceBundler {
	public class BundlerHandler : IHttpHandler {

		#region IHttpHandler Members
		public bool IsReusable {
			get { return true; }
		}

		public void ProcessRequest(HttpContext context) {
			string key = context.Request.QueryString["id"];
			string etag = context.Request.Headers["If-None-Match"];
			string modSince = context.Request.Headers["If-Modified-Since"];

			ICache cache = CacheFactory.GetCache("ResourceBundler");
			string resources = (string)cache[key];
			if (!string.IsNullOrEmpty(resources)) {
				if (key.StartsWith("JS.")) {
					context.Response.AddHeader("Content-Type", "text/javascript");
				} else if (key.StartsWith("CSS.")) {
					context.Response.AddHeader("Content-Type", "text/css");
				}
				context.Response.Write(resources);
			} else {
				context.Response.StatusCode = 500;
				TraceManager.Trace(string.Format("Unable to find resources with key value {0}", key));
				return;
			}
		}
		#endregion

	}
}