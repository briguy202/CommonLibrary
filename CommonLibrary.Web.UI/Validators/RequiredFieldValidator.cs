using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using CommonLibrary.Web.UI.YUI;
using CommonLibrary.Web.UI.ResourceBundler;

namespace CommonLibrary.Web.UI.Validators {
	[System.Web.UI.ToolboxData(@"<{0}:RequiredFieldValidator runat='server' />")]
	public class RequiredFieldValidator : CommonValidatorBase {

		#region Data Members
		private string _initialValue;
		private CommonValidatorBase.DisplayType _display;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the value that should not exist in the input field.  If not set, an empty string is used.
		/// </summary>
		public string InitialValue {
			get { return _initialValue; }
			set { _initialValue = value; }
		}

		/// <summary>
		/// Gets or sets the display type for the validation message.
		/// </summary>
		public new CommonValidatorBase.DisplayType Display {
			get { return _display; }
			set { _display = value; }
		}
		#endregion

		#region Constructors
		#endregion

		#region Methods
		protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer) {
			base.AddAttributesToRender(writer);

			if (this.RenderUplevel) {
				string clientID = this.ClientID;
				Page.ClientScript.RegisterExpandoAttribute(clientID, "evaluationfunction", "RequiredFieldValidatorIsValid");
				Page.ClientScript.RegisterExpandoAttribute(clientID, "initialValue", (string.IsNullOrEmpty(this.InitialValue)) ? string.Empty : this.InitialValue);
				Page.ClientScript.RegisterExpandoAttribute(clientID, "displayErrorCallbackFunction", (string.IsNullOrEmpty(this.DisplayErrorCallbackFunction)) ? string.Empty : this.DisplayErrorCallbackFunction);
				Page.ClientScript.RegisterExpandoAttribute(clientID, "displayType", this.Display.ToString().ToLower());

				if (!string.IsNullOrEmpty(this.DisplayErrorCallbackFunction)) {
					// Set the display to None so that the ASP.NET display doesn't fire.
					this.Display = CommonValidatorBase.DisplayType.None;
				}
			}
		}

		protected override bool EvaluateIsValid() {
			string validationValue = GetControlValidationValue(ControlToValidate);
			return (!string.IsNullOrEmpty(validationValue));
		}

		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);

			if (this.RenderUplevel) {
				Bundler.RegisterFile("/Validators/ClientScripts/RequiredFieldValidator.js", this.GetType().Assembly);
				if (this.Display == CommonValidatorBase.DisplayType.Fade) {
					YUIHelper.Components.Animation();
				}
			}
		}
		#endregion
		
	}
}