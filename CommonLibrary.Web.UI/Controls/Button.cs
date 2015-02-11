using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Web.UI.ResourceBundler;

namespace CommonLibrary.Web.UI.Controls {
	public class Button : System.Web.UI.WebControls.Button {

		#region Data Members
		private string _clientScriptID;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the id by which this element can be accessed through JavaScript.
		/// </summary>
		public string ClientScriptID {
			get {
				if (string.IsNullOrEmpty(_clientScriptID)) {
					_clientScriptID = string.Format("btn_{0}", this.ClientID);
				}
				return _clientScriptID;
			}
			set { _clientScriptID = value; }
		}
		#endregion

		#region Constructors
		public Button() : base() { }
		#endregion

		#region Methods
		/// <summary>
		/// Gets the class that will be added to the button.  Override to apply a custom styling to the button.
		/// </summary>
		/// <returns></returns>
		protected virtual string GetAdditionalCSSClass() {
			return string.Empty;
		}

		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			Bundler.RegisterFile("/Controls/CSS/button.css");
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			string addedClass = this.GetAdditionalCSSClass();
			if (!string.IsNullOrEmpty(addedClass)) {
				addedClass = string.Format(" {0}", addedClass);
			}
			writer.WriteLine("<span id=\"{0}\" class=\"cbtn{1}\">", this.ClientScriptID, addedClass);
			writer.WriteLine("<span class=\"first-child\">");
			base.Render(writer);
			writer.WriteLine("</span>");
			writer.WriteLine("</span>");
		}
		#endregion
					
	}
}