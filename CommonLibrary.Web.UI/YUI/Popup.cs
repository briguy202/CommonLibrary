using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.ComponentModel;

namespace CommonLibrary.Web.UI.YUI {
	public class Popup : WebControl {

		#region Data Members
		private string _headerHTML;
		private string _bodyHTML;
		private string _footerHTML;
		private HtmlGenericControl _containerDiv;
		private ITemplate _contentTemplate;
		private int _popupWidth;
		private bool _isFixedCenter;
		private bool _isClosable;
		private bool _isDraggable;
		private int _zIndex;
		private bool _isModal;
		private bool _isPopupVisible;
		private string _clientScriptID;
		private bool _buildClientSide;
		private bool _isConstrainedToViewport;
		private int _popupHeight;
		#endregion

		#region Properties
		public string FooterHTML {
			get { return _footerHTML; }
			set { _footerHTML = value; }
		}
	
		public string BodyHTML {
			get { return _bodyHTML; }
			set { _bodyHTML = value; }
		}
	
		public string HeaderHTML {
			get { return _headerHTML; }
			set { _headerHTML = value; }
		}
	
		/// <summary>
		/// Gets or sets the id by which this element can be accessed through JavaScript.
		/// </summary>
		public string ClientScriptID {
			get { return _clientScriptID; }
			set { _clientScriptID = value; }
		}

		public bool IsPopupVisible {
			get { return _isPopupVisible; }
			set { _isPopupVisible = value; }
		}
	
		public bool IsModal {
			get { return _isModal; }
			set { _isModal = value; }
		}
	
		public int ZIndex {
			get { return _zIndex; }
			set { _zIndex = value; }
		}
	
		public bool IsDraggable {
			get { return _isDraggable; }
			set { _isDraggable = value; }
		}
	
		public bool IsClosable {
			get { return _isClosable; }
			set { _isClosable = value; }
		}
	
		/// <summary>
		/// Specifies whether the Overlay should be automatically centered in the viewport on window scroll and resize.
		/// </summary>
		public bool IsFixedCenter {
			get { return _isFixedCenter; }
			set { _isFixedCenter = value; }
		}

		/// <summary>
		/// Gets or sets the element's "width" style property.
		/// </summary>
		public int PopupWidth {
			get { return _popupWidth; }
			set { _popupWidth = value; }
		}

		/// <summary>
		/// Gets or sets the element's "height" style property.
		/// </summary>
		public int PopupHeight {
			get { return _popupHeight; }
			set { _popupHeight = value; }
		}

		/// <summary>
		/// If set to true the Overlay will try to remain inside the confines of the size of viewport.
		/// </summary>
		public bool IsConstrainedToViewport {
			get { return _isConstrainedToViewport; }
			set { _isConstrainedToViewport = value; }
		}

		public HtmlGenericControl ContainerDiv {
			get { return _containerDiv; }
			set { _containerDiv = value; }
		}

		[Browsable(false)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate ContentTemplate {
			get { return _contentTemplate; }
			set { _contentTemplate = value; }
		}
		#endregion

		#region Constructors
		public Popup() : base() {
			this.IsPopupVisible = false;
			this.IsDraggable = true;
			this.IsClosable = true;
			this.ZIndex = 20;
			this.IsModal = true;
			this.PopupWidth = 250;
			this.IsFixedCenter = true;
			this.IsConstrainedToViewport = true;
			_buildClientSide = true;
		}
		#endregion

		#region Methods
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			YUIHelper.Components.Panel();

			if (this.ContainerDiv != null) {
				// Build server-side
				_buildClientSide = false;
			} else if (this.ContentTemplate != null) {
				// Build server-side
				_buildClientSide = false;
				this.ContainerDiv = new HtmlGenericControl("div");
				this.Controls.Add(this.ContainerDiv);
				this.ContentTemplate.InstantiateIn(this.ContainerDiv);
			} else {
				// Build client-side
				this.ContainerDiv = new HtmlGenericControl("div");
				this.Controls.Add(this.ContainerDiv);
			}
			this.ContainerDiv.Style.Add(System.Web.UI.HtmlTextWriterStyle.Display, "none");

			this.RenderClientScript();
		}

		protected void RenderClientScript() {
			string id = this.ClientScriptID;
			if (string.IsNullOrEmpty(id)) {
				id = string.Format("pop_{0}", this.ClientID);
			}

			StringBuilder sb = new StringBuilder();
			if (!_buildClientSide) {
				sb.Append("YAHOO.util.Event.addListener(window, \"load\", function() { ");
			}
			sb.AppendFormat("{0} = new YAHOO.widget.Panel(\"{1}\", {{", id, this.ContainerDiv.ClientID);
			sb.AppendFormat("width:\"{0}px\"", this.PopupWidth.ToString());
			sb.AppendFormat(",fixedcenter:{0}", this.IsFixedCenter.ToString().ToLower());
			sb.AppendFormat(",close:{0}", this.IsClosable.ToString().ToLower());
			sb.AppendFormat(",draggable:{0}", this.IsDraggable.ToString().ToLower());
			sb.AppendFormat(",zindex:{0}", this.ZIndex.ToString());
			sb.AppendFormat(",modal:{0}", this.IsModal.ToString().ToLower());
			sb.AppendFormat(",visible:{0}", this.IsPopupVisible.ToString().ToLower());
			sb.AppendFormat(",constraintoviewport:{0}", this.IsConstrainedToViewport.ToString().ToLower());
			sb.Append("});");

			if (!string.IsNullOrEmpty(this.HeaderHTML)) {
				sb.AppendFormat("{0}.setHeader('{1}');", id, this.HeaderHTML);
			}

			if (!string.IsNullOrEmpty(this.BodyHTML)) {
				sb.AppendFormat("{0}.setBody('{1}');", id, this.BodyHTML);
			}

			if (!string.IsNullOrEmpty(this.FooterHTML)) {
				sb.AppendFormat("{0}.setFooter('{1}');", id, this.FooterHTML);
			}

			if (!_buildClientSide) {
				sb.AppendFormat("YAHOO.util.Dom.setStyle('{0}', 'display', '');", this.ContainerDiv.ClientID);
			}
			// If IsModal, append to the document.body so that no ancestor elements can get in front of the mask
			sb.AppendFormat("{0}.render({1});", id, (_buildClientSide || this.IsModal) ? "document.forms[0]" : string.Empty);
			if (!_buildClientSide) {
				sb.Append("});");
			}

			this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), this.ClientID, sb.ToString(), true);
		}
		#endregion
					
	}
}