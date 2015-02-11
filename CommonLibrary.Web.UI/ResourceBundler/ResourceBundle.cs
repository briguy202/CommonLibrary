using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Reflection;

namespace CommonLibrary.Web.UI.ResourceBundler {
	public class ResourceBundle {

		#region Data Members
		private Collection<string> _javascriptResources;
		private Collection<string> _headerJavascriptResources;
		private Collection<string> _cssResources;
		#endregion

		#region Properties
		public Collection<string> CSSResources {
			get { return _cssResources; }
			set { _cssResources = value; }
		}

		public Collection<string> JavascriptResources {
			get { return _javascriptResources; }
			set { _javascriptResources = value; }
		}

		public Collection<string> HeaderJavascriptResources {
			get { return _headerJavascriptResources; }
			set { _headerJavascriptResources = value; }
		}
		#endregion

		#region Constructors
		public ResourceBundle() {
			this.CSSResources = new Collection<string>();
			this.JavascriptResources = new Collection<string>();
			this.HeaderJavascriptResources = new Collection<string>();
		}
		#endregion

		#region Methods
		public bool IsResourceRegistered(string file, Assembly assembly, Collection<string> collection) {
			if (string.IsNullOrEmpty(file)) { throw new ArgumentNullException("file"); }
			if (assembly == null) { throw new ArgumentNullException("assembly"); }

			string _file = file.Replace('/', '.');
			_file = string.Format("{0}:{1}", ResourceBundle.GetAssemblyName(assembly), _file);
			return collection.Contains(_file);
		}

		public bool AddResource(string file, Assembly assembly, Collection<string> collection) {
			if (string.IsNullOrEmpty(file)) { throw new ArgumentNullException("file"); }
			if (assembly == null) { throw new ArgumentNullException("assembly"); }

			string _file = file.Replace('/', '.');
			_file = string.Format("{0}:{1}", ResourceBundle.GetAssemblyName(assembly), _file);

			if (!collection.Contains(_file)) {
				collection.Add(_file);
				return true;
			}

			return false;
		}

		public bool RemoveResource(string file, Assembly assembly, Collection<string> collection) {
			if (string.IsNullOrEmpty(file)) { throw new ArgumentNullException("file"); }
			if (assembly == null) { throw new ArgumentNullException("assembly"); }

			string _file = file.Replace('/', '.');
			_file = string.Format("{0}:{1}", ResourceBundle.GetAssemblyName(assembly), _file);
			if (collection.Contains(_file)) {
				collection.Remove(_file);
				return true;
			}

			return false;
		}

		public static string GetHashKey(Collection<string> resources) {
			if (resources == null) { throw new ArgumentNullException("resources"); }

			if (resources.Count == 0) {
				return string.Empty;
			} else {
				StringBuilder sb = new StringBuilder();
				foreach (string resource in resources) {
					sb.Append(resource);
				}
				//TODO: append a build ID to the end

				string hash = sb.ToString().GetHashCode().ToString();
				hash = string.Format("{0}.{1}", (resources[0].EndsWith(".js")) ? "JS" : "CSS", hash);
				
				return hash;
			}
		}

		public static string GetCollectionValue(Collection<string> collection) {
			StringBuilder sb = new StringBuilder();
			if (collection != null) {
				bool first = true;
				foreach (string value in collection) {
					if (!first) {
						sb.Append(", ");
					}
					first = false;
					sb.Append(value);
				}
			}
			return sb.ToString();
		}

		private static string GetAssemblyName(Assembly assembly) {
			return assembly.FullName.Substring(0, assembly.FullName.IndexOf(','));
		}
		#endregion
		
	}
}