using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CommonLibrary.Framework;
using System.Web;
using System.Net;
using System.Collections.ObjectModel;
using CommonLibrary.Framework.Caching;
using CommonLibrary.Framework.Tracing;

namespace CommonLibrary.Web.UI {
	public class Page : System.Web.UI.Page {

		#region Data Members
		private static bool _isTracingEnabled;
		private bool _isAdministrator;
		private bool _isMetricsEnabled;
		private bool _isRedirectionEnabled;
		private bool _isProtectedRenderingMode;
		private Hashtable _clientVariables;
		private Collection<string> _clientStartupScripts;
		private List<PageTraceLine> _traceLines;
		private eConnectionType _connectionType;
		public enum eConnectionType {
			Secure,
			Unsecure,
			NotSpecified
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets a value indicating what type of connection this page must use.  Default value is "Unsecure".
		/// "Secure" forces the connection to HTTPS, "Unsecure" forces HTTP.
		/// </summary>
		public eConnectionType ConnectionType {
			get { return _connectionType; }
			set { _connectionType = value; }
		}

		/// <summary>
		/// Gets a value indicating if the current session is an administrator session which allows for certain privileged operations.
		/// </summary>
		public bool IsAdministrator {
			get { return _isAdministrator; }
			protected set { _isAdministrator = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating if metrics should be gathered on this page.
		/// </summary>
		public bool IsMetricsEnabled {
			get { return (_isMetricsEnabled && this.Server.MachineName != ConfigurationBase.GetConfigString("LocalMachineName")); }
			protected set { _isMetricsEnabled = value; }
		}

		/// <summary>
		/// Gets a value indicating if tracing will be displayed on the bottom of the page.
		/// </summary>
		public bool IsTracingEnabled {
			get { return _isTracingEnabled; }
			protected set { _isTracingEnabled = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating if protective rendering should be used to mask the site.
		/// </summary>
		public bool IsProtectedRenderingMode {
			get { return _isProtectedRenderingMode; }
			protected set { _isProtectedRenderingMode = value; }
		}

		/// <summary>
		/// Gets a value indicating if the current user is authenticated on the site.
		/// </summary>
		public bool IsAuthenticated {
			get { return User.Identity.IsAuthenticated; }
		}

		/// <summary>
		/// Gets or sets a value indicating if the page will halt before redirecting to a destination, giving the testing user
		/// a chance to see where the redirect will go.
		/// </summary>
		public bool IsRedirectionEnabled {
			get { return _isRedirectionEnabled; }
			protected set { _isRedirectionEnabled = value; }
		}

		/// <summary>
		/// Gets or sets the list of trace lines written to the page.
		/// </summary>
		protected List<PageTraceLine> TraceLines {
			get { return _traceLines; }
			set { _traceLines = value; }
		}
		#endregion

		#region Constructors
		public Page() : base() {
			_traceLines = new List<PageTraceLine>();
			this.ConnectionType = eConnectionType.Unsecure;
			this.IsProtectedRenderingMode = false;
			this.IsTracingEnabled = false;
			this.IsRedirectionEnabled = true;
		}
		#endregion

		#region Methods
		public void AddCSS(string path, string media) {
			HtmlLink cssLink = new HtmlLink();

			cssLink.Href = path;
			cssLink.Attributes.Add("rel", "stylesheet");
			cssLink.Attributes.Add("type", "text/css");
			cssLink.Attributes.Add("media", media);

			Page.Header.Controls.Add(cssLink);
		}

		public bool AddClientVariable(string key, string value, bool useQuotes) {
			if (string.IsNullOrEmpty(key)) {
				throw new ArgumentNullException("key");
			} else if (string.IsNullOrEmpty(value)) {
				throw new ArgumentNullException("value");
			}

			value = value.Trim();
			if (_clientVariables == null) {
				_clientVariables = new Hashtable();
			}

			if (!_clientVariables.Contains(key)) {
				value = value.Replace("'", "\'");
				if (useQuotes) {
					value = string.Format("'{0}'", value);
				}

				_clientVariables.Add(key, value);
				return true;
			} else {
				return false;
			}
		}

		public void RegisterClientScriptInclude(string javscriptFile) {
			this.ClientScript.RegisterClientScriptInclude(javscriptFile, javscriptFile);
		}

		public bool RegisterStartupScript(string javascript) {
			if (string.IsNullOrEmpty(javascript)) {
				throw new ArgumentNullException("javascript");
			}

			if (_clientStartupScripts == null) {
				_clientStartupScripts = new Collection<string>();
			}

			_clientStartupScripts.Add(javascript);
			return true;
		}

		protected override void OnPreInit(EventArgs e) {
			base.OnPreInit(e);

			this.DoConnectionTypeRedirect();

			this.IsAdministrator = this.SetQueryStringValue("admin", ConfigurationBase.GetConfigString("AdminPassword"), false);
			this.IsMetricsEnabled = this.SetQueryStringValue("metrics", "y", true);

			if (this.IsAdministrator) {
				// NOTE: Anything that gets set in here must also be set in the constructor, especially items that by default are
				// supposed to be 'true'.  This is because these values will never be defaulted under non-IsAdministrator settings.
				this.IsProtectedRenderingMode = this.SetQueryStringValue("protect", "y", false);
				this.IsTracingEnabled = this.SetQueryStringValue("trace", "y", false);
				this.IsRedirectionEnabled = this.SetQueryStringValue("redirect", "y", true);

				string expCache = this.Request.QueryString["expireCache"];
				if (!string.IsNullOrEmpty(expCache) && expCache.ToLower() == "y") {
					TraceManager.Trace("Clearing the cache");
					CacheUtilities.ClearCaches();
				}
			}
		}

		protected override void OnPreRenderComplete(EventArgs e) {
			base.OnPreRenderComplete(e);

			if (_clientVariables != null && _clientVariables.Count > 0) {
				string fullString = string.Empty;
				foreach (DictionaryEntry entry in _clientVariables) {
					fullString = string.Concat(fullString, string.Format("var {0} = {1};\n", entry.Key, entry.Value));
				}

				this.ClientScript.RegisterClientScriptBlock(this.GetType(), "__clientVariables", fullString, true);
			}

			if (_clientStartupScripts != null) {
				StringBuilder sb = new StringBuilder();
				sb.Append("(function(){");
				//sb.Append("YAHOO.util.Event.onDOMReady(function(){");
				foreach (string script in _clientStartupScripts) {
					sb.Append(script);
				}
				//sb.Append("})");
				sb.Append("})();");
				this.Page.ClientScript.RegisterStartupScript(this.GetType(), "startupscripts", sb.ToString(), true);
			}
		}

		protected override void Render(HtmlTextWriter writer) {
			base.Render(writer);

			if (this.IsTracingEnabled && this.IsAdministrator) {
				writer.Write("<table cellpadding=\"3\" cellspacing=\"0\" border=\"0\" style=\"width: 100%; margin-top: 15px; background-color: white;\">");
				writer.Write("<tr><td colspan=\"3\" style=\"font-size: 14px; background-color: black; color: white; font-weight: bold;\">Tracing Information</td></tr>");

				writer.Write("<tr>");
				writer.Write("<td><b>Source</b></td>");
				writer.Write("<td><b>Category</b></td>");
				writer.Write("<td style=\"width: 100%;\"><b>Message</b></td>");
				writer.Write("</tr>");

				foreach (PageTraceLine line in this.TraceLines) {
					string style = string.Empty;
					string[] categories = new string[] { };
					Array.Resize<string>(ref categories, line.Categories.Count);
					line.Categories.CopyTo(categories, 0);

					//switch (Logger.GetEnum(categories[0])) {
					//    case Logger.eLogCategory.DataAccess:
					//        style = "background-color: navy; color: white;";
					//        break;
					//    case Logger.eLogCategory.Error:
					//        style = "background-color: red; color: white;";
					//        break;
					//    case Logger.eLogCategory.Caching:
					//        style = "background-color: green; color: white;";
					//        break;
					//    //case TraceLine.eTraceType.Important:
					//    //    style = "background-color: green; color: white;";
					//    //    break;
					//    //case TraceLine.eTraceType.Warning:
					//    //    style = "background-color: yellow; color: black;";
					//    //    break;
					//    default:
					//        style = "background-color: white; color: black;";
					//        break;
					//}

					writer.Write(string.Format("<tr style=\"{0}\">", style));
					writer.Write("<td>");
					if (!string.IsNullOrEmpty(line.SourceFile)) {
						string fileName = line.SourceFile;
						fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
						writer.Write(string.Format("<span title='{0}'>{1}:{2}</span>", line.SourceFile, fileName, (line.SourceLine > 0) ? line.SourceLine.ToString() : string.Empty));
					}
					writer.Write("</td>");
					writer.Write(string.Format("<td>{0}</td>", string.Join(", ", categories)));
					writer.Write(string.Format("<td>{0}</td>", line.Message));
					writer.Write("</tr>");
				}

				writer.Write("</table>");
			}
		}

		public void Redirect(string destination) {
			this.Redirect(destination, HttpStatusCode.Found);
		}

		public void Redirect(string destination, HttpStatusCode statusCode) {
			try {
				if (this.IsRedirectionEnabled) {
					this.Response.AddHeader("Location", destination);
					this.Response.StatusCode = (int)statusCode;
				} else {
					TraceManager.TraceFormat("Redirection disabled, would have redirected to {0}", destination);
					//RenderTraceOutput(Response.Output)
					this.Response.Output.Write(string.Format("<br /><a href=\"{0}\">Click here to follow the redirect to {0}</a>", destination));
				}

				this.Response.Flush();
				this.Response.End();
			} catch (System.Threading.ThreadAbortException) {
				// do nothing with this.
			}
		}

		internal void WriteTrace(string message, ICollection<string> categories, string sourceFile, int sourceLine) {
			PageTraceLine line = new PageTraceLine(message);
			line.SourceFile = sourceFile;
			line.SourceLine = sourceLine;
			line.Categories = categories;
			this.TraceLines.Add(line);
		}

		internal void WriteTrace(string message, ICollection<string> categories) {
			this.WriteTrace(message, categories, null, int.MinValue);
		}

		protected bool SetQueryStringValue(string key, string matchValue, bool defaultValue) {
			bool _return = defaultValue;
			string value = this.Request.QueryString[key];
			HttpCookie requestCookie = this.Request.Cookies[key];

			if (!string.IsNullOrEmpty(value)) {
				_return = (value.ToLower() == matchValue);

				// Set the cookie
				HttpCookie responseCookie = new HttpCookie(key, _return.ToString());
				this.Response.Cookies.Add(responseCookie);
			} else if (requestCookie != null) {
				_return = bool.Parse(requestCookie.Value);
			}

			return _return;
		}

		protected void DoConnectionTypeRedirect() {
			if (this.ConnectionType != eConnectionType.NotSpecified) {
				string protocol = string.Empty;

				if (this.ConnectionType == eConnectionType.Secure && !this.Request.IsSecureConnection) {
					protocol = "https";
				} else if (this.ConnectionType == eConnectionType.Unsecure && this.Request.IsSecureConnection) {
					protocol = "http";
				}

				if (!string.IsNullOrEmpty(protocol)) {
					string redirect = string.Format("{0}://{1}{2}", protocol, this.Request.ServerVariables["SERVER_NAME"], this.Request.Url.PathAndQuery);
					this.Redirect(redirect, HttpStatusCode.MovedPermanently);
				}
			}
		}
		#endregion

	}
}