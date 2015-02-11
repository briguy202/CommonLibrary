using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Web.UI.Controls {
	public class TextBox : System.Web.UI.WebControls.TextBox {

		#region Properties
		private bool _renderClientVariable;

		public bool RenderClientVariable {
			get { return _renderClientVariable; }
			set { _renderClientVariable = value; }
		}
		#endregion

		#region Methods
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			if (this.RenderClientVariable) {
				((CommonLibrary.Web.UI.Page)this.Page).AddClientVariable(this.ID, this.ClientID, true);
			}
		}
		#endregion
	}
}