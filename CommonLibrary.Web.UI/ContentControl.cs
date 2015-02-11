using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using CommonLibrary.Web.UI.ResourceBundler;
using System.Web.UI.HtmlControls;
using System.Web.Security;
using CommonLibrary.Framework.Security;

namespace CommonLibrary.Web.UI {
	public class ContentControl : PlaceHolder {

		#region Data Members
		private int _contentID;
		#endregion

		#region Properties
		public new CommonLibrary.Web.UI.Page Page {
			get { return ((CommonLibrary.Web.UI.Page)base.Page); }
			set { base.Page = value; }
		}

		public int ContentID {
			get { return _contentID; }
			set { _contentID = value; }
		}
		#endregion

		#region Constructors
		public ContentControl() : base() {
			this.ContentID = int.MinValue;
		}
		#endregion

		#region Methods
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
		}

		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);

			if (this.ContentID > int.MinValue) {
				CommonLibrary.Business.Content content = CommonLibrary.Business.Content.GetContentByID(this.ContentID);
				if (content != null && !string.IsNullOrEmpty(content.Text)) {
					Literal ltrl = new Literal();
					ltrl.Text = string.Format("\n<!-- BEGIN CONTENT:{0} -->\n<span id='content_{0}'>{1}</span>\n<!-- END CONTENT:{0} -->", content.ID.ToString(), content.Text);

					if (Roles.IsUserInRole(RoleUtility.ContentEditor)) {
						StringBuilder sb = null;
						HtmlGenericControl div = new HtmlGenericControl("div");
						div.Style.Add("border", "3px dashed red");
						div.ID = string.Format("div_container_{0}", this.ContentID.ToString());
						div.Controls.Add(ltrl);
						this.Controls.Add(div);

						sb = new StringBuilder();
						sb.AppendLine();
						sb.AppendFormat("YAHOO.util.Event.addListener(\"{0}\", \"dblclick\", function(e){{", div.ClientID);
						sb.AppendFormat("GetEditor('{0}', 'content_{0}');", this.ContentID.ToString());
						sb.Append("});");
						sb.AppendLine();
						this.Page.RegisterStartupScript(sb.ToString());
					} else {
						this.Controls.Add(ltrl);
					}
				}
			}
		}

		[AjaxPro.AjaxMethod()]
		public void SaveContent(int id, string contentHTML) {
			CommonLibrary.Business.Content content = CommonLibrary.Business.Content.GetContentByID(id);
			if (content != null) {
				content.Text = contentHTML;
				content.Save();
			}
		}
		#endregion

	}
}