using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using CommonLibrary.Web.UI.ResourceBundler;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using System.Collections;

namespace CommonLibrary.Web.UI.YUI {
	public class DataTable : WebControl {

		#region Data Members
		public enum DataSourceTypes {
			JSON,
			HtmlTable
		}
		private string _dataSourceValue;
		private DataSourceTypes _dataSourceType;
		private bool _enableCalendarEditor;
		private bool _enableColumnResizeReorder;
		private bool _includeDefaultCss;
		private Table _internalTable;
		private Collection<DataTableColumn> _columns;
		#endregion

		#region Properties
		public bool IncludeDefaultCss {
			get { return _includeDefaultCss; }
			set { _includeDefaultCss = value; }
		}

		public bool EnableColumnResizeReorder {
			get { return _enableColumnResizeReorder; }
			set { _enableColumnResizeReorder = value; }
		}

		public bool EnableCalendarEditor {
			get { return _enableCalendarEditor; }
			set { _enableCalendarEditor = value; }
		}

		public Collection<DataTableColumn> Columns {
			get { return _columns; }
			set { _columns = value; }
		}

		public string DataSourceValue {
			get { return _dataSourceValue; }
			set { _dataSourceValue = value; }
		}

		public DataSourceTypes DataSourceType {
			get { return _dataSourceType; }
			set { _dataSourceType = value; }
		}

		public Table InternalTable {
			get { return _internalTable; }
			set { _internalTable = value; }
		}
		#endregion

		#region Constructors
		public DataTable() : base() {
			this.Columns = new Collection<DataTableColumn>();
		}
		#endregion

		#region Methods
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			YUIHelper.Components.DataTable(this.EnableCalendarEditor, this.EnableColumnResizeReorder, this.IncludeDefaultCss);
			this.RenderClientScript();
		}

		protected void RenderClientScript() {
			if (this.InternalTable != null) {
				this.Controls.Add(this.InternalTable);
				this.DataSourceValue = this.InternalTable.ClientID;
			}

			string responseType = string.Empty;
			switch (this.DataSourceType) {
				case DataSourceTypes.JSON:
					responseType = "YAHOO.util.DataSource.TYPE_JSON";
					break;
				case DataSourceTypes.HtmlTable:
					responseType = "YAHOO.util.DataSource.TYPE_HTMLTABLE";
					break;
				default:
					break;
			}

			StringBuilder sb = new StringBuilder();
			string coldefsID = string.Format("coldefs_{0}", this.ClientID);

			sb.AppendLine("YAHOO.util.Event.addListener(window, \"load\", function() { ");
			sb.AppendFormat("var {0}=[", coldefsID);
			foreach (DataTableColumn column in this.Columns) {
				sb.AppendFormat("{{key:\"{0}\",label:\"{1}\",sortable:{2},formatter:\"{3}\"}},", column.Key, column.Label, column.Sortable.ToString().ToLower(), column.DataTypeValue.ToString().ToLower());
			}
			sb.Remove(sb.Length-1, 1);	// Strip off trailing comma.
			sb.Append("];");

			switch (this.DataSourceType) {
				case DataSourceTypes.JSON:
					sb.AppendFormat("var DS = new YAHOO.util.DataSource( {0} );", this.DataSourceValue);
					sb.Append("DS.responseSchema = {{ resultsList: \"value\", fields: [");
					foreach (DataTableColumn column in this.Columns) {
						sb.AppendFormat("{{key:\"{0}\", parser:\"{1}\"}},", column.Key, column.DataTypeValue.ToString().ToLower());
					}
					sb.Remove(sb.Length - 1, 1);	// Strip off trailing comma.
					sb.Append("] };");
					break;
				case DataSourceTypes.HtmlTable:
					sb.AppendFormat("var DS = new YAHOO.util.DataSource( YAHOO.util.Dom.get(\"{0}\") );", this.DataSourceValue);
					sb.Append("DS.responseSchema = { fields: [");
					foreach (DataTableColumn column in this.Columns) {
						sb.AppendFormat("{{key:\"{0}\", parser:\"{1}\"}},", column.Key, column.DataTypeValue.ToString().ToLower());
					}
					sb.Remove(sb.Length - 1, 1);	// Strip off trailing comma.
					sb.Append("] };");
					break;
				default:
					break;
			}
			
			sb.AppendFormat("DS.responseType = {0};", responseType);
			sb.AppendFormat("new YAHOO.widget.DataTable(\"{0}\", {1}, DS);", this.Parent.ClientID, coldefsID);

			sb.AppendLine("});");

			this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "script", sb.ToString(), true);
		}
		#endregion

	}
}