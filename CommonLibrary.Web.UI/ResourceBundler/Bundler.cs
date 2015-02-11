using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Collections.ObjectModel;
using System.Reflection;
using CommonLibrary.Framework.Caching;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Runtime.CompilerServices;
using CommonLibrary.Framework.Tracing;

namespace CommonLibrary.Web.UI.ResourceBundler {
	public static class Bundler {

		#region Data Members
		static ICache _cache;
		public const string SCRIPT_FILES_KEY = "scriptfiles";
		public const string SCRIPT_FILES_WRITTEN = "scriptfileswritten";
		#endregion

		#region Properties
		#endregion

		#region Constructors
		static Bundler() {
			_cache = CacheFactory.GetCache("ResourceBundler");
		}
		#endregion

		#region Methods
		[MethodImpl(MethodImplOptions.NoInlining)]	// Inlining screws with the GetCallingAssembly method call
		public static bool RegisterFile(string file) {
			return Bundler.RegisterFile(file, Assembly.GetCallingAssembly(), false);
		}

		public static bool RegisterFile(string file, Assembly assembly) {
			return Bundler.RegisterFile(file, assembly, false);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]	// Inlining screws with the GetCallingAssembly method call
		public static bool RegisterFile(string file, bool placeInHead) {
			return Bundler.RegisterFile(file, Assembly.GetCallingAssembly(), placeInHead);
		}

		public static bool RegisterFile(string file, Assembly assembly, bool placeInHead) {
			if (Bundler.ResourcesAreWritten()) {
				TraceManager.Trace(string.Format("Request to add file {0} is denied because resources have already been written to the page.", file), FrameworkTraceTypes.Error);
				return false;
			}

			ResourceBundle bundle = Bundler.GetResourceBundle(file, assembly, placeInHead);
			if (bundle != null) {
				Collection<string> resourceCollection = Bundler.GetResourceCollection(file, placeInHead, bundle);
				return bundle.AddResource(file, assembly, resourceCollection);
			} else {
				return false;
			}
		}

		public static bool UnRegisterFile(string file) {
			return Bundler.UnRegisterFile(file, Assembly.GetCallingAssembly(), false);
		}

		public static bool UnRegisterFile(string file, bool placeInHead) {
			return Bundler.UnRegisterFile(file, Assembly.GetCallingAssembly(), placeInHead);
		}

		public static bool UnRegisterFile(string file, Assembly assembly, bool placeInHead) {
			ResourceBundle bundle = Bundler.GetResourceBundle(file, assembly, placeInHead);
			if (bundle != null) {
				Collection<string> resourceCollection = Bundler.GetResourceCollection(file, placeInHead, bundle);
				return bundle.RemoveResource(file, assembly, resourceCollection);
			} else {
				return false;
			}
		}

		public static bool IsResourceRegistered(string file) {
			return Bundler.IsResourceRegistered(file, Assembly.GetCallingAssembly(), false);
		}

		public static bool IsResourceRegistered(string file, bool placeInHead) {
			return Bundler.IsResourceRegistered(file, Assembly.GetCallingAssembly(), placeInHead);
		}

		public static bool IsResourceRegistered(string file, Assembly assembly, bool placeInHead) {
			ResourceBundle bundle = Bundler.GetResourceBundle(file, assembly, placeInHead);
			if (bundle != null) {
				Collection<string> resourceCollection = Bundler.GetResourceCollection(file, placeInHead, bundle);
				return bundle.IsResourceRegistered(file, assembly, resourceCollection);
			} else {
				return false;
			}
		}

		private static void page_PreRenderComplete(object sender, EventArgs e) {
			System.Web.UI.Page page = (System.Web.UI.Page)HttpContext.Current.Handler;
			ResourceBundle resources = (ResourceBundle)page.Items[SCRIPT_FILES_KEY];
			page.Items[SCRIPT_FILES_WRITTEN] = true;

			if (resources.JavascriptResources.Count > 0) {
				Bundler.LogResources(resources.JavascriptResources, "Adding {0} Javascript body resources.");
				page.ClientScript.RegisterClientScriptInclude(SCRIPT_FILES_KEY, string.Format("/ResourceBundle.axd?id={0}", ResourceBundle.GetHashKey(resources.JavascriptResources)));
			}

			if (resources.HeaderJavascriptResources.Count > 0) {
				Bundler.LogResources(resources.HeaderJavascriptResources, "Adding {0} Javascript header resources.");
				HtmlGenericControl scriptControl = new HtmlGenericControl("script");
				scriptControl.Attributes.Add("type", "text/javascript");
				scriptControl.Attributes.Add("src", string.Format("/ResourceBundle.axd?id={0}", ResourceBundle.GetHashKey(resources.HeaderJavascriptResources)));
				page.Header.Controls.Add(scriptControl);
			}

			if (resources.CSSResources.Count > 0) {
				Bundler.LogResources(resources.CSSResources, "Adding {0} CSS resources.");
				HtmlLink cssLink = new HtmlLink();
				cssLink.Href = string.Format("/ResourceBundle.axd?id={0}", ResourceBundle.GetHashKey(resources.CSSResources));
				cssLink.Attributes.Add("rel", "stylesheet");
				cssLink.Attributes.Add("type", "text/css");
				cssLink.Attributes.Add("media", "all");
				page.Header.Controls.Add(cssLink);
			}

			Bundler.AddToCache(resources.CSSResources);
			Bundler.AddToCache(resources.JavascriptResources);
			Bundler.AddToCache(resources.HeaderJavascriptResources);
		}

		private static void AddToCache(Collection<string> resourceList) {
			if (resourceList.Count == 0) { return; }
			string key = ResourceBundle.GetHashKey(resourceList);
			System.Web.UI.Page page = (System.Web.UI.Page)HttpContext.Current.Handler;

			if (_cache[key] == null) {
				StringBuilder sb = new StringBuilder();
				foreach (string script in resourceList) {
					string[] parts = script.Split(':');
					string assemblyName = parts[0];
					string resourceName = parts[1].TrimStart('.');
					string fullName = string.Concat(assemblyName, ".", resourceName);

					Assembly assembly = Assembly.Load(assemblyName);
					Stream stream = assembly.GetManifestResourceStream(fullName);
					if (stream == null) {
						throw new ApplicationException(string.Format("Unable to locate resource {0} within assembly {1}.", fullName, assemblyName));
					}
					StreamReader reader = new StreamReader(stream);
					string contents = reader.ReadToEnd();

					// Manually perform the substitution of embedded web resources within an embedded web resource.
					int i = contents.IndexOf("<%=");
					while (i > -1) {
						int blockStart = i;
						int blockEnd = contents.IndexOf("%>", blockStart + 1) + 2;
						int start = contents.IndexOf("\"");
						int end = contents.IndexOf("\"", start+1);
						string[] pieces = contents.Substring(start + 1, (end - start) - 1).Split(':');
						string resource = pieces[0];
						string typeName = pieces[1];

						contents = contents.Replace(contents.Substring(blockStart, (blockEnd - blockStart)), page.ClientScript.GetWebResourceUrl(assembly.GetType(typeName), resource));
						
						i = contents.IndexOf("<%=", i);
					}

					sb.AppendLine(string.Empty);
					sb.AppendLine("/**********************************************************************/");
					sb.AppendLine(string.Format("/* Bundler file {0} */", resourceName));
					sb.Append(contents);
				}
				_cache.Add(key, sb.ToString());
			}
		}

		public static Collection<string> GetResourceCollection(string file, bool placeInHead, ResourceBundle bundle) {
			Collection<string> collection;
			if (file.EndsWith(".js")) {
				collection = (placeInHead) ? bundle.HeaderJavascriptResources : bundle.JavascriptResources;
			} else {
				collection = bundle.CSSResources;
			}
			return collection;
		}

		public static ResourceBundle GetResourceBundle(string file, Assembly assembly, bool placeInHead) {
			if (string.IsNullOrEmpty(file)) { throw new ArgumentNullException("file"); }
			if (assembly == null) { throw new ArgumentNullException("assembly"); }

			if (HttpContext.Current.Handler is System.Web.UI.Page) {
				System.Web.UI.Page page = (System.Web.UI.Page)HttpContext.Current.Handler;
				if (page.Items[SCRIPT_FILES_KEY] == null) {
					page.Items[SCRIPT_FILES_KEY] = new ResourceBundle();
					page.PreRenderComplete += new EventHandler(page_PreRenderComplete);
				}

				return (ResourceBundle)page.Items[SCRIPT_FILES_KEY];
			}

			return null;
		}

		public static bool ResourcesAreWritten() {
			System.Web.UI.Page page = (System.Web.UI.Page)HttpContext.Current.Handler;
			if (page.Items[SCRIPT_FILES_WRITTEN] == null) {
				page.Items[SCRIPT_FILES_WRITTEN] = false;
			}
			return (bool)page.Items[SCRIPT_FILES_WRITTEN];
		}

		private static void LogResources(Collection<string> resourceList, string format) {
			TraceManager.Trace(string.Format(format, resourceList.Count.ToString()));
			foreach (string value in resourceList) {
				TraceManager.Trace(value);
			}
		}
		#endregion
		
	}
}