using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using CommonLibrary.Web.UI.ResourceBundler;

namespace CommonLibrary.Web.UI.Validators {
	[System.Web.UI.ToolboxData(@"<{0}:RequiredFieldValidator runat='server' />")]
	public abstract class CommonValidatorBase : BaseValidator {

		#region Data Members
		private string _displayErrorCallbackFunction;
		public enum DisplayType {
			Fade,
			Dynamic,
			None,
			Static
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the client side callback method that will be fired when the validation fails.  The callback method
		/// should take in a single parameter which is the validation parameter.
		/// </summary>
		public string DisplayErrorCallbackFunction {
			get { return _displayErrorCallbackFunction; }
			set { _displayErrorCallbackFunction = value; }
		}
		#endregion

		#region Methods
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);

			if (this.RenderUplevel) {
				Bundler.RegisterFile("/Validators/ClientScripts/CommonValidatorBase.js", this.GetType().Assembly);
				Page.ClientScript.RegisterStartupScript(this.GetType(), "OverrideValidatorUpdateDisplay", "ASPValidatorUpdateDisplay=ValidatorUpdateDisplay;ValidatorUpdateDisplay=OverrideValidatorUpdateDisplay;", true);
			}
		}
		#endregion
					
	}
}