using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Framework.Objects {
	public class URLDetail : FileDetail {

		#region Data Members
		ParametersCollection _parameters = new ParametersCollection();
		#endregion

		#region Properties
		public ParametersCollection Parameters {
			get { return _parameters; }
			set { _parameters = value; }
		}
	
		/// <summary>
		/// Returns the full file path and parameters for an URL.  For instance, with the url /Some/URL/default.aspx?param=1, FullFilePath returns default.aspx
		/// </summary>
		public new string FileName {
			get {
				string[] parts = base.FileName.Split('?');
				return parts[0];
			}
			set { base.FileName = value; }
		}

		/// <summary>
		/// Returns the full file path and parameters for an URL.  For instance, with the url /Some/URL/default.aspx?param=1, FullFilePath returns /Some/URL/default.aspx?param=1
		/// </summary>
		public string FullFilePathWithParameters {
			get {
				string _return = base.FullFilePath;
				if (this.Parameters.Count > 0) {
					_return = string.Concat(_return, "?", this.Parameters.ToString());
				}

				return _return;
			}
		}

		/// <summary>
		/// Returns the full file path for an URL.  For instance, with the url /Some/URL/default.aspx?param=1, FullFilePath returns /Some/URL/default.aspx
		/// </summary>
		public new string FullFilePath {
			get {
				return base.FullFilePath;
			}
			set {
				this.Parameters = ParametersCollection.Parse(value);

				string _value = value;
				if (this.FullFilePath.Contains("?")) {
					_value = this.FullFilePath.Split('?')[0];
				}

				base.FullFilePath = _value;
			}
		}
	
		#endregion

		#region Constructors
		public URLDetail() : base(eSlashType.Forward) { }

		public URLDetail(string url) : base(url, eSlashType.Forward) {
			this.FullFilePath = url;	// For some reason, the base doesn't call into the correct prop, so we do it here.
		}
		#endregion

	}
}